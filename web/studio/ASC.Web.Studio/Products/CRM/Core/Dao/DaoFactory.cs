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

#endregion 

namespace ASC.CRM.Core.Dao
{
    public class DaoFactory
    {

        #region Members

        private readonly int tenantID;
        private readonly String storageKey;

        #endregion

        #region Constructor

        public DaoFactory(int tenantID, String storageKey)
        {
            this.tenantID = tenantID;
            this.storageKey = storageKey;
        }

        #endregion

        #region Methods

        public TaskDao GetTaskDao()
        {
            return new TaskDao(tenantID, storageKey);
        }

        public CachedListItem GetCachedListItem()
        {

            return new CachedListItem(tenantID, storageKey);
        }

        public CachedContactDao GetContactDao()
        {
            return new CachedContactDao(tenantID, storageKey);
        }

        public CustomFieldDao GetCustomFieldDao()
        {
           
            return new CustomFieldDao(tenantID, storageKey);
        }

        public DealDao GetDealDao()
        {
            return new CachedDealDao(tenantID, storageKey);
        }

        public DealMilestoneDao GetDealMilestoneDao()
        {
            return new CachedDealMilestoneDao(tenantID,storageKey);
        }

        public ListItemDao GetListItemDao()
        {
            return new ListItemDao(tenantID, storageKey);
        }
        
        public TagDao GetTagDao()
        {
            return new TagDao(tenantID, storageKey);
        }

        public SearchDao GetSearchDao()
        {
            return new SearchDao(tenantID, storageKey);
        }

        public RelationshipEventDao GetRelationshipEventDao()
        {
            return new RelationshipEventDao(tenantID, storageKey);
        }

        public FileDao GetFileDao()
        {
            return new FileDao(tenantID, storageKey);
        }

        public CasesDao GetCasesDao()
        {
            return new CachedCasesDao(tenantID, storageKey);
        }

        public TaskTemplateContainerDao GetTaskTemplateContainerDao()
        {
            return new TaskTemplateContainerDao(tenantID, storageKey);
        }

        public TaskTemplateDao GetTaskTemplateDao()
        {
            return new TaskTemplateDao(tenantID, storageKey);
        }

        public ReportDao GetReportDao()
        {

            return new ReportDao(tenantID, storageKey);

        }

        public ContactInfoDao GetContactInfoDao()
        {
            return new ContactInfoDao(tenantID, storageKey);
        }


        #endregion 
    
      
    }
}
