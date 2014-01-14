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
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;

#endregion

namespace ASC.Web.CRM.Classes
{
    [Serializable]
    [DataContract]
    public class SMTPServerSetting
    {

        public SMTPServerSetting()
        {
            Host = String.Empty;
            Port = 0;
            EnableSSL = false;
            RequiredHostAuthentication = false;
            HostLogin = String.Empty;
            HostPassword = String.Empty;
            SenderDisplayName = String.Empty;
            SenderEmailAddress = String.Empty;
        }

        [DataMember]
        public String Host { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public bool EnableSSL { get; set; }

        [DataMember]
        public bool RequiredHostAuthentication { get; set; }

        [DataMember]
        public String HostLogin { get; set; }

        [DataMember]
        public String HostPassword { get; set; }

        [DataMember]
        public String SenderDisplayName { get; set; }

        [DataMember]
        public String SenderEmailAddress { get; set; }

    }

    [Serializable]
    [DataContract]
    public class CRMSettings : ISettings
    {
        [DataMember(Name = "DefaultCurrency")]
        private string defaultCurrency;

        [DataMember]
        public SMTPServerSetting SMTPServerSetting { get; set; }

        [DataMember]
        public Guid WebFormKey { get; set; }

        public Guid ID
        {
            get { return new Guid("fdf39b9a-ec96-4eb7-aeab-63f2c608eada"); }
        }

        public CurrencyInfo DefaultCurrency
        {
            get { return CurrencyProvider.Get(defaultCurrency); }
            set { defaultCurrency = value.Abbreviation; }
        }

        [DataMember(Name = "ChangeContactStatusGroupAuto")]
        public Boolean? ChangeContactStatusGroupAuto { get; set; }

        [DataMember(Name = "AddTagToContactGroupAuto")]
        public Boolean? AddTagToContactGroupAuto { get; set; }

        [DataMember(Name = "IsConfiguredPortal")]
        public bool IsConfiguredPortal { get; set; }

        public ISettings GetDefault()
        {

            var languageName =  System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var findedCurrency =
                CurrencyProvider.GetAll().Find(item => String.Compare(item.CultureName, languageName, true) == 0);

            if (findedCurrency != null)
                return new CRMSettings
                           {
                               defaultCurrency = findedCurrency.Abbreviation, 
                               IsConfiguredPortal = false, 
                               ChangeContactStatusGroupAuto = null,
                               WebFormKey = Guid.Empty,
                               SMTPServerSetting = new SMTPServerSetting()
                           };

            return new CRMSettings
                       {
                           defaultCurrency = "USD", 
                           IsConfiguredPortal = false,
                           ChangeContactStatusGroupAuto = null,
                           WebFormKey = Guid.Empty,
                           SMTPServerSetting = new SMTPServerSetting()
                       };
        }
    }
}