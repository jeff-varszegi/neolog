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
using System.Runtime.CompilerServices;

namespace NeoLog.Extensions
{
    /// <summary>Extension/utility logic for time spans</summary>
    public static partial class TimeSpanExtensions
    {
        /// <summary>Calculates the number of nanoseconds in this time span</summary>
        /// <param name="timeSpan">this TimeSpan</param>
        /// <param name="operationCount">The number of operations by which to divide the return value, e.g. if this call is being made to assess the performance of an operation in a loop</param>
        /// <returns>The number of nanoseconds in this TimeSpan, optionally divided by a number of operations to get the average</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetNanoseconds(this TimeSpan timeSpan, int operationCount = 0)
        {
            return
                operationCount <= 1 ?
                (double)(timeSpan.Ticks * 100) :
                ((double)(timeSpan.Ticks * 100)) / (double)operationCount;
        }

        /// <summary>Shows time components of this time span in a common format</summary>
        /// <param name="timeSpan">this TimeSpan</param>
        /// <param name="enhancedPrecision">If true, fractional seconds will be reported down to the tick level</param>
        /// <returns>A string representation of this TimeSpan</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this TimeSpan timeSpan, bool enhancedPrecision = false)
        {
            if (enhancedPrecision)
            {
                if (timeSpan.Days > 0)
                    return string.Format("{0}:{1:00}:{2:00}:{3:00}.{4:0000000}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Ticks % 10000000);
                else if (timeSpan.Hours > 0)
                    return string.Format("{0:00}:{1:00}:{2:00}.{3:0000000}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Ticks % 10000000);
                else if (timeSpan.Minutes > 0)
                    return string.Format("{0:00}:{1:00}.{2:0000000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Ticks % 10000000);
                else
                    return string.Format("{0}.{1:0000000}", timeSpan.Seconds, timeSpan.Ticks % 10000000);
            }
            else
            {
                if (timeSpan.Days > 0)
                    return string.Format("{0}:{1:00}:{2:00}:{3:00}.{4:000}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                else if (timeSpan.Hours > 0)
                    return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                else if (timeSpan.Minutes > 0)
                    return string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                else
                    return string.Format("{0}.{1:000}", timeSpan.Seconds, timeSpan.Milliseconds);
            }
        }
    }
}
