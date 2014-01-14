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
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;
using System.IO;
using ASC.Web.Mail.Resources;
using ASC.Core;

namespace ASC.Web.Mail
{
    public static class MailXSLTTranslator
    {
        private static XmlDocument Translate(string xsl_input)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xsl_input);

            var current_culture = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();

            XmlNodeList resources = doc.GetElementsByTagName("resource");
            List<XmlElement> nodes_to_remove = new List<XmlElement>();
            foreach (XmlElement resource_xml in resources)
            {
                string resource_name = resource_xml.GetAttribute("name");
                if (resource_name.Length != 0)
                {
                    string translation = MailResource.ResourceManager.GetString(resource_name, current_culture);
                    XmlText text_node = doc.CreateTextNode(translation);
                    resource_xml.ParentNode.InsertBefore(text_node, resource_xml);
                }
                nodes_to_remove.Add(resource_xml);
            }

            foreach (XmlElement resource_xml in nodes_to_remove)
            {
                resource_xml.ParentNode.RemoveChild(resource_xml);
            }

            return doc;
        }


        public static string CreateVariableInitializer(string xsl_input)
        {
            try
            {
                XmlDocument translated_doc = Translate(xsl_input);

                StringBuilder result = new StringBuilder(4096);
                result.AppendLine();
                using (StringReader reader = new StringReader(translated_doc.InnerXml.Replace("\"", "\\\"")))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.AppendLine("\"" + line.Trim() + "\" +\n");
                    }
                }
                result.AppendLine("\"\"");
                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
