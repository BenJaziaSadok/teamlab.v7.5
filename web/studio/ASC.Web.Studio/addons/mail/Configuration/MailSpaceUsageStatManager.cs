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
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Mail.Configuration
{
    public class MailSpaceUsageStatManager : SpaceUsageStatManager
    {
        private const string MailDatabaseId = "mail";

        public override List<UsageSpaceStatItem> GetStatData()
        {
            if (!DbRegistry.IsDatabaseRegistered(MailDatabaseId))
            {
                DbRegistry.RegisterDatabase(MailDatabaseId, ConfigurationManager.ConnectionStrings[MailDatabaseId]);
            }

            using (var mail_db = new DbManager(MailDatabaseId))
            {
                var query = new SqlQuery("mail_attachment a")
                    .InnerJoin("mail_mail m", Exp.EqColumns("a.id_mail", "m.id"))
                    .Select("m.id_user")
                    .Select("sum(a.size) as size")
                    .Where("m.tenant", TenantProvider.CurrentTenantID)
                    .Where("a.need_remove", 0)
                    .GroupBy(1)
                    .OrderBy(2, false);

                return mail_db.ExecuteList(query)
                    .Select(r =>
                        {
                            var user_id = new Guid(Convert.ToString(r[0]));
                            var user = CoreContext.UserManager.GetUsers(user_id);
                            var item = new UsageSpaceStatItem
                                {
                                    Name = DisplayUserSettings.GetFullUserName(user, false),
                                    ImgUrl = UserPhotoManager.GetSmallPhotoURL(user.ID),
                                    Url = CommonLinkUtility.GetUserProfile(user.ID),
                                    SpaceUsage = Convert.ToInt64(r[1])
                                };
                            return item;
                        })
                    .ToList();
            }
        }
    }
}
