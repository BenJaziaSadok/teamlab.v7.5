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
using System.Web.UI;
using System.Web;

namespace ASC.Web.Files.Import
{
    public class OAuthBase : Page
    {
        private const string CallbackSuccessJavascript =
            "function snd(){{try{{window.opener.OAuthCallback(\"{0}\",\"{1}\");}}catch(err){{alert(err);}}window.close();}} window.onload = snd;";

        private const string CallbackErrorJavascript =
            "function snd(){{try{{window.opener.OAuthError(\"{0}\",\"{1}\");}}catch(err){{alert(err);}}window.close();}} window.onload = snd;";

        protected override void OnInit(EventArgs e)
        {
            HttpContext.Current.PushRewritenUri();
            base.OnInit(e);
        }

        protected void SubmitToken(string accessToken, string source)
        {
            ClientScript.RegisterClientScriptBlock(GetType(), "posttoparent",
                                                   String.Format(CallbackSuccessJavascript, accessToken, source), true);
        }

        protected void SubmitError(string error, string source)
        {
            ClientScript.RegisterClientScriptBlock(GetType(), "posterrortoparent",
                                                   String.Format(CallbackErrorJavascript, error, source), true);
        }
    }
}