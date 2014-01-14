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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Core.Entities;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using Action = ASC.Common.Security.Authorizing.Action;
using Constants = ASC.Core.Users.Constants;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.CRM.Core
{
    public static class CRMSecurity
    {
        #region Members

        private static readonly IAction _actionRead = new Action(new Guid("{6F05C382-8BCA-4469-9424-C807A98C40D7}"), "", true, false);

        #endregion

        #region Check Permissions

        private static ISecurityObjectProvider GetCRMSecurityProvider()
        {
            return new CRMSecurityObjectProvider(Global.DaoFactory);
        }

        private static bool IsPrivate(ISecurityObjectId entity)
        {
            return GetAccessSubjectTo(entity).Any();
        }

        private static bool CanAccessTo(ISecurityObjectId entity)
        {
            return IsAdmin || SecurityContext.CheckPermissions(entity, GetCRMSecurityProvider(), _actionRead);
        }

        private static void MakePublic(ISecurityObjectId entity)
        {
            SetAccessTo(entity, new List<Guid>());
        }

        public static IEnumerable<int> GetPrivateItems(Type objectType)
        {
            if (IsAdmin) return new List<int>();

            return GetPrivateItems(objectType, Guid.Empty, true);

        }

        private static IEnumerable<int> GetPrivateItems(Type objectType, Guid userId, bool withoutUser)
        {
            var query = CoreContext.AuthorizationManager
                                   .GetAces(userId, _actionRead.ID)
                                   .Where(
                                       item =>
                                       !String.IsNullOrEmpty(item.ObjectId) &&
                                       item.ObjectId.StartsWith(objectType.FullName))
                                   .GroupBy(item => item.ObjectId, item => item.SubjectId);

            if (withoutUser)
            {
                if (userId != Guid.Empty)
                    query = query.Where(item => !item.Contains(userId));
                else
                    query = query.Where(item => !item.Contains(SecurityContext.CurrentAccount.ID));

            }

            return query.Select(item => Convert.ToInt32(item.Key.Split(new[] { '|' })[1]));
        }

        public static IEnumerable<int> GetContactsIdByManager(Guid userId)
        {
            return GetPrivateItems(typeof(Company), userId, false)
                .Union(GetPrivateItems(typeof(Person), userId, false));
        }

        public static int GetPrivateItemsCount(Type objectType)
        {
            if (IsAdmin) return 0;

            return GetPrivateItems(objectType).Count();
        }


        private static Dictionary<Guid, String> GetAccessSubjectTo(ISecurityObjectId entity, EmployeeStatus employeeStatus)
        {
            var allAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity,
                                                                               GetCRMSecurityProvider())
                                     .Where(item => item.SubjectId != Constants.GroupEveryone.ID);

            var result = new Dictionary<Guid, String>();

            foreach (var azRecord in allAces)
            {
                if (!result.ContainsKey(azRecord.SubjectId))
                {
                    var userInfo = CoreContext.UserManager.GetUsers(azRecord.SubjectId);
                    var displayName = employeeStatus == EmployeeStatus.All || userInfo.Status == employeeStatus
                                          ? userInfo.DisplayUserName()
                                          : Constants.LostUser.DisplayUserName();
                    result.Add(azRecord.SubjectId, displayName);
                }
            }
            return result;
        }

        private static Dictionary<Guid, String> GetAccessSubjectTo(ISecurityObjectId entity)
        {
            return GetAccessSubjectTo(entity, EmployeeStatus.All);
        }

        private static List<Guid> GetAccessSubjectGuidsTo(ISecurityObjectId entity)
        {
            var allAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity,
                                                                               GetCRMSecurityProvider())
                                     .Where(item => item.SubjectId != Constants.GroupEveryone.ID);

            var result = new List<Guid>();

            foreach (var azRecord in allAces)
            {
                if (!result.Contains(azRecord.SubjectId))
                    result.Add(azRecord.SubjectId);
            }
            return result;
        }

        private static void SetAccessTo(ISecurityObjectId entity, List<Guid> subjectID)
        {
            if (subjectID.Count == 0)
            {
                CoreContext.AuthorizationManager.RemoveAllAces(entity);

                return;
            }

            var currentObjectAces = CoreContext.AuthorizationManager.GetAcesWithInherits(Guid.Empty, _actionRead.ID, entity, GetCRMSecurityProvider());

            currentObjectAces.Where(azRecord => !subjectID.Contains(azRecord.SubjectId))
                             .ToList().ForEach(azRecord =>
                                 {
                                     if ((azRecord.SubjectId == Constants.GroupEveryone.ID) && azRecord.Reaction == AceType.Allow)
                                         return;

                                     CoreContext.AuthorizationManager.RemoveAce(azRecord);
                                 });


            var oldSubjectIDList = currentObjectAces.Select(azRecord => azRecord.SubjectId).ToList();

            subjectID.FindAll(item => !oldSubjectIDList.Contains(item))
                     .ForEach(item => CoreContext.AuthorizationManager.AddAce(new AzRecord(item, _actionRead.ID, AceType.Allow,
                                                                                           entity)));

            CoreContext.AuthorizationManager.AddAce(new AzRecord(Constants.GroupEveryone.ID, _actionRead.ID, AceType.Deny,
                                                                 entity));
        }

        #endregion

        public static void SetAccessTo(File file)
        {
            if (IsAdmin || file.CreateBy == SecurityContext.CurrentAccount.ID || file.ModifiedBy == SecurityContext.CurrentAccount.ID)
                file.Access = FileShare.None;
            else
                file.Access = FileShare.Read;
        }

        public static bool CanAccessTo(Deal deal)
        {
            return CanAccessTo((ISecurityObjectId)deal);
        }

        public static bool CanAccessTo(RelationshipEvent relationshipEvent)
        {
            return CanAccessTo((ISecurityObjectId)relationshipEvent);
        }

        public static bool CanAccessTo(Contact contact)
        {
            return contact.IsShared || IsAdmin || GetAccessSubjectTo(contact).ContainsKey(SecurityContext.CurrentAccount.ID);
        }

        public static bool CanAccessTo(Task task)
        {
            if (IsAdmin || task.ResponsibleID == SecurityContext.CurrentAccount.ID ||
                (task.ContactID == 0 && task.EntityID == 0) || task.CreateBy == SecurityContext.CurrentAccount.ID) return true;

            if (task.ContactID > 0)
            {
                var contactObj = Global.DaoFactory.GetContactDao().GetByID(task.ContactID);

                if (contactObj != null) return CanAccessTo(contactObj);

            //    task.ContactID = 0;

            //    Global.DaoFactory.GetTaskDao().SaveOrUpdateTask(task);

            }

            if (task.EntityType == EntityType.Case)
            {
                var caseObj = Global.DaoFactory.GetCasesDao().GetByID(task.EntityID);

                if (caseObj != null) return CanAccessTo(caseObj);

             //   task.EntityType = EntityType.Any;
             //   task.EntityID = 0;

              //  Global.DaoFactory.GetTaskDao().SaveOrUpdateTask(task);

            }

            if (task.EntityType == EntityType.Opportunity)
            {
                var dealObj = Global.DaoFactory.GetDealDao().GetByID(task.EntityID);

                if (dealObj != null) return CanAccessTo(dealObj);

             //   task.EntityType = EntityType.Any;
             //   task.EntityID = 0;

             //   Global.DaoFactory.GetTaskDao().SaveOrUpdateTask(task);
            }

            return false;

        }

        public static bool CanGoToFeed(Task task)
        {
            return IsAdmin || task.ResponsibleID == SecurityContext.CurrentAccount.ID || task.CreateBy == SecurityContext.CurrentAccount.ID;
        }


        public static bool CanEdit(File file)
        {
            if (!(IsAdmin || file.CreateBy == SecurityContext.CurrentAccount.ID || file.ModifiedBy == SecurityContext.CurrentAccount.ID))
                return false;

            if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                return false;

            return true;
        }

        public static bool CanEdit(RelationshipEvent relationshipEvent)
        {
            return (IsAdmin || relationshipEvent.CreateBy == SecurityContext.CurrentAccount.ID);
        }

        public static bool CanEdit(Task task)
        {
            return (IsAdmin || task.ResponsibleID == SecurityContext.CurrentAccount.ID || task.CreateBy == SecurityContext.CurrentAccount.ID);
        }

        public static bool CanEdit(ListItem listItem)
        {
            return IsAdmin;
        }

        public static bool CanEdit(Contact contact)
        {
            return IsAdmin || GetAccessSubjectTo(contact).ContainsKey(SecurityContext.CurrentAccount.ID);
        }

        public static IEnumerable<Task> FilterRead(IEnumerable<Task> tasks)
        {
            if (tasks == null || !tasks.Any()) return new List<Task>();

            var result = tasks;
            var contactIDs = result.Select(objTask => objTask.ContactID).Distinct();
            
            if (contactIDs.Any())
            {
               contactIDs =  Global.DaoFactory.GetContactDao()
                                  .GetContacts(contactIDs.ToArray())
                                  .FindAll(CanAccessTo)
                                  .Select(x => x.ID);

               result = result.Where(x => x.ContactID == 0 || contactIDs.Contains(x.ContactID));

               if (!result.Any()) return Enumerable.Empty<Task>();
            }

            var casesIds = result.Where(x => x.EntityType == EntityType.Case).Select(x => x.EntityID).Distinct();

            if (casesIds.Any())
            {
                casesIds = Global.DaoFactory.GetCasesDao()
                                   .GetCases(casesIds.ToArray())
                                   .FindAll(CanAccessTo)
                                   .Select(x => x.ID);

                result = result.Where(x => x.EntityID == 0 || casesIds.Contains(x.EntityID));

                if (!result.Any()) return Enumerable.Empty<Task>();
            }

            var dealsIds = result.Where(x => x.EntityType == EntityType.Opportunity).Select(x => x.EntityID).Distinct();

            if (dealsIds.Any())
            {
                dealsIds = Global.DaoFactory.GetDealDao()
                                   .GetDeals(dealsIds.ToArray())
                                   .FindAll(CanAccessTo)
                                   .Select(x => x.ID);

                result = result.Where(x => x.EntityID == 0 || dealsIds.Contains(x.EntityID));

                if (!result.Any()) return Enumerable.Empty<Task>();
            }
            
            return result.ToList();

        }

        public static bool CanAccessTo(Cases cases)
        {
            return CanAccessTo((ISecurityObjectId)cases);
        }

        public static void SetAccessTo(Deal deal, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)deal, subjectID);
        }

        public static void MakePublic(Deal deal)
        {
            MakePublic((ISecurityObjectId)deal);
        }


        public static void SetAccessTo(RelationshipEvent relationshipEvent, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)relationshipEvent, subjectID);
        }

        public static void MakePublic(RelationshipEvent relationshipEvent)
        {
            MakePublic((ISecurityObjectId)relationshipEvent);
        }

        public static void SetAccessTo(Contact contact, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)contact, subjectID);
        }

        public static void MakePublic(Contact contact)
        {
            MakePublic((ISecurityObjectId)contact);
        }

        public static void SetAccessTo(Cases cases, List<Guid> subjectID)
        {
            SetAccessTo((ISecurityObjectId)cases, subjectID);
        }

        public static void MakePublic(Cases cases)
        {
            MakePublic((ISecurityObjectId)cases);
        }

        public static bool IsPrivate(Deal deal)
        {
            return IsPrivate((ISecurityObjectId)deal);
        }

        public static bool IsPrivate(RelationshipEvent relationshipEvent)
        {
            return IsPrivate((ISecurityObjectId)relationshipEvent);
        }

        public static bool IsPrivate(Contact contact)
        {
            return !CanAccessTo(contact);
        }

        public static bool IsPrivate(Cases cases)
        {
            return IsPrivate((ISecurityObjectId)cases);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Deal deal)
        {
            return GetAccessSubjectTo((ISecurityObjectId)deal);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(RelationshipEvent relationshipEvent)
        {
            return GetAccessSubjectTo((ISecurityObjectId)relationshipEvent);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Contact contact)
        {
            return GetAccessSubjectTo((ISecurityObjectId)contact);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Contact contact, EmployeeStatus employeeStatus)
        {
            return GetAccessSubjectTo((ISecurityObjectId)contact, employeeStatus);
        }

        public static Dictionary<Guid, string> GetAccessSubjectTo(Cases cases)
        {
            return GetAccessSubjectTo((ISecurityObjectId)cases);
        }


        public static List<Guid> GetAccessSubjectGuidsTo(Deal deal)
        {
            return GetAccessSubjectGuidsTo((ISecurityObjectId)deal);
        }

        public static List<Guid> GetAccessSubjectGuidsTo(RelationshipEvent relationshipEvent)
        {
            return GetAccessSubjectGuidsTo((ISecurityObjectId)relationshipEvent);
        }

        public static List<Guid> GetAccessSubjectGuidsTo(Contact contact)
        {
            return GetAccessSubjectGuidsTo((ISecurityObjectId)contact);
        }

        public static List<Guid> GetAccessSubjectGuidsTo(Cases cases)
        {
            return GetAccessSubjectGuidsTo((ISecurityObjectId)cases);
        }

        public static void DemandAccessTo(Deal deal)
        {
            if (!CanAccessTo(deal)) throw CreateSecurityException();
        }

        public static void DemandAccessTo(RelationshipEvent relationshipEvent)
        {
            if (!CanAccessTo(relationshipEvent)) throw CreateSecurityException();
        }


        public static void DemandAccessTo(Contact contact)
        {
            if (!CanAccessTo(contact)) throw CreateSecurityException();
        }

        public static void DemandEdit(Task task)
        {
            if (!CanEdit(task)) throw CreateSecurityException();
        }

        public static void DemandEdit(ListItem listItem)
        {
            if (!CanEdit(listItem)) throw CreateSecurityException();
        }


        public static void DemandEdit(File file)
        {
            if (!CanEdit(file)) throw CreateSecurityException();
        }


        public static void DemandAccessTo(Cases cases)
        {
            if (!CanAccessTo(cases)) throw CreateSecurityException();
        }

        public static void DemandAccessTo(File file)
        {
            //   if (!CanAccessTo((File)file)) CreateSecurityException();
        }

        public static Exception CreateSecurityException()
        {
            throw new SecurityException("Access denied.");
        }

        public static bool IsAdmin
        {
            get
            {
                return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID) ||
                       WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, SecurityContext.CurrentAccount.ID);
            }
        }

        public static bool IsAdministrator(Guid userId)
        {
            return CoreContext.UserManager.IsUserInGroup(userId, Constants.GroupAdmin.ID) ||
                   WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, userId);
        }
    }
}