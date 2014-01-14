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

;
window.ASC.Files.Editor = (function () {
    var isInit = false;

    var docIsChanged = false;
    var lockVersion = false;

    var docEditor = null;
    var docServiceParams = null;

    var trackEditTimeout = null;
    var fileSaveAsNew = "";
    var shareLinkParam = "";
    var docKeyForTrack = "";
    var tabId = "";
    var serverErrorMessage = null;
    var editByUrl = false;
    var mustAuth = false;
    var canCreate = true;
    var options = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;
        }

        jq("body").css("overflow-y", "hidden");

        window.onbeforeunload = ASC.Files.Editor.finishEdit;

        ASC.Files.ServiceManager.bind("TrackEditFile", completeTrack);
        ASC.Files.ServiceManager.bind("CanEditFile", completeCanEdit);
    };

    var createFrameEditor = function (serviceParams) {
        jq("#iframeEditor").parents().css("height", "100%").removeClass("clearFix");

        if (serviceParams) {
            var embedded = (serviceParams.type == "embedded");

            var documentConfig = {
                title: serviceParams.file.title,
                url: serviceParams.url,
                fileType: serviceParams.fileType,
                key: serviceParams.key,
                vkey: serviceParams.vkey
            };

            if (ASC.Files.Editor.options) {
                try {
                    documentConfig.options = ASC.Files.Editor.options;
                } catch (e) {
                }
            }

            if (!embedded) {
                documentConfig.info = {
                    author: serviceParams.file.create_by,
                    created: serviceParams.file.create_on
                };
                if (serviceParams.filePath.length) {
                    documentConfig.info.folder = serviceParams.filePath;
                }
                if (serviceParams.sharingSettings.length) {
                    documentConfig.info.sharingSettings = serviceParams.sharingSettings;
                }

                documentConfig.permissions = {
                    edit: serviceParams.canEdit
                };
            }

            var editorConfig = {
                mode: serviceParams.mode,
                canBackToFolder: (serviceParams.folderUrl.length > 0),
                lang: serviceParams.lang,
                canAutosave: !serviceParams.file.provider_key
            };

            if (embedded) {
                editorConfig.embedded = {
                    saveUrl: serviceParams.downloadUrl,
                    embedUrl: serviceParams.embeddedUrl,
                    shareUrl: serviceParams.viewerUrl,
                    toolbarDocked: "top"
                };

                var keyFullscreen = "fullscreen";
                if (location.hash.indexOf(keyFullscreen) < 0) {
                    editorConfig.embedded.fullscreenUrl = serviceParams.embeddedUrl + "#" + keyFullscreen;
                }
            } else {
                editorConfig.canCoAuthoring = true;
                if (ASC.Files.Constants.URL_HANDLER_CREATE) {
                    editorConfig.createUrl = ASC.Files.Constants.URL_HANDLER_CREATE;
                }

                if (serviceParams.sharingSettingsUrl) {
                    editorConfig.sharingSettingsUrl = serviceParams.sharingSettingsUrl;
                }

                editorConfig.templates =
                    jq(serviceParams.templates).map(
                        function (i, item) {
                            return {
                                name: item.Key,
                                icon: item.Value
                            };
                        }).toArray();

                editorConfig.user = {
                    id: serviceParams.user.key,
                    name: serviceParams.user.value
                };

                if (serviceParams.type != "embedded") {
                    var listRecent = getRecentList();
                    if (listRecent && listRecent.length) {
                        editorConfig.recent = listRecent.toArray();
                    }
                }
            }

            var typeConfig = serviceParams.type;
            var documentTypeConfig = serviceParams.documentType;
        }

        var eventsConfig = {
            "onReady": ASC.Files.Editor.readyEditor,
            "onBack": ASC.Files.Editor.backEditor,
            "onDocumentStateChange": ASC.Files.Editor.documentStateChangeEditor,
            "onRequestEditRights": ASC.Files.Editor.requestEditRightsEditor,
            "onSave": ASC.Files.Editor.saveEditor,
            "onError": ASC.Files.Editor.errorEditor
        };

        ASC.Files.Editor.docEditor = new DocsAPI.DocEditor("iframeEditor", {
            width: "100%",
            height: "100%",

            type: typeConfig || "desktop",
            documentType: documentTypeConfig,
            document: documentConfig,
            editorConfig: editorConfig || { canBackToFolder: true },
            events: eventsConfig
        });
    };

    var fixSize = function () {
        var wrapEl = document.getElementById("wrap");
        if (wrapEl) {
            wrapEl.style.height = screen.availHeight + "px";
            window.scrollTo(0, -1);
            wrapEl.style.height = window.innerHeight + "px";
        }
    };

    var readyEditor = function () {
        if (ASC.Files.Editor.serverErrorMessage) {
            docEditorShowError(ASC.Files.Editor.serverErrorMessage);
            return;
        }

        if (checkMessageFromHash()) {
            location.hash = "";
            return;
        }

        if (ASC.Files.Editor.docServiceParams && ASC.Files.Editor.docServiceParams.mode === "edit") {
            ASC.Files.Editor.trackEdit();
        }
    };

    var backEditor = function () {
        clearTimeout(trackEditTimeout);
        var href = ASC.Files.Editor.docServiceParams ? ASC.Files.Editor.docServiceParams.folderUrl : ASC.Files.Constants.URL_FILES_START;
        location.href = href;
    };

    var documentStateChangeEditor = function (event) {
        if (docIsChanged != event.data) {
            document.title = ASC.Files.Editor.docServiceParams.file.title + (event.data ? " *" : "");
            docIsChanged = event.data;
        }
    };

    var errorEditor = function () {
        ASC.Files.Editor.finishEdit();
    };

    var saveEditor = function (event) {
        var urlSavedDoc = event.data;

        if (ASC.Files.Editor.mustAuth) {
            jq(".block-auth").show();
            return true;
        } else if (ASC.Files.Editor.editByUrl) {

            var urlRedirect = ASC.Files.Constants.URL_HANDLER_CREATE;
            urlRedirect += "?action=create";
            urlRedirect += "&title=" + encodeURIComponent(ASC.Files.Editor.docServiceParams.file.title);
            urlRedirect += "&fileUri=" + encodeURIComponent(urlSavedDoc);
            urlRedirect += "&openfolder=true";

            ASC.Files.Editor.docEditor.processSaveResult(true);
            location.href = urlRedirect;
            return false;

        } else {

            var urlAjax = ASC.Files.Constants.URL_HANDLER_SAVE.format(
                encodeURIComponent(ASC.Files.Editor.docServiceParams.file.id),
                ASC.Files.Editor.tabId,
                ASC.Files.Editor.docServiceParams.file.version,
                encodeURIComponent(urlSavedDoc));

            urlAjax += ASC.Files.Editor.fileSaveAsNew;
            urlAjax += ASC.Files.Editor.shareLinkParam;
            urlAjax += "&_=" + new Date().getTime();

            jq.ajax({
                type: "get",
                url: urlAjax,
                complete: completeSave
            });

            return false;
        }
    };

    var requestEditRightsEditor = function () {
        if (ASC.Files.Editor.docServiceParams.linkToEdit) {
            location.href = ASC.Files.Editor.docServiceParams.linkToEdit + ASC.Files.Editor.shareLinkParam;
        } else {
            ASC.Files.ServiceManager.canEditFile("CanEditFile",
                {
                    fileID: ASC.Files.Editor.docServiceParams.file.id,
                    shareLinkParam: ASC.Files.Editor.shareLinkParam
                });
        }
    };

    var trackEdit = function () {
        if (ASC.Files.Editor.editByUrl) {
            return;
        }
        clearTimeout(trackEditTimeout);

        ASC.Files.ServiceManager.trackEditFile("TrackEditFile",
            {
                fileID: ASC.Files.Editor.docServiceParams.file.id,
                tabId: ASC.Files.Editor.tabId,
                docKeyForTrack: ASC.Files.Editor.docKeyForTrack,
                shareLinkParam: ASC.Files.Editor.shareLinkParam,
                lockVersion: ASC.Files.Editor.lockVersion
            });
    };

    var finishEdit = function () {
        if (trackEditTimeout !== null) {
            ASC.Files.ServiceManager.trackEditFile("FinishTrackEditFile",
                {
                    fileID: ASC.Files.Editor.docServiceParams.file.id,
                    tabId: ASC.Files.Editor.tabId,
                    docKeyForTrack: ASC.Files.Editor.docKeyForTrack,
                    shareLinkParam: ASC.Files.Editor.shareLinkParam,
                    finish: true,
                    ajaxsync: true
                });
        }
    };

    var completeSave = function () {
        var responseObj = null;
        var responseText = "";
        try {
            try {
                responseText = arguments[0].responseText;
                if (responseText) {
                    responseObj = jq.parseJSON(responseText);
                }
            } catch (e) {
                responseObj = responseText.split("title>")[1].split("</")[0];
            }
        } catch (e) {
            responseObj = {
                error: true,
                message: "Unknown error on save."
            };
        }
        var errorMessage = null;
        if (arguments[1] == "error" || responseObj && responseObj.error) {
            var saveResult = false;
            errorMessage = responseObj && responseObj.message || responseObj;
        } else {
            saveResult = true;
            ASC.Files.Editor.lockVersion = true;
            ASC.Files.Editor.documentStateChangeEditor({ data: false });
            ASC.Files.Editor.trackEdit();
        }

        ASC.Files.Editor.docEditor.processSaveResult(saveResult === true, errorMessage);
    };

    var completeTrack = function (jsonData, params, errorMessage) {
        clearTimeout(trackEditTimeout);
        if (typeof errorMessage != "undefined") {
            if (errorMessage == null) {
                docEditorShowInfo("Connection is lost");
            } else {
                docEditorShowWarning(errorMessage || "Connection is lost");
            }
            return;
        }

        if (jsonData.key == true) {
            trackEditTimeout = setTimeout(ASC.Files.Editor.trackEdit, 5000/*ASC.Files.Constants.REQUEST_TRACK_DELAY - 1000*/);
        } else {
            errorMessage = jsonData.value;
            ASC.Files.Editor.docEditor.processRightsChange(false, errorMessage);
        }
    };

    var docEditorShowError = function (message) {
        ASC.Files.Editor.docEditor.showMessage("Teamlab Office", message, "error");
    };

    var docEditorShowWarning = function (message) {
        ASC.Files.Editor.docEditor.showMessage("Teamlab Office", message, "warning");
    };

    var docEditorShowInfo = function (message) {
        ASC.Files.Editor.docEditor.showMessage("Teamlab Office", message, "info");
    };

    var checkMessageFromHash = function () {
        var regExpError = /^#error\/(\S+)?/;
        if (regExpError.test(location.hash)) {
            var errorMessage = regExpError.exec(location.hash)[1];
            errorMessage = decodeURIComponent(errorMessage).replace(/\+/g, " ");
            if (errorMessage.length) {
                docEditorShowWarning(errorMessage);
                return true;
            }
        }
        var regExpMessage = /^#message\/(\S+)?/;
        if (regExpMessage.test(location.hash)) {
            errorMessage = regExpMessage.exec(location.hash)[1];
            errorMessage = decodeURIComponent(errorMessage).replace(/\+/g, " ");
            if (errorMessage.length) {
                docEditorShowInfo(errorMessage);
                return true;
            }
        }
        return false;
    };

    var getRecentList = function () {
        if (!ASC.Files.Common.localStorageManager.isAvailable) {
            return null;
        }
        var localStorageKey = ASC.Files.Constants.storageKeyRecent;
        var localStorageCount = 50;
        var recentCount = 10;

        var result = new Array();

        try {
            var recordsFromStorage = ASC.Files.Common.localStorageManager.getItem(localStorageKey);
            if (!recordsFromStorage) {
                recordsFromStorage = new Array();
            }

            if (recordsFromStorage.length > localStorageCount) {
                recordsFromStorage = recordsFromStorage.pop();
            }

            var currentRecord = {
                url: location.href,
                id: ASC.Files.Editor.docServiceParams.file.id,
                title: ASC.Files.Editor.docServiceParams.file.title,
                folder: ASC.Files.Editor.docServiceParams.filePath,
                fileType: ASC.Files.Editor.docServiceParams.fileTypeNum
            };

            var containRecord = jq(recordsFromStorage).is(function () {
                return this.id == currentRecord.id;
            });

            if (!containRecord) {
                recordsFromStorage.unshift(currentRecord);

                ASC.Files.Common.localStorageManager.setItem(localStorageKey, recordsFromStorage);
            }

            result = jq(recordsFromStorage).filter(function () {
                return this.id != currentRecord.id &&
                    this.fileType === currentRecord.fileType;
            });
        } catch (e) {
        }

        return result.slice(0, recentCount);
    };

    var completeCanEdit = function (jsonData, params, errorMessage) {
        var result = typeof jsonData != "undefined";
        // occurs whenever the user tryes to enter edit mode
        ASC.Files.Editor.docEditor.applyEditRights(result, errorMessage);

        if (result) {
            ASC.Files.Editor.tabId = jsonData;
            ASC.Files.Editor.trackEdit();
        }
    };

    return {
        init: init,
        createFrameEditor: createFrameEditor,
        fixSize: fixSize,

        docEditor: docEditor,

        //set in .cs
        docServiceParams: docServiceParams,
        fileSaveAsNew: fileSaveAsNew,
        shareLinkParam: shareLinkParam,
        docKeyForTrack: docKeyForTrack,
        tabId: tabId,
        serverErrorMessage: serverErrorMessage,
        editByUrl: editByUrl,
        mustAuth: mustAuth,
        canCreate: canCreate,
        options: options,

        trackEdit: trackEdit,
        finishEdit: finishEdit,

        //event
        readyEditor: readyEditor,
        backEditor: backEditor,
        documentStateChangeEditor: documentStateChangeEditor,
        requestEditRightsEditor: requestEditRightsEditor,
        errorEditor: errorEditor,
        saveEditor: saveEditor,

        lockVersion: lockVersion
    };
})();

(function ($) {
    ASC.Files.Editor.init();
    $(function () {
        ASC.Files.Editor.createFrameEditor(ASC.Files.Editor.docServiceParams);

        if (jq.browser.mobile || ASC.Files.Editor.docServiceParams && ASC.Files.Editor.docServiceParams.type === "mobile") {
            window.addEventListener("load", ASC.Files.Editor.fixSize);
            window.addEventListener("orientationchange", ASC.Files.Editor.fixSize);
        }

        jq(".block-auth .close").on("click", function () {
            jq(".block-auth").hide();
            ASC.Files.Editor.docEditor.processSaveResult(true);
        });
    });
})(jQuery);

String.prototype.format = function () {
    var txt = this,
        i = arguments.length;

    while (i--) {
        txt = txt.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return txt;
};