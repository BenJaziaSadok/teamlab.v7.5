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
using ASC.Projects.Data;
using ASC.Projects.Engine;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    [TestFixture]
    public class TemplateTests : TestBase
    {
        private TemplateEngine engine = new TemplateEngine(new DaoFactory(DbId, 0), new EngineFactory(DbId, 0, null));

        public TemplateTests()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);
        }

        [Test]
        public void TemplateProjectTest()
        {
            var t = new TemplateProject("T1");
            t.Description = "desc";
            t.Team.Add(Guid.Empty);

            var t2 = engine.SaveTemplateProject(t);
            t2 = engine.GetTemplateProject(t2.Id);

            Assert.AreEqual(t.Title, t2.Title);
            Assert.AreEqual(t.Description, t2.Description);
            Assert.AreNotEqual(t.CreateBy, default(Guid));
            Assert.AreNotEqual(t.CreateOn, default(DateTime));
            Assert.AreEqual(t.Responsible, default(Guid));
            Assert.AreEqual(t.Tags, t2.Tags);
            CollectionAssert.AreEquivalent(t2.Team, new[] { Guid.Empty });

            foreach (var d in engine.GetTemplateProjects())
            {
                engine.RemoveTemplateProject(d.Id);
            }
        }

        [Test]
        public void TemplateMilestoneTest()
        {
            var t = new TemplateMilestone(1, "M1");
            t.DurationInDays = 4;
            t.IsKey = true;
            t.IsNotify = true;
            Assert.IsTrue(t.IsKey);
            Assert.IsTrue(t.IsNotify);
            t.IsKey = false;
            t.IsNotify = false;
            Assert.IsFalse(t.IsKey);
            Assert.IsFalse(t.IsNotify);
            t.IsKey = true;
            t.IsNotify = true;

            var t2 = engine.SaveTemplateMilestone(t);
            t2 = engine.GetTemplateMilestone(t2.Id);

            Assert.AreEqual(t.Title, t2.Title);
            Assert.AreEqual(t.DurationInDays, t2.DurationInDays);
            Assert.AreEqual(t.IsKey, t2.IsKey);
            Assert.AreEqual(t.IsNotify, t2.IsNotify);
            Assert.AreEqual(t.ProjectId, t2.ProjectId);
            Assert.AreNotEqual(t.CreateBy, default(Guid));
            Assert.AreNotEqual(t.CreateOn, default(DateTime));

            foreach (var d in engine.GetTemplateMilestones(1))
            {
                engine.RemoveTemplateMilestone(d.Id);
            }
        }

        [Test]
        public void TemplateMessageTest()
        {
            var t = new TemplateMessage(1, "M1");
            t.Text = "WWW";

            var t2 = engine.SaveTemplateMessage(t);
            t2 = engine.GetTemplateMessage(t2.Id);

            Assert.AreEqual(t.Title, t2.Title);
            Assert.AreEqual(t.ProjectId, t2.ProjectId);
            Assert.AreEqual(t.Text, t2.Text);
            Assert.AreNotEqual(t.CreateBy, default(Guid));
            Assert.AreNotEqual(t.CreateOn, default(DateTime));

            foreach (var d in engine.GetTemplateMessages(1))
            {
                engine.RemoveTemplateMessage(d.Id);
            }
        }

        [Test]
        public void TemplateTaskTest()
        {
            var t = new TemplateTask(1, "M1");
            t.Description = "d";
            t.MilestoneId = 4;
            t.Priority = TaskPriority.Low;
            t.SortOrder = 5;

            var t2 = engine.SaveTemplateTask(t);
            t2 = engine.GetTemplateTask(t2.Id);

            Assert.AreEqual(t.Title, t2.Title);
            Assert.AreEqual(t.ProjectId, t2.ProjectId);
            Assert.AreEqual(t.Description, t2.Description);
            Assert.AreEqual(t.MilestoneId, t2.MilestoneId);
            Assert.AreEqual(t.Priority, t2.Priority);
            Assert.AreEqual(t.Responsible, t2.Responsible);
            Assert.AreEqual(t.SortOrder, t2.SortOrder);
            Assert.AreNotEqual(t.CreateBy, default(Guid));
            Assert.AreNotEqual(t.CreateOn, default(DateTime));

            foreach (var d in engine.GetTemplateTasks(1))
            {
                engine.RemoveTemplateTask(d.Id);
            }
        }
    }
}
