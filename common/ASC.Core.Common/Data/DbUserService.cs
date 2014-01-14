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
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

namespace ASC.Core.Data
{
    public class DbUserService : DbBaseService, IUserService
    {
        public DbUserService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }

        public IDictionary<Guid, UserInfo> GetUsers(int tenant, DateTime from)
        {
            var q = GetUserQuery(tenant, from);
            return ExecList(q).ConvertAll(r => ToUser(r)).ToDictionary(u => u.ID);
        }

        public List<UserInfo> GetUsers(EmployeeStatus status, int tenant)
        {
            var q = GetUserQuery(tenant, default(DateTime));
            return ExecList(q).ConvertAll(r => ToUser(r)).Where(r => r.Status == status).ToList();
        }

        public UserInfo GetUser(int tenant, Guid id)
        {
            var q = GetUserQuery(tenant, default(DateTime)).Where("id", id);
            return ExecList(q).ConvertAll(r => ToUser(r)).SingleOrDefault();
        }

        public UserInfo GetUser(int tenant, string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            var q = GetUserQuery()
                .InnerJoin("core_usersecurity s", Exp.EqColumns("u.id", "s.userid"))
                .Where(login.Contains('@') ? "u.email" : "u.id", login)
                .Where("s.pwdhash", passwordHash)
                .Where("removed", false);
            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("u.tenant", tenant);
            }

            return ExecList(q).ConvertAll(r => ToUser(r)).FirstOrDefault();
        }

        public UserInfo SaveUser(int tenant, UserInfo user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.UserName)) throw new ArgumentOutOfRangeException("Empty username.");

            if (user.ID == default(Guid)) user.ID = Guid.NewGuid();
            user.LastModified = DateTime.UtcNow;
            user.Tenant = tenant;

            ExecAction(db =>
            {
                user.UserName = user.UserName.Trim();
                var q = Query("core_user", tenant)
                    .SelectCount()
                    .Where("username", user.UserName)
                    .Where(!Exp.Eq("id", user.ID.ToString()))
                    .Where("removed", false);
                var count = db.ExecScalar<int>(q);
                if (count != 0)
                {
                    throw new ArgumentOutOfRangeException("Duplicate username.");
                }

                user.Email = user.Email.Trim();
                q = Query("core_user", tenant)
                    .SelectCount()
                    .Where("email", user.Email)
                    .Where(!Exp.Eq("id", user.ID.ToString()))
                    .Where("removed", false);
                count = db.ExecScalar<int>(q);
                if (count != 0)
                {
                    throw new ArgumentOutOfRangeException("Duplicate email.");
                }

                var i = Insert("core_user", tenant)
                    .InColumnValue("id", user.ID.ToString())
                    .InColumnValue("username", user.UserName)
                    .InColumnValue("firstname", user.FirstName)
                    .InColumnValue("lastname", user.LastName)
                    .InColumnValue("sex", user.Sex)
                    .InColumnValue("bithdate", user.BirthDate)
                    .InColumnValue("status", user.Status)
                    .InColumnValue("title", user.Title)
                    .InColumnValue("workfromdate", user.WorkFromDate)
                    .InColumnValue("terminateddate", user.TerminatedDate)
                    .InColumnValue("contacts", user.ContactsToString())
                    .InColumnValue("email", string.IsNullOrEmpty(user.Email) ? user.Email : user.Email.Trim())
                    .InColumnValue("location", user.Location)
                    .InColumnValue("notes", user.Notes)
                    .InColumnValue("removed", user.Removed)
                    .InColumnValue("last_modified", user.LastModified)
                    .InColumnValue("activation_status", user.ActivationStatus)
                    .InColumnValue("culture", user.CultureName)
                    .InColumnValue("phone", user.MobilePhone)
                    .InColumnValue("phone_activation", user.MobilePhoneActivationStatus);

                db.ExecNonQuery(i);
            });

            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            RemoveUser(tenant, id, false);
        }

        public void RemoveUser(int tenant, Guid id, bool immediate)
        {
            var sid = id.ToString();
            var batch = new List<ISqlInstruction>
                            {
                                Delete("core_acl", tenant).Where("subject", sid),
                                Delete("core_subscription", tenant).Where("recipient", sid),
                                Delete("core_subscriptionmethod", tenant).Where("recipient", sid),
                                Delete("core_userphoto", tenant).Where("userid", sid)
                            };

            if (immediate)
            {
                batch.Add(Delete("core_usergroup", tenant).Where("userid", sid));
                batch.Add(Delete("core_user", tenant).Where("id", sid));
                batch.Add(Delete("core_usersecurity", tenant).Where("userid", sid));
            }
            else
            {
                batch.Add(Update("core_usergroup", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where("userid", sid));
                batch.Add(Update("core_user", tenant)
                    .Set("removed", true)
                    .Set("status", (int)EmployeeStatus.Terminated)
                    .Set("terminateddate", DateTime.UtcNow)
                    .Set("last_modified", DateTime.UtcNow)
                    .Where("id", sid));
            }
            ExecBatch(batch);
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            var sql = photo != null && photo.Length != 0 ?
                Insert("core_userphoto", tenant).InColumns("userid", "photo").Values(id.ToString(), photo) :
                (ISqlInstruction)Delete("core_userphoto", tenant).Where("userid", id.ToString());

            ExecNonQuery(sql);
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = ExecScalar<byte[]>(Query("core_userphoto", tenant).Select("photo").Where("userid", id.ToString()));
            return photo ?? new byte[0];
        }

        public string GetUserPassword(int tenant, Guid id)
        {
            var q = Query("core_usersecurity", tenant).Select("pwdhashsha512").Where("userid", id.ToString());
            var h2 = ExecScalar<string>(q);
            return !string.IsNullOrEmpty(h2) ? Crypto.GetV(h2, 1, false) : null;
        }

        public void SetUserPassword(int tenant, Guid id, string password)
        {
            var h1 = !string.IsNullOrEmpty(password) ? Hasher.Base64Hash(password, HashAlg.SHA256) : null;
            var h2 = !string.IsNullOrEmpty(password) ? Crypto.GetV(password, 1, true) : null;
            var i = Insert("core_usersecurity", tenant)
                .InColumnValue("userid", id.ToString())
                .InColumnValue("pwdhash", h1)
                .InColumnValue("pwdhashsha512", h2);
            ExecNonQuery(i);
        }

        public IDictionary<Guid, Group> GetGroups(int tenant, DateTime from)
        {
            var q = GetGroupQuery(tenant, from);
            return ExecList(q)
                .ConvertAll(r => ToGroup(r))
                .ToDictionary(g => g.Id);
        }

        public Group GetGroup(int tenant, Guid id)
        {
            var q = GetGroupQuery(tenant, default(DateTime)).Where("id", id);
            return ExecList(q).ConvertAll(r => ToGroup(r)).SingleOrDefault();
        }

        public Group SaveGroup(int tenant, Group group)
        {
            if (group == null) throw new ArgumentNullException("user");

            if (group.Id == default(Guid)) group.Id = Guid.NewGuid();
            group.LastModified = DateTime.UtcNow;
            group.Tenant = tenant;

            var i = Insert("core_group", tenant)
                .InColumnValue("id", group.Id.ToString())
                .InColumnValue("name", group.Name)
                .InColumnValue("parentid", group.ParentId.ToString())
                .InColumnValue("categoryid", group.CategoryId.ToString())
                .InColumnValue("removed", group.Removed)
                .InColumnValue("last_modified", group.LastModified);
            ExecNonQuery(i);
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            RemoveGroup(tenant, id, false);
        }

        public void RemoveGroup(int tenant, Guid id, bool immediate)
        {
            var batch = new List<ISqlInstruction>();

            var ids = CollectGroupChilds(tenant, id.ToString());

            batch.Add(Delete("core_acl", tenant).Where(Exp.In("subject", ids)));
            batch.Add(Delete("core_subscription", tenant).Where(Exp.In("recipient", ids)));
            batch.Add(Delete("core_subscriptionmethod", tenant).Where(Exp.In("recipient", ids)));
            if (immediate)
            {
                batch.Add(Delete("core_usergroup", tenant).Where(Exp.In("groupid", ids)));
                batch.Add(Delete("core_group", tenant).Where(Exp.In("id", ids)));
            }
            else
            {
                batch.Add(Update("core_usergroup", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where(Exp.In("groupid", ids)));
                batch.Add(Update("core_group", tenant).Set("removed", true).Set("last_modified", DateTime.UtcNow).Where(Exp.In("id", ids)));
            }

            ExecBatch(batch);
        }

        public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            var q = GetUserGroupRefQuery(tenant, from);
            return ExecList(q).ConvertAll(r => ToUserGroupRef(r)).ToDictionary(r => r.CreateKey());
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            if (r == null) throw new ArgumentNullException("userGroupRef");

            r.LastModified = DateTime.UtcNow;
            r.Tenant = tenant;

            var i = Insert("core_usergroup", tenant)
                .InColumnValue("userid", r.UserId.ToString())
                .InColumnValue("groupid", r.GroupId.ToString())
                .InColumnValue("ref_type", (int)r.RefType)
                .InColumnValue("removed", r.Removed)
                .InColumnValue("last_modified", r.LastModified);
            var u = Update("core_user", tenant).Set("last_modified", r.LastModified).Where("id", r.UserId.ToString());

            ExecBatch(i, u);

            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            RemoveUserGroupRef(tenant, userId, groupId, refType, false);
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType, bool immediate)
        {
            var where = Exp.Eq("userid", userId.ToString()) & Exp.Eq("groupid", groupId.ToString()) & Exp.Eq("ref_type", (int)refType);
            var i = immediate ?
                Delete("core_usergroup", tenant).Where(where) :
                (ISqlInstruction)Update("core_usergroup", tenant).Where(where).Set("removed", true).Set("last_modified", DateTime.UtcNow);
            var u = Update("core_user", tenant).Set("last_modified", DateTime.UtcNow).Where("id", userId.ToString());
            ExecBatch(i, u);
        }

        private static SqlQuery GetUserQuery()
        {
            // (select group_concat(g.name separator ', ') from core_group g, core_usergroup r where g.tenant = u.tenant and r.tenant = u.tenant and g.id = r.groupid and r.ref_type = 0 and r.userid = u.id and g.removed = 0 and r.removed = 0)
            var department = new SqlQuery()
                .From("core_group g", "core_usergroup r")
                .Select("group_concat(g.name separator ', ')")
                .Where(Exp.EqColumns("g.tenant", "u.tenant") & Exp.EqColumns("r.tenant", "u.tenant"))
                .Where(Exp.EqColumns("g.id", "r.groupid") & Exp.EqColumns("r.userid", "u.id"))
                .Where("ref_type", (int)UserGroupRefType.Contains)
                .Where("g.removed", false)
                .Where("r.removed", false);

            return new SqlQuery("core_user u")
                .Select("u.id", "u.username", "u.firstname", "u.lastname", "u.sex", "u.bithdate", "u.status", "u.title")
                .Select(department)
                .Select("u.workfromdate", "u.terminateddate", "u.contacts", "u.email", "u.location", "u.notes", "u.removed")
                .Select("u.last_modified", "u.tenant", "u.activation_status", "u.culture", "u.phone", "u.phone_activation");
        }

        private static SqlQuery GetUserQuery(int tenant, DateTime from)
        {
            var q = GetUserQuery();
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));
            return q;
        }

        private static UserInfo ToUser(object[] r)
        {
            var u = new UserInfo
            {
                ID = new Guid((string)r[0]),
                UserName = (string)r[1],
                FirstName = (string)r[2],
                LastName = (string)r[3],
                Sex = r[4] != null ? Convert.ToBoolean(r[4]) : (bool?)null,
                BirthDate = (DateTime?)r[5],
                Status = (EmployeeStatus)Convert.ToInt32(r[6]),
                Title = (string)r[7],
                Department = (string)r[8],
                WorkFromDate = (DateTime?)r[9],
                TerminatedDate = (DateTime?)r[10],
                Email = (string)r[12],
                Location = (string)r[13],
                Notes = (string)r[14],
                Removed = Convert.ToBoolean(r[15]),
                LastModified = Convert.ToDateTime(r[16]),
                Tenant = Convert.ToInt32(r[17]),
                ActivationStatus = (EmployeeActivationStatus)Convert.ToInt32(r[18]),
                CultureName = (string)r[19],
                MobilePhone = (string)r[20],
                MobilePhoneActivationStatus = (MobilePhoneActivationStatus)Convert.ToInt32(r[21])
            };
            u.ContactsFromString((string)r[11]);
            return u;
        }

        private static SqlQuery GetGroupQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_group").Select("id", "name", "parentid", "categoryid", "removed", "last_modified", "tenant");
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));
            return q;
        }

        private Group ToGroup(object[] r)
        {
            return new Group
            {
                Id = new Guid((string)r[0]),
                Name = (string)r[1],
                ParentId = r[2] != null ? new Guid((string)r[2]) : Guid.Empty,
                CategoryId = r[3] != null ? new Guid((string)r[3]) : Guid.Empty,
                Removed = Convert.ToBoolean(r[4]),
                LastModified = Convert.ToDateTime(r[5]),
                Tenant = Convert.ToInt32(r[6]),
            };
        }

        private List<string> CollectGroupChilds(int tenant, string id)
        {
            var result = new List<string>();
            var childs = ExecList(Query("core_group", tenant).Select("id").Where("parentid", id)).ConvertAll(r => (string)r[0]);
            foreach (var child in childs)
            {
                result.Add(child);
                result.AddRange(CollectGroupChilds(tenant, child));
            }
            result.Add(id);
            return result.Distinct().ToList();
        }

        private static SqlQuery GetUserGroupRefQuery(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_usergroup").Select("userid", "groupid", "ref_type", "removed", "last_modified", "tenant");
            if (tenant != Tenant.DEFAULT_TENANT) q.Where("tenant", tenant);
            if (from != default(DateTime)) q.Where(Exp.Ge("last_modified", from));
            return q;
        }

        private static UserGroupRef ToUserGroupRef(object[] r)
        {
            return new UserGroupRef(new Guid((string)r[0]), new Guid((string)r[1]), (UserGroupRefType)Convert.ToInt32(r[2]))
            {
                Removed = Convert.ToBoolean(r[3]),
                LastModified = Convert.ToDateTime(r[4]),
                Tenant = Convert.ToInt32(r[5]),
            };
        }
    }
}
