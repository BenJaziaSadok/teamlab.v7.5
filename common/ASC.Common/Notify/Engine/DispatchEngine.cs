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
using System.Configuration;
using ASC.Notify.Messages;
using log4net;

namespace ASC.Notify.Engine
{
    public class DispatchEngine
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Notify");
        private static readonly ILog logMessages = LogManager.GetLogger("ASC.Notify.Messages");

        private readonly Context context;
        private readonly bool logOnly;


        public DispatchEngine(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");

            this.context = context;
            logOnly = "log".Equals(ConfigurationManager.AppSettings["core.notify.postman"], StringComparison.InvariantCultureIgnoreCase);
            log.DebugFormat("LogOnly: {0}", logOnly);
        }

        public SendResponse Dispatch(INoticeMessage message, string senderName)
        {
            var response = new SendResponse(message, senderName, SendResult.OK);
            if (!logOnly)
            {
                var sender = context.NotifyService.GetSender(senderName);
                if (sender != null)
                {
                    response = sender.DirectSend(message);
                }
                else
                {
                    response = new SendResponse(message, senderName, SendResult.Impossible);
                }

                LogResponce(message, response, sender != null ? sender.SenderName : string.Empty);
            }
            LogMessage(message, senderName);
            return response;
        }
        
        private void LogResponce(INoticeMessage message, SendResponse response, string senderName)
        {
            var logmsg = string.Format("[{0}] sended to [{1}] over {2}, status: {3} ", message.Subject, message.Recipient, senderName, response.Result);
            if (response.Result == SendResult.Inprogress)
            {
                log.Debug(logmsg, response.Exception);
            }
            else if (response.Result == SendResult.Impossible)
            {
                log.Error(logmsg, response.Exception);
            }
            else
            {
                log.Debug(logmsg);
            }
        }
        
        private void LogMessage(INoticeMessage message, string senderName)
        {
            try
            {
                if (logMessages.IsDebugEnabled)
                {
                    logMessages.DebugFormat("[{5}]->[{1}] by [{6}] to [{2}] at {0}\r\n\r\n[{3}]\r\n{4}\r\n{7}",
                        DateTime.Now,
                        message.Recipient.Name,
                        0 < message.Recipient.Addresses.Length ? message.Recipient.Addresses[0] : string.Empty,
                        message.Subject,
                        (message.Body ?? string.Empty).Replace(Environment.NewLine, Environment.NewLine + @"   "),
                        message.Action,
                        senderName,
                        new string('-', 80));
                }
            }
            catch { }
        }
    }
}