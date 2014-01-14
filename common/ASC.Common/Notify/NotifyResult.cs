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

using System.Collections.Generic;
using System.Text;
using ASC.Notify.Messages;

namespace ASC.Notify
{
    public class NotifyResult
    {
        public SendResult Result { get; internal set; }

        public List<SendResponse> Responses { get; set; }


        internal NotifyResult(SendResult result, List<SendResponse> responses)
        {
            Result = result;
            Responses = responses ?? new List<SendResponse>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SendResult: {0} whith {1} sub-results", Result, Responses.Count);
            foreach (SendResponse responce in Responses)
            {
                string recipient = "<recipient:nomessage>";
                string error = "";
                if (responce.NoticeMessage != null)
                {
                    if (responce.NoticeMessage.Recipient != null)
                    {
                        recipient = responce.NoticeMessage.Recipient.Addresses.Length > 0 ?
                            responce.NoticeMessage.Recipient.Addresses[0] :
                            "<no-address>";
                    }
                    else
                    {
                        recipient = "<null-address>";
                    }
                }
                if (responce.Exception != null) error = responce.Exception.Message;
                sb.AppendLine();
                sb.AppendFormat("   {3}->{0}({1})={2} {4}", recipient, responce.SenderName, responce.Result, responce.NotifyAction.ID, error);
            }
            return sb.ToString();
        }
    }
}