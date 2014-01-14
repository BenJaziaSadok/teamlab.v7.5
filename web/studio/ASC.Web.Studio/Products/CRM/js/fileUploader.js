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

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.CRM === "undefined")
    ASC.CRM = function() { return {} };

ASC.CRM.FileUploader = (function($) {

    return {
    
    fileUploader: null,

    filesUploadedInfo: new Array(),

    swfu: new Object,
    
    fileIDs: new Array(),

    OnAllUploadCompleteCallback_function: function()
    { },

    OnUploadCompleteCallback_function: function(file)
    {
        ASC.CRM.FileUploader.fileIDs.push(file.Data);
    },

    OnBeginCallback_function: function(data)
    {
        jq(".pm-ajax-info-block span").text(jq.format(ASC.CRM.Resources.CRMJSResource.UploadingProgress, data.CurrentFile)); 
    },

    isFileAPI: function()
    {
        return typeof FileReader != "undefined" && typeof (new XMLHttpRequest()).upload != "undefined";
    },

    activateUploader: function() {
        var progressTimeSpan = 1000;

        jq("object[id*='SWFUpload']").before('<span id="asc_fileuploaderSWFObj"></span>');

        try { jq("object[id*='SWFUpload']").remove(); } catch (e) { }

        var btnId = jq("#pm_upload_btn").attr("id") ||
                    jq("#pm_swf_button_container a.pm_upload_btn").attr("id"); //fix basic uploader

        if (ASC.CRM.FileUploader.isFileAPI())
        {
            jq("#pm_swf_button_container").hide();
            jq("#pm_upload_pnl").hide();
            btnId = jq(".pm_upload_btn_html5").attr("id"); //fix basic uploader, when html5 on and flash off
        } else {
            jq("#pm_swf_button_container").show();
            jq("#pm_upload_pnl").show();
        }

        if(typeof FileReader == 'undefined')
            jq("#pm_upload_pnl").show();

        var FileUploaderConfig = {
                FileUploadHandler: "ASC.Web.CRM.Classes.FileUploaderHandler, ASC.Web.CRM",
                AutoSubmit : false,
                UploadButtonID : btnId,
                TargetContainerID : 'history_uploadContainer',
                ProgressTimeSpan: progressTimeSpan,
                Data: { 'UserID': Teamlab.profile.id },
                FileSizeLimit : window.fileSizeLimit,
                OnAllUploadComplete : ASC.CRM.FileUploader.OnAllUploadCompleteCallback_function,
                OnUploadComplete : ASC.CRM.FileUploader.OnUploadCompleteCallback_function, 
                OnBegin : ASC.CRM.FileUploader.OnBeginCallback_function,
                
                DeleteLinkCSSClass : 'pm_deleteLinkCSSClass',
                LoadingImageCSSClass : 'pm_loadingCSSClass',
                CompleteCSSClass : 'pm_completeCSSClass',
                DragDropHolder : jq("#pm_DragDropHolder"),
                //FilesHeaderCountHolder : jq("#pm_uploadHeader"),
                OverAllProcessHolder : jq("#pm_overallprocessHolder"),
                OverAllProcessBarCssClass : 'pm_overAllProcessBarCssClass',
                
                AddFilesText : ASC.CRM.Resources.CRMJSResource.UploadFile,
                SelectFilesText : ASC.CRM.Resources.CRMJSResource.UploadFile,
                OverallProgressText : ASC.CRM.Resources.CRMJSResource.OverallProgress
            };
                
        fileUploader = FileHtml5Uploader.InitFileUploader(FileUploaderConfig);

        jq("#" + btnId).unbind("mouseover");//
        setTimeout(function () { jq("#history_uploadContainer").html(""); }, progressTimeSpan + 1);
        jq("#asc_fileuploaderSWFContainer").attr("style", "");
        jq("#"+fileUploader.ButtonID).css("visibility","");
        jq("#pm_upload_btn_html5").show();
    },
    
    showFileUploaderDialog: function()
    {
            var margintop = jq(window).scrollTop() - 135;
            margintop = margintop + 'px';

            jq.blockUI({ message: jq("#fileUploaderPanel"),
                css: {
                    left: '50%',
                    top: '25%',
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: '550px',

                    cursor: 'default',
                    textAlign: 'left',
                    position: 'absolute',
                    'margin-left': '-300px',
                    'margin-top': margintop,
                    'background-color': 'White'
                },

                overlayCSS: {
                    backgroundColor: '#AAA',
                    cursor: 'default',
                    opacity: '0.3'
                },
                focusInput: false,
                baseZ: 666,

                fadeIn: 0,
                fadeOut: 0
            });
        }

}
})(jQuery);