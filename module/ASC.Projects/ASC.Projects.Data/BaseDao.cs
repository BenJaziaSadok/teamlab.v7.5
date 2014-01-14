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
using ASC.Common.Data.Sql;
using ASC.Core;

namespace ASC.Projects.Data
{
    abstract class BaseDao
    {
        protected static readonly string CommentsTable = "projects_comments";
        protected static readonly string FollowingProjectTable = "projects_following_project_participant";
        protected static readonly string MessagesTable = "projects_messages";
        protected static readonly string MilestonesTable = "projects_milestones";
        protected static readonly string ProjectsTable = "projects_projects";
        protected static readonly string ParticipantTable = "projects_project_participant";
        protected static readonly string ProjectTagTable = "projects_project_tag";
        protected static readonly string ReportTable = "projects_report_template";
        protected static readonly string SubtasksTable = "projects_subtasks";
        protected static readonly string TagsTable = "projects_tags";
        protected static readonly string TasksTable = "projects_tasks";
        protected static readonly string TasksResponsibleTable = "projects_tasks_responsible";
        protected static readonly string TemplatesTable = "projects_templates";
        protected static readonly string TimeTrackingTable = "projects_time_tracking";
        protected static readonly string TasksLinksTable = "projects_tasks_links";
        protected static readonly string TasksOrderTable = "projects_tasks_order";

        protected int Tenant { get; private set; }

        protected Guid CurrentUserID { get; private set; }

        protected string DatabaseId { get; private set; }

        protected BaseDao(string dbId, int tenant)
        {
            Tenant = tenant;
            CurrentUserID = SecurityContext.CurrentAccount.ID;
            DatabaseId = dbId;
        }


        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where("tenant_id", Tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumnValue("tenant_id", Tenant);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where("tenant_id", Tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where("tenant_id", Tenant);
        }

        protected static Guid ToGuid(object guid)
        {
            try
            {
                var str = guid as string;
                return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }

        }
    }
}
