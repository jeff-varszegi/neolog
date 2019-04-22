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

namespace NeoLog.Configuration.Sources
{
    /// <summary>Reads a logging configuration directly from a string; allows easy custom loading of configurations from alternate sources</summary>
    public class StringConfigurationSource : ConfigurationSource
    {
        /// <summary>Configuration text</summary>
        public string Text { get; private set; }

        /// <summary>Default constructor</summary>
        private StringConfigurationSource() { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The configuration text to use</param>
        public StringConfigurationSource(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Invalid (null or whitespace) logging configuration text");
            Text = text;
        }

        /// <summary>Gets configuration text</summary>
        /// <returns>Text of a configuration, which may be encrypted</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override string LoadText()
        {
            return Text;
        }

        /// <summary>Stores configuration text</summary>
        /// <param name="text">The text to store</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override void SaveText(string text)
        {
            Text = text;
        }
    }
}
