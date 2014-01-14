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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ASC.Data.Backup.Extensions;

namespace ASC.Data.Backup.Tasks.Data
{
    internal static class DataRowInfoReader
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        public static IEnumerable<DataRowInfo> ReadFromStream(Stream stream)
        {
            var readerSettings = new XmlReaderSettings
                {
                    CheckCharacters = false,
                    CloseInput = false
                };

            using (var xmlReader = XmlReader.Create(stream, readerSettings))
            {
                xmlReader.MoveToContent();
                xmlReader.ReadToFollowing("schema", XmlSchemaNamespace);

                var schema = new Dictionary<string, string>();

                var schemaElement = XNode.ReadFrom(xmlReader) as XElement;
                if (schemaElement != null)
                {
                    foreach (var entry in schemaElement.Descendants(XName.Get("sequence", XmlSchemaNamespace)).Single().Elements(XName.Get("element", XmlSchemaNamespace)))
                    {
                        schema.Add(entry.Attribute("name").ValueOrDefault(), entry.Attribute("type").ValueOrDefault());
                    }
                }

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        var el = XNode.ReadFrom(xmlReader) as XElement;
                        if (el != null)
                        {
                            var dataRowInfo = new DataRowInfo(el.Name.LocalName);
                            foreach (var column in schema)
                            {
                                object value = ConvertToType(el.Element(column.Key).ValueOrDefault(), column.Value);
                                dataRowInfo.InsertItem(column.Key, value);
                            }

                            yield return dataRowInfo;
                        }
                    }
                }
            }
        }

        private static object ConvertToType(string str, string schemaType)
        {
            if (str == null)
            {
                return null;
            }
            if (schemaType == "xs:boolean")
            {
                return Convert.ToBoolean(str);
            }
            if (schemaType == "xs:base64Binary")
            {
                return Convert.FromBase64String(str);
            }
            return str;
        }
    }
}
