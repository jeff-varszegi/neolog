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
using System.Text.RegularExpressions;

using NeoLog.Extensions;
using NeoLog.Formatting.Patterns.Tokens;

namespace NeoLog.Formatting.Patterns
{
    /// <summary>Formats entries using on a token-based pattern language. This type is thread-safe.</summary>
    public sealed class Pattern
    {
        /// <summary>A regular expression used in tokenization</summary>
        private static Regex PatternRegex = new Regex(@"{{[a-zA-Z0-9\.]*[^}]*}}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Tokens to use in formatting messages</summary>
        private Token[] tokens;

        /// <summary>The length to use in iniitalizing a new StringBuilder for formatting an entry</summary>
        private int startingLength;

        /// <summary>Default constructor</summary>
        private Pattern() { }

        /// <summary>Constructs a new pattern</summary>
        /// <param name="text">Pattern text</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pattern(string text)
        {
            text = text ?? "";
            IList<string> tokenStrings = PatternRegex.Tokenize(text);
            startingLength = text.Length + (tokenStrings.Count * 25);

            List<Token> tokenList = new List<Token>(tokenStrings.Count);
            for (int x = 0; x < tokenStrings.Count; x++)
                tokenList.Add(TokenFactory.Default.CreateToken(tokenStrings[x]));
            tokens = tokenList.ToArray();
        }

        /// <summary>Formats the specified entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Format(ref Entry entry)
        {
            StringBuilder sb = new StringBuilder(startingLength);

            for (int x = 0; x < tokens.Length; x++)
                sb.Append(tokens[x].Format(ref entry));
           
            return sb.ToString();
        }
    }
}
