/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */


namespace System
{
    public static class EnumExtension
    {
        public static T TryParseEnum<T>(this Type enumType, string value, T defaultValue) where T : struct
        {
            bool isDefault;
            return TryParseEnum<T>(enumType, value, defaultValue, out isDefault);            
        }

        public static T TryParseEnum<T>(this Type enumType, string value, T defaultValue, out bool isDefault) where T : struct
        {
            isDefault = false;
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                isDefault = true;
                return defaultValue;
            }
        }
    }
}
