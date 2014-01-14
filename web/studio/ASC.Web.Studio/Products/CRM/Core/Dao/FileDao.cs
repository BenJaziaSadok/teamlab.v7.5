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
using ASC.Files.Core;
using ASC.Web.Files.Api;

namespace ASC.CRM.Core.Dao
{
    public class FileDao : AbstractDao
    {
        public FileDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {
        }

        #region Public Methods

        public File GetFile(int id, int version)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                var file = 0 < version ? dao.GetFile(id, version) : dao.GetFile(id);
                return file;
            }
        }

        public void DeleteFile(int id, int version)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                dao.DeleteFile(id);
            }
        }

        public object GetRoot()
        {
            return FilesIntegration.RegisterBunch("crm", "crm_common", "");
        }

        public File SaveFile(File file, System.IO.Stream stream)
        {
            using (var dao = FilesIntegration.GetFileDao())
            {
                return dao.SaveFile(file, stream);
            }
        }

        #endregion
    }
}