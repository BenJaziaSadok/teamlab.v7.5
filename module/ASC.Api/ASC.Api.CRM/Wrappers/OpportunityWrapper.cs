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
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Opportunity
    /// </summary>
    [DataContract(Name = "opportunity", Namespace = "")]
    public class OpportunityWrapper : ObjectWrapperBase
    {
        public OpportunityWrapper(Deal deal)
            : base(deal.ID)
        {
          
            CreateBy = EmployeeWraper.Get(deal.CreateBy);
            Created = (ApiDateTime)deal.CreateOn;
            Title = deal.Title;
            Description = deal.Description;
            Responsible = EmployeeWraper.Get(deal.ResponsibleID);
            BidType = deal.BidType;
            BidValue = deal.BidValue;
            PerPeriodValue = deal.PerPeriodValue;
            SuccessProbability = deal.DealMilestoneProbability;
            ActualCloseDate = (ApiDateTime)deal.ActualCloseDate;
            ExpectedCloseDate = (ApiDateTime)deal.ExpectedCloseDate;

        }

        public OpportunityWrapper(int id)
            : base(id)
        {

        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Members { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Contact { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BidType BidType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public decimal BidValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CurrencyInfoWrapper BidCurrency { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int PerPeriodValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DealMilestoneBaseWrapper Stage { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int SuccessProbability { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime ActualCloseDate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime ExpectedCloseDate { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }

        public static OpportunityWrapper GetSample()
        {
            return new OpportunityWrapper(0)
            {
                CreateBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Responsible = EmployeeWraper.GetSample(),
                Title = "Hotel catalogue",
                Description = "",
                ExpectedCloseDate = (ApiDateTime)DateTime.UtcNow.AddDays(10),
                Contact = ContactBaseWrapper.GetSample(),
                IsPrivate = false,
                SuccessProbability = 65,
                BidType = BidType.FixedBid,
                Stage = DealMilestoneBaseWrapper.GetSample()
            };
        }

    }
}