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

using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using log4net;
using System;
using System.Linq;
using System.Web;

namespace ASC.Geolocation
{
    public class GeolocationHelper
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Geo");

        private readonly string dbid;


        public GeolocationHelper(string dbid)
        {
            this.dbid = dbid;
        }


        public IPGeolocationInfo GetIPGeolocation(string ip)
        {
            try
            {
                var ipformatted = FormatIP(ip);
                using (var db = new DbManager(dbid))
                {
                    var q = new SqlQuery("dbip_location")
                        .Select("ip_start", "ip_end", "country", "city", "timezone_offset", "timezone_name")
                        .Where(Exp.Le("ip_start", ipformatted))
                        .OrderBy("ip_start", false)
                        .SetMaxResults(1);
                    return db
                        .ExecuteList(q)
                        .Select(r => new IPGeolocationInfo()
                        {
                            IPStart = Convert.ToString(r[0]),
                            IPEnd = Convert.ToString(r[1]),
                            Key = Convert.ToString(r[2]),
                            City = Convert.ToString(r[3]),
                            TimezoneOffset = Convert.ToDouble(r[4]),
                            TimezoneName = Convert.ToString(r[5])
                        })
                        .SingleOrDefault(i => ipformatted.CompareTo(i.IPEnd) <= 0) ??
                        IPGeolocationInfo.Default;
                }
            }
            catch (Exception error)
            {
                log.Error(error);
            }
            return IPGeolocationInfo.Default;
        }

        public IPGeolocationInfo GetIPGeolocationFromHttpContext()
        {
            return GetIPGeolocationFromHttpContext(HttpContext.Current);
        }

        public IPGeolocationInfo GetIPGeolocationFromHttpContext(HttpContext context)
        {
            if (context != null && context.Request != null)
            {
                var ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    return GetIPGeolocation(ip);
                }
            }
            return IPGeolocationInfo.Default;
        }

        private static string FormatIP(string ip)
        {
            if (ip.Contains('.'))
            {
                //ip v4
                if (ip.Length == 15)
                {
                    return ip;
                }
                return string.Join(".", ip.Split('.').Select(s => ("00" + s).Substring(s.Length - 1)).ToArray());
            }
            else if (ip.Contains(':'))
            {
                //ip v6
                if (ip.Length == 39)
                {
                    return ip;
                }
                var index = ip.IndexOf("::");
                if (0 <= index)
                {
                    ip = ip.Insert(index + 2, new String(':', 8 - ip.Split(':').Length));
                }
                return string.Join(":", ip.Split(':').Select(s => ("0000" + s).Substring(s.Length)).ToArray());
            }
            else
            {
                throw new ArgumentException("Unknown ip " + ip);
            }
        }
    }
}