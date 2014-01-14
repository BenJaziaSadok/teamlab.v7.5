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
using System.Globalization;
using System.Linq;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Core.Users;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core.Users;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Classes
{
    public partial class ImportDataOperation
    {

        private void ImportOpportunityData()
        {
            var allUsers = ASC.Core.CoreContext.UserManager.GetUsers(EmployeeStatus.All).ToList();

            using (var CSVFileStream = _dataStore.GetReadStream("temp", _CSVFileURI))
            using (CsvReader csv = ImportFromCSV.CreateCsvReaderInstance(CSVFileStream, _importSettings))
            {
                int currentIndex = 0;

                var customFieldDao = _daoFactory.GetCustomFieldDao();
                var contactDao = _daoFactory.GetContactDao();
                var tagDao = _daoFactory.GetTagDao();
                var dealDao = _daoFactory.GetDealDao();
                var dealMilestoneDao = _daoFactory.GetDealMilestoneDao();

                var findedTags = new Dictionary<int, List<String>>();
                var findedCustomField = new List<CustomField>();
                var findedDeals = new List<Deal>();
                var findedDealMembers = new Dictionary<int, List<int>>();

                var dealMilestones = dealMilestoneDao.GetAll();

                while (csv.ReadNextRecord())
                {
                    _columns = csv.GetCurrentRowFields(false);

                    var obj = new Deal();

                    obj.ID = currentIndex;

                    obj.Title = GetPropertyValue("title");

                    if (String.IsNullOrEmpty(obj.Title)) continue;

                    obj.Description = GetPropertyValue("description");

                    var csvResponsibleValue = GetPropertyValue("responsible");
                    var responsible = allUsers.Where(n => n.DisplayUserName().Equals(csvResponsibleValue)).FirstOrDefault();

                    if (responsible != null)
                        obj.ResponsibleID = responsible.ID;
                    else
                        obj.ResponsibleID = Constants.LostUser.ID;

                    DateTime actualCloseDate;

                    DateTime expectedCloseDate;

                    if (DateTime.TryParse(GetPropertyValue("actual_close_date"), out actualCloseDate))
                        obj.ActualCloseDate = actualCloseDate;

                    if (DateTime.TryParse(GetPropertyValue("expected_close_date"), out expectedCloseDate))
                        obj.ExpectedCloseDate = expectedCloseDate;

                    var currency = CurrencyProvider.Get(GetPropertyValue("bid_currency"));

                    if (currency != null)
                        obj.BidCurrency = currency.Abbreviation;
                    else
                        obj.BidCurrency = Global.TenantSettings.DefaultCurrency.Abbreviation;

                    decimal bidValue;

                    var bidValueStr = GetPropertyValue("bid_amount");

                    if (Decimal.TryParse(bidValueStr, out  bidValue))
                        obj.BidValue = bidValue;
                    else
                        obj.BidValue = 0;


                    var bidTypeStr = GetPropertyValue("bid_type");

                    BidType bidType = BidType.FixedBid;

                    if (!String.IsNullOrEmpty(bidTypeStr))
                    {
                        if (String.Compare(CRMDealResource.BidType_FixedBid, bidTypeStr, true) == 0)
                            bidType = BidType.FixedBid;
                        else if (String.Compare(CRMDealResource.BidType_PerDay, bidTypeStr, true) == 0)
                            bidType = BidType.PerDay;
                        else if (String.Compare(CRMDealResource.BidType_PerHour, bidTypeStr, true) == 0)
                            bidType = BidType.PerHour;
                        else if (String.Compare(CRMDealResource.BidType_PerMonth, bidTypeStr, true) == 0)
                            bidType = BidType.PerMonth;
                        else if (String.Compare(CRMDealResource.BidType_PerWeek, bidTypeStr, true) == 0)
                            bidType = BidType.PerWeek;
                        else if (String.Compare(CRMDealResource.BidType_PerYear, bidTypeStr, true) == 0)
                            bidType = BidType.PerYear;
                    }

                    obj.BidType = bidType;

                    if (obj.BidType != BidType.FixedBid)
                    {
                        int perPeriodValue;

                        if (int.TryParse(GetPropertyValue("per_period_value"), out perPeriodValue))
                            obj.PerPeriodValue = perPeriodValue;
                    }

                    int probabilityOfWinning;

                    if (int.TryParse(GetPropertyValue("probability_of_winning"), out probabilityOfWinning))
                        obj.DealMilestoneProbability = probabilityOfWinning;

                    var dealMilestoneTitle = GetPropertyValue("deal_milestone");

                    var tag = GetPropertyValue("tag");

                    if (!String.IsNullOrEmpty(tag))
                        findedTags.Add(obj.ID, tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                    if (String.IsNullOrEmpty(dealMilestoneTitle))
                        obj.DealMilestoneID = dealMilestones[0].ID;
                    else
                    {
                        var dealMilestone = dealMilestones.Find(item => String.Compare(item.Title, dealMilestoneTitle, true) == 0);

                        if (dealMilestone == null)
                            obj.DealMilestoneID = dealMilestones[0].ID;
                        else
                            obj.DealMilestoneID = dealMilestone.ID;
                    }

                    var contactName = GetPropertyValue("client");

                    var localMembersDeal = new List<int>();

                    if (!String.IsNullOrEmpty(contactName))
                    {
                        var contacts = contactDao.GetContactsByName(contactName);

                        if (contacts.Count > 0)
                        {

                            obj.ContactID = contacts[0].ID;

                            localMembersDeal.Add(obj.ContactID);

                        }
                    }

                    var members = GetPropertyValue("member");

                    if (!String.IsNullOrEmpty(members))
                    {
                        var membersList = members.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in membersList)
                        {
                            var findedMember = contactDao.GetContactsByName(item);

                            if (findedMember.Count > 0)
                                localMembersDeal.Add(findedMember[0].ID);
                        }

                    }

                    if (localMembersDeal.Count > 0)
                        findedDealMembers.Add(obj.ID, localMembersDeal);







                    foreach (JProperty jToken in _importSettings.ColumnMapping.Children())
                    {
                        var propertyValue = GetPropertyValue(jToken.Name);

                        if (String.IsNullOrEmpty(propertyValue)) continue;

                        if (!jToken.Name.StartsWith("customField_")) continue;

                        var fieldID = Convert.ToInt32(jToken.Name.Split(new[] { '_' })[1]);

                        findedCustomField.Add(new CustomField
                        {
                            EntityID = obj.ID,
                            EntityType = EntityType.Opportunity,
                            ID = fieldID,
                            Value = propertyValue
                        });
                    }

                    Percentage += 1.0 * 100 / (ImportFromCSV.MaxRoxCount * 2);



                    findedDeals.Add(obj);

                    if (currentIndex + 1 > ImportFromCSV.MaxRoxCount) break;

                    currentIndex++;

                }


                Percentage = 50;

                var newDealIDs = dealDao.SaveDealList(findedDeals);

                Percentage += 12.5;

                findedCustomField.ForEach(item => item.EntityID = newDealIDs[item.EntityID]);

                customFieldDao.SaveList(findedCustomField);

                Percentage += 12.5;

                foreach (var findedDealMemberKey in findedDealMembers.Keys)
                {
                    dealDao.SetMembers(newDealIDs[findedDealMemberKey], findedDealMembers[findedDealMemberKey].ToArray());
                }

                Percentage += 12.5;

                foreach (var findedTagKey in findedTags.Keys)
                {
                    tagDao.SetTagToEntity(EntityType.Opportunity, newDealIDs[findedTagKey], findedTags[findedTagKey].ToArray());
                }

                if (_importSettings.IsPrivate)
                    findedDeals.ForEach(dealItem => CRMSecurity.SetAccessTo(dealItem, _importSettings.AccessList));

                Percentage += 12.5;
            }

            Complete();
        }

    }
}