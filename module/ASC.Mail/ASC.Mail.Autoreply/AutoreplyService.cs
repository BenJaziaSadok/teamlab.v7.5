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
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Mail.Autoreply.AddressParsers;
using ASC.Mail.Net;
using ASC.Mail.Net.MIME;
using ASC.Mail.Net.Mail;
using ASC.Mail.Net.SMTP.Server;
using ASC.Mail.Net.TCP;
using log4net;

namespace ASC.Mail.Autoreply
{
    internal class AutoreplyService : IDisposable
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(AutoreplyService));

        private readonly List<IAddressParser> _addressParsers = new List<IAddressParser>();

        private readonly SMTP_Server _smtpServer;
        private readonly ApiService _apiService;
        private readonly CooldownInspector _cooldownInspector;
        private readonly bool _storeIncomingMail;
        private readonly string _mailFolder;

        public AutoreplyService(AutoreplyServiceConfiguration configuration = null)
        {
            configuration = configuration ?? AutoreplyServiceConfiguration.GetSection();

            _storeIncomingMail = configuration.IsDebug;

            _mailFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), configuration.MailFolder);
            if (!Directory.Exists(_mailFolder))
                Directory.CreateDirectory(_mailFolder);

            _smtpServer = new SMTP_Server
                {
                    MaxBadCommands = configuration.SmtpConfiguration.MaxBadCommands,
                    MaxTransactions = configuration.SmtpConfiguration.MaxTransactions,
                    MaxMessageSize = configuration.SmtpConfiguration.MaxMessageSize,
                    MaxRecipients = configuration.SmtpConfiguration.MaxRecipients,
                    MaxConnectionsPerIP = configuration.SmtpConfiguration.MaxConnectionsPerIP,
                    MaxConnections = configuration.SmtpConfiguration.MaxConnections,
                    Bindings = new[] {new IPBindInfo("localhost", IPAddress.Any, configuration.SmtpConfiguration.Port, SslMode.None, null)},
                };

            _smtpServer.Error += OnSmtpError;
            _smtpServer.SessionCreated += OnSmtpSessionCreated;

            _cooldownInspector = new CooldownInspector(configuration.CooldownConfiguration);

            _apiService = new ApiService(configuration.Https);
        }

        public void Start()
        {
            _smtpServer.Start();
            _apiService.Start();
            _cooldownInspector.Start();
        }

        public void Stop()
        {
            _smtpServer.Stop();
            _apiService.Stop();
            _cooldownInspector.Stop();
        }

        public void Dispose()
        {
            Stop();
            _smtpServer.Dispose();
        }

        public void RegisterAddressParser(IAddressParser addressHandler)
        {
            _addressParsers.Add(addressHandler);
        }

        private void OnSmtpSessionCreated(object sender, TCP_ServerSessionEventArgs<SMTP_Session> e)
        {
            e.Session.Started += (sndr, args) => _log.DebugFormat("session started: {0}", e.Session);
            e.Session.MailFrom += OnSessionMailFrom;
            e.Session.RcptTo += OnSessionRcptTo;
            e.Session.GetMessageStream += OnSessionGetMessageStream;
            e.Session.MessageStoringCanceled += OnSessionMessageStoringCancelled;
            e.Session.MessageStoringCompleted += OnSessionMessageStoringCompleted;
        }

        private void OnSmtpError(object sender, Error_EventArgs e)
        {
            _log.WarnFormat("smtp error: {0}", e.Exception);
        }

        private void OnSessionMailFrom(object sender, SMTP_e_MailFrom e)
        {
            e.Session.Tag = Regex.Replace(e.MailFrom.Mailbox, "^prvs=[0-9a-zA-Z]+=", "", RegexOptions.Compiled); //Set session mailbox
        }

        private void OnSessionRcptTo(object sender, SMTP_e_RcptTo e)
        {
            try
            {
                _log.Debug("start processing rcpt to event");

                var addressTo = e.RcptTo.Mailbox;
                var addressFrom = (string)e.Session.Tag;

                var requestInfo = _addressParsers.Select(routeParser => routeParser.ParseRequestInfo(addressTo))
                                                 .FirstOrDefault(rInfo => rInfo != null);

                if (requestInfo == null)
                {
                    _log.WarnFormat("could not create request from the address {0}", addressTo);
                    e.Reply = new SMTP_Reply(501, "Could not create request from the address " + addressTo);
                    return;
                }

                CoreContext.TenantManager.SetCurrentTenant(requestInfo.Tenant);

                UserInfo user = CoreContext.UserManager.GetUserByEmail(addressFrom);

                if (user.Equals(Constants.LostUser))
                {
                    e.Reply = new SMTP_Reply(501, "Could not find user by email address " + addressFrom);
                    return;
                }

                if (_cooldownInspector != null)
                {
                    var cooldownMinutes = Math.Ceiling(_cooldownInspector.GetCooldownRemainigTime(user.ID).TotalMinutes);
                    if (cooldownMinutes > 0)
                    {
                        e.Reply = new SMTP_Reply(554, string.Format("User {0} can not use the autoreply service for another {1} minutes", addressFrom, cooldownMinutes));
                        return;
                    }

                    _cooldownInspector.RegisterServiceUsage(user.ID);
                }

                requestInfo.User = user;

                _log.DebugFormat("created request info {0}", requestInfo);

                e.Session.Tags.Add(e.RcptTo.Mailbox, requestInfo);

                _log.Debug("complete processing rcpt to event");
            }
            catch (Exception error)
            {
                _log.Error("RCPT TO error", error);
            }
        }

        private void OnSessionGetMessageStream(object sender, SMTP_e_Message e)
        {
            try
            {
                string messageFileName;
                do
                {
                    messageFileName = Path.Combine(_mailFolder, DateTime.UtcNow.ToString("yyyy'-'MM'-'dd HH'-'mm'-'ss'Z'") + ".eml");
                } while (File.Exists(messageFileName));
                e.Stream = new FileStream(messageFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 8096, _storeIncomingMail ? FileOptions.None : FileOptions.DeleteOnClose);
            }
            catch (Exception error)
            {
                _log.Error("error while allocating temporary stream for the message", error);
            }
        }

        private void OnSessionMessageStoringCancelled(object sender, SMTP_e_MessageStored e)
        {
            CloseStream(e.Stream);
        }

        private void OnSessionMessageStoringCompleted(object sender, SMTP_e_MessageStored e)
        {
            try
            {
                _log.Debug("begin processing message storing completed event");

                e.Stream.Flush();
                e.Stream.Seek(0, SeekOrigin.Begin);

                Mail_Message message = Mail_Message.ParseFromStream(e.Stream);
                message.Subject = Regex.Replace(message.Subject, @"\t", "");

                foreach (var requestInfo in e.Session.To
                                             .Where(x => e.Session.Tags.ContainsKey(x.Mailbox))
                                             .Select(x => (ApiRequest)e.Session.Tags[x.Mailbox]))
                {
                    try
                    {
                        _log.Debug("begin process request (" + requestInfo + ")");

                        CoreContext.TenantManager.SetCurrentTenant(requestInfo.Tenant);

                        if (requestInfo.Parameters != null)
                        {
                            foreach (var parameter in requestInfo.Parameters.Where(x => x.ValueResolver != null))
                            {
                                parameter.Value = parameter.ValueResolver.ResolveParameterValue(message);
                            }
                        }
                        if (requestInfo.FilesToPost != null)
                        {
                            requestInfo.FilesToPost = message.AllEntities.Where(IsAttachment).Select(GetAsAttachment).ToList();    
                        }
                        
                        if (requestInfo.FilesToPost == null || requestInfo.FilesToPost.Count > 0)
                        {
                            _apiService.EnqueueRequest(requestInfo);   
                        }

                        _log.Debug("end process request (" + requestInfo + ")");
                    }
                    catch (Exception ex)
                    {
                        _log.Error("error while processing request info", ex);
                    }
                }

                _log.Debug("complete processing message storing completed event");
            }
            catch (Exception error)
            {
                _log.Error("error while processing message storing completed event", error);
            }
            finally
            {
                CloseStream(e.Stream);
            }
        }

        private static bool IsAttachment(MIME_Entity entity)
        {
            return entity.Body.ContentType != null && entity.Body.ContentType.TypeWithSubype != null
                   && entity.ContentDisposition != null && entity.ContentDisposition.DispositionType == MIME_DispositionTypes.Attachment
                   && (!string.IsNullOrEmpty(entity.ContentDisposition.Param_FileName) || !string.IsNullOrEmpty(entity.ContentType.Param_Name))
                   && entity.Body is MIME_b_SinglepartBase;
        }

        private static RequestFileInfo GetAsAttachment(MIME_Entity entity)
        {
            var attachment = new RequestFileInfo
                {
                    Body = ((MIME_b_SinglepartBase)entity.Body).Data
                };

            if (!string.IsNullOrEmpty(entity.ContentDisposition.Param_FileName))
            {
                attachment.Name = entity.ContentDisposition.Param_FileName;
            }
            else if (!string.IsNullOrEmpty(entity.ContentType.Param_Name))
            {
                attachment.Name = entity.ContentType.Param_Name;
            }

            attachment.ContentType = MimeMapping.GetMimeMapping(attachment.Name);

            return attachment;
        }

        private void CloseStream(Stream stream)
        {
            try
            {
                if (stream != null)
                    stream.Close();
            }
            catch (Exception error)
            {
                _log.Error("error while closing the stream", error);
            }
        }
    }
}