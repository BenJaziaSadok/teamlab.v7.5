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

using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;

namespace ASC.Web.Studio.Core.SMS
{
    [Serializable]
    [DataContract]
    public class StudioSmsNotificationSettings : ISettings
    {
        private static Dictionary<string, TenantQuota> smsProducts;


        public Guid ID
        {
            get { return new Guid("{2802df61-af0d-40d4-abc5-a8506a5352ff}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioSmsNotificationSettings { EnableSetting = false, };
        }

        [DataMember(Name = "Enable")]
        public bool EnableSetting { get; set; }


        public static bool Enable
        {
            get
            {
                return SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID).EnableSetting && SentSms < PaidSms;
            }
            set
            {
                var settings = SettingsManager.Instance.LoadSettings<StudioSmsNotificationSettings>(TenantProvider.CurrentTenantID);
                settings.EnableSetting = value;
                SettingsManager.Instance.SaveSettings(settings, CurrentTenant());
            }
        }

        public static int SentSms
        {
            get
            {
                var q = new TenantQuotaRowQuery(CoreContext.TenantManager.GetCurrentTenant().TenantId) { Path = "/sms", };
                var row = CoreContext.TenantManager.FindTenantQuotaRows(q).FirstOrDefault();
                return row != null ? (int)row.Counter : 0;
            }
        }

        public static int PaidSms
        {
            get
            {
                if (smsProducts == null)
                {
                    smsProducts = CoreContext.TenantManager.GetTenantQuotas(true).Where(q => q.GetFeature("sms-product")).ToDictionary(q => q.AvangateId, q => q);
                }

                var paid = 0;
                foreach (var payment in CoreContext.PaymentManager.GetTariffPayments(CurrentTenant()))
                {
                    TenantQuota product;
                    if (smsProducts.TryGetValue(payment.ProductId, out product))
                    {
                        paid += product.ActiveUsers;
                    }
                }
                return paid + CoreContext.TenantManager.GetTenantQuota(CurrentTenant()).ActiveUsers * int.Parse(ConfigurationManager.AppSettings["web.sms-count"] ?? "2");
            }
        }

        public static bool IsVisibleSettings
        {
            get
            {
                return SetupInfo.IsVisibleSettings<StudioSmsNotificationSettings>() && TenantExtra.GetTenantQuota().Sms;
            }
        }

        public static void IncrementSentSms()
        {
            CoreContext.TenantManager.SetTenantQuotaRow(new TenantQuotaRow { Tenant = CurrentTenant(), Path = "/sms", Counter = 1 }, true);
        }


        private static int CurrentTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}