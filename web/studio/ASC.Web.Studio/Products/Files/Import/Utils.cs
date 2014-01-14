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

using System;
using System.Xml.Linq;

namespace ASC.Web.Files.Import
{
    internal static class Utils
    {
        public static DateTime FromUnixTime(long time)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(time);
        }

        public static DateTime FromUnixTime2(long time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

        public static string ElementValueOrDefault(this XElement element, XName name)
        {
            return ElementValueOrDefault(element, name, string.Empty);
        }

        public static string ElementValueOrDefault(this XElement element, XName name, string defaultValue)
        {
            var value = element.Element(name);
            return value != null ? value.Value : defaultValue;
        }

        public static string AttributeValueOrDefault(this XElement element, XName name)
        {
            return AttributeValueOrDefault(element, name, string.Empty);
        }

        public static string AttributeValueOrDefault(this XElement element, XName name, string defaultValue)
        {
            var attribute = element.Attribute(name);
            return attribute != null ? attribute.Value : defaultValue;
        }
    }
}