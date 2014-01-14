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

using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Data.Storage;
using log4net;
using System;
using System.Web.UI;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("PromoCodeController")]
    public partial class PromoCode : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/PromoCode/PromoCode.ascx"; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/PromoCode/promocode.js"));
        }


        [AjaxMethod]
        public object ActivateKey(string promocode)
        {
            if (!string.IsNullOrEmpty(promocode))
            {
                try
                {
                    CoreContext.PaymentManager.ActivateKey(promocode);
                    return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };
                }
                catch (Exception err)
                {
                    LogManager.GetLogger("ASC.Web.FirstTime").Error(err);
                }
            }
            return new { Status = 0, Message = Resources.Resource.EmailAndPasswordIncorrectPromocode };
        }
    }
}