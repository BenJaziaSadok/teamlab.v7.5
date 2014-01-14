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
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class Report
    {
        [Test]
        public void ReportFilterTest()
        {
            var filter = new ReportFilter();
            filter.DepartmentId = Guid.NewGuid();
            filter.FromDate = DateTime.Now.Date;
            filter.ToDate = DateTime.Now.Date;
            filter.MilestoneStatuses.Add(MilestoneStatus.Open);
            filter.MilestoneStatuses.Add(MilestoneStatus.Closed);
            filter.ProjectIds.Add(1);
            filter.ProjectIds.Add(3);
            filter.ProjectStatuses.Add(ProjectStatus.Open);
            filter.ProjectStatuses.Add(ProjectStatus.Closed);
            filter.ProjectTag = "tag";
            filter.TaskStatuses.Add(TaskStatus.Open);
            filter.TaskStatuses.Add(TaskStatus.Closed);
            filter.TimeInterval = ReportTimeInterval.Absolute;
            filter.UserId = Guid.NewGuid();
            filter.ViewType = 4;

            var xml = filter.ToXml();
            var uri = filter.ToUri();
            AreEquals(filter, ReportFilter.FromXml(xml));
            AreEquals(filter, ReportFilter.FromUri(uri));

            filter.TimeInterval = ReportTimeInterval.CurrMonth;
            xml = filter.ToXml();
            uri = filter.ToUri();
            AreEquals(filter, ReportFilter.FromXml(xml));
            AreEquals(filter, ReportFilter.FromUri(uri));
        }

        private void AreEquals(ReportFilter f1, ReportFilter f2)
        {
            Assert.AreEqual(f1.TimeInterval, f2.TimeInterval);
            if (f1.TimeInterval == ReportTimeInterval.Absolute)
            {
                Assert.AreEqual(f1.FromDate, f2.FromDate);
                Assert.AreEqual(f1.ToDate, f2.ToDate);
            }
            Assert.AreEqual(f1.DepartmentId, f2.DepartmentId);
            Assert.AreEqual(f1.UserId, f2.UserId);
            Assert.AreEqual(f1.ViewType, f2.ViewType);
            Assert.AreEqual(f1.ProjectTag, f2.ProjectTag);
            CollectionAssert.AreEquivalent(f1.MilestoneStatuses, f2.MilestoneStatuses);
            CollectionAssert.AreEquivalent(f1.ProjectIds, f2.ProjectIds);
            CollectionAssert.AreEquivalent(f1.ProjectStatuses, f2.ProjectStatuses);
            CollectionAssert.AreEquivalent(f1.TaskStatuses, f2.TaskStatuses);
        }

        [Test]
        public void TestDates()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);
            var filter = new ReportFilter();
            filter.TimeInterval = ReportTimeInterval.NextWeek;
            var d = filter.GetFromDate();
            d = filter.GetToDate();
        }
    }
}
