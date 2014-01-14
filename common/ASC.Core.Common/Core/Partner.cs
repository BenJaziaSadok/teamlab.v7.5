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
using System.Runtime.Serialization;

namespace ASC.Core
{
    [DataContract]
    public class Partner
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        public string LastName { get; set; }

        [DataMember(Name = "Url")]
        public string Url { get; set; }

        [DataMember(Name = "Phone")]
        public string Phone { get; set; }

        [DataMember(Name = "Language")]
        public string Language { get; set; }

        [DataMember(Name = "CompanyName")]
        public string CompanyName { get; set; }

        [DataMember(Name = "Country")]
        public string Country { get; set; }

        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "CountryHasVat")]
        public bool CountryHasVat { get; set; }

        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "VatId")]
        public string VatId { get; set; }

        [DataMember(Name = "CreationDate")]
        public DateTime CreationDate { get; set; }

        [DataMember(Name = "Status")]
        public PartnerStatus Status { get; set; }

        [DataMember(Name = "Comment")]
        public string Comment { get; set; }

        [DataMember(Name = "Portal")]
        public string Portal { get; set; }

        [DataMember(Name = "PortalConfirmed")]
        public bool PortalConfirmed { get; set; }

        [DataMember(Name = "IsAdmin")]
        public bool IsAdmin { get { return PartnerType == PartnerType.Administrator; } }

        [DataMember(Name = "Limit")]
        public decimal Limit { get; set; }

        [DataMember(Name = "Discount")]
        public int Discount { get; set; }

        [DataMember(Name = "PayPalAccount")]
        public string PayPalAccount { get; set; }

        [DataMember(Name = "Deposit")]
        public decimal Deposit { get; set; }

        [DataMember(Name = "Removed")]
        public bool Removed { get; set; }

        [DataMember(Name = "Currency")]
        public string Currency { get; set; }

        [DataMember(Name = "LogoUrl")]
        public string LogoUrl { get; set; }

        [DataMember(Name = "DisplayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "DisplayType")]
        public PartnerDisplayType DisplayType { get; set; }

        [DataMember(Name = "SupportPhone")]
        public string SupportPhone { get; set; }

        [DataMember(Name = "SupportEmail")]
        public string SupportEmail { get; set; }

        [DataMember(Name = "SalesEmail")]
        public string SalesEmail { get; set; }

        [DataMember(Name = "Theme")]
        public string Theme { get; set; }

        [DataMember(Name = "RuAccount")]
        public string RuAccount { get; set; }

        [DataMember(Name = "RuBank")]
        public string RuBank { get; set; }

        [DataMember(Name = "RuKs")]
        public string RuKs { get; set; }

        [DataMember(Name = "RuKpp")]
        public string RuKpp { get; set; }

        [DataMember(Name = "RuBik")]
        public string RuBik { get; set; }

        [DataMember(Name = "RuInn")]
        public string RuInn { get; set; }

        [DataMember(Name = "PartnerType")]
        public PartnerType PartnerType { get; set; }

        [DataMember(Name = "PaymentMethod")]
        public PartnerPaymentMethod PaymentMethod { get; set; }

        [DataMember(Name = "PaymentUrl")]
        public string PaymentUrl { get; set; }

        [DataMember(Name = "AvailableCredit")]
        public decimal AvailableCredit { get; set; }

        public override bool Equals(object obj)
        {
            var p = obj as Partner;
            return p != null && p.Id == Id;
        }

        public override int GetHashCode()
        {
            return (Id ?? string.Empty).GetHashCode();
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}