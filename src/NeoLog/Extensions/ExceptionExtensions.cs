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
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLog.Extensions
{
    /// <summary>Extension/utility logic for exceptions</summary>
    public static partial class ExceptionExtensions
    {
        /// <summary>Gets an extended message, i.e. the original message plus extra exception info, in a compact form</summary>
        /// <param name="ex">this exception</param>
        /// <param name="includeExceptionTypeName">If true, the exception's full type name will be included</param>
        /// <param name="includeThrowingTypeName">If true, the name of the throwing type will be included</param>
        /// <param name="includeFileInfo">If true, the name of the file will be included</param>
        /// <returns>A message for this exception with more information than the original message</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public static string GetExtendedMessage(this Exception ex, bool includeExceptionTypeName = true, bool includeThrowingTypeName = true, bool includeFileInfo = true)
        {
            if (ex == null)
                return "";

            StringBuilder sb = new StringBuilder(100);

            if (includeExceptionTypeName)
                sb.Append(ex.GetType().FullName);

            if (!string.IsNullOrWhiteSpace(ex.Message))
            {
                if (includeExceptionTypeName) sb.Append(": ");
                sb.Append(ex.Message);
            }

            StackTrace st = new StackTrace(ex, true);
            if (st.FrameCount > 0)
            {
                try
                {
                    StackFrame frame = st.GetFrame(0);
                    MethodBase method = frame.GetMethod();
                    string fileName = frame.GetFileName();
                    int lineNumber = frame.GetFileLineNumber();

                    sb.Append(" (");

                    if (includeThrowingTypeName)
                        sb.Append(method.DeclaringType.FullName).Append('.');
                    sb.Append(method.Name);

                    if (includeFileInfo && !string.IsNullOrWhiteSpace(fileName))
                    {
                        sb.Append(',').Append(fileName);
                        if (lineNumber > 0)
                            sb.Append(':').Append(lineNumber);
                    }

                    sb.Append(')');
                }
                catch { }
            }

            return sb.ToString();
        }
    }
}
