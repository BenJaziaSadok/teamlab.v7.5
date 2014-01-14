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

using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;

namespace ASC.Api.Batch
{
    public class ApiBatchRequest
    {
        public ApiBatchRequest()
        {
            //Defaults
            BodyContentType = new ContentType("application/x-www-form-urlencoded"){CharSet = Encoding.UTF8.WebName}.ToString();
            Method = "GET";
        }

        public int Order { get; set; }

        public string[] After { get; set; }
        public string[] Before { get; set; }

        public string RelativeUrl { get; set; }

        public string Name { get; set; }

        private string _method;
        public string Method
        {
            get { return string.IsNullOrEmpty(_method)?"GET":_method; }
            set { _method = value; }
        }

        public string Body { get; set; }

        public string BodyContentType { get; set; }
    }
}