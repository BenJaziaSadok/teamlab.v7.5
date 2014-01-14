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

#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Threading;
    using ASC.Core.Users;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    [TestClass]
    public class UserManagerTest
    {
        [TestMethod]
        public void SearchUsers()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);

            var users = CoreContext.UserManager.Search(null, EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search("", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search("  ", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search("АбРаМсКй", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search("АбРаМсКий", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search("АбРаМсКий", EmployeeStatus.All);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search("иванов николай", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search("ведущий програм", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search("баннов лев", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search("иванов николай", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreEqual(0, users);
        }

        [TestMethod]
        public void DepartmentManagers()
        {
            CoreContext.TenantManager.SetCurrentTenant(1024);

            var deps = CoreContext.UserManager.GetDepartments();
            var users = CoreContext.UserManager.GetUsers();

            var g1 = deps[0];
            var ceo = users[0];
            var u1 = users[1];
            var u2 = users[2];

            
            var ceoTemp = CoreContext.UserManager.GetCompanyCEO();
            CoreContext.UserManager.SetCompanyCEO(ceo.ID);
            ceoTemp = CoreContext.UserManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            Thread.Sleep(TimeSpan.FromSeconds(6));
            ceoTemp = CoreContext.UserManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            
            CoreContext.UserManager.SetDepartmentManager(g1.ID, u1.ID);

            CoreContext.UserManager.SetDepartmentManager(g1.ID, u2.ID);
        }

        [TestMethod]
        public void UserGroupsPerformanceTest()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);

            foreach (var u in CoreContext.UserManager.GetUsers())
            {
                var groups = CoreContext.GroupManager.GetGroups(Guid.Empty);
                foreach (var g in CoreContext.UserManager.GetUserGroups(u.ID))
                {
                    var manager = CoreContext.UserManager.GetUsers(CoreContext.UserManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            var stopwatch = Stopwatch.StartNew();
            foreach (var u in CoreContext.UserManager.GetUsers())
            {
                var groups = CoreContext.GroupManager.GetGroups(Guid.Empty);
                foreach (var g in CoreContext.UserManager.GetUserGroups(u.ID))
                {
                    var manager = CoreContext.UserManager.GetUsers(CoreContext.UserManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            stopwatch.Stop();

            stopwatch.Restart();
            var users = CoreContext.UserManager.GetUsersByGroup(Constants.GroupUser.ID);
            var visitors = CoreContext.UserManager.GetUsersByGroup(Constants.GroupVisitor.ID);
            var all = CoreContext.UserManager.GetUsers();
            stopwatch.Stop();
        }
    }
}
#endif
