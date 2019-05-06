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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NeoLog.Filters
{
    /// <summary>Filters entries based on regular expressions</summary>
    public sealed class RegularExpressionFilter : IFilter
    {
        // TODO inclusive / exclusive

        /// <summary>Regular expression options to use in case-sensitive mode</summary>
        private const RegexOptions CaseSensitiveOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;

        /// <summary>Regular expression options to use in case-insensitive mode</summary>
        private const RegexOptions CaseInsensitiveOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        /// <summary>The regular expressions to test</summary>
        private Regex[] regexes;

        /// <summary>The boolean operator to use when evaluating multiple regular expressions</summary>
        private BooleanOperator booleanOperator;

        /// <summary>Default constructor</summary>
        private RegularExpressionFilter() { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="patterns">The regular expression patterns to use</param>
        /// <param name="caseInsensitive">If true (the default), matching will be case-insensitive, otherwise case-sensitive</param>
        /// <param name="op">Whether to use AND or OR logic when matching against multiple patterns</param>
        public RegularExpressionFilter(IEnumerable<string> patterns, bool caseInsensitive = true, BooleanOperator op = BooleanOperator.Or)
        {
            if (patterns == null) throw new ArgumentNullException();
            List<Regex> list = new List<Regex>(5);
            Regex regex;
            foreach (string pattern in patterns.Distinct())
            {
                try
                {
                    regex = new Regex(pattern, caseInsensitive ? CaseInsensitiveOptions : CaseSensitiveOptions);
                    list.Add(regex);
                }
                catch { }
            }

            regexes = list.ToArray();
        }

        /// <summary>Evaluates an entry</summary>
        /// <param name="entry">The entry to evaluate</param>
        /// <returns>Whether to exclude, include or pass the entry to any remaining filters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FilterResult Evaluate(ref Entry entry)
        {
            string message = entry.Message;
            if (booleanOperator == BooleanOperator.Or)
            {
                for (int x = 0; x < regexes.Length; x++)
                    if (regexes[x].IsMatch(message))
                        return FilterResult.Exclude;

                return FilterResult.Pass;
            }
            else
            {
                for (int x = 0; x < regexes.Length; x++)
                    if (!regexes[x].IsMatch(message))
                        return FilterResult.Pass;

                return FilterResult.Exclude;
            }
        }
    }
}
