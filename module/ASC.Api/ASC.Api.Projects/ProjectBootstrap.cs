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
using System.Web.Configuration;
using ASC.Api.Interfaces;
using ASC.Common.Data;
using ASC.Files.Core;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Core.Calendars;

#endregion

namespace ASC.Api.Projects
{
    public class ProjectBootstrap : IApiBootstrapper
    {
        public void Configure()
        {
            if (!DbRegistry.IsDatabaseRegistered(ProjectApiBase.DbId))
            {
                DbRegistry.RegisterDatabase(ProjectApiBase.DbId, WebConfigurationManager.ConnectionStrings[ProjectApiBase.DbId]);
            }

            if (!DbRegistry.IsDatabaseRegistered(FileConstant.DatabaseId))
            {
                DbRegistry.RegisterDatabase(FileConstant.DatabaseId, WebConfigurationManager.ConnectionStrings[FileConstant.DatabaseId]);
            }

            FilesIntegration.RegisterFileSecurityProvider("projects", "project", new SecurityAdapterProvider());

            //Register prodjects' calendar events
            CalendarManager.Instance.RegistryCalendarProvider(ProjectApi.GetUserCalendars);

        }
    }
}
