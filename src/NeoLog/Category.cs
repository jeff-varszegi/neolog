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

using NeoLog.Collections;
using NeoLog.Utility;

namespace NeoLog
{
    /// <summary>Enumerates entry categories</summary>
    [Flags]
    public enum Category
    {
        /// <summary>Indicates that no category flags have been applied to an entry</summary>
        None = 0,

        /// <summary>Indicates that admin operations are logged</summary>
        Admin = 1,

        /// <summary>Indicates that application data is logged</summary>
        Application = 2,

        /// <summary>Indicates that an entry is made in an audit log</summary>
        Audit = 4,

        /// <summary>Indicates that data, such as content, is accessed, updated or deleted</summary>
        Data = 8,

        /// <summary>Indicates that a security event is logged</summary>
        Security = 16,

        /// <summary>Indicates that system-level data is logged</summary>
        System = 32,

        /// <summary>Indicates that user activity is logged</summary>
        User = 64,

        /// <summary>A custom category</summary>
        Custom1 = 128,

        /// <summary>A custom category</summary>
        Custom2 = 256,

        /// <summary>A custom category</summary>
        Custom3 = 512,

        /// <summary>A custom category</summary>
        Custom4 = 1024,

        /// <summary>A custom category</summary>
        Custom5 = 2048,

        /// <summary>A custom category</summary>
        Custom6 = 4096,

        /// <summary>A custom category</summary>
        Custom7 = 8192,

        /// <summary>A custom category</summary>
        Custom8 = 16384,

        /// <summary>A custom category</summary>
        Custom9 = 32768,

        /// <summary>A custom category</summary>
        Custom10 = 65536
    }

    /// <summary>Extension/utility logic for categories</summary>
    public static class CategoryExtensions
    {
        /// <summary>Reusable immutable lists of categories</summary>
        private static CategoryList[] CachedCategoryLists = new CategoryList[65537];

        /// <summary>Indicates whether this category contains a single value</summary>
        /// <param name="category">this category</param>
        /// <returns>true if this category has a single value/flag, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSingular(this Category category)
        {
            switch ((int)category)
            {
                case 0:
                case 1:
                case 2:
                case 4:
                case 8:
                case 16:
                case 32:
                case 64:
                case 128:
                case 256:
                case 512:
                case 1024:
                case 2048:
                case 4096:
                case 8192:
                case 16384:
                case 32768:
                case 65536:
                    return true;
            }
            return false;
        }

        /// <summary>Lists categories contained within this one, when multiple flags are used</summary>
        /// <param name="category">This category</param>
        /// <returns>A list of categories contained within this one</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<Category> Split(this Category category)
        {
            int intValue = (int)category;

            CategoryList list = CachedCategoryLists[intValue];
            if (list == null)
            {
                list = new CategoryList(category);
                CachedCategoryLists[intValue] = list;
            }

            return list;
        }

        /// <summary>Gets the configured string name for the category</summary>
        /// <param name="category">The category for which to get the name</param>
        /// <returns>The string name for the category</returns>
        public static string GetName(this Category category)
        {
            return CategoryUtility.ConvertToString(category);
        }
    }
}
