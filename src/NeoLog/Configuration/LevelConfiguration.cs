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

namespace NeoLog.Configuration
{
    public class LevelConfiguration
    {
        private static LevelConfiguration NoneDefault = new LevelConfiguration(Level.Trace)
        {
            IsThreadIdEnabled = false
        };

        private static LevelConfiguration TraceDefault = new LevelConfiguration(Level.Trace)
        {
            IsThreadIdEnabled = false
        };

        private static LevelConfiguration DebugDefault = new LevelConfiguration(Level.Debug)
        {
            IsThreadIdEnabled = true
        };

        private static LevelConfiguration InfoDefault = new LevelConfiguration(Level.Info)
        {
            IsThreadIdEnabled = false
        };

        private static LevelConfiguration WarningDefault = new LevelConfiguration(Level.Warning)
        {
            IsThreadIdEnabled = false
        };

        private static LevelConfiguration ExceptionDefault = new LevelConfiguration(Level.Exception)
        {
            IsThreadIdEnabled = false
        };

        private static LevelConfiguration FatalDefault = new LevelConfiguration(Level.Fatal)
        {
            IsThreadIdEnabled = true
        };

        public static LevelConfiguration None { get { return NoneDefault; } }
        public static LevelConfiguration Trace { get { return TraceDefault; } }
        public static LevelConfiguration Debug { get { return DebugDefault; } }
        public static LevelConfiguration Info { get { return InfoDefault; } }
        public static LevelConfiguration Warning { get { return WarningDefault; } }
        public static LevelConfiguration Exception { get { return ExceptionDefault; } }
        public static LevelConfiguration Fatal { get { return FatalDefault; } }

        public Level Level { get; private set; }

        public bool IsCategoryDelimiterEnabled { get; set; }
        public char CategoryDelimiter { get; set; } = '/';

        public bool IsThreadIdEnabled { get; set; }

        public LevelConfiguration Default
        {
            get
            {
                switch (this.Level)
                {
                    case Level.None: return NoneDefault;
                    case Level.Trace: return TraceDefault;
                    case Level.Debug: return DebugDefault;
                    case Level.Info: return InfoDefault;
                    case Level.Warning: return WarningDefault;
                    case Level.Exception: return ExceptionDefault;
                    case Level.Fatal: return FatalDefault;
                    default: return NoneDefault;
                }
            }
        }

        private LevelConfiguration() { }

        public LevelConfiguration(Level level)
        {
            this.Level = level;
        }
    }
}
