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
using ASC.Mail.Autoreply.Utility;
using ASC.Mail.Autoreply.Utility.Html;
using ASC.Mail.Net.Mail;
using HtmlAgilityPack;

namespace ASC.Mail.Autoreply.ParameterResolvers
{
    internal class HtmlContentResolver : IParameterResolver
    {
        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            var messageText = !string.IsNullOrEmpty(mailMessage.BodyHtmlText)
                                  ? mailMessage.BodyHtmlText
                                  : Text2HtmlConverter.Convert(mailMessage.BodyText.Trim(' '));

            messageText = messageText.Replace(Environment.NewLine, "").Replace(@"\t", "");
            messageText = HtmlEntity.DeEntitize(messageText);
            messageText = HtmlSanitizer.Sanitize(messageText);

            return messageText.Trim("<br>").Trim("</br>").Trim(' ');
        }
    }

    internal class PlainTextContentResolver : IParameterResolver
    {
        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            var messageText = new HtmlContentResolver().ResolveParameterValue(mailMessage) as string;

            if (!string.IsNullOrEmpty(messageText))
                messageText = Html2TextConverter.Convert(messageText);

            return messageText;
        }
    }
}
