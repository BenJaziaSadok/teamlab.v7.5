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
using ASC.Web.Files.Classes;

namespace ASC.Files.Core
{
    [DataContract(Name = "entry", Namespace="")]
    [KnownType(typeof(Folder))]
    [KnownType(typeof(File))]
    public abstract class FileEntry
    {
        [DataMember(Name = "id")]
        public object ID { get; set; }

        [DataMember(Name = "title", IsRequired = true)]
        public virtual string Title { get; set; }

        [DataMember(Name = "create_by_id")]
        private Guid CreateById { get; set; }

        [DataMember(Name = "create_by")]
        public string CreateByString { get; set; }

        [DataMember(Name = "create_on")]
        private string CreateOnString
        {
            get { return CreateOn.ToString("g"); }
            set { }
        }

        [DataMember(Name = "modified_on")]
        private string ModifiedOnString
        {
            get { return ModifiedOn.Equals(default(DateTime)) ? null : ModifiedOn.ToString("g"); }
            set { }
        }

        [DataMember(Name = "modified_by")]
        public string ModifiedByString { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }

        [DataMember(Name = "access")]
        public FileShare Access { get; set; }

        [DataMember(Name = "shared")]
        public bool SharedByMe { get; set; }

        [DataMember(Name = "provider_id", EmitDefaultValue = false)]
        public int ProviderId { get; set; }

        [DataMember(Name = "provider_key", EmitDefaultValue = false)]
        public string ProviderKey { get; set; }

        public bool ProviderEntry
        {
            get { return !string.IsNullOrEmpty(ProviderKey); }
        }

        public DateTime? SharedToMeOn { get; set; }

        public String SharedToMeBy { get; set; }

        public virtual Guid CreateBy
        {
            get { return CreateById; }
            set
            {
                CreateById = value;
                CreateByString = Global.GetUserName(CreateById);
            }
        }

        public DateTime CreateOn { get; set; }

        private Guid ModifiedById { get; set; }

        public virtual Guid ModifiedBy
        {
            get { return ModifiedById; }
            set
            {
                ModifiedById = value;
                ModifiedByString = Global.GetUserName(ModifiedById);
            }
        }

        public DateTime ModifiedOn { get; set; }

        public FolderType RootFolderType { get; set; }

        public Guid RootFolderCreator { get; set; }

        public object RootFolderId { get; set; }

        public String UniqID
        {
            get { return String.Format("{0}_{1}", GetType().Name.ToLower(), ID); }
        }


        public override bool Equals(object obj)
        {
            var f = obj as FileEntry;
            return f != null && Equals(f.ID, ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}