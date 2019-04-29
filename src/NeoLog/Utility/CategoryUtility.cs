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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using NeoLog.Configuration;

namespace NeoLog.Utility
{
    /// <summary>Utility routines for dealing with categories</summary>
    internal static class CategoryUtility
    {
        /// <summary>The string delimiter when using multiple categories</summary>
        private static char CategoryDelimiter = ',';

        /// <summary>String delimiters used when splitting multiple categories</summary>
        private static char[] CategoryDelimiters = { ',' };

        /// <summary>A name-to-category mapping, case insensitive</summary>
        private static ConcurrentDictionary<string, Category> CategoryMap = new ConcurrentDictionary<string, Category>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>A name-to-category mapping, case insensitive</summary>
        private static ConcurrentDictionary<Category, string> CategoryReverseMap = new ConcurrentDictionary<Category, string>();

        /// <summary>Static initializer</summary>
        static CategoryUtility()
        {
            CategoryReverseMap[Category.None] = "";
            CategoryReverseMap[Category.Admin] = "admin";
            CategoryReverseMap[Category.Application] = "application";
            CategoryReverseMap[Category.Audit] = "audit";
            CategoryReverseMap[Category.Data] = "data";
            CategoryReverseMap[Category.Security] = "security";
            CategoryReverseMap[Category.System] = "system";
            CategoryReverseMap[Category.User] = "user";


            CategoryReverseMap[Category.Custom1] = "(custom)";
            CategoryReverseMap[Category.Custom2] = "(custom)";
            CategoryReverseMap[Category.Custom3] = "(custom)";
            CategoryReverseMap[Category.Custom4] = "(custom)";
            CategoryReverseMap[Category.Custom5] = "(custom)";
            CategoryReverseMap[Category.Custom6] = "(custom)";
            CategoryReverseMap[Category.Custom7] = "(custom)";
            CategoryReverseMap[Category.Custom8] = "(custom)";
            CategoryReverseMap[Category.Custom9] = "(custom)";
            CategoryReverseMap[Category.Custom10] = "(custom)";
        }

        /// <summary>The number of taken custom categories</summary>
        private static int CustomCategoryCount = 0;

        /// <summary>The maximum number of supported custom categories</summary>
        private static int CustomCategoryMaxCount = 10;

        /// <summary>A listing of custom categories</summary>
        private static Category[] CustomCategories = {
            Category.Custom1,
            Category.Custom2,
            Category.Custom3,
            Category.Custom4,
            Category.Custom5,
            Category.Custom6,
            Category.Custom7,
            Category.Custom8,
            Category.Custom9,
            Category.Custom10,
        };

        /// <summary>Converts a string to a category (using a comma as a delimiter in case of multiples)</summary>
        /// <param name="s">The string to convert</param>
        /// <returns>A category for the specified string; throws an exception if too many custom categories have been defined</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Category ConvertToCategory(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return Category.None;

            Category returnValue = Category.None;

            if (s.IndexOf(CategoryDelimiter) > -1)
            {
                string[] categoryNames = s.Split(CategoryDelimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in categoryNames)
                {
                    string categoryName = c.Trim();
                    if (CategoryMap.TryGetValue(categoryName, out Category category))
                    {
                        returnValue = returnValue | category;
                    }
                    else
                    {
                        lock (CategoryMap)
                        {
                            if (CategoryMap.TryGetValue(categoryName, out category))
                            {
                                returnValue = returnValue | category;
                            }
                            else
                            {
                                if (CustomCategoryCount >= CustomCategoryMaxCount)
                                {
                                    if (LoggingConfiguration.Default.IsExceptionThrowingEnabled)
                                        throw new InvalidOperationException("Maximum number of custom categories (" + CustomCategoryMaxCount + ") already reached; cannot add \"" + categoryName + "\"");
                                }
                                else
                                {
                                    Category customCategory = CustomCategories[CustomCategoryCount++];
                                    CategoryMap[categoryName] = customCategory;
                                    CategoryReverseMap[customCategory] = categoryName;
                                    returnValue = returnValue | customCategory;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                string categoryName = s.Trim();
                if (CategoryMap.TryGetValue(categoryName, out Category category))
                {
                    returnValue = returnValue | category;
                }
                else
                {
                    lock (CategoryMap)
                    {
                        if (CategoryMap.TryGetValue(categoryName, out category))
                        {
                            returnValue = returnValue | category;
                        }
                        else
                        {
                            if (CustomCategoryCount >= CustomCategoryMaxCount)
                            {
                                if (LoggingConfiguration.Default.IsExceptionThrowingEnabled)
                                    throw new InvalidOperationException("Maximum number of custom categories (" + CustomCategoryMaxCount + ") already reached; cannot add \"" + categoryName + "\"");
                            }
                            else
                            {
                                Category customCategory = CustomCategories[CustomCategoryCount++];
                                CategoryMap[categoryName] = customCategory;
                                CategoryReverseMap[customCategory] = categoryName;
                                returnValue = returnValue | customCategory;
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary>Converts a category to a string</summary>
        /// <param name="category">The category to convert</param>
        /// <returns>A string name for the category</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToString(Category category)
        {
            return CategoryReverseMap.TryGetValue(category, out string categoryName) ? categoryName : "";
        }
    }
}
