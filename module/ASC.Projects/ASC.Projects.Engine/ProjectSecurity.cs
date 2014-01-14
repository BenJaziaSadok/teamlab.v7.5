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
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using ASC.Web.Core;
using System.Web;
using System.IO;

namespace ASC.Projects.Engine
{
    public class ProjectSecurity
    {
        private static bool? ganttJsExists = null;


        #region Properties

        private static Guid CurrentUserId
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        private static bool CurrentUserAdministrator
        {
            get { return IsAdministrator(CurrentUserId); }
        }

        private static bool CurrentUserIsVisitor
        {
            get { return IsVisitor(CurrentUserId); }
        }

        public static bool IsProjectsEnabled(Guid userID)
        {
            var projects = WebItemManager.Instance[EngineFactory.ProductId];

            if (projects != null)
            {
                return !projects.IsDisabled(userID);
            }

            return false;
        }

        #endregion

        #region Can Go To Feed

        public static bool CanGoToFeed(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            if (project == null) return false;

            return IsInTeam(project, userId, false) || IsFollow(project, userId);
        }

        public static bool CanGoToFeed(Milestone milestone, Guid userId)
        {
            if (milestone == null) return false;
            if (!CanGoToFeed(milestone.Project, userId)) return false;

            return milestone.Responsible == userId ||
                   GetTeamSecurityForParticipants(milestone.Project, userId, ProjectTeamSecurity.Milestone);
        }

        public static bool CanGoToFeed(Message discussion, Guid userId)
        {
            if (discussion == null) return false;

            if (discussion.CreateBy == userId) return true;
            if (!CanGoToFeed(discussion.Project, userId)) return false;

            var participiants = new MessageEngine(GetFactory(), null).GetSubscribers(discussion);
            return participiants.Any(r => new Guid(r.ID).Equals(userId)) &&
                   GetTeamSecurityForParticipants(discussion.Project, userId, ProjectTeamSecurity.Messages);
        }

        public static bool CanGoToFeed(Task task, Guid userId)
        {
            if (task == null) return false;

            if (task.CreateBy == userId) return true;
            if (!CanGoToFeed(task.Project, userId)) return false;

            if (task.Responsibles.Contains(userId)) return true;

            if (task.Milestone != 0 && !CanReadMilestones(task.Project, userId))
            {
                var milestone = GetFactory().GetMilestoneDao().GetById(task.Milestone);
                if (milestone.Responsible == userId) return true;
            }

            return (GetTeamSecurityForParticipants(task.Project, userId, ProjectTeamSecurity.Tasks));
        }

        #endregion

        #region Can Read

        public static bool CanReadMessages(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Messages);
        }

        public static bool CanReadMessages(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanReadMessages(project, CurrentUserId);
        }

        public static bool CanReadFiles(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Files);
        }

        public static bool CanReadFiles(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanReadFiles(project, CurrentUserId);
        }

        public static bool CanReadTasks(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Tasks);
        }

        public static bool CanReadTasks(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanReadTasks(project, CurrentUserId);
        }

        public static bool CanLinkContact(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanEdit(project);
        }

        public static bool CanReadMilestones(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Milestone);
        }

        public static bool CanReadMilestones(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanReadMilestones(project, CurrentUserId);
        }

        public static bool CanReadContacts(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            return GetTeamSecurity(project, userId, ProjectTeamSecurity.Contacts);
        }

        public static bool CanReadContacts(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanReadContacts(project, CurrentUserId);
        }

        public static bool CanRead(Project project, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            if (project == null) return false;
            return !project.Private || IsInTeam(project, userId);
        }

        public static bool CanRead(Project project)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanRead(project, CurrentUserId);
        }

        public static bool CanRead(Task task, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            if (task == null || !CanRead(task.Project, userId)) return false;

            if (task.Responsibles.Contains(userId)) return true;

            if (!CanReadTasks(task.Project, userId)) return false;
            if (task.Milestone != 0 && !CanReadMilestones(task.Project, userId))
            {
                var m = GetFactory().GetMilestoneDao().GetById(task.Milestone);
                if (!CanRead(m, userId)) return false;
            }

            return true;
        }

        public static bool CanRead(Task task)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanRead(task, CurrentUserId);
        }

        public static bool CanRead(Subtask subtask)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            if (subtask == null) return false;
            return subtask.Responsible == CurrentUserId;
        }

        public static bool CanRead(Milestone milestone, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            if (milestone == null || !CanRead(milestone.Project, userId)) return false;

            if (milestone.Responsible == userId) return true;

            return CanReadMilestones(milestone.Project, userId);
        }

        public static bool CanRead(Milestone milestone)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanRead(milestone, CurrentUserId);
        }

        public static bool CanRead(Message message, Guid userId)
        {
            if (!IsProjectsEnabled(userId)) return false;
            if (message == null || !CanRead(message.Project, userId)) return false;

            return CanReadMessages(message.Project, userId);
        }

        public static bool CanRead(Message message)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return CanRead(message, CurrentUserId);
        }

        public static bool CanReadGantt(Project project)
        {
            if (!ganttJsExists.HasValue && HttpContext.Current != null)
            {
                var file = HttpContext.Current.Server.MapPath("~/products/projects/js/ganttchart_min.js ");
                ganttJsExists = File.Exists(file);
            }
            if (ganttJsExists.HasValue && ganttJsExists.Value == false)
            {
                return false;
            }
            return CanReadTasks(project) && CanReadMilestones(project);
        }

        #endregion

        #region Can Create

        public static bool Can()
        {
            if (CurrentUserIsVisitor) return false;
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return true;
        }

        public static bool Can(object obj)
        {
            if (!Can()) return false;
            if (obj == null) return false;
            return true;
        }

        public static bool CanCreateProject()
        {
            if (!Can()) return false;
            return CurrentUserAdministrator;
        }

        public static bool CanCreateMilestone(Project project)
        {
            if (!Can(project)) return false;
            return IsProjectManager(project);
        }

        public static bool CanCreateMessage(Project project)
        {
            if (!Can(project)) return false;
            if (IsProjectManager(project)) return true;
            return IsInTeam(project) && CanReadMessages(project);
        }

        public static bool CanCreateTask(Project project)
        {
            if (!Can(project)) return false;
            if (IsProjectManager(project)) return true;
            return IsInTeam(project) && CanReadTasks(project);
        }

        public static bool CanCreateSubtask(Task task)
        {
            if (!Can(task)) return false;
            if (IsProjectManager(task.Project)) return true;

            return IsInTeam(task.Project) &&
                   ((task.CreateBy == CurrentUserId) ||
                    !task.Responsibles.Any() ||
                    task.Responsibles.Contains(CurrentUserId));
        }

        public static bool CanCreateComment()
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            return SecurityContext.IsAuthenticated;
        }

        public static bool CanCreateTimeSpend(Project project)
        {
            if (!Can(project)) return false;
            return IsInTeam(project);
        }

        public static bool CanCreateTimeSpend(Task task)
        {
            if (!Can(task)) return false;
            if (IsInTeam(task.Project)) return true;

            return task.Responsibles.Contains(CurrentUserId) ||
                   task.SubTasks.Select(r => r.Responsible).Contains(CurrentUserId);
        }

        #endregion

        #region Can Edit

        public static bool CanEdit(Project project)
        {
            if (!Can(project)) return false;
            return IsProjectManager(project);
        }

        public static bool CanEdit(Milestone milestone)
        {
            if (!Can(milestone)) return false;
            if (IsProjectManager(milestone.Project)) return true;
            if (!CanRead(milestone)) return false;

            return IsInTeam(milestone.Project) &&
                   (milestone.CreateBy == CurrentUserId ||
                    milestone.Responsible == CurrentUserId);
        }

        public static bool CanEdit(Message message)
        {
            if (!Can(message)) return false;
            if (IsProjectManager(message.Project)) return true;
            if (!CanRead(message)) return false;

            return IsInTeam(message.Project) && message.CreateBy == CurrentUserId;
        }

        public static bool CanEdit(Task task)
        {
            if (!Can(task)) return false;
            if (IsProjectManager(task.Project)) return true;

            return IsInTeam(task.Project) &&
                   (task.CreateBy == CurrentUserId ||
                    !task.Responsibles.Any() ||
                    task.Responsibles.Contains(CurrentUserId) ||
                    task.SubTasks.Select(r => r.Responsible).Contains(CurrentUserId));
        }

        public static bool CanEdit(Task task, Subtask subtask)
        {
            if (!Can(subtask)) return false;
            if (CanEdit(task)) return true;

            return IsInTeam(task.Project) &&
                   (subtask.CreateBy == CurrentUserId ||
                    subtask.Responsible == CurrentUserId);
        }

        public static bool CanEditTeam(Project project)
        {
            if (!Can(project)) return false;
            return IsProjectManager(project);
        }

        public static bool CanEditComment(Project project, Comment comment)
        {
            if (!IsProjectsEnabled(CurrentUserId)) return false;
            if (project == null || comment == null) return false;
            return comment.CreateBy == CurrentUserId || IsProjectManager(project);
        }

        public static bool CanEdit(TimeSpend timeSpend)
        {
            if (!Can(timeSpend)) return false;
            if (IsProjectManager(timeSpend.Task.Project)) return true;
            if (timeSpend.PaymentStatus == PaymentStatus.Billed) return false;

            return timeSpend.Person == CurrentUserId || timeSpend.CreateBy == CurrentUserId;
        }

        public static bool CanEditPaymentStatus(TimeSpend timeSpend)
        {
            if (!Can(timeSpend)) return false;
            return IsProjectManager(timeSpend.Task.Project);
        }

        #endregion

        #region Can Delete

        public static bool CanDelete(Task task)
        {
            if (!Can(task)) return false;
            if (IsProjectManager(task.Project)) return true;

            return IsInTeam(task.Project) && task.CreateBy == CurrentUserId;
        }

        public static bool CanDelete(TimeSpend timeSpend)
        {
            if (!Can(timeSpend)) return false;
            if (IsProjectManager(timeSpend.Task.Project)) return true;
            if (timeSpend.PaymentStatus == PaymentStatus.Billed) return false;

            return IsInTeam(timeSpend.Task.Project) &&
                   (timeSpend.CreateBy == CurrentUserId || timeSpend.Person == CurrentUserId);
        }

        #endregion

        #region Demand

        public static void DemandCreateProject()
        {
            if (!CanCreateProject()) throw CreateSecurityException();
        }

        public static void DemandCreateMessage(Project project)
        {
            if (!CanCreateMessage(project)) throw CreateSecurityException();
        }

        public static void DemandCreateMilestone(Project project)
        {
            if (!CanCreateMilestone(project)) throw CreateSecurityException();
        }

        public static void DemandCreateTask(Project project)
        {
            if (!CanCreateTask(project)) throw CreateSecurityException();
        }

        public static void DemandCreateComment()
        {
            if (!CanCreateComment()) throw CreateSecurityException();
        }


        public static void DemandRead(Milestone milestone)
        {
            if (!CanRead(milestone != null ? milestone.Project : null)) throw CreateSecurityException();
        }

        public static void DemandRead(Message message)
        {
            if (!CanRead(message)) throw CreateSecurityException();
        }

        public static void DemandRead(Task task)
        {
            if (!CanRead(task)) throw CreateSecurityException();
        }

        public static void DemandReadFiles(Project project)
        {
            if (!CanReadFiles(project)) throw CreateSecurityException();
        }

        public static void DemandReadTasks(Project project)
        {
            if (!CanReadTasks(project)) throw CreateSecurityException();
        }

        public static void DemandLinkContact(Project project)
        {
            if (!CanEdit(project)) throw CreateSecurityException();
        }


        public static void DemandEdit(Project project)
        {
            if (!CanEdit(project)) throw CreateSecurityException();
        }

        public static void DemandEdit(Message message)
        {
            if (!CanEdit(message)) throw CreateSecurityException();
        }

        public static void DemandEdit(Milestone milestone)
        {
            if (!CanEdit(milestone)) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task)
        {
            if (!CanEdit(task)) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task, Subtask subtask)
        {
            if (!CanEdit(task, subtask)) throw CreateSecurityException();
        }

        public static void DemandEdit(TimeSpend timeSpend)
        {
            if (!CanEdit(timeSpend)) throw CreateSecurityException();
        }

        public static void DemandEditTeam(Project project)
        {
            if (!CanEditTeam(project)) throw CreateSecurityException();
        }

        public static void DemandEditComment(Project project, Comment comment)
        {
            if (!CanEditComment(project, comment)) throw CreateSecurityException();
        }


        public static void DemandDeleteTimeSpend(TimeSpend timeSpend)
        {
            if (!CanDelete(timeSpend)) throw CreateSecurityException();
        }

        public static void DemandAuthentication()
        {
            if (!SecurityContext.CurrentAccount.IsAuthenticated)
            {
                throw CreateSecurityException();
            }
        }

        #endregion

        #region GetFactory

        private static Core.DataInterfaces.IDaoFactory GetFactory()
        {
            return new DaoFactory("projects", CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        #endregion

        #region Is.. block

        public static bool IsAdministrator(Guid userId)
        {
            return CoreContext.UserManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
                   WebItemSecurity.IsProductAdministrator(EngineFactory.ProductId, userId);
        }

        private static bool IsProjectManager(Project project)
        {
            return IsProjectManager(project, CurrentUserId);
        }

        private static bool IsProjectManager(Project project, Guid userId)
        {
            return (IsAdministrator(userId) || (project != null && project.Responsible == userId)) &&
                   !CurrentUserIsVisitor;
        }

        public static bool IsVisitor(Guid userId)
        {
            return CoreContext.UserManager.GetUsers(userId).IsVisitor();
        }

        public static bool IsInTeam(Project project)
        {
            return IsInTeam(project, CurrentUserId);
        }

        public static bool IsInTeam(Project project, Guid userId)
        {
            return IsInTeam(project, userId, true);
        }

        public static bool IsInTeam(Project project, Guid userId, bool includeAdmin)
        {
            var isAdmin = includeAdmin && IsAdministrator(userId);
            return isAdmin || (project != null && GetFactory().GetProjectDao().IsInTeam(project.ID, userId));
        }

        private static bool IsFollow(Project project, Guid userId)
        {
            var isAdmin = IsAdministrator(userId);
            var isPrivate = project != null && (!project.Private || isAdmin);

            return isPrivate && GetFactory().GetProjectDao().IsFollow(project.ID, userId);
        }

        #endregion

        #region TeamSecurity

        private static bool GetTeamSecurity(Project project, Guid userId, ProjectTeamSecurity security)
        {
            if (IsProjectManager(project, userId) || project == null || !project.Private) return true;
            var dao = GetFactory().GetProjectDao();
            var s = dao.GetTeamSecurity(project.ID, userId);
            return (s & security) != security && dao.IsInTeam(project.ID, userId);
        }

        private static bool GetTeamSecurityForParticipants(Project project, Guid userId, ProjectTeamSecurity security)
        {
            if (IsProjectManager(project, userId) || !project.Private) return true;
            var dao = GetFactory().GetProjectDao();
            var s = dao.GetTeamSecurity(project.ID, userId);
            return (s & security) != security;
        }

        #endregion

        #region Exeption

        public static Exception CreateSecurityException()
        {
            throw new System.Security.SecurityException("Access denied.");
        }

        public static Exception CreateGuestSecurityException()
        {
            throw new System.Security.SecurityException("A guest cannot be appointed as responsible.");
        }

        #endregion
    }
}