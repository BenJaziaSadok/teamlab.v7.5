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
using System.Reflection;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using NotifySourceBase = ASC.Core.Notify.NotifySource;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{

    public class NotifySource : NotifySourceBase
    {
        public static NotifySource Instance
        {
            get;
            private set;
        }

        static NotifySource()
        {
            Instance = new NotifySource();
        }

        public NotifySource()
            : base(new Guid("{13FF36FB-0272-4887-B416-74F52B0D0B02}"))
        {

        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                NotifyConstants.Event_ResponsibleForTask,
                NotifyConstants.Event_ResponsibleForOpportunity,
                NotifyConstants.Event_AddRelationshipEvent,
                NotifyConstants.Event_TaskReminder,
                NotifyConstants.Event_SetAccess,
                NotifyConstants.Event_ExportCompleted,
                NotifyConstants.Event_ImportCompleted,
                NotifyConstants.Event_CreateNewContact);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(CRMPatternResource.patterns);
        }
    }
}