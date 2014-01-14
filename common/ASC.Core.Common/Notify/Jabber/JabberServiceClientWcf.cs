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
using ASC.Common.Module;

namespace ASC.Core.Notify.Jabber
{
    public class JabberServiceClientWcf : BaseWcfClient<IJabberService>, IJabberService, IDisposable
    {
        public IDictionary<string, string> GetClientConfiguration(int tenantId)
        {
            return Channel.GetClientConfiguration(tenantId);
        }

        public bool IsUserAvailable(string username, int tenantId)
        {
            return Channel.IsUserAvailable(username, tenantId);
        }

        public int GetNewMessagesCount(string userName, int tenantId)
        {
            return Channel.GetNewMessagesCount(userName, tenantId);
        }

        public string GetUserToken(string userName, int tenantId)
        {
            return Channel.GetUserToken(userName, tenantId);
        }

        public void SendCommand(string from, string to, string command, int tenantId)
        {
            Channel.SendCommand(from, to, command, tenantId);
        }

        public void SendMessage(string to, string subject, string text, int tenantId)
        {
            Channel.SendMessage(to, subject, text, tenantId);
        }
    }
}
