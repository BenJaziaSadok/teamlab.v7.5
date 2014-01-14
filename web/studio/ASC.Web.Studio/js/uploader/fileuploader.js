if (typeof(ASC) == 'undefined') {
    ASC = {};
}
if (typeof(ASC.Controls) == 'undefined') {
    ASC.Controls = {};
}
if (typeof(ASC.Controls.FileUploaderGlobalConfig) == 'undefined') {
    ASC.Controls.FileUploaderGlobalConfig = {};
}

ASC.Controls.FlashDetector = new function () {
    // Flash Player Version Detection - Rev 1.6
    // Detect Client Browser type
    // Copyright(c) 2005-2006 Adobe Macromedia Software, LLC. All rights reserved.
    var isIE = (navigator.appVersion.indexOf("MSIE") != -1) ? true : false;
    var isWin = (navigator.appVersion.toLowerCase().indexOf("win") != -1) ? true : false;
    var isOpera = (navigator.userAgent.indexOf("Opera") != -1) ? true : false;

    this.ControlVersion = function () {
        var version;
        var axo;

        // NOTE : new ActiveXObject(strFoo) throws an exception if strFoo isn't in the registry

        try {
            // version will be set for 7.X or greater players
            axo = new ActiveXObject("ShockwaveFlash.ShockwaveFlash.7");
            version = axo.GetVariable("$version");
        } catch (e) {
        }

        if (!version) {
            try {
                // version will be set for 6.X players only
                axo = new ActiveXObject("ShockwaveFlash.ShockwaveFlash.6");

                // installed player is some revision of 6.0
                // GetVariable("$version") crashes for versions 6.0.22 through 6.0.29,
                // so we have to be careful. 

                // default to the first public version
                version = "WIN 6,0,21,0";

                // throws if AllowScripAccess does not exist (introduced in 6.0r47)		
                axo.AllowScriptAccess = "always";

                // safe to call for 6.0r47 or greater
                version = axo.GetVariable("$version");

            } catch (e) {
            }
        }

        if (!version) {
            try {
                // version will be set for 4.X or 5.X player
                axo = new ActiveXObject("ShockwaveFlash.ShockwaveFlash.3");
                version = axo.GetVariable("$version");
            } catch (e) {
            }
        }

        if (!version) {
            try {
                // version will be set for 3.X player
                axo = new ActiveXObject("ShockwaveFlash.ShockwaveFlash.3");
                version = "WIN 3,0,18,0";
            } catch (e) {
            }
        }

        if (!version) {
            try {
                // version will be set for 2.X player
                axo = new ActiveXObject("ShockwaveFlash.ShockwaveFlash");
                version = "WIN 2,0,0,11";
            } catch (e) {
                version = -1;
            }
        }

        return version;
    };

    // JavaScript helper required to detect Flash Player PlugIn version information
    this.GetSwfVer = function () {
        // NS/Opera version >= 3 check for Flash plugin in plugin array
        var flashVer = -1;

        if (navigator.plugins != null && navigator.plugins.length > 0) {
            if (navigator.plugins["Shockwave Flash 2.0"] || navigator.plugins["Shockwave Flash"]) {
                var swVer2 = navigator.plugins["Shockwave Flash 2.0"] ? " 2.0" : "";
                var flashDescription = navigator.plugins["Shockwave Flash" + swVer2].description;
                var descArray = flashDescription.split(" ");
                var tempArrayMajor = descArray[2].split(".");
                var versionMajor = tempArrayMajor[0];
                var versionMinor = tempArrayMajor[1];
                var versionRevision = descArray[3];
                if (versionRevision == "") {
                    versionRevision = descArray[4];
                }
                if (versionRevision[0] == "d") {
                    versionRevision = versionRevision.substring(1);
                } else if (versionRevision[0] == "r") {
                    versionRevision = versionRevision.substring(1);
                    if (versionRevision.indexOf("d") > 0) {
                        versionRevision = versionRevision.substring(0, versionRevision.indexOf("d"));
                    }
                }
                flashVer = versionMajor + "." + versionMinor + "." + versionRevision;
            }
        }
            // MSN/WebTV 2.6 supports Flash 4
        else if (navigator.userAgent.toLowerCase().indexOf("webtv/2.6") != -1)
            flashVer = 4;
            // WebTV 2.5 supports Flash 3
        else if (navigator.userAgent.toLowerCase().indexOf("webtv/2.5") != -1)
            flashVer = 3;
            // older WebTV supports Flash 2
        else if (navigator.userAgent.toLowerCase().indexOf("webtv") != -1)
            flashVer = 2;
        else if (isIE && isWin && !isOpera) {
            flashVer = this.ControlVersion();
        }
        return flashVer;
    };

    // When called with reqMajorVer, reqMinorVer, reqRevision returns true if that version or greater is available
    this.DetectFlashVer = function (reqMajorVer, reqMinorVer, reqRevision) {
        versionStr = this.GetSwfVer();
        if (versionStr == -1) {
            return false;
        } else if (versionStr != 0) {
            if (isIE && isWin && !isOpera) {
                // Given "WIN 2,0,0,11"
                tempArray = versionStr.split(" "); // ["WIN", "2,0,0,11"]
                tempString = tempArray[1]; // "2,0,0,11"
                versionArray = tempString.split(","); // ['2', '0', '0', '11']
            } else {
                versionArray = versionStr.split(".");
            }
            var versionMajor = versionArray[0];
            var versionMinor = versionArray[1];
            var versionRevision = versionArray[2];

            // is the major.revision >= requested major.revision AND the minor version >= requested minor
            if (versionMajor > parseFloat(reqMajorVer)) {
                return true;
            } else if (versionMajor == parseFloat(reqMajorVer)) {
                if (versionMinor > parseFloat(reqMinorVer))
                    return true;
                else if (versionMinor == parseFloat(reqMinorVer)) {
                    if (versionRevision >= parseFloat(reqRevision))
                        return true;
                }
            }
            return false;
        }
    };
};

ASC.Controls.FileUploaderSWFLocation = '';

ASC.Controls.FileUploader = function (config) {

    var _onBegin = function (data) {
        _renderUploadItem(data);

        if (config.OnBegin)
            config.OnBegin(data);
    };

    var _onEnd = function (data) {
        _renderUploadItem(data);

        if (config.OnEnd)
            config.OnEnd(data);
    };

    var _onProgress = function (data) {
        _renderUploadItem(data);

        if (config.OnProgress)
            config.OnProgress(data);
    };

    var _onAllUploadComplete = function () {

        if (config.OnAllUploadComplete)
            config.OnAllUploadComplete();
    };

    var _onUploadComplete = function (data, serverData) {
        var servData;
        try {
            servData = eval('(' + serverData + ')');
        } catch (err) {
            servData = { Success: false, Message: "Error" };
        }
        data.ServerData = servData;

        FileUploader.UploadedFilesCount++;

        if (config.FilesHeaderCountHolder != null) {
            jq(config.FilesHeaderCountHolder).text(config.FilesHeaderText.format(FileUploader.UploadedFilesCount, FileUploader.GetTotalFileCount()));
        }

        if (config.OverAllProcessHolder != null) {
            var percent = Math.round(FileUploader.UploadedFilesCount * 100 / FileUploader.GetTotalFileCount());
            jq("div." + config.OverAllProcessBarCssClass + " div").css("width", percent + "%");
            jq("td.fu_percent").text(percent + "%");
            jq(config.OverAllProcessHolder).show();
        }

        if (servData.FileURL)
            data.FileURL = servData.FileURL;

        if (servData.FileName && servData.FileName != '')
            data.CurrentFile = servData.FileName;

        var id = new String(data.UploadId).replace('-', '_');
        if (jq("#fu_item_filename" + id).length == 0) {
            jq("#fu_item_" + id).append("<input type='hidden' id='fu_item_filename" + id + "' value='" + data.CurrentFile + "'>");
        }

        if (data.Swf)
            _renderUploadItem(data);

        if (config.OnUploadComplete)
            config.OnUploadComplete(servData);
    };

    this.RequestUrl = config.ProgressUrl || "UploadProgress.ashx?";
    this.RequestTimeSpan = config.ProgressTimeSpan || 500;
    this.AutoSubmit = config.AutoSubmit;
    this.ButtonID = config.UploadButtonID;
    this.file_types = config.file_types;
    this.inputTitle = config.title || config.SelectFilesText || "";

    this.UploadedFilesCount = 0;
    this.FilesTotalCount = 0;

    var MakeUploadItem = function (fileObj) {
        return {
            UploadId: fileObj.id,
            CurrentFile: fileObj.name,
            TotalBytes: fileObj.size,
            Progress: 0,
            Swf: true
        };
    };

    var uploader = this;

    var _uploadStart = function (fileObj) {
        this.UploadedFilesCount = 0;

        if (config.OverAllProcessHolder != null) {
            jq(config.OverAllProcessHolder).show();
        }

        var data = MakeUploadItem(fileObj);
        data.HasDelete = false;
        _onBegin(data);
    };

    var _uploadProgress = function (fileObj, bytesLoaded, totalBytes) {
        var data = MakeUploadItem(fileObj);
        data.HasDelete = false;
        data.Progress = bytesLoaded / totalBytes;

        _onProgress(data);
    };

    var _uploadSuccess = function (fileObj, serverData, receivedResponse) {
        var data = MakeUploadItem(fileObj);
        data.HasDelete = false;
        data.Progress = 1;

        _onEnd(data);
        _onUploadComplete(data, serverData);

    };

    var _uploadComplete = function (fileObj) {
        if (this.getStats().files_queued != 0)
            this.startUpload();
        else
            _onAllUploadComplete();
    };

    var _renderUploadItem = function (data) {
        if (data.UploadId == null)
            return;

        var mb = 1024 * 1024;
        var kb = 1024;

        var id = new String(data.UploadId).replace('-', '_');

        var sb = '<div id="fu_item_' + id + '" style="margin:0px 0px;">' +
            '<table cellspacing="0" cellpadding="0" style="width:100%;border-bottom:1px solid #D1D1D1;"><tr style="height:40px;" valign="middle">';

        //file name
        var curFileName = (jq("#fu_item_filename" + id).length == 0 ? data.CurrentFile : jq("#fu_item_filename" + id).val());
        sb += '<td style="width:70%; padding-left:10px;"><div class="' + (config.FileNameCSSClass || ASC.Controls.FileUploaderGlobalConfig.FileNameCSSClass) + '" style="padding-right:5px;" title="' + curFileName + '">';
        sb += curFileName;

        var barClass = '';

        var len = data.CurrentFile.split('.').length;
        if (config.file_types != null && config.file_types != "" &&
            data.CurrentFile.split('.')[len - 1].toLowerCase().match("(" + config.file_types.replace(/; \*./g, ")|(").replace('*.', '').replace(';', '') + ")") == null) {
            data.ReturnCode = 2;
        }

        if (data.ReturnCode && data.ReturnCode != 0) {
            barClass = (config.ErrorBarCSSClass || ASC.Controls.FileUploaderGlobalConfig.ErrorBarCSSClass);
        } else {
            barClass = (config.ProgressBarCSSClass || ASC.Controls.FileUploaderGlobalConfig.ProgressBarCSSClass);
        }

        if (data.ReturnCode && data.ReturnCode != 0) {
            data.Progress = 1;
            sb += '<div class="' + (config.ErrorTextCSSClass || ASC.Controls.FileUploaderGlobalConfig.ErrorTextCSSClass) + '">';
        } else {
            sb += '<div ' + ((config.DescriptionCSSClass || ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass) ? 'class="' + (config.DescriptionCSSClass || ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass) + '"' : 'style="padding:3px; font-size:10px; color:gray;"') + '>';
        }

        if (data.ReturnCode && data.ReturnCode == 1) {
            sb += (config.ErrorFileSizeLimitText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileSizeLimitText);
            if (typeof config.FileSizeLimitExceedCallback == "function") {
                config.FileSizeLimitExceedCallback();
            }
        } else if (data.ReturnCode && data.ReturnCode == 2) {
            sb += config.ErrorFileTypeText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileTypeText;
        }

        sb += '</div>';
        sb += '</td>';

        //size
        sb += '<td style="padding-left: 10px; width:20%; color: #83888D;">';
        if (data.TotalBytes && data.TotalBytes > 0) {
            if (data.TotalBytes <= mb)
                //TODO: KB to resource
                sb += (data.TotalBytes / kb).toFixed(2) + ' KB';
            else
                //TODO: MB to resource
                sb += (data.TotalBytes / mb).toFixed(2) + ' MB';
        }

        sb += '&nbsp;</td>';

        sb += '<td style="width:10%;">&nbsp;';
        if (data.Progress == 1) {
            sb += '<a id="fu_item_complete_{0}" class="{1}"></a>'.format(id, config.CompleteCSSClass || ASC.Controls.FileUploaderGlobalConfig.CompleteCSSClass || '');
        } else if (data.HasDelete || (data.ServerData && config.AfterUploadDeleteCallback) || (data.UploadedItem && data.UploadedItem.RemoveHandler)) {
            sb += '<a id="fu_item_delete_{0}" class="{1}" >{2}</a>'.format(id, config.DeleteLinkCSSClass || ASC.Controls.FileUploaderGlobalConfig.DeleteLinkCSSClass || '', '');
        } else if (data.Progress > 0) {
            sb += '<a id="fu_item_loading_{0}" class="{1}"></a>'.format(id, config.LoadingImageCSSClass || ASC.Controls.FileUploaderGlobalConfig.LoadingImageCSSClass || '');
        }
        sb += '</td>';

        sb += '</tr></table><div id="fu_item_progress_{3}" class="{0}" style="margin-top:-41px; float:left; width:{1};height:40px;{2}" ></div></div>'.format(barClass, (data.Progress * 100).toFixed() + '%', (data.Progress == 1 ? "background-color:#EDF6FD;" : (data.Progress > 0 ? "background-color:#C2DFED;" : "")), id);

        if (jq('#fu_item_' + id).length != 0)
            jq('#fu_item_' + id).replaceWith(sb);
        else {
            if (data.UploadedItem && data.UploadedItem.TargetContainerID != null)
                jq('#' + data.UploadedItem.TargetContainerID).append(sb);
            else
                jq('#' + config.TargetContainerID).append(sb);
        }

        if (data.UploadedItem && data.UploadedItem.TargetContainerID != null)
            jq('#' + data.UploadedItem.TargetContainerID).scrollTo("#fu_item_" + id);
        else
            jq('#' + config.TargetContainerID).scrollTo("#fu_item_" + id);

        if (jq.browser.msie && jq.browser.version < 9) {
            if (data.Progress == 1) {
                jq("#fu_item_progress_" + id).css("background-color", "#BBDCF9");
            } else if (data.Progress > 0) {
                jq("#fu_item_progress_" + id).css("background-color", "#7AC7E8");
            }
            jq("#fu_item_progress_" + id).fadeTo(0, 0.2);
        }

        if (data.UploadedItem && data.UploadedItem.RemoveHandler) {
            jq('#fu_item_delete_' + id).bind("click", function () {
                if (data.UploadedItem.RemoveHandler(data.UploadedItem))
                    jq('#fu_item_' + id).remove();

                return false;
            });
        } else if (data.HasDelete || (data.ServerData && config.AfterUploadDeleteCallback))
            jq('#fu_item_delete_' + id).unbind("click").bind("click", function () {

                if (data.ServerData && config.AfterUploadDeleteCallback) {
                    config.AfterUploadDeleteCallback(data.ServerData);
                } else {
                    if (data.Swf)
                        swfuploader.cancelUpload(data.UploadId);
                    else
                        uploader.RemoveAjaxUploadHandler(data.UploadId);

                    FileUploader.HandlerFileSelected(false);
                }

                jq('#fu_item_' + id).remove();
                return false;
            });
    };

    if (config.UploadedFiles != null && config.UploadedFiles.length > 0) {
        for (var j = 0; j < config.UploadedFiles.length; j++) {
            var fitem = config.UploadedFiles[j];
            _renderUploadItem({
                UploadId: 'asc_pfu_item_' + j,
                CurrentFile: fitem.Name,
                TotalBytes: fitem.Size,
                Progress: 1,
                FileURL: fitem.URL,
                UploadedItem: fitem
            });
        }
    }

    if (config.FilesHeaderCountHolder != null)
        jq(config.FilesHeaderCountHolder).text(config.SelectFileText);

    if (config.OverAllProcessHolder != null) {
        jq(config.OverAllProcessHolder).html("<table cellpadding='5' cellspacing='0' width='100%' style='padding:10px 0;' class='describe-text'><tr><td width='100'>" + config.OverallProgressText + ": </td><td>" + '<div style="margin-top: 1px;" class="{0}"><div></div></div>'.format(config.OverAllProcessBarCssClass) + "</td><td class='fu_percent' width='20'></td></tr></table>");
        jq("div." + config.OverAllProcessBarCssClass + " div").css("width", "0%");
        jq("td.fu_percent").text("0%");
        jq(config.OverAllProcessHolder).hide();
    }

    this.UploadItems = new Array();
    var swfuploader = null;

    this.GetSWFObj = function () {
        return swfuploader;
    };

    var submitUrl = 'UploadProgress.ashx?submit=' + config.FileUploadHandler;
    for (var prop in config.Data)
        submitUrl += '&' + prop + '=' + config.Data[prop];

    var disableFlash = false;
    if (config.DisableFlash != null && config.DisableFlash != undefined)
        disableFlash = config.DisableFlash;
    else if (ASC.Controls.FileUploaderGlobalConfig.DisableFlash != null && ASC.Controls.FileUploaderGlobalConfig.DisableFlash != undefined)
        disableFlash = ASC.Controls.FileUploaderGlobalConfig.DisableFlash;


    var isFlashInstalled = ASC.Controls.FlashDetector.DetectFlashVer(8, 0, 0);
    if (!isFlashInstalled) {
        jq('#studio_fuplswitcherBox').hide();
    }

    jq("#" + this.ButtonID).css('visibility', '');

    this.IsSWF = isFlashInstalled && (disableFlash != true);
    if (this.IsSWF) {
        var uploadButton = jq('#' + uploader.ButtonID);

        if (config.SelectFilesText != null && config.SelectFilesText != "")
            uploadButton.attr("title", config.SelectFilesText).text(config.SelectFilesText);

        jq("#ProgressFileUploader").insertAfter(jq('#' + uploader.ButtonID));

        jq('#asc_fileuploaderSWFContainer').css({
            'width': uploadButton.outerWidth() || uploadButton.width() + 'px',
            'height': uploadButton.outerHeight() || uploadButton.height() + 'px',
            'position': 'absolute',
            'display': 'block'
        });

        var _reRouteSWF = function () {
            var pos = uploadButton.offset();

            jq('#asc_fileuploaderSWFContainer').css({
                'left': pos.left + "px",
                'top': pos.top + 'px',
                'width': uploadButton.outerWidth() || uploadButton.width() + 'px',
                'height': uploadButton.outerHeight() || uploadButton.height() + 'px',
                'position': 'absolute',
                'display': 'block'
            });
        };

        uploadButton.mouseover(function () {
            //_reRouteSWF();
        });

        jq('body').append('<style type="text/css"> .swfupload{ z-index:99999; position: absolute;}</style>');

        swfuploader = new SWFUpload({
            // Backend Settings
            upload_url: submitUrl,
            post_params: null,
            file_types: (this.file_types != "" && this.file_types != null ? this.file_types : "*.*"),

            file_size_limit: 1024 * ((config.FileSizeLimit != null && config.FileSizeLimit != 0) ? config.FileSizeLimit : "0"), //flash check Mb
            file_queued_handler: function (fileObj) {
                if (config.SingleUploader) {
                    if (config.FileNameFilter != null && config.FileNameFilter != ""
                        && config.FileNameFilter.toLowerCase() != fileObj.name.replace(new RegExp("[\\\\/:*?\"<>|';\+]", 'gim'), '_').toLowerCase()) {
                        if (config.FileNameExeptionText != null && config.FileNameExeptionText != "")
                            alert(config.FileNameExeptionText);

                        return;
                    }

                    jq('#' + config.TargetContainerID).html('');
                    FileUploader.FilesTotalCount = 1;
                    try {
                        while (this.getStats().files_queued > 1) {
                            this.cancelUpload();
                        }
                    } catch (e) {
                    }
                    ;
                }
                FileUploader.HandlerFileSelected(true);
                var data = MakeUploadItem(fileObj);
                data.HasDelete = true;

                _renderUploadItem(data);
                if (config.shouldReRoute != null && config.shouldReRoute == true)
                    _reRouteSWF();
            },

            file_queue_error_handler: function (file, errorCode, message) {
                try {
                    var errorName = "";
                    switch (errorCode) {
                        case SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED:
                            errorName = "QUEUE LIMIT EXCEEDED";
                            break;
                        case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
                            errorName = (config.ErrorFileSizeLimitText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileSizeLimitText || "FILE SIZE LIMIT");
                            break;
                        case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
                            errorName = (config.ErrorFileEmptyText || ASC.Controls.FileUploaderGlobalConfig.ErrorFileEmptyText || "ZERO BYTE FILE");
                            break;
                        case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
                            errorName = "INVALID FILE TYPE";
                            break;
                        default:
                            errorName = "UNKNOWN";
                            break;
                    }

                    alert(errorName);

                } catch (ex) {
                }
            },

            upload_start_handler: _uploadStart,
            upload_progress_handler: _uploadProgress,
            upload_success_handler: _uploadSuccess,
            upload_complete_handler: _uploadComplete,
            file_dialog_complete_handler: this.AutoSubmit ? function () {
                this.startUpload();
            } : null,
            // Button settings
            button_placeholder_id: "asc_fileuploaderSWFObj",
            button_width: uploadButton.outerWidth() || uploadButton.width() || 130,
            button_height: uploadButton.outerHeight() || uploadButton.height() || 20,

            button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
            button_action: this.SingleUploader ? SWFUpload.BUTTON_ACTION.SELECT_FILE : SWFUpload.BUTTON_ACTION.SELECT_FILES,
            button_cursor: -2,
            button_image_url: '',

            // Flash Settings
            flash_url: ASC.Controls.FileUploaderSWFLocation,

            // Debug Settings
            debug: false
        });

        if (config.shouldReRoute == null || config.shouldReRoute == true)
            _reRouteSWF();
    } else {
        this.CurrentUtx = 0;
        this.ButtonInc = 0;

        this.SetFormData = function (scope) {
            var cbDate = new Date();
            var utx = Date.UTC(cbDate.getYear(), cbDate.getMonth(), cbDate.getDay(), cbDate.getHours(), cbDate.getMinutes(), cbDate.getSeconds(), cbDate.getMilliseconds());

            utx = Math.floor(Math.random() * 100000) + utx;
            utx += Math.floor(Math.random() * 100000);

            var prevButton = jq('#' + scope.ButtonID);
            var newButton = prevButton.clone();

            scope.ButtonID = 'uplb_' + (scope.ButtonInc++);
            newButton.attr('id', scope.ButtonID);
            prevButton.replaceWith(newButton);
            newButton.addClass("studioFileUploaderSelect");

            if (config.SelectFilesText != null && config.SelectFilesText != "")
                jq(newButton).attr("title", config.SelectFilesText).text(config.SelectFilesText);

            jq('.fileuploadinput').remove();

            var au = new AjaxUpload(scope.ButtonID, {
                title: this.inputTitle,
                parentDialog: jq("#" + scope.ButtonID).parents('.blockUI').length > 0 ? jq("#" + scope.ButtonID).parents('.blockUI')[0] : undefined,
                action: submitUrl,
                autoSubmit: scope.AutoSubmit,
                onChange: function (file, extension) {

                    FileUploader.HandlerFileSelected(true);

                    if (config.SingleUploader) {

                        if (config.FileNameFilter != null && config.FileNameFilter != ""
                            && config.FileNameFilter.toLowerCase() != file.replace(new RegExp("[\\\\/:*?\"<>|';\+]", 'gim'), '_').toLowerCase()) {
                            if (config.FileNameExeptionText != null && config.FileNameExeptionText != "")
                                alert(config.FileNameExeptionText);

                            return false;
                        }

                        jq('#' + config.TargetContainerID).html('');

                        FileUploader.FilesTotalCount = 1;

                        if (scope.UploadItems.length > 0)
                            scope.UploadItems.splice(0, scope.UploadItems.length);
                    }

                    scope.UploadItems.push(this);

                    _renderUploadItem({
                        UploadId: this.Utx,
                        CurrentFile: file,
                        TotalBytes: 0,
                        Progress: 0,
                        HasDelete: true,
                        Swf: false
                    });

                    scope.SetFormData(scope);
                },
                onSubmit: function (file, extension) {

                    scope.CurrentUtx = this.Utx;

                    _onBegin({
                        UploadId: this.Utx,
                        CurrentFile: file,
                        TotalBytes: 0,
                        Progress: 0,
                        HasDelete: true,
                        Swf: false
                    });

                    scope.OnMonitor(scope);
                },
                onComplete: function (file, response) {

                    _onUploadComplete({
                        UploadId: this.Utx,
                        CurrentFile: file,
                        TotalBytes: 0,
                        Progress: 0,
                        HasDelete: true,
                        Swf: false
                    }, response);

                    if (scope.UploadItems.length == 0 || (scope.UploadItems.length == 1 && scope.UploadItems[0].Utx == this.Utx))
                        _onAllUploadComplete();
                }
            });

            au.Utx = utx;
            au.setData({ __UixdId: utx });
        };

        this.OnMonitor = function (scope) {
            var callback = function (data) {
                var o = arguments.callee.target;
                if (data.IsFinished == true) {
                    _onEnd(data);

                    o.RemoveAjaxUploadHandler(data.UploadId);

                    o.Submit();
                } else {
                    _onProgress(data);
                    setTimeout(function () {
                        o.OnMonitor(o);
                    }, o.RequestTimeSpan);
                }
            };

            callback.target = scope;

            var cbDate = new Date();
            var utx = Date.UTC(cbDate.getYear(), cbDate.getMonth(), cbDate.getDay(), cbDate.getHours(), cbDate.getMinutes(), cbDate.getSeconds(), cbDate.getMilliseconds());

            var url = scope.RequestUrl + '__UixdId=' + scope.CurrentUtx;
            url += "&boost=" + utx;

            if (config.FileSizeLimit != null && config.FileSizeLimit != 0)
                url += "&limit=" + config.FileSizeLimit;

            if (config.file_types != null && config.file_types != "")
                url += "&file_types=" + config.file_types;

            jq.getJSON(url, callback);
        };

        this.SetFormData(this);
    }

    this.RemoveAjaxUploadHandler = function (uploadId) {
        for (var i = 0; i < this.UploadItems.length; i++) {
            var item = this.UploadItems[i];
            if (item.Utx == uploadId) {
                this.UploadItems.splice(i, 1);
                break;
            }
        }
        ;
    };

    this.Submit = function () {

        jq("a[id*='fu_item_delete_']").hide();
        if (!this.AutoSubmit) {
            jq("#" + this.ButtonID).css('visibility', 'hidden');
        }

        if (FileHtml5Uploader.EnableHtml5) {
            FileHtml5Uploader.UploadMultipleFiles();
            return false;
        }

        if (this.IsSWF) {
            jq("#SWFUpload_" + (SWFUpload.movieCount - 1)).css('top', -jq("#SWFUpload_" + (SWFUpload.movieCount - 1)).offset().top);
            swfuploader.startUpload();
        } else {
            if (this.UploadItems.length > 0)
                this.UploadItems[0].submit();
        }
    };

    this.GetUploadFileCount = function () {
        if (FileHtml5Uploader.EnableHtml5) {
            return FileHtml5Uploader.GetUploadFileCount();
        }
        if (this.IsSWF) {
            try {
                return swfuploader.getStats().files_queued;
            } catch (exs) {
                return 0;
            }
        } else
            return this.UploadItems.length;
    };

    this.GetTotalFileCount = function () {
        return this.FilesTotalCount;
    };

    this.HandlerFileSelected = function (increase) {
        if (increase && !config.SingleUploader)
            this.FilesTotalCount++;
        else if (!increase)
            this.FilesTotalCount--;

        if (config.AddFilesText != "" && config.AddFilesText != null) {
            if (this.IsSWF)
                jq("#" + config.UploadButtonID).text(config.AddFilesText);
            else
                jq("a[id^='uplb_']").text(config.AddFilesText);
        }
        if (config.SubmitPanelAfterSelectHolder != null)
            jq(config.SubmitPanelAfterSelectHolder).show();

        if (config.FilesHeaderCountHolder != null && this.GetTotalFileCount() > 0)
            jq(config.FilesHeaderCountHolder).text(config.SelectedFilesText.format(this.GetTotalFileCount()));
        else if (config.FilesHeaderCountHolder != null && this.GetTotalFileCount() == 0 && config.SelectFilesText != null) {
            jq(config.FilesHeaderCountHolder).attr("title", config.SelectFileText).text(config.SelectFileText);
            //jq(config.SubmitPanelAfterSelectHolder).hide();
        }

        if (config.TargetContainerID != "" && config.TargetContainerID != null)
            jq(jq('#' + config.TargetContainerID + ' table')[0]).css("border-top", "1px solid #d1d1d1");

    };
};