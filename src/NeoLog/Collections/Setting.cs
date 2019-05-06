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

namespace NeoLog.Collections
{
    /// <summary>A configuration setting</summary>
    public sealed class Setting
    {
        /// <summary></summary>
        private const long DefaultLongValue = 0L;

        /// <summary></summary>
        private const int DefaultIntValue = 0;
        
        /// <summary></summary>
        private const double DefaultDoubleValue = 0.0D;
        
        /// <summary></summary>
        private const float DefaultFloatValue = 0.0F;
        
        /// <summary></summary>
        private const string DefaultStringValue = "";

        /// <summary>The name of this setting (may be empty/null during auto-conversion)</summary>
        public string Name { get; internal set; }

        /// <summary>The value of this setting</summary>
        public object Value { get; private set; }

        /// <summary>Indicates the native storage type of this setting</summary>
        internal SettingType SettingType { get; private set; }

        /// <summary>Default constructor</summary>
        private Setting() { }

        /// <summary>Constructs a new setting</summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The value of the setting</param>
        public Setting(string name, object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Name = name ?? "";

            if (value is string)
            {
                SettingType = SettingType.String;
                Value = value;
            }
            else if (value is int)
            {
                SettingType = SettingType.Integer;
                Value = (long)(int)value;
            }
            else if (value is long)
            {
                SettingType = SettingType.Integer;
                Value = value;
            }
            else if (value is float)
            {
                SettingType = SettingType.FloatingPoint;
                Value = (double)(float)value;
            }
            else if (value is double)
            {
                SettingType = SettingType.FloatingPoint;
                Value = value;
            }
            else if (value is decimal) // TODO make decimal first-rate setting type
            {
                SettingType = SettingType.FloatingPoint;
                Value = (double)(decimal)value;
            }
            else if (value is bool)
            {
                SettingType = SettingType.Boolean;
                Value = value;
            }
        }

        #region Implicit conversions

        // TODO handle out-of-range conversions

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator bool(Setting setting)
        {
            if (setting == null) return false;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value;
                case SettingType.Integer: return (int)setting.Value > 0;
                case SettingType.FloatingPoint: return (double)setting.Value > 0D;
                case SettingType.String:
                    string stringValue = (string)setting.Value;
                    return
                        stringValue.Equals("true", StringComparison.InvariantCultureIgnoreCase) |
                        stringValue.Equals("1", StringComparison.InvariantCultureIgnoreCase) |
                        stringValue.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(bool value) { return new Setting("", value); }

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator long(Setting setting)
        {
            if (setting == null) return DefaultLongValue;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value ? 1L : DefaultLongValue;
                case SettingType.Integer: return (long)setting.Value;
                case SettingType.FloatingPoint: return (long)setting.Value;
                case SettingType.String:
                    if (long.TryParse((string)setting.Value, out long result))
                        return result;
                    else
                        return DefaultLongValue;
            }
            return DefaultLongValue;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(long value) { return new Setting("", value); }

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator int(Setting setting)
        {
            if (setting == null) return DefaultIntValue;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value ? 1 : DefaultIntValue;
                case SettingType.Integer: return (int)setting.Value;
                case SettingType.FloatingPoint: return (int)setting.Value;
                case SettingType.String:
                    if (int.TryParse((string)setting.Value, out int result))
                        return result;
                    else
                        return DefaultIntValue;
            }
            return DefaultIntValue;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(int value) { return new Setting("", value); }

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator double(Setting setting)
        {
            if (setting == null) return DefaultDoubleValue;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value ? 1L : DefaultDoubleValue;
                case SettingType.Integer: return (double)setting.Value;
                case SettingType.FloatingPoint: return (double)setting.Value;
                case SettingType.String:
                    if (double.TryParse((string)setting.Value, out double result))
                        return result;
                    else
                        return DefaultDoubleValue;
            }
            return DefaultDoubleValue;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(double value) { return new Setting("", value); }

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator float(Setting setting)
        {
            if (setting == null) return DefaultFloatValue;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value ? 1 : DefaultFloatValue;
                case SettingType.Integer: return (float)setting.Value;
                case SettingType.FloatingPoint: return (float)setting.Value;
                case SettingType.String:
                    if (float.TryParse((string)setting.Value, out float result))
                        return result;
                    else
                        return DefaultFloatValue;
            }
            return DefaultFloatValue;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(float value) { return new Setting("", value); }

        /// <summary>An implicit conversion</summary>
        /// <param name="setting">The setting to convert</param>
        public static implicit operator string(Setting setting)
        {
            if (setting == null) return DefaultStringValue;

            switch (setting.SettingType)
            {
                case SettingType.Boolean: return (bool)setting.Value ? "true" : "false";
                case SettingType.Integer: return setting.Value.ToString();
                case SettingType.FloatingPoint: return setting.Value.ToString();
                case SettingType.String: return (string)setting.Value;
            }
            return DefaultStringValue;
        }

        /// <summary>An implicit conversion</summary>
        /// <param name="value">The value to convert to a setting</param>
        public static implicit operator Setting(string value) { return new Setting("", value); }

        #endregion Implicit conversions

    }
}
