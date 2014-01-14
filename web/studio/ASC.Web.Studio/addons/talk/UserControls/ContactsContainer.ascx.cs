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

namespace ASC.Web.Talk.UserControls
{
  public partial class ContactsContainer : System.Web.UI.UserControl
  {
      protected void Page_Load(object sender, EventArgs e)
      {
          Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.contactscontainer.js"));
          Page.RegisterInlineScript("ASC.TMTalk.contactsContainer.init();");
      }
  }
}