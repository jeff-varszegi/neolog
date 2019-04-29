﻿/***********************************************************************************************************************
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

namespace NeoLog.Formatting.Patterns.Tokens
{
    /// <summary>A token for OS info</summary>
    internal sealed class OsToken : Token
    {
        /// <summary>The value to use when replacing token placeholders</summary>
        private static string Value;

        /// <summary>Static initializer</summary>
        static OsToken()
        {
            TokenFactory.Default.Register(typeof(OsToken), "{{os}}");

            try
            {
                Value = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            }
            catch
            {
                Value = "";
            }
        }
        
        /// <summary>Default constructor</summary>
        private OsToken() : base(null) { }

        /// <summary>Constructs a new instance</summary>
        /// <param name="text">The source text of this token</param>
        public OsToken(string text) : base(text) { }

        /// <summary>Formats an entry</summary>
        /// <param name="entry">The entry to format</param>
        /// <returns>A string representation of the specified entry</returns>
        public override string Format(ref Entry entry)
        {
            return Value;
        }
    }
}