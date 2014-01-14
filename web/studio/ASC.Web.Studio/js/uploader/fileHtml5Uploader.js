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

var FileHtml5Uploader = new function() {

    if (!jq("#fileHtml5UploaderDragDropPanel").length) {
        var inlineStyle = "<style id=\"fileHtml5UploaderDragDropPanel\" type=\"text/css\">";
        inlineStyle += ".file-html5-uploader-dragdrop-panel {  border: 1px dashed #BBBBBB; } ";
        inlineStyle += "</style>";
        jq(inlineStyle).appendTo("head");
    }

    this.config = '';

    this.SubmitUrl = '';

    this.check_ready = true;
    this.file_stek = new Array();

    this.EnableHtml5 = true;

    this.UploadedFilesCount = 0;

    this.InitFileUploader = function(conf) {
        if (jq.browser.mobile)
            return null;
        if (typeof FileHtml5Uploader.EnableHtml5Flag != "undefined") {
            this.EnableHtml5 = FileHtml5Uploader.EnableHtml5Flag;
        }
        FileHtml5Uploader.EnableHtml5 = FileHtml5Uploader.EnableHtml5
                                        && ASC.Controls.FlashDetector.DetectFlashVer(8, 0, 0)
                                        && (ASC.Controls.FileUploaderGlobalConfig.DisableFlash != true)
                                        && (conf.disableFlash != true)
                                        && (typeof window.FileReader != 'undefined')
                                        && (typeof (new XMLHttpRequest()).upload != 'undefined');

        if (FileHtml5Uploader.EnableHtml5) {
            FileHtml5Uploader.config = conf;
            FileHtml5Uploader.SubmitUrl = FileHtml5Uploader.GenerateSubmitUrl();
            FileHtml5Uploader.InitHtml5Uploader();
        }

        FileUploader = new ASC.Controls.FileUploader(conf);
        FileUploader.FilesTotalCount = 0;

        if (FileHtml5Uploader.config.FilesHeaderCountHolder != null)
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).text(FileHtml5Uploader.config.SelectFileText + " " + FileHtml5Uploader.config.DragDropText);

        //hide swf object button
        if (FileHtml5Uploader.EnableHtml5) {
            var SWFUploadObject = jq("#SWFUpload_" + (SWFUpload.movieCount - 1));
            if (SWFUploadObject.length != 0)
                SWFUploadObject.css('top', -Math.max(jq("#SWFUpload_" + (SWFUpload.movieCount - 1)).offset().top, 1000));
        }

        return FileUploader;
    };

    this.InitHtml5Uploader = function() {
        jq('#studio_fuplswitcherBox').hide();
        jq('#asc_fileuploaderSWFContainer').hide();

        FileHtml5Uploader.UploadedFilesCount = 0;

        if (FileHtml5Uploader.config.OverAllProcessHolder != null)
            jq(FileHtml5Uploader.config.OverAllProcessHolder).hide();

        setTimeout(function() {
            FileHtml5Uploader.FilesToUpload = new Array(0);

            var buttonText = (FileHtml5Uploader.config.SelectFilesText != null && FileHtml5Uploader.config.SelectFilesText != "" ? FileHtml5Uploader.config.SelectFilesText : jq("#" + FileHtml5Uploader.config.UploadButtonID).text());

            jq('#' + FileHtml5Uploader.config.UploadButtonID).html("<div id='fu_multi_container' style='overflow: hidden; width: 128px; height: 20px; cursor: pointer;'></div><a id='fu_button_select' class='button gray middle' href='javascript:void(0);' style='margin: -20px 0 5px 13px; cursor: pointer;' title='" + buttonText + "'>" + buttonText + "</a>");
            jq('#' + FileHtml5Uploader.config.UploadButtonID).css("padding-top", "15px");
            jq('#' + FileHtml5Uploader.config.UploadButtonID).show();
            jq('#FileMultipleUploadInput').appendTo(jq('#fu_multi_container').html(''));

            if (jq('#FileMultipleUploadInput').length == 0)
                jq('#fu_multi_container').html('<input type="file" onchange="FileHtml5Uploader.OnMultipleInputChange(this.files);" multiple="multiple" id="FileMultipleUploadInput">');

            jq('#FileMultipleUploadInput').css({
                'cursor': 'pointer',
                'direction': 'rtl',
                'float': 'left',
                'font-size': '128px',
                'height': '20px',
                'opacity': '0'
            })

            jq('#FileMultipleUploadInput').show();
            if (FileHtml5Uploader.config.SingleUploader) {
                jq('#FileMultipleUploadInput').removeAttr('multiple');
            }

            if (FileHtml5Uploader.config.OverAllProcessHolder != null) {
                jq(FileHtml5Uploader.config.OverAllProcessHolder).html("<table cellpadding='5' cellspacing='0' width='100%' style='padding:10px 0;' class='describe-text'><tr><td width='100'>" + FileHtml5Uploader.config.OverallProgressText + ": </td><td>" + '<div style="margin-top: 1px;" class="{0}"><div></div></div>'.format(FileHtml5Uploader.config.OverAllProcessBarCssClass) + "</td><td class='fu_percent' width='20'></td></tr></table>");
                jq("div." + FileHtml5Uploader.config.OverAllProcessBarCssClass + " div").css("width", "0%");
                jq("td.fu_percent").text("0%");
            }
        }, 500);

        jq('#FileMultipleUploadInput').bind("dragover", FileHtml5Uploader.OnDragOver.bind(this))//illuminated area for the cast
		   .bind("dragenter", function() { return false; })//simply process idle event
		   .bind('dragleave', FileHtml5Uploader.OnDragLeave.bind(this))//extinguish lights
		   .bind("drop", FileHtml5Uploader.OnFilesDrop.bind(this)); //handler is throwing on the field

        if (FileHtml5Uploader.config.DragDropHolder != null) {
            jq(FileHtml5Uploader.config.DragDropHolder).bind("dragover", FileHtml5Uploader.OnDragOver.bind(this))//illuminated area for the cast
		       .bind("dragenter", function() { return false; })//simply process idle event
		       .bind('dragleave', FileHtml5Uploader.OnDragLeave.bind(this))//extinguish lights
		       .bind("drop", FileHtml5Uploader.OnFilesDrop.bind(this)); //handler is throwing on the field
        }

        //do not give the user to quit the file by the field throwing
        jq(document)
			.bind('dragenter', function() { return false; })
			.bind('dragleave', function() { return false; })
			.bind('dragover', function(e) {
			    var dt = e.originalEvent.dataTransfer;
			    if (!dt) { return; }
			    dt.dropEffect = 'none';
			    return false;
			}
			.bind(this));
    };

    this.FilesToUpload = new Array();

    this.FilesToRemove = new Array();

    this.UploadedFilesCount = 0;

    this.OnMultipleInputChange = function(files) {
        if (!files.length || files.length == 0) {
            return;
        }

        //single uploader mode
        if (FileHtml5Uploader.config.SingleUploader) {

            if (FileHtml5Uploader.config.FileNameFilter != null && FileHtml5Uploader.config.FileNameFilter != ""
			    && FileHtml5Uploader.config.FileNameFilter.toLowerCase() != (files[0].name || files[0].fileName).replace(new RegExp("[\\\\/:*?\"<>|';\+]", 'gim'), '_').toLowerCase()) {
                if (FileHtml5Uploader.config.FileNameExeptionText != null && FileHtml5Uploader.config.FileNameExeptionText != "")
                    alert(FileHtml5Uploader.config.FileNameExeptionText);

                return;
            }

            for (var i = 0; i < FileHtml5Uploader.FilesToUpload.length; i++) {
                jq('#fu_item_delete_' + (FileHtml5Uploader.FilesToUpload[i].name || FileHtml5Uploader.FilesToUpload[i].fileName).hashCode()).click();
            }
            jq('#' + FileHtml5Uploader.config.TargetContainerID).html('');
        }

        //if autosubmit => all files are already loaded
        if (FileHtml5Uploader.config.AutoSubmit || FileHtml5Uploader.config.SingleUploader) {
            FileHtml5Uploader.FilesToUpload.splice(0, FileHtml5Uploader.FilesToUpload.length);
        }

        if (FileHtml5Uploader.config.AddFilesText != "" && FileHtml5Uploader.config.AddFilesText != null)
            jq("#fu_button_select").text(FileHtml5Uploader.config.AddFilesText);

        var uploadedElements = jq('input[type="hidden"][id^="fu_itemName_hidden_"]');

        if (FileHtml5Uploader.config.TargetContainerID != "" && FileHtml5Uploader.config.TargetContainerID != null &&
		    jq('#' + FileHtml5Uploader.config.TargetContainerID + ' div.top-line')[0] == null)
            jq('#' + FileHtml5Uploader.config.TargetContainerID).append("<div style='border-top:1px solid #d1d1d1;' class='top-line'></div>");

        for (var j = 0; j < files.length; j++) {
            var file = files[j];
            var fileID = (file.name || file.fileName).hashCode();
            if (uploadedElements.filter('[id="fu_itemName_hidden_{0}"]'.format(fileID)).length > 0) {
                continue;
            }

            var exceedLimit = false;
            if (FileHtml5Uploader.CheckFileSize(file) && FileHtml5Uploader.CheckFileType(file)) {
                FileHtml5Uploader.FilesToUpload.push(file);
            } else {
                FileHtml5Uploader.FilesToRemove.push(fileID);
                exceedLimit = true;
            }

            FileHtml5Uploader.RenderItemToUpload(file, exceedLimit);
        }

        FileHtml5Uploader.RemoveFileExceedingLimit();

        if (FileHtml5Uploader.config.FilesHeaderCountHolder != null && FileHtml5Uploader.FilesToUpload.length > 0)
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).text(FileHtml5Uploader.config.SelectedFilesText.format(FileHtml5Uploader.FilesToUpload.length));

        if (FileHtml5Uploader.config.AutoSubmit) {
            FileHtml5Uploader.UploadMultipleFiles();
        }

        if (FileHtml5Uploader.FilesToUpload.length != 0 && FileHtml5Uploader.config.SubmitPanelAfterSelectHolder != null)
            jq(FileHtml5Uploader.config.SubmitPanelAfterSelectHolder).show();

        jq("#FileMultipleUploadInput").val("");
    };

    this.RenderItemToUpload = function(data, exceedLimit) {

        var dataName = data.name || data.fileName;
        var id = dataName.hashCode();
        var mb = 1024 * 1024;
        var kb = 1024;

        var sb = '<div id="fu_item_' + id + '" style="margin:0px 0px;">' +
                 '<table cellspacing="0" cellpadding="0" style="width:100%;border-bottom:1px solid #D1D1D1;"><tr style="height:40px !important;" valign="middle">';

        //file name
        sb += '<td style="width:70%; padding-left:10px;"><div class="' + (FileHtml5Uploader.config.FileNameCSSClass || ASC.Controls.FileUploaderGlobalConfig.FileNameCSSClass) + '" style="padding-right:5px;" title="' + dataName + '" >';
        sb += dataName;
        sb += '<input type="hidden" id="fu_itemName_hidden_{0}" value="{0}"/>'.format(id);

        var len = dataName.split('.').length;

        if (FileHtml5Uploader.config.file_types != null && FileHtml5Uploader.config.file_types != "" &&
		    dataName.split('.')[len - 1].toLowerCase().match("(" + FileHtml5Uploader.config.file_types.replace(/; \*./g, ")|(").replace('*.', '').replace(';', '') + ")") == null) {
            data.ReturnCode = 2;
        }

        var barClass;
        if (data.ReturnCode && data.ReturnCode != 0) {
            barClass = (FileHtml5Uploader.config.ErrorBarCSSClass || ASC.Controls.FileUploaderGlobalConfig.ErrorBarCSSClass);
        }
        else {
            barClass = (FileHtml5Uploader.config.ProgressBarCSSClass || ASC.Controls.FileUploaderGlobalConfig.ProgressBarCSSClass);
        }

        if (data.ReturnCode && data.ReturnCode != 0)
            sb += '<div class="' + (FileHtml5Uploader.config.ErrorTextCSSClass || ASC.Controls.FileUploaderGlobalConfig.ErrorTextCSSClass) + '">';
        else
            sb += '<div ' + ((FileHtml5Uploader.config.DescriptionCSSClass || ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass) ? 'class="' + (FileHtml5Uploader.config.DescriptionCSSClass || ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass) + '"' : 'style="padding:3px; font-size:10px; color:gray;"') + '>';

        if (data.ReturnCode && data.ReturnCode == 1) {
            sb += (FileHtml5Uploader.config.ErrorFileSizeLimitText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileSizeLimitText);
            if (typeof FileHtml5Uploader.config.FileSizeLimitExceedCallback == "function") {
                FileHtml5Uploader.config.FileSizeLimitExceedCallback();
            }
        }
        else if (data.ReturnCode && data.ReturnCode == 2) {
            sb += FileHtml5Uploader.config.ErrorFileTypeText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileTypeText;
        }

        sb += '</div>';
        sb += '</td>';

        //size
        sb += '<td style="padding-left: 10px; width:20%; {0}">'.format(exceedLimit ? 'color: red;' : 'color: #83888D;');
        var dataSize = data.fileSize || data.size;
        if (dataSize && dataSize > 0 || dataSize == 0) {
            if (dataSize <= mb)
            //TODO: KB to resource
                sb += (dataSize / kb).toFixed(2) + ' KB';
            else
            //TODO: MB to resource
                sb += (dataSize / mb).toFixed(2) + ' MB';
        }

        sb += '&nbsp;</td>';

        //remove link
        sb += '<td style="width:10%;">&nbsp;';
        sb += '<a id="fu_item_delete_{0}" class="{1}" href="{3};">{2}</a>'.format(id, FileHtml5Uploader.config.DeleteLinkCSSClass || ASC.Controls.FileUploaderGlobalConfig.DeleteLinkCSSClass || '', ''/*FileHtml5Uploader.config.DeleteText || ASC.Controls.FileUploaderGlobalConfig.DeleteText || 'Delete'*/, "javascript:FileHtml5Uploader.RemoveFileFromUpload('{0}')".format(dataName.hashCode()));
        sb += '<a id="fu_item_loading_{0}" class="{1}" style="display:none;"></a>'.format(id, FileHtml5Uploader.config.LoadingImageCSSClass || ASC.Controls.FileUploaderGlobalConfig.LoadingImageCSSClass || '');
        sb += '<a id="fu_item_complete_{0}" class="{1}" style="display:none;"></a>'.format(id, FileHtml5Uploader.config.CompleteCSSClass || ASC.Controls.FileUploaderGlobalConfig.CompleteCSSClass || '');
        sb += '</td>';

        sb += '</tr></table><div class="{0}" style="margin-top:-41px; float:left; width:{1};height:40px;" ></div></div>'.format(barClass, (0 * 100).toFixed() + '%');

        if (jq('#fu_item_' + id).length != 0) {
            jq('#fu_item_' + id).replaceWith(sb);
        }
        else {
            if (data.UploadedItem && data.UploadedItem.TargetContainerID != null) {
                jq('#' + data.UploadedItem.TargetContainerID).append(sb);
            }
            else {
                jq('#' + FileHtml5Uploader.config.TargetContainerID).append(sb);
            }
        }

        if (data.UploadedItem && data.UploadedItem.TargetContainerID != null)
            jq('#' + data.UploadedItem.TargetContainerID).scrollTo("#fu_item_" + id);
        else
            jq('#' + FileHtml5Uploader.config.TargetContainerID).scrollTo("#fu_item_" + id);

        if (data.UploadedItem && data.UploadedItem.RemoveHandler) {
            jq('#fu_item_delete_' + id).bind("click", function() {
                if (data.UploadedItem.RemoveHandler(data.UploadedItem))
                    jq('#fu_item_' + id).remove();

                return false;
            });
        }
        else if (data.HasDelete || (data.ServerData && FileHtml5Uploader.config.AfterUploadDeleteCallback))
            jq('#fu_item_delete_' + id).bind("click", function() {
                if (data.ServerData && FileHtml5Uploader.config.AfterUploadDeleteCallback) {
                    FileHtml5Uploader.config.AfterUploadDeleteCallback(data.ServerData);
                }
                else {
                    if (data.Swf)
                        swfuploader.cancelUpload(data.UploadId);
                    else
                        uploader.RemoveAjaxUploadHandler(data.UploadId);
                }

                jq('#fu_item_' + id).remove();
                return false;
            });
    };

    this.RemoveFileFromUpload = function(fileNameHash) {
        for (var i = 0; i < FileHtml5Uploader.FilesToUpload.length; i++) {
            if ((FileHtml5Uploader.FilesToUpload[i].name || FileHtml5Uploader.FilesToUpload[i].fileName).hashCode() == fileNameHash) {
                FileHtml5Uploader.FilesToUpload.splice(i, 1);
                break;
            }
        }

        if (FileHtml5Uploader.config.FilesHeaderCountHolder != null && FileHtml5Uploader.FilesToUpload.length > 0)
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).text(FileHtml5Uploader.config.SelectedFilesText.format(FileHtml5Uploader.FilesToUpload.length));
        else if (FileHtml5Uploader.config.FilesHeaderCountHolder != null && FileHtml5Uploader.FilesToUpload.length == 0 && FileHtml5Uploader.config.SelectFilesText != null) {
            jq(FileHtml5Uploader.config.SubmitPanelAfterSelectHolder).hide();
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).attr("title", FileHtml5Uploader.config.SelectFileText).text(FileHtml5Uploader.config.SelectFileText);
        }

        FileHtml5Uploader.RemoveDisplayedFile(fileNameHash);
    };

    this.RemoveDisplayedFile = function(fileID) {
        jq('#fu_item_' + fileID).animate({ height: 'hide', opacity: 'hide' }, 200);
        setTimeout(function() { jq('#fu_item_' + fileID).remove(); }, 200);
    };

    this.RemoveFileExceedingLimit = function() {
        setTimeout(function() {
            for (var i = 0; i < FileHtml5Uploader.FilesToRemove.length; i++) {
                FileHtml5Uploader.RemoveDisplayedFile(FileHtml5Uploader.FilesToRemove[i]);
            }
        }, 3000);

        if (FileHtml5Uploader.FilesToUpload.length == 0 && FileHtml5Uploader.config.SubmitPanelAfterSelectHolder != null) {
            jq(FileHtml5Uploader.config.SubmitPanelAfterSelectHolder).hide();
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).text(FileHtml5Uploader.config.SelectFileText);

        }
    };

    this.GetUploadFileCount = function() {
        if (FileHtml5Uploader.config.AutoSubmit && FileHtml5Uploader.config.TargetContainerID) {
            return jq('#' + FileHtml5Uploader.config.TargetContainerID).children().filter('[id^="fu_item_"]').length;
        }

        return FileHtml5Uploader.FilesToUpload.length;
    };

    this.CheckFileSize = function(file) {
        var size = file.fileSize || file.size;
        if (size == 0 || size > FileHtml5Uploader.config.FileSizeLimit) {
            if (typeof FileHtml5Uploader.config.FileSizeLimitExceedCallback == "function") {
                FileHtml5Uploader.config.FileSizeLimitExceedCallback();
            }
            return false;
        }
        return true;
    };

    this.CheckFileType = function(file) {
        var fileName = file.name || file.fileName;
        var len = fileName.split('.').length;

        if (FileHtml5Uploader.config.file_types != null && FileHtml5Uploader.config.file_types != "" &&
		    fileName.split('.')[len - 1].toLowerCase().match("(" + FileHtml5Uploader.config.file_types.replace(/; \*./g, ")|(").replace('*.', '').replace(';', '') + ")") == null) {
            return false;
        }
        return true;
    };

    this.UploadMultipleFiles = function() {
        for (var i = 0; i < FileHtml5Uploader.FilesToUpload.length; i++) {
            FileHtml5Uploader.UploadFile(FileHtml5Uploader.FilesToUpload[i]);
        }
    };

    this.GenerateSubmitUrl = function() {
        var submitUrl = 'UploadProgress.ashx?submit=' + FileHtml5Uploader.config.FileUploadHandler;
        var data = FileHtml5Uploader.config.Data;
        for (var prop in data) {
            submitUrl += '&{0}={1}'.format(prop, data[prop]);
        }
        return submitUrl;
    };

    this.UploadFile = function(file) {

        if (FileHtml5Uploader.file_stek.length == 0 && FileHtml5Uploader.check_ready == true) {
            FileHtml5Uploader.check_ready = false;
            FileHtml5Uploader._UploadFile(file);
        }
        else {
            FileHtml5Uploader.file_stek.push(file);
        }

    };

    this._UploadFile = function(file) {
        var xhr = new XMLHttpRequest();

        xhr.addEventListener("progress", function(e) { FileHtml5Uploader.UpdateProgress(e, file, xhr); } .bind(this), false);
        xhr.onprogress = function(e) { FileHtml5Uploader.UpdateProgress(e, file, xhr) } .bind(this);
        xhr.upload.addEventListener("progress", function(e) { FileHtml5Uploader.UpdateProgress(e, file, xhr); } .bind(this), false);

        xhr.onload = function(e) { FileHtml5Uploader.TransferComplete(e, file, this); };

        xhr.open("POST", "{0}&fileName={1}&type=html5&fileContentType={2}".format(FileHtml5Uploader.SubmitUrl, encodeURIComponent(file.name || file.fileName), encodeURIComponent(file.type)));

        try {
            xhr.send(file);
        } catch (exc) {
            FileHtml5Uploader.TransferComplete(null, file, {});
        }

        if (FileHtml5Uploader.config.OnBegin) {
            var f = file;
            f.CurrentFile = file.name || file.fileName;
            FileHtml5Uploader.config.OnBegin(f);
        }
    };

    // progress on transfers from the server to the client (downloads)
    this.UpdateProgress = function(evt, file, xhr) {
        var fileHash = (file.name || file.fileName).hashCode();
        if (evt.lengthComputable) {
            var percentComplete = evt.loaded / evt.total;
            jq('#fu_item_delete_{0}'.format(fileHash)).hide();
            jq('#fu_item_loading_{0}'.format(fileHash)).show();

            jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(fileHash)).width((percentComplete * 100).toFixed() + '%').css("background-color", "#C2DFED");
        }

        if (FileHtml5Uploader.config.OverAllProcessHolder != null) {
            jq(FileHtml5Uploader.config.OverAllProcessHolder).show();
        }

        jq('#fu_item_delete_' + fileHash).unbind('click').click(function() {
            xhr.abort();
        });
    };

    this.TransferComplete = function(evt, file, xhr) {

        FileHtml5Uploader.check_ready = true;

        var serverData = eval('({0})'.format(xhr.responseText));
        FileHtml5Uploader.config.OnUploadComplete(serverData);
        //if (serverData.Success)
        FileHtml5Uploader.UploadedFilesCount += 1;

        if (FileHtml5Uploader.config.FilesHeaderCountHolder != null) {
            jq(FileHtml5Uploader.config.FilesHeaderCountHolder).text(FileHtml5Uploader.config.FilesHeaderText.format(FileHtml5Uploader.UploadedFilesCount, FileHtml5Uploader.GetUploadFileCount()));
        }

        if (FileHtml5Uploader.config.OverAllProcessHolder != null) {
            var percent = Math.round(FileHtml5Uploader.UploadedFilesCount * 100 / FileHtml5Uploader.GetUploadFileCount());
            jq("div." + FileHtml5Uploader.config.OverAllProcessBarCssClass + " div").css("width", percent + "%");
            jq("td.fu_percent").text(percent + "%");
            jq(FileHtml5Uploader.config.OverAllProcessHolder).show();
        }

        //in autosubmit mode UploadedFilesCount could be more than GetUploadFileCount.
        if (FileHtml5Uploader.UploadedFilesCount >= FileHtml5Uploader.GetUploadFileCount()) {
            FileHtml5Uploader.config.OnAllUploadComplete();
        }

        var fileHash = (file.name || file.fileName).hashCode();

        jq('#fu_item_delete_' + fileHash).unbind('click').click(function() {
            if (FileHtml5Uploader.config.AutoSubmit) {
                FileHtml5Uploader.config.AfterUploadDeleteCallback(serverData);
            }
        });

        jq('#fu_item_delete_{0}'.format(fileHash)).hide();
        jq('#fu_item_loading_{0}'.format(fileHash)).hide();
        if (serverData.Success) {
            jq('#fu_item_complete_{0}'.format(fileHash)).show();
            jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(fileHash)).width('100%').css("background-color", "#EDF6FD");
        }
        else
            jq('#fu_item_{0} div.studioFileUploaderProgressBar'.format(fileHash)).width('100%').css("background-color", "#FFE4C4");

        jq("#" + FileHtml5Uploader.config.TargetContainerID).scrollTo("#fu_item_" + fileHash);

        if (FileHtml5Uploader.file_stek.length != 0 && FileHtml5Uploader.check_ready == true) {
            setTimeout("FileHtml5Uploader.UploadFileWithDelay()", 100);
        }
    };

    this.UploadFileWithDelay = function() {
        var file = FileHtml5Uploader.file_stek.shift();
        FileHtml5Uploader.check_ready = false;
        FileHtml5Uploader._UploadFile(file);
    };

    //extinguish lights
    this.OnDragLeave = function() {
        return FileHtml5Uploader.HideDragHighlight();
    };
    //highlights
    this.OnDragOver = function(e) {
        if (FileHtml5Uploader.file_stek.length != 0 || FileHtml5Uploader.check_ready != true)
            return;

        var dt = e.originalEvent.dataTransfer;
        if (!dt) return;

        //check that the drop of files
        //FF
        if (dt.types.contains && !dt.types.contains("Files")) return;
        //Chrome
        if (dt.types.indexOf && dt.types.indexOf("Files") == -1) return;

        if (jq.browser.webkit) dt.dropEffect = 'copy';

        FileHtml5Uploader.ShowDragHighlight();

        return false;
    };
    //casting process
    this.OnFilesDrop = function(e) {
        if (FileHtml5Uploader.file_stek.length != 0 || FileHtml5Uploader.check_ready != true)
            return;

        var dt = e.originalEvent.dataTransfer;
        if (!dt && !dt.files) return;

        FileHtml5Uploader.HideDragHighlight();

        var files = dt.files;
        FileHtml5Uploader.OnMultipleInputChange(files);
        return false;
    };

    this.HideDragHighlight = function() {
        if (FileHtml5Uploader.config.DragDropHolder != null)
            jq(FileHtml5Uploader.config.DragDropHolder).removeClass("file-html5-uploader-dragdrop-panel");
    };

    this.ShowDragHighlight = function() {
        if (FileHtml5Uploader.config.DragDropHolder != null)
            jq(FileHtml5Uploader.config.DragDropHolder).addClass("file-html5-uploader-dragdrop-panel");
    };
};