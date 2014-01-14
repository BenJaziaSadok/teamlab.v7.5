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
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class Milestones
    {
        [Test]
        public void Test()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);


            IDaoFactory daoFactory = new DaoFactory("projects", 0);


            var result = daoFactory.GetReportDao().BuildMilestonesReport(new ReportFilter());


            Console.WriteLine(result.Count);

        }

        [Test]
        public void MilestoneDeadline()
        {
            ASC.Core.CoreContext.TenantManager.SetCurrentTenant(0);

            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            var milestones = daoFactory.GetMilestoneDao().GetByDeadLine(DateTime.Now);


            Console.WriteLine(milestones.Count);

        }
    }
}
