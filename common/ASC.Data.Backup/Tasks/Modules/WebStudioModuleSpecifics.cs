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
using ASC.Data.Backup.Tasks.Data;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal class WebStudioModuleSpecifics : ModuleSpecificsBase
    {
        private readonly TableInfo[] _tables = new[]
            {
                new TableInfo("webstudio_fckuploads") {TenantColumn = "TenantID", InsertMethod = InsertMethod.None},
                new TableInfo("webstudio_settings") {TenantColumn = "TenantID", UserIDColumns = new[] {"UserID"}},
                new TableInfo("webstudio_uservisit") {TenantColumn = "tenantid", InsertMethod = InsertMethod.None},
            };

        public override ModuleName ModuleName
        {
            get { return ModuleName.WebStudio; }
        }

        public override IEnumerable<TableInfo> Tables
        {
            get { return _tables; }
        }

        public override IEnumerable<RelationInfo> TableRelations
        {
            get { return new List<RelationInfo>(); }
        }

    }
}
