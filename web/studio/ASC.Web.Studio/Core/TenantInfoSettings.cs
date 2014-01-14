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
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class TenantInfoSettings : ISettings
    {
        [DataMember(Name = "LogoSize")]
        public Size CompanyLogoSize { get; private set; }

        [DataMember(Name = "LogoFileName")] private string _companyLogoFileName;

        [DataMember(Name = "Default")]
        private bool _isDefault { get; set; }

        #region ISettings Members

        public ISettings GetDefault()
        {
            return new TenantInfoSettings
                       {
                           _isDefault = true
                       };
        }

        public void RestoreDefault()
        {
            _isDefault = true;

            var currentTenant = CoreContext.TenantManager.GetCurrentTenant();
            currentTenant.Name = Resources.Resource.StudioWelcomeHeader;
            CoreContext.TenantManager.SaveTenant(currentTenant);

            var store = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "logo");
            try
            {
                store.DeleteFiles("", "*", false);
            }
            catch
            {
            }
            CompanyLogoSize = default(Size);
        }

        public void SetCompanyLogo(string companyLogoFileName, byte[] data)
        {
            var store = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "logo");

            if (!_isDefault)
            {
                try
                {
                    store.DeleteFiles("", "*", false);
                }
                catch
                {
                }
            }
            using (var memory = new MemoryStream(data))
            using (var image = Image.FromStream(memory))
            {
                CompanyLogoSize = image.Size;
                memory.Seek(0, SeekOrigin.Begin);
                store.Save(companyLogoFileName, memory);
                _companyLogoFileName = companyLogoFileName;
            }
            _isDefault = false;
        }

        public string GetAbsoluteCompanyLogoPath()
        {
            if (_isDefault)
            {
                var logoName =
                    CoreContext.Configuration.YourDocs
                        ? "logoyd"
                        : "logo";

                return WebImageSupplier.GetAbsoluteWebPath("logo/" + logoName + ".png");
            }

            var store = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "logo");
            return store.GetUri(_companyLogoFileName ?? "").ToString();
        }

        public Guid ID
        {
            get { return new Guid("{5116B892-CCDD-4406-98CD-4F18297C0C0A}"); }
        }

        #endregion
    }
}