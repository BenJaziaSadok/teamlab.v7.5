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
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Files.Core;
using FilesGlobal = ASC.Web.Files.Classes.Global;

namespace ASC.Api.CRM
{
    public class CRMApiBase
    {
        private DaoFactory _crmDaoFactory;
        private IDaoFactory _filesDaoFactory;

        protected DaoFactory DaoFactory
        {

            get
            {
                if (_crmDaoFactory == null)
                {
                    _crmDaoFactory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId, CRMConstants.DatabaseId);
                }
                return _crmDaoFactory;
            }
        }

        protected IDaoFactory FilesDaoFactory
        {

            get
            {
                if (_filesDaoFactory == null)
                {
                    _filesDaoFactory = FilesGlobal.DaoFactory;
                }
                return _filesDaoFactory;
            }
        }
    }
}
