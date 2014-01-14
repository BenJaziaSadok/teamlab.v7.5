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
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Api.Collections;
using ASC.Specific;
using ASC.Web.CRM.Classes;
using EnumExtension = ASC.Web.CRM.Classes.EnumExtension;

#endregion


namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///    Returns the detailed information about the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <returns>
        ///    Opportunity
        /// </returns>
        /// <short>Get opportunity by ID</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"opportunity/{opportunityid:[0-9]+}")]
        public OpportunityWrapper GetDealByID(int opportunityid)
        {
            if (opportunityid <= 0)
                throw new ArgumentException();

            var deal = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (deal == null)
                throw new ItemNotFoundException();
           
            return ToOpportunityWrapper(deal);

        }


        /// <summary>
        ///    Updates the selected opportunity to the stage with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="stageid">Opportunity stage ID</param>
        /// <returns>
        ///    Opportunity
        /// </returns>
        /// <short>Update opportunity stage</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Update(@"opportunity/{opportunityid:[0-9]+}/stage/{id:[0-9]+}")]
        public OpportunityWrapper UpdateToDealMilestone(
                                                          int opportunityid,
                                                          int stageid
                                                        )
        {
            if (opportunityid <= 0 || stageid <= 0)
                throw new ArgumentException();

            var deal = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (deal == null)
                throw new ItemNotFoundException();

            var stage = DaoFactory.GetDealMilestoneDao().GetByID(stageid);

            if (stage == null)
                throw new ItemNotFoundException();

            deal.DealMilestoneID = stageid;
            deal.DealMilestoneProbability = stage.Probability;

            deal.ActualCloseDate = stage.Status != DealMilestoneStatus.Open ? DateTime.UtcNow : DateTime.MinValue;
            DaoFactory.GetDealDao().EditDeal(deal);

            return ToOpportunityWrapper(deal);
        }

        /// <summary>
        ///   Sets access rights for the selected opportunity with the parameters specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="isPrivate">Opportunity privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set rights to opportunity</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity 
        /// </returns>
        [Update("opportunity/{opportunityid:[0-9]+}/access")]
        public OpportunityWrapper SetAccessToDeal(
                                                    int opportunityid,
                                                    bool isPrivate,
                                                    IEnumerable<Guid> accessList)
        {

            if (opportunityid <= 0)
                throw new ArgumentException();

            var deal = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (deal == null)
                throw new ItemNotFoundException();

            return SetAccessToDeal(deal, isPrivate, accessList);
        }


        private OpportunityWrapper SetAccessToDeal(
                                                  Deal deal,
                                                  bool isPrivate,
                                                  IEnumerable<Guid> accessList)
        {
            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
            {
                CRMSecurity.SetAccessTo(deal, accessListLocal);
            }
            else
            {
                CRMSecurity.MakePublic(deal);
            }

            return ToOpportunityWrapper(deal);
        }

        /// <summary>
        ///   Sets access rights for other users to the list of all opportunities matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="responsibleid">Opportunity responsible</param>
        /// <param optional="true" name="opportunityStagesid">Opportunity stage ID</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="contactAlsoIsParticipant">Participation status: take into account opportunities where the contact is a participant or not</param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <param optional="true" name="stageType" remark="Allowed values: {Open, ClosedAndWon, ClosedAndLost}">Opportunity stage type</param>
        /// <param name="isPrivate">Opportunity privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set opportunity access rights</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity list
        /// </returns>
        [Update("opportunity/filter/access")]
        public IEnumerable<OpportunityWrapper> SetAccessToBatchDeal(
                                                                    Guid responsibleid,
                                                                    int opportunityStagesid,
                                                                    IEnumerable<String> tags,
                                                                    int contactid,
                                                                    DealMilestoneStatus? stageType,
                                                                    bool? contactAlsoIsParticipant,
                                                                    ApiDateTime fromDate,
                                                                    ApiDateTime toDate,
                                                                    bool isPrivate,
                                                                    IEnumerable<Guid> accessList
                                                                    )
        {
            var result = new List<Deal>();
            var deals = DaoFactory.GetDealDao().GetDeals(_context.FilterValue,
                                                      responsibleid,
                                                      opportunityStagesid,
                                                      tags,
                                                      contactid,
                                                      stageType,
                                                      contactAlsoIsParticipant,
                                                      fromDate, toDate, 0, 0, null);
            if (!deals.Any())
                return Enumerable.Empty<OpportunityWrapper>();

            foreach (var deal in deals)
            {
                if (deal == null)
                    throw new ItemNotFoundException();

                if (!(CRMSecurity.IsAdmin || deal.CreateBy == ASC.Core.SecurityContext.CurrentAccount.ID))
                    continue;

                SetAccessToDeal(deal.ID, isPrivate, accessList);

                result.Add(deal);
            }

            return ToListOpportunityWrapper(result);
        }

        /// <summary>
        ///   Sets access rights for other users to the list of opportunities with the IDs specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID list</param>
        /// <param name="isPrivate">Opportunity privacy: private or not</param>
        /// <param name="accessList">List of users with access</param>
        /// <short>Set opportunity access rights</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity list
        /// </returns>
        [Update("opportunity/access")]
        public IEnumerable<OpportunityWrapper> SetAccessToBatchDeal(
                                                                    IEnumerable<int> opportunityid,
                                                                    bool isPrivate,
                                                                    IEnumerable<Guid> accessList)
        {
            var result = new List<OpportunityWrapper>();

            foreach (var id in opportunityid)
            {
                try
                {
                    result.Add(SetAccessToDeal(id, isPrivate, accessList));

                }
                catch (Exception)
                {

                }
            }

            return result;
        }


        /// <summary>
        ///   Deletes the group of opportunities with the IDs specified in the request
        /// </summary>
        /// <param name="opportunityids">Opportunity ID list</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete opportunity group</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   Opportunity list
        /// </returns>
        [Delete(@"opportunity")]
        public IEnumerable<OpportunityWrapper> DeleteBatchDeals(IEnumerable<int> opportunityids)
        {
            if (opportunityids == null || !opportunityids.Any())
                throw new ArgumentException();

            var opportunities = DaoFactory.GetDealDao().GetDeals(opportunityids.ToArray());
            var result = opportunities.Select(ToOpportunityWrapper);

            DaoFactory.GetDealDao().DeleteBatchDeals(opportunities);

            return result;
        }

        /// <summary>
        ///   Deletes the list of all opportunities matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="responsibleid">Opportunity responsible</param>
        /// <param optional="true" name="opportunityStagesid">Opportunity stage ID</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="contactAlsoIsParticipant">Participation status: take into account opportunities where the contact is a participant or not</param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <param optional="true" name="stageType" remark="Allowed values: {Open, ClosedAndWon, ClosedAndLost}">Opportunity stage type</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete opportunity group</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   Opportunity list
        /// </returns>
        [Delete(@"opportunity/filter")]
        public IEnumerable<OpportunityWrapper> DeleteBatchDeals(
                                                                Guid responsibleid,
                                                                int opportunityStagesid,
                                                                IEnumerable<String> tags,
                                                                int contactid,
                                                                DealMilestoneStatus? stageType,
                                                                bool? contactAlsoIsParticipant,
                                                                ApiDateTime fromDate,
                                                                ApiDateTime toDate)
        {
            var deals = DaoFactory.GetDealDao().GetDeals(_context.FilterValue,
                                             responsibleid,
                                             opportunityStagesid,
                                             tags,
                                             contactid,
                                             stageType,
                                             contactAlsoIsParticipant,
                                             fromDate, toDate, 0, 0, null);

            if (!deals.Any()) return Enumerable.Empty<OpportunityWrapper>();


            var result = ToListOpportunityWrapper(deals);

            DaoFactory.GetDealDao().DeleteBatchDeals(deals);

            return result;
        }

        /// <summary>
        ///   Returns the list of all opportunities matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="responsibleid">Opportunity responsible</param>
        /// <param optional="true" name="opportunityStagesid">Opportunity stage ID</param>
        /// <param optional="true" name="tags">Tags</param>
        /// <param optional="true" name="contactid">Contact ID</param>
        /// <param optional="true" name="contactAlsoIsParticipant">Participation status: take into account opportunities where the contact is a participant or not</param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <param optional="true" name="stageType" remark="Allowed values: {Open, ClosedAndWon, ClosedAndLost}">Opportunity stage type</param>
        /// <short>Get opportunity list</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///   Opportunity list
        /// </returns>
        [Read(@"opportunity/filter")]
        public IEnumerable<OpportunityWrapper> GetDeals(
                                                        Guid responsibleid,
                                                        int opportunityStagesid,
                                                        IEnumerable<String> tags,
                                                        int contactid,
                                                        DealMilestoneStatus? stageType,
                                                        bool? contactAlsoIsParticipant,
                                                        ApiDateTime fromDate,
                                                        ApiDateTime toDate)
        {
            DealSortedByType dealSortedByType;

            IEnumerable<OpportunityWrapper> result;

            var searchString = _context.FilterValue;

            OrderBy dealsOrderBy;

            if (ASC.Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out dealSortedByType))
                dealsOrderBy = new OrderBy(dealSortedByType, !_context.SortDescending);
            else if (String.IsNullOrEmpty(_context.SortBy))
                dealsOrderBy = new OrderBy(DealSortedByType.Stage, true);
            else
                dealsOrderBy = null;


            var fromIndex = (int)_context.StartIndex;
            var count = (int)_context.Count;


            if (dealsOrderBy != null)
            {
                result = ToListOpportunityWrapper(DaoFactory.GetDealDao().GetDeals(
                    searchString,
                    responsibleid,
                    opportunityStagesid,
                    tags,
                    contactid,
                    stageType,
                    contactAlsoIsParticipant,
                    fromDate,
                    toDate,
                    fromIndex,
                    count,
                    dealsOrderBy));

                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();

            }
            else
            {
                result = ToListOpportunityWrapper(DaoFactory.GetDealDao().GetDeals(
                    searchString,
                    responsibleid,
                    opportunityStagesid,
                    tags,
                    contactid,
                    stageType,
                    contactAlsoIsParticipant,
                    fromDate,
                    toDate,
                    0, 0, null));
            }

            
            int totalCount;

            if (result.Count() < count)
                totalCount = fromIndex + result.Count();
            else
                totalCount = DaoFactory.GetDealDao().GetDealsCount(searchString,
                                                                    responsibleid,
                                                                    opportunityStagesid,
                                                                    tags,
                                                                    contactid,
                                                                    stageType,
                                                                    contactAlsoIsParticipant,
                                                                    fromDate,
                                                                    toDate);

            _context.SetTotalCount(totalCount);

            return result.ToSmartList();
        }

        /// <summary>
        ///    Deletes the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <short>Delete opportunity</short> 
        /// <category>Opportunities</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Opportunity
        /// </returns>
        [Delete(@"opportunity/{opportunityid:[0-9]+}")]
        public OpportunityWrapper DeleteDeal(int opportunityid)
        {
            if (opportunityid <= 0)
                throw new ArgumentException();

            var opportunity = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (opportunity == null)
                throw new ItemNotFoundException();

            var result = ToOpportunityWrapper(opportunity);

            DaoFactory.GetDealDao().DeleteDeal(opportunityid);

            return result;
        }

        /// <summary>
        ///    Creates the opportunity with the parameters specified in the request
        /// </summary>
        /// <short>Create opportunity</short> 
        /// <param name="contactid">Opportunity primary contact</param>
        /// <param optional="true" name="members">Participants</param>
        /// <param name="title">Opportunity title</param>
        /// <param optional="true" name="description">Opportunity description</param>
        /// <param name="responsibleid">Opportunity responsible</param>
        /// <param name="bidType" remark="Allowed values: FixedBid, PerHour, PerDay,PerWeek, PerMonth, PerYear">Bid</param>
        /// <param optional="true" name="bidValue">Amount of transaction</param>
        /// <param optional="true" name="bidCurrencyAbbr">Currency (Abbreviation)</param>
        /// <param name="perPeriodValue">Period</param>
        /// <param name="stageid">Stage ID</param>
        /// <param optional="true" name="successProbability">Opportunity success probability</param>
        /// <param optional="true" name="actualCloseDate">Actual opportunity closure date</param>
        /// <param optional="true" name="expectedCloseDate">Expected opportunity closure date</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param name="isPrivate">Opportunity privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access to the opportunity</param>
        /// <category>Opportunities</category>
        /// <returns>
        ///  Opportunity
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Create(@"opportunity")]
        public OpportunityWrapper CreateDeal(
                                      int contactid,
                                      IEnumerable<int> members,
                                      String title,
                                      String description,
                                      Guid responsibleid,
                                      BidType bidType,
                                      decimal bidValue,
                                      String bidCurrencyAbbr,
                                      int perPeriodValue,
                                      int stageid,
                                      int successProbability,
                                      ApiDateTime actualCloseDate,
                                      ApiDateTime expectedCloseDate,
                                      IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                      bool isPrivate,
                                      IEnumerable<Guid> accessList)
        {

            if (String.IsNullOrEmpty(title) || responsibleid == Guid.Empty)
                throw new ArgumentException();

            var deal = new Deal
                           {

                               Title = title,
                               Description = description,
                               ResponsibleID = responsibleid,
                               BidType = bidType,
                               BidValue = bidValue,
                               PerPeriodValue = perPeriodValue,
                               DealMilestoneID = stageid,
                               DealMilestoneProbability = successProbability,
                               ContactID = contactid,
                               ActualCloseDate = actualCloseDate,
                               ExpectedCloseDate = expectedCloseDate,
                               BidCurrency = bidCurrencyAbbr,
                                                              
                           };

            if (String.IsNullOrEmpty(deal.BidCurrency))
                deal.BidCurrency = Global.TenantSettings.DefaultCurrency.Abbreviation;

            deal.ID = DaoFactory.GetDealDao().CreateNewDeal(deal);

            deal.CreateBy = ASC.Core.SecurityContext.CurrentAccount.ID;
            deal.CreateOn = DateTime.UtcNow;

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(deal, accessListLocal);
            else
                CRMSecurity.MakePublic(deal);

            if (members != null && members.Count() > 0)
                DaoFactory.GetDealDao().SetMembers(deal.ID, members.ToArray());

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Opportunity, deal.ID, field.Key, field.Value);
            }

            return ToOpportunityWrapper(deal);

        }


        /// <summary>
        ///    Updates the selected opportunity with the parameters specified in the request
        /// </summary>
        /// <short>Update opportunity</short>
        ///<param name="opportunityid">Opportunity ID</param>
        ///<param name="contactid">Opportunity primary contact</param>
        /// <param optional="true" name="members">Participants</param>
        /// <param name="title">Opportunity title</param>
        /// <param optional="true" name="description">Opportunity description</param>
        /// <param name="responsibleid">Opportunity responsible</param>
        /// <param name="bidType" remark="Allowed values: FixedBid, PerHour, PerDay,PerWeek, PerMonth, PerYear">Bid</param>
        /// <param optional="true" name="bidValue">Amount of transaction</param>
        /// <param optional="true" name="bidCurrencyAbbr">Currency (Abbreviation)</param>
        /// <param name="perPeriodValue">Period</param>
        /// <param name="stageid">Stage ID</param>
        /// <param optional="true" name="successProbability">Opportunity success probability</param>
        /// <param optional="true" name="actualCloseDate">Actual opportunity closure date</param>
        /// <param optional="true" name="expectedCloseDate">Expected opportunity closure date</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param name="isPrivate">Opportunity privacy: private or not</param>
        /// <param optional="true" name="accessList">List of users with access to the opportunity</param>
        /// <category>Opportunities</category>
        /// <returns>
        ///  Opportunity
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Update(@"opportunity/{opportunityid:[0-9]+}")]
        public OpportunityWrapper UpdateDeal(
                                      int opportunityid,
                                      int contactid,
                                      IEnumerable<int> members,
                                      String title,
                                      String description,
                                      Guid responsibleid,
                                      BidType bidType,
                                      decimal bidValue,
                                      String bidCurrencyAbbr,
                                      int perPeriodValue,
                                      int stageid,
                                      int successProbability,
                                      ApiDateTime actualCloseDate,
                                      ApiDateTime expectedCloseDate,
                                      IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                      bool isPrivate,
                                      IEnumerable<Guid> accessList)
        {

            if (String.IsNullOrEmpty(title) || responsibleid == Guid.Empty)
                throw new ArgumentException();

            var deal = new Deal
            {
                ID = opportunityid,
                Title = title,
                Description = description,
                ResponsibleID = responsibleid,
                BidType = bidType,
                BidValue = bidValue,
                PerPeriodValue = perPeriodValue,
                DealMilestoneID = stageid,
                DealMilestoneProbability = successProbability,
                ContactID = contactid,
                ActualCloseDate = actualCloseDate,
                ExpectedCloseDate = expectedCloseDate
            };

            if (String.IsNullOrEmpty(deal.BidCurrency))
                deal.BidCurrency = Global.TenantSettings.DefaultCurrency.Abbreviation;
            
            DaoFactory.GetDealDao().EditDeal(deal);

            deal = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (members != null && members.Count() > 0)
                DaoFactory.GetDealDao().SetMembers(deal.ID, members.ToArray());

            var accessListLocal = accessList.ToList();

            if (isPrivate && accessListLocal.Count > 0)
                CRMSecurity.SetAccessTo(deal, accessListLocal);
            else
                CRMSecurity.MakePublic(deal);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Opportunity, deal.ID, field.Key, field.Value);
            }

            return ToOpportunityWrapper(deal);

        }



        /// <summary>
        ///    Returns the list of all contacts associated with the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <short>Get all opportunity contacts</short> 
        /// <category>Opportunities</category>
        /// <returns>Contact list</returns>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        [Read(@"opportunity/{opportunityid:[0-9]+}/contact")]
        public IEnumerable<ContactWrapper> GetDealMembers(int opportunityid)
        {

            var opporunity = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (opporunity == null)
                throw new ItemNotFoundException();

            var contactIDs = DaoFactory.GetDealDao().GetMembers(opportunityid);

            if (contactIDs == null)
                return new ItemList<ContactWrapper>();

            var result = ToListContactWrapper(DaoFactory.GetContactDao().GetContacts(contactIDs)).ToList();

            result.ForEach(item =>
                               {
                                   if (item.ID == opporunity.ContactID)
                                       item.CanEdit = false;
                               });

            return result;

        }

        /// <summary>
        ///   Adds the selected contact to the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <short>Add opportunity contact</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Participant
        /// </returns>
        [Create(@"opportunity/{opportunityid:[0-9]+}/contact/{contactid:[0-9]+}")]
        public ContactWrapper AddMemberToDeal(int opportunityid, int contactid)
        {
            if ((opportunityid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var result = ToContactWrapper(contact);

            DaoFactory.GetDealDao().AddMember(opportunityid, contactid);

            return result;

        }

        /// <summary>
        ///   Deletes the selected contact from the opportunity with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <short>Delete opportunity contact</short> 
        /// <category>Opportunities</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Participant
        /// </returns>
        [Delete(@"opportunity/{opportunityid:[0-9]+}/contact/{contactid:[0-9]+}")]
        public ContactWrapper DeleteMemberFromDeal(int opportunityid, int contactid)
        {
            if ((opportunityid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var result = ToContactWrapper(contact);

            DaoFactory.GetDealDao().RemoveMember(opportunityid, contactid);

            return result;
        }

        /// <summary>
        ///    Returns the list of 30 opportunities in the CRM module with prefix
        /// </summary>
        /// <param optional="true" name="prefix"></param>
        /// <param optional="true" name="contactID"></param>
        /// <category>Opportunities</category>
        /// <returns>
        ///    Opportunities list
        /// </returns>
        /// <visible>false</visible>
        [Read(@"opportunity/byprefix")]
        public IEnumerable<OpportunityWrapper> GetDealsByPrefix(string prefix, int contactID)
        {
            List<OpportunityWrapper> result = new List<OpportunityWrapper>();

            if (contactID > 0)
            {
                var findedDeals = DaoFactory.GetDealDao().GetDealsByContactID(contactID);

                foreach (var item in findedDeals)
                {
                    if (item.Title.IndexOf(prefix) != -1)
                        result.Add(ToOpportunityWrapper(item));
                }

                _context.SetTotalCount(findedDeals.Count);
            }
            else
            {
                var maxItemCount = 30;
                var findedDeals = Global.DaoFactory.GetDealDao().GetDealsByPrefix(prefix, 0, maxItemCount);
                foreach (var item in findedDeals)
                {
                    result.Add(ToOpportunityWrapper(item));
                }
            }

            return result;
        }

        private IEnumerable<OpportunityWrapper> ToListOpportunityWrapper(ICollection<Deal> deals)
        {
            if (deals == null || deals.Count == 0) return new List<OpportunityWrapper>();

            var result = new List<OpportunityWrapper>();

            var contactIDs = new List<int>();
            var dealIDs = new List<int>();
            var dealMilestoneIDs = new List<int>();

            foreach (var deal in deals)
            {
                contactIDs.Add(deal.ContactID);
                dealIDs.Add(deal.ID);
                dealMilestoneIDs.Add(deal.DealMilestoneID);
            }

            dealMilestoneIDs = dealMilestoneIDs.Distinct().ToList();

            var contacts = new Dictionary<int, ContactBaseWrapper>();

            var customFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, dealIDs.ToArray())
                             .GroupBy(item => item.EntityID)
                             .ToDictionary(item => item.Key, item => item.Select(x => ToCustomFieldBaseWrapper(x)));

            var dealMilestones = DaoFactory.GetDealMilestoneDao().GetAll(dealMilestoneIDs.ToArray())
                               .ToDictionary(item => item.ID, item => new DealMilestoneBaseWrapper(item));


            var dealMembers = DaoFactory.GetDealDao().GetMembers(dealIDs.ToArray());

            foreach (var value in dealMembers.Values)
                contactIDs.AddRange(value);

            contactIDs = contactIDs.Distinct().ToList();

            if (contactIDs.Count > 0)
                DaoFactory.GetContactDao().GetContacts(contactIDs.ToArray()).ForEach(item =>
                                                                               {
                                                                                   if (item == null) return;

                                                                                   contacts.Add(item.ID, ToContactBaseWrapper(item));
                                                                               });

            foreach (var deal in deals)
            {

                var dealWrapper = new OpportunityWrapper(deal);

                if (contacts.ContainsKey(deal.ContactID))
                    dealWrapper.Contact = contacts[deal.ContactID];

                if (customFields.ContainsKey(deal.ID))
                    dealWrapper.CustomFields = customFields[deal.ID];
                else
                    dealWrapper.CustomFields = new List<CustomFieldBaseWrapper>();

                if (dealMembers.ContainsKey(dealWrapper.ID))
                    dealWrapper.Members = dealMembers[dealWrapper.ID].Where(contacts.ContainsKey).Select(item => contacts[item]);
                else
                    dealWrapper.Members = new List<ContactBaseWrapper>();

                if (dealMilestones.ContainsKey(deal.DealMilestoneID))
                    dealWrapper.Stage = dealMilestones[deal.DealMilestoneID];

                dealWrapper.IsPrivate = CRMSecurity.IsPrivate(deal);

                if (dealWrapper.IsPrivate)
                    dealWrapper.AccessList = CRMSecurity.GetAccessSubjectTo(deal)
                                      .Select(item => EmployeeWraper.Get(item.Key)).ToItemList();



                if (!String.IsNullOrEmpty(deal.BidCurrency))
                    dealWrapper.BidCurrency = ToCurrencyInfoWrapper(CurrencyProvider.Get(deal.BidCurrency));

                result.Add(dealWrapper);
            }

            return result;
        }

        private OpportunityWrapper ToOpportunityWrapper(Deal deal)
        {
            var dealWrapper = new OpportunityWrapper(deal);

            if (deal.ContactID > 0)
                dealWrapper.Contact = ToContactBaseWrapper(DaoFactory.GetContactDao().GetByID(deal.ContactID));

            if (deal.DealMilestoneID > 0)
            {
                var dealMilestone = DaoFactory.GetDealMilestoneDao().GetByID(deal.DealMilestoneID);

                if (dealMilestone == null)
                    throw new ItemNotFoundException();

                dealWrapper.Stage =  new DealMilestoneBaseWrapper(dealMilestone);
            }

            dealWrapper.AccessList = CRMSecurity.GetAccessSubjectTo(deal)
                                     .Select(item => EmployeeWraper.Get(item.Key)).ToItemList();

            dealWrapper.IsPrivate = CRMSecurity.IsPrivate(deal);

            if (!String.IsNullOrEmpty(deal.BidCurrency))
                dealWrapper.BidCurrency = ToCurrencyInfoWrapper(CurrencyProvider.Get(deal.BidCurrency));

            dealWrapper.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, deal.ID, false).ConvertAll(item => new CustomFieldBaseWrapper(item)).ToSmartList();

            dealWrapper.Members = new List<ContactBaseWrapper>();

            var memberIDs = DaoFactory.GetDealDao().GetMembers(deal.ID);
            var membersList = DaoFactory.GetContactDao().GetContacts(memberIDs);
            var membersWrapperList = new List<ContactBaseWrapper>();

            foreach (var member in membersList)
            {
                if (member == null) continue;
                membersWrapperList.Add(ToContactBaseWrapper(member));
            }

            dealWrapper.Members = membersWrapperList;

            return dealWrapper;
        }
    }
}
