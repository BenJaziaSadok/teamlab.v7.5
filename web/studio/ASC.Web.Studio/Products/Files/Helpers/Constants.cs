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
using ASC.Web.Studio.Utility;

namespace ASC.Files.Core
{
    public static class FileConstant
    {
        public static readonly string ModuleId = "files";

        public static readonly string StorageModule = "files";

        public static readonly string StorageDomainTmp = "files_temp";

        public static readonly string StorageDomainTemplate = "files_template";

        public static readonly string DatabaseId = "files";

        public static readonly Guid ShareLinkId = new Guid("{D77BD6AF-828B-41f5-84ED-7FFE2565B13A}");
      
        public const string TemplateDocPath = "templatedocuments/";
    }
}

namespace ASC.Web.Files.Classes
{
    public static class UrlConstant
    {
        public const string DownloadTitle = "download";
        public const string Error = "error";
        public const string New = "new";
        public const string ProjectId = "prjid";
        public const string DocumentType = "doctype";
        public const string Template = "template";

        public const string ParamsSave =
            "?" + CommonLinkUtility.Action + "=save&"
            + CommonLinkUtility.FileId + "={0}&"
            + "tabId={1}&"
            + CommonLinkUtility.Version + "={2}&"
            + CommonLinkUtility.FileUri + "={3}";

        public const string ParamsDemo =
            "?" + CommonLinkUtility.Action + "=download&"
            + CommonLinkUtility.TryParam + "={0}";
    }
}