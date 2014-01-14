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
using AjaxPro;
using ASC.Thrdparty.Configuration;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    [AjaxNamespace("AjaxPro.ContactsSearchView")]
    public partial class ContactsSearchView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(this.GetType());
            _ctrlContactsSearchContainer.Options.IsPopup = true;
        }

        [AjaxMethod]
        public string FindContactByName(string searchUrl, string contactNamespace)
        {
            var crunchBaseKey = KeyStorage.Get("crunchBaseKey");
            if (!string.IsNullOrEmpty(crunchBaseKey))
            {
                crunchBaseKey = string.Format("api_key={0}", crunchBaseKey);
                searchUrl += "&" + crunchBaseKey;
            }

            var findGet = System.Net.WebRequest.Create(searchUrl);
            var findResp = findGet.GetResponse();

            if (findResp != null)
            {
                var findStream = findResp.GetResponseStream();
                if (findStream != null)
                {
                    var sr = new System.IO.StreamReader(findStream);
                    var s = sr.ReadToEnd();
                    var permalink = Newtonsoft.Json.Linq.JObject.Parse(s)["permalink"].ToString().HtmlEncode();

                    searchUrl = @"http://api.crunchbase.com/v/1/" + contactNamespace + "/" + permalink + ".js";
                    if (!string.IsNullOrEmpty(crunchBaseKey))
                    {
                        searchUrl += "?" + crunchBaseKey;
                    }

                    var infoGet = System.Net.WebRequest.Create(searchUrl);
                    var infoResp = infoGet.GetResponse();

                    if (infoResp != null)
                    {
                        var infoStream = infoResp.GetResponseStream();
                        if (infoStream != null)
                        {
                            sr = new System.IO.StreamReader(infoStream);
                            s = sr.ReadToEnd();
                            return s;
                        }
                    }
                    s = sr.ReadToEnd();
                    
                    return s;
                }
            }
            return string.Empty;
        }

    }
}