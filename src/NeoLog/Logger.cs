/***********************************************************************************************************************
*  MIT License: NeoLog, A High-Performance Logging System. (https://github.com/NeoLog)                                 *
*  Copyright (c) 2019 Jeffrey Varszegi                                                                                 *
*                                                                                                                      *
*  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated        *
*  documentation files (the "Software"), to deal in the Software without restriction, including without limitation     *
*  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and    *
*  to permit persons to whom the Software is furnished to do so, subject to the following conditions:                  *
*                                                                                                                      *
*  The above copyright notice and this permission notice shall be included in all copies or substantial portions of    *
*  the Software.                                                                                                       *
*                                                                                                                      *
*  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO    *
*  THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE      *
*  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF           *
*  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS   *
*  IN THE SOFTWARE.                                                                                                    *
***********************************************************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NeoLog.Configuration;
using NeoLog.Formatting.Patterns;
using NeoLog.Utility;

namespace NeoLog
{
    /// <summary>A base class for ILogger implementations</summary>
    public abstract class Logger : ILogger
    {
        /// <summary>Used in synchronization</summary>
        private object monitor = new object();

        /// <summary>Used in synchronization</summary>
        private object flushMonitor = new object();

        /// <summary>The current buffer being written to, in buffering mode</summary>
        private EntryBuffer currentBuffer;
        
        private int entryCount = 0;

        private ConcurrentQueue<EntryBuffer> buffers = new ConcurrentQueue<EntryBuffer>();

        /// <summary>Filters to use in evaluating entries</summary>
        private IFilter[] filters = { };

        /// <summary>Adds an entry filter</summary>
        /// <param name="filter">The filter to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFilter(IFilter filter)
        {
            if (filter == null) return;
            lock (monitor)
            {
                if (Status != LoggerStatus.Stopped) throw new InvalidOperationException("Can only add filters to a stopped logger");
                if (!filters.Contains(filter))
                {
                    IList<IFilter> list = filters.ToList();
                    list.Add(filter);
                    filters = list.ToArray();
                }
            }
        }

        /// <summary>Indicates whether the specified entry is filtered out, i.e. excluded by one or more filters</summary>
        /// <param name="entry">The entry to test</param>
        /// <returns>true if the entry is filtered out, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsEntryExcluded(ref Entry entry)
        {
            for (int x = 0; x < filters.Length; x++)
                if (filters[x].Excludes(ref entry))
                    return true;

            return false;
        }

        /// <summary>Formats the specified entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string FormatEntry(ref Entry entry)
        {
            return entryPattern.Format(ref entry);
        }

        #region Life-cycle methods

        /// <summary>The pattern to use in formatting entries for this logger</summary>
        private Pattern entryPattern;

        /// <summary>Whether buffering mode is enabled for this logger</summary>
        bool isBufferingEnabled;

        /// <summary>Whether, when in unbuffered mode, async calls will still be used</summary>
        bool isUnbufferedAsyncEnabled;

        /// <summary>Handle pre-start initialization steps based on the configuration</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessConfiguration()
        {
            if (configuration == null)
                configuration = DefaultConfiguration.Copy();
            if (configuration == null)
                throw new InvalidOperationException("Cannot start a logger without a configuration");

            isBufferingEnabled = configuration.IsBufferingEnabled;
            isUnbufferedAsyncEnabled = configuration.IsUnbufferedAsyncEnabled;

            if (string.IsNullOrWhiteSpace(configuration.EntryFormat))
                throw new InvalidOperationException("Cannot start a logger without a defined entry format");
            else
                entryPattern = new Pattern(configuration.EntryFormat);


        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start() {

            lock (monitor)
            {
                if (Status != LoggerStatus.Stopped) return;
                ProcessConfiguration();
                Status = LoggerStatus.Starting;
            }

            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                lock (monitor)
                {
                    Status = LoggerStatus.Error;
                    try { BackupLogger.LogException(e, "Exception while initializing logger " + this.GetType().FullName); } catch { }
                    return;
                }
            }

            lock (monitor)
            {
                this.Status = LoggerStatus.Started;
            }
        }

        /// <summary>Acquires resources needed by this logger</summary>
        protected virtual void Initialize() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Stop() {
            lock (monitor)
            {
                if (Status != LoggerStatus.Started) return;
                Status = LoggerStatus.Stopping;
            }

            try
            {
                Flush();
            }
            catch (Exception e)
            {
                lock (monitor)
                {
                    try { BackupLogger.LogException(e, "Exception while flushing logger " + this.GetType().FullName); } catch { }
                }
            }

            try
            {
                Terminate();
            }
            catch (Exception e)
            {
                lock (monitor)
                {
                    try { BackupLogger.LogException(e, "Exception while terminating logger " + this.GetType().FullName); } catch { }
                }
            }

            lock (monitor)
            {
                this.Status = LoggerStatus.Stopped;
            }
        }

        /// <summary>Releases resources held by this logger</summary>
        protected virtual void Terminate() { }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EntryBuffer SwapBuffer()
        {
            throw new NotImplementedException();
        }

        /// <summary>Flushes buffered output</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush() {
            // TODO Error handling, retry logic
            lock (monitor)
            {
                try
                {
                    EntryBuffer lastBuffer = SwapBuffer();
                    while (buffers.TryDequeue(out EntryBuffer buffer))
                    {
                        if (buffer.Count > 0)
                            Write(buffer);
                        buffer.Release();
                    }

                    if (lastBuffer.Count > 0)
                        Write(lastBuffer);
                    lastBuffer.Release();
                }
                catch (Exception e)
                {
                    try { BackupLogger.LogException(e, "Exception while flushing buffer for " + this.GetType().FullName); } catch { }
//                    Status = LoggerStatus.Error;
//                    Stop();
                    return;
                }
            }
        }

        /// <summary>Disposes of this logger</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            try { Stop(); } catch { }
        }

        #endregion Life cycle methods

        /// <summary>The configuration used by this logger</summary>
        private LoggerConfiguration configuration;

        /// <summary>Gets a default configuration for this logger type, if any</summary>
        protected abstract LoggerConfiguration DefaultConfiguration { get; }

        /// <summary>The configuration used by this logger</summary>
        public LoggerConfiguration Configuration {
            get
            {
                return configuration ?? DefaultConfiguration;
            }
            set
            {
                if (value == null) throw new InvalidOperationException("Cannot set a null configuration");
                lock (monitor)
                {
                    if (Status != LoggerStatus.Stopped) throw new InvalidOperationException("Can only set a configuration on a stopped logger");
                    configuration = value;
                }
            }
        }

        /// <summary>The current status of this logger</summary>
        public LoggerStatus Status { get; private set; } = LoggerStatus.Stopped;

        /// <summary>A logger to receive messages when this logger cannot write to output</summary>
        public ILogger BackupLogger { get; set; }

        #region Log methods

        /// <summary>Adds a log entry</summary>
        /// <param name="level">The level of the entry</param>
        /// <param name="exception">The exception for this warning</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddEntry(Level level, Exception exception = null, string message = null, string context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            if (Status != LoggerStatus.Started)
            {
                if (configuration == null || configuration.IsExceptionThrowingEnabled)
                    throw new InvalidOperationException("The logger is not started--cannot write entry");
                else
                    return;
            }

            if (isBufferingEnabled)
            {

            }
            else
            {
                Entry entry = new Entry(level, Timekeeper.GetCurrentTimestamp(), message, exception, context, data, tag, Category.None, user, 0, properties);
                if (IsEntryExcluded(ref entry)) return;

                if (isUnbufferedAsyncEnabled)
                {
                    Task.Run(() => Write(ref entry));
                }
                else
                {
                    try
                    {
                        Write(ref entry);
                    } catch (Exception e)
                    {
                        if (this.BackupLogger != null)
                        {
                            try
                            {
                                this.BackupLogger.LogException(e, "Cannot log message", this);
                            } catch { }
                        }
                    }
                }
            }
        }

        /// <summary>Logs a message at the trace level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogTrace(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Trace, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the debug level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogDebug(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Debug, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the informational level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogInfo(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Info, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogWarning(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Warning, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="exception">The exception for this warning</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogWarning(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Warning, exception, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogException(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Exception, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogException(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Exception, exception, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogFatal(string message, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Fatal, null, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogFatal(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null)
        {
            AddEntry(Level.Fatal, exception, message, context.ConvertToString(), data, tag, category, user, properties);
        }

        #region Output methods

        /// <summary>Writes the contents of the specified entry buffer, without clearing it</summary>
        /// <param name="buffer">The entries to write</param>
        protected abstract void Write(EntryBuffer buffer);

        /// <summary>Writes the specified entry to output</summary>
        /// <param name="entry">The entry to write</param>
        protected abstract void Write(ref Entry entry);

        #endregion Output methods

        #endregion Log methods
    }
}
