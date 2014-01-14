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
using System.Web;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Resources;

namespace ASC.Web.Talk
{
    public partial class DefaultTalk : MainPage
    {
        private TalkConfiguration cfg;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/talk/css/default/talk.overview.css"));

            Title = HeaderStringHelper.GetPageTitle(TalkResource.ProductName);
            Master.DisabledSidePanel = true;

            cfg = new TalkConfiguration();
        }

        public string ServerAddress
        {
            get { return cfg.ServerAddress; }
        }

        public string ServerName
        {
            get { return cfg.ServerName; }
        }

        public string ServerPort
        {
            get { return cfg.ServerPort; }
        }

        public string UserName
        {
            get { return cfg.UserName; }
        }

        public string JID
        {
            get { return cfg.Jid; }
        }
    }
}