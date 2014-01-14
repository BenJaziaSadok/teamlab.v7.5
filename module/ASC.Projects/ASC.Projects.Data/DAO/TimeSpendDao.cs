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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    class TimeSpendDao : BaseDao, ITimeSpendDao
    {
        private readonly string[] columns = new[] { "id", "note", "date", "hours", "relative_task_id", "person_id", "project_id", "create_on", "create_by", 
            "payment_status", "status_changed" };

        public TimeSpendDao(string dbId, int tenantID) : base(dbId, tenantID) { }

        public List<TimeSpend> GetByFilter(TaskFilter filter)
        {
            var query = CreateQuery();

            if (filter.Max != 0 && !filter.Max.Equals(int.MaxValue))
            {
                query.SetFirstResult((int)filter.Offset);
                query.SetMaxResults((int)filter.Max * 2);
            }

            if (filter.MyProjects || filter.MyMilestones)
            {
                query.InnerJoin(ParticipantTable + " ppp", Exp.EqColumns("t.project_id", "ppp.project_id") & Exp.Eq("ppp.removed", false) & Exp.EqColumns("t.tenant_id", "ppp.tenant"));
                query.Where("ppp.participant_id", CurrentUserID);
            }

            if (filter.ProjectIds.Count != 0)
            {
                query.Where(Exp.In("t.project_id", filter.ProjectIds));
            }

            if (filter.Milestone.HasValue || filter.MyMilestones)
            {
                query.InnerJoin(MilestonesTable + " pm", Exp.EqColumns("pm.tenant_id", "t.tenant_id") & Exp.EqColumns("pm.project_id", "t.project_id"));
                query.InnerJoin(TasksTable + " pt", Exp.EqColumns("pt.tenant_id", "t.tenant_id") & Exp.EqColumns("pt.id", "t.relative_task_id") & Exp.EqColumns("pt.milestone_id", "pm.id"));

                if (filter.Milestone.HasValue)
                {
                    query.Where("pm.id", filter.Milestone);
                }
                else if (filter.MyMilestones)
                {
                    query.Where(Exp.Gt("pm.id", 0));
                }
            }

            if (filter.TagId != 0)
            {
                query.InnerJoin(ProjectTagTable + " ptag", Exp.EqColumns("ptag.project_id", "t.project_id"));
                query.Where("ptag.tag_id", filter.TagId);
            }

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["TimeSpend"];
                sortColumns.Remove(filter.SortBy);

                query.OrderBy("t." + filter.SortBy, filter.SortOrder);

                foreach (var sort in sortColumns.Keys)
                {
                    query.OrderBy("t." + sort, sortColumns[sort]);
                }
            }

            if (filter.UserId != Guid.Empty)
            {
                query.Where("t.person_id", filter.UserId);
            }


            if (filter.DepartmentId != Guid.Empty)
            {
                query.InnerJoin("core_usergroup cug", Exp.Eq("cug.removed", false) & Exp.EqColumns("cug.userid", "t.person_id") & Exp.EqColumns("cug.tenant", "t.tenant_id"));
                query.Where("cug.groupid", filter.DepartmentId);
            }

            if (!filter.FromDate.Equals(DateTime.MinValue) && !filter.FromDate.Equals(DateTime.MaxValue) &&             
                !filter.ToDate.Equals(DateTime.MinValue) && !filter.ToDate.Equals(DateTime.MaxValue))
            {
                query.Where(Exp.Between("t.date", filter.FromDate, filter.ToDate));
            }

            if (filter.PaymentStatuses.Any())
            {
                query.Where(Exp.In("payment_status", filter.PaymentStatuses));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query.Where(Exp.Like("t.note", filter.SearchText, SqlLike.AnyWhere));
            }

            query.GroupBy("t.id");

            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(query).ConvertAll(r => ToTimeSpend(r));
            }
        }

        public List<TimeSpend> GetByProject(int projectId)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(CreateQuery().Where("t.project_id", projectId)).ConvertAll(r => ToTimeSpend(r));
            }
        }

        public List<TimeSpend> GetByTask(int taskId)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(CreateQuery().Where("t.relative_task_id", taskId)).ConvertAll(r => ToTimeSpend(r));
            }
        }

        public TimeSpend GetById(int id)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(CreateQuery().Where("t.id", id))
                                .ConvertAll(r => ToTimeSpend(r))
                                .SingleOrDefault();
            }
        }

        public TimeSpend Save(TimeSpend timeSpend)
        {
            using (var db = new DbManager(DatabaseId))
            {
                timeSpend.Date = TenantUtil.DateTimeToUtc(timeSpend.Date);
                timeSpend.StatusChangedOn = TenantUtil.DateTimeToUtc(timeSpend.StatusChangedOn);

                var insert = Insert(TimeTrackingTable)
                    .InColumnValue("id", timeSpend.ID)
                    .InColumnValue("note", timeSpend.Note)
                    .InColumnValue("date", timeSpend.Date)
                    .InColumnValue("hours", timeSpend.Hours)
                    .InColumnValue("relative_task_id", timeSpend.Task.ID)
                    .InColumnValue("person_id", timeSpend.Person.ToString())
                    .InColumnValue("project_id", timeSpend.Task.Project.ID)
                    .InColumnValue("create_on", timeSpend.CreateOn)
                    .InColumnValue("create_by", CurrentUserID)
                    .InColumnValue("payment_status", timeSpend.PaymentStatus)
                    .InColumnValue("status_changed", timeSpend.StatusChangedOn)
                    .Identity(1, 0, true);

                timeSpend.ID = db.ExecuteScalar<int>(insert);

                return timeSpend;
            }
        }

        public void Delete(int id)
        {
            using (var db = new DbManager(DatabaseId))
            {
                db.ExecuteNonQuery(Delete(TimeTrackingTable).Where("id", id));
            }
        }

        private SqlQuery CreateQuery()
        {
            return new SqlQuery(TimeTrackingTable + " t")
                .Select(columns.Select(c => "t." + c).ToArray())
                .Where("t.tenant_id", Tenant);
        }

        private static TimeSpend ToTimeSpend(IList<object> r)
        {
            return new TimeSpend
                       {
                           ID = Convert.ToInt32(r[0]),
                           Note = (string) r[1],
                           Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[2])),
                           Hours = Convert.ToSingle(r[3]),
                           Task = new Task {ID = Convert.ToInt32(r[4])},
                           Person = ToGuid(r[5]),
                           CreateOn = r[7] != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[7])) : default(DateTime),
                           CreateBy = r[8] != null ? ToGuid(r[8]) : ToGuid(r[5]),
                           PaymentStatus = (PaymentStatus)r[9],
                           StatusChangedOn = r[10] != null ? TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[10])) : default(DateTime)
                       };
        }
    }
}
