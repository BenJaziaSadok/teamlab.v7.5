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

namespace ASC.Data.Backup.Tasks
{
    public class ColumnMapper
    {
        private readonly Dictionary<string, object> _mappings = new Dictionary<string, object>(); 

        public int GetTenantMapping()
        {
            var mapping = GetMapping("tenants_tenants", "id");
            return mapping != null ? Convert.ToInt32(mapping) : -1;
        }

        public string GetUserMapping(string oldValue)
        {
            var mapping = GetMapping("core_user", "id", oldValue);
            return mapping != null ? Convert.ToString(mapping) : null;
        }

        public void SetMapping(string tableName, string columnName, object newValue)
        {
            var mapping = new MappingWithCondition {NewValue = newValue, Condition = null};
            _mappings[GetMappingKey(tableName, columnName)] = mapping;
        }

        public void SetMapping(string tableName, string columnName, object newValue, Func<object, bool> mappingCondition)
        {
            var mapping = new MappingWithCondition {NewValue = newValue, Condition = mappingCondition};
            _mappings[GetMappingKey(tableName, columnName)] = mapping;
        }

        public object GetMapping(string tableName, string columnName)
        {
            string mappingKey = GetMappingKey(tableName, columnName);
            return _mappings.ContainsKey(mappingKey) ? ((MappingWithCondition)_mappings[mappingKey]).NewValue : null;
        }

        public void SetMapping(string tableName, string columnName, object oldValue, object newValue)
        {
            if (tableName == "tenants_tenants")
            {
                SetMapping(tableName, columnName, newValue);
            }
            _mappings[GetMappingKey(tableName, columnName, oldValue)] = newValue;
        }

        public object GetMapping(string tableName, string columnName, object oldValue)
        {
            string mappingKey = GetMappingKey(tableName, columnName, oldValue);
            if (_mappings.ContainsKey(mappingKey))
            {
                return _mappings[mappingKey];
            }
            mappingKey = GetMappingKey(tableName, columnName);
            if (_mappings.ContainsKey(mappingKey))
            {
                var mapping = (MappingWithCondition)_mappings[mappingKey];
                if (mapping.Condition == null || mapping.Condition(oldValue))
                {
                    return mapping.NewValue;
                }
            }
            return null;
        }

        private static string GetMappingKey(string tableName, string columnName)
        {
            return (tableName + "/" + columnName + "/").ToLower();
        }

        private static string GetMappingKey(string tableName, string columnName, object oldValue)
        {
            return GetMappingKey(tableName, columnName) + "/v=" + oldValue;
        }

        private class MappingWithCondition
        {
            public object NewValue { get; set; }
            public Func<object, bool> Condition { get; set; }
        }

        public static ColumnMapper ForRestoreDemoPortal(int tenantID)
        {
            var columnMapper = new ColumnMapper();

            columnMapper.SetMapping("tenants_tenants", "id", tenantID);

            //set up for projects:
            columnMapper.SetMapping("projects_comments", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_messages", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_messages", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_milestones", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_milestones", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_milestones", "deadline", DateTime.UtcNow.AddDays(7), val => !IsDefaultDateTime(val));
            columnMapper.SetMapping("projects_projects", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_projects", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_project_participant", "created", DateTime.UtcNow);
            columnMapper.SetMapping("projects_project_participant", "updated", DateTime.UtcNow);
            columnMapper.SetMapping("projects_report_template", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_subtasks", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_subtasks", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_tags", "create_on", DateTime.UtcNow, val => !IsDefaultDateTime(val));
            columnMapper.SetMapping("projects_tags", "last_modified_on", DateTime.UtcNow, val => !IsDefaultDateTime(val));
            columnMapper.SetMapping("projects_tasks", "deadline", DateTime.UtcNow.AddDays(7), val => !IsDefaultDateTime(val));
            columnMapper.SetMapping("projects_tasks", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_tasks", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_tasks", "start_date", DateTime.UtcNow);
            columnMapper.SetMapping("projects_templates", "create_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_templates", "last_modified_on", DateTime.UtcNow);
            columnMapper.SetMapping("projects_time_tracking", "create_on", DateTime.UtcNow);

            //set up for crm:
            columnMapper.SetMapping("crm_task", "deadline", DateTime.UtcNow.AddDays(7), val => !IsDefaultDateTime(val));

            return columnMapper;
        }

        private static bool IsDefaultDateTime(object val)
        {
            string strVal = Convert.ToString(val);
            if (!string.IsNullOrEmpty(strVal))
            {
                DateTime dt;
                if (DateTime.TryParse(strVal, out dt))
                {
                    return dt == DateTime.MinValue;
                }
            }
            return true;
        }
    }
}
