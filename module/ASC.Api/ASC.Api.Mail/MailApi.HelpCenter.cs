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

using ASC.Api.Attributes;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using System;

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        ///    Returns the string with html of help center page
        /// </summary>
        /// <returns>String with html of help center page</returns>
        /// <short>Get html of help center page</short> 
        /// <category>HelpCenter</category>
        [Read(@"helpcenter")]
        public string GetHelpCenterHtml()
        {
            return HelpCenter.RenderControlToString(new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"));
        }
    }
}
