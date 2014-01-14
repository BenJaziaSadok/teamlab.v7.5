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

using ASC.Web.Studio.Controls.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

using ASC.Projects.Engine;

namespace ASC.Web.Projects
{
    public partial class Projects : BasePage
    {
        protected override string CookieKeyForPagination
        {
            get
            {
                return "projectsKeyForPagination";
            }
        }

        protected override void PageLoad()
        {
            if (RequestContext.IsInConcreteProject)
            {
                if (string.Compare(UrlParameters.ActionType, "edit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    _content.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("ProjectAction.ascx")));
                    Master.DisabledPrjNavPanel = true;
                    return;
                }

                Response.Redirect(String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx?prjID=" + RequestContext.GetCurrentProjectId()));
            }
            else
            {
                if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (ProjectSecurity.IsAdministrator(Participant.ID))
                    {
                        _content.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("ProjectAction.ascx")));
                        return;
                    }
                    Response.Redirect("projects.aspx");
                }
            }

            RenderControls();

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.Projects);
        }

        private void RenderControls()
        {  
            if (!RequestContext.HasAnyProjects())
            {
                var button = "";
                if (ProjectSecurity.CanCreateProject())
                {
                    _content.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("DashboardEmptyScreen.ascx")));
                    button = string.Format("<a href='projects.aspx?action=add' class='projectsEmpty baseLinkAction addFirstElement'>{0}<a>", ProjectResource.CreateFirstProject);
                }

                _content.Controls.Add(new EmptyScreenControl
                {
                    Header = ProjectResource.EmptyListProjHeader,
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("projects_logo.png", ProductEntryPoint.ID),
                    Describe = ProjectResource.EmptyListProjDescribe,
                    ButtonHTML = button
                });

            }
            else
            {
                _listProjects.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("ProjectsList.ascx")));
                _listProjects.Controls.Add(Masters.BasicTemplate.RenderEmptyScreenForFilter(ProjectsCommonResource.Filter_NoProjects,
                                                                     ProjectResource.DescrEmptyListProjFilter));
                Page.RegisterInlineScript(@"
                        if (!jq.getURLParam('action') && location.href.indexOf('projects.aspx') > 0) {
                            ASC.Projects.AllProject.init(false, " + EntryCountOnPage + ",'" + CookieKeyForPagination + "', " + Global.VisiblePageCount + ");}", true);
            }
        }
    }
}
