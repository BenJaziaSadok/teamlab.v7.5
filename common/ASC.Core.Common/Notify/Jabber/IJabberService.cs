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
using System.ServiceModel;

namespace ASC.Core.Notify.Jabber
{
    [ServiceContract]
    public interface IJabberService
    {
        [OperationContract]
        IDictionary<string, string> GetClientConfiguration(int tenantId);

        [OperationContract]
        bool IsUserAvailable(string username, int tenantId);

        [OperationContract]
        int GetNewMessagesCount(string userName, int tenantId);
        
        [OperationContract]
        string GetUserToken(string userName, int tenantId);
        
        [OperationContract]
        void SendCommand(string from, string to, string command, int tenantId);
        
        [OperationContract]
        void SendMessage(string to, string subject, string text, int tenantId);
    }
}