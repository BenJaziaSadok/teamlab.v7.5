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
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Configuration;
using ASC.Web.Studio.Utility;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;

namespace ASC.Web.Projects
{
    public partial class ProjectTemplates : BasePage
    {
        public bool EmptyListTemplates { get; set; }

        protected override void PageLoad()
        {
            if (!Participant.IsAdmin || Participant.IsVisitor)
                Response.Redirect(PathProvider.BaseVirtualPath, true);

            if (!String.IsNullOrEmpty(UrlParameters.EntityID))
            {
                if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var projectAction = (Controls.Templates.CreateProject)LoadControl(PathProvider.GetControlVirtualPath("CreateProject.ascx"));
                    _content.Controls.Add(projectAction);
                    return;
                }

                if (string.Compare(UrlParameters.ActionType, "edit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var projectAction = (Controls.Templates.EditTemplate)LoadControl(PathProvider.GetControlVirtualPath("EditTemplate.ascx"));
                    _content.Controls.Add(projectAction);
                    return;
                }
            }
            else
            {
                if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var projectAction = (Controls.Templates.EditTemplate)LoadControl(PathProvider.GetControlVirtualPath("EditTemplate.ascx"));
                    _content.Controls.Add(projectAction);
                    return;
                }
            }



            var templates = Global.EngineFactory.GetTemplateEngine().GetAll();
            EmptyListTemplates = templates.Count <= 0;

            if (!EmptyListTemplates)
            {
                Master.JsonPublisher(templates, "templates");
                _hintPopup.Options.IsPopup = true;
            }

            var escNoTmpl = new Studio.Controls.Common.EmptyScreenControl
                {
                    Header = ProjectTemplatesResource.EmptyListTemplateHeader,
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("project-templates_logo.png", ProductEntryPoint.ID),
                    Describe = ProjectTemplatesResource.EmptyListTemplateDescr,
                    ID = "escNoTmpl",
                    ButtonHTML = string.Format("<a href='projectTemplates.aspx?action=add' class='projectsEmpty baseLinkAction addFirstElement'>{0}<a>", ProjectTemplatesResource.EmptyListTemplateButton)
                };
            _escNoTmpl.Controls.Add(escNoTmpl);

            Title = HeaderStringHelper.GetPageTitle(ProjectTemplatesResource.AllProjectTmpl);
        }
    }
}