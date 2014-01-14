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
using System.Text;
using ActiveUp.Net.Mail;
using System.IO;
using ASC.Mail.Aggregator.Extension;
using System.Diagnostics;
using ASC.Mail.Aggregator.Authorization;
using System.Text.RegularExpressions;
using System.Linq;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        public static void TestPrepareMessage()
        {
            var charset = Encoding.UTF8.HeaderName;
            const string text = "тест";
            const string html = "<a href='www.teamlab.com'>" + text + "</a>";

            var message = new Message
                {
                    From = {Email = "test.mail@qip.ru", Name = Codec.RFC2047Encode(text)}
                };

            message.To.Add("test.mail@qip.ru", Codec.RFC2047Encode(text));

            message.Subject = Codec.RFC2047Encode(text);

            message.BodyText.Charset = charset;
            message.BodyText.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            message.BodyText.Text = text;
            message.BodyHtml.Charset = charset;
            message.BodyHtml.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            message.BodyHtml.Text = html;

            message.StoreToFile(@"test_send_prepared.eml");
        }

        public static MailMessageItem TestParseMessage(string eml_path)
        {
            var w = new Stopwatch();

            w.Start();

            var m =
                Parser.ParseMessageFromFile(eml_path);

            w.Stop();

            Console.WriteLine("1 Parser takes {0} seconds", w.Elapsed.TotalSeconds);

            w.Reset();
            w.Start();
            
            var mail_info = new MailMessageItem(m);

            w.Stop();

            Console.WriteLine("2 Parser takes {0} seconds", w.Elapsed.TotalSeconds);

            return mail_info;
        }

        public static void TestSanitizeHtmlBody(string eml_path, bool load_images)
        {
            var mail_info = TestParseMessage(eml_path);
            var html_body = HtmlSanitizer.Sanitize(mail_info.HtmlBody, load_images);
            _log.Info(html_body);
        }


        private static Pop3Client TestGetPop3Client(string pop3_server, string pop3_account, string pop3_password, int pop3_port, bool use_ssl)
        {
            var pop = MailClientBuilder.Pop();

            Debug.WriteLine("Connect to Pop3:");
            
            var watch = new Stopwatch();
            watch.Start();

            var result = use_ssl ? 
                                pop.ConnectSsl(pop3_server, pop3_port, pop3_account, pop3_password) : 
                                pop.Connect(pop3_server, pop3_port, pop3_account, pop3_password);

            watch.Stop();

            Debug.WriteLine("Elapsed Connect pop: " + watch.ElapsedMilliseconds);

            if (!result.StartsWith("+"))
                throw new Exception("Bad connection result: " + result);

            return pop;
        }

        public static List<string> TestGetPopHeaderMD5(string pop3_server, string pop3_account, string pop3_password, int pop3_port, bool use_ssl)
        {
            var headers_md5 = new List<string>();

            var pop = MailClientBuilder.Pop();
            Debug.WriteLine("Pop3:");
            try
            {

                pop = TestGetPop3Client(pop3_server, pop3_account, pop3_password, pop3_port, use_ssl);

                var watch = new Stopwatch();

                watch.Start();

                for (var i = 1; i <= pop.MessageCount; i++)
                {
                    try
                    {
                        var header = pop.RetrieveHeaderObject(i);
                        var unique_identifier = string.Format("{0}|{1}|{2}|{3}", header.From.Email, header.Subject, header.DateString, header.MessageId);
                        var md5 = unique_identifier.GetMD5();
                        headers_md5.Add(md5);
                        Debug.WriteLine("# " + i + "MD5: " + md5);
                        Debug.WriteLine("unique_identifier: " + unique_identifier);
                    }
                    catch
                    { //SKEEP RetrieveHeader ERROR
                        if (!pop.IsConnected)
                            break;
                    }
                }

                watch.Stop();

                Debug.WriteLine("Elapsed Get pop md5: " + watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (pop != null && pop.IsConnected)
                    pop.Disconnect();
            }

            return headers_md5;
        }

        public static void TestSendMessageFromFile(string smtp_server, string smtp_account, string smtp_password, int smtp_port, 
            bool smtp_auth, bool use_ssl, string eml_path, string to_address)
        {
            try
            {

                var message = Parser.ParseMessageFromFile(eml_path);

                message.To = new AddressCollection {to_address};

                if (!use_ssl)
                {
                    if (!smtp_auth)
                        SmtpClient.Send(message, smtp_server, smtp_port);
                    else
                        SmtpClient.Send(message, smtp_server, smtp_port, smtp_account, smtp_password, SaslMechanism.Login);
                }
                else
                {
                    if (!smtp_auth)
                        SmtpClient.SendSsl(message, smtp_server, smtp_port);
                    else
                        SmtpClient.SendSsl(message, smtp_server, smtp_port, smtp_account, smtp_password, SaslMechanism.Login);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static List<string> TestGetImapHeaderMD5(string imap_server, string imap_account, string imap_password, int imap_port, bool use_ssl)
        {
            var headers_md5 = new List<string>();

            var imap = MailClientBuilder.Imap();
            Debug.WriteLine("Imap:");
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                if (use_ssl)
                {
                    imap.ConnectSsl(imap_server, imap_port);
                }
                else
                {
                    imap.Connect(imap_server, imap_port);
                }

                watch.Stop();

                Debug.WriteLine("Elapsed Connect imap: " + watch.ElapsedMilliseconds);

                watch.Reset();
                watch.Start();

                imap.LoginFast(imap_account, imap_password, "");

                watch.Stop();

                Debug.WriteLine("Elapsed LoginFast imap: " + watch.ElapsedMilliseconds);

                watch.Reset();
                watch.Start();

                const string folder_name = "inbox";
                
                var mb = imap.ExamineMailbox(folder_name);
                
                watch.Stop();

                Debug.WriteLine("Elapsed ExamineMailbox imap: " + watch.ElapsedMilliseconds);

                watch.Reset();
                watch.Start();

                for (var i = 1; i <= mb.MessageCount; i++)
                {
                    var header = mb.Fetch.HeaderLinesPeek(i, new[] { "Date", "Subject", "Message-ID", "From" });
                    var unique_identifier = string.Format("{0}|{1}|{2}|{3}", header["Date"], header["Subject"], header["Message-ID"], header["From"]);
                    var md5 = unique_identifier.GetMD5();
                    headers_md5.Add(md5);
                    Debug.WriteLine("# " + i + "MD5: " + md5);
                    Debug.WriteLine("unique_identifier: " + unique_identifier);
                }
                watch.Stop();

                Debug.WriteLine("Elapsed Get imap md5: " + watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (imap.IsConnected)
                    imap.Disconnect();
            }

            return headers_md5;
        }

        public static void TestPOPReceiveMessage(string pop3_server, string pop3_account, string pop3_password, int pop3_port, bool use_ssl, string uidl)
        {
            var pop = MailClientBuilder.Pop();
            try
            {
                _log.Debug("Connecting to {0}", pop3_account);

                var result = use_ssl ? pop.ConnectSsl(pop3_server, pop3_port, pop3_account, pop3_password) : 
                                    pop.Connect(pop3_server, pop3_port, pop3_account, pop3_password);

                if (!result.StartsWith("+")) return;
                if (pop.UniqueIdExists(uidl))
                {
                    _log.Info("Message with this uidl exists!");

                    var index = pop.GetMessageIndex(uidl);

                    _log.Info("StoreMessage(index: {0})", index);

                    var destination_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"strange.eml");

                    pop.UpdateStats();

                    //pop.StoreMessage(index, false, destination_path);

                    var message = pop.RetrieveMessageObject(index);

                    var mail_info = new MailMessageItem(message);

                    _log.Info(mail_info.HtmlBody);

                    if (File.Exists(destination_path))
                        _log.Info("Message stored successfully!\r\n");
                    else
                        _log.Error("Message is missing in destination path!\r\n");
                }
                else
                    _log.Info("Message with this uidl not exists!\r\n");
            }
            catch (Pop3Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    if (pop.IsConnected)
                    {
                        pop.Disconnect();
                    }
                }
                catch
                { }
            }
        }

        private static readonly Regex _imapUidlRegx = new Regex("(\\d+).*?(\\d+).*?(\\d+)");

        public static void TestIMAPReceiveMessage(string imap_server, string imap_account, string imap_password, int imap_port, bool use_ssl, string uidl)
        {
            var imap = MailClientBuilder.Imap();
            try
            {
                _log.Debug("Connecting to {0}", imap_account);

                var match = _imapUidlRegx.Match(uidl);

                if (!match.Success)
                {
                    _log.Error("Bad UIDL");
                    return;
                }

                var uid = Convert.ToInt32(match.Groups[1].Value);
                var folder = Convert.ToInt32(match.Groups[2].Value);
                var uid_validity = Convert.ToInt32(match.Groups[3].Value);

                if (use_ssl)
                    imap.ConnectSsl(imap_server, imap_port);
                else
                    imap.Connect(imap_server, imap_port);

                imap.Login(imap_account, imap_password, "");

                var folders = MailQueueItem.GetImapMailboxes(imap, imap_server);

                var founded = (from n in folders
                        where n.folder_id == folder
                        select n)
                        .FirstOrDefault();

                if (string.IsNullOrEmpty(founded.name))
                {
                    _log.Error("Bad UIDL folder");
                    return;
                }

                var mb = imap.SelectMailbox(founded.name);

                if (mb.UidValidity != uid_validity)
                {
                    _log.Error("Bad UID_VALIDITY");
                    return;
                }

                var message = mb.Fetch.UidMessageObject(uid);

                var mail_info = new MailMessageItem(message);

// ReSharper disable UnusedVariable
                var sanitazed = HtmlSanitizer.Sanitize(mail_info.HtmlBody, true);
// ReSharper restore UnusedVariable
            }
            catch (Pop3Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                try
                {
                    if (imap.IsConnected)
                    {
                        imap.Disconnect();
                    }
                }
                catch
                { }
            }
        }

        public static void TestReceiveMessageImap()
        {
            var imap = MailClientBuilder.Imap();
            try
            {
                const string imap_server = "imap.googlemail.com";
                const int imap_port = 993;
                const string imap_account = "profi.troll.4test@gmail.com";
                const string imap_password = "Isadmin123";

                imap.ConnectSsl(imap_server, imap_port);
                imap.Login(imap_account, imap_password, "");

                if (!imap.IsConnected) return;
                var inbox = imap.SelectMailbox("inbox");
                if (inbox.MessageCount > 0)
                {
                    var header = inbox.Fetch.HeaderObject(1);

                    Console.WriteLine("Subject: {0} From :{1} ", header.Subject, header.From.Email);
                }
                else
                {
                    Console.WriteLine("There is no message in the imap4 account");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (imap.IsConnected)
                {
                    imap.Disconnect();
                }
            }
        }

        public static bool TestPop3Connect(string pop3_server, string pop3_account, string pop3_password, int pop3_port, bool use_ssl)
        {
            try
            {
                var pop3_client = TestGetPop3Client(pop3_server, pop3_account, pop3_password, pop3_port, use_ssl);

                if (pop3_client != null && pop3_client.IsConnected)
                {
                    pop3_client.Disconnect();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public static void TestGoogleImapLoginViaOAuth2(string account, string refresh_token)
        {
            var authorizatior = new GoogleOAuth2Authorization();

            var granted_access = authorizatior.RequestAccessToken(refresh_token);

            if (granted_access == null) return;
            var imap = MailClientBuilder.Imap();
            imap.ConnectSsl("imap.googlemail.com", 993);
            imap.LoginOAuth2(account, granted_access.AccessToken);
            //Do some work...
            imap.Disconnect();
        }

        public static void TestGoogleSmtpLoginViaOAuth2(string account, string refresh_token)
        {
            var authorizatior = new GoogleOAuth2Authorization();

            var granted_access = authorizatior.RequestAccessToken(refresh_token);

            if (granted_access == null) return;
            var smtp = MailClientBuilder.Smtp();
            smtp.ConnectSsl("smtp.googlemail.com", 465);
            smtp.Authenticate(account, granted_access.AccessToken, SaslMechanism.OAuth2);
            //Do some work...
            smtp.Disconnect();
        }
    }
}
