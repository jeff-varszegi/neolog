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
using System.Linq;
using System.Text;

namespace NeoLog.Configuration
{
    /// <summary>The overall logging configuration for an entire system</summary>
    public class LoggingConfiguration
    {
        /// <summary>A default/global configuration</summary>        
        private static LoggingConfiguration DefaultValue = new LoggingConfiguration();

        /// <summary>A default/global configuration</summary>        
        public static LoggingConfiguration Default
        {
            get { return DefaultValue; }
            set { if (value != null) DefaultValue = value; }
        }

        /// <summary>Used in synchronization</summary>
        private object monitor = new object();

        /// <summary>Logger configurations which comprise this overall configuration</summary>
        private List<LoggerConfiguration> loggerConfigurations = new List<LoggerConfiguration>();

        /// <summary>The logger for exceptions</summary>
        public Logger ExceptionLogger { get; set; } // TODO

        /// <summary>Gets l
        /// ogger configurations which comprise this overall configuration</summary>
        public IList<LoggerConfiguration> LoggerConfigurations
        {
            get
            {
                return loggerConfigurations.ToList();
            }
        }

        /// <summary>Indicates whether exception throwing is enabled, or exceptions will be silently logged or suppressed</summary>
#if DEBUG
        public bool IsExceptionThrowingEnabled { get; set; } = true;
#else
        public bool IsExceptionThrowingEnabled { get; set; } = false;
#endif

        /// <summary>Adds a logger configuration</summary>
        /// <param name="loggerConfiguration">The configuration to add</param>
        public void Add(LoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration == null) return;

            lock (monitor)
            {
                if (loggerConfigurations.Contains(loggerConfiguration)) return;
                List<LoggerConfiguration> newList = loggerConfigurations.ToList();
                newList.Add(loggerConfiguration);
                loggerConfigurations = newList;
            }
        }
    }
}
