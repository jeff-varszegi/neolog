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

namespace NeoLog.Utility
{
    /// <summary>Provides simple symmetric encryption and decryption for strings</summary>
    public static class EncryptionUtility
    {
        /// <summary>Password-encrypts text</summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="password">The password to use</param>
        /// <returns>An encrypted version of the specified text</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Encrypt(string text, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>Password-decrypts text</summary>
        /// <param name="text">The text to decrypt</param>
        /// <param name="password">The password to use</param>
        /// <returns>A decrypted version of the specified text</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Decrypt(string text, string password)
        {
            throw new NotImplementedException();
        }
    }
}
