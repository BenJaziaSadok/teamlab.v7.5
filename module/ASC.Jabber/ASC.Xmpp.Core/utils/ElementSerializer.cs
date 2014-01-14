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

#region using

using System.Text;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.utils
{
    public class ElementSerializer
    {
        public static string SerializeElement(Node element)
        {
            return element.ToString(Encoding.UTF8);
        }

        public static T DeSerializeElement<T>(string serialized) where T : class
        {
            var doc = new Document();
            doc.LoadXml(string.Format("<root>{0}</root>", serialized));
            return doc.RootElement.FirstChild as T;
        }
    }
}