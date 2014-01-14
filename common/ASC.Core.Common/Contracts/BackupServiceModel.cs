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

namespace ASC.Core.Common.Contracts
{
    [DataContract]
    public class BackupResult
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool Completed { get; set; }

        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public int Percent { get; set; }

        [DataMember]
        public DateTime ExpireDate { get; set; }
    }

    [DataContract]
    public class TransferRegion
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string BaseDomain { get; set; }

        [DataMember]
        public bool IsCurrentRegion { get; set; }
    }

    [DataContract]
    public class TransferRequest
    {
        [DataMember]
        public int TenantId { get; set; }

        [DataMember]
        public string TargetRegion { get; set; }

        [DataMember]
        public bool NotifyUsers { get; set; }

        [DataMember]
        public bool BackupMail { get; set; }
    }
}