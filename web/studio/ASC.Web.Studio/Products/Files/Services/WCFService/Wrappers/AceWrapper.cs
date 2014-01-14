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
using ASC.Files.Core.Security;
using ASC.Web.Files.Resources;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "ace_wrapper", Namespace = "")]
    public class AceWrapper
    {
        [DataMember(Name = "id", Order = 1)]
        public Guid SubjectId { get; set; }

        [DataMember(Name = "title", Order = 2)]
        public string SubjectName { get; set; }

        [DataMember(Name = "is_group", Order = 3)]
        public bool SubjectGroup { get; set; }

        [DataMember(Name = "owner", Order = 4)]
        public bool Owner { get; set; }

        [DataMember(Name = "ace_status", Order = 5)]
        public FileShare Share { get; set; }

        [DataMember(Name = "locked", Order = 6)]
        public bool LockedRights { get; set; }

        [DataMember(Name = "disable_remove", Order = 7)]
        public bool DisableRemove { get; set; }

        [DataMember(Name = "message", Order = 8, EmitDefaultValue = false, IsRequired = false)]
        public string Message { get; set; }
    }

    [DataContract(Name = "sharingSettings", Namespace = "")]
    public class AceShortWrapper
    {
        [DataMember(Name = "user")]
        public string User { get; set; }

        [DataMember(Name = "permissions")]
        public string Permissions { get; set; }

        public AceShortWrapper(AceWrapper aceWrapper)
        {
            var permission = string.Empty;

            switch (aceWrapper.Share)
            {
                case FileShare.Read:
                    permission = FilesCommonResource.AceStatusEnum_Read;
                    break;
                case FileShare.ReadWrite:
                    permission = FilesCommonResource.AceStatusEnum_ReadWrite;
                    break;
                case FileShare.Restrict:
                    permission = FilesCommonResource.AceStatusEnum_Restrict;
                    break;
            }

            User = aceWrapper.SubjectName;
            Permissions = permission;
        }
    }
}