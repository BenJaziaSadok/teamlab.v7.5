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

using ASC.Web.Studio.UserControls.Common.HelpCenter;

namespace ASC.Web.Projects
{
    public partial class Help : BasePage
    {
        protected override void PageLoad()
        {
            HelpHolder.Controls.Add(LoadControl(HelpCenter.Location));
        }
    }
}
