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
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Core.Tenants
{
    [DataContract]
    [DebuggerDisplay("{Name}")]
    public class TenantQuota : ICloneable
    {
        public static readonly TenantQuota Default = new TenantQuota(Tenant.DEFAULT_TENANT)
            {
                Name = "Default",
                MaxFileSize = 25 * 1024 * 1024, // 25Mb
                MaxTotalSize = long.MaxValue,
                ActiveUsers = int.MaxValue,
            };

        [DataMember(Name = "Id", Order = 10)]
        public int Id { get; private set; }

        [DataMember(Name = "Name", Order = 20)]
        public string Name { get; set; }

        [DataMember(Name = "MaxFileSize", Order = 30)]
        public long MaxFileSize { get; set; }

        [DataMember(Name = "MaxTotalSize", Order = 40)]
        public long MaxTotalSize { get; set; }

        [DataMember(Name = "ActiveUsers", Order = 50)]
        public int ActiveUsers { get; set; }

        [DataMember(Name = "Features", Order = 60)]
        public string Features { get; set; }

        [DataMember(Name = "Price", Order = 70)]
        public decimal Price { get; set; }

        [DataMember(Name = "Price2", Order = 80)]
        public decimal Price2 { get; set; }

        [DataMember(Name = "AvangateId", Order = 90)]
        public string AvangateId { get; set; }

        [DataMember(Name = "Visible", Order = 100)]
        public bool Visible { get; set; }

        public bool Trial
        {
            get { return GetFeature("trial"); }
            set { SetFeature("trial", value); }
        }

        public bool HasBackup
        {
            get { return GetFeature("backup"); }
            set { SetFeature("backup", value); }
        }

        public bool HasDomain
        {
            get { return GetFeature("domain"); }
            set { SetFeature("domain", value); }
        }

        public bool DocsEdition
        {
            get { return GetFeature("docs"); }
            set { SetFeature("docs", value); }
        }

        [DataMember(Name = "Year", Order = 110)]
        public bool Year
        {
            get { return GetFeature("year"); }
            set { SetFeature("year", value); }
        }

        public bool NonProfit
        {
            get { return GetFeature("non-profit"); }
            set { SetFeature("non-profit", value); }
        }

        public bool Sms
        {
            get { return GetFeature("sms"); }
            set { SetFeature("sms", value); }
        }

        public bool Gantt
        {
            get { return GetFeature("gantt"); }
            set { SetFeature("gantt", value); }
        }

        public bool Visitor
        {
            get { return GetFeature("visitor"); }
            set { SetFeature("visitor", value); }
        }


        public TenantQuota(int tenant)
        {
            Id = tenant;
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var q = obj as TenantQuota;
            return q != null && q.Id == Id;
        }


        public bool GetFeature(string feature)
        {
            return !string.IsNullOrEmpty(Features) && Features.Split(' ', ',', ';').Contains(feature);
        }

        internal void SetFeature(string feature, bool set)
        {
            var features = (Features ?? string.Empty).Split(' ', ',', ';').ToList();
            if (set && !features.Contains(feature))
            {
                features.Add(feature);
            }
            else if (!set && features.Contains(feature))
            {
                features.Remove(feature);
            }
            Features = string.Join(",", features.ToArray());
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}