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
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Projects
{
    public partial class Contacts : BasePage
    {
        public bool CanLinkContact { get; set; }

        protected override void PageLoad()
        {
            if (!ProjectSecurity.CanReadContacts(Project))
            {
                Response.Redirect("projects.aspx?prjID=" + Project.ID, true);
            }

            var crmEnabled = WebItemManager.Instance[new Guid("6743007C-6F95-4d20-8C88-A8601CE5E76D")];

            if (crmEnabled == null || crmEnabled.IsDisabled())
            {
                Response.Redirect(String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx?prjID=" + RequestContext.GetCurrentProjectId()));
            }

            CanLinkContact = ProjectSecurity.CanLinkContact(Project);

            var button = "";

            if(CanLinkContact)
            {
                button = "<a class='link-with-entity baseLinkAction'>" + ProjectsCommonResource.EmptyScreenContactsButton + "</a>";
            }

            var escNoContacts = new Studio.Controls.Common.EmptyScreenControl
            {
                Header = ProjectsCommonResource.EmptyScreenContasctsHeader,
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_persons.png", ProductEntryPoint.ID),
                Describe = ProjectsCommonResource.EmptyScreenContactsDescribe,
                ID = "escNoContacts",
                ButtonHTML = button,
                CssClass = "display-none"
            };
            emptyScreen.Controls.Add(escNoContacts);

            Page.Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.ModuleContacts);

            Master.RegisterCRMResources();
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("contacts.js"));
        }
    }
}
