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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Masters;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Utility;
using ASC.Core.Users;
using Newtonsoft.Json;

namespace ASC.Web.Projects.Controls.Templates
{
    public partial class CreateProject : BaseUserControl
    {
        private int projectTmplId;

        public Template Templ { get; set; }

        protected bool IsAdmin
        {
            get { return Page.Participant.IsAdmin; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin || Page.Participant.IsVisitor)
                HttpContext.Current.Response.Redirect(PathProvider.BaseVirtualPath, true);

            if (Request["project"] != null)
            {
                var newProjectId = Create();
                Response.Clear();
                Response.ContentType = "text/html; charset=utf-8";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Charset = Encoding.UTF8.WebName;
                Response.Write(newProjectId);
                Response.End();
            }

            if (Int32.TryParse(UrlParameters.EntityID, out projectTmplId))
            {
                Templ = Global.EngineFactory.GetTemplateEngine().GetByID(projectTmplId);
                Page.Master.JsonPublisher(Templ, "template");
            }

            LoadProjectManagerSelector();
            LoadProjectTeamSelector();

            Page.Title = HeaderStringHelper.GetPageTitle(ProjectTemplatesResource.CreateProjFromTmpl);

            _attantion.Options.IsPopup = true;

            var script = @"
                if (jq.getURLParam('action') == 'add' && jq.getURLParam('id')) {
                    ASC.Projects.CreateProjectFromTemplate.init(""<span class='chooseResponsible nobody'><span class='dottedLink'>" + ProjectTemplatesResource.ChooseResponsible + @"</span></span>"");
                    };";

            Page.RegisterInlineScript(script);
        }

        private void LoadProjectManagerSelector()
        {
            var projectManagerSelector = new AdvancedUserSelector
                {
                    ID = "projectManagerSelector",
                    DefaultGroupText = CustomResourceHelper.GetResource("EmployeeAllDepartments"),
                    EmployeeType = EmployeeType.User
                };
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

        public int Create()
        {
            var newProject = Parser<Project>(Request["project"]);
            var team = new List<Guid>();
            var listMilestones = new List<Milestone>();
            var listTasks = new List<Task>();
            var notifyManager = Convert.ToBoolean(Request["notifyManager"]);
            var notifyResponsibles = Convert.ToBoolean(Request["notifyResponsibles"]);

            var projectEngine = Global.EngineFactory.GetProjectEngine();
            var participantEngine = Global.EngineFactory.GetParticipantEngine();
            var taskEngine = Global.EngineFactory.GetTaskEngine();
            var milestoneEngine = Global.EngineFactory.GetMilestoneEngine();

            if (Request["team"] != null)
            {
                team = Parser<List<Guid>>(Request["team"]);
            }

            if (Request["milestones"] != null)
            {
                listMilestones = Parser<List<Milestone>>(Request["milestones"]);
            }

            if (Request["noAssignTasks"] != null)
            {
                listTasks = Parser<List<Task>>(Request["noAssignTasks"]);
            }

            if (ProjectSecurity.CanCreateProject())
            {
                if (newProject != null)
                {
                    projectEngine.SaveOrUpdate(newProject, notifyManager);
                    projectEngine.AddToTeam(newProject, participantEngine.GetByID(newProject.Responsible), true);

                    //add team
                    foreach (var participant in team.Where(participant => participant != Guid.Empty))
                    {
                        projectEngine.AddToTeam(newProject, participantEngine.GetByID(participant), true);
                    }

                    foreach (var milestone in listMilestones)
                    {
                        var milestoneTasks = milestone.Tasks;
                        milestone.Description = string.Empty;
                        milestone.Project = newProject;
                        milestoneEngine.SaveOrUpdate(milestone, notifyResponsibles);

                        foreach (var task in milestoneTasks)
                        {
                            task.Status = TaskStatus.Open;
                            task.Milestone = milestone.ID;
                            task.Project = newProject;
                            taskEngine.SaveOrUpdate(task, null, notifyResponsibles);
                        }
                    }

                    //add no assign tasks

                    foreach (var task in listTasks)
                    {
                        task.Project = newProject;
                        task.Status = TaskStatus.Open;
                        taskEngine.SaveOrUpdate(task, null, notifyResponsibles);
                    }

                    return newProject.ID;
                }

            }

            return 0;
        }

        private static T Parser<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}