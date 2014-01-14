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
using System.Web.UI;
using ASC.Web.Community.Controls;
using ASC.Data.Storage;
using System.Web;
using ASC.Web.Community.Resources;

namespace ASC.Web.Community
{
    public partial class CommunityMasterPage : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(GetFileStaticRelativePath("common.js"));
            Page.RegisterStyleControl(GetFileStaticRelativePath("community_common.css"));

            _sideNavigation.Controls.Add(LoadControl(NavigationSidePanel.Location));

            if (!(Page is _Default))
            {
                var script = new StringBuilder();
                script.Append("window.ASC=window.ASC||{};");
                script.Append("window.ASC.Community=window.ASC.Community||{};");
                script.Append("window.ASC.Community.Resources={};");
                script.AppendFormat("window.ASC.Community.Resources.HelpTitleAddNew=\"{0}\";", CommunityResource.HelpTitleAddNew);
                script.AppendFormat("window.ASC.Community.Resources.HelpContentAddNew=\"{0}\";", CommunityResource.HelpContentAddNew);
                script.AppendFormat("window.ASC.Community.Resources.HelpTitleSettings=\"{0}\";", CommunityResource.HelpTitleSettings);
                script.AppendFormat("window.ASC.Community.Resources.HelpContentSettings=\"{0}\";", CommunityResource.HelpContentSettings);
                script.AppendFormat("window.ASC.Community.Resources.HelpTitleNavigateRead=\"{0}\";", CommunityResource.HelpTitleNavigateRead);
                script.AppendFormat("window.ASC.Community.Resources.HelpContentNavigateRead=\"{0}\";", CommunityResource.HelpContentNavigateRead);
                script.AppendFormat("window.ASC.Community.Resources.HelpTitleSwitchModules=\"{0}\";", CommunityResource.HelpTitleSwitchModules);
                script.AppendFormat("window.ASC.Community.Resources.HelpContentSwitchModules=\"{0}\";", CommunityResource.HelpContentSwitchModules);

                Page.RegisterInlineScript(script.ToString());
            }
            else
            {
                Master.DisabledHelpTour = true;
            }
        }

        protected string GetFileStaticRelativePath(String fileName)
        {
            if (fileName.EndsWith(".js"))
            {
                return ResolveUrl("~/products/community/js/" + fileName);
            }
            if (fileName.EndsWith(".ascx"))
            {
                return VirtualPathUtility.ToAbsolute("~/products/community/controls/" + fileName);
            }
            if (fileName.EndsWith(".css"))
            {
                return ResolveUrl("~/products/community/app_themes/default/" + fileName);
            }
            if (fileName.EndsWith(".png") || fileName.EndsWith(".gif") || fileName.EndsWith(".jpg"))
            {
                return WebPath.GetPath("/products/community/app_themes/default/images/" + fileName);
            }
            return fileName;
        }
    }
}