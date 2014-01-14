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
using ASC.Core.Tenants;
using ASC.Specific;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Name = "case", Namespace = "")]
    public class CasesWrapper : ObjectWrapperBase
    {

        public CasesWrapper()
            : base(0)
        {

        }

        public CasesWrapper(Cases cases)
            : base(cases.ID)
        {

            CreateBy = EmployeeWraper.Get(cases.CreateBy);
            Created = (ApiDateTime)cases.CreateOn;
            Title = cases.Title;
            IsClosed = cases.IsClosed;

            IsPrivate = CRMSecurity.IsPrivate(cases);

            if (IsPrivate)
            AccessList = CRMSecurity.GetAccessSubjectTo(cases).SkipWhile(
                item => item.Key == ASC.Core.Users.Constants.GroupEveryone.ID)
                .Select(item => EmployeeWraper.Get(item.Key));

           
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Members { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }
        
        public static CasesWrapper GetSample()
        {

            return new CasesWrapper
            {
                IsClosed = false,
                Title = "Exhibition organization",
                Created = (ApiDateTime)DateTime.UtcNow,
                CreateBy = EmployeeWraper.GetSample(),
                IsPrivate = false,
                CustomFields = new[] { CustomFieldBaseWrapper.GetSample()}
            };
        }


    }
}
