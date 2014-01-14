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
using System.Linq;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ProjectAction : BaseUserControl
    {
        protected Project Project { get { return Page.Project; } }

        protected string ProjectTags { get; set; }

        protected string UrlProject { get; set; }

        private ProjectFat ProjectFat { get; set; }

        protected bool HasTemplates { get; private set; }

        private static TagEngine TagEngine
        {
            get { return Global.EngineFactory.GetTagEngine(); }
        }

        protected int ActiveTasksCount
        {
            get { return ProjectFat == null ? 0 : ProjectFat.GetTasks().Count(t => t.Status == TaskStatus.Open); }
        }

        protected int ActiveMilestonesCount
        {
            get { return ProjectFat == null ? 0 : ProjectFat.GetMilestones().Count(m => m.Status == MilestoneStatus.Open); }
        }

        protected bool IsProjectCreatedFromCrm { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Participant.IsVisitor)
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            IsProjectCreatedFromCrm = (Request.Params["opportunityID"] != null || Request.Params["contactID"] != null);

            Bootstrap();

            LoadProjectManagerSelector();

            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("projectaction.js"));

            if (Project != null)
            {
                RenderScript();
                ProjectFat = new ProjectFat(Project);
                return;
            }

            Page.Master.RegisterCRMResources();


            LoadProjectTeamSelector();
        }

        private void RenderScript()
        {
            if(IsEditingProjectAvailable())
             {
                 Page.RegisterInlineScript(@" var status = '" +  GetProjectStatus() + @"';
                        jq('.dottedHeader').removeClass('dottedHeader');
                        jq('#projectTitleContainer .inputTitleContainer').css('width', '100%');
                        jq('#projectDescriptionContainer').show();
                        jq('#notifyManagerCheckbox').attr('disabled', 'disabled');
                        jq('#projectStatus option[value=' + status + ']').attr('selected', true);
                        jq('#projectTagsContainer').show();
                    ");
             } 
        }

        private void Bootstrap()
        {
            _hintPopupDeleteProject.Options.IsPopup = true;
            _hintPopupActiveTasks.Options.IsPopup = true;
            _hintPopupActiveMilestones.Options.IsPopup = true;

            if (Project != null && !ProjectSecurity.CanEdit(Project))
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            if (Project != null)
            {
                projectTitle.Text = Project.Title;
                projectDescription.Text = Project.Description;

                var tags = TagEngine.GetProjectTags(Project.ID).Select(r => r.Value).ToArray();
                ProjectTags = string.Join(", ", tags);

                HasTemplates = false;

                Page.Title = HeaderStringHelper.GetPageTitle(Project.Title);
                UrlProject = "tasks.aspx?prjID=" + Project.ID;
            }
            else
            {
                Page.Title = HeaderStringHelper.GetPageTitle(ProjectResource.CreateNewProject);
                ProjectTags = "";
            }
        }

        private void LoadProjectManagerSelector()
        {
            var projectManagerSelector = new AdvancedUserSelector
                {
                    ID = "projectManagerSelector",
                    DefaultGroupText = CustomResourceHelper.GetResource("EmployeeAllDepartments"),
                    EmployeeType = EmployeeType.User
                };
            if (Project != null)
            {
                projectManagerSelector.SelectedUserId = Project.Responsible;
            }
            projectManagerPlaceHolder.Controls.Add(projectManagerSelector);
        }

        private void LoadProjectTeamSelector()
        {
            var projectTeamSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            projectTeamSelector.BehaviorID = "projectTeamSelector";
            projectTeamSelector.DisabledUsers.Add(new Guid());
            projectTeamSelector.Title = ProjectResource.ManagmentTeam;
            projectTeamSelector.SelectedUserListTitle = ProjectResource.Team;

            projectTeamPlaceHolder.Controls.Add(projectTeamSelector);
        }

        protected string GetPageTitle()
        {
            return Project == null ? ProjectResource.CreateNewProject : ProjectResource.EditProject;
        }

        protected bool IsNotificationManagerAvailable()
        {
            return Project == null && Page.Participant.IsAdmin;
        }

        protected bool IsEditingProjectAvailable()
        {
            return Project != null;
        }

        protected string GetProjectStatus()
        {
            return Project != null ? Project.Status.ToString().ToLowerInvariant() : string.Empty;
        }

        protected string GetProjectStatusTitle()
        {
            if (Project == null) return string.Empty;
            var status = Project.Status;
            return status == ProjectStatus.Open
                       ? ProjectResource.ActiveProject
                       : status == ProjectStatus.Paused
                             ? ProjectResource.PausedProject
                             : ProjectResource.ClosedProject;
        }

        protected string RenderProjectPrivacyCheckboxValue()
        {
            return Project != null && Project.Private
                       ? "checked"
                       : "";
        }

        protected string GetActiveTasksUrl()
        {
            return Project == null ? string.Empty : string.Format("tasks.aspx?prjID={0}#status=open", Project.ID);
        }

        protected string GetActiveMilestonesUrl()
        {
            return Project == null ? string.Empty : string.Format("milestones.aspx?prjID={0}#status=open", Project.ID);
        }

        protected string GetProjectActionButtonTitle()
        {
            return Project == null ? ProjectResource.AddNewProject : ProjectResource.SaveProject;
        }
    }
}