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
using ASC.Projects.Data;

namespace ASC.Projects.Engine
{
    public class EngineFactory
    {
        public static readonly Guid ProductId = new Guid("1e044602-43b5-4d79-82f3-fd6208a11960");

        private readonly IDaoFactory daoFactory;
        private readonly string dbId;
        private readonly int tenantID;

        public bool DisableNotifications { get; set; }

        public EngineFactory(string dbId, int tenantID)
        {
            this.dbId = dbId;
            this.tenantID = tenantID;

            daoFactory = new DaoFactory(dbId, tenantID);
        }

        public FileEngine GetFileEngine()
        {
            return new FileEngine(dbId, tenantID);
        }

        public ProjectEngine GetProjectEngine()
        {
            return new CachedProjectEngine(daoFactory, this);
        }

        public MilestoneEngine GetMilestoneEngine()
        {
            return new MilestoneEngine(daoFactory, this);
        }

        public CommentEngine GetCommentEngine()
        {
            return new CommentEngine(daoFactory);
        }

        public SearchEngine GetSearchEngine()
        {
            return new SearchEngine(daoFactory);
        }

        public TaskEngine GetTaskEngine()
        {
            return new TaskEngine(daoFactory, this);
        }

        public SubtaskEngine GetSubtaskEngine()
        {
            return new SubtaskEngine(daoFactory, this);
        }

        public MessageEngine GetMessageEngine()
        {
            return new MessageEngine(daoFactory, this);
        }

        public TimeTrackingEngine GetTimeTrackingEngine()
        {
            return new TimeTrackingEngine(daoFactory);
        }

        public ParticipantEngine GetParticipantEngine()
        {
            return new ParticipantEngine(daoFactory);
        }

        public TagEngine GetTagEngine()
        {
            return new TagEngine(daoFactory);
        }

        public ReportEngine GetReportEngine()
        {
            return new ReportEngine(daoFactory);
        }

        public TemplateEngine GetTemplateEngine()
        {
            return new TemplateEngine(daoFactory, this);
        }
    }
}
