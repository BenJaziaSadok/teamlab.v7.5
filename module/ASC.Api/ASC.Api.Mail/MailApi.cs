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

using System.Configuration;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.Mail.Aggregator;

namespace ASC.Api.Mail
{
    public partial class MailApi : IApiEntryPoint
    {
        public ApiContext _context;
        private MailBoxManager _mailBoxManager;
        private MailSendQueue _mailSendQueue;

        protected MailBoxManager mailBoxManager
        {
            get
            {
                if (_mailBoxManager == null)
                    _mailBoxManager = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 25);
                return _mailBoxManager;
            }
        }

        private MailSendQueue sendQueue
        {
            get
            {
                if (_mailSendQueue == null)
                    _mailSendQueue = new MailSendQueue(mailBoxManager);
                return _mailSendQueue;
            }
        }


        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        public MailApi(ApiContext context)
        {
            _context = context;
        }


        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "mail"; }
        }

        private int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        private string Username
        {
            get { return SecurityContext.CurrentAccount.ID.ToString(); }
        }
    }
}
