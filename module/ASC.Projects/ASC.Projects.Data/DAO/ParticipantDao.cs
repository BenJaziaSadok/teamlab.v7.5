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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Projects.Core.DataInterfaces;
using System.Collections.Generic;

namespace ASC.Projects.Data.DAO
{
    class ParticipantDao : BaseDao, IParticipantDao
    {
        public ParticipantDao(string dbId, int tenant) : base(dbId, tenant) { }


        public int[] GetFollowingProjects(Guid participant)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(new SqlQuery(FollowingProjectTable).Select("project_id").Where("participant_id", participant.ToString()))
                    .ConvertAll(r => Convert.ToInt32(r[0]))
                    .ToArray();
            }
        }

        public int[] GetMyProjects(Guid participant)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(new SqlQuery(ParticipantTable).Select("project_id").Where("participant_id", participant.ToString()))
                    .ConvertAll(r => Convert.ToInt32(r[0]))
                    .ToArray();
            }
        }
        public List<int> GetInterestedProjects(Guid participant)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var unionQ = new SqlQuery(FollowingProjectTable).Select("project_id")
                                                                .Where("participant_id", participant.ToString())
                                                                .Union(
                                                                new SqlQuery(ParticipantTable)
                                                                .Select("project_id")
                                                                .Where("participant_id",participant.ToString()));

                return db.ExecuteList(unionQ).ConvertAll(r => Convert.ToInt32(r[0]));
            }
        }

        public void AddToFollowingProjects(int project, Guid participant)
        {
            using (var db = new DbManager(DatabaseId))
            {
                db.ExecuteNonQuery(
                    new SqlInsert(FollowingProjectTable, true)
                        .InColumnValue("project_id", project)
                        .InColumnValue("participant_id", participant.ToString()));

                var projDao = new ProjectDao(db.DatabaseId, Tenant);
                projDao.UpdateLastModified(project);
            }
        }

        public void RemoveFromFollowingProjects(int project, Guid participant)
        {
            using (var db = new DbManager(DatabaseId))
            {
                db.ExecuteNonQuery(
                    new SqlDelete(FollowingProjectTable)
                        .Where("project_id", project)
                        .Where("participant_id", participant.ToString()));

                var projDao = new ProjectDao(db.DatabaseId, Tenant);
                projDao.UpdateLastModified(project);
            }
        }
    }
}
