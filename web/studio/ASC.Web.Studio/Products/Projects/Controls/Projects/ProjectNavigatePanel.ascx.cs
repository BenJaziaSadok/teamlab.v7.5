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
using System.Linq;
using System.Text;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Core.Users;
using ASC.Web.Studio.UserControls.Common.TabsNavigator;

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ProjectNavigatePanel : BaseUserControl
    {
        public string CurrentPage { get { return Page.Master.CurrentPage; } }

        public string EssenceTitle { get; set; }

        public string EssenceStatus { get; set; }

        public bool IsSubcribed { get { return Page.Master.IsSubcribed; } }

        public bool InConcreteProjectModule { get; set; }

        protected Project Project { get { return Page.Project; } }

        protected string ProjectLeaderName { get; set; }

        protected bool CanEditProject { get; set; }

        protected bool CanDeleteProject { get; set; }

        protected bool IsFollowed { get; set; }

        protected bool IsInTeam { get; set; }

        protected string UpLink { get; set; }

        public bool ShowGanttChartFlag { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            EssenceTitle = Page.Master.EssenceTitle;
            EssenceStatus = Page.Master.EssenceStatus;

            _hintPopup.Options.IsPopup = true;
            _projectDescriptionPopup.Options.IsPopup = true;

            CanEditProject = ProjectSecurity.CanEdit(Project);
            CanDeleteProject = ProjectSecurity.IsAdministrator(Page.Participant.ID);
            ProjectLeaderName = Global.EngineFactory.GetParticipantEngine().GetByID(Project.Responsible).UserInfo.DisplayUserName();
            IsFollowed = Global.EngineFactory.GetProjectEngine().IsFollow(Project.ID, Page.Participant.ID);
            IsInTeam = Global.EngineFactory.GetProjectEngine().IsInTeam(Project.ID, Page.Participant.ID);
            InConcreteProjectModule = RequestContext.IsInConcreteProjectModule;
            InitScripts();

            SetShowGanttChartFlag();

            if (string.IsNullOrEmpty(EssenceTitle))
            {
                EssenceTitle = Project.Title;
                EssenceStatus = Project.Status != ProjectStatus.Open ? LocalizedEnumConverter.ConvertToString(Project.Status).ToLower() : "";
            }

            if (!InConcreteProjectModule)
            {
                Tabs.TabItems.AddRange(ModuleManager.GetModules(Project).Select(r=> r.ConvertToNavigatorItem(CurrentPage)));
            }

            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                var urlReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();

                if (CurrentPage == "tasks")
                {
                    UpLink = "tasks.aspx";
                }

                if (CurrentPage == "messages")
                {
                    UpLink = "messages.aspx";
                }

                if (!string.IsNullOrEmpty(UpLink))
                {
                    if (urlReferrer.IndexOf("add", StringComparison.OrdinalIgnoreCase) > 0 || urlReferrer.IndexOf("edit", StringComparison.OrdinalIgnoreCase) > 0)
                        UpLink = string.Format("{0}?prjID={1}", UpLink, Project.ID);

                    if ((urlReferrer.IndexOf("messages", StringComparison.OrdinalIgnoreCase) > 0 || urlReferrer.IndexOf("tasks", StringComparison.OrdinalIgnoreCase) > 0) && urlReferrer.IndexOf("prjid", StringComparison.OrdinalIgnoreCase) > 0)
                        UpLink = string.Format("{0}?prjID={1}", UpLink, Project.ID);
                }
            }
        }

        private void InitScripts()
        {
            var dropdownId = "";
            if (!InConcreteProjectModule)
            {
                dropdownId = "projectActions";
            }
            else switch (CurrentPage)
            {
                case "timetracking":
                    dropdownId = "projectActions";
                    break;
                case "messages":
                    dropdownId = "discussionActions";
                    break;
                case "tasks":
                    dropdownId = "taskActions";
                    break;
            }
            var script = new StringBuilder();

            script.Append("jq.dropdownToggle({");
            script.Append("switcherSelector: '.project-title .menu-small',");
            script.Append("dropdownID: '" + dropdownId + "',");
            script.Append("addTop: -4,");
            script.Append("addLeft: -11,");
            script.Append("showFunction: function(switcherObj, dropdownItem) {");
            script.Append("jq('.project-title .menu-small').removeClass('active');");
            script.Append("if (dropdownItem.is(':hidden')) {");
            script.Append("switcherObj.addClass('active');");
            script.Append("}},");
            script.Append("hideFunction: function() {");
            script.Append("jq('.project-title .menu-small').removeClass('active');");
            script.Append("}");
            script.Append("});");

            Page.RegisterInlineScript(script.ToString());
        }

        protected void SetShowGanttChartFlag()
        {
            if (InConcreteProjectModule || Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
            {
                ShowGanttChartFlag = false;
                return;
            }
            if (ProjectSecurity.CanReadGantt(Project))
            {
                ShowGanttChartFlag = true;
                return;
            }

            ShowGanttChartFlag = false;
        }
    }
}