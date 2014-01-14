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

using System.Collections.Generic;
using ASC.Common.Data.Sql;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        #region db defines

        // ReSharper disable InconsistentNaming
        public const string MAIL_DISPLAY_IMAGES = "mail_display_images";
        // ReSharper restore InconsistentNaming

        public struct DisplayImagesFields
        {
            public static string id_user = "id_user";
            public static string id_tenant = "tenant";
            public static string address = "address";
        };

        #endregion

        #region public methods

        public void AddDisplayImagesAddress(int tenant, string user, string address)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                    new SqlInsert(MAIL_DISPLAY_IMAGES)
                        .InColumnValue(DisplayImagesFields.id_tenant, tenant)
                        .InColumnValue(DisplayImagesFields.id_user, user)
                        .InColumnValue(DisplayImagesFields.address, address));
            }
        }

        public void RemovevDisplayImagesAddress(int tenant, string user, string address)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                    new SqlDelete(MAIL_DISPLAY_IMAGES)
                        .Where(GetUserWhere(user, tenant))
                        .Where(DisplayImagesFields.address, address));
            }
        }

        public List<string> GetDisplayImagesAddresses(int tenant, string user)
        {
            List<string> addresses;

            using (var db = GetDb())
            {
                addresses = db.ExecuteList(
                    new SqlQuery(MAIL_DISPLAY_IMAGES)
                        .Select(DisplayImagesFields.address)
                        .Where(GetUserWhere(user, tenant)))
                        .ConvertAll(fields => fields[0].ToString());
            }

            return addresses;
        }

        #endregion

    }
}
