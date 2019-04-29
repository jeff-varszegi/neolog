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
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLog.Formatting.Patterns.Tokens
{
    /// <summary>A token for ISO 8601 timestamps, down to the millisecond</summary>
    internal sealed class TimestampToken : Token
    {
        /// <summary>Static initializer</summary>
        static TimestampToken()
        {
            TokenFactory.Default.Register(typeof(TimestampToken), "{{timestamp}}");
        }

        /// <summary>Default constructor</summary>
        private TimestampToken() : base(null) { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The source text of this token</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimestampToken(string text) : base(text) { }

        /// <summary>Formats a timestamp</summary>
        /// <param name="timestamp">The timestamp to format</param>
        /// <returns>A string representation of the specified timestamp</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatTimestamp(ref DateTime timestamp)
        {
            char[] characters = new char[24];
            int i;

            i = timestamp.Year;
            characters[0] = (char)((i / 1000) + 48);
            characters[1] = (char)(((i % 1000) / 100) + 48);
            characters[2] = (char)(((i % 100) / 10) + 48);
            characters[3] = (char)((i % 10) + 48);
            //
            characters[4] = '-';
            //
            i = timestamp.Month;
            characters[5] = (char)((i / 10) + 48);
            characters[6] = (char)((i % 10) + 48);
            //
            characters[7] = '-';
            //
            i = timestamp.Day;
            characters[8] = (char)((i / 10) + 48);
            characters[9] = (char)((i % 10) + 48);
            //
            characters[10] = 'T';
            //
            i = timestamp.Hour;
            characters[11] = (char)((i / 10) + 48);
            characters[12] = (char)((i % 10) + 48);
            //
            characters[13] = ':';
            //
            i = timestamp.Minute;
            characters[14] = (char)((i / 10) + 48);
            characters[15] = (char)((i % 10) + 48);
            //
            characters[16] = ':';
            //
            i = timestamp.Second;
            characters[17] = (char)((i / 10) + 48);
            characters[18] = (char)((i % 10) + 48);
            //
            characters[19] = '.';
            //
            i = timestamp.Millisecond;
            characters[20] = (char)((i / 100) + 48);
            characters[21] = (char)(((i % 100) / 10) + 48);
            characters[22] = (char)((i % 10) + 48);
            //
            characters[23] = 'Z';

            return new string(characters);
        }

        /// <summary>Formats an entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Format(ref Entry entry)
        {
            return FormatTimestamp(ref entry.Timestamp);
        }
    }
}
