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
using System.Runtime.Remoting.Messaging;

using ASC.Api.Impl;
using ASC.Core;
using ASC.Projects.Engine;

namespace ASC.Api.Projects
{
    public class ProjectApiBase
    {
        internal const string DbId = "projects";//Copied from projects
        protected ApiContext _context;
        private EngineFactory _engineFactory;

        protected EngineFactory EngineFactory
        {
            get
            {
                if (_engineFactory==null)
                {
                    _engineFactory = new EngineFactory(DbId, TenantId);
                }
                //NOTE: don't sure if it's need to be here since remoting is gone
                if (CallContext.GetData("CURRENT_ACCOUNT") == null && SecurityContext.IsAuthenticated)
                {
                  CallContext.SetData("CURRENT_ACCOUNT", SecurityContext.CurrentAccount.ID);
                }
                return _engineFactory;
            }
        }

        private static int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        protected static Guid CurrentUserId
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }
    }
}
