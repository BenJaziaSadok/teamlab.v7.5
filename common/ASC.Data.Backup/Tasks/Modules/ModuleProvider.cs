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

using System.Collections.Generic;
using System.Linq;
using ASC.Data.Backup.Exceptions;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal static class ModuleProvider
    {
        private static readonly List<IModuleSpecifics> AllModules;

        static ModuleProvider()
        {
            AllModules = new IModuleSpecifics[]
                {
                    new TenantsModuleSpecifics(),
                    new ProjectsModuleSpecifics(),
                    new CommunityModuleSpecifics(),
                    new WebStudioModuleSpecifics(),
                    new CrmModuleSpecifics(),
                    new MailModuleSpecifics(),
                    new CrmModuleSpecifics2(),
                    new FilesModuleSpecifics(),
                    new CalendarModuleSpecifics(),
                    new CoreModuleSpecifics()
                }
                .OrderBy(m => m, new ModuleComparer())
                .ToList();
        }

        public static IModuleSpecifics GetByStorageModule(string storageModuleName, string storageDomainName = null)
        {
            switch (storageModuleName)
            {
                case "files":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Files);

                case "projects":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Projects);

                case "crm":
                    return AllModules.FirstOrDefault(m => m.ModuleName == (storageDomainName == "mail_messages" ? ModuleName.Crm2 : ModuleName.Crm));

                case "forum":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Community);
                
                case "mailaggregator":
                    return AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Mail);

                default:
                    return null;
            }
        }

        public static IEnumerable<IModuleSpecifics> GetAll()
        {
            return AllModules;
        }

        private class ModuleComparer : IComparer<IModuleSpecifics>
        {
            public int Compare(IModuleSpecifics x, IModuleSpecifics y)
            {
                var typeOfX = x.GetType();
                var typeOfY = y.GetType();
                bool xParentToY = y.TableRelations.Any(r => r.ParentModule == typeOfX);
                bool yParentToX = x.TableRelations.Any(r => r.ParentModule == typeOfY);
                if (xParentToY && yParentToX)
                    throw ThrowHelper.CantOrderModules(new[] {typeOfX, typeOfY});
                if (xParentToY)
                    return -1;
                if (yParentToX)
                    return 1;
                return 0;
            }
        }
    }
}
