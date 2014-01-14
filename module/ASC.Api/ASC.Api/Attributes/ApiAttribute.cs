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

#region usings

using System;
using System.Collections.Generic;
using System.Web.Routing;

#endregion

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiAttribute : Attribute
    {


        public ApiAttribute(string httpMethod, string path, bool requiresAuthorization)
        {
            Method = httpMethod;
            Path = path;
            RequiresAuthorization = requiresAuthorization;
        }

        public string Method { get; set; }
        public string Path { get; set; }
        public bool RequiresAuthorization { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CreateAttribute : ApiAttribute
    {
        public CreateAttribute(string path, bool requiresAuthorization)
            : base("POST", path, requiresAuthorization)
        {
        }
        public CreateAttribute(string path) : base("POST", path,true)
        {
        }
    }



    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UpdateAttribute : ApiAttribute
    {
        public UpdateAttribute(string path, bool requiresAuthorization)
            : base("PUT", path, requiresAuthorization)
        {
        }
        public UpdateAttribute(string path) : base("PUT", path,true)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DeleteAttribute : ApiAttribute
    {
        public DeleteAttribute(string path, bool requiresAuthorization)
            : base("DELETE", path, requiresAuthorization)
        {
        }
        public DeleteAttribute(string path)
            : base("DELETE", path,true)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ReadAttribute : ApiAttribute
    {
        public ReadAttribute(string path, bool requiresAuthorization)
            : base("GET", path, requiresAuthorization)
        {
        }
        public ReadAttribute(string path) : base("GET", path,true)
        {
        }
    }
}