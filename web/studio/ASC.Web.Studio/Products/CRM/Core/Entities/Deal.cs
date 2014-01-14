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

#region Usings

using System;
using System.Runtime.Serialization;
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class Deal : DomainObject, ISecurityObjectId
    {
        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid? LastModifedBy { get; set; }

        public DateTime? LastModifedOn { get; set; }

        [DataMember(Name = "contact_id")]
        public int ContactID { get; set; }

        [DataMember(Name = "contact")]
        public Contact Contact { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "responsible_id")]
        public Guid ResponsibleID { get; set; }

        [DataMember(Name = "bid_type")]
        public BidType BidType { get; set; }

        [DataMember(Name = "bid_value")]
        public decimal BidValue { get; set; }

        [DataMember(Name = "bid_currency")]
        public string BidCurrency { get; set; }
        
        [DataMember(Name = "per_period_value")]
        public int PerPeriodValue { get; set; }

        [DataMember(Name = "deal_milestone")]
        public int DealMilestoneID { get; set; }

        [DataMember(Name = "deal_milestone_probability")]
        public int DealMilestoneProbability { get; set; }

        public DateTime ActualCloseDate { get; set; }

        [DataMember(Name = "actual_close_date")]
        private String ActualCloseDateStr
        {
            get
            {
                return ActualCloseDate.Date == DateTime.MinValue.Date
                           ? string.Empty : ActualCloseDate.ToString(DateTimeExtension.DateFormatPattern);
            }
            set { ; }
        }

        
        public DateTime ExpectedCloseDate { get; set; }

        [DataMember(Name = "expected_close_date")]
        private String ExpectedCloseDateStr
        {
            get
            {
                return ExpectedCloseDate.Date == DateTime.MinValue.Date
                           ? string.Empty : ExpectedCloseDate.ToString(DateTimeExtension.DateFormatPattern);
            }
            set { ; }
        }
        
        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}
