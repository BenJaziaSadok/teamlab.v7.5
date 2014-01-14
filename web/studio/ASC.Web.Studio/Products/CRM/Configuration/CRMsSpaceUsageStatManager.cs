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
using System.Configuration;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Files.Core;
using ASC.Web.Core;
using System.Web;
using ASC.Web.Studio.Utility;

namespace ASC.Web.CRM.Configuration
{
    public class CRMSpaceUsageStatManager : SpaceUsageStatManager
    {        
        public override List<UsageSpaceStatItem> GetStatData()
        {
            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
            {
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId, ConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            }

            using (var filedb = new DbManager(FileConstant.DatabaseId))            
            {
                var q = new SqlQuery("files_file f")
                    .Select("b.right_node")
                    .SelectSum("f.content_length")
                    .InnerJoin("files_folder_tree t", Exp.EqColumns("f.folder_id", "t.folder_id"))
                    .InnerJoin("files_bunch_objects b", Exp.EqColumns("t.parent_id", "b.left_node"))
                    .Where("b.tenant_id", TenantProvider.CurrentTenantID)
                    .Where(Exp.Like("b.right_node", "crm/crm_common/", SqlLike.StartWith))
                    .GroupBy(1);

                return filedb.ExecuteList(q)
                    .Select(r => new UsageSpaceStatItem
                        {

                            Name = Resources.CRMCommonResource.ProductName,
                            SpaceUsage = Convert.ToInt64(r[1]),
                            Url = VirtualPathUtility.ToAbsolute(PathProvider.StartURL())
                        })
                    .ToList();
            }        
        }
    }
}
