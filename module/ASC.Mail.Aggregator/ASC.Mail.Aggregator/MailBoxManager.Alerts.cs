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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ASC.Common.Data.Sql;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        #region public defines

        [DataContract]
        public struct MailAlert{
            [DataMember]
            public Int64 id;
            [DataMember]
            public string data;
        }

        #endregion

        #region private defines

        public enum AlertTypes
        {
            DeliveryFailure  = 1,
            LinkFailure = 2,
            ExportFailure = 3
        }

        [DataContract]
        private struct DeliveryFailure
        {
            [DataMember]
            public int type;
            [DataMember]
            public string subject;
            [DataMember]
            public string from;
            [DataMember]
            public int message_id;
        }

        [DataContract]
        private struct CrmOperationFailure
        {
            [DataMember]
            public int type;
            [DataMember]
            public int message_id;
        }

        #endregion

        #region db defines

        private const string MAIL_ALERTS = "mail_alerts";

        private struct MailAlerts
        {
            public static string id = "id";
            public static string id_user = "id_user";
            public static string id_tenant = "tenant";
            public static string data = "data";
        };

        #endregion

        #region public methods

        public List<MailAlert> GetMailAlerts(int tenant, string id_user)
        {
            using (var db = GetDb())
            {
                return db.ExecuteList(
                    new SqlQuery(MAIL_ALERTS)
                        .Select(MailAlerts.id, MailAlerts.data)
                        .Where(GetUserWhere(id_user, tenant)))
                    .ConvertAll(x => new MailAlert{
                        id=Convert.ToInt64(x[0]),
                        data=(string)x[1]
                    });
            }
        }

        public int CreateDeliveryFailureAlert(int tenant, string id_user, string subject, string from, int message_id)
        {
                DeliveryFailure data = new DeliveryFailure();
                data.type = (int)AlertTypes.DeliveryFailure;
                data.from = from;
                data.message_id = message_id;
                data.subject = subject;

                return CreateAlert(tenant, id_user, data);
        }

        public int CreateCrmOperationFailureAlert(int tenant, string id_user, int message_id, AlertTypes type)
        {
            var data = new CrmOperationFailure
                {
                    type = (int) type,
                    message_id = message_id
                };

            return CreateAlert(tenant, id_user, data);
        }

        public void DeleteAlert(int tenant, string id_user, long id)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                    new SqlDelete(MAIL_ALERTS)
                        .Where(GetUserWhere(id_user, tenant))
                        .Where(MailAlerts.id, id));
            }
        }

        #endregion

        #region private methods

        private int CreateAlert(int tenant, string id_user, object data)
        {
            using (var db = GetDb())
            {
                string json_data;
                using (var stream = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(data.GetType());
                    serializer.WriteObject(stream, data);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        json_data = reader.ReadToEnd();
                    }
                }
                return db.ExecuteNonQuery(
                        new SqlInsert(MAIL_ALERTS)
                            .InColumnValue(MailAlerts.id_tenant, tenant)
                            .InColumnValue(MailAlerts.id_user, id_user)
                            .InColumnValue(MailAlerts.data, json_data));
            }
        }

        #endregion
    }
}
