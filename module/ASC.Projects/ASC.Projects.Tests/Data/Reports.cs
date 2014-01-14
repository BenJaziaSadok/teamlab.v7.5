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
using System.Reflection;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Reports : TestBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void BuildMilestoneListReport()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var result = daoFactory.GetReportDao().BuildMilestonesReport(new ReportFilter());
            Console.WriteLine(result.Count);
        }
    }
}
