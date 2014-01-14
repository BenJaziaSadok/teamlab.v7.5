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
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Common.Support
{
    public partial class Support : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/Support/Support.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected bool MakeUserVoice
        {
            get { return !string.IsNullOrEmpty(SetupInfo.UserVoiceURL); }
        }
    }
}