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
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core.Tenants;
using ASC.Specific;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    [DataContract(Name = "customField", Namespace = "")]
    public class CustomFieldWrapper : CustomFieldBaseWrapper
    {
        public CustomFieldWrapper(int id)
            : base(id)
        {

        }
         public CustomFieldWrapper(CustomField customField)
            : base(customField)
        {

         }

        [DataMember]
        public int RelativeItemsCount { get; set; }
    }

    /// <summary>
    ///  User custom fields
    /// </summary>
    [DataContract(Name = "customField", Namespace = "")]
    public class CustomFieldBaseWrapper : ObjectWrapperBase
    {
        public CustomFieldBaseWrapper(int id):base(id)
        {
            
        }

        public CustomFieldBaseWrapper(CustomField customField)
            : base(customField.ID)
        {
            EntityId = customField.EntityID;
            Label = customField.Label;
            FieldValue = customField.Value;
            FieldType = customField.FieldType;
            Position = customField.Position;
            Mask = customField.Mask;
        }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String Label { get; set; }

        [DataMember]
        public String FieldValue { get; set; }

        [DataMember]
        public CustomFieldType FieldType { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public String Mask { get; set; }

        public static CustomFieldBaseWrapper GetSample()
        {
            return  new CustomFieldBaseWrapper(0)
                        {
                            Position = 10,
                            EntityId = 14523423,
                            FieldType = CustomFieldType.Date,
                            FieldValue = ((ApiDateTime)DateTime.UtcNow).ToString(),
                            Label = "Birthdate",
                            Mask = ""
                        };
        }
    }
}
