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
using System.Web;
using System.Collections;
using System.Collections.Generic;

using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Projects.Engine;
using ASC.Projects.Core.Domain;

namespace ASC.Web.Projects.Classes
{
    public class RequestContext
    {
        static ProjectFat Projectctx { get { return Hash["_projectctx"] as ProjectFat; } set { Hash["_projectctx"] = value; } }
        static int? ProjectId { get { return Hash["_projectId"] as int?; } set { Hash["_projectId"] = value; } }
        static int? ProjectsCount { get { return Hash["_projectsCount"] as int?; } set { Hash["_projectsCount"] = value; } }

        static List<Project> UserProjects { get { return Hash["_userProjects"] as List<Project>; } set { Hash["_userProjects"] = value; } }
        static List<Project> UserFollowingProjects { get { return Hash["_userFollowingProjects"] as List<Project>; } set { Hash["_userFollowingProjects"] = value; } }

        #region Project

        public static bool IsInConcreteProject
        {
            get { return !String.IsNullOrEmpty(UrlParameters.ProjectID); }
        }

        public static bool IsInConcreteProjectModule
        {
            get { return IsInConcreteProject && !String.IsNullOrEmpty(UrlParameters.EntityID); }
        }

        public static Project GetCurrentProject(bool isthrow = true)
        {
            if (Projectctx == null)
            {
                var project = Global.EngineFactory.GetProjectEngine().GetByID(GetCurrentProjectId(isthrow));

                if (project == null)
                {
                    if (isthrow) throw new ApplicationException("ProjectFat not finded");
                }
                else
                    Projectctx = new ProjectFat(project);
            }

            return Projectctx != null ? Projectctx.Project : null;
        }

        public static int GetCurrentProjectId(bool isthrow = true)
        {
            if (!ProjectId.HasValue)
            {
                int pid;
                if (!Int32.TryParse(UrlParameters.ProjectID, out pid))
                {
                    if (isthrow)
                        throw new ApplicationException("ProjectFat Id parameter invalid");
                }
                else
                    ProjectId = pid;
            }
            return ProjectId.HasValue ? ProjectId.Value : -1;
        }

        #endregion

        #region Projects

        public static bool HasAnyProjects()
        {
            if (
                ProjectsCount.HasValue && ProjectsCount.Value > 0
                ||
                UserProjects != null && UserProjects.Count > 0
                ||
                UserFollowingProjects != null && UserFollowingProjects.Count > 0)
                return true;

            return GetProjectsCount() > 0;
        }

        public static bool HasCurrentUserAnyProjects()
        {
            return GetCurrentUserProjects().Count > 0 || GetCurrentUserFollowingProjects().Count > 0;
        }

        public static int GetProjectsCount()
        {
            if (!ProjectsCount.HasValue)
                ProjectsCount = Global.EngineFactory.GetProjectEngine().GetAll().Count();
            return ProjectsCount.Value;
        }

        public static List<Project> GetCurrentUserProjects()
        {
            return UserProjects ??
                   (UserProjects =
                    Global.EngineFactory.GetProjectEngine().GetByParticipant(SecurityContext.CurrentAccount.ID));
        }

        public static List<Project> GetCurrentUserFollowingProjects()
        {
            return UserFollowingProjects ??
                   (UserFollowingProjects =
                    Global.EngineFactory.GetProjectEngine().GetFollowing(SecurityContext.CurrentAccount.ID));
        }

        #endregion

        private static bool CanCreate(Func<Project, bool> canCreate, bool checkConreteProject)
        {
            if (checkConreteProject && IsInConcreteProject)
            {
                var project = GetCurrentProject();
                return project.Status != ProjectStatus.Closed && canCreate(project);
            }

            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
            return isAdmin
                       ? HasAnyProjects()
                       : GetCurrentUserProjects().Where(canCreate).Any();
        }

        public static bool CanCreateTask(bool checkConreteProject = false)
        {
            return CanCreate(ProjectSecurity.CanCreateTask, checkConreteProject);
        }

        public static bool CanCreateMilestone(bool checkConreteProject = false)
        {
            return CanCreate(ProjectSecurity.CanCreateMilestone, checkConreteProject);   
        }

        public static bool CanCreateDiscussion(bool checkConreteProject = false)
        {
            return CanCreate(ProjectSecurity.CanCreateMessage, checkConreteProject);
        }

        public static bool CanCreateTime(bool checkConreteProject = false)
        {
            if (checkConreteProject && IsInConcreteProject)
            {
                var project = GetCurrentProject();
                var taskCount = Global.EngineFactory.GetProjectEngine().GetTaskCount(project.ID, null);
                return project.Status != ProjectStatus.Closed && taskCount > 0 && ProjectSecurity.CanCreateTimeSpend(project);
            }

            return CanCreate(ProjectSecurity.CanCreateTimeSpend, false);
        }

        #region internal

        const string StorageKey = "PROJECT_REQ_CTX";

        static Hashtable Hash
        {
            get
            {
                if (HttpContext.Current == null) throw new ApplicationException("Not in http request");

                var hash = (Hashtable)HttpContext.Current.Items[StorageKey];
                if (hash == null)
                {
                    hash = new Hashtable();
                    HttpContext.Current.Items[StorageKey] = hash;
                }
                return hash;
            }
        }

        #endregion
    }

    public class ProjectFat
    {
        readonly Project project;

        internal ProjectFat(Project project)
        {
            this.project = project;
            Responsible = Global.EngineFactory.GetParticipantEngine().GetByID(this.project.Responsible).UserInfo;
        }

        public Project Project { get { return project; } }

        List<Participant> team;
        public List<Participant> GetTeam()
        {
            return team ?? (team = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID)
                                       .OrderBy(p => p.UserInfo, UserInfoComparer.Default)
                                       .ToList());
        }

        public List<Participant> GetActiveTeam()
        {
            var projectTeam = GetTeam();

            if (ProjectSecurity.CanEditTeam(Project))
            {
                var engine = Global.EngineFactory.GetProjectEngine();
                var deleted = projectTeam.FindAll(u => u.UserInfo.Status != EmployeeStatus.Active || !CoreContext.UserManager.UserExists(u.ID));
                foreach (var d in deleted)
                {
                    engine.RemoveFromTeam(Project, d, true);
                }
            }

            var active = projectTeam.FindAll(u => u.UserInfo.Status != EmployeeStatus.Terminated && CoreContext.UserManager.UserExists(u.ID));
            return active.OrderBy(u => u.UserInfo, UserInfoComparer.Default).ToList();
        }

        public bool IsResponsible()
        {
            return Responsible.ID == SecurityContext.CurrentAccount.ID;
        }

        List<Milestone> milestones;
        public List<Milestone> GetMilestones()
        {
            return milestones ?? (milestones = Global.EngineFactory.GetMilestoneEngine().GetByProject(Project.ID));
        }

        private List<Task> tasks;
        public List<Task> GetTasks()
        {
            return tasks ?? (tasks = Global.EngineFactory.GetTaskEngine().GetByProject(Project.ID, null, Guid.Empty));
        }

        private List<Message> discussions;
        public List<Message> GetDiscussions()
        {
            return discussions ?? (discussions = Global.EngineFactory.GetMessageEngine().GetByProject(Project.ID));
        }

        public UserInfo Responsible { get; private set; }
    }
}
