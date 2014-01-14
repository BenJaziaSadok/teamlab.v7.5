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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Specific;
using ASC.Web.Core.Calendars;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Api.Projects.Calendars
{
    public class ProjectCalendar : BaseCalendar
    {
        private class Event : BaseEvent{}

        private Project _project;        
        private EngineFactory _engine;
        private Guid _userId;
        private bool _following;

        public ProjectCalendar(EngineFactory engine, Guid userId, Project project, string backgroundColor,  string textColor, SharingOptions sharingOptions, bool following)
        {
            _project = project;
            _engine = engine;
            _userId = userId;
            _following = following;

            this.Context.HtmlBackgroundColor = backgroundColor;
            this.Context.HtmlTextColor = textColor;
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = () => ASC.Web.Projects.Resources.ProjectsCommonResource.ProductName;
            this.Id = _project.UniqID;
            this.EventAlertType = EventAlertType.Hour;
            this.Name = _project.Title;
            this.Description = _project.Description;
            this.SharingOptions = sharingOptions;            
        }
      
        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var events = new List<IEvent>();

            List<Task> tasks = new List<Task>();
            if (!_following)
                tasks = _engine.GetTaskEngine().GetByProject(_project.ID, TaskStatus.Open, userId);

            var milestones = _engine.GetMilestoneEngine().GetByStatus(_project.ID, MilestoneStatus.Open);

            foreach(var m in milestones)
            {
                events.Add(new Event()
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = this.Id,                    
                    UtcStartDate = m.DeadLine,
                    UtcEndDate = m.DeadLine,
                    AllDayLong = true,
                    Id = m.UniqID,
                    Name = ASC.Web.Projects.Resources.MilestoneResource.Milestone + ": " + m.Title,
                    Description = m.Description
                });
            }

            foreach (var t in tasks)
            {
                var start = t.StartDate;

                if (!t.Deadline.Equals(DateTime.MinValue))
                {
                    start = start.Equals(DateTime.MinValue) ? t.Deadline : t.StartDate;
                }
                else
                {
                    start = DateTime.MinValue;
                }

                events.Add(new Event()
                {
                    AlertType = EventAlertType.Never,
                    CalendarId = this.Id,
                    UtcStartDate = start,
                    UtcEndDate = t.Deadline,
                    AllDayLong = true,
                    Id = t.UniqID,
                    Name = ASC.Web.Projects.Resources.TaskResource.Task + ": " + t.Title,
                    Description = t.Description
                });
            }

            return events;
        }       

        public override TimeZoneInfo TimeZone
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TimeZone; }
        }
    }
}
