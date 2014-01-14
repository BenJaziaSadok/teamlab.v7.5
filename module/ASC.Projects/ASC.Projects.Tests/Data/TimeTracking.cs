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
using ASC.Core;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

#endregion

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class TimeTracking
    {

        private static readonly ILog _logger =
LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void BuildUsersWorkReport()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);
            
            Project project = daoFactory.GetProjectDao().GetById(5);

            Console.WriteLine(daoFactory.GetTimeSpendDao().GetByProject(project.ID).Count);
            
        }

        [Test]
        public  void  SaveOrUpdateTimeSpend()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);
            
            Project project = daoFactory.GetProjectDao().GetById(5);
 
            var timeSpend = new TimeSpend
                                {
                                    Date = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                                    Hours = 10, 
                                    Note = "asdfasdf", 
                                    Person = SecurityContext.CurrentAccount.ID,
                                    Project = project.ID,
                                };

            daoFactory.GetTimeSpendDao().Save(timeSpend);
        }

        [Test]
        public void HasTime()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);
        }
    }
}
