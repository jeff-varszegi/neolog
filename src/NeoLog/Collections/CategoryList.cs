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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoLog.Collections
{
    /// <summary>An immutable list of categories</summary>
    public sealed class CategoryList : IList<Category>
    {
        /// <summary>The collection of categories wrapped by this list</summary>
        private Category[] categories;

        /// <summary>The original category used to construct this list; may be multiple</summary>
        private Category category;

        public int Count { get; private set; }

        /// <summary>Constructs a new instance, splitting the specified category into its component parts if it is not singular</summary>
        /// <param name="category">The category with which to populate the list</param>
        public CategoryList(Category category)
        {
            this.category = category;
            int intValue = (int)category;

            if (category.IsSingular())
            {
                categories = new Category[] { category };
                Count = 1;
                return;
            }

            int maxIndex = intValue > 65536 ? 65536 : intValue;

            Count = 0;
            for (int x = 1; x <= maxIndex; x *= 2)
                if ((intValue & x) == x)
                    Count++;

            categories = new Category[Count];
            Count = 0;
            for (int x = 1; x <= maxIndex; x *= 2)
                if ((intValue & x) == x)
                    categories[Count++] = (Category)x;
        }

        /// <summary>Accesses items in this list by index</summary>
        /// <param name="index">The index of the item to access</param>
        /// <returns>The category at the specified index</returns>
        public Category this[int index] {
            get { return categories[index]; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>Returns true, as this list is read-only</summary>
        public bool IsReadOnly { get; } = true;

        /// <summary>Unsupported</summary>
        public void Add(Category item) { throw new InvalidOperationException(); }

        /// <summary>Unsupported</summary>
        public void Clear() { throw new InvalidOperationException(); }

        /// <summary>Indicates whether this category contains the specified one</summary>
        /// <param name="otherCategory">The category for which to check</param>
        /// <returns>true if the other category (which may be multiple) is completely contained within this one</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Category otherCategory)
        {
            return ((category & otherCategory) == category);
        }

        /// <summary>Copies this list to the specified array</summary>
        /// <param name="array">The array to which to copy</param>
        /// <param name="arrayIndex">The index at which to begin copying</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Category[] array, int arrayIndex)
        {
            Array.Copy(categories, 0, array, arrayIndex, Count);
        }

        /// <summary>Finds the index of the specified category, which must be singular (else the operation is meaningless)</summary>
        /// <param name="item">The category for which to find the index</param>
        /// <returns>The index of the specified category, or -1 if not found; throws an exception if the specified category is multiple</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(Category item)
        {
            if (!item.IsSingular()) throw new InvalidOperationException("This operation can only be used to check for singular categories");
            for (int x = 0; x < Count; x++)
                if (categories[x] == item)
                    return x;
            return -1;
        }

        /// <summary>Unsupported</summary>
        public void Insert(int index, Category item) { throw new InvalidOperationException(); }

        /// <summary>Unsupported</summary>
        public bool Remove(Category item) { throw new InvalidOperationException(); }

        /// <summary>Unsupported</summary>
        public void RemoveAt(int index) { throw new InvalidOperationException(); }

        #region Enumerator

        /// <summary>Enumerates a category list's items</summary>
        private class CategoryListEnumerator : IEnumerator<Category>
        {
            /// <summary>The list to be enumerated</summary>
            private CategoryList list;

            /// <summary>The index of the current item</summary>
            private int index = -1;

            /// <summary>Constructs a new instance</summary>
            /// <param name="list">The list to enumerate</param>
            public CategoryListEnumerator(CategoryList list)
            {
                this.list = list;
            }

            /// <summary>The current item</summary>
            public Category Current {
                get
                {
                    if (index < 0 || index >= list.Count) throw new IndexOutOfRangeException();
                    return list[index];
                }
            }

            /// <summary>The current item</summary>
            object IEnumerator.Current
            {
                get
                {
                    if (index < 0 || index >= list.Count) throw new IndexOutOfRangeException();
                    return list[index];
                }
            }

            /// <summary>Disposes of this enumerator</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() {}

            /// <summary>Moves to the next item in the list</summary>
            /// <returns>true if a next item is available, otherwise false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (index < list.Count - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            /// <summary>Resets this enumerator to the beginning of the list</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                index = -1;
            }
        }

        /// <summary>Gets an enumerator for this list</summary>
        /// <returns>An enumerator for this list</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Category> GetEnumerator()
        {
            return new CategoryListEnumerator(this);
        }

        /// <summary>Gets an enumerator for this list</summary>
        /// <returns>An enumerator for this list</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CategoryListEnumerator(this);
        }

        #endregion
    }
}
