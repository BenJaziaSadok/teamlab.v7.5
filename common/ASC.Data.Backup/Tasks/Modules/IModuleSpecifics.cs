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
using System.Data;
using ASC.Data.Backup.Tasks.Data;

namespace ASC.Data.Backup.Tasks.Modules
{
    public enum ModuleName
    {
        Calendar,
        Community,
        Core,
        Crm,
        Crm2,
        Files,
        Mail,
        Projects,
        Tenants,
        WebStudio
    }

    internal interface IModuleSpecifics
    {
        string ConnectionStringName { get; }
        ModuleName ModuleName { get; }

        IEnumerable<TableInfo> Tables { get; }
        IEnumerable<RelationInfo> TableRelations { get; }

        IEnumerable<TableInfo> GetTablesOrdered(); 
            
        IDbCommand CreateSelectCommand(IDbConnection connection, int tenantId, TableInfo table);
        IDbCommand CreateDeleteCommand(IDbConnection connection, int tenantId, TableInfo table);
        IDbCommand CreateInsertCommand(IDbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row);

        bool TryAdjustFilePath(ColumnMapper columnMapper, ref string filePath);
    }
}
