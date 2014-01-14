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
using HtmlAgilityPack;

namespace ASC.Mail.Autoreply.Utility.Html
{
    public class HtmlSanitizer
    {
        private static readonly List<string> MaliciousTags = new List<string> { "script", "style" };
        private static readonly List<string> LineBreakers = new List<string> { "p", "div", "blockquote", "br" };  
        private static readonly List<string> WhiteList = new List<string> {"b", "strong", "it", "em", "dfn", "sub", "sup", "strike", "s", "del", "code", "kbd", "samp", "ins", "h1", "h2", "h3", "h4", "h5", "h6"};

        public static String Sanitize(String html)
        {
            if (String.IsNullOrEmpty(html))
                return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            var sw = new StringWriter();
            ProcessNode(doc.DocumentNode, sw);
            sw.Flush();
            return sw.ToString();
        }

        private static void ProcessContent(HtmlNode node, TextWriter outText)
        {
            foreach (var child in node.ChildNodes)
            {
                ProcessNode(child, outText);
            }
        }

        private static void ProcessNode(HtmlNode node, TextWriter outText)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    break;

                case HtmlNodeType.Document:
                    ProcessContent(node, outText);
                    break;

                case HtmlNodeType.Element:
                    var name = node.Name.ToLowerInvariant();

                    if (MaliciousTags.Contains(name))
                        break;

                    if (WhiteList.Contains(name) && node.HasChildNodes && node.Closed)
                    {
                        outText.Write("<{0}>", name);
                        ProcessContent(node, outText);
                        outText.Write("</{0}>", name);
                        break;
                    }

                    if (name.Equals("img") && node.HasAttributes && node.Attributes["src"] != null)
                    {
                        outText.Write("<img src={0}/>", node.Attributes["src"]);
                    }
                    else if (LineBreakers.Contains(name))
                    {
                        outText.Write("<br>");
                    }

                    if (node.HasChildNodes)
                        ProcessContent(node, outText);
                    
                    break;

                case HtmlNodeType.Text:
                    var text = ((HtmlTextNode) node).Text;

                    if (HtmlNode.IsOverlappedClosingElement(text))
                        break;

                    if (text.Trim().Length > 0)
                        outText.Write(text);

                    break;
            }
        }
    }
}
