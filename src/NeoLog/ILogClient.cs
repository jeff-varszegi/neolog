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
    /// <summary>A marker interface which gives access to logging methods</summary>
    public interface ILogClient {}

    /// <summary>Extension methods for the ILogClient interface</summary>
    public static partial class ILogClientExtensions
    {
        /// <summary>Logs a message at the trace level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogTrace(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the debug level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogDebug(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the informational level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogInfo(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogWarning(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the warning level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="exception">The exception for this warning</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogWarning(this ILogClient context, Exception exception, string message = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogException(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the exception level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogException(this ILogClient context, Exception exception, string message = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogFatal(this ILogClient context, string message, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}

        /// <summary>Logs a message at the fatal level</summary>
        /// <param name="context">The context for the message</param>
        /// <param name="exception">The exception to write</param>
        /// <param name="message">The message to write</param>
        /// <param name="data">The data for the entry</param>
        /// <param name="tag">The tag for the entry</param>
        /// <param name="category">The category for the entry</param>
        /// <param name="user">The user for the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        public static void LogFatal(this ILogClient context, Exception exception, string message = null, object data = null, string tag = null, string category = null, string user = null, IDictionary<string, string> properties = null) {}
    }
}
