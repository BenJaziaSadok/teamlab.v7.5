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
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Classes;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///     Returns the list of all currencies currently available on the portal
        /// </summary>
        /// <short>Get currency list</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///    List of available currencies
        /// </returns>
        [Read(@"settings/currency")]
        public IEnumerable<CurrencyInfoWrapper> GetAvaliableCurrency()
        {
            return CurrencyProvider.GetAll().ConvertAll(item => new CurrencyInfoWrapper(item)).ToItemList();
        }


        /// <summary>
        ///     Returns the result of convertation from one currency to another
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromcurrency">Old currency key</param>
        /// <param name="tocurrency">New currency key</param>
        /// <short>Get the result of convertation</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///    Decimal result of convertation
        /// </returns>
        [Read(@"settings/currency/convert")]
        public Decimal ConvertAmount(Decimal amount, String fromcurrency, String tocurrency)
        {
            return CurrencyProvider.MoneyConvert(amount, fromcurrency, tocurrency);
        }


        /// <summary>
        ///     Returns the summary table with rates for selected currency
        /// </summary>
        /// <param name="currency">currency</param>
        /// <short>Get the summary table</short> 
        /// <category>Opportunities</category>
        /// <returns>
        ///    Dictionary of currencies and rates
        /// </returns>
        [Read(@"settings/currency/{currency}/summarytable")]
        public IEnumerable<CurrencyRateInfoWrapper> GetSummaryTable(String currency)
        {
            var result = new List<CurrencyRateInfoWrapper>();
            var table = CurrencyProvider.MoneyConvert(CurrencyProvider.Get(currency));
            table.ToList().ForEach(tableItem => result.Add(ToCurrencyRateInfoWrapper(tableItem.Key, tableItem.Value)));
            return result;
        }

        [Update(@"contact/status/settings")]
        public Boolean? UpdateCRMContactStatusSettings(Boolean? changeContactStatusGroupAuto)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.ChangeContactStatusGroupAuto = changeContactStatusGroupAuto;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            return changeContactStatusGroupAuto;
        }

        [Update(@"contact/tag/settings")]
        public Boolean? UpdateCRMContactTagSettings(Boolean? addTagToContactGroupAuto)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.AddTagToContactGroupAuto = addTagToContactGroupAuto;

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            return addTagToContactGroupAuto;
        }

        [Update(@"settings")]
        public void SetIsPortalConfigured(Boolean? configured, Guid? webFormKey)
        {
            var tenantSettings = Global.TenantSettings;
            tenantSettings.IsConfiguredPortal = configured ?? true;
            tenantSettings.WebFormKey = webFormKey ?? Guid.NewGuid();
            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);
        }

        protected CurrencyInfoWrapper ToCurrencyInfoWrapper(CurrencyInfo currencyInfo)
        {
            if (currencyInfo ==  null) return null;

            return new CurrencyInfoWrapper(currencyInfo);
        }

        protected CurrencyRateInfoWrapper ToCurrencyRateInfoWrapper(CurrencyInfo currencyInfo, Decimal rate)
        {
            if (currencyInfo ==  null) return null;

            return new CurrencyRateInfoWrapper(currencyInfo, rate);
        }
    }
}
