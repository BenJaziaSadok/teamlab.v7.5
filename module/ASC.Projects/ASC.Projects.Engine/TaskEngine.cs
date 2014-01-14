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

using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Services.NotifyService;
using IDaoFactory = ASC.Projects.Core.DataInterfaces.IDaoFactory;

namespace ASC.Projects.Engine
{
    public class TaskEngine : ProjectEntityEngine
    {
        private readonly EngineFactory factory;
        private readonly ITaskDao taskDao;
        private readonly IMilestoneDao milestoneDao;
        private readonly ISubtaskDao subtaskDao;

        public TaskEngine(IDaoFactory daoFactory, EngineFactory factory)
            : base(NotifyConstants.Event_NewCommentForTask, factory)
        {
            this.factory = factory;
            taskDao = daoFactory.GetTaskDao();
            milestoneDao = daoFactory.GetMilestoneDao();
            subtaskDao = daoFactory.GetSubtaskDao();
        }

        #region Get Tasks

        public IEnumerable<Task> GetAll()
        {
            return taskDao.GetAll().Where(CanRead);
        }

        public List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant)
        {
            var listTask = taskDao.GetByProject(projectId, status, participant).Where(CanRead).ToList();
            subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public TaskFilterOperationResult GetByFilter (TaskFilter filter)
        {
            if (filter.Offset < 0 || filter.Max < 0)
                return null;

            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
            
            var count = taskDao.GetByFilterCount(filter, isAdmin);
            
            var filterLimit = filter.Max;
            var filterOffset = filter.Offset;

            if (filterOffset > count.TasksTotal)
                return new TaskFilterOperationResult(count); //there are some records but we cant see them due to offset

            var taskList = new List<Task>();
            if (filter.HasTaskStatuses)
            {
                taskList = taskDao.GetByFilter(filter, isAdmin);
            }
            else if (filterOffset > count.TasksOpen && count.TasksClosed != 0)
            {
                filter.TaskStatuses.Add(TaskStatus.Closed);
                filter.Offset = filterOffset - count.TasksOpen;
                taskList = taskDao.GetByFilter(filter, isAdmin);
            }
            else
            {
                //TODO: to one sql query using UNION ALL
                if (count.TasksOpen != 0)
                {
                    filter.TaskStatuses.Add(TaskStatus.Open);
                    taskList = taskDao.GetByFilter(filter, isAdmin);
                }

                if (taskList.Count < filterLimit && count.TasksClosed != 0)
                {
                    filter.TaskStatuses.Clear();
                    filter.TaskStatuses.Add(TaskStatus.Closed);
                    filter.Offset = 0;
                    filter.Max = filterLimit - taskList.Count;
                    taskList.AddRange(taskDao.GetByFilter(filter, isAdmin));
                }
            }

            filter.Offset = filterOffset;
            filter.Max = filterLimit;
            filter.TaskStatuses.Clear();

            subtaskDao.GetSubtasks(ref taskList);

            var taskLinks = taskDao.GetLinks(taskList).ToList();

            taskList = taskList.GroupJoin(taskLinks, task => task.ID, link => link.DependenceTaskId, (task, linksCol) =>
            {
                task.Links.AddRange(linksCol);
                return task;
            }).ToList();

            taskList = taskList.GroupJoin(taskLinks, task => task.ID, link => link.ParentTaskId, (task, linksCol) =>
            {
                task.Links.AddRange(linksCol);
                return task;
            }).ToList();

            return new TaskFilterOperationResult(taskList, count);
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
            return taskDao.GetByFilterCount(filter, isAdmin).TasksTotal;
        }

        public List<Task> GetByResponsible(Guid responsibleId, params TaskStatus[] statuses)
        {
            var listTask = taskDao.GetByResponsible(responsibleId, statuses).Where(CanRead).ToList();
            subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public List<Task> GetMilestoneTasks(int milestoneId)
        {
            var listTask = taskDao.GetMilestoneTasks(milestoneId).Where(CanRead).ToList();
            subtaskDao.GetSubtasks(ref listTask);
            return listTask;
        }

        public Task GetByID(int id)
        {
            return GetByID(id, true);
        }

        public Task GetByID(int id, bool checkSecurity)
        {
            var task = taskDao.GetById(id);

            if (task != null)
            {
                task.SubTasks = subtaskDao.GetSubtasks(task.ID);
                task.Links = taskDao.GetLinks(task.ID).ToList();
            }

            if (!checkSecurity)
                return task;
            
            return CanRead(task) ? task : null;
        }

        public List<Task> GetByID(ICollection<int> ids)
        {
            var listTask = taskDao.GetById(ids).Where(CanRead).ToList();
            subtaskDao.GetSubtasks(ref listTask);
            return listTask;

        }

        public bool IsExists(int id)
        {
            return taskDao.IsExists(id);
        }

        private static bool CanRead(Task task)
        {
            return ProjectSecurity.CanRead(task);
        }

        #endregion

        #region Save, Delete, Notify

        public Task SaveOrUpdate(Task task, IEnumerable<int> attachedFileIds, bool notifyResponsible)
        {
            return SaveOrUpdate(task, attachedFileIds, notifyResponsible, false);
        }

        public Task SaveOrUpdate(Task task, IEnumerable<int> attachedFileIds, bool notifyResponsible, bool isImport)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("task.Project");

            // check guests responsibles
            foreach (var responsible in task.Responsibles)
            {
                if (ProjectSecurity.IsVisitor(responsible))
                {
                    ProjectSecurity.CreateGuestSecurityException();
                }
            }

            var milestone = task.Milestone != 0 ? milestoneDao.GetById(task.Milestone) : null;
            var milestoneResponsible = milestone != null ? milestone.Responsible : Guid.Empty;

            var removeResponsibles = new List<Guid>();
            var inviteToResponsibles = new List<Guid>();

            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();

            var isNew = task.ID == default(int); //Task is new

            if (isNew)
            {
                if (task.CreateBy == default(Guid)) task.CreateBy = SecurityContext.CurrentAccount.ID;
                if (task.CreateOn == default(DateTime)) task.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateTask(task.Project);

                task = taskDao.Save(task);

                inviteToResponsibles.AddRange(task.Responsibles);
            }
            else
            {
                var oldTask = GetByID(new[] {task.ID}).FirstOrDefault();

                if (oldTask == null) throw new ArgumentNullException("task");

                var newResponsibles = task.Responsibles.ToList();
                var oldResponsibles = oldTask.Responsibles.ToList();

                removeResponsibles.AddRange(oldResponsibles.Where(p => !newResponsibles.Contains(p)));
                inviteToResponsibles.AddRange(newResponsibles.Where(participant => !oldResponsibles.Contains(participant)));

                //changed task
                ProjectSecurity.DemandEdit(oldTask);

                task = taskDao.Save(task);
            }

            if (attachedFileIds != null && attachedFileIds.Any())
            {
                foreach (var attachedFileId in attachedFileIds)
                {
                    AttachFile(task, attachedFileId, false);
                }
            }

            var senders = new HashSet<Guid>(task.Responsibles) { task.Project.Responsible, milestoneResponsible, task.CreateBy };
            senders.Remove(Guid.Empty);

            foreach (var subscriber in senders)
            {
                Subscribe(task, subscriber);
            }

            inviteToResponsibles.RemoveAll(r => r.Equals(Guid.Empty));
            removeResponsibles.RemoveAll(r => r.Equals(Guid.Empty));

            NotifyTask(task, inviteToResponsibles, removeResponsibles, isNew, notifyResponsible);

            return task;
        }

        public Task ChangeStatus(Task task, TaskStatus newStatus)
        {
            ProjectSecurity.DemandEdit(task);

            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("Project can be null.");
            if (task.Status == newStatus) return task;

            var senders = GetSubscribers(task);

            if (newStatus == TaskStatus.Closed && !factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutTaskClosing(senders, task);

            if (newStatus == TaskStatus.Open && !factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutTaskResumed(senders, task);

            task.Status = newStatus;
            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();
            task.StatusChangedOn = TenantUtil.DateTimeNow();

            //subtask
            if (newStatus == TaskStatus.Closed)
            {
                if (!task.Responsibles.Any())
                    task.Responsibles.Add(SecurityContext.CurrentAccount.ID);

                subtaskDao.CloseAllSubtasks(task);
            }

            return taskDao.Save(task);
        }

        public Task MoveToMilestone(Task task, int milestoneID)
        {
            ProjectSecurity.DemandEdit(task);

            if (task == null) throw new ArgumentNullException("task");
            if (task.Project == null) throw new Exception("Project can be null.");

            var newMilestone = milestoneID != 0;
            var milestone = milestoneDao.GetById(newMilestone ? milestoneID : task.Milestone);

            var senders = GetSubscribers(task);

            if (!factory.DisableNotifications && senders.Count != 0)
                NotifyClient.Instance.SendAboutTaskRemoved(senders, task, milestone, newMilestone);

            task.Milestone = milestoneID;
            task.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            task.LastModifiedOn = TenantUtil.DateTimeNow();

            return taskDao.Save(task);
        }

        public void NotifyTask(Task task, IEnumerable<Guid> inviteToResponsibles, IEnumerable<Guid> removeResponsibles, bool isNew, bool notifyResponsible)
        {
            if (factory.DisableNotifications) return;

            var senders = GetSubscribers(task);
            senders = senders.FindAll(r => !inviteToResponsibles.Contains(new Guid(r.ID)) && !removeResponsibles.Contains(new Guid(r.ID)));

            if (senders.Any())
            {
                if (isNew)
                {
                    NotifyClient.Instance.SendAboutTaskCreating(senders, task);
                }
                else
                {
                    NotifyClient.Instance.SendAboutTaskEditing(senders, task);
                }
            }

            if (notifyResponsible)
                NotifyResponsible(task, inviteToResponsibles.ToList(), removeResponsibles.ToList());
        }

        private void NotifyResponsible(Task task, List<Guid> inviteToResponsibles, List<Guid> removeResponsibles)
        {
            if (factory.DisableNotifications) return;

            if (inviteToResponsibles.Any())
                NotifyClient.Instance.SendAboutResponsibleByTask(inviteToResponsibles, task);

            if (removeResponsibles.Any())
                NotifyClient.Instance.SendAboutRemoveResponsibleByTask(removeResponsibles, task);
        }

        public void SendReminder(Task task)
        {
            //Don't send anything if notifications are disabled
            if (factory.DisableNotifications || task.Responsibles == null || !task.Responsibles.Any()) return;

            NotifyClient.Instance.SendReminderAboutTask(task.Responsibles, task);
        }


        public void Delete(Task task)
        {
            if (task == null) throw new ArgumentNullException("task");

            ProjectSecurity.DemandEdit(task);
            task.Links.ForEach(link => taskDao.RemoveLink(link));
            task.SubTasks.ForEach(subTask => subtaskDao.Delete(subTask.ID));
            taskDao.Delete(task.ID);

            var recipients = GetSubscribers(task);

            if (recipients.Count != 0)
            {
                NotifyClient.Instance.SendAboutTaskDeleting(recipients, task);
            }
        }

        #endregion

        #region Link

        public void AddLink(Task parentTask, Task dependentTask, TaskLinkType linkType)
        {
            CheckLink(parentTask, dependentTask, linkType);

            var link = new TaskLink
            {
                ParentTaskId = parentTask.ID,
                DependenceTaskId = dependentTask.ID,
                LinkType = linkType
            };

            if (taskDao.IsExistLink(link))
                throw new Exception("link already exist");

            ProjectSecurity.DemandEdit(dependentTask);
            ProjectSecurity.DemandEdit(parentTask);

            taskDao.AddLink(link);
        }

        public void RemoveLink(Task dependentTask, Task parentTask)
        {
            ProjectSecurity.DemandEdit(dependentTask);

            taskDao.RemoveLink(new TaskLink {DependenceTaskId = dependentTask.ID, ParentTaskId = parentTask.ID});
        }

        private static void CheckLink(Task parentTask, Task dependentTask)
        {
            if (parentTask == null) throw new ArgumentNullException("parentTask");
            if (dependentTask == null) throw new ArgumentNullException("dependentTask");

            if (parentTask.ID == dependentTask.ID)
            {
                throw new Exception("it is impossible to create a link between one and the same task");
            }

/*            if (parentTask.Status == TaskStatus.Closed || dependentTask.Status == TaskStatus.Closed)
            {
                throw new Exception("Such link don't be created. Task closed.");
            }*/

            if (parentTask.Milestone != dependentTask.Milestone)
            {
                throw new Exception("Such link don't be created. Different Milestones");
            }

        }

        private static void CheckLink(Task parentTask, Task dependentTask, TaskLinkType linkType)
        {
            CheckLink(parentTask, dependentTask);

            switch (linkType)
            {
                case TaskLinkType.End:
                    if ((parentTask.Deadline.Equals(DateTime.MinValue) && parentTask.Milestone == 0) || (dependentTask.Deadline.Equals(DateTime.MinValue) && dependentTask.Milestone == 0))
                    {
                        throw new Exception("Such link don't be created. Incorrect task link type.");
                    }
                    break;

                case TaskLinkType.EndStart:
                    if ((parentTask.Deadline.Equals(DateTime.MinValue) && parentTask.Milestone == 0))
                    {
                        throw new Exception("Such link don't be created. Incorrect task link type.");
                    }
                    break;
            }
        }

        #endregion
    }
}
