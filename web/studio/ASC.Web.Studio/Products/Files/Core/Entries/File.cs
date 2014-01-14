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
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Configuration;
using ASC.Files.Core.Security;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core
{
    [Flags]
    [DataContract]
    public enum FileStatus
    {
        [EnumMember] None = 0x0,

        [EnumMember] IsEditing = 0x1,

        [EnumMember] IsNew = 0x2,

        [EnumMember] IsConverting = 0x4,

        [EnumMember] IsOriginal = 0x8
    }

    [DataContract(Name = "file", Namespace = "")]
    [DebuggerDisplay("{Title} ({ID} v{Version})")]
    public class File : FileEntry
    {
        private FileStatus _status;

        public File()
        {
            Version = 1;
        }

        public object FolderID { get; set; }

        [DataMember(EmitDefaultValue = true, Name = "version", IsRequired = false)]
        public int Version { get; set; }

        public String PureTitle
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        public override string Title
        {
            get
            {
                return String.IsNullOrEmpty(ConvertedType)
                           ? base.Title
                           : FileUtility.ReplaceFileExtension(base.Title, FileUtility.GetInternalExtension(base.Title));
            }
            set { base.Title = value; }
        }

        public long ContentLength { get; set; }

        [DataMember(EmitDefaultValue = false, Name = "content_length", IsRequired = true)]
        public String ContentLengthString
        {
            get { return FileSizeComment.FilesSizeToString(ContentLength); }
            set { }
        }

        public FilterType FilterType
        {
            get
            {
                switch (FileUtility.GetFileTypeByFileName(Title))
                {
                    case FileType.Image:
                        return FilterType.ImagesOnly;
                    case FileType.Document:
                        return FilterType.DocumentsOnly;
                    case FileType.Presentation:
                        return FilterType.PresentationsOnly;
                    case FileType.Spreadsheet:
                        return FilterType.SpreadsheetsOnly;
                }

                return FilterType.None;
            }
        }

        [DataMember(EmitDefaultValue = true, Name = "file_status", IsRequired = false)]
        public FileStatus FileStatus
        {
            get
            {
                if (!FileLocker.IsLocked(ID))
                {
                    _status &= (~FileStatus.IsEditing);
                }
                else
                {
                    _status |= FileStatus.IsEditing;
                }

                if (!FileConverter.IsConverting(this))
                {
                    _status &= (~FileStatus.IsConverting);
                }
                else
                {
                    _status |= FileStatus.IsConverting;
                }

                DateTime requiredDate;
                if (
                    DateTime.TryParse(WebConfigurationManager.AppSettings["files.mark-original.date"], out requiredDate)
                    && ModifiedOn.Date < requiredDate
                    && String.IsNullOrEmpty(ConvertedType)
                    && !ProviderEntry
                    && new List<string> { ".docx", ".xlsx", ".pptx" }.Contains(FileUtility.GetFileExtension(Title))
                    )
                {
                    _status |= FileStatus.IsOriginal;
                }
                else
                {
                    _status &= (~FileStatus.IsOriginal);
                }

                return _status;
            }
            set { _status = value; }
        }

        public String ThumbnailURL { get; set; }

        public String FileDownloadUrl
        {
            get { return CommonLinkUtility.GetFileDownloadUrl(ID); }
        }

        public String ViewUrl
        {
            get { return CommonLinkUtility.GetFileViewUrl(ID); }
        }

        public string ConvertedType { get; set; }

        public string ConvertedExtension
        {
            get
            {
                if (string.IsNullOrEmpty(ConvertedType)) return FileUtility.GetFileExtension(Title);

                //hack: Use only for old internal format

                var curFileType = FileUtility.GetFileTypeByFileName(Title);
                switch (curFileType)
                {
                    case FileType.Image:
                        return ConvertedType == ".zip" ? ".pptt" : ConvertedType;
                    case FileType.Spreadsheet:
                        return ConvertedType != ".xlsx" ? ".xlst" : ConvertedType;
                }
                return ConvertedType;
            }
        }

        public object NativeAccessor { get; set; }

        public static string Serialize(File file)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(File));
                serializer.WriteObject(ms, file);
                ms.Seek(0, SeekOrigin.Begin);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }
    }

    public class SharedFile : File
    {
        public SharedFile()
        {
            Shares = new List<SmallShareRecord>();
        }

        public override Guid CreateBy { get; set; }

        public override Guid ModifiedBy { get; set; }

        public List<SmallShareRecord> Shares { get; set; }
    }
}