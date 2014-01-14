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

#region Import

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security;
using System.Threading;
using ASC.CRM.Core.Entities;
using ASC.Common.Security;
using System.Linq;
using System.Linq.Expressions;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using Action = ASC.Common.Security.Authorizing.Action;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;
using ASC.Web.Files.Api;
using ASC.Files.Core.Security;
using ASC.Files.Core;

#endregion

namespace ASC.CRM.Core
{

    public class FileSecurity : IFileSecurity
    {


        #region IFileSecurity Members

        public bool CanCreate(FileEntry file, Guid userId)
        {
            return true;
        }

        public bool CanDelete(FileEntry file, Guid userId)
        {
            return file.CreateBy == userId || file.ModifiedBy == userId || CRMSecurity.IsAdmin;

        }

        public bool CanEdit(FileEntry file, Guid userId)
        {
            return file.CreateBy == userId || file.ModifiedBy == userId || CRMSecurity.IsAdmin;
        }

        public bool CanRead(FileEntry file, Guid userId)
        {
            var eventDao = Global.DaoFactory.GetRelationshipEventDao();
            var tagDao = FilesIntegration.GetTagDao();

            var eventIds = tagDao.GetTags(file.ID, FileEntryType.File, TagType.System)
                       .Where(x => x.TagName.StartsWith("RelationshipEvent_"))
                       .Select(x => Convert.ToInt32(x.TagName.Split(new[] { '_' })[1]));

            if (!eventIds.Any()) return false;

            return CRMSecurity.CanAccessTo(eventDao.GetByID(eventIds.First()));
        }

        #endregion
    }

    public class FileSecurityProvider : IFileSecurityProvider
    {
        public IFileSecurity GetFileSecurity(string data)
        {
            return new FileSecurity();
        }

    }
}