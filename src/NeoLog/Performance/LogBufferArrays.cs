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

namespace NeoLog
{
    [ObsoleteAttribute("The single purpose of this class is to measure performance penalties of a different programming approach; remove the ObsoleteAttribute to obtain microbenchmarks", true)]
    public class LogBufferArrays
    {
        private const int MinimumLength = 100;
        
        public int Count { get; set; }
        public int Length { get; private set; }

        public Level[] Levels { get; private set; }
        public DateTime[] Timestamps { get; private set; }
        public string[] Messages { get; private set; }
        public Exception[] Exceptions { get; private set; }
        public string[] Contexts { get; private set; }
        public object[] Data { get; private set; }
        public string[] Tags { get; private set; }
        public string[] Categories { get; private set; }
        public int[] ThreadIds { get; private set; }

        public LogBufferArrays(int length)
        {
            Length = length < MinimumLength ? MinimumLength : length;
            Levels = new Level[Length];
            Timestamps = new DateTime[Length];
            Messages = new string[Length];
            Exceptions = new Exception[Length];
            Contexts = new string[Length];
            Data = new object[Length];
            Tags = new string[Length];
            Categories = new string[Length];
            ThreadIds = new int[Length];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;
            Array.Clear(Exceptions, 0, Length);
            Array.Clear(Contexts, 0, Length);
            Array.Clear(Data, 0, Length);
            Array.Clear(Tags, 0, Length);
            Array.Clear(Categories, 0, Length);
            Array.Clear(ThreadIds, 0, Length);
        }
    }
}
