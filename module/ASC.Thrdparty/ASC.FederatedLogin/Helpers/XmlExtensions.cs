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

using System.Xml.XPath;

namespace ASC.FederatedLogin.Helpers
{
    public static class XmlExtensions
    {
        public static string SelectNodeValue(this XPathNavigator nav, string xpath)
        {
            var node = nav.SelectSingleNode(xpath);
            return node!=null ? node.Value : string.Empty;
        }
    }
}