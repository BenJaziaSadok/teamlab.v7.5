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

using ASC.Common.Module;
using ASC.Notify;
using ASC.Notify.Messages;

namespace ASC.Core.Notify
{
    public class NotifyServiceClient : BaseWcfClient<INotifyService>, INotifyService
    {
        public void SendNotifyMessage(NotifyMessage m)
        {
            Channel.SendNotifyMessage(m);
        }

        public void InvokeSendMethod(string service, string method, int tenant, params object[] parameters)
        {
            Channel.InvokeSendMethod(service, method, tenant, parameters);
        }
    }
}
