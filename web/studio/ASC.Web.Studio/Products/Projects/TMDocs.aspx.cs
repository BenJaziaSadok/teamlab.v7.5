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

using System.Web;
using ASC.Projects.Engine;
using ASC.Web.Files.Controls;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects
{
    public partial class TMDocs : BasePage
    {
        protected override void PageLoad()
        {
            if (RequestContext.IsInConcreteProject && !ProjectSecurity.CanReadFiles(Project))
            {
                Response.Redirect("projects.aspx?prjID=" + Project.ID, true);
            }

            var mainContent = (MainContent) LoadControl(MainContent.Location);
            mainContent.FolderIDCurrentRoot = Project == null ? Files.Classes.Global.FolderProjects : FileEngine2.GetRoot(Project.ID);
            mainContent.TitlePage = ProjectsCommonResource.ModuleName;
            CommonContainerHolder.Controls.Add(mainContent);

            Title = HeaderStringHelper.GetPageTitle(ProjectsFileResource.Files);

            Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/products/files/masters/styles.ascx")));
            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/files/masters/FilesScripts.ascx")));
            Page.RegisterClientLocalizationScript(typeof(Files.Masters.ClientScripts.FilesLocalizationResources));
            Page.RegisterClientScript(typeof(Files.Masters.ClientScripts.FilesConstantsResources));
            Page.RegisterInlineScript(@"ZeroClipboard.setMoviePath('" + CommonLinkUtility.ToAbsolute("~/js/flash/zeroclipboard/zeroclipboard10.swf") + "');", true);
        }
    }
}