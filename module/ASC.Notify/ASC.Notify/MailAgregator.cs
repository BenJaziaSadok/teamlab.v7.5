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
using System.Text;
using ASC.Notify.Messages;
using System.Threading;
using System.Net.Mail;
using log4net;
using System.ComponentModel;
using System.Configuration;
using ASC.Common.Utils;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace ASC.Notify
{
    class MailAgregator
    {
        private SmtpClient clientSMTP = new SmtpClient();
        private readonly ILog log = LogManager.GetLogger(typeof(NotifyService));
        private const string HtmlForm =
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
<html>
<head>
  <meta content=""text/html;charset=UTF-8"" http-equiv=""Content-Type"">
</head>
<body>{0}</body>
</html>";

        public MailAgregator()
        {
        }

        #region SMTPclient
        internal void SendBySmtpClient(NotifyMessage m)
        {
            using (var message = BuildMailMessage(m))
            {
                clientSMTP.Send(message);
            }
        }
        #endregion


        #region AmazonSES
        internal void SendByAmazonSES(NotifyMessage m)
        {
            using (var message = BuildMailMessage(m))
            {
                var awsEmail = new AWSEmail();
                awsEmail.SendEmail(message);
            }
        }
        #endregion AmazonSES

        
        private MailMessage BuildMailMessage(NotifyMessage message)
        {
            var email = new MailMessage
            {
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                From = new MailAddress(message.From),
            };

            var recipients = message.To.Split('|');
            foreach (string address in recipients)
            {
                email.To.Add(address);
            }

            email.ReplyTo = message.ReplayTo!=null ? new MailAddress(message.ReplayTo) : email.From;

            email.Subject = message.Subject.Trim(' ', '\t', '\n', '\r');

            if (message.ContentType == "html")
            {
                email.Body = HtmlUtil.GetText(message.Content);
                var html = String.Format(HtmlForm, message.Content);
                var alternate = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, "text/html");
                email.AlternateViews.Add(alternate);
            }
            else
            {
                email.Body = message.Content;
            }

            return email;
        }
    }
}
