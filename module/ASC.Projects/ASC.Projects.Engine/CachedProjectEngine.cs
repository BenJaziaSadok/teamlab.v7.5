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
using ASC.Core;
using ASC.Core.Caching;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class CachedProjectEngine : ProjectEngine
    {
        private static readonly ICache Cache = new AspCache();
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);


        public CachedProjectEngine(IDaoFactory daoFactory, EngineFactory factory)
            : base(daoFactory, factory)
        {
        }

        public override int Count()
        {
            var key = GetCountKey();
            var value = Cache.Get(key);
            if (value != null)
            {
                return (int)value;
            }
            var count = base.Count();
            Cache.Insert(key, count, DateTime.UtcNow.Add(CacheExpiration));
            return count;
        }

        public override Project SaveOrUpdate(Project project, bool notifyManager, bool isImport)
        {
            var p = base.SaveOrUpdate(project, notifyManager, isImport);
            Cache.Remove(GetCountKey());
            return p;
        }

        public override void Delete(int projectId)
        {
            base.Delete(projectId);
            Cache.Remove(GetCountKey());
        }

        private static string GetCountKey()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId + "/projects/count";
        }
    }
}
