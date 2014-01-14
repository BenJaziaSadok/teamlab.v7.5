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
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Resources;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Classes
{
    public partial class ImportDataOperation
    {
        private void ImportCaseData()
        {
            using (var CSVFileStream = _dataStore.GetReadStream("temp", _CSVFileURI))
            using (CsvReader csv = ImportFromCSV.CreateCsvReaderInstance(CSVFileStream, _importSettings))
            {
                var casesDao = _daoFactory.GetCasesDao();
                var customFieldDao = _daoFactory.GetCustomFieldDao();
                var tagDao = _daoFactory.GetTagDao();

                var findedTags = new Dictionary<int, List<String>>();
                var findedCustomField = new List<CustomField>();
                var findedCases = new List<ASC.CRM.Core.Entities.Cases>();
                var findedCasesMembers = new Dictionary<int, List<int>>();


                int currentIndex = 0;

                while (csv.ReadNextRecord())
                {
                    _columns = csv.GetCurrentRowFields(false);

                    var objCases = new ASC.CRM.Core.Entities.Cases();

                    objCases.ID = currentIndex;

                    objCases.Title = GetPropertyValue("title");

                    if (String.IsNullOrEmpty(objCases.Title)) continue;

                    foreach (JProperty jToken in _importSettings.ColumnMapping.Children())
                    {
                        var propertyValue = GetPropertyValue(jToken.Name);

                        if (String.IsNullOrEmpty(propertyValue)) continue;

                        if (!jToken.Name.StartsWith("customField_")) continue;

                        var fieldID = Convert.ToInt32(jToken.Name.Split(new[] { '_' })[1]);

                        findedCustomField.Add(new CustomField
                        {
                            EntityID = objCases.ID,
                            EntityType = EntityType.Case,
                            ID = fieldID,
                            Value = propertyValue
                        });
                    }

                    var tag = GetPropertyValue("tag");

                    if (!String.IsNullOrEmpty(tag))
                        findedTags.Add(objCases.ID, tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());

                    var localMembersCases = new List<int>();

                    var members = GetPropertyValue("member");

                    if (!String.IsNullOrEmpty(members))
                    {
                        var membersList = members.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var item in membersList)
                        {
                            var findedMember = _daoFactory.GetContactDao().GetContactsByName(item);

                            if (findedMember.Count > 0)
                                localMembersCases.Add(findedMember[0].ID);
                        }
                    }

                    if (localMembersCases.Count > 0)
                        findedCasesMembers.Add(objCases.ID, localMembersCases);

                    objCases.ID = currentIndex;

                    findedCases.Add(objCases);

                    if (currentIndex + 1 > ImportFromCSV.MaxRoxCount) break;

                    currentIndex++;
                }

                Percentage = 62.5;

                var newIDs = casesDao.SaveCasesList(findedCases);

                findedCustomField.ForEach(item => item.EntityID = newIDs[item.EntityID]);

                customFieldDao.SaveList(findedCustomField);

                Percentage += 12.5;

                foreach (var findedCasesMemberKey in findedCasesMembers.Keys)
                {
                    _daoFactory.GetDealDao().SetMembers(newIDs[findedCasesMemberKey], findedCasesMembers[findedCasesMemberKey].ToArray());
                }

                Percentage += 12.5;

                foreach (var findedTagKey in findedTags.Keys)
                {
                    tagDao.SetTagToEntity(EntityType.Case, newIDs[findedTagKey], findedTags[findedTagKey].ToArray());
                }

                if (_importSettings.IsPrivate)
                    findedCases.ForEach(dealItem => CRMSecurity.SetAccessTo(dealItem, _importSettings.AccessList));


                Percentage += 12.5;
            }

            Complete();
        }
    }
}