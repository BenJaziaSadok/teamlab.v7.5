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
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Mail.Autoreply.ParameterResolvers;

namespace ASC.Mail.Autoreply
{
    internal class ApiRequest
    {
        public string Method { get; set; }

        public string Url { get; set; }

        public List<RequestParameter> Parameters { get; set; } 

        public Tenant Tenant { get; set; }

        public UserInfo User { get; set; }

        public List<RequestFileInfo> FilesToPost { get; set; }

        public ApiRequest(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Url = url.Trim('/');
            }
        }

        public override string ToString()
        {
            return string.Format("t:{0}; u:{1}; {2} {3}", Tenant.TenantId, User.ID, Method, Url);
        }
    }

    internal class RequestParameter
    {
        public string Name { get; private set; }
        public object Value { get; set; }
        public IParameterResolver ValueResolver { get; private set; }

        public RequestParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public RequestParameter(string name, IParameterResolver valueResolver)
        {
            Name = name;
            ValueResolver = valueResolver;
        }
    }

    internal class RequestFileInfo
    {
        public byte[] Body { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}