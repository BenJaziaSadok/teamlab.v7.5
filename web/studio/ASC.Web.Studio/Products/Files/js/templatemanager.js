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

window.ASC.Files.TemplateManager = (function () {
    var isInit = false,
        tempatesHandlerPath = "",
        xslTemplate = null,
        customEvents = {
            CreateNewFile: "createnewfile",
            CreateFolder: "createfolder",

            CheckEditing: "checkediting",

            GetTreeSubFolders: "gettreesubfolders",
            GetFolderInfo: "getfolderinfo",
            GetFolderItems: "getfolderitems",

            GetSharedInfo: "getsharedinfo",
            GetSharedInfoShort: "getsharedinfoshort",
            SetAceObject: "setaceobject",
            UnSubscribeMe: "unsubscribeme",
            GetShortenLink: "getshortenlink",
            SendLinkToEmail: "sendlinktoemail",

            MarkAsRead: "markasread",
            GetNews: "getnews",

            FolderRename: "folderrename",
            FileRename: "filerename",

            DeleteItem: "deleteitem",
            EmptyTrash: "emptytrash",

            GetFileHistory: "getfilehistory",
            ReplaceVersion: "replaceversion",
            SetCurrentVersion: "setcurrentversion",

            IsZohoAuthentificated: "iszohoauthentificated",
            GetImportData: "getImportData",
            ExecImportData: "execImportData",

            Download: "download",
            GetTasksStatuses: "getTasksStatuses",
            TerminateTasks: "terminatetasks",

            MoveFilesCheck: "movefilescheck",
            MoveItems: "moveitems",

            GetSiblingsImage: "getsiblingsimage",

            GetThirdParty: "getthirdparty",
            SaveThirdParty: "savethirdparty",
            DeleteThirdParty: "deletethirdparty",
            ChangeAccessToThirdparty: "changeaccesstothirdparty",

            UpdateIfExist: "updateifexist",
            GetHelpCenter: "gethelpcenter",

            StoreOriginalFiles: "storeoriginalfiles",
            ConvertCurrentFile: "convertcurrentfile",
            ChunkUploadCheckConversion: "chunkuploadcheckconversion",
            ChunkUploadGetFileFromServer: "chunkuploadgetfilefromserver"
        };

    var init = function (templatesHandler) {
        if (isInit === false) {
            isInit = true;

            tempatesHandlerPath = templatesHandler;
        }
    };

    var getTemplate = function () {
        if (xslTemplate) {
            return xslTemplate;
        }

        xslTemplate = ASC.Controls.XSLTManager.loadXML(tempatesHandlerPath);
        return xslTemplate;
    };

    var translate = function (xmlData) {
        var xslData = getTemplate();
        return ASC.Controls.XSLTManager.translate(xmlData, xslData);
    };

    var translateFromString = function (xmlData) {
        var xslData = getTemplate();
        return ASC.Controls.XSLTManager.translateFromString(xmlData, xslData);
    };

    return {
        events: customEvents,

        translate: translate,
        translateFromString: translateFromString,

        init: init
    };
})();

(function ($) {
    $(function () {
        ASC.Files.TemplateManager.init(ASC.Files.Constants.URL_TEMPLATES_HANDLER);
    });
})(jQuery);