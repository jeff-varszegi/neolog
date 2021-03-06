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

using System;

namespace NeoLog
{
    /// <summary>Enumerates logging levels</summary>
    [Flags]
    public enum Level
    {
        /// <summary>A level indicating that no logging should or will be performed</summary>
        None = 0,
        /// <summary>A tracing logging level</summary>
        Trace = 1,
        /// <summary>A debugging logging level</summary>
        Debug = 2,
        /// <summary>An informational logging level</summary>
        Info = 4,
        /// <summary>A warning logging level</summary>
        Warning = 8,
        /// <summary>A An exception logging level</summary>
        Exception = 16,
        /// <summary>A fatal-error logging level</summary>
        Fatal = 32
    }
}
