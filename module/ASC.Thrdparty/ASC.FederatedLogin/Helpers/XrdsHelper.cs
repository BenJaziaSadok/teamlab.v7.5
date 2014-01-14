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
using System.Web;

namespace ASC.FederatedLogin.Helpers
{
    public class XrdsHelper
    {
        private const string Xrds =
            @"<xrds:XRDS xmlns:xrds=""xri://$xrds"" xmlns:openid=""http://openid.net/xmlns/1.0"" " +
            @"xmlns=""xri://$xrd*($v*2.0)""><XRD><Service " +
            @"priority=""1""><Type>http://specs.openid.net/auth/2.0/return_to</Type><URI " +
            @"priority=""1"">{0}</URI></Service><Service><Type>http://specs.openid.net/extensions/ui/icon</Type><UR" +
            @"I>{1}</URI></Service></XRD></xrds:XRDS>";


        internal static void RenderXrds(HttpResponse responce, string location, string iconlink)
        {
            responce.Write(string.Format(Xrds,location,iconlink));
        }

        public static void AppendXrdsHeader()
        {
            AppendXrdsHeader("~/openidlogin.ashx");
        }

        public static void AppendXrdsHeader(string handlerVirtualPath)
        {
            HttpContext.Current.Response.AppendHeader(
                "X-XRDS-Location",
                new Uri(HttpContext.Current.Request.GetUrlRewriter(), HttpContext.Current.Response.ApplyAppPathModifier(handlerVirtualPath)).AbsoluteUri);
        }
    }
}