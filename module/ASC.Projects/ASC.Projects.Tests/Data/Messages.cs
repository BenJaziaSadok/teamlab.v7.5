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

using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Messages : TestBase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Test]
        public void SaveOrUpdateTest()
        {
            var daoFactory = new DaoFactory("projects", 0);
            var messageDao = daoFactory.GetMessageDao();

            var message = new Message()
            {
                Title = "New Message",
                Project = daoFactory.GetProjectDao().GetById(1),
                Content = "Content",
            };

            messageDao.Save(message);
        }
    }
}
