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

using NeoLog.Configuration.Parsers;

namespace NeoLog.Configuration.Sources
{
    /// <summary>An abstract base class for IConfigurationSource implementations</summary>
    public abstract class ConfigurationSource : IConfigurationSource
    {
        /// <summary>Gets a logging configuration</summary>
        /// <param name="password">A password to use in optional decryption</param>
        /// <param name="resaveEncrypted">If true and a password is supplied, requests that the configuration source encrypt and resave the configuration after a successful load</param>
        /// <returns>A logging configuration</returns>
        public LoggingConfiguration GetConfiguration(string password = null, bool resaveEncrypted = false)
        {
            string text = LoadText();
            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("Invalid (blank) configuration text");

            bool isEncrypted = IsEncrypted(text);
            if (isEncrypted && string.IsNullOrWhiteSpace(password)) throw new ArgumentException("No password supplied--cannot decrypt text");

            if (isEncrypted)
                text = Decrypt(text, password);
            else if (resaveEncrypted)
                SaveText(Encrypt(text, password));

            IConfigurationParser parser = ConfigurationParserFactory.GetParser(text);
            if (parser == null)
                throw new FormatException("Parser for configuration format not found");

            return parser.Parse(text);
        }

        /// <summary>Indicates whether the specified text is encrypted</summary>
        /// <param name="text">The text to test</param>
        /// <returns>true if encrypted, otherwise false</returns>
        private bool IsEncrypted(string text)
        {
            return true;
        }

        /// <summary>Decrypts the specified text</summary>
        /// <param name="text">The text to decrypt</param>
        /// <param name="password">The password to use</param>
        /// <returns>Decrypted text</returns>
        private string Decrypt(string text, string password)
        {
            return text;
        }

        /// <summary>Encrypts the specified text</summary>
        /// <param name="text">The text to encrypt</param>
        /// <param name="password">The password to use</param>
        /// <returns>Encrypted text</returns>
        private string Encrypt(string text, string password)
        {
            return text;
        }

        /// <summary>Gets configuration text</summary>
        /// <returns>Text of a configuration, which may be encrypted</returns>
        protected abstract string LoadText();

        /// <summary>Stores configuration text</summary>
        /// <param name="text">The text to store</param>
        protected abstract void SaveText(string text);
    }
}
