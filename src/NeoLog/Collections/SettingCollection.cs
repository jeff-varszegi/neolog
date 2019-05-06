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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeoLog.Collections
{
    /// <summary>A thread-safe collection of name-value pairs, indexed values of which can be implicitly cast to various types (bool, int, float, string etc.)</summary>
    public sealed class SettingCollection : IDictionary<string, Setting>
    {
        /// <summary>Underlying thread-safe settings storage</summary>
        private ConcurrentDictionary<string, Setting> settings = new ConcurrentDictionary<string, Setting>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>A fallback parent collection for this collection</summary>
        private SettingCollection parent;

        /// <summary>A fallback parent collection from which to get settings missing in this one</summary>
        public SettingCollection Parent
        {
            get
            {
                return parent;
            }

            set
            {
                lock (this)
                {
                    if (parent != null) throw new InvalidOperationException("Parent can only be set once");
                    else if (value == null) return;

                    // Cyclical redundancy check
                    SettingCollection ancestor = value;
                    while (ancestor != null)
                    {
                        if (ancestor == this) return;
                        ancestor = ancestor.parent;
                    }

                    parent = value;
                }
            }
        }

        /// <summary>Accesses a setting by name</summary>
        /// <param name="settingName">The name of the setting to access</param>
        /// <returns>The setting with the specified name</returns>
        public Setting this[string settingName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("Invalid (null or whitespace) setting name");
                settingName = settingName.Trim();
                if (settings.TryGetValue(settingName, out Setting setting))
                {
                    return setting;
                }
                else
                {
                    return parent != null ? parent[settingName] : new Setting(settingName, "");
                }
            }
            set
            {
                if (string.IsNullOrWhiteSpace(settingName)) throw new ArgumentException("Invalid (null or whitespace) setting name");
                settings[settingName] = value;
            }
        }

        /// <summary>Copies this collection</summary>
        /// <returns>A copy of this collection</returns>
        public SettingCollection Copy()
        {
            SettingCollection copy = new SettingCollection();
            foreach (var pair in settings.ToArray())
                copy[pair.Key] = pair.Value;
            copy.parent = this.parent;
            return copy;
        }

        /// <summary>Merges settings from the specified collection into this one</summary>
        /// <param name="other"></param>
        public void Merge(SettingCollection other)
        {
            if (other == null) return;

            foreach (var pair in other.ToArray())
            {
                if (!settings.ContainsKey(pair.Key))
                    settings[pair.Key] = pair.Value;
            }

            parent = parent ?? other.parent;
        }

        /// <summary>Gets a collection of setting names</summary>
        public ICollection<string> Keys { get { return settings.Keys; } }

        /// <summary>Gets a collection of settings</summary>
        public ICollection<Setting> Values { get { return settings.Values; } }

        /// <summary>The number of settings stored in this collection</summary>
        public int Count { get { return settings.Count; } }

        public bool IsReadOnly => throw new NotImplementedException();

        /// <summary>Adds a setting</summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The value of the setting</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(string name, Setting value)
        {
            this[name] = value;
        }

        /// <summary>Adds a setting</summary>
        /// <param name="item">The item to add</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(KeyValuePair<string, Setting> item)
        {
            this[item.Key] = item.Value;
        }

        /// <summary>Removes all settings from this collection</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            settings.Clear();
        }

        /// <summary>Indicates whether this collection contains the specified mapping</summary>
        /// <param name="item">The item for which to check</param>
        /// <returns>true if found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(KeyValuePair<string, Setting> item)
        {
            return settings.ContainsKey(item.Key);
        }

        /// <summary>Indicates whether this collection contains the specified setting</summary>
        /// <param name="key">The key for which to check</param>
        /// <returns>true if the setting was found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(string key)
        {
            return settings.ContainsKey(key);
        }

        /// <summary>Not implemented</summary>
        public void CopyTo(KeyValuePair<string, Setting>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets an enumerator of key-value pairs stored in this collection</summary>
        /// <returns>An enumerator</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<string, Setting>> GetEnumerator()
        {
            return settings.GetEnumerator();
        }

        /// <summary></summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return settings.TryRemove(key, out Setting setting);
        }

        /// <summary>Removes the specified item</summary>
        /// <param name="item">The item to remove</param>
        /// <returns>true if found and removed, otherwise false</returns>
        public bool Remove(KeyValuePair<string, Setting> item)
        {
            return settings.TryRemove(item.Key, out Setting setting);
        }

        /// <summary>Attempts to find a setting</summary>
        /// <param name="name">The name of the setting to find</param>
        /// <param name="value">The setting, if found</param>
        /// <returns>true if found, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(string key, out Setting value)
        {
            return settings.TryGetValue(key, out value);
        }

        /// <summary>Gets an enumerator for key-value pairs</summary>
        /// <returns>An enumerator for name/setting pairs</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return settings.GetEnumerator();
        }
    }
}
