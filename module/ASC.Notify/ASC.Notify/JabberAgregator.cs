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
using ASC.Core.Common;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;

namespace ASC.Notify
{
    class JabberAgregator
    {
        private JabberServiceClient jsc = new JabberServiceClient();
        public JabberAgregator()
        {
        }

        internal void SendByJabber(NotifyMessage m)
        {
            jsc.SendMessage(m.To, m.Subject, m.Content, m.Tenant);
        }
    }
}
