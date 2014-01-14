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
using System.Linq;

namespace ASC.Common.Web
{
    public class RouteCallInfo
    {
        public int? Tid { get; set; }
        public string Url { get; set; }
        public string AttachmentUrl { get; set; }
        public bool IsNewRequest { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public bool CleanupHtml { get; set; }

        public RouteCallInfo()
        {
            CleanupHtml = true;//Default
        }

        public override string ToString()
        {
            return string.Format("{0} {1} T:{2},{3}", Method.ToUpper(),Url,Tid,string.Join(",",Params.Select(x=>string.Format("{0}={1}",x.Key,x.Value)).ToArray()));
        }
    }
}