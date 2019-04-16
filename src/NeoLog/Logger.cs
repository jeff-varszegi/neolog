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
using NeoLog.Configuration;

namespace NeoLog
{
    /// <summary>A base class for ILogger implementations</summary>
    public abstract class Logger : ILogger
    {
        /// <summary></summary>
        public LoggerConfiguration Configuration { get; set; }

        /// <summary>A logger to receive messages when this logger cannot write to output</summary>
        public ILogger BackupLogger { get; set; }

        /// <summary>Logs a message at the trace level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogTrace(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the debug level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogDebug(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the informational level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogInfo(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogWarning(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="exception">The exception for this warning</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogWarning(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogException(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogException(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogFatal(string message, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="context">The context for the message</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="identity">The identity making the entry</param>
        public void LogFatal(Exception exception, string message = null, object context = null, object data = null, string tag = null, string category = null, string identity = null) { }

        /// <summary>Writes the contents of the specified entry buffer, without clearing it</summary>
        /// <param name="buffer">The entries to write</param>
        protected abstract void Write(EntryBuffer buffer);

        /// <summary>Writes the specified entry to output</summary>
        /// <param name="entry">The entry to write</param>
        protected abstract void Write(ref Entry entry);
    }
}
