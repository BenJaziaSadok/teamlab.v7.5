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

using ASC.Core.Configuration;
using ASC.Core.Tenants;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ASC.Core
{
    class ClientConfiguration : IConfigurationClient
    {
        private readonly ITenantService tenantService;
        private readonly SmtpSettings configSmtpSettings;


        public bool Standalone
        {
            get { return ConfigurationManager.AppSettings["core.base-domain"] == "localhost"; }
        }

        public bool YourDocs
        {
            get { return ConfigurationManager.AppSettings["core.your-docs"] == "true" || YourDocsDemo; }
        }

        public bool YourDocsDemo
        {
            get { return ConfigurationManager.AppSettings["core.your-docs.demo"] == "true"; }
        }

        public SmtpSettings SmtpSettings
        {
            get
            {
                return configSmtpSettings ?? Deserialize(GetSetting("SmtpSettings"));
            }
            set
            {
                if (configSmtpSettings != null)
                {
                    throw new InvalidOperationException("Mail Settings defined in the configuration file.");
                }
                SaveSetting("SmtpSettings", Serialize(value));
            }
        }

        public string SKey
        {
            get
            {
                return GetSetting("DocKey") ?? ConfigurationManager.AppSettings["files.docservice.key"];
            }
            set
            {
                if (Standalone)
                {
                    SaveSetting("DocKey", value);
                }
            }
        }


        public ClientConfiguration(ITenantService service)
        {
            this.tenantService = service;
            this.configSmtpSettings = GetSmtpSettingsFromConfig();
        }


        public void SaveSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = value != null ? Encoding.UTF8.GetBytes(value) : null;
            if (data != null)
            {
                data = Crypto.GetV(data, 2, true);
            }
            tenantService.SetTenantSettings(Tenant.DEFAULT_TENANT, key, data);
        }

        public string GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key);
            return data != null ? Encoding.UTF8.GetString(Crypto.GetV(data, 2, false)) : null;
        }

        public string GetKey(int tenant)
        {
            if (Standalone)
            {
                var key = GetSetting("PortalId");
                if (string.IsNullOrEmpty(key))
                {
                    lock (tenantService)
                    {
                        // thread safe
                        key = GetSetting("PortalId");
                        if (string.IsNullOrEmpty(key))
                        {
                            SaveSetting("PortalId", key = Guid.NewGuid().ToString());
                        }
                    }
                }
                return key;
            }
            else
            {
                var t = tenantService.GetTenant(tenant);
                return t != null && !string.IsNullOrWhiteSpace(t.PaymentId) ? t.PaymentId : (ConfigurationManager.AppSettings["core.payment-region"] + tenant);
            }
        }

        private string Serialize(SmtpSettings smtp)
        {
            if (smtp == null) return null;
            return string.Join("#",
                               new[]
                                   {
                                       smtp.CredentialsDomain,
                                       smtp.CredentialsUserName,
                                       smtp.CredentialsUserPassword,
                                       smtp.Host,
                                       smtp.Port.HasValue ? smtp.Port.Value.ToString() : string.Empty,
                                       smtp.SenderAddress,
                                       smtp.SenderDisplayName,
                                       smtp.EnableSSL.ToString()
                                   });
        }

        private SmtpSettings Deserialize(string value)
        {
            if (string.IsNullOrEmpty(value)) return new SmtpSettings();

            var props = value.Split(new[] { '#' }, StringSplitOptions.None);
            props = Array.ConvertAll(props, p => !string.IsNullOrEmpty(p) ? p : null);
            return new SmtpSettings
                       {
                           CredentialsDomain = props[0],
                           CredentialsUserName = props[1],
                           CredentialsUserPassword = props[2],
                           Host = props[3],
                           Port = String.IsNullOrEmpty(props[4]) ? null : (int?)Int32.Parse(props[4]),
                           SenderAddress = props[5],
                           SenderDisplayName = props[6],
                           EnableSSL = 7 < props.Length && !string.IsNullOrEmpty(props[7]) && Convert.ToBoolean(props[7])
                       };
        }

        private SmtpSettings GetSmtpSettingsFromConfig()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrEmpty(smtpClient.Host))
            {
                // section /configuration/system.net/mailSettings/smtp not empty.
                var smtpSettings = new SmtpSettings();

                smtpSettings.Host = smtpClient.Host;
                smtpSettings.Port = smtpClient.Port;
                smtpSettings.EnableSSL = 400 < smtpClient.Port;

                var credentials = smtpClient.Credentials as NetworkCredential;
                if (credentials != null)
                {
                    smtpSettings.CredentialsDomain = credentials.Domain;
                    smtpSettings.CredentialsUserName = credentials.UserName;
                    smtpSettings.CredentialsUserPassword = credentials.Password;
                }

                var mailMessage = new MailMessage();
                if (mailMessage.From != null)
                {
                    smtpSettings.SenderAddress = mailMessage.From.Address;
                    smtpSettings.SenderDisplayName = mailMessage.From.DisplayName;
                }

                return smtpSettings;
            }
            else
            {
                return null;
            }
        }
    }
}