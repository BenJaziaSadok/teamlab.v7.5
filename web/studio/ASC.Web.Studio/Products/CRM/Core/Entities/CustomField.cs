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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace ASC.CRM.Core.Entities
{
    
    [DataContract]
    public class CustomField : DomainObject
    {
        [DataMember(Name = "entity_type")]
        public EntityType EntityType { get; set; }
        
        [DataMember(Name = "entity_id")]
        public int EntityID { get; set; }

        [DataMember(Name = "label")]
        public String Label { get; set; }

        [DataMember(Name = "value")]
        public String Value { get; set; }

        [DataMember(Name = "fieldType")]
        public CustomFieldType FieldType { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "mask")]
        public String Mask { get; set; }

        public override int GetHashCode()
        {
            return string.Format("{0}|{1}|{2}", GetType().FullName, Label, (int)FieldType).GetHashCode();
        }
    }
}
