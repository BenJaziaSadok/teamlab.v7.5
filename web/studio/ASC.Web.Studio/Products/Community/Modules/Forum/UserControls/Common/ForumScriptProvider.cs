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
using System.Web.UI;
using System.Text;
using ASC.Data.Storage;

namespace ASC.Web.UserControls.Forum.Common
{
    public class ForumScriptProvider : Control
    {
        public bool RegistrySearchHelper { get; set; }

        public Guid SettingsID { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl(Community.Forum.ForumManager.BaseVirtualPath + "/js/forum.js"));

            if (RegistrySearchHelper)
                Page.RegisterBodyScripts(ResolveUrl(Community.Forum.ForumManager.BaseVirtualPath + "/js/searchhelper.js"));


            var script = new StringBuilder();
            script.Append("if (typeof(ForumManager)=== 'undefined'){ForumManager = {};}");
            script.Append("ForumManager.QuestionEmptyMessage = '" + Resources.ForumUCResource.QuestionEmptyMessage + "';");
            script.Append("ForumManager.SubjectEmptyMessage = '" + Resources.ForumUCResource.SubjectEmptyMessage + "';");
            script.Append("ForumManager.ApproveTopicButton = '" + Resources.ForumUCResource.ApproveButton + "';");
            script.Append("ForumManager.OpenTopicButton = '" + Resources.ForumUCResource.OpenTopicButton + "';");
            script.Append("ForumManager.CloseTopicButton = '" + Resources.ForumUCResource.CloseTopicButton + "';");
            script.Append("ForumManager.StickyTopicButton = '" + Resources.ForumUCResource.StickyTopicButton + "';");
            script.Append("ForumManager.ClearStickyTopicButton = '" + Resources.ForumUCResource.ClearStickyTopicButton + "';");
            script.Append("ForumManager.DeleteTopicButton = '" + Resources.ForumUCResource.DeleteButton + "';");
            script.Append("ForumManager.EditTopicButton = '" + Resources.ForumUCResource.EditButton + "';");
            script.Append("ForumManager.ConfirmMessage = '" + Resources.ForumUCResource.ConfirmMessage + "';");
            script.Append("ForumManager.NameEmptyString = '" + Resources.ForumUCResource.NameEmptyString + "';");
            script.Append("ForumManager.SaveButton = '" + Resources.ForumUCResource.SaveButton + "';");
            script.Append("ForumManager.CancelButton = '" + Resources.ForumUCResource.CancelButton + "';");
            script.Append("ForumManager.SettingsID = '" + SettingsID + "';");
            script.Append("ForumManager.TextEmptyMessage = '" + Resources.ForumUCResource.TextEmptyMessage + "';");

            Page.RegisterInlineScript(script.ToString());

        }

    }
}
