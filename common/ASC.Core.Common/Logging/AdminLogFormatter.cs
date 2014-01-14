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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace ASC.Core.Common.Logging
{
    class AdminLogFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg != null && format != null)
            {
                if (format.ToLower() == "json")
                {
                    return SerializeJson(arg);
                }
                if (format.ToLower() == "xml")
                {
                    return SerializeXml(arg);
                }
            }
            return string.Format("{0}", arg);
        }

        private static string SerializeXml(object obj)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            using (var sw = new StringWriter())
            using (var writer = new XmlTextWriter(sw))
            {
                serializer.WriteObject(writer, obj);
                return sw.ToString();
            }
        }

        private static string SerializeJson(object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
