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
using System.Security;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class TimeTrackingEngine
    {
        private readonly ITimeSpendDao _timeSpendDao;
        private readonly ITaskDao _taskDao;


        public TimeTrackingEngine(IDaoFactory daoFactory)
        {
            _timeSpendDao = daoFactory.GetTimeSpendDao();
            daoFactory.GetProjectDao();
            _taskDao = daoFactory.GetTaskDao();
        }

        public List<TimeSpend> GetByFilter(TaskFilter filter)
        {
            var listTimeSpend = new List<TimeSpend>();

            while (true)
            {
                var timeSpend = _timeSpendDao.GetByFilter(filter);
                timeSpend = GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task));

                if (filter.LastId != 0)
                {
                    var lastTimeSpendIndex = timeSpend.FindIndex(r => r.ID == filter.LastId);

                    if (lastTimeSpendIndex >= 0)
                    {
                        timeSpend = timeSpend.SkipWhile((r, index) => index <= lastTimeSpendIndex).ToList();
                    }
                }

                listTimeSpend.AddRange(timeSpend);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listTimeSpend = listTimeSpend.Take((int)filter.Max).ToList();

                if (listTimeSpend.Count == filter.Max || timeSpend.Count == 0) break;

                if (listTimeSpend.Count != 0)
                    filter.LastId = listTimeSpend.Last().ID;

                filter.Offset += filter.Max;
            }

            return listTimeSpend;
        }

        public float GetByFilterTotal(TaskFilter filter)
        {
            return GetByFilter(filter).Sum(r => r.Hours);
        }

        public float GetByFilterTotalBilled(TaskFilter filter)
        {
            return GetByFilter(filter).Where(r => r.PaymentStatus == PaymentStatus.Billed).Sum(r => r.Hours);
        }

        public List<TimeSpend> GetByTask(int taskId)
        {
            var timeSpend = _timeSpendDao.GetByTask(taskId);
            return GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task));
        }

        public List<TimeSpend> GetByProject(int projectId)
        {
            var timeSpend = _timeSpendDao.GetByProject(projectId);
            return GetTasks(timeSpend).FindAll(r => ProjectSecurity.CanRead(r.Task));
        }

        public TimeSpend GetByID(int id)
        {
            var timeSpend = _timeSpendDao.GetById(id);
            timeSpend.Task = _taskDao.GetById(timeSpend.Task.ID);
            return timeSpend;
        }

        public TimeSpend SaveOrUpdate(TimeSpend timeSpend)
        {
            ProjectSecurity.DemandEdit(timeSpend);

            // check guest responsible
            if (ProjectSecurity.IsVisitor(timeSpend.Person))
            {
                ProjectSecurity.CreateGuestSecurityException();
            }

            timeSpend.CreateOn = DateTime.UtcNow;
            return _timeSpendDao.Save(timeSpend);
        }

        public TimeSpend ChangePaymentStatus(TimeSpend timeSpend, PaymentStatus newStatus)
        {
            if (!ProjectSecurity.CanEditPaymentStatus(timeSpend)) throw new SecurityException("Access denied.");

            if (timeSpend == null) throw new ArgumentNullException("timeSpend");

            var task = _taskDao.GetById(timeSpend.Task.ID);

            if (task == null) throw new Exception("Task can't be null.");

            ProjectSecurity.DemandEdit(task);
            ProjectSecurity.DemandEdit(timeSpend);

            if (timeSpend.PaymentStatus == newStatus) return timeSpend;

            timeSpend.PaymentStatus = newStatus;

            return _timeSpendDao.Save(timeSpend);
        }

        public void Delete(TimeSpend timeSpend)
        {
            ProjectSecurity.DemandDeleteTimeSpend(timeSpend);
            _timeSpendDao.Delete(timeSpend.ID);
        }

        private List<TimeSpend> GetTasks(List<TimeSpend> listTimeSpend)
        {
            var listTasks = _taskDao.GetById(listTimeSpend.Select(r => r.Task.ID).ToList());

            listTimeSpend.ForEach(t => t.Task = listTasks.Find(task => task.ID == t.Task.ID));

            return listTimeSpend;
        }
    }
}