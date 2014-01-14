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
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;

namespace ASC.Core.Notify.Senders
{
    public class JabberSender : INotifySender
    {
        private JabberServiceClient service = new JabberServiceClient();


        public void Init(IDictionary<string, string> properties)
        {
        }

        public NoticeSendResult Send(NotifyMessage m)
        {
            service.SendMessage(m.To, m.Subject, m.Content, m.Tenant);
            return NoticeSendResult.OK;
        }
    }
}
