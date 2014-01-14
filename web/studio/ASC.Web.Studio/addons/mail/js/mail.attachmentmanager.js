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

window.AttachmentManager = (function($) {
    var attachments = [];
    var documents_before_save = [];
    var documents_in_load = [];
    var config = null;
    var next_order_number = 0;
    var max_one_attachment_size = 15;
    var max_one_attachment_bytes = max_one_attachment_size * 1024 * 1024;
    var max_all_attachments_size = 25;
    var max_all_attachments_bytes = max_all_attachments_size * 1024 * 1024;
    var uploader = null;
    var is_switcher_exists = false;
    var save_heandler_id = null;
    var add_docs_heandler_id = null;
    var need_attach_documents = false;
    var upload_container_id = 'newMessage';
    var files_container = 'mail_attachments';
    var init_stream_id = '';
    var is_saving = false;
    var max_file_name_len = 63;

    function init(stream_id, loaded_files, init_config) {
        init_stream_id = stream_id;
        next_order_number = 0;
        stopUploader();
        window.DocumentsPopup.unbind(window.DocumentsPopup.events.SelectFiles);
        var data = { stream: stream_id, messageId: mailBox.currentMessageId };

        config = init_config || {
            runtimes: 'html5,flash,html4',
            browse_button: 'attachments_browse_btn',
            container: upload_container_id,
            max_file_size: max_one_attachment_bytes,
            url: generateSubmitUrl(data),
            flash_swf_url: '/js/uploader/plupload.flash.swf',
            drop_element: upload_container_id
        };

        uploader = new window.plupload.Uploader(config);

        $('#' + upload_container_id)
            .bind("dragenter", function() { return false; })//simply process idle event
            .bind('dragleave', function() {
                return hideDragHighlight();
            })//extinguish lights
            .bind("dragover", function() {
                if (uploader.features.dragdrop) {
                    showDragHighlight();
                }
                if ($.browser.safari) {
                    return true;
                }
                return false;
            })//illuminated area for the cast
            .bind("drop", function() {
                hideDragHighlight();
                return false;
            }); //handler is throwing on the field

        uploader.bind('BeforeUpload', onBeforeUpload);
        uploader.bind('FilesAdded', onFilesAdd);
        uploader.bind('UploadFile', onUploadFile);
        uploader.bind('UploadProgress', onUploadProgress);
        uploader.bind('FileUploaded', onFileUploaded);
        uploader.bind('Error', onError);
        uploader.bind('UploadComplete', onUploadComplete);
        uploader.bind('StateChanged', onStateChanged);

        window.DocumentsPopup.bind(window.DocumentsPopup.events.SelectFiles, selectDocuments);
        save_heandler_id = serviceManager.bind(window.Teamlab.events.saveMailMessage, onSaveMessage);
        add_docs_heandler_id = serviceManager.bind(window.Teamlab.events.addMailDocument, onAttachDocument);
        $('#attachments_limit_txt').text(window.MailScriptResource.AttachmentsLimitLabel
            .replace('%1', max_one_attachment_size)
            .replace('%2', max_all_attachments_size));

        addAttachments(loaded_files);

        uploader.init();

        if (uploader.runtime == "flash" ||
            uploader.runtime == "html4" ||
            uploader.runtime == "") {
            is_switcher_exists = true;
            renderSwitcher();
        }

        messagePage.initImageZoom();
    }

    function renderSwitcher() {
        if (uploader.runtime == "flash" || uploader.runtime == "")
            $("#switcher a").text(window.MailScriptResource.ToBasicUploader);
        else
            $("#switcher a").text(window.MailScriptResource.ToFlashUploader);

        if (is_switcher_exists)
            $("#switcher").show();
    }

    function switchMode() {
        var is_flash = uploader.runtime == "flash" || uploader.runtime == "";
        config.runtimes = is_flash ? "html4" : "flash,html4";
        var data = { stream: init_stream_id, messageId: mailBox.currentMessageId };
        config.url = generateSubmitUrl(data);
        if (is_flash) {
            $("#" + upload_container_id + " div.flash").remove();
        }
        else {
            $("#" + upload_container_id + " form").remove();
        }
        $('#' + files_container + ' tbody').empty();


        init(init_stream_id, attachments, config);
    }

    function onBeforeUpload(up, file) {
        var res = getTotalUploadedSize() + (file.size || 0) <= max_all_attachments_bytes;
        if (!res) {
            setTimeout(function() {
                up.trigger('Error', { file: file, message: window.MailScriptResource.AttachmentsTotalLimitError });
            }, 0);
        };
        return res;
    }

    function onFilesAdd(up, files) {
        // Fires while when the user selects files to upload.
        // console.log('Try to add files', files);

        var file, i, len = files.length;

        for (i = 0; i < len; i++) {
            file = files[i];
            file.orderNumber = getOrderNumber();
            var attachment = convertPUfileToAttachment(file);
            addAttachment(attachment);
        }

        if (mailBox.currentMessageId < 1) {
            if (!is_saving) {
                is_saving = true;
                messagePage.saveMessage();
            }
            return;
        }

        uploader.stop();

        for (i = 0; i < len; i++) {
            file = files[i];
            var pos = searchFileIndex(uploader.files, file.fileId);
            if (pos < 0 /*&& !limit_exceeded*/) {
                file.status = window.plupload.QUEUED;
                file.loaded = 0;
                file.percent = 0;
                uploader.files.push(file);
                uploader.trigger('QueueChanged');
            }
        }

        if (tasksExist())
            uploader.start();
    }

    function convertPUfileToAttachment(file) {
        var attachment = {
            contentId: null,
            contentType: '',
            fileId: -1,
            fileName: file.name,
            orderNumber: file.orderNumber,
            size: file.size,
            storedName: '',
            streamId: '',
            error: file.error || ''
        };

        completeAttachment(attachment);

        return attachment;
    }

    function convertDocumentToAttachment(document) {
        var name = document.title;
        var ext = getAttachmentExtension(name);
        var new_ext = '';
        var is_internal_document = false;
        switch (ext) {
            case '.doct':
                new_ext = '.docx';
                break;
            case '.xlst':
                new_ext = '.xlsx';
                break;
            case '.pptt':
                new_ext = '.pptx';
            default:
                break;
        }

        if (new_ext != '') {
            name = getAttachmentName(name) + new_ext;
            is_internal_document = true;
        }

        var attachment = {
            contentId: null,
            contentType: '',
            fileId: -1,
            fileName: name,
            orderNumber: document.orderNumber,
            size: is_internal_document ? 0 : document.size,
            storedName: '',
            streamId: '',
            error: document.error || ''
        };

        completeAttachment(attachment);

        return attachment;
    }

    function onUploadFile(up, file) {
        // Fires when a file is to be uploaded by the runtime.
        // console.log('UploadFile', file);
        displayAttachmentProgress(file.orderNumber, true);
    }

    function onUploadProgress(up, file) {
        // Fires while a file is being uploaded.
        // console.log('Progress ' + file.percent + '% name: ' + file.name + ' data_id: ' + file.fileId);
        setAttachmentProgress(file.orderNumber, file.percent);
        if (file.status == window.plupload.DONE && file.percent == 100) {
            displayAttachmentProgress(file.orderNumber, false);
        }
    }

    function onFileUploaded(up, file, resp) {
        // Fires when a file is successfully uploaded.
        // console.log('FileUploaded', file, resp);
        displayAttachmentProgress(file.orderNumber, false);
        var response = JSON.parse(resp.response);
        if (response) {
            if (!response.Success) {
                response.Data.error = response.Message;
                response.Data.size = 0;
            }
            updateAttachment(file.orderNumber, response.Data);
        }
    }

    function onStateChanged(up) {
        // Fires when the overall state is being changed for the upload queue.
        switch (up.state) {
            case window.plupload.STARTED:
                onPreUploadStart();
                // console.log('StateChanged -> STARTED');
                break;
            case window.plupload.STOPPED:
                // console.log('StateChanged -> STOPPED');
                break;
        }
    }

    function getSizeString(size) {
        var size_string = '';
        if (size != undefined) {
            var mb = 1024 * 1024;
            var kb = 1024;
            if (size <= mb)
                if (size <= kb)
                size_string = size + ' ' + window.MailScriptResource.Bytes;
            else
                size_string = (size / kb).toFixed(2) + ' ' + window.MailScriptResource.Kilobytes;
            else
                size_string = (size / mb).toFixed(2) + ' ' + window.MailScriptResource.Megabytes;
        }
        return size_string;
    }

    function addAttachments(attachments_list) {
        var i, len = attachments_list.length;
        for (i = 0; i < len; i++) {
            var attachment = attachments_list[i];
            attachment.orderNumber = getOrderNumber();
            addAttachment(attachment);
        }
    }

    function addAttachment(attachment) {
        if (next_order_number < attachment.orderNumber)
            next_order_number = attachment.orderNumber;

        var html = prepareFileRow(attachment);

        $('#' + files_container + ' tbody').append(html);

        if (attachments == undefined)
            attachments = [];

        if (uploader.runtime == "html4" &&
            attachment.error == '') {
            displayAttachmentProgress(attachment.orderNumber, true);
            setAttachmentProgress(attachment.orderNumber, 100);
        }

        attachments.push(attachment);
        calculateAttachments();
        messagePage.updateEditAttachmentsActionMenu();
    }

    function updateAttachment(order_number, update_info) {
        if (order_number == undefined || order_number < 0) return;

        var pos = searchFileIndex(attachments, order_number);
        if (pos > -1) {
            var attachment = attachments[pos];
            attachment.contentType = update_info.contentType;
            attachment.fileId = update_info.fileId;
            attachment.fileName = update_info.fileName;
            attachment.size = update_info.size || 0;
            attachment.storedName = update_info.storedName;
            attachment.fileNumber = update_info.fileNumber;
            attachment.streamId = update_info.streamId;
            attachment.error = update_info.error || '';

            completeAttachment(attachment);
            calculateAttachments();

            var html = prepareFileRow(attachment);
            $('#' + files_container + ' .row[data_id=' + order_number + ']').replaceWith(html);
            messagePage.initImageZoom();
            messagePage.updateEditAttachmentsActionMenu();
        } else {
            // Attachemnt has been removed, need remove it from storage
            deleteStoredAttachment(update_info.fileId);
        }
    }

    function prepareFileRow(attachment) {
        if (attachment == undefined) return '';

        var html = $.tmpl("attachmentTmpl", attachment, {
            cutFileName: cutFileName,
            fileSizeToStr: getSizeString,
            getFileNameWithoutExt: getAttachmentName,
            getFileExtension: getAttachmentExtension
        });

        return html;
    }

    function displayAttachmentProgress(order_number, show) {
        var item_progress = $('#item_progress_' + order_number);
        if (item_progress != undefined) {
            if (show) {
                item_progress.show();
            } else {
                item_progress.hide();
            }
        }
    }

    function setAttachmentProgress(order_number, percent) {
        if (percent == undefined || percent < 0) return;

        var item_progress = $('#item_progress_' + order_number + ':visible .progress-slider');
        if (item_progress != undefined && item_progress.length == 1) {
            item_progress.css('width', percent + '%');
        }
    }

    function removeAllAttachments() {
        var temp_collection = attachments.slice(); // clone array
        for (var i = 0; i < temp_collection.length; i++) {
            var attachment = temp_collection[i];
            removeAttachment(attachment.orderNumber);
        }
        calculateAttachments();
    }


    function searchFileIndex(collection, order_number) {
        var pos = -1;
        for (var i = 0; i < collection.length; i++) {
            var file = collection[i];
            if (file.orderNumber == order_number) {
                pos = i;
                break;
            }
        }
        return pos;
    }

    function deleteStoredAttachment(file_id) {
        if (file_id != undefined && file_id > 0) {
            if (mailBox.currentMessageId > 1)
                messagePage.deleteMessageAttachment(file_id);
        }
    }

    function removeFromUploaderQueue(order_number) {
        var pos = searchFileIndex(uploader.files, order_number);
        if (pos > -1) {
            var need_start = false;
            if (uploader.files[pos].status == window.plupload.STARTED) {
                uploader.stop();
                need_start = true;
            }
            uploader.removeFile(uploader.files[pos]);
            if (need_start)
                uploader.start();
        }
    }

    function removeAttachment(order_number) {
        var pos = searchFileIndex(attachments, order_number);
        if (pos > -1) {
            var attachment = attachments[pos];
            deleteStoredAttachment(attachment.fileId);
            attachments.splice(pos, 1);
        }

        pos = searchFileIndex(documents_in_load, order_number);
        if (pos > -1) {
            var document = documents_in_load[pos];
            deleteStoredAttachment(document.fileId);
            documents_in_load.splice(pos, 1);
        }

        $('#' + files_container + ' .row[data_id=' + order_number + ']').remove();
        calculateAttachments();
        removeFromUploaderQueue(order_number);
    }

    function getAttachmentName(full_name) {
        if (full_name) {
	        var last_dot_index = full_name.lastIndexOf('.');
            return last_dot_index > -1 ? full_name.substr(0, last_dot_index) : full_name;
        }
        return '';
    }

    function getAttachmentExtension(full_name) {
        if (full_name) {
            var last_dot_index = full_name.lastIndexOf('.');
            return last_dot_index > -1 ? full_name.substr(last_dot_index) : '';
        }
        return '';
    }
    
    function getAttachmentWarningByExt(ext) {
        switch (ext) {
            case '.exe':
                return window.MailScriptResource.AttachmentsExecutableWarning;
        default:
            return '';
        }
    }

    function selectDocuments(e, documents) {
        if (mailBox.currentMessageId < 1) {
            if (!need_attach_documents) {
                need_attach_documents = true;
                documents_before_save = documents;
                messagePage.saveMessage();
            }
            else
                $.merge(documents_before_save, documents);

            return;
        }

        hideError();

        var total_new_size = getTotalUploadedSize();

        for (var i = 0; i < documents.data.length; i++) {
            var document = documents.data[i];
            document.orderNumber = getOrderNumber();
            var attachment = convertDocumentToAttachment(document);

            addAttachment(attachment);

            if (total_new_size + attachment.size > max_all_attachments_bytes) {
                onAttachDocumentError({ attachment: attachment }, [window.MailScriptResource.AttachmentsTotalLimitError]);
            } else {
                total_new_size += attachment.size;
                documents_in_load.push(attachment);
                displayAttachmentProgress(attachment.orderNumber, true);
                setAttachmentProgress(attachment.orderNumber, 100);

                var data = {
                    fileId: document.id,
                    version: document.version,
                    shareLink: document.downloadUrl,
                    streamId: init_stream_id
                };

                serviceManager.attachDocuments(mailBox.currentMessageId, data, { attachment: attachment }, { error: onAttachDocumentError });
            }
        }
        onPreUploadStart();
        documents_before_save = [];
    }

    function freeLoadedDocument(documnent) {
        var pos = searchFileIndex(documents_in_load, documnent.orderNumber);
        if (pos > -1) {
            displayAttachmentProgress(documnent.orderNumber, false);
            documents_in_load.splice(pos, 1);
        }
    }

    function onAttachDocumentError(params, error) {
        var attachment = params.attachment;
        attachment.error = error[0];
        attachment.size = 0;
        
        updateAttachment(params.attachment.orderNumber, attachment);
        freeLoadedDocument(attachment);
    }

    function onAttachDocument(params, document) {
        if (document) {
            updateAttachment(params.attachment.orderNumber, document);
            freeLoadedDocument(params.attachment);
        }
        else
            onAttachDocumentError(params, window.MailScriptResource.AttachmentsUnknownError);

        completeUploading();
    }

    function onPreUploadStart() {
        hideError();
        messagePage.setDirtyMessage();
        if (is_switcher_exists)
            $("#switcher").hide();
    }

    function tasksExist() {
        for (var i = 0; i < uploader.files.length; i++) {
            var file = uploader.files[i];
            if (file.status == window.plupload.QUEUED) {
                return true;
            }
        }
        return false;
    }

    function completeUploading() {
        if (documents_in_load.length == 0 &&
            uploader.state != window.plupload.STARTED) {
            if (is_switcher_exists)
                $("#switcher").show();
            messagePage.resetDirtyMessage();
            calculateAttachments();
        }
    }

    function onUploadComplete() {
        if (tasksExist()) {
            uploader.start();
            return;
        }
        uploader.splice();
        completeUploading();
        // console.log('[Upload complete]');
    }

    function onSaveMessage() {
        if (mailBox.currentMessageId > 0) {
            messagePage.disableButton($('#editMessagePage .btnSave'), false);
            var data = { stream: init_stream_id, messageId: mailBox.currentMessageId };
            uploader.settings.url = generateSubmitUrl(data);
            is_saving = false;

            if (need_attach_documents) {
                need_attach_documents = false;
                selectDocuments({}, documents_before_save);
                documents_before_save = [];
            }

            if (tasksExist()) {
                uploader.start();
            }

            if (save_heandler_id != null || save_heandler_id != undefined) {
                serviceManager.unbind(save_heandler_id);
            }
        }
    }

    function isLoading() {
        return documents_in_load.length > 0 || uploader != undefined && uploader.state == window.plupload.STARTED;
    }

    function getPlUploaderError(code) {
        switch (code) {
            case window.plupload.GENERIC_ERROR:
                return window.MailAttachmentsResource.PL_GENERIC_ERROR;
            case window.plupload.HTTP_ERROR:
                return window.MailAttachmentsResource.PL_HTTP_ERROR;
            case window.plupload.IO_ERROR:
                return window.MailAttachmentsResource.PL_IO_ERROR;
            case window.plupload.FILE_EXTENSION_ERROR:
                return window.MailAttachmentsResource.PL_FILE_EXTENSION_ERROR;
            case window.plupload.SECURITY_ERROR:
                return window.MailAttachmentsResource.PL_SECURITY_ERROR;
            case window.plupload.INIT_ERROR:
                return window.MailAttachmentsResource.PL_INIT_ERROR;
            case window.plupload.FILE_SIZE_ERROR:
                return window.MailAttachmentsResource.PL_FILE_SIZE_ERROR;
            case window.plupload.IMAGE_FORMAT_ERROR:
                return window.MailAttachmentsResource.PL_IMAGE_FORMAT_ERROR;
            case window.plupload.IMAGE_MEMORY_ERROR:
                return window.MailAttachmentsResource.PL_IMAGE_MEMORY_ERROR;
            case window.plupload.IMAGE_DIMENSIONS_ERROR:
                return window.MailAttachmentsResource.PL_IMAGE_DIMENSIONS_ERROR;
            default:
                return undefined;
        }
    }

    function onError(up, resp) {
        if (resp.file != undefined) {
            var error_message = resp.code != undefined ?
                getPlUploaderError(resp.code) || resp.message :
                resp.message;

            var new_resp = {
                response: JSON.stringify({
                    Success: false,
                    FileName: resp.file.name,
                    FileURL: '',
                    Data: {
                        contentId: null,
                        contentType: "",
                        fileId: -1,
                        fileName: resp.file.name,
                        fileNumber: -1,
                        size: 0,
                        storedName: "",
                        streamId: ""
                    },
                    Message: error_message
                })
            };

            onFileUploaded(up, resp.file, new_resp);

        } else
            showError(resp.message);
    }

    function hideError() {
        var error_limit_cnt = $('#id_block_errors_container');
        error_limit_cnt.hide();
    }

    function showError(error_text) {
        var error_limit_cnt = $('#id_block_errors_container');
        error_limit_cnt.show();
        error_limit_cnt.find('span').html(error_text);
    }

    function calculateAttachments() {
        if (is_saving)
            return;

        var full_size_in_bytes = 0;

        for (var i = 0; i < attachments.length; i++) {
            var attachment = attachments[i];
            full_size_in_bytes += attachment.size;
        }

        $('#attachments_count_label').text(window.MailResource.Attachments + (attachments.length > 0 ? " (" + attachments.length + "): " : ":"));
        $('#full-size-label').text(full_size_in_bytes > 0 ? window.MailResource.FullSize + ": " + getSizeString(full_size_in_bytes) : '');
    }

    function getLoadedAttachments() {
        var loaded_attachments = [];
        for (var i = 0; i < attachments.length; i++) {
            var file = attachments[i];
            if (file.fileId > 0) {
                loaded_attachments.push(
                    {
                        fileId: file.fileId,
                        fileName: file.fileName,
                        size: file.size,
                        contentType: file.contentType,
                        fileNumber: file.fileNumber,
                        storedName: file.storedName,
                        streamId: file.streamId
                    });
            }
        }
        return loaded_attachments;
    }

    function getTotalUploadedSize() {
        var total_uploaded_bytes = 0;
        for (var i = 0; i < attachments.length; i++) {
            var attachment = attachments[i];
            if (attachment.fileId != undefined && attachment.fileId > 0)
                total_uploaded_bytes += attachment.size;
        }
        return total_uploaded_bytes;
    }

    function getCookie(name) {
        var cookie = " " + document.cookie;
        var search = " " + name + "=";
        var set_str = null;
        var offset;
        var end;
        if (cookie.length > 0) {
            offset = cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = cookie.indexOf(";", offset);
                if (end == -1) {
                    end = cookie.length;
                }
                set_str = unescape(cookie.substring(offset, end));
            }
        }
        return (set_str);
    }

    function hideDragHighlight() {
        $('#' + upload_container_id)
            .removeClass('attachment_drag_highlight');
    }

    function showDragHighlight() {
        $('#' + upload_container_id)
            .addClass('attachment_drag_highlight');
    }

    function stopUploader() {
        $('#' + files_container + ' tbody').empty();
        attachments = [];
        if (add_docs_heandler_id != null || add_docs_heandler_id != undefined)
            serviceManager.unbind(add_docs_heandler_id);
        if (save_heandler_id != null || save_heandler_id != undefined)
            serviceManager.unbind(save_heandler_id);
        if (uploader != undefined) {
            uploader.unbindAll();
            uploader.stop();
            uploader.splice();
        }
    }

    function generateSubmitUrl(data) {
        var submit_url = 'UploadProgress.ashx?submit=ASC.Web.Mail.HttpHandlers.FilesUploader,ASC.Web.Mail';
        submit_url += '&asc_auth_key=' + encodeURIComponent(getCookie("asc_auth_key"));
        for (var prop in data) {
            submit_url += '&{0}={1}'.format(prop, data[prop]);
        }
        return submit_url;
    }

    function hideUpload() {
        $('#' + upload_container_id).hide();
    }

    function showUpload() {
        $('#' + upload_container_id).show();
    }

    function reloadAttachments(files) {
        var temp_collection = attachments.slice(); // clone array
        for (var i = 0; i < temp_collection.length; i++) {
            var attachment = temp_collection[i];
            if (attachment.fileId != -1)
                removeAttachment(attachment.orderNumber);
        }
        calculateAttachments();
        addAttachments(files);
        messagePage.initImageZoom();
    }


    function cutFileName(name){
        if(name.length <= max_file_name_len)
            return name;
        return name.substr(0, max_file_name_len-3) + '...';
    }

    function getAttachment(order_number) {
        var pos = searchFileIndex(attachments, order_number);
        if (pos > -1) {
            return attachments[pos];
        }
        return null;
    }
    
    function completeAttachment(attachment) {
        var name = attachment.fileName;
        var file_id = attachment.fileId || -1;
        var ext = getAttachmentExtension(name);
        var warn = getAttachmentWarningByExt(ext);

        attachment.isImage = file_id > 0 ? ASC.Files.Utility.CanImageView(name) : false;
        attachment.iconCls = ASC.Files.Utility.getCssClassByFileTitle(name, true);
        attachment.canView = file_id > 0 ? TMMail.canViewInDocuments(name) : false;
        attachment.canEdit = file_id > 0 ? TMMail.canEditInDocuments(name) : false;
        attachment.warn = warn;

        if (file_id <= 0)
            attachment.handlerUrl = '';
        else {
            attachment.handlerUrl = attachment.canView ?
                TMMail.getViewDocumentUrl(file_id) :
                TMMail.getAttachmentDownloadUrl(file_id);
        }

        return attachment;
    }

    function getOrderNumber() {
        return next_order_number + 1;
    }

    return {
        GetAttachments: getLoadedAttachments,
        MaxAttachmentSize: max_one_attachment_size,
        MaxTotalSize: max_all_attachments_size,
        InitUploader: init,
        StopUploader: stopUploader,
        RemoveAttachment: removeAttachment,
        RemoveAll: removeAllAttachments,
        IsLoading: isLoading,
        SwitchMode: switchMode,
        ShowUpload: showUpload,
        HideUpload: hideUpload,
        GetFileName: getAttachmentName,
        GetFileExtension: getAttachmentExtension,
        GetSizeString: getSizeString,
        ReloadAttachments: reloadAttachments,
        CutFileName: cutFileName,
        GetAttachment: getAttachment,
        CompleteAttachment: completeAttachment
    };

})(jQuery)