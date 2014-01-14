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

window.ASC.Files.Anchor = (function () {
    var isInit = false;

    var anchorRegExp = {
        error: new RegExp("^error(?:\/(\\S+))?"),
        imprt: new RegExp("^import(?:\/(\\w+))?"),
        preview: new RegExp("^preview(?:\/|%2F)" + ASC.Files.Constants.entryIdRegExpStr),
        setting: new RegExp("^setting(?:=?(\\w+)?)?"),
        folder: new RegExp("^" + ASC.Files.Constants.entryIdRegExpStr),
        help: new RegExp("^help(?:=(\\d+))?"),
        anyanchor: new RegExp("^(\\S+)?"),
    };

    var init = function () {
        if (isInit === false) {
            isInit = true;

            ASC.Controls.AnchorController.bind(anchorRegExp.anyanchor, onValidation);
            ASC.Controls.AnchorController.bind(anchorRegExp.error, onError);
            ASC.Controls.AnchorController.bind(anchorRegExp.folder, onFolderSelect);
            ASC.Controls.AnchorController.bind(anchorRegExp.preview, onPreview);
            ASC.Controls.AnchorController.bind(anchorRegExp.imprt, onImport);
            ASC.Controls.AnchorController.bind(anchorRegExp.setting, onSetting);
            ASC.Controls.AnchorController.bind(anchorRegExp.help, onHelp);
        }
    };

    /* Events */

    var onValidation = function (hash) {
        if (anchorRegExp.error.test(hash)
            || anchorRegExp.imprt.test(hash)
            || anchorRegExp.preview.test(hash)
            || anchorRegExp.setting.test(hash)
            || anchorRegExp.folder.test(hash)
            || anchorRegExp.help.test(hash)) {
            return;
        }

        ASC.Files.Anchor.defaultFolderSet();
    };

    var onError = function (errorString) {
        ASC.Files.UI.displayInfoPanel(decodeURIComponent(errorString || ASC.Files.FilesJSResources.UnknownErrorText).replace(/\+/g, " "), true);
        if (jq.browser.msie) {
            setTimeout(ASC.Files.Anchor.defaultFolderSet, 3000);
        } else {
            ASC.Files.Anchor.defaultFolderSet();
        }
    };

    var onFolderSelect = function (itemid) {
        if (jq.browser.safari) {
            itemid = decodeURIComponent(itemid);
        }
        jq("#treeViewSelector .jstree-open").addClass("jstree-closed").removeClass("jstree-open");

        if (ASC.Files.Common.isCorrectId(itemid)) {
            ASC.Files.Folders.currentFolder.id = itemid;
        } else {
            ASC.Files.Folders.currentFolder.id = "";
        }

        ASC.Files.Actions.hideAllActionPanels();

        ASC.Files.UI.updateFolderView();
    };

    var onPreview = function (fileId) {
        if (jq.browser.safari) {
            fileId = decodeURIComponent(fileId);
        }
        if (typeof ASC.Files.ImageViewer != "undefined") {
            ASC.Files.ImageViewer.init(fileId);
        } else {
            ASC.Files.Anchor.defaultFolderSet();
        }
    };

    var onHelp = function (helpId) {
        ASC.Files.UI.displayHelp(helpId);
    };

    var onSetting = function (settingTab) {
        switch (settingTab) {
            case "thirdparty":
                if (ASC.Files.ThirdParty) {
                    ASC.Files.ThirdParty.showSettingThirdParty();
                } else {
                    ASC.Files.Anchor.defaultFolderSet();
                    return;
                }
                break;
            default:
                ASC.Files.UI.displayCommonSetting();
        }
        ASC.Files.CreateMenu.disableMenu();
    };

    var onImport = function (source) {
        if (ASC.Files.Import) {
            ASC.Files.Folders.eventAfter = ASC.Files.Import.selectEventBySource(source);
        }

        ASC.Files.Anchor.defaultFolderSet();
    };

    /* Methods */
    var fixHash = function (hash) {
        if (jq.browser.mozilla || jq.browser.safari) {
            hash = encodeURIComponent(hash);
        }
        return hash;
    };

    var move = function (hash, safe) {
        hash = ASC.Files.Anchor.fixHash(hash);

        if (safe) {
            ASC.Controls.AnchorController.safemove(hash);
        } else {
            ASC.Controls.AnchorController.move(hash);
        }
    };

    var madeAnchor = function (folderId, safemode) {
        if (!ASC.Files.Common.isCorrectId(folderId)) {
            folderId = ASC.Files.Folders.currentFolder.id;
        }

        ASC.Files.UI.lastSelectedEntry = jq("#filesMainContent .file-row:has(.checkbox input:checked)").map(function () {
            var entryData = ASC.Files.UI.getObjectData(this);
            return { entryType: entryData.entryType, entryId: entryData.entryId };
        });

        ASC.Files.Anchor.move(folderId, safemode === true);
    };

    var navigationSet = function (param, safemode, savefilter) {
        ASC.Files.UI.resetSelectAll(false);
        ASC.Files.UI.amountPage = 0;

        safemode = safemode === true;
        savefilter = safemode || savefilter === true;
        if (!savefilter) {
            ASC.Files.Filter.clearFilter(true);
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
        if (!ASC.Files.Tree.folderIdCurrentRoot) {
            if (!ASC.Files.Constants.FOLDER_ID_MY_FILES) {
                ASC.Files.Anchor.navigationSet(ASC.Files.Constants.FOLDER_ID_COMMON_FILES);
            } else if (ASC.Files.Folders.currentFolder.id != ASC.Files.Constants.FOLDER_ID_MY_FILES || ASC.Files.UI.isSettingsPanel()) {
                ASC.Files.Anchor.navigationSet(ASC.Files.Constants.FOLDER_ID_MY_FILES);
            }
        } else if (ASC.Files.Folders.currentFolder.id != ASC.Files.Tree.folderIdCurrentRoot) {
            ASC.Files.Anchor.navigationSet(ASC.Files.Tree.folderIdCurrentRoot);
        }
    };

    return {
        init: init,
        fixHash: fixHash,

        move: move,
        navigationSet: navigationSet,
        defaultFolderSet: defaultFolderSet
    };
})();

(function ($) {
    $(function () {
        ASC.Files.Anchor.init();
    });
})(jQuery);