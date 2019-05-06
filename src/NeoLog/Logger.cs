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
using NeoLog.Extensions;
using NeoLog.Formatting.Patterns;
using NeoLog.Utility;

namespace NeoLog
{
    /// <summary>A base class for ILogger implementations</summary>
    public abstract class Logger : ILogger
    {
        #region Static members

        /// <summary>A minimum buffer-flush interval, in milliseconds</summary>
        private static TimeSpan MinimumBufferFlushInterval = new TimeSpan(1000000L);

        /// <summary>A maximum buffer-flush interval, in milliseconds</summary>
        private static TimeSpan MaximumBufferFlushInterval = new TimeSpan(1, 0, 0, 0);

        #endregion Static members

        #region Fields and properties

        /// <summary>The pattern to use in formatting entries for this logger</summary>
        private Pattern entryPattern;

        /// <summary>Whether buffering mode is enabled for this logger</summary>
        bool isBufferingEnabled;

        /// <summary>Whether, when in unbuffered mode, async calls will still be used</summary>
        bool isUnbufferedAsyncEnabled;

        /// <summary>Indicates if output has been raised when the logger is unstarted but an attempt is made to use it; this is done only once by design</summary>
        private bool isUnstartedExceptionRaised = false;

        /// <summary>Used in synchronization</summary>
        private object monitor = new object();

        /// <summary>Used in synchronizing access to the current buffer</summary>
        private object currentBufferMonitor = new object();

        /// <summary>The current buffer being written to, in buffering mode</summary>
        private EntryBuffer currentBuffer;

        /// <summary>Buffers waiting to be written to output</summary>
        private ConcurrentQueue<EntryBuffer> buffers = new ConcurrentQueue<EntryBuffer>();

        /// <summary>Filters to use in evaluating entries</summary>
        private IFilter[] filters = { };

        /// <summary>A logger to receive messages when this logger cannot write to output</summary>
        public ILogger BackupLogger { get; set; }

        #region Configuration

        /// <summary>The configuration used by this logger</summary>
        private LoggerConfiguration configuration;

        /// <summary>Gets a default configuration for this logger type, if any</summary>
        protected abstract LoggerConfiguration DefaultConfiguration { get; }

        /// <summary>The configuration used by this logger</summary>
        public LoggerConfiguration Configuration
        {
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

        #endregion Configuration

        #region Threading

        /// <summary>The thread allocated to drive this logger in buffering mode</summary>
        private Thread thread;

        /// <summary>The intervals between automatic buffer flushes</summary>
        private TimeSpan bufferFlushInterval;

        /// <summary>The current status of this logger</summary>
        public LoggerStatus Status { get; private set; } = LoggerStatus.Stopped;

        #endregion Threading

        #endregion Fields and properties

        #region Methods

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

        /// <summary>Raises the specified exception, which may involve throwing, writing to the backup logger, or doing nothing depending on the configuration</summary>
        /// <param name="message">The message of the exception to raise</param>
        /// <param name="exception">An optional exception object</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseException(string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                if (exception != null)
                {
                    message = exception.GetExtendedMessage();
                    exception = null;
                }
                else
                {
                    return;
                }
            }

            if (exception != null)
                message = message + " (" + exception.GetExtendedMessage() + ")";

            if ((bool)configuration?.IsExceptionThrowingEnabled)
            {
                throw new InvalidOperationException(message);

            }
            else
            {
                if (this.BackupLogger != null)
                {
                    Entry entry = new Entry(Level.Exception, Timekeeper.GetCurrentTimestamp(), message);
                }
            }
        }

        #region Life-cycle methods

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

            bufferFlushInterval = configuration.BufferFlushInterval;
            if (bufferFlushInterval < MinimumBufferFlushInterval)
                bufferFlushInterval = MinimumBufferFlushInterval;
            else if (bufferFlushInterval > MaximumBufferFlushInterval)
                bufferFlushInterval = MaximumBufferFlushInterval;
                                
            if (string.IsNullOrWhiteSpace(configuration.EntryFormat))
                throw new InvalidOperationException("Cannot start a logger without a defined entry format");
            else
                entryPattern = new Pattern(configuration.EntryFormat);
        }

        /// <summary>A hook for logger-specific configuration handling during initialization; called during Start()</summary>
        protected virtual void Configure() {}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start() {

            lock (monitor)
            {
                if (Status != LoggerStatus.Stopped) return;
                ProcessConfiguration();
                isUnstartedExceptionRaised = false;
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
                    Status = LoggerStatus.Stopped;
                    this.RaiseException("Exception while initializing logger " + this.GetType().FullName, e);
                    return;
                }
            }

            thread = new Thread(new ThreadStart(Process));
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();

            lock (monitor)
            {
                this.Status = LoggerStatus.Started;
            }
        }

        /// <summary>A hook to acquire  resources needed by this logger; called during Start()</summary>
        protected virtual void Initialize() { }

        /// <summary>When in buffered mode, repeatedly flushes the buffer until the logger is stopped</summary>
        private void Process() {
            while (Status == LoggerStatus.Started)
            {
                Thread.Sleep(bufferFlushInterval);
                try { Flush(); }
                catch (Exception e) {
                    try { BackupLogger.LogException(e); } catch { }
                }
            }
        }

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
                try { BackupLogger.LogException(e); } catch { }
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

        /// <summary>Performs an atomic buffer swap</summary>
        /// <returns>The old/pre-swap current buffer, or null if left in place or nonexistent</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EntryBuffer SwapBuffer()
        {
            if (currentBuffer?.Count == 0) return null;
            
            EntryBuffer newBuffer = EntryBuffer.GetBuffer();
            EntryBuffer oldBuffer = Interlocked.Exchange(ref this.currentBuffer, newBuffer);
            return newBuffer;
        }

        /// <summary>Flushes buffered output</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush() {
            // TODO Error handling, retry logic
            lock (monitor)
            {
                try
                {
                    while (buffers.TryDequeue(out EntryBuffer buffer))
                    {
                        if (buffer.Count > 0)
                            Write(buffer);
                        buffer.Release();
                    }

                    EntryBuffer lastBuffer = SwapBuffer();
                    if (lastBuffer != null)
                    {
                        if (lastBuffer.Count > 0)
                            Write(lastBuffer);
                        lastBuffer.Release();
                    }
                }
                catch (Exception e)
                {
                    RaiseException("Exception while flushing buffer for " + this.GetType().FullName);
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

        #region Log methods

        #region Backup logging

        /// <summary>Writes the contents of the specified entry buffer, without clearing it, to the backup logger</summary>
        /// <param name="buffer">The entries to write</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBackup(EntryBuffer buffer)
        {
            if (this.BackupLogger != null)
            {
                try
                {
                    Logger backupLogger = (Logger)this.BackupLogger; // TODO eliminate cast
                    backupLogger.Write(buffer);
                }
                catch { }
            }
        }

        /// <summary>Writes the specified entry to the backup logger</summary>
        /// <param name="entry">The entry to write</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteBackup(ref Entry entry)
        {
            if (this.BackupLogger != null)
            {
                try
                {
                    Logger backupLogger = (Logger)this.BackupLogger; // TODO eliminate cast
                    backupLogger.Write(ref entry);
                }
                catch { }
            }
        }

        #endregion Backup logging

        #region Entry preparation

        /// <summary>Formats the specified entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string FormatEntry(ref Entry entry)
        {
            return entryPattern.Format(ref entry);
        }

        #endregion Entry preparation

        /// <summary>Indicates whether this logger's filters allow this entry to be written. Called from Write()</summary>
        /// <returns>A result indicating whether the specified entry should be written</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ShouldWrite(ref Entry entry)
        {
            FilterResult result;
            for (int x = 0; x < filters.Length; x++)
            {
                result = filters[x].Evaluate(ref entry);
                switch (result)
                {
                    case FilterResult.Exclude: return false;
                    case FilterResult.Include: return true;
                    case FilterResult.Pass: break;
                }
            }

            return true;
        }

        /// <summary>Gets a buffer which can receive input</summary>
        /// <returns>An entry buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EntryBuffer GetBuffer()
        {
            EntryBuffer buffer = currentBuffer;
            if (buffer.Count < buffer.Length)
            {
                return buffer;
            }
            else
            {
                EntryBuffer nextBuffer = EntryBuffer.GetBuffer();

                lock (currentBufferMonitor)
                {
                    buffer = currentBuffer;
                    if (buffer.Count >= buffer.Length) 
                    {
                        currentBuffer = nextBuffer;
                        buffers.Enqueue(buffer);
                        return currentBuffer;
                    }
                }

                // If we have gotten this far, the buffers were already switched
                Task.Run(() => nextBuffer.Release()); // allows work to be queued async without large-scale buffering
                return currentBuffer;
            }
        }

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
                if ((bool)configuration?.IsAutoStartEnabled)
                {
                    Start();
                }
                else if (!isUnstartedExceptionRaised)
                {
                    isUnstartedExceptionRaised = true;
                    RaiseException("The logger is not started--cannot write entry");
                    return;
                }
            }

            if (isBufferingEnabled)
            {
                EntryBuffer buffer;
                DateTime timestamp = Timekeeper.GetCurrentTimestamp();
                int threadId = 0; // TODO
                while ((buffer = this.GetBuffer()) != null)
                {
                    // The only reason adding an entry should fail is if the buffer is full, i.e. a swap is needed
                    if (buffer.AddEntry(level, timestamp, message, exception, context, data, tag, category, user, threadId, properties) ||
                        Status != LoggerStatus.Started) // i.e. the logger has been stopped in the meantime
                        return;
                }
            }
            else
            {
                Entry entry = new Entry(level, Timekeeper.GetCurrentTimestamp(), message, exception, context, data, tag, category, user, 0, properties);

                if (isUnbufferedAsyncEnabled)
                {
                    Task.Run(() => Write(ref entry)); // allows work to be queued async without large-scale buffering
                }
                else
                {
                    Write(ref entry);

                    /*
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
                    */
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

        #endregion Methods
    }
}
