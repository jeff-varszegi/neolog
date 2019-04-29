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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NeoLog
{
    /// <summary>A collection of log entries</summary>
    public sealed class EntryBuffer
    {
        #region Static members

        /// <summary>A default length to use</summary>
        private const int DefaultLength = 5000;

        /// <summary>The maximum size of the reusable-buffer pool</summary>
        private const int MaximumBufferPoolCount = 20000;
        
        /// <summary>A pool of reusable buffers</summary>
        private static ConcurrentQueue<EntryBuffer> BufferPool = new ConcurrentQueue<EntryBuffer>();

        /// <summary>Allocates an entry buffer</summary>
        /// <returns>An empty entry buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntryBuffer GetBuffer()
        {
            if (BufferPool.TryDequeue(out EntryBuffer buffer))
            {
                lock (buffer.monitor)
                    buffer.isPooled = false;
                return buffer;
            }
            else
                return new EntryBuffer();
        }

        #endregion Static members

        /// <summary>A locking object used in synchronization</summary>
        private object monitor = new object();

        /// <summary>Indicates whether this buffer is pooled (true) or ready for use (false)</summary>
        private bool isPooled = false;

        private int testCount;

        private int count;
        
        /// <summary>The number of entries in this buffer</summary>
        public int Count { get { return count; } }

        /// <summary>The total size of this buffer, which may include some unused entries</summary>
        public int Length { get; } = DefaultLength;

        /// <summary>The entries which make up this buffer</summary>
        public Entry[] Entries { get; } = new Entry[DefaultLength];

        public bool AddEntry(
            Level level,
            DateTime timestamp,
            string message = null,
            Exception exception = null,
            string context = null,
            object data = null,
            string tag = null,
            Category category = Category.None,
            string user = null,
            int threadId = 0,
            IDictionary<string, string> properties = null
            )
        {
            int newCount = Interlocked.Increment(ref testCount); // Increment this first to see if we're running off the edge of the buffer...
            if (newCount > Length)                               // ... and roll back as necessary
            {
                Interlocked.Decrement(ref testCount);
                return false;
            }
            else
            {
                Interlocked.Increment(ref count);
                UpdateEntry(
                    ref Entries[newCount - 1],
                    level,
                    timestamp,
                    message,
                    exception,
                    context,
                    data,
                    tag,
                    category,
                    user,
                    threadId,
                    properties
                    );
                return true;
            }
        }

        /// <summary>Updates a buffered entry in-place, to avoid data copying</summary>
        /// <param name="entry">The entry to update</param>
        /// <param name="level">The log level of the entry</param>
        /// <param name="timestamp">The timestamp of the entry</param>
        /// <param name="message">The message of the entry</param>
        /// <param name="exception">The exception of the entry</param>
        /// <param name="context">The context of the entry</param>
        /// <param name="data">The data of the entry</param>
        /// <param name="tag">The tag(s) of the entry</param>
        /// <param name="category">The category/categories of the entry</param>
        /// <param name="user">The user identity for the entry, if any</param>
        /// <param name="threadId">The thread ID of the entry</param>
        /// <param name="properties">Custom key-value pairs for the entry</param>
        private void UpdateEntry(
            ref Entry entry,
            Level level,
            DateTime timestamp,
            string message = null,
            Exception exception = null,
            string context = null,
            object data = null,
            string tag = null,
            Category category = Category.None,
            string user = null,
            int threadId = 0,
            IDictionary<string, string> properties = null
            )
        {
            entry.Level = level;
            entry.Timestamp = timestamp;
            entry.Message = message;
            entry.Exception = exception;
            entry.Context = context;
            entry.Data = data;
            entry.Tag = tag;
            entry.Category = category;
            entry.User = user;
            entry.ThreadId = threadId;
            entry.Properties = properties;
        }

        /// <summary>Clears the data held in this buffer</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            lock (monitor)
            {
                Array.Clear(Entries, 0, Length);
                testCount = 0;
                count = 0;
            }
        }

        /// <summary>Releases this buffer to the pool for reuse</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release()
        {
            lock (monitor)
            {
                if (isPooled)
                    return;
                Clear();
                if (BufferPool.Count < MaximumBufferPoolCount) // it's all right to be a little over
                {
                    isPooled = true;
                    BufferPool.Enqueue(this);
                }
            }
        }
    }
}
