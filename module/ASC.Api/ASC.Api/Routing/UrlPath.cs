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
using System.Linq.Expressions;
using System.Reflection;
using ASC.Api.Attributes;
using ASC.Api.Interfaces;
using ASC.Common.Web;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Api.Routing
{
    public static class UrlPath
    {
        public static string ResolveUrl(Expression<Action> functionToCall)
        {
            return Resolve(functionToCall, (method, args) => ServiceLocator.Current.GetInstance<IApiRouteConfigurator>().ResolveRoute(method, args).Url);
        }

        public static RouteCallInfo ResolveRouteCall(Expression<Action> functionToCall)
        {
            return Resolve(functionToCall, (method, args) => ServiceLocator.Current.GetInstance<IApiRouteConfigurator>().ResolveRoute(method, args));
        }

        private static T Resolve<T>(Expression<Action> functionToCall, Func<MethodInfo, Dictionary<string, object>, T> resolver)
        {
            if (functionToCall.Body.NodeType == ExpressionType.Call)
            {
                var methodCall = (MethodCallExpression)functionToCall.Body;
                //Ensure parameter
                if (methodCall.Method.GetCustomAttributes(typeof(ApiAttribute), true).Cast<ApiAttribute>().Any())
                {
                    //It has apicall attr

                    //Build an argument list
                    var callArgs = methodCall.Arguments.Select(x => GetValue(x)).ToArray();
                    var arguments = new Dictionary<string, object>();
                    var methodParams = methodCall.Method.GetParameters();
                    for (var index = 0; index < methodParams.Length; index++)
                    {
                        var parameterInfo = methodParams[index];
                        if (index < callArgs.Length)
                        {
                            arguments.Add(parameterInfo.Name, callArgs[index]);
                        }
                    }
                    return resolver(methodCall.Method,arguments);
                }

            }
            return default(T);
        }

        private static object GetValue(Expression expression)
        {
            return GetValue(expression, null);
        }

        private static object GetValue(Expression expression, MemberExpression member)
        {
            //Resolving epressions to values
            if (expression is ConstantExpression)
            {
                if (member == null)
                    return ((ConstantExpression)expression).Value;

                //get by name
                return ((ConstantExpression)expression).Value.GetType().InvokeMember(member.Member.Name,
                                                                                      BindingFlags.Public |
                                                                                      BindingFlags.NonPublic |
                                                                                      BindingFlags.Instance | BindingFlags.GetField, null,
                                                                                      ((ConstantExpression)
                                                                                       expression).Value,
                                                                                      new object[] { });
            }
            if (expression is MemberExpression)
            {
                return GetValue(((MemberExpression)expression).Expression, (MemberExpression)expression);
            }
            if (expression is MethodCallExpression)
            {
                var methodCallExpr = ((MethodCallExpression)expression);
                return methodCallExpr.Method.Invoke(methodCallExpr.Object,
                                                    methodCallExpr.Arguments.Select(x => GetValue(x)).ToArray());
            }
            return null;
        }
    }
}