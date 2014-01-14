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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Core;

namespace ASC.Web.Community.Forum
{
    public partial class ForumMasterPage : MasterPage
    {
        public string SearchText { get; set; }

        public PlaceHolder ActionsPlaceHolder { get; set; }

        public string CurrentPageCaption
        {
            get { return ForumContainer.CurrentPageCaption; }
            set { ForumContainer.CurrentPageCaption = value; }
        }

        public ForumMasterPage()
        {
            ActionsPlaceHolder = new PlaceHolder();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _scriptProvider.SettingsID = ForumManager.Settings.ID;
            if (Page is NewPost || Page is EditTopic)
                _scriptProvider.RegistrySearchHelper = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ForumContainer.Options.InfoType = InfoType.Alert;

            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/jquery/jquery.ui.sortable.js"));
            Page.RegisterBodyScripts(ResolveUrl(ForumManager.BaseVirtualPath + "/js/forummaker.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/app_themes/default/style.css"));

            var sb = new StringBuilder();
            sb.Append(" ForumMakerProvider.All='" + Resources.ForumResource.All + "'; ");
            sb.Append(" ForumMakerProvider.ConfirmMessage='" + Resources.ForumResource.ConfirmMessage + "'; ");
            sb.Append(" ForumMakerProvider.SaveButton='" + Resources.ForumResource.SaveButton + "'; ");
            sb.Append(" ForumMakerProvider.CancelButton='" + Resources.ForumResource.CancelButton + "'; ");
            sb.Append(" ForumMakerProvider.NameEmptyString='" + Resources.ForumResource.NameEmptyString + "'; ");
            sb.Append(" ForumContainer_PanelInfoID = '" + ForumContainer.GetInfoPanelClientID() + "'; ");

            Page.RegisterInlineScript(sb.ToString());

            SearchText = "";

            if (!String.IsNullOrEmpty(Request["search"]))
                SearchText = Request["search"];
        }
    }
}