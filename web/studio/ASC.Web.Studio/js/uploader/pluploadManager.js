
String.prototype.hashCode = function() {
    if (this.length == 0) return 0;
    var hash = 0;
    var c;
    for (var i = 0; i < this.length; i++) {
        c = this.charCodeAt(i);
        hash = 31 * hash + c;
        hash = hash & hash; // Convert to 32bit integer
    }
    return hash;
};

if (typeof (ASC) == 'undefined')
    ASC = {};

if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = {};

if (typeof(ASC.Controls.FileUploaderGlobalConfig) == 'undefined')
    ASC.Controls.FileUploaderGlobalConfig = {};

var UPLOADER_STATUS =
{
    FILE_ADDED: 0,
    FILE_UPLOADING: 1,
    FILE_UPLOAD_COMPLETE: 2,

    ERROR: 100,
    FILE_ERROR_SIZE: 101,
    FILE_ERROR_TYPE: 102
};

var FileUploadManager = new function() {

    this.renderedItemTemplate = "asc_html_itemTemplate_uploader";
    this.renderedProgressTemplate = "asc_html_progressTemplate_uploader";

    this.UploadedFiles = [];

    this.config = '';
    this.templatedData = { id: '', fileName: '', fileNameCss: '', returnCode: 0, progressBarCss: '', descriptionCss: '', deleteCss: '', loadingImageCss: '', completeCss: '', descriptionText: '', exceedLimit: false, fileSize: 0, progressBarWidth: 0, status: '', serverData: {}, addon: {} };

    this.autoUpload = false;
    this.deleteAfterUpload = false;
    this._uploader = null;

    this.maxSize = 0;
    this.SubmitUrl = '';

    this.check_ready = true;
    this.file_stek = new Array();

    this.UploadedFilesCount = 0;

    this.FilesToUpload = new Array();
    this.FilesToRemove = new Array();

    /* */
    this.OnFileProgress = null;
    this.OnPreUploadStart = null;
    this.OnUploadStart = null;
    this.OnUploadComplete = null;
    this.OnPostUploadComplete = null;
    this.OnCustomParseUpload = null;
    this.OnCustomCalculateAttachments = null;
    this.OnFilesAdd = null;
    this.OnFileUploaded = null;
    this.OnFileRemove = null;
    this.OnError = null;
    /* */

    this.GenerateSubmitUrl = function() {
        var submitUrl = 'UploadProgress.ashx?submit=' + FileUploadManager.config.FileUploadHandler;
        submitUrl += '&asc_auth_key=' + encodeURIComponent(getCookie("asc_auth_key"));
        var data = FileUploadManager.config.Data;
        for (var prop in data) {
            submitUrl += '&{0}={1}'.format(prop, data[prop]);
        }
        return submitUrl;
    };

    this.InitFileUploader = function(config) {
        if (jq.browser.mobile)
            return null;

        if (config.Events == null || config.Events == undefined)
            config.Events = {};

        FileUploadManager.config = config;
        FileUploadManager.maxSize = FileUploadManager.config.MaxSize / 1024 / 1024;
        FileUploadManager.deleteAfterUpload = config.DeleteAfterUpload;

        var uploader = new plupload.Uploader({
            runtimes: (config.Runtimes == null || config.Runtimes == undefined || config.Runtimes == "") ? "html5,flash,html4" : config.Runtimes,
            browse_button: config.BrowseButton,
            //upload_button: config.UploadButton,
            container: config.Container,
            max_file_size: config.MaxSize,
            url: FileUploadManager.GenerateSubmitUrl(),
            flash_swf_url: config.SwfFile,
            drop_element: config.DropPanel,
            filters: config.Filters == null ? [] : config.Filters
        });

        uploader.init();

        if (config.UploadButton == null || config.UploadButton == undefined) {
            FileUploadManager.autoUpload = true;
        }

        if (FileUploadManager.config.DropPanel != null ) {
            jq('#' + FileUploadManager.config.DropPanel)
		       .bind("dragenter", function() { return false; })//simply process idle event
		       .bind('dragleave', FileUploadManager.OnDragLeave.bind(this))//extinguish lights
	            .bind("dragover", FileUploadManager.OnDragOver.bind(this))//illuminated area for the cast
		       .bind("drop", FileUploadManager.OnFilesDrop.bind(this)); //handler is throwing on the field
        }

        this.BindSystemEvents(uploader);
        this.BindEvents(config.Events);

        jq('#' + FileUploadManager.config.UploadButton).click(function(e) {
            uploader.start();
            e.preventDefault();
        });

        this.RenderUploader();

        if (FileUploadManager.config.OverAllProcessHolder != null) {
            jq(FileUploadManager.config.OverAllProcessHolder).html('');
            jq(FileUploadManager.config.OverAllProcessHolder).append(jq.tmpl(FileUploadManager.renderedProgressTemplate, { overallProgressText: FileUploadManager.config.OverallProgressText, overAllProcessBarCssClass: FileUploadManager.config.OverAllProcessBarCssClass }));
            jq("div." + FileUploadManager.config.OverAllProcessBarCssClass + " div").css("width", "0%");
            jq("td.fu_percent").text("0%");
        }
        this._uploader = uploader;
        FileUploadManager.RenderSwitcher();
    };

    this.BindEvents = function(events) {
        if (events.OnFileProgress == null || events.OnFileProgress == undefined)
            FileUploadManager.OnFileProgress = FileUploadManager.FileProgress;
        else
            FileUploadManager.OnFileProgress = events.OnFileProgress;

        if (events.OnUploadStart == null || events.OnUploadStart == undefined)
            FileUploadManager.OnUploadStart = FileUploadManager.UploadStart;
        else
            FileUploadManager.OnUploadStart = events.OnUploadStart;

        if (events.OnUploadComplete == null || events.OnUploadComplete == undefined)
            FileUploadManager.OnUploadComplete = FileUploadManager.UploadComplete;
        else
            FileUploadManager.OnUploadComplete = events.OnUploadComplete;

        if (events.OnFilesAdd == null || events.OnFilesAdd == undefined)
            FileUploadManager.OnFilesAdd = FileUploadManager.FilesAdd;
        else
            FileUploadManager.OnFilesAdd = events.OnFilesAdd;

        if (events.OnFileUploaded == null || events.OnFileUploaded == undefined)
            FileUploadManager.OnFileUploaded = FileUploadManager.FileUploaded;
        else
            FileUploadManager.OnFileUploaded = events.OnFileUploaded;

        if (events.OnError == null || events.OnError == undefined)
            FileUploadManager.OnError = FileUploadManager.Error;
        else
            FileUploadManager.OnError = events.OnError;

        if (events.OnPostUploadComplete != null && events.OnPostUploadComplete != undefined)
            FileUploadManager.OnPostUploadComplete = events.OnPostUploadComplete;

        if (events.OnPreUploadStart != null && events.OnPreUploadStart != undefined)
            FileUploadManager.OnPreUploadStart = events.OnPreUploadStart;

        if (events.OnCustomParseUpload != null && events.OnCustomParseUpload != undefined)
            FileUploadManager.OnCustomParseUpload = events.OnCustomParseUpload;

        if (events.OnCustomCalculateAttachments != null && events.OnCustomCalculateAttachments != undefined)
            FileUploadManager.OnCustomCalculateAttachments = events.OnCustomCalculateAttachments;
    }

    this.IsFlash = function(uploader) {
        if (uploader == null)
            return false;
        return uploader.runtime == "flash" || uploader.runtime == "";
    }

    this.RenderSwitcher = function() {
        if (FileUploadManager.config.Switcher == null)
            return;

        if (FileUploadManager.IsFlash(FileUploadManager._uploader))
            jq("#switcher").html(FileUploadManager.config.Switcher.ToBasic);
        else
            jq("#switcher").html(FileUploadManager.config.Switcher.ToFlash);

    }

    this.BindSystemEvents = function(uploader) {
        uploader.bind('StateChanged', function(up) {
            switch (up.state) {
                case plupload.STARTED:
                    if (FileUploadManager.OnPreUploadStart != null)
                        FileUploadManager.OnPreUploadStart();

                    FileUploadManager.OnUploadStart(up);
                    break;
                case plupload.DONE:
                    break;
                case plupload.FAILED:
                    break;

                case plupload.FILE_SIZE_ERROR:
                    break;
                case plupload.STOPPED:
                    break;
            }
        });

        uploader.bind('UploadProgress', function(up, file) {
            FileUploadManager.OnFileProgress(up, file);
        });

        uploader.bind('FileUploaded', function(up, file, resp) {
            FileUploadManager.OnFileUploaded(up, file, resp);
        });

        uploader.bind('Error', function(up, resp) {
            FileUploadManager.OnError(up, resp);
        });

        uploader.bind('UploadComplete', function(up, files) {
            FileUploadManager.OnUploadComplete(up, files);

            if (FileUploadManager.OnPostUploadComplete != null)
                FileUploadManager.OnPostUploadComplete();

        });

        uploader.bind('FilesAdded', function(up, files) {
            FileUploadManager.OnFilesAdd(up, files);
        });

        // remove file binding
        jq(document).on('click', '#' + FileUploadManager.config.TargetContainerID + ' .fu_item_delete', function() {
            var itemId = "fu_item_" + this.id.substring(this.id.lastIndexOf("_") + 1, this.id.length);
            var name = jq("#" + itemId + " .name").attr("title");
            var removedfile = null;
            jq.each(uploader.files, function(i, file) {
                if (file.name == name) {
                    removedfile = file;
                    try
                    {
                        uploader.removeFile(file);
                    }
                    catch(e) {
                        uploader.removeFile(file);
                    }
                    jq('#' + FileUploadManager.config.TargetContainerID + " #" + itemId).remove();

                    if (FileUploadManager.config.Events.OnFileRemove != null)
                        FileUploadManager.config.Events.OnFileRemove(uploader, removedfile);

                    return false;
                }
            });
            return false;
        });
    }

    this.FilesAdd = function(up, files) {
        jq.each(files, function(i, file) {
            var renderedData = FileUploadManager.PrepareCommonDataToRender(file, UPLOADER_STATUS.FILE_ADDED, null);
            FileUploadManager.RenderItemToUpload(renderedData);

            if (FileUploadManager.autoUpload) {
                jq('#fu_item_loading_{0}'.format(renderedData.id)).show();
                jq('#fu_item_delete_{0}'.format(renderedData.id)).hide();
            }
        });

        if (FileUploadManager.autoUpload)
            up.start();
        //up.refresh(); // Reposition Flash/Silverlight
    };

    this.FileUploaded = function(up, file, resp) {

        FileUploadManager.UploadedFilesCount += 1;

        if (FileUploadManager.config.FilesHeaderCountHolder != null) {
            jq(FileUploadManager.config.FilesHeaderCountHolder).text(FileUploadManager.config.FilesHeaderText.format(FileUploadManager.UploadedFilesCount, FileUploadManager.GetUploadFileCount()));
        }
        FileUploadManager.RenderItemToUpload(FileUploadManager.PrepareCommonDataToRender(file, UPLOADER_STATUS.FILE_UPLOAD_COMPLETE, resp));

        if (FileUploadManager.config.OverAllProcessHolder != null) {
            var percent = Math.round(FileUploadManager.UploadedFilesCount * 100 / FileUploadManager.GetUploadFileCount());
            jq("div." + FileUploadManager.config.OverAllProcessBarCssClass + " div").css("width", percent + "%");
            jq("td.fu_percent").text(percent + "%");
            jq(FileUploadManager.config.OverAllProcessHolder).show();
        }

        FileUploadManager.CalculateAttachments(JSON.parse(resp.response));
    };

    this.FileProgress = function(up, file) {
        var renderedData = FileUploadManager.PrepareCommonDataToRender(file, UPLOADER_STATUS.FILE_UPLOADING, null);
        FileUploadManager.RenderItemToUpload(renderedData);
    };

    this.UploadStart = function(up) {
        jq(FileUploadManager.config.OverAllProcessHolder).show();
    };

    this.UploadComplete = function(up, files) {
        jq("div." + FileUploadManager.config.OverAllProcessBarCssClass + " div").css("width", "100%");
        jq("td.fu_percent").text("100%");
        FileUploadManager.RemoveFileExceedingLimit(up);
    };

    this.Error = function(up, resp) {
        var file = { name: resp.file.name, size: resp.file.size, percent: 0, status: UPLOADER_STATUS.FILE_ERROR };
        FileUploadManager.FilesToRemove.push((resp.file.name + resp.file.size).hashCode());
        FileUploadManager.RenderItemToUpload(FileUploadManager.PrepareCommonDataToRender(file, UPLOADER_STATUS.FILE_ERROR, null));
        FileUploadManager._uploader.refresh();
        FileUploadManager.RemoveFileExceedingLimit(up);
    };

    this.SwitchMode = function() {

        var isFlash = FileUploadManager.IsFlash(FileUploadManager._uploader);
        FileUploadManager.config.Runtimes = isFlash ? "html4" : "flash,html4";
        if (isFlash) {
            jq("#" + FileUploadManager.config.Container + " div.flash").remove();
        }
        else {
            jq("#" + FileUploadManager.config.Container + "files_uploadDialogContainer form").remove();
        }
	  
        FileUploadManager.InitFileUploader(FileUploadManager.config);
        FileUploadManager._uploader.refresh();
        FileUploadManager.RenderSwitcher();
    }

    this.RemoveFileExceedingLimit = function(up) {
        setTimeout(function() {
            for (var i = 0; i < FileUploadManager.FilesToRemove.length; i++) {
                FileUploadManager.RemoveDisplayedFile(FileUploadManager.FilesToRemove[i], up);
            }
            FileUploadManager.FilesToRemove = [];
        }, 3000);
        setTimeout(function() { FileUploadManager._uploader.refresh(); }, 3500);
    };

    this.RemoveDisplayedFile = function(fileID, up) {
        jq('#fu_item_' + fileID).animate({ height: 'hide', opacity: 'hide' }, 200);
        setTimeout(function() { jq('#fu_item_' + fileID).remove(); up.refresh(); }, 200);
    };

    //extinguish lights
    this.OnDragLeave = function() {
        return FileUploadManager.HideDragHighlight();
    };
    //highlights
	this.OnDragOver = function (e) {
		if (FileUploadManager._uploader.features.dragdrop) {
			FileUploadManager.ShowDragHighlight();
		}

		if (jQuery.browser.safari) {
			return true;
		}
		return false;
	};
    //casting process
    this.OnFilesDrop = function(e) {
        FileUploadManager.HideDragHighlight();

        return false;
    };

	this.HideDragHighlight = function () {
		if (FileUploadManager.config.DropPanel != null) {
			jq("#" + FileUploadManager.config.DropPanel).css(FileUploadManager.config.HideDragHighlightStyle == null ? {
				"border-style": "solid",
				"border-color": "#d1d1d1",
				"background-color": "#FFF"
			} : FileUploadManager.config.HideDragHighlightStyle);
		}
	};

	this.ShowDragHighlight = function () {
		if (FileUploadManager.config.DropPanel != null) {
			jq("#" + FileUploadManager.config.DropPanel).css(FileUploadManager.config.ShowDragHighlightStyle == null ? {
				"border-style": "dashed",
				"border-color": "#98BCA1",
				"background-color": "#E0FFE0"
			} : FileUploadManager.config.ShowDragHighlightStyle);
		}
	};

    this.GetUploadFileCount = function() {
        if (FileUploadManager.autoUpload && FileUploadManager.config.TargetContainerID) {
            return jq('#' + FileUploadManager.config.TargetContainerID).children().filter('[id^="fu_item_"]').length;
        }

        return FileUploadManager.FilesToUpload.length;
    };

    this.GetFileSize = function(resp, file) {
        if (!(file.size == null || file.size == ""))
            return file.size;
        if (resp != null && resp.Data != null)
        {
            if (resp.Data.Size != null)
                return resp.Data.Size;

            if (resp.Data.size != null)
                return resp.Data.size;
        }
        return 0;
    }

    this.PrepareCommonDataToRender = function(file, status, resp) {
        var renderedData = FileUploadManager.templatedData;
        if (resp != null)
            renderedData.serverData = JSON.parse(resp.response);
        renderedData.id = (file.name + file.size).hashCode();
        renderedData.fileName = file.name;
        renderedData.returnCode = status == undefined ? 100 : status;
        renderedData.descriptionCss = ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass;
        renderedData.deleteCss = FileUploadManager.config.DeleteLinkCSSClass || ASC.Controls.FileUploaderGlobalConfig.DeleteLinkCSSClass || '';
        renderedData.loadingCss = FileUploadManager.config.LoadingImageCSSClass || ASC.Controls.FileUploaderGlobalConfig.LoadingImageCSSClass || '';
        renderedData.completeCss = FileUploadManager.config.CompleteCSSClass || ASC.Controls.FileUploaderGlobalConfig.CompleteCSSClass || '';
        renderedData.descriptionText = '';

        if (status == UPLOADER_STATUS.FILE_ERROR_SIZE)
            renderedData.exceedLimit = true;
        renderedData.fileSize = FileUploadManager.GetFileSize(resp == null ? null : JSON.parse(resp.response), file);
        renderedData.progressBarWidth = file.percent + '%';

        renderedData.status = (status == undefined ? "" : status);

        var barClass;
        if (renderedData.status == undefined || renderedData.status > 100)
            barClass = "";
        else
            barClass = (FileUploadManager.config.ProgressBarCSSClass || ASC.Controls.FileUploaderGlobalConfig.ProgressBarCSSClass);
        renderedData.progressBarCss = barClass;

        return renderedData;
    }

    this.RenderUploader = function() {
	      
        if (FileUploadManager.autoUpload) {
            jq(FileUploadManager.config.UploadButton).hide();
        }

        jQuery.template(FileUploadManager.renderedItemTemplate, FileUploadManager.config.ItemTemplate || ASC.Controls.FileUploaderGlobalConfig.DefaultItemTemplate);
        jQuery.template(FileUploadManager.renderedProgressTemplate, FileUploadManager.config.ProgressTemplate || ASC.Controls.FileUploaderGlobalConfig.DefaultProgressTemplate);

        FileUploadManager.UploadedFilesCount = 0;
    }

    this.RemoveDisplayedFile = function(fileID) {
        jq('#fu_item_' + fileID).animate({ height: 'hide', opacity: 'hide' }, 200);
        setTimeout(function() { jq('#fu_item_' + fileID).remove(); }, 200);
    };

    this.RenderItemToUpload = function(data) {
	    if (jQuery.browser.safari)
		    FileUploadManager.HideDragHighlight();
        var mb = 1024 * 1024;
        var kb = 1024;

        data.exceedLimit = (data.fileSize > FileUploadManager.config.MaxSize);

        if (data.fileSize && data.fileSize > 0 || data.fileSize == 0) {
            if (data.fileSize == 0) {
                data.fileSize = "";
            }
            else if (data.fileSize <= mb)
            //TODO: KB to resource
                data.fileSize = (data.fileSize / kb).toFixed(2) + ' KB';
            else
            //TODO: MB to resource
                data.fileSize = (data.fileSize / mb).toFixed(2) + ' MB';
        }

        if (FileUploadManager.config.OnPrepareRenderData != null) {
            FileUploadManager.config.OnPrepareRenderData(data);
        }

        if (jq('#fu_item_' + data.id).length != 0) {
            jq('#fu_item_' + data.id).replaceWith(jq.tmpl(FileUploadManager.renderedItemTemplate, data));
        }
        else {
            jq('#' + FileUploadManager.config.TargetContainerID).append(jq.tmpl(FileUploadManager.renderedItemTemplate, data));
        }


        if (data.status == undefined) {
            //console.log(data);
        }
        else {
            if (data.status == UPLOADER_STATUS.FILE_UPLOADING) {
                jq('#fu_item_delete_{0}'.format(data.id)).hide();
                jq('#fu_item_loading_{0}'.format(data.id)).show();

                jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(data.id)).width((data.percent * 100).toFixed() + '%').css("background-color", "#C2DFED");
            }


            if (data.status == UPLOADER_STATUS.FILE_UPLOAD_COMPLETE) {

                jq('#fu_item_loading_{0}'.format(data.id)).hide();

                if (data.serverData && data.serverData.Success) {
                    jq('#fu_item_complete_{0}'.format(data.id)).show();

                    if (FileUploadManager.deleteAfterUpload)
                        jq('#fu_item_delete_{0}'.format(data.id)).show();
                    else
                        jq('#fu_item_delete_{0}'.format(data.id)).hide();

                    if (FileUploadManager.config.TransferCompleteSuccessCssStyle != null)
                        jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(data.id)).css(FileUploadManager.config.TransferCompleteSuccessCssStyle);
                    else
                        jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(data.id)).width('100%').css("background-color", "#EDF6FD");

                }
                else {
                    if (FileUploadManager.config.TransferCompleteFailureCssStyle != null)
                        jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(data.id)).css(FileUploadManager.config.TransferCompleteFailureCssStyle);
                    else
                        jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(data.id)).width('100%').css("background-color", "#FFE4C4");

                    FileUploadManager.FilesToRemove.push(data.id);
                }
            }
            jq("#" + FileUploadManager.config.TargetContainerID).scrollTo("#fu_item_" + data.id);
        }
    };

    this.CalculateAttachments = function(serverData) {
        if (FileUploadManager.OnCustomCalculateAttachments) {
            FileUploadManager.OnCustomCalculateAttachments(serverData);
        }
        else {
            if (serverData.CustomData) {
                FileUploadManager.FileUploadManager();
            }
            if (serverData.Data) {
                var file = { fileName: serverData.Data.FileName, size: serverData.Data.Size, offsetPhysicalPath: serverData.Data.OffsetPhysicalPath, contentType: serverData.Data.ContentType };
                FileUploadManager.UploadedFiles.push(file);
            }
        }
    }
}

function getCookie(name) {
    var cookie = " " + document.cookie;
    var search = " " + name + "=";
    var setStr = null;
    var offset = 0;
    var end = 0;
    if (cookie.length > 0) {
        offset = cookie.indexOf(search);
        if (offset != -1) {
            offset += search.length;
            end = cookie.indexOf(";", offset);
            if (end == -1) {
                end = cookie.length;
            }
            setStr = unescape(cookie.substring(offset, end));
        }
    }
    return (setStr);
};