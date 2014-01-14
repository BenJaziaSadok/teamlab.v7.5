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
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Specific;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "file", Namespace = "")]
    public class FileWrapper : FileEntryWrapper
    {
        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public object FolderId { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public int Version { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public String ContentLength { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = true)]
        public long PureContentLength { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = true, IsRequired = false)]
        public FileStatus FileStatus { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public String ViewUrl { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public String WebUrl { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        public FileWrapper(File file)
            : base(file)
        {
            FolderId = file.FolderID;
            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, SecurityContext.CurrentAccount.ID))
            {
                RootFolderType = FolderType.SHARE;
                using (var folderDao = Global.DaoFactory.GetFolderDao())
                {
                    var parentFolder = folderDao.GetFolder(file.FolderID);
                    if (!Global.GetFilesSecurity().CanRead(parentFolder))
                        FolderId = Global.FolderShare;
                }
            }

            Version = file.Version;
            ContentLength = file.ContentLengthString;
            FileStatus = file.FileStatus;
            PureContentLength = file.ContentLength;
            try
            {
                ViewUrl = file.ViewUrl;

                WebUrl = CommonLinkUtility.GetFileWebPreviewUrl(file.Title, file.ID);
            }
            catch (Exception)
            {
                //Don't catch anything here because of httpcontext
            }
        }

        private FileWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileWrapper GetSample()
        {
            return new FileWrapper
                {
                    Access = FileShare.ReadWrite,
                    Updated = (ApiDateTime) DateTime.UtcNow,
                    Created = (ApiDateTime) DateTime.UtcNow,
                    CreatedBy = EmployeeWraper.GetSample(),
                    Id = new Random().Next(),
                    RootFolderType = FolderType.BUNCH,
                    SharedByMe = false,
                    Title = "Some titile",
                    UpdatedBy = EmployeeWraper.GetSample(),
                    ContentLength = 12345.ToString(),
                    FileStatus = FileStatus.IsNew,
                    FolderId = 12334,
                    Version = 3,
                    ViewUrl = "http://www.teamlab.com/viewfile?fileid=2221"
                };
        }
    }
}