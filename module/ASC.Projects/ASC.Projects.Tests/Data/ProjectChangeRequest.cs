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

using ASC.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

#endregion

namespace ASC.Projects.Tests.Data
{
    public class ProjectChangeRequestTest : TestBase
    {

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void AcceptProjectChangeRequest()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            IProjectDao _projectDao = daoFactory.GetProjectDao();
            IProjectChangeRequestDao _projectChangeRequestDao = daoFactory.GetProjectChangeRequestDao();

            ProjectChangeRequest projectChangeRequest = daoFactory.GetProjectChangeRequestDao().GetById(48);

            Project project = projectChangeRequest.RequestType == ProjectRequestType.Edit ? _projectDao.GetById(projectChangeRequest.ProjectID) : new Project();

            project.Title = projectChangeRequest.Title;
            project.Description = projectChangeRequest.Description;
            project.Responsible = projectChangeRequest.Responsible;
            project.Status = projectChangeRequest.Status;


            project = _projectDao.Save(project);
            daoFactory.GetProjectDao().AddToTeam(project.ID, project.Responsible);
            _projectChangeRequestDao.Delete(projectChangeRequest.ID);
        }

        [Test]
        public void SaveOrUpdateTest()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            ProjectChangeRequest projectChangeRequest = new ProjectChangeRequest();

            projectChangeRequest.ProjectID = 10;
            projectChangeRequest.RequestType = ProjectRequestType.Create;
            projectChangeRequest.Status = ProjectStatus.Open;
            projectChangeRequest.Description = "asdf";
            projectChangeRequest.Title = "New Project 123";
            projectChangeRequest.Responsible = SecurityContext.CurrentAccount.ID;
            //  projectChangeRequest.CreateBy = new Participant(ASC.Core.SecurityContext.CurrentAccount.ID);

            daoFactory.GetProjectChangeRequestDao().Save(projectChangeRequest);
        }
    }
}
