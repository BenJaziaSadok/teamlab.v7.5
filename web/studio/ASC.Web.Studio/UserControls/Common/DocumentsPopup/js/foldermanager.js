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

window.ASC.Files.Folders = (function () {
    var tasksTimeout = null;
    var bulkStatuses = false;

    var currentFolder = {};
    var folderContainer = "";

    var eventAfter = null;
    var typeNewDoc = "";

    var moveToFolder = "";
    var isCopyTo = false;

    /* Methods*/

    var madeAnchor = function (folderId, safemode) {
        if (!ASC.Files.Common.isCorrectId(folderId)) {
            folderId = ASC.Files.Folders.currentFolder.id;
        }

        ASC.Files.UI.selectedEntry = jq("#filesMainContent .file-row:has(.checkbox input:checked)").map(function () {
            var entryData = ASC.Files.UI.getObjectData(this);
            return {entryType: entryData.entryType, entryId: entryData.entryId};
        });

        if (safemode === true) {
            ASC.Controls.AnchorController.safemove(folderId);
        } else {
            ASC.Controls.AnchorController.move(folderId);
        }
    };

    var navigationSet = function (param, safemode, savefilter) {
        safemode = safemode === true;
        savefilter = savefilter === true;
        if (!savefilter) {
        }

        if (ASC.Files.Common.isCorrectId(param)) {
            if (!safemode || ASC.Files.Folders.currentFolder.id != param) {
                ASC.Files.Folders.currentFolder.id = param;
                ASC.Files.Folders.folderContainer = "";
            }
            madeAnchor(null, safemode);
        } else {
            ASC.Files.Folders.currentFolder.id = "";
            ASC.Files.Folders.folderContainer = param;
            madeAnchor(param, safemode);
        }
    };

    var defaultFolderSet = function () {
        if (!ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT) {
            if (!ASC.Files.Constants.FOLDER_ID_MY_FILES) {
                ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_COMMON_FILES);
            } else {
                if (ASC.Files.Folders.currentFolder.id != ASC.Files.Constants.FOLDER_ID_MY_FILES) {
                    ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_MY_FILES);
                }
            }
        } else {
            if (ASC.Files.Folders.currentFolder.id != ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT) {
                ASC.Files.Folders.navigationSet(ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT);
            }
        }
    };

    var getFolderItems = function (isAppend, countAppend) {
        var filterSettings = ASC.Files.Filter.getFilterSettings();

        ASC.Files.ServiceManager.getFolderItems(ASC.Files.TemplateManager.events.GetFolderItems,
            {
                folderId: ASC.Files.Folders.currentFolder.id,
                from: (isAppend ? jq("#filesMainContent .file-row[name!=\"addRow\"]").length : 0),
                count: countAppend || ASC.Files.Constants.COUNT_ON_PAGE,
                append: isAppend === true,
                filter: filterSettings.filter,
                subject: filterSettings.subject,
                text: filterSettings.text,
                compactView: jq("#filesMainContent").hasClass("compact")
            }, {orderBy: filterSettings.sorter});
    };

    var clickOnFolder = function (folderId) {
        if (ASC.Files.Folders.folderContainer == "trash") {
            return;
        }
        ASC.Files.Folders.navigationSet(folderId);
    };

    var clickOnFile = function (fileId, version, fileTitle, isNew) {
        if (ASC.Files.Folders.folderContainer == "trash") {
            return;
        }

        if (ASC.Files.Share) {
            ASC.Files.Share.removeNewIcon("file", fileId);
        }

        fileTitle = fileTitle || ASC.Files.UI.getEntryTitle("file", fileId);

        var url = ASC.Files.Utility.GetFileViewUrl(fileId, version);

        if (ASC.Files.Utility.CanWebView(fileTitle)) {
            url = ASC.Files.Utility.GetFileWebViewerUrl(fileId, version) + (isNew ? "&new=true" : "");
            window.open(url, "_blank");
            return;
        }

        if (typeof ASC.Files.ImageViewer != "undefined" && ASC.Files.Utility.CanImageView(fileTitle)) {
            url = ASC.Files.ImageViewer.getPreviewUrl(fileId);
            ASC.Controls.AnchorController.move(url);
            return;
        }

        if (jq.browser.mobile) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_MobileDownload, true);
        } else {
            window.open(url, "_blank");
        }
    };
    return {
        eventAfter: eventAfter,

        currentFolder: currentFolder,
        folderContainer: folderContainer,
        defaultFolderSet: defaultFolderSet,
        getFolderItems: getFolderItems,

        clickOnFolder: clickOnFolder,
        clickOnFile: clickOnFile
    };
})();