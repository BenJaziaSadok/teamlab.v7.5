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
using System.Collections.Generic;
using System.Linq;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;


namespace ASC.Web.Projects.Controls.Common
{
    public partial class NavigationSidePanel : BaseUserControl
    {
        public Project Project { get { return Page.Project; } }

        public string CurrentPage { get; set; }

        public List<Project> MyProjects { get; set; }

        protected Dictionary<string, bool> ParticipantSecurityInfo { get; set; }

        protected bool CanCreate { get; set; }

        protected bool IsProjectAdmin { get; set; }

        protected bool IsFullAdmin { get; set; }

        protected bool isStandalone
        {
            get { return CoreContext.Configuration.Standalone; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentPage = Page.Master.CurrentPage;
            MyProjects = RequestContext.GetCurrentUserProjects();

            Page.RegisterInlineScript(@"ASC.Projects.navSidePanel.init();");

            InitControls();

            IsProjectAdmin = Page.Participant.IsAdmin;
            IsFullAdmin = Page.Participant.IsFullAdmin;

            ParticipantSecurityInfo = new Dictionary<string, bool>
                                          {
                                              {"Project", IsProjectAdmin},
                                              {"Milestone", RequestContext.CanCreateMilestone()},
                                              {"Task", RequestContext.CanCreateTask()},
                                              {"Discussion", RequestContext.CanCreateDiscussion()},
                                              {"Time", RequestContext.CanCreateTime()},
                                              {"ProjectTemplate", IsProjectAdmin}
                                          };

            CanCreate = ParticipantSecurityInfo.Any(r => r.Value);
        }

        private void InitControls()
        {
            _taskAction.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("TaskAction.ascx")));
            _milestoneAction.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("MilestoneAction.ascx")));

            if (CurrentPage == "tmdocs")
            {
                CreateDocsHolder.Controls.Add(LoadControl(Files.Controls.CreateMenu.Location));
                RenderFolderTree();
            }
            else
            {
                if (RequestContext.IsInConcreteProject)
                    CurrentPage = "projects";
            }

            var help = (HelpCenter)LoadControl(HelpCenter.Location);
            help.IsSideBar = true;
            HelpHolder.Controls.Add(help);
            SupportHolder.Controls.Add(LoadControl(Support.Location));
        }

        private void RenderFolderTree()
        {
            var tree = (Files.Controls.Tree) LoadControl(Files.Controls.Tree.Location);
            tree.FolderIDCurrentRoot = Files.Classes.Global.FolderProjects;
            placeHolderFolderTree.Controls.Add(tree);
        }

        protected bool IsInConcreteProject()
        {
            return Project != null && MyProjects.Any(r => r.ID == Project.ID);
        }
    }
}