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
using System.ServiceModel;
using log4net;

namespace ASC.Core.Notify.Jabber
{
    public class JabberServiceClient
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JabberServiceClient));

        private static readonly TimeSpan timeout = TimeSpan.FromMinutes(2);

        private static DateTime lastErrorTime = default(DateTime);

        private static bool IsServiceProbablyNotAvailable()
        {
            return lastErrorTime != default(DateTime) && lastErrorTime + timeout > DateTime.Now;
        }


        public bool SendMessage(string to, string subject, string text, int tenantId)
        {
            if (IsServiceProbablyNotAvailable()) return false;

            using (var service = new JabberServiceClientWcf())
            {
                try
                {
                    service.SendMessage(to, subject, text, tenantId);
                    return true;
                }
                catch (FaultException e)
                {
                    log.Error(e);
                    throw;
                }
                catch (CommunicationException e)
                {
                    log.Error(e);
                    lastErrorTime = DateTime.Now;
                }
                catch (TimeoutException e)
                {
                    log.Error(e);
                    lastErrorTime = DateTime.Now;
                }
            }

            return false;
        }
    }
}
