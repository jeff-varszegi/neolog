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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NeoLog.Formatting.Data
{
    /// <summary>Formats data as XML</summary>
    internal class XmlDataFormatter : IDataFormatter
    {
        #region Static members

        /// <summary>Reusable XML serializers for encountered data types</summary>
        private static ConcurrentDictionary<string, XmlSerializer> Serializers = new ConcurrentDictionary<string, XmlSerializer>();

        /// <summary>Serialization settings</summary>
        private static XmlWriterSettings InlineSettingsWithoutHeader = new XmlWriterSettings()
        {
            Async = false,
            Indent = false,
            OmitXmlDeclaration = true
        };

        /// <summary>Serialization settings</summary>
        private static XmlWriterSettings InlineSettingsWithHeader = new XmlWriterSettings()
        {
            Async = false,
            Indent = false,
            OmitXmlDeclaration = false
        };

        /// <summary>Serialization settings</summary>
        private static XmlWriterSettings IndentedSettingsWithoutHeader = new XmlWriterSettings()
        {
            Async = false,
            Indent = true,
            OmitXmlDeclaration = true
        };

        /// <summary>Serialization settings</summary>
        private static XmlWriterSettings IndentedSettingsWithHeader = new XmlWriterSettings()
        {
            Async = false,
            Indent = true,
            OmitXmlDeclaration = false
        };

        #endregion Static members

        /// <summary>Indicates the data format supported by this formatter</summary>
        public DataFormat DataFormat { get; } = DataFormat.Xml;

        /// <summary>Formats the specified data as XML</summary>
        /// <param name="data">The data to format</param>
        /// <param name="options">Formatting options to use</param>
        /// <returns>An XML representation of the specified data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Format(object data, DataFormatOptions options)
        {
            if (data == null) return "";

            XmlSerializer serializer;
            Type type = data.GetType();
            string typeName = type.FullName;
            if (!Serializers.TryGetValue(typeName, out serializer))
            {
                serializer = new XmlSerializer(type);
                Serializers[typeName] = serializer;
            }

            StringBuilder sb = new StringBuilder(64);
            XmlWriterSettings settings;

            if (options.HasFlag(DataFormatOptions.IncludeOptionalDeclarations))
            {
                if (options.HasFlag(DataFormatOptions.Indented))
                    settings = IndentedSettingsWithHeader;
                else
                    settings = InlineSettingsWithHeader;
            }
            else
            {
                if (options.HasFlag(DataFormatOptions.Indented))
                    settings = IndentedSettingsWithoutHeader;
                else
                    settings = InlineSettingsWithoutHeader;
            }

            using (var sw = new StringWriter(sb))
            {
                using (XmlWriter w = XmlWriter.Create(sw)) // TODO options
                {
                    serializer.Serialize(w, data);
                    return sb.ToString();
                }
            }
        }
    }
}
