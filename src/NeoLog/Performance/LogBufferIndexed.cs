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

namespace NeoLog.Performance
{
    [ObsoleteAttribute("The single purpose of this struct is to measure performance penalties of a different programming approach; remove the ObsoleteAttribute to obtain microbenchmarks", true)]
    public class LogBufferIndexed
    {
        private const int MinimumLength = 100;
        
        public int Count { get; private set; }

        public int Length { get; private set; }

        private Level[] levels;

        private DateTime[] timestamps;

        private string[] messages;

        private Exception[] exceptions;

        private string[] contexts;

        private object[] data;

        private string[] tags;

        private string[] categories;

        private int[] threadIds;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetEntry(int index,
            out Level level,
            out DateTime timestamp,
            out string message,
            out Exception exception,
            out string context,
            out object data,
            out string tag,
            out string category,
            out int threadId
            )
        {
            level = levels[index];
            timestamp = timestamps[index];
            message = messages[index];
            exception = exceptions[index];
            context = contexts[index];
            data = this.data[index];
            tag = tags[index];
            category = categories[index];
            threadId = threadIds[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(
            Level level,
            DateTime timestamp,
            string message,
            Exception exception,
            string context,
            object data,
            string tag,
            string category,
            int threadId
            )
        {
            levels[Count] = level;
            timestamps[Count] = timestamp;
            messages[Count] = message;

            if (exception != null)
                exceptions[Count] = exception;
            if (context != null)
                contexts[Count] = context;
            if (data != null)
                this.data[Count] = data;
            if (tag != null)
                tags[Count] = tag;
            if (category != null)
                categories[Count] = category;

            Count++;
        }

        public LogBufferIndexed(int length)
        {
            Length = length < MinimumLength ? MinimumLength : length;
            levels = new Level[Length];
            timestamps = new DateTime[Length];
            messages = new string[Length];
            exceptions = new Exception[Length];
            contexts = new string[Length];
            data = new object[Length];
            tags = new string[Length];
            categories = new string[Length];
            threadIds = new int[Length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;
            Array.Clear(exceptions, 0, Length);
            Array.Clear(contexts, 0, Length);
            Array.Clear(data, 0, Length);
            Array.Clear(tags, 0, Length);
            Array.Clear(categories, 0, Length);
            Array.Clear(threadIds, 0, Length);
        }
    }
}
