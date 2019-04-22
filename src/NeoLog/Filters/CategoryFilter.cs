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

namespace NeoLog.Filters
{
    /// <summary>Filters entries based on category</summary>
    public sealed class CategoryFilter : IFilter
    {
        /// <summary>A canonical empty string array</summary>
        private static string[] EmptyStringArray = new string[] {};

        /// <summary>The categories to match</summary>
        private string[] categories;

        /// <summary>If true, entries must match at least one of the filter's categories; if false, none of them</summary>
        private bool isInclusive;

        /// <summary>Default constructor</summary>
        private CategoryFilter() { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="categories">The categories on which to filter</param>
        /// <param name="inclusive">If true, entries must match at least one category; if false, none</param>
        public CategoryFilter(IEnumerable<string> categories, bool inclusive = true)
        {
            if (categories == null)
            {
                categories = new string[] {};
                isInclusive = false;
                return;
            }

            List<string> list = new List<string>(5);
            foreach(string category in categories.Distinct())
            {
                if (!string.IsNullOrWhiteSpace(category))
                    list.Add(category.Trim());
            }

            if (list.Count == 0)
            {
                categories = new string[] { };
                isInclusive = false;
            }
            else
            {
                categories = list.ToArray();
                isInclusive = inclusive;
            }
        }

        /// <summary>Indicates whether this filter matches the specified entry, i.e. excludes it from output</summary>
        /// <param name="entry">The entry to test</param>
        /// <returns>true if the entry should be excluded, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Excludes(ref Entry entry)
        {
            // TODO: category splitting (punch through settings?)
            if (isInclusive)
            {
                if (string.IsNullOrWhiteSpace(entry.Category))
                    return true;

                for (int x = 0; x < categories.Length; x++)
                    if (entry.Category.Equals(categories[x], StringComparison.InvariantCultureIgnoreCase))
                        return false;

                return true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entry.Category))
                    return false;

                for (int x = 0; x < categories.Length; x++)
                    if (entry.Category.Equals(categories[x], StringComparison.InvariantCultureIgnoreCase))
                        return true;

                return false;
            }
        }
    }
}
