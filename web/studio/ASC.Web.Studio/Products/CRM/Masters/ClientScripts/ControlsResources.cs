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
using System.Linq;
using System.Collections.Generic;
using System.Web;
using ASC.Web.CRM.Resources;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Core;
using ASC.Web.CRM.Configuration;

namespace ASC.Web.CRM.Masters.ClientScripts
{

    #region Data for Common Data

    public class CommonData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("ProfileRemoved", ASC.Core.Users.Constants.LostUser.DisplayUserName());
        }
    }

    #endregion 


    #region Data for SmtpSender

    public class SmtpSenderData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("smtpSettings", Global.TenantSettings.SMTPServerSetting);
        }
    }

    #endregion  

    #region classes for Contact Views

    public class ListContactViewData : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus);
            var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact).ToList();

            var contactStages = listItems.ConvertAll(item => new
            {
                value = item.ID,
                title = item.Title.HtmlEncode(),
                classname = "colorFilterItem color_" + item.Color.Replace("#", "").ToLower()
            });
            contactStages.Insert(0, new
            {
                value = 0,
                title = CRMCommonResource.NotSpecified,
                classname = "colorFilterItem color_0"
            });


            listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactType);
            var contactTypes = listItems.ConvertAll(item => new
            {
                value = item.ID,
                title = item.Title.HtmlEncode()
            });
             contactTypes.Insert(0, new
            {
                value = 0,
                title = CRMContactResource.CategoryNotSpecified,
            });
           
            yield return RegisterObject("contactStages", contactStages);
            yield return RegisterObject("contactTypes", contactTypes);
            yield return RegisterObject("contactTags", tags.ConvertAll(item => new
                                                        {
                                                        value = item.HtmlEncode(),
                                                        title = item.HtmlEncode()
                                                        }));
            yield return RegisterObject("smtpSettings", Global.TenantSettings.SMTPServerSetting);
        }
    }

    #endregion

    #region classes for Task Views

    public class ListTaskViewData : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {

            var taskCategories = Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory);
           
            yield return RegisterObject("taskCategories", 
                                        taskCategories.ConvertAll(item => new
                                        {
                                            value = item.ID,
                                            title = item.Title.HtmlEncode(),
                                            cssClass = "task_category " + item.AdditionalParams.Split('.').FirstOrDefault()
                                        })
                
                );
        }
    }

    public class TaskActionViewData : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList<UserInfo>();
            admins.AddRange(WebItemSecurity.GetProductAdministrators(ProductEntryPoint.ID).ToList());
            admins = admins.Distinct().ToList();
            admins = admins.SortByUserName();

            var taskCategories = Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory);

            yield return RegisterObject("adminList",
                                        admins.ConvertAll(item => new
                                        {
                                            avatarSmall = item.GetSmallPhotoURL(),
                                            displayName = item.DisplayUserName(),
                                            id = item.ID,
                                            title = item.Title.HtmlEncode()
                                        }));
            yield return RegisterObject("taskActionViewCategories",
                                        taskCategories.ConvertAll (item => new
                                        {
                                            id = item.ID,
                                            title = item.Title.HtmlEncode(),
                                            cssClass = "task_category " + item.AdditionalParams.Split('.').FirstOrDefault()
                                        }));

        }
    }

    #endregion

    #region classes for Cases Views

    public class ListCasesViewData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Case).ToList();

            yield return RegisterObject("caseTags", tags.ConvertAll(item => new
                                                    {
                                                        value = item.HtmlEncode(),
                                                        title = item.HtmlEncode()
                                                    }));
        }
    }

    #endregion

    #region classes for Opportunity Views

    public class ListDealViewData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var dealMilestones = Global.DaoFactory.GetDealMilestoneDao().GetAll();
            var tags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Opportunity).ToList();

            yield return RegisterObject("dealMilestones", dealMilestones.ConvertAll(
                                                            item => new
                                                            {
                                                                value = item.ID,
                                                                title = item.Title,
                                                                classname = "colorFilterItem color_" + item.Color.Replace("#", "").ToLower()
                                                            }));
            yield return RegisterObject("dealTags", tags.ConvertAll(
                                                            item => new
                                                            {
                                                                value = item.HtmlEncode(),
                                                                title = item.HtmlEncode()
                                                            }));
        }
    }

    public class ExchangeRateViewData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var defaultCurrency = Global.TenantSettings.DefaultCurrency;
            var publisherDate = CurrencyProvider.GetPublisherDate;

            yield return RegisterObject("defaultCurrency", defaultCurrency);
            yield return RegisterObject("ratesPublisherDisplayDate", String.Format("{0} {1}", publisherDate.ToShortDateString(), publisherDate.ToShortTimeString()));
        }
    }

    #endregion

    #region Data for History View

    public class HistoryViewData: ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Data"; }
        }

        protected override string GetCacheHash()
        {
            return Guid.NewGuid().ToString();
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var eventsCategories = Global.DaoFactory.GetListItemDao().GetItems(ListType.HistoryCategory);
            var systemCategories = Global.DaoFactory.GetListItemDao().GetSystemItems();

            yield return RegisterObject("eventsCategories", eventsCategories.ConvertAll
                                                        (item => new
                                                        {
                                                            id = item.ID,
                                                            value = item.ID,
                                                            title = item.Title.HtmlEncode(),
                                                            cssClass = "event_category " + item.AdditionalParams.Split('.').FirstOrDefault()
                                                        }
                                                        ));
            yield return RegisterObject("systemCategories", systemCategories.ConvertAll
                                                        (item => new
                                                        {
                                                            id = item.ID,
                                                            value = item.ID,
                                                            title = item.Title.HtmlEncode(),
                                                            cssClass = "event_category " + item.AdditionalParams.Split('.').FirstOrDefault()
                                                        }
                                                        ));
            yield return RegisterObject("historyEntityTypes", new[] {
                                                            new {
                                                                value = (int)EntityType.Opportunity,
                                                                displayname = CRMDealResource.Deal,
                                                                apiname = EntityType.Opportunity.ToString().ToLower()
                                                            },
                                                            new {
                                                                value = (int)EntityType.Case,
                                                                displayname = CRMCasesResource.Case,
                                                                apiname = EntityType.Case.ToString().ToLower()
                                                            }
                                                        });
        }
    }

    #endregion

}