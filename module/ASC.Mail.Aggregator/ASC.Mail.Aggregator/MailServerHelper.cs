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
using System.Security.Authentication;
using ActiveUp.Net.Common;
using ActiveUp.Net.Mail;

namespace ASC.Mail.Aggregator
{
    public class MailServerSettings
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string AccountName { get; set; }
        public string AccountPass { get; set; }
        public SaslMechanism AuthenticationType { get; set; }
        public EncryptionType EncryptionType { get; set; }
    }

    public static class MailServerHelper
    {
        private const int WAIT_TIMEOUT = 5000;

        static public bool TestSmtp(MailServerSettings settings)
        {
            var smtp = MailClientBuilder.Smtp();
            string s_result = String.Empty;
            try
            {
                IAsyncResult async_res;
                if (settings.EncryptionType == EncryptionType.None || settings.EncryptionType == EncryptionType.StartTLS)
                {
                    async_res = smtp.BeginConnect(settings.Url, settings.Port, null);

                    if (!async_res.AsyncWaitHandle.WaitOne(WAIT_TIMEOUT))
                        throw new SmtpConnectionException(MailQueueItem.CONNECTION_TIMEOUT_ERROR);

                    if (settings.AuthenticationType != SaslMechanism.None || settings.EncryptionType == EncryptionType.StartTLS)
                        smtp.SendEhloHelo();

                    if (settings.EncryptionType == EncryptionType.StartTLS)
                        smtp.StartTLS(settings.Url);

                    if (settings.AuthenticationType != SaslMechanism.None)
                    {
                        s_result = smtp.Authenticate(settings.AccountName, settings.AccountPass, settings.AuthenticationType);
                    }
                }
                else
                {
                    async_res = smtp.BeginConnectSsl(settings.Url, settings.Port, null);

                    if (!async_res.AsyncWaitHandle.WaitOne(WAIT_TIMEOUT))
                        throw new SmtpConnectionException(MailQueueItem.CONNECTION_TIMEOUT_ERROR);

                    if (settings.AuthenticationType != SaslMechanism.None)
                    {
                        s_result = smtp.Authenticate(settings.AccountName, settings.AccountPass, settings.AuthenticationType);
                    }
                }

                if (settings.AuthenticationType != SaslMechanism.None && !s_result.StartsWith("+"))
                    throw new SmtpConnectionException(s_result);

                return true;
            }
            finally
            {
                if (smtp.IsConnected) smtp.Disconnect();
            }
        }

        static public bool TryTestSmtp(MailServerSettings settings, out string last_error)
        {
            try
            {
                last_error = String.Empty;
                return TestSmtp(settings);
            }
            catch (Exception ex)
            {
                last_error = ex.Message;
                return false;
            }
        }

        static public bool Test(BaseProtocolClient ingoing_mail_client, MailServerSettings settings)
        {
            try
            {
                IAsyncResult async_res;
                switch (settings.EncryptionType)
                {
                    case EncryptionType.StartTLS:
                        async_res = ingoing_mail_client.BeginConnect(settings.Url, settings.Port, null);
                        break;
                    case EncryptionType.SSL:
                        async_res = ingoing_mail_client.BeginConnectSsl(settings.Url, settings.Port, null);
                        break;
                    default:
                        async_res = ingoing_mail_client.BeginConnect(settings.Url, settings.Port, null);
                        break;
                }

                if (!async_res.AsyncWaitHandle.WaitOne(WAIT_TIMEOUT))
                    throw new ImapConnectionException(MailQueueItem.CONNECTION_TIMEOUT_ERROR);

                if (settings.EncryptionType == EncryptionType.StartTLS)
                {
                    ingoing_mail_client.StartTLS(settings.Url);
                }

                if (settings.AuthenticationType == SaslMechanism.Login)
                {
                    ingoing_mail_client.Login(settings.AccountName, settings.AccountPass, "");
                }
                else
                {
                    async_res = ingoing_mail_client.BeginAuthenticate(settings.AccountName, settings.AccountPass, settings.AuthenticationType, null);
                }

                if (!async_res.AsyncWaitHandle.WaitOne(WAIT_TIMEOUT))
                    throw new ImapConnectionException(MailQueueItem.CONNECTION_TIMEOUT_ERROR);

                if (async_res.AsyncState == null)
                    throw new AuthenticationException("Auth failed. Check your settings.");

                string s_result = ingoing_mail_client.EndConnectSsl(async_res).ToLowerInvariant();

                if (s_result.IndexOf("success", StringComparison.Ordinal) == -1 &&
                    s_result.IndexOf("+", StringComparison.Ordinal) == -1 &&
                    s_result.IndexOf("ok", StringComparison.Ordinal) == -1)
                    throw new ImapConnectionException(s_result);

                return true;
            }
            finally
            {
                if (ingoing_mail_client.IsConnected)
                {
                    ingoing_mail_client.Disconnect();
                }
            }
        }

        static public bool TryTestImap(MailServerSettings settings, out string last_error)
        {
            try
            {
                last_error = String.Empty;
                return Test(MailClientBuilder.Imap(), settings);
            }
            catch (Exception ex)
            {
                last_error = ex.Message;
                return false;
            }
        }

        public static bool TryTestPop(MailServerSettings settings, out string last_error)
        {
            try
            {
                last_error = String.Empty;
                return Test(MailClientBuilder.Pop(), settings);
            }
            catch (Exception ex)
            {
                last_error = ex.Message;
                return false;
            }
        }
    }
}
