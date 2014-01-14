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
using System.Web;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Masters;
using ASC.Web.Studio;

using Newtonsoft.Json.Linq;
using Global = ASC.Web.Projects.Classes.Global;

namespace ASC.Web.Projects
{
    public abstract class BasePage : MainPage
    {
        #region Properties

        public Participant Participant { get; private set; }

        public Project Project { get; set; }

        protected virtual string CookieKeyForPagination { get { return ""; } }

        protected virtual int EntryCountOnPage { get; set; }

        #endregion

        protected BasePage()
        {
            if (CallContext.GetData("CURRENT_ACCOUNT") == null && SecurityContext.IsAuthenticated)
            {
                CallContext.SetData("CURRENT_ACCOUNT", SecurityContext.CurrentAccount.ID);
            }

            Participant = Global.EngineFactory.GetParticipantEngine().GetByID(SecurityContext.CurrentAccount.ID);
            Participant.IsAdmin = WebItemSecurity.IsProductAdministrator(EngineFactory.ProductId, SecurityContext.CurrentAccount.ID);

            if (RequestContext.IsInConcreteProject)
            {
                Project = RequestContext.GetCurrentProject(false);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (RequestContext.IsInConcreteProject && Project == null)
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
                return;
            }

            InitPageNumberAndCountOnPageFromCookie();
            PageLoad();
        }

        protected abstract void PageLoad();

        public new BasicTemplate Master
        {
            get { return base.Master as BasicTemplate; }
        }

        public void InitPageNumberAndCountOnPageFromCookie()
        {
            if (EntryCountOnPage == 0)
                EntryCountOnPage = Global.EntryCountOnPage;

            var cookieForPagination = Request.Cookies[CookieKeyForPagination];

            if (cookieForPagination == null) return;

            var cookieForPaginationJson = HttpUtility.UrlDecode(cookieForPagination.Value);
            try
            {
                EntryCountOnPage = JObject.Parse(cookieForPaginationJson).Value<int>("countOnPage");
            }
            catch (Exception)
            {
            }
        }
    }
}
