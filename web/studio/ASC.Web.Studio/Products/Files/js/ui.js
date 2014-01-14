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

window.ASC.Files.UI = (function () {
    var isInit = false;

    var timeCheckEditing = null;
    var timeTooltip = null;

    var currentPage = 0;
    var amountPage = 0;

    var lastSelectedEntry = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;
        }

        jq(window).resize(function () {
            ASC.Files.UI.fixContentHeaderWidth();
        });

        window.onbeforeunload = function () {
            if (ASC.Files.ChunkUploads && ASC.Files.ChunkUploads.uploaderBusy) {
                return (ASC.Files.FilesJSResources.ConfirmLeavePage || "Are you sure you want to leave the current page. Downloading files will be interrupted.");
            }
        };

        if (jq.browser.msie) {
            //fix Flash & IE URL hash problem
            documentTitleFix();
        }
    };

    var getEntryObject = function (entryType, entryId) {
        if (entryType && entryId) {
            if (!ASC.Files.Common.isCorrectId(entryId)) {
                return null;
            }
            return jq("#filesMainContent .file-row[data-id=\"" + entryType + "_" + entryId + "\"]");
        }
        return null;
    };

    var getObjectData = function (entryObject) {
        entryObject = jq(entryObject);
        if (!entryObject.is(".file-row")) {
            entryObject = entryObject.closest(".file-row");
        }
        if (entryObject.length == 0) {
            return null;
        }

        var entryDataStr = entryObject.find("input:hidden[name=\"entry_data\"]").val();
        var resulat = jq.parseJSON(entryDataStr);
        if (!ASC.Files.Common.isCorrectId(resulat.id)) {
            return null;
        }

        resulat.entryId = resulat.id;
        resulat.entryType = (resulat.entryType === "file" ? "file" : "folder");
        resulat.create_by = entryObject.find("input:hidden[name=\"create_by\"]").val();
        resulat.modified_by = entryObject.find("input:hidden[name=\"modified_by\"]").val();
        resulat.entryObject = entryObject;
        resulat.error = (resulat.error != "" ? resulat.error : false);
        resulat.title = resulat.title.trim();
        return resulat;
    };

    var parseItemId = function (itemId) {
        if (typeof itemId == "undefined" || itemId == null) {
            return null;
        }

        var entryType = itemId.indexOf("file_") == "0" ? "file" : "folder";
        var entryId = itemId.substring((entryType + "_").length);

        if (!ASC.Files.Common.isCorrectId(entryId)) {
            return null;
        }
        return { entryType: entryType, entryId: entryId };
    };

    var getEntryTitle = function (entryType, entryId) {
        if (!ASC.Files.Common.isCorrectId(entryId)) {
            return null;
        }
        return ASC.Files.UI.getObjectTitle(ASC.Files.UI.getEntryObject(entryType, entryId));
    };

    var getObjectTitle = function (entryObject) {
        entryObject = jq(entryObject);
        if (!entryObject.is(".file-row")) {
            entryObject = entryObject.closest(".file-row");
        }
        if (entryObject.length == 0) {
            return null;
        }

        return ASC.Files.UI.getObjectData(entryObject).title.trim();
    };

    var updateFolderView = function () {
        if (!ASC.Files.Common.isCorrectId(ASC.Files.Folders.currentFolder.id)) {
            ASC.Files.Anchor.move("");
            return;
        }

        ASC.Files.UI.hideAllContent(true);

        ASC.Files.UI.stickContentHeader();

        ASC.Files.Folders.getFolderItems(false);
    };

    var switchFolderView = function (toCompact) {
        var storageKey = ASC.Files.Constants.storageKeyCompactView;
        if (typeof toCompact == "undefined") {
            toCompact = ASC.Files.Common.localStorageManager.getItem(storageKey) === true;
        }
        ASC.Files.Common.localStorageManager.setItem(storageKey, toCompact);

        jq("#switchViewFolder").toggleClass("compact", toCompact === true);
        jq("#filesMainContent").toggleClass("compact", toCompact === true);
        if (toCompact !== true) {
            if (jq(document).height() - jq(window).height() <= jq(window).scrollTop() + 350) {
                ASC.Files.Folders.showMore();
            }
        }

        ASC.Files.Mouse.updateMainContentArea();
    };

    var isSettingsPanel = function () {
        return jq("#settingCommon, #settingThirdPartyPanel, #helpPanel").is(":visible");
    };

    var fixContentHeaderWidth = function () {
        var headerFixed = jq("#mainContentHeader.stick-panel:visible");
        headerFixed.css("width",
            headerFixed.parent().innerWidth()
                - parseInt(headerFixed.css("margin-left"))
                - parseInt(headerFixed.css("margin-right")));
    };

    var stickContentHeader = function () {
        var boxTop = jq("#mainContentHeader .down_arrow");
        var movingPopupShift = boxTop.offset().top + boxTop.outerHeight();
        var movingPopupObj = jq("#filesSelectorPanel:visible");

        ASC.Files.Common.stickMovingPanel("mainContentHeader", movingPopupObj, movingPopupShift, false);

        ASC.Files.UI.fixContentHeaderWidth();
    };

    var blockObjectById = function (entryType, entryId, value, message, incycle) {
        return ASC.Files.UI.blockObject(ASC.Files.UI.getEntryObject(entryType, entryId), value, message, incycle);
    };

    var blockObject = function (entryObj, value, message, incycle) {
        value = value === true;
        entryObj = jq(entryObj);
        if (!entryObj.length
            || entryObj.hasClass("checkloading") && value) {
            return;
        }

        ASC.Files.UI.selectRow(entryObj, false);
        if (!incycle) {
            ASC.Files.UI.updateMainContentHeader();
        }
        ASC.Files.UI.hideLinks(entryObj);

        entryObj.toggleClass("loading checkloading", value === true);
        if (value === true) {
            entryObj.block({ message: "", baseZ: 99 });
            if (typeof message != "undefined" && message) {
                entryObj.children("div.blockUI.blockOverlay").attr("title", message);
            }
        } else {
            entryObj.unblock();
            entryObj.css("position", "static");
        }
    };

    var hideLinks = function (entryObj) {
        jq(entryObj).removeClass("row-over");
    };

    var showLinks = function (entryObj) {
        if (jq("#filesSelector").css("display") == "block"
            || jq(entryObj).hasClass("loading")) {
            return;
        }

        jq(entryObj).addClass("row-over");
    };

    var editingFile = function (entryObj) {
        return !entryObj.hasClass("folder-row") && entryObj.hasClass("on-edit");
    };

    var editableFile = function (fileData) {
        var fileObj = fileData.entryObject;
        var fileType = fileData.entryType;
        var title = fileData.title;

        return fileType == "file"
            && ASC.Files.Folders.folderContainer != "trash"
            && !fileObj.hasClass("row-rename")
            && ASC.Files.UI.accessibleItem(fileObj)
            && ASC.Files.Utility.CanWebEdit(title);
    };

    var highlightExtension = function (rowLink, entryTitle) {
        var fileExt = ASC.Files.Utility.GetFileExtension(entryTitle);
        var entrySplitTitle = entryTitle.substring(0, entryTitle.length - fileExt.length);
        rowLink.html("{0}<span class=\"file-extension\">{1}</span>".format(entrySplitTitle, fileExt));
    };

    var addRowHandlers = function (entryObject) {
        var listEntry = (entryObject || jq("#filesMainContent .file-row"));

        listEntry.each(function () {
            var entryData = ASC.Files.UI.getObjectData(this);

            var entryId = entryData.entryId;
            var entryType = entryData.entryType;
            var entryObj = entryData.entryObject;
            var entryTitle = entryData.title;

            if (ASC.Files.Folders.folderContainer == "trash") {
                entryObj.find(".entry-descr .title-created").remove();
                entryObj.find(".entry-descr .title-removed").removeClass("display-none");
                if (entryType == "folder") {
                    entryObj.find(".create-date").remove();
                    entryObj.find(".modified-date").removeClass("display-none");
                }
            } else {
                entryObj.find(".entry-descr .title-removed").remove();
                if (entryType == "folder") {
                    entryObj.find(".modified-date").remove();
                }
            }
            var rowLink = entryObj.find(".entry-title .name a");

            var ftClass = (entryType == "file" ? ASC.Files.Utility.getCssClassByFileTitle(entryTitle) : ASC.Files.Utility.getFolderCssClass());
            entryObj.find(".thumb-" + entryType).addClass(ftClass);

            if (!entryObj.hasClass("checkloading")) {

                if (entryType == "file") {
                    if (rowLink.is(":not(:has(.file-extension))")) {
                        ASC.Files.UI.highlightExtension(rowLink, entryTitle);
                    }

                    if (ASC.Files.Folders.folderContainer != "trash") {
                        var entryUrl = ASC.Files.Utility.GetFileDownloadUrl(entryId);

                        if ((ASC.Files.Utility.CanWebView(entryTitle) || ASC.Files.Utility.CanWebEdit(entryTitle))
                            && !ASC.Files.Utility.MustConvert(entryTitle)
                            && ASC.Resources.Master.TenantTariffDocsEdition) {
                            entryUrl = ASC.Files.Utility.GetFileWebEditorUrl(entryId);
                            rowLink.attr("href", entryUrl).attr("target", "_blank");
                        } else if (typeof ASC.Files.ImageViewer != "undefined" && ASC.Files.Utility.CanImageView(entryTitle)) {
                            entryUrl = "#" + ASC.Files.ImageViewer.getPreviewHash(entryId);
                            rowLink.attr("href", entryUrl);
                        } else {
                            rowLink.attr("href", jq.browser.mobile ? "" : entryUrl);
                        }
                    }

                    if (ASC.Files.UI.editableFile(entryData)) {
                        ASC.Files.UI.lockEditFile(entryObj, ASC.Files.UI.editingFile(entryObj));
                        if (ASC.Files.Utility.MustConvert(entryTitle)) {
                            entryObj.find(".pencil:not(.convert-action)").remove();
                            entryObj.find(".convert-action").removeClass("display-none");
                        } else {
                            entryObj.find(".convert-action").remove();
                            if (ASC.Files.Utility.CanCoAuhtoring(entryTitle)) {
                                entryObj.addClass("can-coauthoring");
                            }
                            entryUrl = ASC.Files.Utility.GetFileWebEditorUrl(entryId);
                            entryObj.find(".file-edit").attr("href", entryUrl);
                        }
                    } else {
                        entryObj.addClass("cannot-edit");
                        entryObj.find(".pencil").remove();
                    }
                } else if (ASC.Files.Folders.folderContainer != "trash") {
                    entryUrl = "#" + ASC.Files.Anchor.fixHash(entryId);
                    rowLink.attr("href", entryUrl);
                }

                if (ASC.Files.Folders.folderContainer == "trash"
                    || entryData.error) {
                    rowLink.attr("href", "#" + ASC.Files.Folders.currentFolder.id);
                }

                if (!jq("#filesMainContent").hasClass("without-share")
                    && (ASC.Files.Folders.folderContainer == "forme" && !ASC.Files.UI.accessibleItem(entryObj)
                        || ASC.Files.Constants.YOUR_DOCS && entryType == "folder")) {
                    entryObj.addClass("without-share");
                }

                if (jq.browser.msie) {
                    entryObj.find("*").attr("unselectable", "on");
                }
            }
        });

        ASC.Files.UI.switchFolderView();
    };

    var clickRow = function (event) {
        var e = ASC.Files.Common.fixEvent(event);

        if (!(e.button == 0 || (jq.browser.msie && e.button == 1))) {
            return true;
        }

        var target = jq(e.srcElement || e.target);

        try {
            if (target.is("a, .menu-small")) {
                return true;
            }
        } catch (e) {
            return true;
        }

        var entryObj =
            target.is(".file-row")
                ? target
                : target.closest(".file-row");

        ASC.Files.UI.selectRow(entryObj, !entryObj.hasClass("row-selected"));
        ASC.Files.UI.updateMainContentHeader();
        return true;
    };

    var updateMainContentHeader = function () {
        ASC.Files.UI.resetSelectAll(jq("#filesMainContent .file-row:has(.checkbox input:not(:checked))").length == 0);
        if (jq("#filesMainContent .file-row:has(.checkbox input:checked)").length == 0) {
            jq("#mainContentHeader .menuAction.unlockAction").removeClass("unlockAction");
            if (ASC.Files.Folders.folderContainer == "trash") {
                jq("#mainEmptyTrash").addClass("unlockAction");
            }
        } else {
            ASC.Files.Actions.showActionsViewPanel();
        }
    };

    var selectRow = function (entryObj, value) {
        if (!entryObj.hasClass("file-row")) {
            entryObj = entryObj.closest(".file-row");
        }

        if (entryObj.hasClass("row-selected") == value) {
            return false;
        }

        if (entryObj.hasClass("checkloading") && value) {
            return false;
        }

        if (entryObj.hasClass("new-folder") || jq(entryObj).hasClass("row-rename")) {
            value = false;
        }

        entryObj.find(".checkbox input").prop("checked", value === true);
        entryObj.toggleClass("row-selected", value);

        return true;
    };

    var resetSelectAll = function (param) {
        jq("#filesSelectAllCheck").prop("checked", param === true);
    };

    var checkSelectAll = function (value) {
        var selectionChanged = false;
        var selectedString = ".row-selected";
        if (value) {
            selectedString = ":not(" + selectedString + ")";
        }
        jq("#filesMainContent .file-row" + selectedString + ":not(.checkloading)").each(function () {
            selectionChanged = ASC.Files.UI.selectRow(jq(this), value) || selectionChanged;
        });
        if (selectionChanged) {
            ASC.Files.UI.updateMainContentHeader();
        }
    };

    var checkSelect = function (filter) {
        ASC.Files.Actions.hideAllActionPanels();
        jq("#filesMainContent .file-row:not(.checkloading)").each(function () {
            var sel;
            var fileTitle = ASC.Files.UI.getObjectTitle(this);
            switch (filter) {
                case "folder":
                    sel = jq(this).hasClass("folder-row");
                    break;
                case "file":
                    sel = !jq(this).hasClass("folder-row");
                    break;
                case "document":
                    sel = ASC.Files.Utility.FileIsDocument(fileTitle) && !jq(this).hasClass("folder-row");
                    break;
                case "presentation":
                    sel = ASC.Files.Utility.FileIsPresentation(fileTitle) && !jq(this).hasClass("folder-row");
                    break;
                case "spreadsheet":
                    sel = ASC.Files.Utility.FileIsSpreadsheet(fileTitle) && !jq(this).hasClass("folder-row");
                    break;
                case "image":
                    sel = ASC.Files.Utility.FileIsImage(fileTitle) && !jq(this).hasClass("folder-row");
                    break;
                default:
                    return false;
            }

            ASC.Files.UI.selectRow(jq(this), sel);
            return true;
        });
        ASC.Files.UI.updateMainContentHeader();
    };

    var displayInfoPanel = function (str, warn) {
        if (str === "" || typeof str === "undefined") {
            return;
        }

        if (warn === true) {
            toastr.error(str);
        } else {
            toastr.success(str);
        }
    };

    var accessibleItem = function (entryObj) {
        var entryData = entryObj
            ? ASC.Files.UI.getObjectData(entryObj)
            : ASC.Files.Folders.currentFolder;

        var access = ASC.Files.Constants.USER_ADMIN || !entryData || entryData.create_by_id == ASC.Files.Constants.USER_ID;

        if (entryData
            && entryData.entryType == "folder"
            && (entryData.entryId === ASC.Files.Constants.FOLDER_ID_SHARE
                || entryData.entryId === ASC.Files.Constants.FOLDER_ID_PROJECT
                || entryData.entryId === ASC.Files.Constants.FOLDER_ID_TRASH)) {
            access = false;
        }

        var entryId = entryData ? entryData.entryId : null;
        var entryType = entryData ? entryData.entryType : null;
        if (entryType == "folder") {
            if (entryId == ASC.Files.Constants.FOLDER_ID_COMMON_FILES && !ASC.Files.Constants.USER_ADMIN) {
                return false;
            }

            if (entryId == ASC.Files.Constants.FOLDER_ID_SHARE) {
                return false;
            }

            if (entryId == ASC.Files.Constants.FOLDER_ID_TRASH) {
                return false;
            }

            if (entryId == ASC.Files.Constants.FOLDER_ID_PROJECT) {
                return false;
            }

            if (ASC.Files.Constants.YOUR_DOCS_DEMO && !entryData.provider_key) {
                return false;
            }
        }

        var curAccess = parseInt(entryData ? entryData.access : ASC.Files.Constants.AceStatusEnum.None);

        if (entryType == "folder" && entryId == ASC.Files.Folders.currentFolder.id) {
            curAccess = parseInt(ASC.Files.Folders.currentFolder.access);
        }

        switch (curAccess) {
            case ASC.Files.Constants.AceStatusEnum.None:
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                return true;
            case ASC.Files.Constants.AceStatusEnum.Read:
            case ASC.Files.Constants.AceStatusEnum.Restrict:
                return false;
            default:
                return access;
        }
    };

    var accessAdmin = function (entryObj) {
        var entryData = entryObj
            ? ASC.Files.UI.getObjectData(entryObj)
            : ASC.Files.Folders.currentFolder;

        var access = parseInt(entryData ? entryData.access : ASC.Files.Constants.AceStatusEnum.None);

        var entryId = entryData ? entryData.entryId : null;
        var entryType = entryData ? entryData.entryType : null;

        if (entryType == "folder" && entryId == ASC.Files.Folders.currentFolder.id) {
            access = ASC.Files.Folders.currentFolder.access;
        }

        if (access == ASC.Files.Constants.AceStatusEnum.Restrict) {
            entryObj.remove();
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.AceStatusEnum_Restrict, true);
            return false;
        }

        if (ASC.Files.Folders.currentFolder.id == ASC.Files.Constants.FOLDER_ID_PROJECT) {
            return false;
        }

        return (access == ASC.Files.Constants.AceStatusEnum.None
            || ASC.Files.Folders.folderContainer == "corporate" && ASC.Files.Constants.USER_ADMIN
            || ASC.Files.Folders.currentFolder.id != ASC.Files.Constants.FOLDER_ID_PROJECT && entryData.create_by_id == ASC.Files.Constants.USER_ID);

    };

    var lockEditFileById = function (fileId, edit, listBy) {
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        return ASC.Files.UI.lockEditFile(fileObj, edit, listBy);
    };

    var lockEditFile = function (fileObj, edit, listBy) {
        if (fileObj.hasClass("folder-row")) {
            return;
        }

        if (edit) {
            fileObj.addClass("on-edit");

            var strBy = ASC.Files.FilesJSResources.TitleEditingFile;
            if (listBy) {
                strBy = ASC.Files.FilesJSResources.TitleEditingFileBy.format(listBy);
            }

            fileObj.find(".pencil.file-editing").attr("title", strBy);
        } else {
            fileObj.removeClass("on-edit");
        }
    };

    var checkEditing = function () {
        clearTimeout(ASC.Files.UI.timeCheckEditing);

        var list = jq("#filesMainContent .file-row.on-edit");
        if (list.length == 0) {
            return;
        }

        var data = {};
        data.entry = new Array();
        for (var i = 0; i < list.length; i++) {
            var fileId = ASC.Files.UI.getObjectData(list[i]).entryId;
            data.entry.push(fileId);
        }

        ASC.Files.ServiceManager.checkEditing(ASC.Files.TemplateManager.events.CheckEditing, { list: data.entry }, { stringList: data });
    };

    var displayEntryTooltip = function (entryObj, entryType, entryId) {
        entryObj = entryObj || ASC.Files.UI.getEntryObject(entryType, entryId);

        var entryData = ASC.Files.UI.getObjectData(entryObj);

        Encoder.EncodeType = "!entity";
        var jsonData = {
            entryTooltip:
                {
                    type: entryType,
                    title: Encoder.htmlEncode(entryData.title),
                    create_by: Encoder.htmlEncode(entryData.create_by),
                    date_type:
                        (ASC.Files.Folders.folderContainer == "trash"
                            ? "remove"
                            : (entryType == "folder"
                                ? "create"
                                : (entryData.version > 1
                                    ? "update"
                                    : "upload"))),
                    modified_on:
                        (entryType == "file"
                            ? entryData.modified_on
                            : (ASC.Files.Folders.folderContainer == "trash"
                                ? entryData.modified_on
                                : entryData.create_on)),
                    version: entryData.version, //file
                    length: entryData.content_length, //file
                    error: entryData.error,

                    provider_key: entryData.provider_key,
                    total_files: parseInt(entryObj.find(".countFiles").html()) || 0, //folder
                    total_sub_folder: parseInt(entryObj.find(".countFolders").html()) || 0//folder
                }
        };
        Encoder.EncodeType = "entity";
        var stringData = ASC.Files.Common.jsonToXml(jsonData);

        var htmlTootlip = ASC.Files.TemplateManager.translateFromString(stringData);

        jq("#entryTooltip").html(htmlTootlip);

        jq.dropdownToggle().toggle(entryObj.find(".thumb-" + entryType), "entryTooltip");
    };

    var hideEntryTooltip = function () {
        clearTimeout(ASC.Files.UI.timeTooltip);
        jq("#entryTooltip").hide();

        var linkRow = jq("#filesMainContent .file-row .entry-title .name a[data-title]").removeAttr("data-title");
        linkRow.attr("title", linkRow.text());
    };

    var checkEmptyContent = function () {
        var countAppend = ASC.Files.Constants.COUNT_ON_PAGE - jq("#filesMainContent .file-row").length;
        if (countAppend > 0) {
            if (ASC.Files.UI.currentPage < ASC.Files.UI.amountPage) {
                ASC.Files.Folders.getFolderItems(true, countAppend);
            } else {
                if (countAppend >= ASC.Files.Constants.COUNT_ON_PAGE) {
                    ASC.Files.EmptyScreen.displayEmptyScreen();
                }
            }
        }
    };

    var checkButtonBack = function (buttonSelector, panelSelector) {
        if (ASC.Files.Tree.pathParts.length > 1) {
            var parentFolder = ASC.Files.Tree.pathParts[ASC.Files.Tree.pathParts.length - 2];

            jq(buttonSelector)
                .attr("data-id", parentFolder.Key)
                .attr("href", "#" + parentFolder.Key)
                .attr("title", parentFolder.Value);

            jq(panelSelector || buttonSelector).show();
        } else {
            jq(panelSelector || buttonSelector).hide();
        }
    };

    var checkCharacter = function (input) {
        jq(input).unbind("keyup").bind("keyup", function () {
            var str = jq(this).val();
            if (str.search(ASC.Files.Common.characterRegExp) != -1) {
                jq(this).val(ASC.Files.Common.replaceSpecCharacter(str));
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_SpecCharacter.format(ASC.Files.Common.characterString), true);
            }
        });
    };

    var documentTitleFix = function () {
        if (jq.browser.msie) {
            setInterval(function () {
                ASC.Files.UI.setDocumentTitle(ASC.Files.Folders.currentFolder.title);
            }, 200);
        }
    };

    var setDocumentTitle = function (titlePrefix) {
        titlePrefix = titlePrefix || ASC.Files.Folders.currentFolder.title;
        titlePrefix = titlePrefix ? titlePrefix + " - " : "";
        var titlePostfix = jq("#contentPanel").attr("data-title") || "TeamLab";
        document.title = titlePrefix + titlePostfix;
    };

    var hideAllContent = function (show) {
        if (typeof ASC.Files.ImageViewer != "undefined") {
            ASC.Files.ImageViewer.resetWorkspace();
        }
        show = !!show;

        if (!show) {
            jq("#treeViewContainer .selected").removeClass("selected");
            jq("#treeViewContainer .parent-selected").removeClass("parent-selected");
        }

        jq("#treeSecondary .currentCategory").removeClass("currentCategory");
        jq("#treeSecondary .active").removeClass("active");

        jq("#settingCommon").hide();
        jq("#settingThirdPartyPanel").hide();
        jq("#helpPanel").hide();
        jq("#contentPanel").toggle(show);
    };

    var displayHelp = function (helpId) {
        if (!jq("#helpPanel").length) {
            ASC.Files.Anchor.defaultFolderSet();
        }

        ASC.Files.UI.hideAllContent();

        var params = { helpId: helpId };

        if (!jq("#helpPanel").text().trim().length) {
            params.update = true;
            ASC.Files.ServiceManager.request("get",
                "json",
                ASC.Files.TemplateManager.events.GetHelpCenter,
                params,
                "gethelpcenter");
        } else {
            ASC.Files.EventHandler.onGetHelpCenter(null, params);
        }
    };

    var displayCommonSetting = function () {
        ASC.Files.UI.hideAllContent();
        LoadingBanner.hideLoading();
        jq("#treeSetting").addClass("currentCategory open");
        jq(".settings-link-common").addClass("active");
        jq("#settingCommon").show();

        ASC.Files.UI.setDocumentTitle(ASC.Files.FilesJSResources.TitleSettingsCommon);
    };

    var displayTariffDocsEdition = function () {
        ASC.Files.UI.blockUI("#tariffLimitDocsEditionPanel", 500, 300, 0);
    };

    var displayTariffFileSizeExceed = function () {
        ASC.Files.UI.blockUI("#tariffLimitExceedStoragePanel", 500, 300, 0);
    };

    var blockUI = function (obj, width, height, top) {
        if (ASC.Files.Mouse) {
            ASC.Files.Mouse.finishMoveTo();
            ASC.Files.Mouse.finishSelecting();
        }
        return StudioBlockUIManager.blockUI(obj, width, height, top, "absolute");
    };

    return {
        init: init,
        parseItemId: parseItemId,
        getEntryObject: getEntryObject,
        getObjectData: getObjectData,

        getEntryTitle: getEntryTitle,
        getObjectTitle: getObjectTitle,

        currentPage: currentPage,
        amountPage: amountPage,

        updateFolderView: updateFolderView,
        switchFolderView: switchFolderView,
        isSettingsPanel: isSettingsPanel,

        blockObjectById: blockObjectById,
        blockObject: blockObject,
        hideLinks: hideLinks,
        showLinks: showLinks,

        highlightExtension: highlightExtension,

        addRowHandlers: addRowHandlers,

        updateMainContentHeader: updateMainContentHeader,

        lastSelectedEntry: lastSelectedEntry,
        checkSelectAll: checkSelectAll,
        checkSelect: checkSelect,
        selectRow: selectRow,
        clickRow: clickRow,
        resetSelectAll: resetSelectAll,

        displayInfoPanel: displayInfoPanel,

        editingFile: editingFile,
        editableFile: editableFile,

        checkEditing: checkEditing,
        timeCheckEditing: timeCheckEditing,

        lockEditFileById: lockEditFileById,
        lockEditFile: lockEditFile,

        accessibleItem: accessibleItem,
        accessAdmin: accessAdmin,

        checkCharacter: checkCharacter,
        setDocumentTitle: setDocumentTitle,

        stickContentHeader: stickContentHeader,
        fixContentHeaderWidth: fixContentHeaderWidth,

        displayEntryTooltip: displayEntryTooltip,
        hideEntryTooltip: hideEntryTooltip,
        timeTooltip: timeTooltip,

        checkEmptyContent: checkEmptyContent,
        checkButtonBack: checkButtonBack,

        hideAllContent: hideAllContent,
        displayHelp: displayHelp,
        displayCommonSetting: displayCommonSetting,

        displayTariffDocsEdition: displayTariffDocsEdition,
        displayTariffFileSizeExceed: displayTariffFileSizeExceed,

        blockUI: blockUI
    };
})();

(function ($) {
    ASC.Files.UI.init();

    $(function () {
        jq("#switchToNormal").click(function () {
            ASC.Files.UI.switchFolderView(false);
        });

        jq("#switchToCompact").click(function () {
            ASC.Files.UI.switchFolderView(true);
        });

        jq("#filesMainContent").on("click", ".file-row", ASC.Files.UI.clickRow);

        jq("#filesMainContent").on("click", ".checkbox input", function (event) {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.UI.selectRow(jq(this), this.checked == true);
            ASC.Files.UI.updateMainContentHeader();
            jq(this).blur();
            ASC.Files.Common.cancelBubble(event);
        });

        jq("#filesListUp").click(function () {
            jq(document).scrollTop(0);
            return false;
        });

        jq(document).keydown(function (event) {
            if (jq(".blockUI:visible").length != 0 ||
                jq(".files-popup-win:visible").length != 0 ||
                jq("#promptRename").length != 0 ||
                jq("#promptCreateFolder").length != 0 ||
                jq("#promptCreateFile").length != 0) {
                return true;
            }

            if (!e) {
                var e = event;
            }

            e = ASC.Files.Common.fixEvent(e);

            var target = e.target || e.srcElement;
            try {
                if (jq(target).is("input")) {
                    return true;
                }
            } catch (e) {
                return true;
            }

            var code = e.keyCode || e.which;

            if (code == ASC.Files.Common.keyCode.a && e.ctrlKey) {
                if (jq.browser.opera) {
                    setTimeout(function () {
                        jq("#filesSelectAllCheck").focus();
                    }, 1);
                }
                ASC.Files.UI.checkSelectAll(true);
                return false;
            }

            return true;
        });

        jq(document).keyup(function (event) {
            if (jq(".blockUI:visible").length != 0 ||
                jq(".files-popup-win:visible").length != 0 ||
                jq("#promptRename").length != 0 ||
                jq("#promptCreateFolder").length != 0 ||
                jq("#promptCreateFile").length != 0) {
                return true;
            }

            if (!e) {
                var e = event;
            }

            e = ASC.Files.Common.fixEvent(e);

            var target = e.target || e.srcElement;
            try {
                if (jq(target).is("input")) {
                    return true;
                }
            } catch (e) {
                return true;
            }

            var code = e.keyCode || e.which;

            if (code == ASC.Files.Common.keyCode.deleteKey) {
                ASC.Files.Folders.deleteItem();
                return false;
            }

            if (code == ASC.Files.Common.keyCode.f && e.shiftKey) {
                ASC.Files.Folders.createFolder();
                return false;
            }

            if (code == ASC.Files.Common.keyCode.n && e.shiftKey) {
                ASC.Files.Folders.typeNewDoc = "document";
                ASC.Files.Folders.createNewDoc();
                return false;
            }

            if (code == ASC.Files.Common.keyCode.esc) {
                if (jq("#filesMovingTooltip").is(":visible")) {
                    ASC.Files.Mouse.finishMoveTo();
                    return false;
                }

                if (jq("#filesSelector").is(":visible")) {
                    ASC.Files.Mouse.finishSelecting();
                    return false;
                }

                if (ASC.Files.Actions) {
                    ASC.Files.Actions.hideAllActionPanels();
                }
                return true;
            }
            return true;
        });

        jq("#bottomLoaderPanel").draggable(
            {
                axis: "x",
                handle: ".progress-dialog-header",
                containment: "body"
            }
        );

        jq("#bottomLoaderPanel").on("drag", ".progress-dialog-header", function () {
            ASC.Files.Actions.hideAllActionPanels();
        });

    });
})(jQuery);