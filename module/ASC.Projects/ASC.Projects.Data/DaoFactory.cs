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

#region Usings

using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Data.DAO;

#endregion

namespace ASC.Projects.Data
{
    public class DaoFactory : IDaoFactory
    {
        private readonly string dbId;
        private readonly int tenant;


        public DaoFactory(string dbId, int tenant)
        {
            this.dbId = dbId;
            this.tenant = tenant;
        }


        public IProjectDao GetProjectDao()
        {
            return new CachedProjectDao(dbId, tenant);
        }

        public IParticipantDao GetParticipantDao()
        {
            return new ParticipantDao(dbId, tenant);
        }

        public IMilestoneDao GetMilestoneDao()
        {
            return new CachedMilestoneDao(dbId, tenant);
        }

        public ITaskDao GetTaskDao()
        {
            return new CachedTaskDao(dbId, tenant);
        }

        public ISubtaskDao GetSubtaskDao()
        {
            return new CachedSubtaskDao(dbId, tenant);
        }

        public IMessageDao GetMessageDao()
        {
            return new CachedMessageDao(dbId, tenant);
        }

        public ICommentDao GetCommentDao()
        {
            return new CommentDao(dbId, tenant);
        }

        public ITemplateDao GetTemplateDao()
        {
            return new TemplateDao(dbId, tenant);
        }

        public ITimeSpendDao GetTimeSpendDao()
        {
            return new TimeSpendDao(dbId, tenant);
        }

        public IReportDao GetReportDao()
        {
            return new ReportDao(dbId, tenant);
        }

        public ISearchDao GetSearchDao()
        {
            return new SearchDao(dbId, tenant);
        }

        public ITagDao GetTagDao()
        {
            return new TagDao(dbId, tenant);
        }
    }
}
