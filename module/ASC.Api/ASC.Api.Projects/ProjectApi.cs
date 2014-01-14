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
using System.Web.Configuration;

using ASC.Api.Documents;
using ASC.Api.Impl;
using ASC.Api.Projects.Calendars;
using ASC.Web.Core.Calendars;
using ASC.Common.Data;
using ASC.Projects.Engine;
using ASC.Core;

namespace ASC.Api.Projects
{
    ///<summary>
    /// Projects access
    ///</summary>
    public partial class ProjectApi : ProjectApiBase, Interfaces.IApiEntryPoint
    {
        private readonly DocumentsApi documentsApi;

        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "project"; }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="context"></param>
        ///<param name="documentsApi">Docs api</param>
        public ProjectApi(ApiContext context, DocumentsApi documentsApi)
        {
            this.documentsApi = documentsApi;
            _context = context;
        }

        internal static List<BaseCalendar> GetUserCalendars(Guid userId)
        {
            if (!DbRegistry.IsDatabaseRegistered(DbId))
                DbRegistry.RegisterDatabase(DbId, WebConfigurationManager.ConnectionStrings[DbId]);

            var tenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var engineFactory = new EngineFactory(DbId, tenantId);

            var cals = new List<BaseCalendar>();
            var engine = engineFactory.GetProjectEngine();
            var projects = engine.GetByParticipant(userId);

            if (projects != null)
            {
                var team = engine.GetTeam(projects.Select(r => r.ID).ToList());

                foreach (var p in projects)
                {
                    var sharingOptions = new SharingOptions();
                    foreach (var participant in team.Where(r => r.ProjectID == p.ID))
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem { Id = participant.ID, IsGroup = false });

                    var index = p.ID % CalendarColors.BaseColors.Count;
                    cals.Add(new ProjectCalendar(engineFactory, userId, p, CalendarColors.BaseColors[index].BackgroudColor, CalendarColors.BaseColors[index].TextColor, sharingOptions, false));
                }
            }

            var folowingProjects = engine.GetFollowing(userId);
            if (folowingProjects != null)
            {
                var team = engine.GetTeam(folowingProjects.Select(r => r.ID).ToList());

                foreach (var p in folowingProjects)
                {
                    if (projects != null && projects.Exists(proj => proj.ID == p.ID))
                        continue;

                    var sharingOptions = new SharingOptions();
                    sharingOptions.PublicItems.Add(new SharingOptions.PublicItem { Id = userId, IsGroup = false });

                    foreach (var participant in team.Where(r => r.ProjectID == p.ID))
                        sharingOptions.PublicItems.Add(new SharingOptions.PublicItem { Id = participant.ID, IsGroup = false });

                    var index = p.ID % CalendarColors.BaseColors.Count;
                    cals.Add(new ProjectCalendar(engineFactory, userId, p, CalendarColors.BaseColors[index].BackgroudColor, CalendarColors.BaseColors[index].TextColor, sharingOptions, true));
                }
            }

            return cals;
        }
    }
}
