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
using System.Text;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.BusinessObjects
{
    internal class ColumnCollection
    {   
        private List<SelectColumnInfo> _columns = new List<SelectColumnInfo>();

        public class SelectColumnInfo{

            public string Name{get; set;}
            public int Ind{get; set;}

            public T Parse<T>(object[] row)
            {               
                if (typeof(T).Equals(typeof(int)))                
                    return (T)((object)Convert.ToInt32(row[this.Ind]));
                
                if (typeof(T).Equals(typeof(Guid)))
                    return (T)((object)new Guid(Convert.ToString(row[this.Ind])));
                
                if (typeof(T).Equals(typeof(Boolean)))
                    return (T)((object)(Convert.ToBoolean(row[this.Ind])));

                if (typeof(T).Equals(typeof(DateTime)))                
                    return (T)((object)Convert.ToDateTime(row[this.Ind]));

                if (typeof(T).Equals(typeof(RecurrenceRule)))
                {
                    return (T)((object) RecurrenceRule.Parse(Convert.ToString(row[this.Ind])));
                }

                if (typeof(T).Equals(typeof(TimeZoneInfo)))
                {
                    if (IsNull(row))
                        return (T)(object)null;

                    return (T)((object)TimeZoneInfo.FindSystemTimeZoneById(Convert.ToString(row[this.Ind])));
                }
                
                
                return (T)(object)(Convert.ToString(row[this.Ind])??"");
            }

            public bool IsNull(object[] row)
            {
                return row[this.Ind] == null || row[this.Ind] == DBNull.Value;
            }
        }

        public SelectColumnInfo RegistryColumn(string selectName)
        {
            var c = new SelectColumnInfo() { Name = selectName, Ind = _columns.Count};
            _columns.Add(c);
            return c;
        }        

        public string[] SelectQuery
        {
            get
            {
                return _columns.Select(c => c.Name).ToArray();
            }
        }
    }
}
