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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using ASC.Files.Core.Security;

namespace ASC.Files.Core
{
    [DataContract]
    public enum FolderType
    {
        [EnumMember] DEFAULT = 0,

        [EnumMember] COMMON = 1,

        [EnumMember] BUNCH = 2,

        [EnumMember] TRASH = 3,

        [EnumMember] USER = 5,

        [EnumMember] SHARE = 6,

        [EnumMember] CRM = 7,

        [EnumMember] Projects = 8,

        [EnumMember] PHOTOS = 9
    }

    [DataContract(Name = "folder", Namespace = "")]
    [DebuggerDisplay("{Title} ({ID})")]
    public class Folder : FileEntry
    {
        public FolderType FolderType { get; set; }

        public object ParentFolderID { get; set; }

        [DataMember(Name = "total_files", EmitDefaultValue = true, IsRequired = false)]
        public int TotalFiles { get; set; }

        [DataMember(Name = "total_sub_folder", EmitDefaultValue = true, IsRequired = false)]
        public int TotalSubFolders { get; set; }

        [DataMember(Name = "shareable")]
        public bool Shareable { get; set; }

        [DataMember(Name = "isnew")]
        public int NewForMe { get; set; }

        [DataMember(Name = "folder_url", EmitDefaultValue = false)]
        public string FolderUrl { get; set; }

        public Folder()
        {
            Title = String.Empty;
        }
    }

    public class SharedFolder : Folder
    {
        public SharedFolder()
        {
            Shares = new List<SmallShareRecord>();
        }

        public override Guid CreateBy { get; set; }

        public override Guid ModifiedBy { get; set; }

        public List<SmallShareRecord> Shares { get; set; }
    }
}