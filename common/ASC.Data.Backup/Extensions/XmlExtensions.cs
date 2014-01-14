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

using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ASC.Data.Backup.Extensions
{
    public static class XmlExtensions
    {
        public static string ValueOrDefault(this XElement el)
        {
            return el != null ? el.Value : null;
        }

        public static string ValueOrDefault(this XAttribute attr)
        {
            return attr != null ? attr.Value : null;
        }

        public static void WriteTo(this XElement el, Stream stream)
        {
            WriteTo(el, stream, Encoding.UTF8);
        }

        public static void WriteTo(this XElement el, Stream stream, Encoding encoding)
        {
            var data = encoding.GetBytes(el.ToString(SaveOptions.None));
            stream.Write(data, 0, data.Length);
        }
    }
}
