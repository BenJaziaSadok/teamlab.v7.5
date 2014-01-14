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

using System.Reflection;
using System.Configuration.Install;

namespace ASC.Mail.StorageCleaner
{
    /// <summary>
    /// Helper for insatll/uninstall of win services
    /// </summary>
    public static class ServiceSelfInstaller
    {
        #region Fileds
        private static readonly string ExePath = Assembly.GetExecutingAssembly().Location;
        #endregion
        #region Methods

        /// <summary>
        ////Install the service
        /// </summary>
        /// <returns>Result of the instllation</returns>
        public static bool Install()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Uninstall the service
        /// </summary>
        /// <returns>Result of the uninstallation</returns>
        public static bool Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new[] { "/u", ExePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
