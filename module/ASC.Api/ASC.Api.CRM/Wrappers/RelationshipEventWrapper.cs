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
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Api.Documents;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    [DataContract(Name = "entity", Namespace = "")]
    public class EntityWrapper
    {
        [DataMember]
        public String EntityType { get; set; }

        [DataMember]
        public int EntityId { get; set; }

        [DataMember]
        public String EntityTitle { get; set; }

        public static EntityWrapper GetSample()
        {
            return new EntityWrapper
                       {
                           EntityId = 123445,
                           EntityType = "opportunity",
                           EntityTitle = "Household appliances internet shop"
                       };
        }
    }


    [DataContract(Name = "historyEvent", Namespace = "")]
    public class RelationshipEventWrapper :
        ObjectWrapperBase
    {

        public RelationshipEventWrapper():
            base(0)
        {
            
        }

        public RelationshipEventWrapper(RelationshipEvent relationshipEvent)
            : base(relationshipEvent.ID)
        {

            CreateBy = EmployeeWraper.Get(relationshipEvent.CreateBy);
            Created = (ApiDateTime)relationshipEvent.CreateOn;
            Content = relationshipEvent.Content;
            Files = new List<FileWrapper>();
            CanEdit = CRMSecurity.CanEdit(relationshipEvent);

        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Content { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public HistoryCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<FileWrapper> Files { get; set; }

        public static RelationshipEventWrapper GetSample()
        {
            return new RelationshipEventWrapper
                        {
                           CanEdit = true,
                           Category = HistoryCategoryBaseWrapper.GetSample(),
                           Entity = EntityWrapper.GetSample(),
                           Contact = ContactBaseWrapper.GetSample(),
                           Created = (ApiDateTime)DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample(),
                           Files = new[] {FileWrapper.GetSample()},
                           Content = @"Agreed to meet at lunch and discuss the client commercial offer
                                       "
                        };
        }

    }
}
