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

#region Import

using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.FederatedLogin.Profile;
using System.Linq;
using System.Linq.Expressions;
using System.Configuration;

#endregion


namespace ASC.FederatedLogin
{
    public class AccountLinker
    {
        private readonly string dbid;
        private const string LinkTable = "account_links";


        public AccountLinker(string dbid)
        {
            this.dbid = dbid;
        }

        public IEnumerable<string> GetLinkedObjects(string id, string provider)
        {
            return GetLinkedObjects(new LoginProfile() { Id = id, Provider = provider });
        }

        public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
        {
            //Retrieve by uinque id
            return GetLinkedObjectsByHashId(profile.HashId);
        }

        public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
        {
            //Retrieve by uinque id
            using (var db = new DbManager(dbid))
            {
                var query = new SqlQuery(LinkTable)
                    .Select("id").Where("uid", hashid).Where(!Exp.Eq("provider", string.Empty));
                return db.ExecuteList(query).ConvertAll(x => (string)x[0]);
            }
        }

        public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
        {
            //Retrieve by uinque id
            using (var db = new DbManager(dbid))
            {
                var query = new SqlQuery(LinkTable)
                    .Select("profile").Where("id", obj);
                return db.ExecuteList(query).ConvertAll(x => LoginProfile.CreateFromSerializedString((string)x[0]));
            }
        }

        public void AddLink(string obj, LoginProfile profile)
        {
            using (var db = new DbManager(dbid))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlInsert(LinkTable, true)
                            .InColumnValue("id", obj)
                            .InColumnValue("uid", profile.HashId)
                            .InColumnValue("provider", profile.Provider)
                            .InColumnValue("profile", profile.ToSerializedString())
                            .InColumnValue("linked", DateTime.UtcNow)
                        );
                    tx.Commit();
                }
            }
        }

        public void AddLink(string obj, string id, string provider)
        {
            AddLink(obj, new LoginProfile() { Id = id, Provider = provider });
        }

        public void RemoveLink(string obj, string id, string provider)
        {
            RemoveLink(obj, new LoginProfile() { Id = id, Provider = provider });
        }

        public void RemoveLink(string obj, LoginProfile profile)
        {
            using (var db = new DbManager(dbid))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                            .Where("uid", profile.HashId)
                        );
                    tx.Commit();
                }
            }
        }

        public void Unlink(string obj)
        {
            using (var db = new DbManager(dbid))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                        );
                    tx.Commit();
                }
            }
        }

        public void RemoveProvider(string obj, string provider)
        {
            using (var db = new DbManager(dbid))
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteScalar<int>(
                        new SqlDelete(LinkTable)
                            .Where("id", obj)
                            .Where("provider", provider)
                        );
                    tx.Commit();
                }
            }
        }
    }
}