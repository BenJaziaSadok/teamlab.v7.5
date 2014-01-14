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
using System.Linq;
using ASC.CRM.Core.Entities;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.Files.Api;
using ASC.Web.Studio.Helpers;
using ASC.Web.Studio.Utility;
using log4net;

#endregion

namespace ASC.CRM.Core.Dao
{

    public class ManagerDao : AbstractDao
    {

        #region Constructor

        public ManagerDao(int tenantID, String storageKey)
            : base(tenantID, storageKey)
        {


        }

        #endregion

        #region Methods

        public void Add(Guid managerID)
        {
            DbManager.ExecuteNonQuery(Insert("crm_manager").InColumnValue("id", managerID));

        }

        public void Remove(Guid managerID)
        {

            DbManager.ExecuteNonQuery(Delete("crm_manager").Where(Exp.Eq("id", managerID)));

        }

        public List<UserInfo> GetAll(bool includeAdmins)
        {

            var managers = DbManager.ExecuteList(Query("crm_manager").Select("id")).ConvertAll(row=> ASC.Core.CoreContext.UserManager.GetUsers(ToGuid(row[0])));
           
            if (includeAdmins)
              return managers.Union(ASC.Core.CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID)).Distinct().ToList();

            return managers;
        }

        #endregion

    }

}