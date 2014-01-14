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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Studio.Core.Users;
using AjaxPro;
using System.IO;
using ASC.Web.Studio.Utility;



namespace ASC.Web.Studio.UserControls.Common.Banner
{
    [AjaxNamespace("AjaxPro.BannerController")]
    public partial class Banner : UserControl
    {
        public static string Location { get { return "~/usercontrols/common/banner/banner.ascx"; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/banner/js/banner.js"));
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
        }       
       
        protected string GetImgUrlBanner()
        {
            string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            string catalog = HttpContext.Current.Server.MapPath("~/skins/default/images/banner").ToLowerInvariant();
            string img = HttpContext.Current.Server.MapPath("~/skins/default/images/banner/banner_portal_" + language + ".png").ToLowerInvariant();
            string url = "";
            if (!string.IsNullOrEmpty(catalog) && !string.IsNullOrEmpty(img) )
            {
                if (!Directory.GetFiles(catalog).Contains(img))
                {
                    language = "en";
                }
                url = VirtualPathUtility.ToAbsolute("~/skins/default/images/banner/banner_portal_" + language + ".png");
            }            

            return url;
        }


        #region AjaxMethod
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse JoinToAffiliateProgram()
        {
            var resp = new AjaxResponse();
            try
            {
                resp.rs1 = "1";
                resp.rs2 = AffiliateHelper.Join();
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }
            return resp;

        }
        #endregion

    }
}