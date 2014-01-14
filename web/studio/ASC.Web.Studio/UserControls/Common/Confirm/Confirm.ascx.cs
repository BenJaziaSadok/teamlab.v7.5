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
using System.Web.UI;
using ASC.Data.Storage;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class Confirm : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/usercontrols/common/confirm/confirm.ascx"; } }

        public Confirm()
        {
            AdditionalID = "";
        }

        public string Title { get; set; }
        public string Value { get; set; }
        public string SelectTitle { get; set; }
        public string AdditionalID { get; set; }
        public string SelectJSCallback { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _studioConfirm.Options.IsPopup = true;
            _confirmEnterCode.Value = String.Format("StudioConfirm.Select('{0}',{1});", AdditionalID, SelectJSCallback);

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/confirm/js/confirm.js"));
        }

    }
}