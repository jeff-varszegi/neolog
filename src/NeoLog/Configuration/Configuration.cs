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

using NeoLog.Collections;

namespace NeoLog.Configuration
{
    /// <summary>A configuration for a logging component</summary>
    public class Configuration
    {
        /// <summary>Default required settings</summary>
        private static string[] DefaultRequiredSettings = { };

        /// <summary>Ad-hoc settings for this configuration</summary>
        public SettingCollection Settings { get; private set; } = new SettingCollection();

        /// <summary>A fallback configuration, from which to get values missing in this one</summary>
        private Configuration parent;

        /// <summary>The fallback parent of this configuration</summary>
        public Configuration Parent
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
                    Configuration ancestor = value;
                    while (ancestor != null)
                    {
                        if (ancestor == this) return;
                        ancestor = ancestor.parent;
                    }

                    SettingCollection newSettings = Settings.Copy();
                    newSettings.Parent = value.Settings;
                    parent = value;
                    Settings = newSettings;
                }
            }
        }

        /// <summary>Settings required for this configuration to be valid</summary>
        public virtual IEnumerable<string> RequiredSettings { get { return DefaultRequiredSettings; } }

        /// <summary>Copies this configuration</summary>
        /// <typeparam name="T">The type of this instance</typeparam>
        /// <returns>A copy of this configuration</returns>
        public T Copy<T>() where T: Configuration
        {
            object o = typeof(T);
            T copy = (T)this.MemberwiseClone();
            copy.Settings = this.Settings.Copy();
            return copy;
        } 
    }
}
