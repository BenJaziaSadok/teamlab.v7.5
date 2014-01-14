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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{
    public static class NotifyConstants
    {

        public static readonly INotifyAction Event_SetAccess = new NotifyAction("SetAccess", "set access for users");

        public static readonly INotifyAction Event_ResponsibleForTask = new NotifyAction("ResponsibleForTask", "responsible for task");

        public static readonly INotifyAction Event_TaskReminder = new NotifyAction("TaskReminder", "auto reminder about task");

        public static readonly INotifyAction Event_ResponsibleForOpportunity = new NotifyAction("ResponsibleForOpportunity", "responsible for opportunity");
       
        public static readonly INotifyAction Event_AddRelationshipEvent = new NotifyAction("AddRelationshipEvent", "add relationship event");

        public static readonly INotifyAction Event_ExportCompleted = new NotifyAction("ExportCompleted", "export is completed");

        public static readonly INotifyAction Event_ImportCompleted = new NotifyAction("ImportCompleted", "import is completed");
      
        public static readonly INotifyAction Event_CreateNewContact = new NotifyAction("CreateNewContact", "create new contact");
        
        public static readonly string Tag_AdditionalData = "AdditionalData";

        public static readonly string Tag_EntityID = "EntityID";

        public static readonly string Tag_EntityTitle = "EntityTitle";

        public static readonly string Tag_EntityRelativeURL = "EntityRelativeURL";

    }
}