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
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.UserControls.Common.TabsNavigator;

namespace ASC.Web.Projects.Classes
{
    public class ModuleManager
    {
        public class ProjectCommonModule : Module
        {
            public override Guid ProjectId { get { return ProductEntryPoint.ID; } }

            public virtual string GetCount() { return ""; }

            protected readonly Project CurrentProject;
            protected readonly TaskFilter Filter;

            protected ProjectCommonModule(Project currentProject)
            {
                CurrentProject = currentProject;

                Filter = new TaskFilter
                {
                    ProjectIds = new List<int> { CurrentProject.ID },
                    MilestoneStatuses = new List<MilestoneStatus> { MilestoneStatus.Open },
                    TaskStatuses = new List<TaskStatus> { TaskStatus.Open }
                };
            }

            public TabsNavigatorItem ConvertToNavigatorItem(string currentPage)
            {
                return new TabsNavigatorItem
                    {
                        TabName = string.Format("{0}<span id='{2}Module' class='count'>{1}</span>", Name, GetCount(), ModuleSysName.ToLower()),
                        IsSelected = currentPage.ToLower() == ModuleSysName.ToLower(),
                        TabHref = string.Format(StartURL, CurrentProject.ID)
                    };
            }
        }

        private sealed class MilestonesModule : ProjectCommonModule
        {
            public MilestonesModule(Project currentProject) 
                : base(currentProject)
            {
                ID = ProductEntryPoint.MilestoneModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "milestones.aspx?prjID={0}");
                ModuleSysName = "Milestones";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 2;
                Name = MilestoneResource.Milestones;
            }

            public override string GetCount()
            {
                var count = Global.EngineFactory.GetMilestoneEngine().GetByFilterCount(Filter);
                return count != 0 ? string.Format(" ({0})", count) : "";
            }
        }

        private sealed class TasksModule : ProjectCommonModule
        {
            public TasksModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.TasksModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "tasks.aspx?prjID={0}");
                ModuleSysName = "Tasks";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 1;
                Name = TaskResource.Tasks;
            }

            public override string GetCount()
            {
                var count = Global.EngineFactory.GetTaskEngine().GetByFilterCount(Filter);
                return count != 0 ? string.Format(" ({0})", count) : "";
            }
        }

/*        private sealed class GanttChartModule : ProjectCommonModule
        {
            public GanttChartModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.GanttChartModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "ganttchart.aspx?prjID={0}");
                ModuleSysName = "GanttChart";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 3;
                Name = ProjectResource.GanttGart;
            }
        }*/

        private sealed class MessagesModule : ProjectCommonModule
        {
            public MessagesModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.MessagesModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "messages.aspx?prjID={0}");
                ModuleSysName = "Messages";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 4;
                Name = MessageResource.Messages;
            }

            public override string GetCount()
            {
                var count = Global.EngineFactory.GetMessageEngine().GetByFilterCount(Filter);
                return count != 0 ? string.Format(" ({0})", count) : "";
            }
        }

        private sealed class DocumentsModule : ProjectCommonModule
        {
            public DocumentsModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.DocumentsModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "tmdocs.aspx?prjID={0}");
                ModuleSysName = "TMDocs";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 6;
                Name = ProjectsFileResource.Documents;
            }

            public override string GetCount()
            {
                var folderId = Global.EngineFactory.GetFileEngine().GetRoot(CurrentProject.ID);
                using (var folderDao = FilesIntegration.GetFolderDao())
                {
                    var count = folderDao.GetItemsCount(folderId, true);
                    return count != 0 ? string.Format(" ({0})", count) : "";
                }
            }
        }

        private sealed class TimeTrackingModule : ProjectCommonModule
        {
            public TimeTrackingModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.TimeTrackingModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "timetracking.aspx?prjID={0}");
                ModuleSysName = "TimeTracking";
                Context.DefaultSortOrder = 5;
                Name = ProjectsCommonResource.TimeTracking;
            }

            public override string GetCount()
            {
                var time = Global.EngineFactory.GetTimeTrackingEngine().GetByProject(CurrentProject.ID).Sum(r => r.Hours);
                var hours = (int)time;
                var minutes = (int)(Math.Round((time - hours) * 60));
                var result = hours + ":" + minutes.ToString("D2");
                return !result.Equals("0:00", StringComparison.InvariantCulture) ? string.Format(" ({0})", result) : "";
            }
        }

        private sealed class ProjectTeamModule : ProjectCommonModule
        {
            public ProjectTeamModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.ProjectTeamModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "projectTeam.aspx?prjID={0}");
                ModuleSysName = "ProjectTeam";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 8;
                Name = ProjectResource.ProjectTeam;
            }

            public override string GetCount()
            {
                var count = Global.EngineFactory.GetProjectEngine().GetTeam(CurrentProject.ID).Count;
                return count != 0 ? string.Format(" ({0})", count) : "";
            }
        }

        private sealed class ContactsModule : ProjectCommonModule
        {
            public ContactsModule(Project currentProject)
                : base(currentProject)
            {
                ID = ProductEntryPoint.ContactsModuleID;
                StartURL = String.Concat(PathProvider.BaseAbsolutePath, "contacts.aspx?prjID={0}");
                ModuleSysName = "Contacts";
                DisplayedAlways = true;
                Context.DefaultSortOrder = 7;
                Name = ProjectsCommonResource.ModuleContacts;
            }
        }

        public static List<ProjectCommonModule> GetModules(Project project)
        {
            var modules = new List<ProjectCommonModule>
                          {
                              new MilestonesModule(project),
                              new TasksModule(project),
                              new ProjectTeamModule(project)
                          };

            if (ProjectSecurity.CanReadFiles(project))
                modules.Add(new DocumentsModule(project));

            if (ProjectSecurity.CanReadMessages(project))
                modules.Add(new MessagesModule(project));

            if (ProjectSecurity.CanCreateTimeSpend(project))
                modules.Add(new TimeTrackingModule(project));


            var crmEnabled = WebItemManager.Instance[new Guid("6743007C-6F95-4d20-8C88-A8601CE5E76D")];

            if (crmEnabled != null && !crmEnabled.IsDisabled() && ProjectSecurity.CanReadContacts(project))
            {
                modules.Add(new ContactsModule(project));
            }

            return modules.OrderBy(r => r.Context.DefaultSortOrder).ToList();
        }
    }
}
