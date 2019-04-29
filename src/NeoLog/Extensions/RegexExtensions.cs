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

namespace NeoLog.Extensions
{
    /// <summary>Provides extension/utility logic for Regex objects</summary>
    internal static class RegexExtensions
    {
        /// <summary>Splits the specified string based on the pattern of this regular expression, returning the text of all matches and non-matches in order</summary>
        /// <param name="regex">this regex</param>
        /// <param name="text">The text to tokenize</param>
        /// <returns>A list of tokens in source order</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<string> Tokenize(this Regex regex, string text)
        {
            if (regex == null || string.IsNullOrEmpty(text)) return new List<string>();

            List<string> tokens = new List<string>();
            int index = 0;
            foreach (Match match in regex.Matches(text))
            {
                if (match.Index > index)
                    tokens.Add(text.Substring(index, match.Index - index));
                tokens.Add(match.Value);
                index = match.Index + match.Length;
            }

            if (index < text.Length)
                tokens.Add(text.Substring(index, text.Length - index));

            return tokens;
        }
    }
}
