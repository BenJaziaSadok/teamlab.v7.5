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
using System.Web.Routing;
using ASC.Api.Impl;
using ASC.Api.Interfaces;

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class|AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public abstract class ApiCallFilter:Attribute
    {
        public virtual void PreMethodCall(IApiMethodCall method,ApiContext context, IEnumerable<object> arguments){}

        public virtual void PostMethodCall(IApiMethodCall method, ApiContext context, object methodResponce){}

        public virtual void ErrorMethodCall(IApiMethodCall method, ApiContext context, Exception e) { }
    }
}