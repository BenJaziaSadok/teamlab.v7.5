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
using System.Web;
using System.Text;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;

using ASC.Common.Notify.Patterns;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

using Textile;
using Textile.Blocks;


namespace ASC.Notify.Textile
{
    public class TextileStyler : IPatternStyler
    {
        private static readonly Regex VelocityArguments = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);


        static TextileStyler()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ASC.Notify.Textile.Resources.style.css"))
            using (var reader = new StreamReader(stream))
            {
                BlockAttributesParser.Styler = new StyleReader(reader.ReadToEnd());
            }
        }

        public void ApplyFormating(NoticeMessage message)
        {
            var output = new StringBuilderTextileFormatter();
            var formatter = new TextileFormatter(output);

            if (!string.IsNullOrEmpty(message.Subject))
            {
                message.Subject = VelocityArguments.Replace(message.Subject, m => m.Result("${arg}"));
            }

            if (!string.IsNullOrEmpty(message.Body))
            {
                formatter.Format(message.Body);

                var logoMail = ConfigurationManager.AppSettings["web.logo.mail"];
                var logo = string.IsNullOrEmpty(logoMail) ? "http://cdn.teamlab.com/media/newsletters/images/header_04.jpg" : logoMail;
                message.Body = Resources.TemplateResource.HtmlMaster.Replace("%CONTENT%", output.GetFormattedText()).Replace("%LOGO%", logo);

                var footer = message.GetArgument("WithPhoto");
                var res = String.Empty;
                if (footer != null)
                {                   
                    switch ((string)footer.Value)
                    {
                        case "photo":
                            res = Resources.TemplateResource.FooterWithPhoto;
                            break;
                        case "links":
                            res = Resources.TemplateResource.FooterWithLinks;
                            break;
                        default:
                            res = String.Empty;
                            break;
                    }                   
                }
                message.Body = message.Body.Replace("%FOOTER%", res);

                var mail = message.Recipient.Addresses.FirstOrDefault(r => r.Contains("@"));
                var domain = ConfigurationManager.AppSettings["web.teamlab-site"];
                var site = string.IsNullOrEmpty(domain) ? "http://teamlab.com" : domain;
                var link = site + string.Format("/Unsubscribe.aspx?id={0}", HttpServerUtility.UrlTokenEncode(Security.Cryptography.InstanceCrypto.Encrypt(Encoding.UTF8.GetBytes(mail.ToLowerInvariant()))));
                var text = string.Format(Resources.TemplateResource.TextForFooter, link);

                message.Body = message.Body.Replace("%TEXTFOOTER%", text);
            }
        }
    }
}