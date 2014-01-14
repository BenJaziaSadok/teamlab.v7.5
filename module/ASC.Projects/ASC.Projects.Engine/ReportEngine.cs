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
using System.Globalization;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;

namespace ASC.Projects.Engine
{
    public class ReportEngine
    {
        private readonly IReportDao reportDao;
        private readonly IProjectDao projectDao;

        public ReportEngine(IDaoFactory daoFactory)
        {
            reportDao = daoFactory.GetReportDao();
            projectDao = daoFactory.GetProjectDao();
        }


        public List<ReportTemplate> GetTemplates(Guid userId)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new System.Security.SecurityException("Access denied.");

            return reportDao.GetTemplates(userId);
        }

        public List<ReportTemplate> GetAutoTemplates()
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new System.Security.SecurityException("Access denied.");

            return reportDao.GetAutoTemplates();
        }

        public ReportTemplate GetTemplate(int id)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new System.Security.SecurityException("Access denied.");

            return reportDao.GetTemplate(id);
        }

        public ReportTemplate SaveTemplate(ReportTemplate template)
        {
            if (template == null) throw new ArgumentNullException("template");

            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new System.Security.SecurityException("Access denied.");

            if (template.CreateOn == default(DateTime)) template.CreateOn = TenantUtil.DateTimeNow();
            if (template.CreateBy.Equals(Guid.Empty)) template.CreateBy = SecurityContext.CurrentAccount.ID;
            return reportDao.SaveTemplate(template);
        }

        public void DeleteTemplate(int id)
        {
            if (ProjectSecurity.IsVisitor(SecurityContext.CurrentAccount.ID)) throw new System.Security.SecurityException("Access denied.");

            reportDao.DeleteTemplate(id);
        }


        public IList<object[]> BuildUsersWithoutActiveTasks(TaskFilter filter)
        {
            var result = new List<object[]>();

            var users = new List<Guid>();
            if (filter.UserId != Guid.Empty) users.Add(filter.UserId);
            else if (filter.DepartmentId != Guid.Empty)
            {
                users.AddRange(CoreContext.UserManager.GetUsersByGroup(filter.DepartmentId).Select(u => u.ID));
            }
            else if (filter.HasProjectIds)
            {
                users.AddRange(projectDao.GetTeam(filter.ProjectIds).Select(r => r.ID));
            }
            else if (!filter.HasProjectIds)
            {
                users.AddRange(CoreContext.UserManager.GetUsers().Select(u => u.ID));
            }

            foreach (var row in reportDao.BuildUsersStatisticsReport(filter))
            {
                users.Remove((Guid)row[0]);
                if ((long)row[1] == 0 && (long)row[2] == 0)
                {
                    result.Add(row);
                }
            }
            result.AddRange(users.Select(u => new object[] { u, 0, 0, 0 }));

            return result;
        }

        public IList<object[]> BuildUsersWorkload(TaskFilter filter)
        {
            return reportDao.BuildUsersStatisticsReport(filter);
        }

        public IList<object[]> BuildUsersActivityReport(TaskFilter filter)
        {
            return reportDao.BuildUsersActivityReport(filter);
        }
    }
}
