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
    /// <summary>A token for raw text</summary>
    internal sealed class DateToken : Token
    {
        /// <summary>Indicates whether to use the default format</summary>
        private bool useDefaultFormat;

        /// <summary>Static initializer</summary>
        static DateToken()
        {
            TokenFactory.Default.Register(typeof(DateToken), "{{date}}|{{date:[^}]*}}");
        }

        /// <summary>A date-time format to use</summary>
        private string format;

        /// <summary>Default constructor</summary>
        private DateToken() : base(null) { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The source text of this token</param>
        public DateToken(string text) : base(text)
        {
            if (string.IsNullOrWhiteSpace(ParameterText))
            {
                useDefaultFormat = true;
            }
            else
            {
                format = ParameterText;
                useDefaultFormat = false;
            }
        }

        /// <summary>Formats the specified date-time</summary>
        /// <param name="timestamp">The date-time to format</param>
        /// <returns>A string representation of the specified date-time</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string Format(ref DateTime timestamp)
        {
            return timestamp.ToString(format);
        }

        /// <summary>Formats a timestamp</summary>
        /// <param name="timestamp">The timestamp to format</param>
        /// <returns>A string representation of the specified timestamp</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string FormatTimestamp(ref DateTime timestamp)
        {
            char[] characters = new char[10];
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

            return new string(characters);
        }

        /// <summary>Formats an entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Format(ref Entry entry)
        {
            return useDefaultFormat ? FormatTimestamp(ref entry.Timestamp) : Format(ref entry.Timestamp);
        }
    }
}
