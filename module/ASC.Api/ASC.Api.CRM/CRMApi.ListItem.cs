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
using System.Linq;
using System.Security;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///   Creates an opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: 0 (Open), 1 (ClosedAndWon),2 (ClosedAndLost)">Stage type</param>
        /// <short>Create opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [Create(@"opportunity/stage")]
        public DealMilestoneWrapper CreateDealMilestone(
            String title,
            String description,
            String color,
            int successProbability,
            DealMilestoneStatus stageType)
        {

            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            if (successProbability < 0)
                successProbability = 0;

            var dealMilestone = new DealMilestone
                                    {
                                        Title = title,
                                        Color = color,
                                        Description = description,
                                        Probability = successProbability,
                                        Status = stageType
                                    };

            dealMilestone.ID = DaoFactory.GetDealMilestoneDao().Create(dealMilestone);

            return ToDealMilestoneWrapper(dealMilestone);
        }


        /// <summary>
        ///    Updates the selected opportunity stage with the parameters (title, description, success probability, etc.) specified in the request
        /// </summary>
        /// <param name="id">Opportunity stage ID</param>
        /// <param name="title">Title</param>
        /// <param name="description">Description</param>
        /// <param name="color">Color</param>
        /// <param name="successProbability">Success probability</param>
        /// <param name="stageType" remark="Allowed values: Open, ClosedAndWon, ClosedAndLost">Stage type</param>
        /// <short>Update opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [Update(@"opportunity/stage/{id:[0-9]+}")]
        public DealMilestoneWrapper UpdateDealMilestone(
            int id,
            String title,
            String description,
            String color,
            int successProbability,
            DealMilestoneStatus stageType)
        {
            if (id <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            if (successProbability < 0)
                successProbability = 0;

            var curDealMilestoneExist = DaoFactory.GetDealMilestoneDao().IsExist(id);
            if (!curDealMilestoneExist)
                throw new ItemNotFoundException();

            var dealMilestone = new DealMilestone
            {
                Title = title,
                Color = color,
                Description = description,
                Probability = successProbability,
                Status = stageType,
                ID = id
            };

            DaoFactory.GetDealMilestoneDao().Edit(dealMilestone);

            return ToDealMilestoneWrapper(dealMilestone);
        }


        /// <summary>
        ///    Updates the selected opportunity stage with the color specified in the request
        /// </summary>
        /// <param name="id">Opportunity stage ID</param>
        /// <param name="color">Color</param>
        /// <short>Update opportunity stage color</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Opportunity stage
        /// </returns>
        [Update(@"opportunity/stage/{id:[0-9]+}/color")]
        public DealMilestoneWrapper UpdateDealMilestoneColor(int id, String color)
        {
            if (id <= 0)
                throw new ArgumentException();

            var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(id);
            if (dealMilestone == null)
                throw new ItemNotFoundException();

            dealMilestone.Color = color;

            DaoFactory.GetDealMilestoneDao().ChangeColor(id, color);

            return ToDealMilestoneWrapper(dealMilestone);
        }


		/// <summary>
		///    Updates the available opportunity stages order with the list specified in the request
		/// </summary>
		/// <short>
		///    Update opportunity stages order
		/// </short>
		/// <param name="ids">Opportunity stage ID list</param>
        /// <category>Opportunities</category>
        /// <returns>
        ///    Opportunity stages
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("opportunity/stage/reorder")]
        public IEnumerable<DealMilestoneWrapper> UpdateDealMilestonesOrder(IEnumerable<int> ids)
        {
            if (ids == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<DealMilestoneWrapper>();
            foreach (var id in ids)
            {
                try
                {
                    var dealMilestoneWrapper = ToDealMilestoneWrapper(DaoFactory.GetDealMilestoneDao().GetByID(id));

                    result.Add(dealMilestoneWrapper);
                }
                catch (ArgumentException)
                { }
            }

            DaoFactory.GetDealMilestoneDao().Reorder(ids.ToArray<int>());

            return result;
        }


        /// <summary>
        ///   Deletes the opportunity stage with the ID specified in the request
        /// </summary>
        /// <short>Delete opportunity stage</short> 
        /// <category>Opportunities</category>
        /// <param name="id">Opportunity stage ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity stage
        /// </returns>
        [Delete(@"opportunity/stage/{id:[0-9]+}")]
        public DealMilestoneWrapper DeleteDealMilestone(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(id);

            if (dealMilestone == null)
                throw new ItemNotFoundException();

            var result = ToDealMilestoneWrapper(dealMilestone);

            DaoFactory.GetDealMilestoneDao().Delete(id);

            return result;
        }




        /// <summary>
        ///   Creates a new history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Create history category</short> 
        /// <category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        [Create(@"history/category")]
        public HistoryCategoryWrapper CreateHistoryCategory(String title,
                                               String description,
                                               String imageName,
                                               int sortOrder)
        {
            if (String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName
            };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.HistoryCategory, listItem);

            return ToHistoryCategoryWrapper(listItem);
        }


        /// <summary>
        ///   Updates the selected history category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">History category ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Update history category</short> 
        ///<category>History</category>
        ///<returns>History category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"history/category/{id:[0-9]+}")]
        public HistoryCategoryWrapper UpdateHistoryCategory(
           int id,
           String title,
           String description,
           String imageName,
           int sortOrder)
        {
            if (id <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var curHistoryCategoryExist = DaoFactory.GetListItemDao().IsExist(id);
            if (!curHistoryCategoryExist)
                throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id
            };

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().EditItem(ListType.HistoryCategory, listItem);

            return ToHistoryCategoryWrapper(listItem);
        }


        /// <summary>
        ///    Updates the icon of the selected history category
        /// </summary>
        /// <param name="id">History category ID</param>
        /// <param name="imageName">icon name</param>
        /// <short>Update history category icon</short> 
        /// <category>History</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    History category
        /// </returns>
        [Update(@"history/category/{id:[0-9]+}/icon")]
        public HistoryCategoryWrapper UpdateHistoryCategoryIcon(int id, String imageName)
        {
            if (id <= 0)
                throw new ArgumentException();

            var historyCategory = DaoFactory.GetListItemDao().GetByID(id);
            if (historyCategory == null)
                throw new ItemNotFoundException();

            historyCategory.AdditionalParams = imageName;

            DaoFactory.GetListItemDao().ChangePicture(id, imageName);

            return ToHistoryCategoryWrapper(historyCategory);
        }


		/// <summary>
		///    Updates the history categories order with the list specified in the request
		/// </summary>
		/// <short>
		///    Update history categories order
		/// </short>
		/// <param name="titles">History category title list</param>
        /// <category>History</category>
        /// <returns>
        ///    History categories
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("history/category/reorder")]
        public IEnumerable<HistoryCategoryWrapper> UpdateHistoryCategoriesOrder(IEnumerable<string> titles)
        {
            if (titles == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<HistoryCategoryWrapper>();
            foreach (var title in titles)
            {
                try
                {
                    var historyCategoryWrapper = ToHistoryCategoryWrapper(DaoFactory.GetListItemDao().GetByTitle(ListType.HistoryCategory, title));

                    result.Add(historyCategoryWrapper);
                }
                catch (ArgumentException)
                { }
            }

            DaoFactory.GetListItemDao().ReorderItems(ListType.HistoryCategory, titles.ToArray<String>());

            return result;
        }


        /// <summary>
        ///   Deletes the selected history category with the ID specified in the request
        /// </summary>
        /// <short>Delete history category</short> 
        /// <category>History</category>
        /// <param name="id">History category ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>History category</returns>
        [Delete(@"history/category/{id:[0-9]+}")]
        public HistoryCategoryWrapper DeleteHistoryCategory(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(id);

            if (listItem == null)
                throw new ItemNotFoundException();

            var result = ToHistoryCategoryWrapper(listItem);

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.HistoryCategory, id);

            return result;
        }




        /// <summary>
        ///   Creates a new task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Create task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [Create(@"task/category")]
        public TaskCategoryWrapper CreateTaskCategory(String title,
                                               String description,
                                               String imageName,
                                               int sortOrder)
        {
            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName
            };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.TaskCategory, listItem);

            return ToTaskCategoryWrapper(listItem);
        }


        /// <summary>
        ///   Updates the selected task category with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Task category ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="sortOrder">Order</param>
        ///<param name="imageName">Image name</param>
        ///<short>Update task category</short> 
        ///<category>Tasks</category>
        ///<returns>Task category</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        ///<returns>
        ///    Task category
        ///</returns>
        [Update(@"task/category/{id:[0-9]+}")]
        public TaskCategoryWrapper UpdateTaskCategory(
                                                   int id,
                                                   String title,
                                                   String description,
                                                   String imageName,
                                                   int sortOrder)
        {
            if (id <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var curTaskCategoryExist =  DaoFactory.GetListItemDao().IsExist(id);
            if (!curTaskCategoryExist)
                throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                Title = title,
                Description = description,
                SortOrder = sortOrder,
                AdditionalParams = imageName,
                ID = id
            };

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().EditItem(ListType.TaskCategory, listItem);

            return ToTaskCategoryWrapper(listItem);
        }


        /// <summary>
        ///    Updates the icon of the task category with the ID specified in the request
        /// </summary>
        /// <param name="id">Task category ID</param>
        /// <param name="imageName">icon name</param>
        /// <short>Update task category icon</short> 
        /// <category>Tasks</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Task category
        /// </returns>
        [Update(@"task/category/{id:[0-9]+}/icon")]
        public TaskCategoryWrapper UpdateTaskCategoryIcon(int id, String imageName)
        {
            if (id <= 0)
                throw new ArgumentException();

            var taskCategory = DaoFactory.GetListItemDao().GetByID(id);
            if (taskCategory == null)
                throw new ItemNotFoundException();

            taskCategory.AdditionalParams = imageName;

            DaoFactory.GetListItemDao().ChangePicture(id, imageName);

            return ToTaskCategoryWrapper(taskCategory);
        }


		/// <summary>
		///    Updates the task categories order with the list specified in the request
		/// </summary>
		/// <short>
		///    Update task categories order
		/// </short>
		/// <param name="titles">Task category title list</param>
        /// <category>Tasks</category>
        /// <returns>
        ///    Task categories
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("task/category/reorder")]
        public IEnumerable<TaskCategoryWrapper> UpdateTaskCategoriesOrder(IEnumerable<string> titles)
        {
            if (titles == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<TaskCategoryWrapper>();
            foreach (var title in titles)
            {
                try
                {
                    var taskCategoryWrapper = ToTaskCategoryWrapper(DaoFactory.GetListItemDao().GetByTitle(ListType.TaskCategory, title));

                    result.Add(taskCategoryWrapper);
                }
                catch (ArgumentException)
                { }
            }

            DaoFactory.GetListItemDao().ReorderItems(ListType.TaskCategory, titles.ToArray<String>());

            return result;
        }


        /// <summary>
        ///   Deletes the task category with the ID specified in the request
        /// </summary>
        /// <short>Delete task category</short> 
        /// <category>Tasks</category>
        /// <param name="categoryid">Task category ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        [Delete(@"task/category/{categoryid:[0-9]+}")]
        public TaskCategoryWrapper DeleteTaskCategory(int categoryid)
        {
            if (categoryid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(categoryid);

            if (listItem == null)
                throw new ItemNotFoundException();

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.TaskCategory, categoryid);

            return ToTaskCategoryWrapper(listItem);
        }



        /// <summary>
        ///   Creates a new contact status with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact status</returns>
        /// <short>Create contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [Create(@"contact/status")]
        public ContactStatusWrapper CreateContactStatus(String title,
                                             String description,
                                             String color,
                                             int sortOrder)
        {
            var listItem = new ListItem
                               {
                                   Title = title,
                                   Description = description,
                                   Color = color,
                                   SortOrder = sortOrder
                               };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.ContactStatus, listItem);

            return ToContactStatusWrapper(listItem);
        }


        /// <summary>
        ///   Updates the selected contact status with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Contact status ID</param>
        ///<param name="title">Title</param>
        ///<param name="description">Description</param>
        ///<param name="color">Color</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact status</returns>
        /// <short>Update contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [Update(@"contact/status/{id:[0-9]+}")]
        public ContactStatusWrapper UpdateContactStatus(
                                                    int id,
                                                    String title,
                                                    String description,
                                                    String color,
                                                    int sortOrder)
        {
            if (id <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var curListItemExist =  DaoFactory.GetListItemDao().IsExist(id);
            if (!curListItemExist)
                throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                ID = id,
                Title = title,
                Description = description,
                Color = color,
                SortOrder = sortOrder
            };

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().EditItem(ListType.ContactStatus, listItem);

            return ToContactStatusWrapper(listItem);
        }


        /// <summary>
        ///    Updates the color of the selected contact status with the new color specified in the request
        /// </summary>
        /// <param name="id">Contact status ID</param>
        /// <param name="color">Color</param>
        /// <short>Update contact status color</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Contact status
        /// </returns>
        [Update(@"contact/status/{id:[0-9]+}/color")]
        public ContactStatusWrapper UpdateContactStatusColor(int id, String color)
        {
            if (id <= 0)
                throw new ArgumentException();

            var contactStatus = DaoFactory.GetListItemDao().GetByID(id);
            if (contactStatus == null)
                throw new ItemNotFoundException();

            contactStatus.Color = color;

            DaoFactory.GetListItemDao().ChangeColor(id, color);

            return ToContactStatusWrapper(contactStatus);
        }


		/// <summary>
		///    Updates the contact statuses order with the list specified in the request
		/// </summary>
		/// <short>
		///    Update contact statuses order
		/// </short>
		/// <param name="titles">Contact status title list</param>
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact statuses
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("contact/status/reorder")]
        public IEnumerable<ContactStatusWrapper> UpdateContactStatusesOrder(IEnumerable<string> titles)
        {
            if (titles == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<ContactStatusWrapper>();
            foreach (var title in titles)
            {
                try
                {
                    var contactTypeWrapper = ToContactStatusWrapper(DaoFactory.GetListItemDao().GetByTitle(ListType.ContactStatus, title));

                    result.Add(contactTypeWrapper);
                }
                catch (ArgumentException)
                { }
            }

            DaoFactory.GetListItemDao().ReorderItems(ListType.ContactStatus, titles.ToArray<String>());

            return result;
        }


        /// <summary>
        ///   Deletes the contact status with the ID specified in the request
        /// </summary>
        /// <short>Delete contact status</short> 
        /// <category>Contacts</category>
        /// <param name="contactStatusid">Contact status ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        /// <returns>
        ///  Contact status
        /// </returns>
        [Delete(@"contact/status/{contactStatusid:[0-9]+}")]
        public ContactStatusWrapper DeleteContactStatus(int contactStatusid)
        {
            if (contactStatusid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactStatusid);

            if (listItem == null)
                throw new ItemNotFoundException();

            var contactStatus = ToContactStatusWrapper(listItem);

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.ContactStatus, contactStatusid);

            return contactStatus;
        }




        /// <summary>
        ///   Returns the status of the contact for the ID specified in the request
        /// </summary>
        /// <param name="contactStatusid">Contact status ID</param>
        /// <returns>Contact status</returns>
        /// <short>Get contact status</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"contact/status/{contactStatusid:[0-9]+}")]
        public ContactStatusWrapper GetContactStatusByID(int contactStatusid)
        {
            if (contactStatusid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactStatusid);

            if (listItem == null)
                throw new ItemNotFoundException();

            return ToContactStatusWrapper(listItem);
        }



        /// <summary>
        ///   Creates a new contact type with the parameters (title, etc.) specified in the request
        /// </summary>
        ///<param name="title">Title</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Create contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [Create(@"contact/type")]
        public ContactTypeWrapper CreateContactType(String title, int sortOrder)
        {
            var listItem = new ListItem
                               {
                                   Title = title,
                                   Description = String.Empty,
                                   SortOrder = sortOrder
                               };

            listItem.ID = DaoFactory.GetListItemDao().CreateItem(ListType.ContactType, listItem);

            return ToContactTypeWrapper(listItem);
        }


        /// <summary>
        ///   Updates the selected contact type with the parameters (title, description, etc.) specified in the request
        /// </summary>
        ///<param name="id">Contact type ID</param>
        ///<param name="title">Title</param>
        ///<param name="sortOrder">Order</param>
        ///<returns>Contact type</returns>
        /// <short>Update contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <returns>
        ///    Contact type
        /// </returns>
        [Update(@"contact/type/{id:[0-9]+}")]
        public ContactTypeWrapper UpdateContactType(
                                                    int id,
                                                    String title,
                                                    int sortOrder)
        {
            if (id <= 0 || String.IsNullOrEmpty(title))
                throw new ArgumentException();

            var curListItemExist = DaoFactory.GetListItemDao().IsExist(id);
            if (!curListItemExist)
                throw new ItemNotFoundException();

            var listItem = new ListItem
            {
                ID = id,
                Title = title,
                SortOrder = sortOrder
            };

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().EditItem(ListType.ContactType, listItem);

            return ToContactTypeWrapper(listItem);
        }



		/// <summary>
		///    Updates the contact types order with the list specified in the request
		/// </summary>
		/// <short>
		///    Update contact types order
		/// </short>
		/// <param name="titles">Contact type title list</param>
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact types
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("contact/type/reorder")]
        public IEnumerable<ContactTypeWrapper> UpdateContactTypesOrder(IEnumerable<string> titles)
        {
            if (titles == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<ContactTypeWrapper>();
            foreach (var title in titles)
            {
                try
                {
                    var contactTypeWrapper = ToContactTypeWrapper(DaoFactory.GetListItemDao().GetByTitle(ListType.ContactType, title));

                    result.Add(contactTypeWrapper);
                }
                catch (ArgumentException)
                { }
            }

            DaoFactory.GetListItemDao().ReorderItems(ListType.ContactType, titles.ToArray<String>());

            return result;
        }


        /// <summary>
        ///   Deletes the contact type with the ID specified in the request
        /// </summary>
        /// <short>Delete contact type</short> 
        /// <category>Contacts</category>
        /// <param name="contactTypeid">Contact type ID</param>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="SecurityException"></exception>
        /// <returns>
        ///  Contact type
        /// </returns>
        [Delete(@"contact/type/{contactTypeid:[0-9]+}")]
        public ContactTypeWrapper DeleteContactType(int contactTypeid)
        {
            if (contactTypeid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactTypeid);

            if (listItem == null)
                throw new ItemNotFoundException();

            var contactType = ToContactTypeWrapper(listItem);

            CRMSecurity.DemandEdit(listItem);

            DaoFactory.GetListItemDao().DeleteItem(ListType.ContactType, contactTypeid);

            return contactType;
        }


        /// <summary>
        ///   Returns the type of the contact for the ID specified in the request
        /// </summary>
        /// <param name="contactTypeid">Contact type ID</param>
        /// <returns>Contact type</returns>
        /// <short>Get contact type</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"contact/type/{contactTypeid:[0-9]+}")]
        public ContactTypeWrapper GetContactTypeByID(int contactTypeid)
        {
            if (contactTypeid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(contactTypeid);

            if (listItem == null)
                throw new ItemNotFoundException();

            return ToContactTypeWrapper(listItem);
        }

        /// <summary>
        ///  Returns the stage of the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="stageid">Opportunity stage ID</param>
        /// <returns>Opportunity stage</returns>
        /// <short>Get opportunity stage</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"opportunity/stage/{stageid:[0-9]+}")]
        public DealMilestoneWrapper GetDealMilestoneByID(int stageid)
        {
            if (stageid <= 0)
                throw new ArgumentException();

            var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(stageid);

            if (dealMilestone == null)
                throw new ItemNotFoundException();

            return ToDealMilestoneWrapper(dealMilestone);
        }

        /// <summary>
        ///    Returns the category of the task with the ID specified in the request
        /// </summary>
        /// <param name="categoryid">Task category ID</param>
        /// <returns>Task category</returns>
        /// <short>Get task category</short> 
        /// <category>Tasks</category>
        ///<exception cref="ItemNotFoundException"></exception>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"task/category/{categoryid:[0-9]+}")]
        public TaskCategoryWrapper GetTaskCategoryByID(int categoryid)
        {
            if (categoryid <= 0)
                throw new ArgumentException();

            var listItem = DaoFactory.GetListItemDao().GetByID(categoryid);

            if (listItem == null)
                throw new ItemNotFoundException();

            return ToTaskCategoryWrapper(listItem);
        }
      

        /// <summary>
        ///    Returns the list of all history categories available on the portal
        /// </summary>
        /// <short>Get all history categories</short> 
        /// <category>History</category>
        /// <returns>
        ///    List of all history categories
        /// </returns>
        [Read(@"history/category")]
        public IEnumerable<HistoryCategoryWrapper> GetHistoryCategoryWrapper()
        {
            var result = DaoFactory.GetListItemDao().GetItems(ListType.HistoryCategory).ConvertAll(item => new HistoryCategoryWrapper(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory);

            result.ForEach(x =>
            {
                if (relativeItemsCount.ContainsKey(x.ID))
                    x.RelativeItemsCount = relativeItemsCount[x.ID];
            });
            return result;
        }


        /// <summary>
        ///    Returns the list of all task categories available on the portal
        /// </summary>
        /// <short>Get all task categories</short> 
        /// <category>Tasks</category>
        /// <returns>
        ///    List of all task categories
        /// </returns>
        [Read(@"task/category")]
        public IEnumerable<TaskCategoryWrapper> GetTaskCategories()
        {
            var result = DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory).ConvertAll(item => new TaskCategoryWrapper(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory);

            result.ForEach(x =>
                               {
                                   if (relativeItemsCount.ContainsKey(x.ID))
                                       x.RelativeItemsCount = relativeItemsCount[x.ID];
                               });
            return result;
        }

        /// <summary>
        ///    Returns the list of all contact statuses available on the portal
        /// </summary>
        /// <short>Get all contact statuses</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    List of all contact statuses
        /// </returns>
        [Read(@"contact/status")]
        public IEnumerable<ContactStatusWrapper> GetContactStatuses()
        {
            var result = DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus).ConvertAll(item => new ContactStatusWrapper(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus);

            result.ForEach(x =>
                               {
                                   if (relativeItemsCount.ContainsKey(x.ID))
                                       x.RelativeItemsCount = relativeItemsCount[x.ID];
                               });
            return result;
        }


        /// <summary>
        ///    Returns the list of all contact types available on the portal
        /// </summary>
        /// <short>Get all contact types</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    List of all contact types
        /// </returns>
        [Read(@"contact/type")]
        public IEnumerable<ContactTypeWrapper> GetContactTypes()
        {
            var result = DaoFactory.GetListItemDao().GetItems(ListType.ContactType).ConvertAll(item => new ContactTypeWrapper(item));

            var relativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactType);

            result.ForEach(x =>
                               {
                                   if (relativeItemsCount.ContainsKey(x.ID))
                                       x.RelativeItemsCount = relativeItemsCount[x.ID];
                               });

            return result;
        }


        /// <summary>
        ///    Returns the list of all opportunity stages available on the portal
        /// </summary>
        /// <short>Get all opportunity stages</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   List of all opportunity stages
        /// </returns>
        [Read(@"opportunity/stage")]
        public IEnumerable<DealMilestoneWrapper> GetDealMilestones()
        {
            var result = DaoFactory.GetDealMilestoneDao().GetAll().ConvertAll(item => new DealMilestoneWrapper(item));

            var relativeItemsCount = DaoFactory.GetDealMilestoneDao().GetRelativeItemsCount();

            result.ForEach(x =>
            {
                if (relativeItemsCount.ContainsKey(x.ID))
                    x.RelativeItemsCount = relativeItemsCount[x.ID];
            });

            return result;
        }



        public ContactStatusWrapper ToContactStatusWrapper(ListItem listItem)
        {
            var result = new ContactStatusWrapper(listItem)
            {
                RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactStatus, listItem.ID)
            };

            return result;
        }

        public ContactTypeWrapper ToContactTypeWrapper(ListItem listItem)
        {
            var result = new ContactTypeWrapper(listItem)
            {
                RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.ContactType, listItem.ID)
            };

            return result;
        }

        public HistoryCategoryWrapper ToHistoryCategoryWrapper(ListItem listItem)
        {
            var result = new HistoryCategoryWrapper(listItem)
            {
                RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.HistoryCategory, listItem.ID)
            };
            return result;
        }

        public TaskCategoryWrapper ToTaskCategoryWrapper(ListItem listItem)
        {
            var result = new TaskCategoryWrapper(listItem)
            {
                RelativeItemsCount = DaoFactory.GetListItemDao().GetRelativeItemsCount(ListType.TaskCategory, listItem.ID)
            };
            return result;
        }

        private DealMilestoneWrapper ToDealMilestoneWrapper(DealMilestone dealMilestone)
        {
            var result = new DealMilestoneWrapper(dealMilestone)
            {
                RelativeItemsCount = DaoFactory.GetDealMilestoneDao().GetRelativeItemsCount(dealMilestone.ID)
            };
            return result;
        }

    }
}