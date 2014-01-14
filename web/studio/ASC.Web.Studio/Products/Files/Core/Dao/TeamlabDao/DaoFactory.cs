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

using ASC.Core;
using ASC.Files.Core.Security;

namespace ASC.Files.Core.Data
{
    public class DaoFactory : IDaoFactory
    {
        public DaoFactory()
        {
            
        }

        public IFileDao GetFileDao()
        {
            return new FileDao(CoreContext.TenantManager.GetCurrentTenant().TenantId, FileConstant.DatabaseId);
        }

        public IFolderDao GetFolderDao()
        {
            return new FolderDao(CoreContext.TenantManager.GetCurrentTenant().TenantId, FileConstant.DatabaseId);
        }

        public ITagDao GetTagDao()
        {
            return new TagDao(CoreContext.TenantManager.GetCurrentTenant().TenantId, FileConstant.DatabaseId);
        }

        public ISecurityDao GetSecurityDao()
        {
            return new SecurityDao(CoreContext.TenantManager.GetCurrentTenant().TenantId, FileConstant.DatabaseId);
        }

        public IProviderDao GetProviderDao()
        {
            return null;
        }
    }
}