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
    /// <summary>A read-only string dictionary for small data sets</summary>
    internal sealed class StringDictionary : IDictionary<string, string>
    {
        /// <summary>A null/empty value</summary>
        public static StringDictionary EmptyStringDictionary = new StringDictionary();

        /// <summary>The keys for this mapping</summary>
        private string[] keys;

        /// <summary>The values for this mapping</summary>
        private string[] values;

        /// <summary>The key/value pairs for this mapping</summary>
        private KeyValuePair<string, string>[] keyValuePairs;

        /// <summary>Gets the key/value pairs for this mapping</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private KeyValuePair<string, string>[] GetKeyValuePairs()
        {
            if (keyValuePairs == null)
            {
                keyValuePairs = new KeyValuePair<string, string>[Count];
                for (int x = 0; x < Count; x++)
                    keyValuePairs[x] = new KeyValuePair<string, string>(keys[x], values[x]);
            }
            return keyValuePairs;
        }

        /// <summary>The number of mappings in this dictionary</summary>
        public int Count { get; private set; }

        /// <summary>Gets a value by key</summary>
        public string this[string key]
        {
            get
            {
                for (int x = 0; x < Count; x++)
                    if (keys[x] == key)
                        return values[x];
                return "";
            }
            set => throw new InvalidOperationException();
        }

        /// <summary>Gets the keys for this mapping</summary>
        public ICollection<string> Keys
        {
            get
            {
                return keys;
            }
        }

        /// <summary>Gets the values for this mapping</summary>
        public ICollection<string> Values
        {
            get
            {
                return values;
            }
        }

        /// <summary>Returns true</summary>
        public bool IsReadOnly { get { return true; } }

        /// <summary>Unsupported</summary>
        public void Add(string key, string value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Unsupported</summary>
        public void Add(KeyValuePair<string, string> item)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Unsupported</summary>
        public void Clear()
        {
            throw new InvalidOperationException();
        }

        /// <summary>Determines whether the specified pair exists</summary>
        /// <param name="item">The pair to find</param>
        /// <returns>true if found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(KeyValuePair<string, string> item)
        {
            for (int x = 0; x < Count; x++)
                if (keys[x] == item.Key && values[x] == item.Value)
                    return true;
            return false;
        }

        /// <summary>Determines whether the specified key exists</summary>
        /// <param name="key">The key to find</param>
        /// <returns>true if found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(string key)
        {
            for(int x = 0; x < Count; x++)
                if (keys[x] == key)
                    return true;
            return false;
        }

        /// <summary>Unsupported</summary>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>Unsupported</summary>
        public bool Remove(string key)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Unsupported</summary>
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Tries to get a value from this dictionary</summary>
        /// <param name="key">The key to find</param>
        /// <param name="value">The value, if found, or an empty string</param>
        /// <returns>true if found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(string key, out string value)
        {
            for (int x = 0; x < Count; x++)
            {
                if (keys[x] == key)
                {
                    value = values[x];
                    return true;
                }
            }
            value = "";
            return false;
        }

        /// <summary>An enumerator for this collection type</summary>
        private sealed class StringDictionaryEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            /// <summary>The current index</summary>
            private int index = -1;

            /// <summary>The key/value pairs enumerated by this instance</summary>
            private KeyValuePair<string, string>[] pairs;

            /// <summary>Gets the current pair</summary>
            public KeyValuePair<string, string> Current
            {
                get
                {
                    if (index < 0 || index >= pairs.Length) throw new IndexOutOfRangeException();
                    return pairs[index];
                }
            }

            /// <summary>Gets the current pair</summary>
            object IEnumerator.Current
            {
                get
                {
                    if (index < 0 || index >= pairs.Length) throw new IndexOutOfRangeException();
                    return pairs[index];
                }
            }

            /// <summary>Constructs a new instance</summary>
            /// <param name="pairs">The pairs to enumerate</param>
            public StringDictionaryEnumerator(KeyValuePair<string, string>[] pairs)
            {
                this.pairs = pairs;
            }

            /// <summary>Disposes of this instance</summary>
            public void Dispose() {}

            /// <summary>Moves to the next position</summary>
            /// <returns>true if the next value is available, otherwise false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (index < pairs.Length)
                {
                    index++;
                    return true;
                }
                return false;
            }

            /// <summary>Resets this enumerator to the beginning of the sequence</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                index = -1;
            }
        }

        /// <summary>Unsupported</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new StringDictionaryEnumerator(GetKeyValuePairs());
        }

        /// <summary>Gets an enumerator</summary>
        /// <returns>An enumerator for this dictionary</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StringDictionaryEnumerator(GetKeyValuePairs());
        }
    }
}
