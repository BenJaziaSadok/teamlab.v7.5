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
using ASC.Projects.Core.Domain;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Controls.Common;

#endregion

namespace ASC.Web.Projects.Controls.Tasks
{
    public partial class TaskList : BaseUserControl
    {
        protected int AllTasksCount { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopup.Options.IsPopup = true;
            _hintPopupTaskRemove.Options.IsPopup = true;
            _hintPopupTaskRemind.Options.IsPopup = true;
            moveTaskContainer.Options.IsPopup = true;

            var filter = new TaskFilter();

            if (RequestContext.IsInConcreteProject)
                filter.ProjectIds.Add(RequestContext.GetCurrentProjectId());

            AllTasksCount = Global.EngineFactory.GetTaskEngine().GetByFilterCount(filter);
        }
    }
}
