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

#region Import

using System;
using System.Collections.Generic;
using System.Reflection;
using ASC.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

#endregion

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class Projects : TestBase
    {

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void SaveOrUpdateTest()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Project project = daoFactory.GetProjectDao().GetById(10);

            Console.WriteLine(daoFactory.GetProjectDao().GetTeam(project.ID).Length);

            daoFactory.GetProjectDao().AddToTeam(project.ID, new Guid("777fc1c2-b444-4303-9d71-f8766796e4b4"));

            Console.WriteLine(daoFactory.GetProjectDao().GetTeam(project.ID).Length);

        }

        /// <summary>
        ///    project_id, project_title, project_leader, project_status,milestone_count, task_count, participian_count
        /// </summary>
        /// <param name="projectStatus"></param>
        /// <returns></returns>
        public void BuildProjectListReport()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            ///  IList queryResult = daoFactory.GetProjectDao().BuildProjectListReport(null);

            // Console.WriteLine(queryResult.Count);


        }

        public void BuildProjectWithoutOpenMilestone()
        {


        }

        [Test]
        public void BuildProjectWithoutOpenTask()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Console.WriteLine(daoFactory.GetReportDao().BuildProjectWithoutOpenMilestone(new ReportFilter()).Count);

        }

        [Test]
        public void SaveOrUpdateTest123()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Project newProject = new Project();

            newProject.Title = "Test project 2";
            newProject.Description = "Description";
            newProject.Responsible = SecurityContext.CurrentAccount.ID;

            daoFactory.GetProjectDao().Save(newProject);
            daoFactory.GetProjectDao().AddToTeam(newProject.ID, SecurityContext.CurrentAccount.ID);
            Console.WriteLine(newProject.ID);

        }

        [Test]
        public void LoadProject()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Console.WriteLine(daoFactory.GetProjectDao().GetTeam(15).Length);


        }

        [Test]
        public void GetTaskCount()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Console.WriteLine(daoFactory.GetProjectDao().GetTaskCount(new List<int>(new[] { 1 }), TaskStatus.Open, TaskStatus.NotAccept, TaskStatus.Closed));
        }

        [Test]
        public void AddProjectTags()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Project project = daoFactory.GetProjectDao().GetById(11);

            var tags = daoFactory.GetTagDao().GetProjectTags(project.ID);

            Console.WriteLine(tags.Length);
        }


        [Test]
        public void GetProjectTags()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Project project = daoFactory.GetProjectDao().GetById(8);

            var tags = daoFactory.GetTagDao().GetProjectTags(project.ID);
            Console.WriteLine(tags.Length);
        }
    }
}
