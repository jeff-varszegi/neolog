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
using System.Collections.Generic;

namespace NeoLog
{
    /// <summary>A log entry, which stores a message together with its timestamp and other important information</summary>
    public struct Entry
    {
        /// <summary>The level of this entry</summary>
        public Level Level;

        /// <summary>The timestamp of this entry</summary>
        public DateTime Timestamp;

        /// <summary>The message of this entry</summary>
        public string Message;

        /// <summary>The exception of this entry, if any</summary>
        public Exception Exception;

        /// <summary>The context of this entry</summary>
        public string Context;

        /// <summary>The data of this entry</summary>
        public object Data;

        /// <summary>The tag(s) of this entry</summary>
        public string Tag;

        /// <summary>The category (or delimited categories) of this entry</summary>
        public string Category;

        /// <summary>The identity, e.g. user, for the entry</summary>
        public string Identity;

        /// <summary>The thread ID of this entry (defaults to 0 if not set)</summary>
        public int ThreadId;

        /// <summary>Extensible key/value pairs for this entry</summary>
        public IDictionary<string, string> Properties;

        /*
        /// <summary>Constructs an entry</summary>
        /// <param name="level">The log level of the entry</param>
        /// <param name="timestamp">The timestamp of the entry</param>
        /// <param name="message">The message of the entry</param>
        /// <param name="exception">The exception of the entry</param>
        /// <param name="context">The context of the entry</param>
        /// <param name="data">The data of the entry</param>
        /// <param name="tag">The tag(s) of the entry</param>
        /// <param name="category">The category/categories of the entry</param>
        /// <param name="threadId">The thread ID of the entry</param>
        public EntryStruct(Level level, DateTime timestamp, string message, Exception exception, string context, object data, string tag, string category, int threadId)
        {
            this.Level = level;
            this.Timestamp = timestamp;
            this.Message = message;
            this.Exception = exception;
            this.Context = context;
            this.Data = data;
            this.Tag = tag;
            this.Category = category;
            this.ThreadId = threadId;
        }
        */
    }
}
