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
using ASC.Data.Backup.Tasks.Data;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal class CalendarModuleSpecifics : ModuleSpecificsBase
    {
        private readonly TableInfo[] _tables = new[]
            {
                new TableInfo("calendar_calendars") {AutoIncrementColumn = "id", TenantColumn = "tenant", UserIDColumns = new[] {"owner_id"}},
                new TableInfo("calendar_calendar_item"),
                new TableInfo("calendar_calendar_user") {UserIDColumns = new[] {"user_id"}},
                new TableInfo("calendar_events") {AutoIncrementColumn = "id", TenantColumn = "tenant", UserIDColumns = new[] {"owner_id"}},
                new TableInfo("calendar_event_item"),
                new TableInfo("calendar_event_user") {UserIDColumns = new[] {"user_id"}},
                new TableInfo("calendar_notifications") {TenantColumn = "tenant", UserIDColumns = new[] {"user_id"}}
            };

        private readonly RelationInfo[] _tableRelations = new[]
            {
                new RelationInfo("calendar_calendars", "id", "calendar_calendar_item", "calendar_id"),
                new RelationInfo("calendar_calendars", "id", "calendar_calendar_user", "calendar_id"),
                new RelationInfo("calendar_calendars", "id", "calendar_events", "calendar_id"),
                new RelationInfo("calendar_events", "id", "calendar_event_item", "event_id"),
                new RelationInfo("calendar_events", "id", "calendar_event_user", "event_id"),
                new RelationInfo("calendar_events", "id", "calendar_notifications", "event_id"),
                new RelationInfo("core_user", "id", "calendar_calendar_item", "item_id", typeof(TenantsModuleSpecifics), 
                    x => Convert.ToInt32(x["is_group"]) == 0),
                new RelationInfo("core_group", "id", "calendar_calendar_item", "item_id", typeof(TenantsModuleSpecifics), 
                    x => Convert.ToInt32(x["is_group"]) == 1 && !Helpers.IsEmptyOrSystemGroup(Convert.ToString(x["item_id"]))),
                new RelationInfo("core_user", "id", "calendar_event_item", "item_id", typeof(TenantsModuleSpecifics), 
                    x => Convert.ToInt32(x["is_group"]) == 0),
                new RelationInfo("core_group", "id", "calendar_event_item", "item_id", typeof(TenantsModuleSpecifics), 
                    x => Convert.ToInt32(x["is_group"]) == 1 && !Helpers.IsEmptyOrSystemGroup(Convert.ToString(x["item_id"])))
            };

        public override ModuleName ModuleName
        {
            get { return ModuleName.Calendar; }
        }

        public override IEnumerable<TableInfo> Tables
        {
            get { return _tables; }
        }

        public override IEnumerable<RelationInfo> TableRelations
        {
            get { return _tableRelations; }
        }

        protected override string GetSelectCommandConditionText(int tenantId, TableInfo table)
        {
            if (table.Name == "calendar_calendar_item" || table.Name == "calendar_calendar_user")
                return "inner join calendar_calendars as t1 on t1.id = t.calendar_id where t1.tenant = " + tenantId;

            if (table.Name == "calendar_event_item" || table.Name == "calendar_event_user")
                return "inner join calendar_events as t1 on t1.id = t.event_id where t1.tenant = " + tenantId;

            return base.GetSelectCommandConditionText(tenantId, table);
        }
    }
}
