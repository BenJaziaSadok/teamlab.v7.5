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
using System.Collections.Generic;
using ASC.Core.Users;
using ASC.Projects.Engine;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Classes
{
    public class Global
    {

        #region Constants

        public static readonly string DbID = "projects";

        public static readonly String FileKeyFormat = "{0}/{1}/{2}/{3}"; // ProjectID/FileID/FileVersion/FileTitle

        public static readonly int EntryCountOnPage = 25;
        public static readonly int VisiblePageCount = 3;

        public static readonly String FileStorageModule = "projects";
        public static readonly String FileStorageModuleTemp = "projects_temp";

        public static readonly KeyValuePair<FileUtility.CsvDelimiter, string> ReportCsvDelimiter =
            new KeyValuePair<FileUtility.CsvDelimiter, string>(FileUtility.CsvDelimiter.Tab, "\t");

        #endregion

        #region Property

        public static EngineFactory EngineFactory
        {
            get { return new EngineFactory(DbID, TenantProvider.CurrentTenantID); }
        }

        #endregion

        #region Methods

        public static string GetHTMLUserAvatar(UserInfo user)
        {
            var imgPath = user.GetBigPhotoURL();
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" alt='' src=\"" + imgPath + "\"/>";

            return "";
        }

        #endregion
    }
}