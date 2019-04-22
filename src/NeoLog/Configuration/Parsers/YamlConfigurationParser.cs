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
using System.Text;

namespace NeoLog.Configuration.Parsers
{
    /// <summary>Parses logging configurations from YAML</summary>
    internal class YamlConfigurationParser : IConfigurationParser
    {
        /// <summary>A default/singleton instance</summary>
        public static YamlConfigurationParser Default { get; } = new YamlConfigurationParser();

        /// <summary>Indicates whether this parser is appropriate for the type of configuration text</summary>
        /// <param name="text">The text to test</param>
        /// <returns>true if this parser can likely handle the text, otherwise false</returns>
        public bool HandlesFormat(string text)
        {
            text = text.Trim();
            return !text.StartsWith("<") && !text.StartsWith("{");
        }

        /// <summary>Parses a configuration from text</summary>
        /// <param name="text">The text to parse</param>
        /// <returns>A logging configuration</returns>
        public LoggingConfiguration Parse(string text)
        {
            throw new NotImplementedException();
        }
    }
}
