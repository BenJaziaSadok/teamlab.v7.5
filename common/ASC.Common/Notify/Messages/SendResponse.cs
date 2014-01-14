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
using ASC.Notify.Model;
using ASC.Notify.Recipients;

namespace ASC.Notify.Messages
{
    [Serializable]
    public class SendResponse
    {
        public SendResponse()
        {
            Result = SendResult.OK;
        }

        public SendResponse(INotifyAction action, IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            Exception = exc;
            Recipient = recipient;
            NotifyAction = action;
        }

        public SendResponse(INotifyAction action, string senderName, IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            SenderName = senderName;
            Exception = exc;
            Recipient = recipient;
            NotifyAction = action;
        }

        public SendResponse(INotifyAction action, string senderName, IRecipient recipient, SendResult sendResult)
        {
            SenderName = senderName;
            Recipient = recipient;
            Result = sendResult;
            NotifyAction = action;
        }

        public SendResponse(INoticeMessage message, string sender, SendResult result)
        {
            NoticeMessage = message;
            SenderName = sender;
            Result = result;
            if (message != null)
            {
                Recipient = message.Recipient;
                NotifyAction = message.Action;
            }
        }

        public SendResponse(INoticeMessage message, string sender, Exception exc)
        {
            NoticeMessage = message;
            SenderName = sender;
            Result = SendResult.Impossible;
            Exception = exc;
            if (message != null)
            {
                Recipient = message.Recipient;
                NotifyAction = message.Action;
            }
        }

        public INoticeMessage NoticeMessage { get; internal set; }

        public INotifyAction NotifyAction { get; internal set; }

        public SendResult Result { get; set; }

        public Exception Exception { get; set; }

        public string SenderName { get; internal set; }

        public IRecipient Recipient { get; internal set; }
    }
}