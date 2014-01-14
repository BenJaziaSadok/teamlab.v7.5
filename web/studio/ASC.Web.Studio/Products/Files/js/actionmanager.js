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

window.ASC.Files.Actions = (function () {
    var isInit = false;
    var clipGetLink = null;

    var currentEntryData = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;

            jq(document).click(function (event) {
                jq.dropdownToggle().registerAutoHide(event, "", "#filesActionsPanel");
                jq.dropdownToggle().registerAutoHide(event, ".row-selected .menu-small", "#filesActionPanel",
                    function () {
                        jq(".row-selected.row-lonely-select").removeClass("row-lonely-select");
                    });
            });

            jq.dropdownToggle(
                {
                    switcherSelector: "#mainContentHeader .down_arrow",
                    dropdownID: "filesSelectorPanel",
                    anchorSelector: ".menuActionSelectAll"
                });
        }
    };

    /* Methods*/

    var showActionsViewPanel = function (event) {
        jq("#buttonUnsubscribe, #buttomDelete, #buttomMoveto, #buttomCopyto").hide();
        jq("#mainContentHeader .unlockAction").removeClass("unlockAction");
        var count = jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):has(.checkbox input:checked)").length;
        var countWithRights = count;
        var countIsNew = 0;
        var onlyThirdParty = (ASC.Files.ThirdParty && !ASC.Files.ThirdParty.isThirdParty());
        var countThirdParty = 0;
        var canConvert = false;

        jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):has(.checkbox input:checked)").each(function () {
            var entryObj = jq(this);

            if (ASC.Files.UI.editingFile(entryObj) || !ASC.Files.UI.accessAdmin(entryObj)) {
                countWithRights--;
            }

            if (entryObj.find(".is-new:visible").length > 0) {
                countIsNew++;
            }

            if (ASC.Files.ThirdParty) {
                if (!ASC.Files.ThirdParty.isThirdParty() && ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                    countThirdParty++;
                } else {
                    onlyThirdParty = false;
                }
            } else {
                onlyThirdParty = false;
            }

            if (!canConvert && !entryObj.hasClass("folder-row")) {
                var entryData = ASC.Files.UI.getObjectData(entryObj);
                var entryTitle = entryData.title;
                var formats = ASC.Files.Utility.GetConvertFormats(entryTitle);
                canConvert = formats.length > 0;
            }
        });

        if (count > 0) {
            jq("#buttonDownload, #buttonUnsubscribe, #buttomRestore, #buttomCopyto").show().find("span").html(count);
            jq("#mainDownload, #mainUnsubscribe, #mainRestore, #mainCopy").addClass("unlockAction");
            if (ASC.Files.Folders.folderContainer != "forme") {
                jq("#buttonUnsubscribe").hide();
            }
        }

        if (canConvert) {
            jq("#buttonConvert").show().find("span").html(count);
        } else {
            jq("#buttonConvert").hide();
        }
        jq("#mainConvert").toggleClass("unlockAction", canConvert);

        if (countIsNew > 0) {
            jq("#mainMarkRead").addClass("unlockAction");
        }

        if (countWithRights > 0) {
            jq("#buttomDelete, #buttomMoveto").show().find("span").html(countWithRights - countThirdParty);
            jq("#mainDelete, #mainMove").addClass("unlockAction");
        }

        if (ASC.Files.Folders.folderContainer == "trash") {
            jq("#buttomDelete, #buttomMoveto, #buttomCopyto").hide();
            jq("#mainDelete, #mainMove, #mainCopy").removeClass("unlockAction");
        }

        jq("#buttomRestore, #buttomEmptyTrash").hide();
        jq("#mainRestore, #mainEmptyTrash").removeClass("unlockAction");
        if (ASC.Files.Folders.folderContainer == "trash") {
            if (count > 0) {
                jq("#buttonDownload, #buttomDelete, #buttomRestore").show();
                jq("#mainDownload, #mainDelete, #mainRestore").addClass("unlockAction");
            } else {
                jq("#buttonDownload, #buttonConvert").hide();
                jq("#mainDownload, #mainConvert").removeClass("unlockAction");
            }
            jq("#buttomEmptyTrash").show();
            jq("#mainEmptyTrash").addClass("unlockAction");
        }

        if (onlyThirdParty) {
            jq("#buttomDelete, #buttomMoveto").hide();
            jq("#mainDelete, #mainMove").removeClass("unlockAction");
        }

        if (typeof event != "undefined") {
            var e = ASC.Files.Common.fixEvent(event);
            var dropdownItem = jq("#filesActionsPanel");
            var correctionX = document.body.clientWidth - (e.pageX - pageXOffset + dropdownItem.innerWidth()) > 0 ? 0 : dropdownItem.innerWidth();
            var correctionY = document.body.clientHeight - (e.pageY - pageYOffset + dropdownItem.innerHeight()) > 0 ? 0 : dropdownItem.innerHeight();

            dropdownItem.css({
                "top": e.pageY - correctionY,
                "left": e.pageX - correctionX
            });

            dropdownItem.toggle();
        }
    };

    var showActionsPanel = function (event, entryData) {
        if (ASC.Files.Actions.clipGetLink) {
            ASC.Files.Actions.clipGetLink.destroy();
        }

        var e = ASC.Files.Common.fixEvent(event);

        var target = jq(e.srcElement || e.target);

        entryData = entryData || ASC.Files.UI.getObjectData(target);

        var entryObj = entryData.entryObject;
        if (entryObj.is(".loading")) {
            return true;
        }

        ASC.Files.Actions.currentEntryData = entryData;

        ASC.Files.UI.checkSelectAll(false);
        ASC.Files.UI.selectRow(entryObj, true);
        ASC.Files.UI.updateMainContentHeader();

        var accessibleObj = ASC.Files.UI.accessibleItem(entryObj);
        var accessAdminObj = ASC.Files.UI.accessAdmin(entryObj);

        jq("#actionPanelFolders, #actionPanelFiles").hide();
        if (entryData.entryType === "file") {
            jq("#filesOpen,\
                #filesEdit,\
                #filesDownload,\
                #filesGetLink,\
                #filesShareAccess,\
                #filesUnsubscribe,\
                #filesVersions,\
                #filesMoveto,\
                #filesCopyto,\
                #filesRename,\
                #filesRestore,\
                #filesRemove").css("display", "");

            if (ASC.Files.Utility.GetConvertFormats(entryTitle).length) {
                jq("#filesConvert").show();
            } else {
                jq("#filesConvert").hide();
            }

            var entryTitle = entryData.title;

            if (!ASC.Files.Utility.CanWebView(entryTitle)
                && (typeof ASC.Files.ImageViewer == "undefined" || !ASC.Files.Utility.CanImageView(entryTitle))) {
                jq("#filesOpen").hide();
            }

            var countVersion = entryData.version || 0;
            if (countVersion < 2
                || ASC.Files.Folders.folderContainer == "trash"
                || entryObj.find("#contentVersions").length != 0) {
                jq("#filesVersions").hide();
            } else {
                jq("#filesVersions span").html(countVersion);
            }

            var editingFile = ASC.Files.UI.editingFile(entryObj);
            if (editingFile) {
                jq("#filesVersions,\
                    #filesMoveto,\
                    #filesRename,\
                    #filesRemove").hide();
            }

            if (!ASC.Files.UI.editableFile(entryData)
                || editingFile && !ASC.Files.Utility.CanCoAuhtoring(entryTitle)) {
                jq("#filesEdit").hide();
            }

            if (ASC.Files.Folders.folderContainer == "trash") {
                jq("#filesOpen,\
                    #filesEdit,\
                    #filesGetLink,\
                    #filesShareAccess,\
                    #filesVersions,\
                    #filesMoveto,\
                    #filesCopyto,\
                    #filesRename").hide();

                jq("#filesRemove, #filesRestore").css("display", "");
            } else {
                jq("#filesRestore").hide();
                if (jq.browser.mobile) {
                    jq("#filesGetLink").hide();
                }
            }

            if (!accessibleObj) {
                jq("#filesEdit,\
                    #filesRename").hide();
            }

            if (!accessAdminObj) {
                jq("#filesMoveto,\
                    #filesRemove").hide();
            }

            if (entryObj.is(".without-share *, .without-share")) {
                jq("#filesShareAccess").hide();
            }

            if (ASC.Files.Folders.folderContainer != "forme") {
                jq("#filesUnsubscribe").hide();
            }

            jq("#actionPanelFiles").show();

        } else {
            jq("#foldersOpen,\
                #foldersDownload,\
                #foldersShareAccess,\
                #foldersUnsubscribe,\
                #foldersMoveto,\
                #foldersCopyto,\
                #foldersRename,\
                #foldersRestore,\
                #foldersRemove,\
                #foldersRemoveThirdparty,\
                #foldersChangeThirdparty").css("display", "");

            if (ASC.Files.Folders.folderContainer == "trash") {
                jq("#foldersOpen,\
                    #foldersShareAccess,\
                    #foldersMoveto,\
                    #foldersCopyto,\
                    #foldersRename").hide();
            } else {
                jq("#foldersRestore").hide();
            }

            if (!accessibleObj || ASC.Files.Folders.currentFolder.id == ASC.Files.Constants.FOLDER_ID_PROJECT) {
                jq("#foldersRename,\
                    #foldersRemoveThirdparty,\
                    #foldersChangeThirdparty").hide();
            }

            if (!accessAdminObj) {
                jq("#foldersMoveto,\
                    #foldersRemove,\
                    #foldersRemoveThirdparty,\
                    #foldersChangeThirdparty").hide();
            }

            if (entryObj.is(".without-share *, .without-share")) {
                jq("#foldersShareAccess").hide();
            }

            if (ASC.Files.Folders.folderContainer != "forme") {
                jq("#foldersUnsubscribe").hide();
            }

            if (ASC.Files.ThirdParty && ASC.Files.ThirdParty.isThirdParty(entryObj)) {
                if (ASC.Files.ThirdParty.isThirdParty()
                    || ASC.Files.Folders.currentFolder.id == ASC.Files.Constants.FOLDER_ID_SHARE) {
                    jq("#foldersRemoveThirdparty,\
                        #foldersChangeThirdparty").hide();
                } else {
                    jq("#foldersRemove,\
                        #foldersMoveto").hide();

                    if (entryData.error) {
                        jq("#foldersOpen,\
                            #foldersDownload,\
                            #foldersCopyto,\
                            #foldersRename").hide();
                    }
                }
            } else {
                jq("#foldersRemoveThirdparty,\
                    #foldersChangeThirdparty").hide();
            }

            jq("#actionPanelFolders").show();
        }

        var dropdownItem = jq("#filesActionPanel");

        if (target.is(".menu-small")) {
            entryObj.addClass("row-lonely-select");

            dropdownItem.css(
                {
                    "top": target.offset().top + target.outerHeight(),
                    "left": "auto",
                    "right": jq(window).width() - target.offset().left - target.width() - 2,
                    "margin": "5px -8px 0 0"
                })
                .find(".corner-top").css(
                    {
                        "display": "block"
                    });
        } else {
            var correctionX = document.body.clientWidth - (e.pageX - pageXOffset + dropdownItem.innerWidth()) > 0 ? 0 : dropdownItem.innerWidth();
            var correctionY = document.body.clientHeight - (e.pageY - pageYOffset + dropdownItem.innerHeight()) > 0 ? 0 : dropdownItem.innerHeight();

            dropdownItem.css(
                {
                    "top": e.pageY - correctionY,
                    "left": e.pageX - correctionX,
                    "right": "auto",
                    "margin": "0"
                })
                .find(".corner-top").css(
                    {
                        "display": "none"
                    });
        }

        dropdownItem.toggle();

        if (jq("#filesGetLink").is(":visible")) {
            var url = entryObj.find(".entry-title .name a").prop("href");
            var offsetLink = jq("#filesGetLink").offset();
            var offsetDialog = jq("#filesActionPanel").offset();

            ASC.Files.Actions.clipGetLink = new ZeroClipboard.Client();
            ASC.Files.Actions.clipGetLink.setText(url);
            ASC.Files.Actions.clipGetLink.glue("filesGetLink", "filesGetLink",
                {
                    zIndex: 670,
                    left: offsetLink.left - offsetDialog.left + "px",
                    top: offsetLink.top - offsetDialog.top + "px"
                });

            ASC.Files.Actions.clipGetLink.addEventListener("onComplete", function () {
                ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoCopyLink);
                ASC.Files.Actions.hideAllActionPanels();
            });
            ASC.Files.Actions.clipGetLink.addEventListener("onMouseOver", function () {
                jq("#filesGetLink").children("a").css({ "text-decoration": "underline" });
            });
            ASC.Files.Actions.clipGetLink.addEventListener('onMouseOut', function () {
                jq("#filesGetLink").children("a").css({ "text-decoration": "" });
            });
        }

        return true;
    };

    var onContextMenu = function (event) {
        ASC.Files.Actions.hideAllActionPanels();

        var e = ASC.Files.Common.fixEvent(event);

        if (typeof e == "undefined" || !e) {
            return true;
        }

        var target = jq(e.srcElement || e.target);
        if (target.is("input[type=text]")) {
            return true;
        }

        var entryData = ASC.Files.UI.getObjectData(target);
        if (!entryData) {
            if (target.is("a[href]")) {
                return true;
            }
            return false;
        }
        var entryObj = entryData.entryObject;

        if (entryObj.hasClass("new-folder") || entryObj.hasClass("row-rename") || entryObj.hasClass("error-entry")) {
            return false;
        }

        jq("#filesMainContent .row-over").removeClass("row-over");
        jq("#filesMainContent .row-lonely-select").removeClass("row-lonely-select");
        var count = jq("#filesMainContent .file-row.row-selected").length;

        if (count > 1 && entryObj.hasClass("row-selected")) {
            ASC.Files.Actions.showActionsViewPanel(event);
        } else {
            ASC.Files.Actions.showActionsPanel(event, entryData);
        }

        return false;
    };

    var hideAllActionPanels = function () {
        if (ASC.Files.Actions.clipGetLink) {
            ASC.Files.Actions.clipGetLink.destroy();
        }

        jq(".files-popup-win:visible").hide();
        jq(".studio-action-panel:visible").hide();
        jq("#filesMainContent .file-row.row-lonely-select").removeClass("row-lonely-select");
        ASC.Files.UI.hideEntryTooltip();
    };

    var checkEditFile = function (fileId, winEditor, isNew) {
        isNew = (isNew === true);
        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        if (!isNew && fileObj.hasClass("is-new-for-web-editor")) {
            isNew = true;
            fileObj.removeClass("is-new-for-web-editor");
        }

        ASC.Files.UI.lockEditFile(fileObj, true);
        ASC.Files.UI.checkEditing();

        var url = ASC.Files.Utility.GetFileWebEditorUrl(fileId) + (isNew ? "&new=true" : "");

        if (winEditor && winEditor.location) {
            winEditor.location.href = url;
        } else {
            winEditor = window.open(url, "_blank");
        }

        try {
            var onloadFunction = function () {
                var fileIdLocal = fileId;
                if (fileIdLocal) {
                    ASC.Files.UI.lockEditFileById(fileIdLocal, true);
                    ASC.Files.UI.checkEditing();
                }
            };
            winEditor.onload = onloadFunction;

            if (jq.browser.msie) {
                var bodyIe;
                var checkIeLoaded = function () {
                    bodyIe = winEditor.document.getElementsByTagName("body");
                    if (bodyIe[0] == null) {
                        setTimeout(checkIeLoaded, 10);
                    } else {
                        onloadFunction();
                    }
                };
                checkIeLoaded();
            }
        } catch (ex) {
        }

        try {
            winEditor.onunload = function () {
                if (fileId) {
                    ASC.Files.UI.checkEditing();
                }
            };
        } catch (ex) {
        }

        return ASC.Files.Marker.removeNewIcon("file", fileId);
    };

    return {
        init: init,
        showActionsViewPanel: showActionsViewPanel,
        showActionsPanel: showActionsPanel,
        onContextMenu: onContextMenu,

        checkEditFile: checkEditFile,

        hideAllActionPanels: hideAllActionPanels,

        clipGetLink: clipGetLink,
        currentEntryData: currentEntryData
    };
})();

(function ($) {
    ASC.Files.Actions.init();
    $(function () {

        jq("#filesSelectAll").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.UI.checkSelectAll(true);
        });

        jq("#filesSelectFile, #filesSelectFolder, #filesSelectDocument,\
            #filesSelectPresentation, #filesSelectSpreadsheet, #filesSelectImage").click(function () {
                var filter = this.id.replace("filesSelect", "").toLowerCase();
                ASC.Files.UI.checkSelect(filter);
            });

        jq("#filesDownload, #foldersDownload").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.download(ASC.Files.Actions.currentEntryData.entryType, ASC.Files.Actions.currentEntryData.id);
        });

        jq("#filesRename, #foldersRename").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.rename(ASC.Files.Actions.currentEntryData.entryType, ASC.Files.Actions.currentEntryData.id);
        });

        jq("#filesRemove, #foldersRemove").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.deleteItem(ASC.Files.Actions.currentEntryData.entryType, ASC.Files.Actions.currentEntryData.id);
        });

        jq("#filesShareAccess, #foldersShareAccess").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Share.getSharedInfo(ASC.Files.Actions.currentEntryData.entryType, ASC.Files.Actions.currentEntryData.id, ASC.Files.Actions.currentEntryData.title);
        });

        jq("#filesUnsubscribe, #foldersUnsubscribe").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Share.unSubscribeMe(ASC.Files.Actions.currentEntryData.entryType, ASC.Files.Actions.currentEntryData.id);
        });

        jq("#filesConvert").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Converter.showToConvert(ASC.Files.Actions.currentEntryData.entryObject);
        });

        jq("#filesOpen").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.clickOnFile(ASC.Files.Actions.currentEntryData, false);
            ASC.Files.Actions.currentEntryData.entryObject.removeClass("isNewForWebEditor");
            return false;
        });

        jq("#filesVersions").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.showVersions(ASC.Files.Actions.currentEntryData.entryObject, ASC.Files.Actions.currentEntryData.id);
        });

        jq("#filesEdit").click(function () {
            PopupKeyUpActionProvider.CloseDialog();
            ASC.Files.Folders.clickOnFile(ASC.Files.Actions.currentEntryData, true, ASC.Files.Actions.currentEntryData.version || -1);
        });

        jq("#foldersOpen").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.clickOnFolder(ASC.Files.Actions.currentEntryData.id);
        });

        jq("#foldersRemoveThirdparty").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showDeleteDialog(null, null, null, ASC.Files.Actions.currentEntryData.title, ASC.Files.Actions.currentEntryData);
        });

        jq("#foldersChangeThirdparty").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showChangeDialog(ASC.Files.Actions.currentEntryData);
        });

        jq("#filesMainContent").bind("contextmenu", function (event) {
            return ASC.Files.Actions.onContextMenu(event);
        });

        jq("#filesMainContent").on("click", ".menu-small", ASC.Files.Actions.showActionsPanel);
    });
})(jQuery);