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
using System.Reflection;
using System.Web.Routing;
using ASC.Api.Attributes;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiMethodCall : IEquatable<IApiMethodCall>
    {
        string RoutingUrl { get; set; }
        Type ApiClassType { get; set; }
        MethodInfo MethodCall { get; set; }
        string HttpMethod { get; set; }
        string Name { get; set; }
        long CacheTime { get; set; }
        bool ShouldCache { get; }
        void SetParams(ParameterInfo[] value);
        ParameterInfo[] GetParams();
        string FullPath { get; set; }
        RouteValueDictionary Constraints { get; set; }
        bool RequiresAuthorization { get; set; }
        bool SupportsPoll { get; set; }
        string RoutingPollUrl { get; set; }
        object Invoke(object instance, object[] args);
        IEnumerable<ApiCallFilter> Filters { get; set; }
        ICollection<IApiResponder> Responders { get; set; } 
    }
}