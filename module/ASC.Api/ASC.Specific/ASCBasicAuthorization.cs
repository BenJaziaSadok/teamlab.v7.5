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
using System.Net;
using System.Text;
using System.Web;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using ASC.Core;


namespace ASC.Specific
{
    public class AscBasicTestAuthorization : AscBasicAuthorization
    {
        protected override void Authentificate(string username, string password)
        {
            //set tennant to 0 for testing
            CoreContext.TenantManager.SetCurrentTenant(0);
            base.Authentificate(username, password);
        }
    }

    public class AscBasicAuthorization : IApiAuthorization
    {
        private readonly ILog _log;


        public AscBasicAuthorization()
        {

        }

        public AscBasicAuthorization(ILog log)
        {
            _log = log;
        }

        
        public bool Authorize(HttpContextBase context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                try
                {
                    //Try basic
                    var authorization = context.Request.Headers["Authorization"];
                    if (string.IsNullOrEmpty(authorization))
                    {
                        return false;
                    }
                    authorization = authorization.Trim();
                    if (authorization.IndexOf(',') != -1)
                        authorization = authorization.Substring(0, authorization.IndexOf(',')).Trim();

                    if (authorization.IndexOf("Basic", 0) != 0)
                    {
                        return false;
                    }

                    // cut the word "basic" and decode from base64
                    // get "username:password"
                    var tempConverted = Convert.FromBase64String(authorization.Substring(6));
                    var user = new ASCIIEncoding().GetString(tempConverted);

                    // get "username"
                    // get "password"
                    var usernamePassword = user.Split(new[] { ':' });
                    var username = usernamePassword[0];
                    var password = usernamePassword[1];
                    _log.Debug("Basic Authorizing {0}", username);
                    Authentificate(username, password);
                }
                catch (Exception) { }
            }
            return SecurityContext.IsAuthenticated;
        }

        protected virtual void Authentificate(string username, string password)
        {
            var u = CoreContext.UserManager.GetUserByEmail(username) ?? CoreContext.UserManager.GetUserByUserName(username) ?? CoreContext.UserManager.GetUsers(new Guid(username));
            if (!ASC.Core.Users.Constants.LostUser.Equals(u))
            {
                SecurityContext.AuthenticateMe(u.ID.ToString(), password);
            }
        }

        public bool OnAuthorizationFailed(HttpContextBase context)
        {
            if (!string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.StatusDescription = HttpStatusCode.Unauthorized.ToString();
                string realm = String.Format("Basic Realm=\"{0}\"", context.Request.GetUrlRewriter().Host);
                context.Response.AppendHeader("WWW-Authenticate", realm);
                return true;
            }
            return false;
        }
    }
}