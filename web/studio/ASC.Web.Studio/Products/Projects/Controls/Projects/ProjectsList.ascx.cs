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

#region Usings

using System;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Controls.Common;

#endregion

namespace ASC.Web.Projects.Controls.Projects
{
    public partial class ProjectsList : BaseUserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopupTasks.Options.IsPopup = true;
            _hintPopupMilestones.Options.IsPopup = true;           
        }
    }
}