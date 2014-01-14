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
using System.Threading;
using ASC.Core.Tenants;
using ASC.Core.Users;

namespace ASC.Core.Caching
{
    public class CachedUserService : IUserService
    {
        private const string USERS = "users/";
        private const string GROUPS = "groups/";
        private const string REFS = "refs/";

        private readonly IUserService service;
        private readonly ICache cache;
        private TrustInterval trustInterval;
        private int getchanges = 0;


        public TimeSpan CacheExpiration { get; set; }

        public TimeSpan DbExpiration { get; set; }

        public TimeSpan PhotoExpiration { get; set; }

        public CachedUserService(IUserService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();

            CacheExpiration = TimeSpan.FromHours(1);
            DbExpiration = TimeSpan.FromMinutes(1);
            PhotoExpiration = TimeSpan.FromMinutes(10);
        }


        public IDictionary<Guid, UserInfo> GetUsers(int tenant, DateTime from)
        {
            var users = GetUsers(tenant);
            lock (users)
            {
                return (from == default(DateTime) ? users.Values : users.Values.Where(u => u.LastModified >= from)).ToDictionary(u => u.ID);
            }
        }

        public UserInfo GetUser(int tenant, Guid id)
        {
            var users = GetUsers(tenant);
            lock (users)
            {
                UserInfo u;
                users.TryGetValue(id, out u);
                return u;
            }
        }

        public UserInfo GetUser(int tenant, string login, string passwordHash)
        {
            return service.GetUser(tenant, login, passwordHash);
        }

        public UserInfo SaveUser(int tenant, UserInfo user)
        {
            user = service.SaveUser(tenant, user);
            InvalidateCache();
            return user;
        }

        public void RemoveUser(int tenant, Guid id)
        {
            service.RemoveUser(tenant, id);
            InvalidateCache();
        }

        public byte[] GetUserPhoto(int tenant, Guid id)
        {
            var photo = cache.Get(GetUserPhotoCacheKey(tenant, id));
            if (photo == null)
            {
                photo = service.GetUserPhoto(tenant, id);
                cache.Insert(GetUserPhotoCacheKey(tenant, id), photo, PhotoExpiration);
            }
            return (byte[])photo;
        }

        public void SetUserPhoto(int tenant, Guid id, byte[] photo)
        {
            cache.Remove(GetUserPhotoCacheKey(tenant, id));
            service.SetUserPhoto(tenant, id, photo);
        }

        private static string GetUserPhotoCacheKey(int tenant, Guid userId)
        {
            return string.Format("userphoto-{0}-{1}", tenant, userId);
        }

        public string GetUserPassword(int tenant, Guid id)
        {
            return service.GetUserPassword(tenant, id);
        }

        public void SetUserPassword(int tenant, Guid id, string password)
        {
            service.SetUserPassword(tenant, id, password);
        }


        public IDictionary<Guid, Group> GetGroups(int tenant, DateTime from)
        {
            var groups = GetGroups(tenant);
            lock (groups)
            {
                return (from == default(DateTime) ? groups.Values : groups.Values.Where(g => g.LastModified >= from)).ToDictionary(g => g.Id);
            }
        }

        public Group GetGroup(int tenant, Guid id)
        {
            var groups = GetGroups(tenant);
            lock (groups)
            {
                Group g;
                groups.TryGetValue(id, out g);
                return g;
            }
        }

        public Group SaveGroup(int tenant, Group group)
        {
            group = service.SaveGroup(tenant, group);
            InvalidateCache();
            return group;
        }

        public void RemoveGroup(int tenant, Guid id)
        {
            service.RemoveGroup(tenant, id);
            InvalidateCache();
        }


        public IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from)
        {
            GetChangesFromDb();

            var key = REFS + tenant.ToString();
            var refs = cache.Get(key) as IDictionary<string, UserGroupRef>;
            if (refs == null)
            {
                refs = service.GetUserGroupRefs(tenant, default(DateTime));
                cache.Insert(key, new UserGroupRefStore(refs), CacheExpiration);
            }
            lock (refs)
            {
                return from == default(DateTime) ? refs : refs.Values.Where(r => r.LastModified >= from).ToDictionary(r => r.CreateKey());
            }
        }

        public UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r)
        {
            r = service.SaveUserGroupRef(tenant, r);

            var key = REFS + tenant.ToString();
            var refs = cache.Get(key) as IDictionary<string, UserGroupRef>;
            if (refs != null)
            {
                lock (refs)
                {
                    refs[r.CreateKey()] = r;
                }
            }
            else
            {
                InvalidateCache();
            }
            return r;
        }

        public void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            service.RemoveUserGroupRef(tenant, userId, groupId, refType);
            InvalidateCache();
        }


        private IDictionary<Guid, UserInfo> GetUsers(int tenant)
        {
            GetChangesFromDb();

            var key = USERS + tenant.ToString();
            var users = cache.Get(key) as IDictionary<Guid, UserInfo>;
            if (users == null)
            {
                users = service.GetUsers(tenant, default(DateTime));
                cache.Insert(key, users, CacheExpiration);
            }
            return users;
        }

        private IDictionary<Guid, Group> GetGroups(int tenant)
        {
            GetChangesFromDb();

            var key = GROUPS + tenant.ToString();
            var groups = cache.Get(key) as IDictionary<Guid, Group>;
            if (groups == null)
            {
                groups = service.GetGroups(tenant, default(DateTime));
                cache.Insert(key, groups, CacheExpiration);
            }
            return groups;
        }

        private void GetChangesFromDb()
        {
            if (Interlocked.CompareExchange(ref getchanges, 1, 0) == 0)
            {
                try
                {
                    if (trustInterval == null)
                    {
                        trustInterval = new TrustInterval();
                        trustInterval.Start(DbExpiration);
                    }
                    if (!trustInterval.Expired) return;

                    var starttime = trustInterval.StartTime;
                    trustInterval.Start(DbExpiration);

                    //get and merge changes in cached tenants
                    foreach (var tenantGroup in service.GetUsers(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(u => u.Tenant))
                    {
                        var users = cache.Get(USERS + tenantGroup.Key) as IDictionary<Guid, UserInfo>;
                        if (users != null)
                        {
                            lock (users)
                            {
                                foreach (var u in tenantGroup)
                                {
                                    users[u.ID] = u;
                                }
                            }
                        }
                    }

                    foreach (var tenantGroup in service.GetGroups(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(g => g.Tenant))
                    {
                        var groups = cache.Get(GROUPS + tenantGroup.Key) as IDictionary<Guid, Group>;
                        if (groups != null)
                        {
                            lock (groups)
                            {
                                foreach (var g in tenantGroup)
                                {
                                    groups[g.Id] = g;
                                }
                            }
                        }
                    }

                    foreach (var tenantGroup in service.GetUserGroupRefs(Tenant.DEFAULT_TENANT, starttime).Values.GroupBy(r => r.Tenant))
                    {
                        var refs = cache.Get(REFS + tenantGroup.Key) as IDictionary<string, UserGroupRef>;
                        if (refs != null)
                        {
                            lock (refs)
                            {
                                foreach (var r in tenantGroup)
                                {
                                    refs[r.CreateKey()] = r;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    getchanges = 0;
                }
            }
        }

        private void InvalidateCache()
        {
            trustInterval.Expire();
        }
    }
}
