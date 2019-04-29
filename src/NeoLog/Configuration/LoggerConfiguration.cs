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

using NeoLog.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLog.Configuration
{
    public class LoggerConfiguration
    {
        private const string Iso8601Format = "yyyy'-'dd'-'MM'T'HH':'mm':'ss'.'fffZ";

        private static TimeSpan MinimumBufferFlushInterval = new TimeSpan(0, 0, 1);

        private LoggerConfiguration parent;
        private List<LoggerConfiguration> children = new List<LoggerConfiguration>();

        public string LoggerName { get; set; }
        public Type LoggerType { get; set; }

        public LoggerConfiguration Parent
        {
            get { return parent; }
            set
            {
                lock (this)
                {
                    if (value == null)
                    {
                        if (parent != null)
                            lock (parent)
                                parent.children.Remove(this);
                        parent = null;
                        return;
                    }

                    LoggerConfiguration ancestor = value;
                    while (ancestor != null)
                        if (ancestor == this)
                            return;
                        else
                            ancestor = ancestor.Parent;

                    lock (value)
                    {
                        value.children.Add(this);
                        this.parent = value;
                    }
                }
            }
        }

        public IEnumerable<LoggerConfiguration> Children {
            get
            {
                lock (this)
                    return children.ToList();
            }
        }

        /// <summary>When buffering mode is disabled, this indicates whether logging calls will be asynchronously dispatched (defaults to false)</summary>
        public bool IsUnbufferedAsyncEnabled { get; set; } = false;

        /// <summary>Whether log buffering is enabled for this logger (defaults to true)</summary>
        public bool IsBufferingEnabled { get; set; } = true;

        private TimeSpan bufferFlushInterval = new TimeSpan(0, 0, 5);

        public TimeSpan BufferFlushInterval
        {
            get { return bufferFlushInterval; }
            set
            {
                bufferFlushInterval = value <= MinimumBufferFlushInterval ? MinimumBufferFlushInterval : value;
            }
        }

        private int maximumBufferCount = int.MaxValue;

        public int MaximumBufferCount
        {
            get { return maximumBufferCount; }
            set
            {
                maximumBufferCount = value <= 0 ? int.MaxValue : value; ;
            }
        }

        public Level LogLevelDefault { get; set; } = Level.Info;

        private string logPrefix = "";
        public string LogPrefix
        {
            get { return logPrefix; }
            set
            {
                logPrefix = value ?? "";
            }
        }

        private string dateTimeFormat = Iso8601Format;

        public string DateTimeFormat {
            get { return dateTimeFormat; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    dateTimeFormat = Iso8601Format;
                else
                    dateTimeFormat = value.Trim();
            }
        }

        public bool IsExceptionThrowingEnabled { get; set; } = true;
        public bool IsExceptionCompactFormatEnabled { get; set; }
        public int ExceptionCompactFormatDepth { get; set; } = 1;

        private const string DefaultEntryFormat = "{{timestamp}} [{{level}}] {{message}}";

        private string entryFormat = DefaultEntryFormat;
        public string EntryFormat
        {
            get
            {
                return entryFormat;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.IndexOf("{{") < 0)
                    entryFormat = DefaultEntryFormat;
                else
                    entryFormat = value;
            }
        }

        public string TokenSeparator { get; set; } = " ";

        public string LevelPrefix { get; set; } = "[";
        public string LevelSuffix { get; set; } = "]";
        public Case LevelCase { get; set; } = Case.Upper;
        public int LevelPadWidth { get; set; } = 9;
        public int LevelMaxWidth { get; set; } = 1;

        public string ContextPrefix { get; set; } = "";
        public string ContextSuffix { get; set; } = "";

        public string DataPrefix { get; set; } = "";
        public string DataSuffix { get; set; } = "";

        public string TagPrefix { get; set; } = "#";
        public string TagDelimiter { get; set; } = " ";
        public string TagsPrefix { get; set; } = "";
        public string TagsSuffix { get; set; } = "";

        public string CategoryPrefix { get; set; } = "#";
        public string CategoryDelimiter { get; set; } = " ";
        public string CategoriesPrefix { get; set; } = "";
        public string CategoriesSuffix { get; set; } = "";

        public bool IsThreadTrackingEnabled { get; set; }
        // TODO properties per thread?

        public bool IsContextTrackingEnabled { get; set; }

        public bool IsRepetitionFilteringEnabled { get; set; }

        public bool IsEncryptionEnabled { get; set; }
        public bool IsIdentityEncryptionEnabled { get; set; }

        public bool IsRelativeStackTraceEnabled { get; set; }

        public bool IsAutoStartEnabled { get; set; } = true;

        public LoggerConfiguration() { }

        public LoggerConfiguration Copy()
        {
            return (LoggerConfiguration)this.MemberwiseClone();
        }
    }
}
