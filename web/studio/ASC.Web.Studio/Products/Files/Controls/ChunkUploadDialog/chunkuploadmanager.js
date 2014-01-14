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

window.ASC.Files.ChunkUploads = (function () {

    //properties
    var chunkUploader = null;
    var convertTimeout = null;
    var isInit = false;
    var uploaderBusy = false;
    var converterBusy = false;
    var tenantQuota = {};
    var successfullyUploadedFiles = 0;
    var convertQueue = new Array();

    //init chunk upload dialog
    var init = function () {
        if (isInit === false) {
            isInit = true;
            setTimeout(function() {
                initTenantQuota();
            }, 3000);
            activateUploader();
            blockDocumentDrop();
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.ChunkUploadCheckConversion, onCheckConvertStatus);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.ChunkUploadGetFileFromServer, onGetFileFromServer);
        }

        if (!("draggable" in document.createElement("span"))) {
            jq("#emptyContainer .emptyContainer_dragDrop").remove();
        }
    };

    var initTenantQuota = function () {
        window.Teamlab.getQuotas({}, {
            success: function (params, data) {
                tenantQuota = data;
                changeFooterText();
                if (chunkUploader) {
                    chunkUploader.settings.max_file_size = tenantQuota.maxFileSize;
                    chunkUploader.refresh();
                }
            }
        });
    };

    var getUploadSession = function (file) {
        var urlFormat = jq.format("{0}files/{1}/upload/create_session.json", ASC.Resources.Master.ApiPath, file.fid);
        var initResponse = null;

        jq.ajax({
            type: 'POST',
            data: {
                fileName: file.name,
                fileSize: file.size
            },
            url: jq.format(urlFormat, file.fid),
            async: false,
            success: function (data) {
                initResponse = data.response;
            },
            error: function (data) {
                var resp = jq.parseJSON(data.responseText);
                initResponse = "{\"success\":false,\"message\":\"" + resp.error.message + "\"}";
            }
        });

        var session = responseToJson(initResponse);
        return session;
    };

    var initDialogView = function () {
        var storageKey = ASC.Files.Constants.storageKeyUploaderCompactView;
        var compactView = ASC.Files.Common.localStorageManager.getItem(storageKey);

        jq("#chunkUploadDialog").toggleClass("min", compactView === true);
        jq("#uploadCompactViewCbx").prop("checked", compactView === true);

        jq(".store-original").prop("checked", (ASC.Files.Common.storeOriginal === true));
    };

    //uploader custom events
    var onUploadStart = function (up) {
        changeHeaderText(ASC.Files.FilesJSResources.Uploading);
        showUploadDialod();
        ASC.Files.ChunkUploads.uploaderBusy = true;
    };

    var onBeforeUpload = function (up, file) {
        var uploadSession = getUploadSession(file);
        if (uploadSession.Success) {
            up.settings.url = uploadSession.Data.location;
        } else {
            onFileUploadingError(uploadSession, file);
        }
    };

    var onFilesAdded = function (up, files) {
        var folderId = ASC.Files.Folders.currentFolder.id;
        var canShare = ASC.Files.Share && ASC.Files.Folders.currentFolder.shareable;

        for (var i = 0; i < files.length; i++) {
            if (correctFile(files[i])) {
                files[i].canShare = canShare;
                files[i].name = ASC.Files.Common.replaceSpecCharacter(files[i].name);
                renderFileRow(files[i], false);
            } else {
                up.removeFile(files[i]);
            }
        }

        for (var j = 0; j < up.files.length; j++) {
            if (!up.files[j].hasOwnProperty("fid")) {
                up.files[j].fid = folderId;
            }
            if (!up.files[j].hasOwnProperty("canShare")) {
                up.files[j].canShare = canShare;
            }
        }

        jq("#abortUploadigBtn").removeClass("display-none");
        up.start();
    };

    var onChunkUploaded = function (up, file, info) {
        var result = responseToJson(info.response);
        if (!result.Success) {
            onFileUploadingError(result, file);
        }
    };

    var onFileProgress = function (up, file) {
        if (file.actionText == ASC.Files.FilesJSResources.FileUploading) {
            updateFileRow(file);
            changeHeaderText();
        }
    };

    var onFileUploaded = function (up, file, info) {
        var result = responseToJson(info.response);
        if (result && result.Success) {
            successfullyUploadedFiles++;
            file.data = result.Data;

            var canConvert = checkFileConvert(file.data);
            var showRow = file.fid == ASC.Files.Folders.currentFolder.id && !ASC.Files.UI.isSettingsPanel();
            var showData = !canConvert || (canConvert && ASC.Files.Common.storeOriginal);

            if (showRow) {
                var message = ASC.Files.FilesJSResources.InfoUploadedSuccess.format(successfullyUploadedFiles);
                ASC.Files.UI.displayInfoPanel(message);
                getFileFromServer(file.data.id, file.data.version, showData);
            }

            if (canConvert) {
                addFileToConvertQueue(file.data.id, file.data.version);
                renderFileRow(file, true);

                if (!ASC.Files.ChunkUploads.converterBusy) {
                    ASC.Files.ChunkUploads.converterBusy = true;
                    checkConvertStatus();
                }
            } else {
                updateFileRow(file);
            }
        } else {
            onFileUploadingError(result, file);
        }
    };

    var onUploadComplete = function (up, files) {
        initTenantQuota();
        changeHeaderText(ASC.Files.FilesJSResources.UploadComplete);
        jq("#abortUploadigBtn").addClass("display-none");
        ASC.Files.ChunkUploads.uploaderBusy = false;
        successfullyUploadedFiles = 0;
    };

    var onError = function (up, error) {
        if (error.code == window.plupload.FILE_SIZE_ERROR || error.code == window.plupload.FILE_EXTENSION_ERROR) {
            if (!correctFile(error.file)) {
                return;
            }
        }
        ASC.Files.UI.displayInfoPanel(error.message, true);
    };

    var onFileUploadingError = function (res, file) {
        ASC.Files.UI.displayInfoPanel(res.Message, true);
        showFileUploadingError(file.id, res.Message);
        onAbortFileUploading(file.id);
    };

    var onDragLeave = function () {
        hideDragHighlight();
    };

    var onDragOver = function (e) {
        var dt = e.originalEvent.dataTransfer;
        if (!canDrop(dt)) {
            return true;
        }

        //bugfix chrome
        if (jq.browser.webkit) {
            dt.dropEffect = "copy";
        }

        showDragHighlight();
        return false;
    };

    var onFilesDrop = function (e) {
        var dt = e.originalEvent.dataTransfer;
        if (!canDrop(dt)) {
            return true;
        }

        hideDragHighlight();
        return false;
    };

    var onAbortFileUploading = function (fileId) {
        for (var i = 0; i < chunkUploader.files.length; i++) {
            var file = chunkUploader.files[i];
            if (file.id == fileId) {
                if (file.status == window.plupload.STARTED) {
                    chunkUploader.stop();
                    chunkUploader.removeFile(file);
                    chunkUploader.start();
                }
                if (file.status == window.plupload.STOPPED) {
                    chunkUploader.removeFile(file);
                }
            }
        }

        if (chunkUploader.files.length == 0)
            onUploadComplete();
    };

    var onAbortUploading = function () {
        chunkUploader.stop();
        chunkUploader.files.clear();
        onUploadComplete();
    };

    var activateUploader = function () {
        if (!window.plupload)
            return false;

        successfullyUploadedFiles = 0;

        var config = {
            drop_element: "mainContent",
            runtimes: "html5,flash,html4",
            max_file_size: tenantQuota.maxFileSize,
            chunk_size: ASC.Files.Constants.CHUNK_UPLOAD_SIZE,
            flash_swf_url: jq("#flashSwfUrl").val(),
            browse_button: "buttonUpload"
        };

        if (ASC.Files.Constants.UPLOAD_FILTER) {
            var ext = ASC.Files.Utility.Resource.ExtsUploadable.join(",").replace(/\./g, '');
            config.filters = [
                { title: "Uploadable Formats", extensions: ext }
            ];
        }

        chunkUploader = new window.plupload.Uploader(config);
        chunkUploader.init();

        chunkUploader.bind('StateChanged', function (up) {
            switch (up.state) {
                case window.plupload.STARTED:
                    onUploadStart(up);
                    break;
                case window.plupload.DONE:
                case window.plupload.FAILED:
                case window.plupload.FILE_SIZE_ERROR:
                case window.plupload.STOPPED:
                    break;
            }
        });

        chunkUploader.bind('UploadProgress', function (up, file) {
            onFileProgress(up, file);
        });

        chunkUploader.bind('FileUploaded', function (up, file, resp) {
            onFileUploaded(up, file, resp);
        });

        chunkUploader.bind('Error', function (up, resp) {
            onError(up, resp);
        });

        chunkUploader.bind('UploadComplete', function (up, files) {
            onUploadComplete(up, files);
        });

        chunkUploader.bind('FilesAdded', function (up, files) {
            onFilesAdded(up, files);
        });

        chunkUploader.bind('BeforeUpload', function (up, file) {
            onBeforeUpload(up, file);
        });

        chunkUploader.bind('ChunkUploaded', function (up, file, info) {
            onChunkUploaded(up, file, info);
        });

        jq("#mainContent")
            .bind("dragenter", function () {
                return false;
            })
            .bind('dragleave', onDragLeave.bind(this))
            .bind("dragover", onDragOver.bind(this))
            .bind("drop", onFilesDrop.bind(this));

        return chunkUploader;
    };

    //get file data
    var responseToJson = function (res) {
        var response = jq.parseJSON(res);
        if (response == null)
            return false;

        var result = {
            Success: response.success,
            Message: response.message
        };

        if (typeof (response.data) == "string") {
            result.Data = jq.parseJSON(response.data);
        } else if (typeof (response.data) == "object") {
            result.Data = response.data;
        }

        return result;
    };

    var correctFile = function (file) {
        var posExt = ASC.Files.Utility.GetFileExtension(file.name);

        if (ASC.Files.Constants.UPLOAD_FILTER && jq.inArray(posExt, ASC.Files.Utility.Resource.ExtsUploadable) == -1) {
            return false;
        }

        var sizeF = (file.size);

        if (sizeF <= 0) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_EmptyFile, true);
            return false;
        }

        if (tenantQuota.maxFileSize < sizeF) {
            ASC.Files.UI.displayInfoPanel(jq.format("{0} ({1})", ASC.Files.FilesJSResources.ErrorMassage_FileSize, FileSizeManager.filesSizeToString(tenantQuota.maxFileSize)), true);
            return false;
        }

        if (tenantQuota.availableSize < sizeF) {
            ASC.Files.UI.displayTariffFileSizeExceed();
            return false;
        }

        return true;
    };

    var getFileDataByObj = function (entryObject) {
        entryObject = jq(entryObject);
        if (!entryObject.is(".fu-row")) {
            entryObject = entryObject.closest(".fu-row");
        }
        if (entryObject.length == 0) {
            return null;
        }

        var fileId = entryObject.attr("id");
        var res = null;

        jq.each(chunkUploader.files, function (i, file) {
            if (file.id == fileId) {
                res = file;
            }
        });

        return res;
    };

    var getFileDataById = function (id, version) {
        var res = null;

        jq.each(chunkUploader.files, function (i, file) {
            if (file.data && file.data.id == id && file.data.version == parseInt(version)) {
                res = file;
            }
        });

        return res;
    };

    var getFileFromServer = function (fileId, fileVersion, showData) {
        ASC.Files.ServiceManager.getFile(ASC.Files.TemplateManager.events.ChunkUploadGetFileFromServer,
            {
                fileId: fileId,
                version: fileVersion,
                show: showData,
                isStringXml: false
            });
    };

    var onGetFileFromServer = function (stringXmlFile, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }
        writeFileRow(params.fileId, stringXmlFile, params.show);
    };

    //uploader row actions
    var renderFileRow = function (file, isConverterRow) {
        if (!file) {
            return false;
        }

        file.fileTypeCssClass = ASC.Files.Utility.getCssClassByFileTitle(file.name, true);
        file.percent = 0;

        if (!isConverterRow) {
            file.actionText = ASC.Files.FilesJSResources.FileUploading;
            file.completeActionText = ASC.Files.FilesJSResources.FileUploaded;
        } else {
            file.actionText = ASC.Files.FilesJSResources.FileConverting;
            file.completeActionText = ASC.Files.FilesJSResources.FileConverted;
        }

        file.showAnim = chunkUploader.runtime == "html4" && !isConverterRow;

        var $newRow = jq("#fileUploaderRowTmpl").tmpl(file); //in bundle var $newRow = jq.tmpl("fileUploaderRowTmpl", file);
        var $exstRow = jq("#" + file.id);

        if ($exstRow.length != 0) {
            $exstRow.replaceWith($newRow);
        } else {
            jq("#uploadFilesTable tbody").append($newRow);
        }

        if (!isConverterRow) {
            jq("#" + file.id).addClass("upload");
        } else {
            jq("#" + file.id).addClass("convert");
        }
    };

    var updateFileRow = function (file) {
        if (!file) {
            return false;
        }

        var $row = jq("#" + file.id);

        $row.find(".progress-slider").css({ "width": file.percent + "%" });
        if (file.status == window.plupload.DONE && file.percent == 100) {
            $row.removeClass("upload")
                .removeClass("convert")
                .addClass("done");

            if (file.hasOwnProperty("convertedData")) {
                $row.find(".fu-title-cell").text(file.convertedData.title);
                $row.find(".fu-title-cell").attr("title", file.convertedData.title);
                $row.find(".ftFile_21").attr("class", ASC.Files.Utility.getCssClassByFileTitle(file.convertedData.title, true));
            }
        }
    };

    var showFileUploadingError = function (fileId, errorText) {
        var $row = jq("#" + fileId);
        var $errorBox = jq("#" + fileId + " .popup_helper");
        $row.removeClass("upload")
            .removeClass("convert")
            .removeClass("done")
            .removeClass("canceled")
            .addClass("error");

        if (!errorText) {
            errorText = ASC.Files.FilesJSResources.UnknownErrorText;
        }

        $errorBox.text(errorText);
    };

    var showFileUploadingCancel = function (fileId) {
        var $row = jq("#" + fileId);
        $row.removeClass("upload")
            .removeClass("convert")
            .removeClass("done")
            .removeClass("error")
            .addClass("canceled");
    };

    //change uploader dialog info text
    var changeHeaderText = function (text) {
        if (!text) {
            var filesCount = chunkUploader.files.length;
            var percentdownloaded = 0;
            if (chunkUploader.total.size > 0) {
                percentdownloaded = parseInt(chunkUploader.total.loaded / chunkUploader.total.size * 100);
            }
            if (percentdownloaded > 0) {
                text = ASC.Files.FilesJSResources.UploadingProgress.format(filesCount, percentdownloaded);
            } else {
                text = ASC.Files.FilesJSResources.Uploading;
            }
        }

        jq("#chunkUploadDialogHeader").text(text);
    };

    var changeFooterText = function () {
        var usedSize = window.FileSizeManager.filesSizeToString(tenantQuota.usedSize);
        var storageSize = window.FileSizeManager.filesSizeToString(tenantQuota.storageSize);
        jq("#chunkUploadDialog .info-container .free-space").text(
            ASC.Files.FilesJSResources.UsedSize.format(usedSize, storageSize)
        );
    };

    //dragdrop
    var blockDocumentDrop = function () {
        jq(document)
            .bind("dragenter", function () {
                return false;
            })
            .bind("dragleave", function () {
                return false;
            })
            .bind("dragover", function (e) {
                var dt = e.originalEvent.dataTransfer;
                if (!dt) {
                    return true;
                }
                dt.dropEffect = "none";
                return false;
            }.bind(this));
    };

    var canDrop = function (dataTransfer) {
        if (chunkUploader.runtime != "html5") {
            return false;
        }

        if (jq.browser.safari) {
            return false;
        }

        if (!dataTransfer) {
            return false;
        }

        if (!ASC.Files.UI.accessibleItem()) {
            return false;
        }

        //file?
        //FF
        if (dataTransfer.types.contains && !dataTransfer.types.contains("Files")) {
            return false;
        }

        //Chrome
        if (dataTransfer.types.indexOf && dataTransfer.types.indexOf("Files") == -1) {
            return false;
        }

        return true;
    };

    var hideDragHighlight = function () {
        jq("#mainContent").removeClass("selected");
    };

    var showDragHighlight = function () {
        jq("#mainContent").addClass("selected");
    };

    //upload file actions
    var abortFileUploading = function (obj) {
        var file = getFileDataByObj(obj);
        if (file != null) {
            showFileUploadingCancel(file.id);
            onAbortFileUploading(file.id);
        }
    };

    var abortUploading = function (clearDialog) {
        if (clearDialog) {
            jq("#uploadFilesTable tbody").html("");
        } else {
            jq("#uploadFilesTable .fu-row:not(.done)").each(function () {
                var fileId = jq(this).attr("id");
                showFileUploadingCancel(fileId);
            });
        }
        onAbortUploading();
    };

    var shareUploadedFile = function (obj) {
        var fileData = getFileDataByObj(obj);
        if (fileData.canShare) {
            var entryData = fileData.convertedData ? fileData.convertedData : fileData.data;
            ASC.Files.Share.getSharedInfo("file", entryData.id, entryData.title);
        }
    };

    var showUploadingErrorText = function (obj) {
        var row = jq(obj).closest(".fu-row");
        var rowId = row.attr("id");
        jq(obj).helper({
            BlockHelperID: rowId + ' .popup_helper',
            position: 'fixed'
        });
    };

    var disableBrowseButton = function (disable) {
        chunkUploader.disableBrowse(disable);
    };

    //convert functions
    var checkFileConvert = function (fileData) {
        if (!ASC.Files.Constants.ENABLE_UPLOAD_CONVERT) {
            return false;
        }

        return ASC.Files.Utility.MustConvert(fileData.title);
    };

    var addFileToConvertQueue = function (id, version) {
        ASC.Files.Marker.removeNewIcon("file", id);

        ASC.Files.ChunkUploads.convertQueue.push({
            id: id,
            version: version,
            needToRun: true
        });
    };

    var removeFileFromConvertQueue = function (id, version) {
        var itemIndex = -1;

        jq.each(ASC.Files.ChunkUploads.convertQueue, function (index, item) {
            if (item.id == id && item.version == version) {
                itemIndex = index;
            }
        });

        if (itemIndex != -1) {
            ASC.Files.ChunkUploads.convertQueue.splice(itemIndex, 1);
        }
    };

    var changeConvertQueueItemStatus = function (id, version) {
        jq.each(ASC.Files.ChunkUploads.convertQueue, function (index, item) {
            if (item.id == id && item.version == version) {
                item.needToRun = false;
            }
        });
    };

    var checkConvertStatus = function () {
        var data = {};
        data.entry = new Array();

        jq.each(ASC.Files.ChunkUploads.convertQueue, function (index, item) {
            data.entry.push({
                entry: [item.id, item.version, item.needToRun === true]
            });
        });

        if (data.entry.length > 0) {
            ASC.Files.ServiceManager.request("post",
                "json",
                ASC.Files.TemplateManager.events.ChunkUploadCheckConversion,
                {},
                { stringListList: data },
                "checkconversion");
        } else {
            ASC.Files.ChunkUploads.converterBusy = false;
        }
    };

    var onCheckConvertStatus = function (obj, params, errorMessage) {
        if (typeof obj !== "object" && typeof errorMessage != "undefined" || obj == null) {
            errorMessage = errorMessage || ASC.Files.FilesJSResources.ErrorMassage_ErrorConvert;
        } else if (!obj.length) {
            errorMessage = ASC.Files.FilesJSResources.ErrorMassage_ErrorConvert;
        }

        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            clearTimeout(convertTimeout);
            jq.each(ASC.Files.ChunkUploads.convertQueue, function (index, item) {
                var fileData = getFileDataById(item.id, item.version);
                if (fileData) {
                    showFileUploadingError(fileData.id, errorMessage);
                }
            });
            showFileData();
            ASC.Files.ChunkUploads.convertQueue.clear();
            ASC.Files.ChunkUploads.converterBusy = false;
            return false;
        }

        for (var i = 0; i < obj.length; i++) {
            var source = jq.parseJSON(obj[i].source);
            var file = getFileDataById(source.id, source.version);
            var percent = 0;
            var isDone = false;
            var fileResult = null;

            changeConvertQueueItemStatus(source.id, source.version);

            if (obj[i].error != "") {
                ASC.Files.UI.displayInfoPanel(obj[i].error, true);
                removeFileFromConvertQueue(source.id, source.version);
                if (file) {
                    showFileUploadingError(file.id, obj[i].error);
                    showFileData(file.data.id);
                }
            } else {
                try {
                    percent = obj[i].progress;
                    fileResult = jq.parseJSON(obj[i].result);

                    if (fileResult) {
                        var fileObj = ASC.Files.UI.getEntryObject("file", fileResult.file.id);
                        var odjData = ASC.Files.UI.getObjectData(fileObj);
                    }

                    if (odjData != null && odjData.version == parseInt(fileResult.file.version)) {
                        percent = 100;
                        isDone = true;
                    } else if (percent == 100 && fileResult) {
                        if (fileResult.folderId == ASC.Files.Folders.currentFolder.id && !ASC.Files.UI.isSettingsPanel()) {
                            var stringXmlFile = fileResult.file.fileXml;
                            writeFileRow(fileResult.file.id, stringXmlFile, true);
                        }
                        if (!ASC.Files.Common.storeOriginal) {
                            ASC.Files.UI.getEntryObject("file", source.id).remove();
                        } else {
                            showFileData(source.id);
                        }
                        percent = 100;
                        isDone = true;
                    }

                    if (isDone) {
                        removeFileFromConvertQueue(source.id, source.version);
                    }

                    if (file) {
                        file.percent = percent;
                        if (isDone && fileResult) {
                            file.convertedData = fileResult.file;
                            if (!ASC.Files.Common.storeOriginal) {
                                delete file.data;
                            }
                        }
                        updateFileRow(file);
                    }
                } catch (e) {
                }
            }
        }

        if (ASC.Files.ChunkUploads.convertQueue.length == 0) {
            ASC.Files.ChunkUploads.converterBusy = false;
            clearTimeout(convertTimeout);
        } else {
            convertTimeout = setTimeout(checkConvertStatus, ASC.Files.Constants.REQUEST_CONVERT_DELAY);
        }
    };

    //upload dialog actions
    var showUploadDialod = function () {
        if (jq("#chunkUploadDialog:visible").length == 0) {
            ASC.Files.Actions.hideAllActionPanels();
            initDialogView();
            jq("#chunkUploadDialog").removeClass("display-none");
        }
    };

    var closeUploadDialod = function () {
        ASC.Files.ChunkUploads.abortUploading(true);
        jq("#chunkUploadDialog").addClass("display-none");
    };

    var minimizeUploadDialod = function () {
        jq("#chunkUploadDialog").addClass("min");
    };

    var maximizeUploadDialod = function () {
        jq("#chunkUploadDialog").removeClass("min");
    };

    var toggleUploadDialod = function () {
        if (jq("#chunkUploadDialog").hasClass("min")) {
            maximizeUploadDialod();
        } else {
            minimizeUploadDialod();
        }
    };

    var changeCompactView = function (obj) {
        var isCompact = jq(obj).prop("checked");
        var storageKey = ASC.Files.Constants.storageKeyUploaderCompactView;
        ASC.Files.Common.localStorageManager.setItem(storageKey, isCompact);
    };

    var writeFileRow = function (fileId, stringXmlFile, showData) {
        var params = {
            fileId: fileId,
            show: showData,
            isStringXml: true
        };

        var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
        if (fileObj.length != 0) {
            ASC.Files.EventHandler.onReplaceVersion(stringXmlFile, params);
        } else {
            ASC.Files.EventHandler.onGetFile(stringXmlFile, params);
        }
    };

    var showFileData = function (fileId) {
        if (fileId) {
            var fileObj = ASC.Files.UI.getEntryObject("file", fileId);
            if (fileObj.length != 0 && fileObj.hasClass("new-file")) {
                ASC.Files.EmptyScreen.hideEmptyScreen();
                fileObj.removeClass("new-file").show().yellowFade();
            }
        } else {
            var list = jq("#filesMainContent .new-file");
            if (list.length > 0) {
                ASC.Files.EmptyScreen.hideEmptyScreen();
                list.removeClass("new-file").show().yellowFade();
            }
        }
    };

    return {
        uploaderBusy: uploaderBusy,
        converterBusy: converterBusy,
        convertQueue: convertQueue,

        init: init,
        initTenantQuota: initTenantQuota,

        abortFileUploading: abortFileUploading,
        abortUploading: abortUploading,

        shareUploadedFile: shareUploadedFile,
        showUploadingErrorText: showUploadingErrorText,

        disableBrowseButton: disableBrowseButton,

        showUploadDialod: showUploadDialod,
        closeUploadDialod: closeUploadDialod,
        minimizeUploadDialod: minimizeUploadDialod,
        maximizeUploadDialod: maximizeUploadDialod,
        toggleUploadDialod: toggleUploadDialod,

        changeCompactView: changeCompactView
    };
})();

(function ($) {

    $(function () {

        ASC.Files.ChunkUploads.init();

        ASC.Files.Common.storeOriginal = jq("#chunkUploadDialog .store-original").prop("checked");

        jq("#chunkUploadDialog .actions-container.close").click(function () {
            ASC.Files.ChunkUploads.closeUploadDialod();
        });

        jq("#chunkUploadDialog .actions-container.minimize").click(function () {
            ASC.Files.ChunkUploads.minimizeUploadDialod();
        });

        jq("#chunkUploadDialog .actions-container.maximize").click(function () {
            ASC.Files.ChunkUploads.maximizeUploadDialod();
        });

        jq("#chunkUploadDialog").on("dblclick", ".progress-dialog-header", function () {
            ASC.Files.ChunkUploads.toggleUploadDialod();
        });

        jq("#chunkUploadDialog").on("click", ".fu-row.done .share", function () {
            ASC.Files.ChunkUploads.shareUploadedFile(this);
        });

        jq("#chunkUploadDialog").on("click", ".fu-row.upload .abort-file-uploadig", function () {
            ASC.Files.ChunkUploads.abortFileUploading(this);
        });

        jq("#chunkUploadDialog").on("click", "#abortUploadigBtn", function () {
            ASC.Files.ChunkUploads.abortUploading(false);
        });

        jq("#chunkUploadDialog").on("click", ".upload-error", function () {
            ASC.Files.ChunkUploads.showUploadingErrorText(this);
        });

        jq.dropdownToggle({
            switcherSelector: "#uploadSettingsSwitcher",
            dropdownID: "uploadSettingsPanel",
            afterShowFunction: function () {
                //hack to dialog with position:fixed
                var pos = jq("#uploadSettingsSwitcher").position();
                var height = jq("#uploadSettingsSwitcher").height() + 5;
                jq("#uploadSettingsPanel").css({ top: pos.top + height + "px", left: pos.left + "px" });
            }
        });

        jq("#chunkUploadDialog").on("change", "#uploadCompactViewCbx", function () {
            ASC.Files.ChunkUploads.changeCompactView(this);
        });

        jq("#chunkUploadDialog .files-container").scroll(function () {
            ASC.Files.Actions.hideAllActionPanels();
        });
    });

})(jQuery);