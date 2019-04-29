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

namespace NeoLog.Formatting.Patterns
{
    /// <summary>A pattern token which emits a value for an entry</summary>
    public abstract class Token
    {
        /// <summary>Delimiters for parameter values embedded within token declarations</summary>
        private static char[] ParameterDelimiters = { ':', ' ' };

        /// <summary>Parameter text passed to the token function, or an empty string if none is found</summary>
        protected string ParameterText { get; private set; }

        /// <summary>The source text of this token</summary>
        protected string Text { get; private set; }

        /// <summary>Default constructor</summary>
        private Token() { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The source text of this token</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Token(string text) {
            Text = text ?? "";

            if (Text.StartsWith("{{"))
            {
                string innerText = Text.Trim('{', '}');
                int delimiterIndex = innerText.IndexOfAny(ParameterDelimiters);
                if (delimiterIndex > -1)
                    ParameterText = innerText.Substring(delimiterIndex + 1);
                else
                    ParameterText = "";
            }
            else
            {
                ParameterText = "";
            }
        }

        /// <summary>Generates text for this token type, relevant to the specified entry</summary>
        /// <param name="entry">The entry for which to generate token text</param>
        /// <returns>A string representation of this token for the specified entry</returns>
        public abstract string Format(ref Entry entry);
    }
}
