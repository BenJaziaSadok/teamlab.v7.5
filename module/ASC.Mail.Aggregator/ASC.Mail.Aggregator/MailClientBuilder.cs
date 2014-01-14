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
using System.Web.Configuration;
using ActiveUp.Net.Common;
using ActiveUp.Net.Mail;
using NLog;

namespace ASC.Mail.Aggregator
{
    

    public class MailClientBuilder
    {
        private enum MailClientType
        {
            Imap,
            Pop3,
            Smtp
        }

        public static Imap4Client Imap()
        {
            return (Imap4Client) BuildMailClient(MailClientType.Imap);
        }

        public static Pop3Client Pop()
        {
            return (Pop3Client)BuildMailClient(MailClientType.Pop3);
        }

        public static SmtpClient Smtp()
        {
            return (SmtpClient)BuildMailClient(MailClientType.Smtp);
        }

        private static BaseProtocolClient BuildMailClient(MailClientType type)
        {
            BaseProtocolClient client = null;
            switch (type)
            {
                case MailClientType.Imap: 
                    client = new Imap4Client();
                    break;
                case MailClientType.Pop3: 
                    client = new Pop3Client();
                    break;
                case MailClientType.Smtp: 
                    client = new SmtpClient();
                    break;
                default:
                    throw new ArgumentException(String.Format("Unknown client type: {0}", type));
            }

            try
            {
                client.SendTimeout = Convert.ToInt32(WebConfigurationManager.AppSettings["mail.SendTcpTimeout"]);
                client.ReceiveTimeout = Convert.ToInt32(WebConfigurationManager.AppSettings["mail.RecieveTcpTimeout"]);
            }
            catch (Exception e)
            {
                client.ReceiveTimeout = 30000;
                client.SendTimeout = 30000;

                var logger = LogManager.GetLogger("MailBoxManager");
                var message = String.Format("Problems with config parsing for SendTimeout: {0} or RecieveTimeout: {1}. Values was reseted to default - 30000.\n",
                        WebConfigurationManager.AppSettings["mail.SendTcpTimeout"],
                        WebConfigurationManager.AppSettings["mail.RecieveTcpTimeout"]);
                logger.DebugException(message, e);
            }

            return client;
        }
    }
}
