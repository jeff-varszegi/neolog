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

namespace NeoLog.Performance
{
    [ObsoleteAttribute("The single purpose of this class is to measure performance penalties of a different programming approach; remove the ObsoleteAttribute to obtain microbenchmarks", true)]
    public class EntryObject
    {
        public Level Level { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public string Context { get; private set; }
        public object Data { get; private set; }
        public string Tag { get; private set; }
        public string Category { get; private set; }
        public int ThreadId { get; private set; }

        public EntryObject(Level level, DateTime timestamp, string message, Exception exception, string context, object data, string tag, string category, int threadId)
        {
            this.Level = level;
            this.Timestamp = timestamp;
            this.Message = message;
            this.Exception = exception;
            this.Context = context;
            this.Data = data;
            this.Tag = tag;
            this.Category = category;
            this.ThreadId = threadId;
        }
    }
}
