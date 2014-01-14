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
using ASC.Api.Employee;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Specific;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Namespace = "")]
    public abstract class FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public object Id { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Title { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool SharedByMe { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(Order = 51, EmitDefaultValue = false)]
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime _updated;

        /// <summary>
        /// </summary>
        [DataMember(Order = 52, EmitDefaultValue = false)]
        public ApiDateTime Updated
        {
            get
            {
                return _updated < Created ? Created : _updated;
            }
            set { _updated = value; }
        }

        /// <summary>
        /// </summary>
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public FolderType RootFolderType { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(Order = 41, EmitDefaultValue = false)]
        public EmployeeWraper UpdatedBy { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        protected FileEntryWrapper(FileEntry entry)
        {
            Id = entry.ID;
            Title = entry.Title;
            Access = entry.Access;
            SharedByMe = entry.SharedByMe;
            Created = (ApiDateTime)entry.CreateOn;
            CreatedBy = EmployeeWraper.Get(entry.CreateBy);
            Updated = (ApiDateTime)entry.ModifiedOn;
            UpdatedBy = EmployeeWraper.Get(entry.ModifiedBy);
            RootFolderType = entry.RootFolderType;
        }

        /// <summary>
        /// 
        /// </summary>
        protected FileEntryWrapper()
        {

        }
    }
}