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
using System.Runtime.CompilerServices;

using NeoLog.Configuration;

namespace NeoLog.Loggers
{
    /// <summary>A logger which writes </summary>
    public sealed class ConsoleLogger : Logger
    {
        /// <summary></summary>
        private const string DefaultEntryFormat = "{{timestamp}} {{level case=upper center=true}} {{message}}";

        /// <summary>A reusable configuration</summary>
        private static LoggerConfiguration StaticConfiguration = new LoggerConfiguration()
        {
            IsBufferingEnabled = false,
            IsUnbufferedAsyncEnabled = true,
            EntryFormat = DefaultEntryFormat
        };

        /// <summary>A default configuration for this logger type</summary>
        protected override LoggerConfiguration DefaultConfiguration
        {
            get
            {
                return StaticConfiguration.Copy();
            }
        }

        /// <summary>Acquires resources needed by this logger</summary>
        protected override void Initialize()
        {

        }

        /// <summary>Writes entries in the specified buffer to the console</summary>
        /// <param name="buffer">The log entries to write</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Write(EntryBuffer buffer)
        {
            for (int x = 0; x < buffer.Count; x++)
                try { Write(ref buffer.Entries[x]); } catch { }
        }

        /// <summary>Writes the specified entry to the console</summary>
        /// <param name="entry">The entry to write</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Write(ref Entry entry)
        {
            Console.WriteLine(FormatEntry(ref entry));
        }
    }
}
