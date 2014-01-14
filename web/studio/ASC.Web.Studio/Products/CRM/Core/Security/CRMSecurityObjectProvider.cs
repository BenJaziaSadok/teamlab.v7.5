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
using System.Collections.Generic;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using Action = ASC.Common.Security.Authorizing.Action;
#endregion

namespace ASC.CRM.Core
{
    public class CRMSecurityObjectProvider : ISecurityObjectProvider
    {
        private readonly DaoFactory _daoFactory;

        public CRMSecurityObjectProvider(DaoFactory daoFactory)
        {
            _daoFactory = daoFactory;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            int contactId;
            int entityId;
            EntityType entityType;

            if (objectId is Task)
            {
                var task = (Task)objectId;

                contactId = task.ContactID;
                entityId = task.EntityID;
                entityType = task.EntityType;
            }
            else if (objectId is RelationshipEvent)
            {
                var eventObj = (RelationshipEvent)objectId;

                contactId = eventObj.ContactID;
                entityId = eventObj.EntityID;
                entityType = eventObj.EntityType;
                
            }
            else
            {
                return null;
            }

            if (entityId == 0 && contactId == 0) return null;

            if (entityId == 0)
            return new Company
                {
                    ID = contactId,
                    CompanyName = "fakeCompany"
                };
                
                //   return _daoFactory.GetContactDao().GetByID(contactId);

            switch (entityType)
            {

                case EntityType.Opportunity:
                    return new Deal
                        {
                            ID = entityId,
                            Title = "fakeDeal"
                        };
                   // return _daoFactory.GetDealDao().GetByID(entityId);
                case EntityType.Case:
                    return new Cases
                        {
                            ID = entityId, 
                            Title = "fakeCases"
                        };
                  //  return _daoFactory.GetCasesDao().GetByID(entityId);
            }

            return null;
        }

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return false; }
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
 
          //   Constants.Everyone
          // if (_daoFactory.GetManagerDao().GetAll(false).Contains(ASC.Core.CoreContext.UserManager.GetUsers(account.ID)))
          //   return new Action[]
            throw new NotImplementedException();
        }
    }
}