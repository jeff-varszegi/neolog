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

namespace NeoLog.Configuration.Parsers
{
    /// <summary>Gets configuration parsers</summary>
    internal static class ConfigurationParserFactory
    {
        /// <summary>Parsers to use in testing and parsing configurations</summary>
        private static IConfigurationParser[] Parsers =
        {
            JsonConfigurationParser.Default,
            XmlConfigurationParser.Default,
            YamlConfigurationParser.Default
        };

        /// <summary>Gets a parser for the specified text</summary>
        /// <param name="text">The text for which to get an appropriate parser</param>
        /// <returns>A parser, or null if none is found</returns>
        public static IConfigurationParser GetParser(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Invalid (null or whitespace) configuration text");

            foreach (IConfigurationParser parser in Parsers)
            {
                if (parser.HandlesFormat(text))
                    return parser;
            }

            return null;
        }
    }
}
