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

namespace ASC.Projects.Engine
{
    public class MilestoneEngine
    {
        private readonly EngineFactory _engineFactory;
        private readonly IMilestoneDao _milestoneDao;

        public MilestoneEngine(IDaoFactory daoFactory, EngineFactory engineFactory)
        {
            _engineFactory = engineFactory;
            _milestoneDao = daoFactory.GetMilestoneDao();
        }

        #region Get Milestones

        public IEnumerable<Milestone> GetAll()
        {
            return _milestoneDao.GetAll().Where(CanRead);
        }

        public List<Milestone> GetByFilter(TaskFilter filter)
        {
            var listMilestones = new List<Milestone>();

            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);

            while (true)
            {
                var milestones = _milestoneDao.GetByFilter(filter, isAdmin);

                if (filter.LastId != 0)
                {
                    var lastMilestoneIndex = milestones.FindIndex(r => r.ID == filter.LastId);

                    if (lastMilestoneIndex >= 0)
                    {
                        milestones = milestones.SkipWhile((r, index) => index <= lastMilestoneIndex).ToList();
                    }
                }

                listMilestones.AddRange(milestones);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listMilestones = listMilestones.Take((int) filter.Max).ToList();

                if (listMilestones.Count == filter.Max || milestones.Count == 0) break;

                if (listMilestones.Count != 0)
                    filter.LastId = listMilestones.Last().ID;

                filter.Offset += filter.Max;
            }

            return listMilestones;
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            var isAdmin = ProjectSecurity.IsAdministrator(SecurityContext.CurrentAccount.ID);
            return _milestoneDao.GetByFilterCount(filter, isAdmin);
        }

        public List<Milestone> GetByProject(int projectId)
        {
            var milestones = _milestoneDao.GetByProject(projectId).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus)
        {
            var milestones = _milestoneDao.GetByStatus(projectId, milestoneStatus).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetUpcomingMilestones(int max, params int[] projects)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = _milestoneDao.GetUpcomingMilestones(offset, 2 * max, projects);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetLateMilestones(int max, params int[] projects)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = _milestoneDao.GetLateMilestones(offset, 2 * max, projects);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetByDeadLine(DateTime deadline)
        {
            return _milestoneDao.GetByDeadLine(deadline).Where(CanRead).ToList();
        }

        public Milestone GetByID(int id)
        {
            return GetByID(id, true);
        }

        public Milestone GetByID(int id, bool checkSecurity)
        {
            var m = _milestoneDao.GetById(id);

            if (!checkSecurity)
                return m;

            return CanRead(m) ? m : null;
        }

        public bool IsExists(int id)
        {
            return GetByID(id) != null;
        }

        public string GetLastModified()
        {
            return _milestoneDao.GetLastModified();
        }

        private static bool CanRead(Milestone m)
        {
            return ProjectSecurity.CanRead(m);
        }

        #endregion

        #region Save, Delete, Notify

        public Milestone SaveOrUpdate(Milestone milestone)
        {
            return SaveOrUpdate(milestone, false, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible)
        {
            return SaveOrUpdate(milestone, notifyResponsible, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible, bool import)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("milestone.project is null");
            if (milestone.Responsible.Equals(Guid.Empty)) throw new Exception("milestone.responsible is empty");

            // check guest responsible
            if (ProjectSecurity.IsVisitor(milestone.Responsible))
            {
                ProjectSecurity.CreateGuestSecurityException();
            }

            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();

            var isNew = milestone.ID == default(int);//Task is new
            var oldResponsible = Guid.Empty;

            if (isNew)
            {
                if (milestone.CreateBy == default(Guid)) milestone.CreateBy = SecurityContext.CurrentAccount.ID;
                if (milestone.CreateOn == default(DateTime)) milestone.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreateMilestone(milestone.Project);
                milestone = _milestoneDao.Save(milestone);
            }
            else
            {
                var oldMilestone = _milestoneDao.GetById(new[] {milestone.ID}).FirstOrDefault();

                if (oldMilestone == null) throw new ArgumentNullException("milestone");

                oldResponsible = oldMilestone.Responsible;

                ProjectSecurity.DemandEdit(milestone);
                milestone = _milestoneDao.Save(milestone);

            }


            if (!milestone.Responsible.Equals(Guid.Empty))
                NotifyMilestone(milestone, notifyResponsible, isNew, oldResponsible);

            return milestone;
        }

        public Milestone ChangeStatus(Milestone milestone, MilestoneStatus newStatus)
        {
            ProjectSecurity.DemandEdit(milestone);

            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("Project can be null.");
            if (milestone.Status == newStatus) return milestone;
            if (milestone.ActiveTaskCount != 0 && newStatus == MilestoneStatus.Closed) throw new Exception("Can not close a milestone with open tasks");

            milestone.Status = newStatus;
            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();
            milestone.StatusChangedOn = TenantUtil.DateTimeNow();

            var senders = new HashSet<Guid> { milestone.Project.Responsible, milestone.CreateBy, milestone.Responsible };

            if (newStatus == MilestoneStatus.Closed && !_engineFactory.DisableNotifications && senders.Count != 0)
            {
                NotifyClient.Instance.SendAboutMilestoneClosing(senders, milestone);
            }

            if (newStatus == MilestoneStatus.Open && !_engineFactory.DisableNotifications && senders.Count != 0)
            {
                NotifyClient.Instance.SendAboutMilestoneResumed(senders, milestone);
            }

            return _milestoneDao.Save(milestone);
        }

        private void NotifyMilestone(Milestone milestone, bool notifyResponsible, bool isNew, Guid oldResponsible)
        {
            //Don't send anything if notifications are disabled
            if (_engineFactory.DisableNotifications) return;

            if (isNew && milestone.Project.Responsible != SecurityContext.CurrentAccount.ID && !milestone.Project.Responsible.Equals(milestone.Responsible))
            {
                NotifyClient.Instance.SendAboutMilestoneCreating(new List<Guid> { milestone.Project.Responsible }, milestone);
            }

            if (notifyResponsible && milestone.Responsible != SecurityContext.CurrentAccount.ID)
            {
                if (isNew || !oldResponsible.Equals(milestone.Responsible))
                    NotifyClient.Instance.SendAboutResponsibleByMilestone(milestone);
                else
                    NotifyClient.Instance.SendAboutMilestoneEditing(milestone);
            }
        }


        public void Delete(Milestone milestone)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");

            ProjectSecurity.DemandEdit(milestone);
            _milestoneDao.Delete(milestone.ID);

            var users = new HashSet<Guid> { milestone.Project.Responsible, milestone.Responsible };

            NotifyClient.Instance.SendAboutMilestoneDeleting(users, milestone);
        }

        #endregion
    }
}