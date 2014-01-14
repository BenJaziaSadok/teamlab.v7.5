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
using ASC.Projects.Core.Domain;
using ASC.Projects.Data;
using log4net;
using NUnit.Framework;

namespace ASC.Projects.Tests.Data
{
    public class Comments
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Projects.Tests");


        [Test]
        public void CountTest()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Task task = daoFactory.GetTaskDao().GetById(1337);
            Console.WriteLine(daoFactory.GetCommentDao().Count(task));
        }

        [Test]
        public void SaveOrUpdate()
        {
            IDaoFactory daoFactory = new DaoFactory("projects", 0);

            Task task = daoFactory.GetTaskDao().GetById(25);

            Comment comment = new Comment
                                  {
                                      Content = "ghb",
                                      TargetUniqID = typeof(Task).Name + task.ID.ToString(),
                                  };

            // task.Comments.Add(comment);

            daoFactory.GetCommentDao().Save(comment);

            Console.WriteLine(comment.ID);
        }
    }
}
