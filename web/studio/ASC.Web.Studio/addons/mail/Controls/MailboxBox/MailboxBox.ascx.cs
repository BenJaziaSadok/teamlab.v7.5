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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Mail.Controls
{
  public partial class MailboxBox : System.Web.UI.UserControl
  {
    public static string Location { get { return "~/addons/mail/Controls/MailboxBox/MailboxBox.ascx"; } }

    public string Content { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
      Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "mailaccounts_script", WebPath.GetPath("addons/mail/js/mail.accounts.js"));
    }
  }
}