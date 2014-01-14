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
using System.Runtime.Serialization;
using ASC.Files.Core;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "composite_data")]
    public class DataWrapper
    {
        [DataMember(IsRequired = false, Name = "entries", EmitDefaultValue = false)]
        public List<FileEntry> Entries { get; set; }

        [DataMember(IsRequired = false, Name = "total", EmitDefaultValue = true)]
        public int Total { get; set; }

        [DataMember(IsRequired = false, Name = "path_parts", EmitDefaultValue = true)]
        public ItemDictionary<object, String> FolderPathParts { get; set; }

        [DataMember(IsRequired = false, Name = "folder_info", EmitDefaultValue = true)]
        public Folder FolderInfo { get; set; }

        [DataMember(IsRequired = false, Name = "root_folders_id_marked_as_new", EmitDefaultValue = true)]
        public ItemDictionary<object, int> RootFoldersIdMarkedAsNew { get; set; }
    }
}