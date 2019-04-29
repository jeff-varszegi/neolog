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

using System.Runtime.CompilerServices;

namespace NeoLog.Formatting.Patterns.Tokens
{
    /// <summary>A token for entry levels</summary>
    internal sealed class LevelToken : Token
    {
        /// <summary>Static initializer</summary>
        static LevelToken()
        {
            TokenFactory.Default.Register(typeof(LevelToken), "{{level[^}]*}}");
        }

        /// <summary>Values to emit for different entry levels</summary>
        private string[] values;

        /// <summary>Default constructor</summary>
        private LevelToken() : base(null) { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The source text of this token</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LevelToken(string text) : base(text) {

            values = new string[33];

            values[0] = "";
            values[1] = "Trace";
            values[2] = "Debug";
            values[4] = "Info";
            values[8] = "Warning";
            values[16] = "Exception";
            values[32] = "Fatal";

            if (ParameterText.Contains("case=upper"))
            {
                for (int x = 1; x < values.Length; x++)
                    if (values[x] != null)
                        values[x] = values[x].ToUpper();
            }
            else if (ParameterText.Contains("case=lower"))
            {
                for (int x = 1; x < values.Length; x++)
                    if (values[x] != null)
                        values[x] = values[x].ToLower();
            }

            if (ParameterText.Contains("center=true"))
            {
                values[1] = values[1].PadLeft(7).PadRight(9);
                values[2] = values[2].PadLeft(7).PadRight(9);
                values[4] = values[4].PadLeft(6).PadRight(9);
                values[8] = values[8].PadLeft(8).PadRight(9);
                values[32] = values[32].PadLeft(7).PadRight(9);
            }
            else if (ParameterText.Contains("pad=true"))
            {
                for (int x = 1; x < values.Length; x++)
                    if (values[x] != null)
                        values[x] = values[x].PadRight(9);
            }
        }

        /// <summary>Formats an entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string Format(ref Entry entry)
        {
            return values[(int)entry.Level];
        }
    }
}
