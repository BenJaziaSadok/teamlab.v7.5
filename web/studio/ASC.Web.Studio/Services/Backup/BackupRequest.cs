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

using System.Runtime.Serialization;

namespace ASC.Web.Studio.Services.Backup
{
    [DataContract(Namespace = "")]
    public enum BackupRequestStatus
    {
        [EnumMember(Value = "started")]
        Started,
        [EnumMember(Value = "working")]
        Working,
        [EnumMember(Value = "uploading")]
        Uploading,
        [EnumMember(Value = "done")]
        Done,
        [EnumMember(Value = "expired")]
        Expired,
        [EnumMember(Value = "error")]
        Error,
    }

    [DataContract(Namespace = "", Name = "backuprequest")]
    public class BackupRequest
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "status")]
        public BackupRequestStatus Status { get; set; }

        [DataMember(Name = "percentdone")]
        public int Percentdone { get; set; }

        [DataMember(Name = "completed")]
        public bool Completed { get; set; }

        [DataMember(Name = "link")]
        public string FileLink { get; set; }
    }
}