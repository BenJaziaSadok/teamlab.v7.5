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

(function () {
    // init jQuery Datepicker
    if (jQuery && jQuery.datepicker) {
        jQuery.datepicker.setDefaults({
            //changeMonth: true,
            //changeYear: true,
            prevText: "",
            nextText: "",
            firstDay: ASC.Resources.Master.FirstDay,
            dateFormat: ASC.Resources.Master.DatepickerDatePattern,
            dayNamesMin: ASC.Resources.Master.DayNames,
            dayNamesShort: ASC.Resources.Master.DayNames,
            dayNames: ASC.Resources.Master.DayNamesFull,
            monthNamesShort: ASC.Resources.Master.MonthNames,
            monthNames: ASC.Resources.Master.MonthNamesFull
        });
    }

    // init API Manager
    ServiceManager.init(ASC.Resources.Master.ApiPath);
    ServiceFactory.init({
        responses: {
            isme: ASC.Resources.Master.ApiResponsesMyProfile
        },
        portaldatetime: {
            utcoffsettotalminutes: ASC.Resources.Master.TimezoneOffsetMinutes,
            displayname: ASC.Resources.Master.TimezoneDisplayName
        },
        names: {
            months: ASC.Resources.Master.MonthNamesFull,
            shortmonths: ASC.Resources.Master.MonthNames,
            days: ASC.Resources.Master.DayNamesFull,
            shortdays: ASC.Resources.Master.DayNames
        },
        formats: {
            datetime: ASC.Resources.Master.DateTimePattern,
            time: ASC.Resources.Master.TimePattern,
            date: ASC.Resources.Master.DatePattern
        },
        avatars: {
            small: ASC.Resources.Master.AvatarSmall,
            medium: ASC.Resources.Master.AvatarMedium,
            large: ASC.Resources.Master.AvatarLarge
        },
        supportedfiles: {
            imgs: ASC.Files.Utility.Resource.ExtsImagePreviewed,
            docs: ASC.Files.Utility.Resource.ExtsWebPreviewed
        }
    });
    Teamlab.init();

    //init StudioManagement
    StudioManagement._addGroup = ASC.Resources.Master.AddDepartmentHeader;
    StudioManagement._editGroup = ASC.Resources.Master.EditDepartmentHeader;
    StudioManagement._btnAddGroup = ASC.Resources.Master.Resource.AddButton;
    StudioManagement._btnEditGroup = ASC.Resources.Master.Resource.EditButton;
    StudioManagement._confirmDeleteGroup = ASC.Resources.Master.ConfirmRemoveDepartment;

    // init LoadingBanner
    LoadingBanner.strLoading = ASC.Resources.Master.Resource.LoadingProcessing;
    LoadingBanner.strDescription = ASC.Resources.Master.Resource.LoadingDescription;

    // init AuthManager
    AuthManager.ConfirmMessage = ASC.Resources.Master.Resource.ConfirmMessage;
    AuthManager.ConfirmRemoveUser = ASC.Resources.Master.ConfirmRemoveUser;

    // init uploader settings
    if (typeof(ASC) == 'undefined') {
        ASC = {};
    }
    if (typeof(ASC.Controls) == 'undefined') {
        ASC.Controls = {};
    }
    if (typeof(ASC.Controls.FileUploaderGlobalConfig) == 'undefined') {
        ASC.Controls.FileUploaderGlobalConfig = {};
    }

    ASC.Controls.FileUploaderGlobalConfig.DeleteText = ASC.Resources.Master.Resource.DeleteButton;
    ASC.Controls.FileUploaderGlobalConfig.ErrorFileSizeLimitText = ASC.Resources.Master.ErrorFileSizeLimit;
    ASC.Controls.FileUploaderGlobalConfig.ErrorFileEmptyText = ASC.Resources.Master.Resource.ErrorFileEmptyText;
    ASC.Controls.FileUploaderGlobalConfig.ErrorFileTypeText = ASC.Resources.Master.Resource.ErrorFileTypeText;

    ASC.Controls.FileUploaderGlobalConfig.DescriptionCSSClass = 'studioFileUploaderDescription';
    ASC.Controls.FileUploaderGlobalConfig.FileNameCSSClass = 'studioFileUploaderFileName';
    ASC.Controls.FileUploaderGlobalConfig.DeleteLinkCSSClass = 'linkAction';
    ASC.Controls.FileUploaderGlobalConfig.ProgressBarCSSClass = 'studioFileUploaderProgressBar';
    ASC.Controls.FileUploaderGlobalConfig.ErrorBarCSSClass = 'studioFileUploaderErrorProgressBar';
    ASC.Controls.FileUploaderGlobalConfig.ErrorTextCSSClass = 'studioFileUploaderErrorDescription';

    ASC.Controls.FileUploaderGlobalConfig.DefaultItemTemplate = "<div id=\"fu_item_${id}\" class=\"fu_item\"><table cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%;border-bottom:1px solid #D1D1D1;\"><tr style=\"height:40px !important;\" valign=\"middle\"><td style=\"width:70%; padding-left:10px;\"><div class=\"name ${fileNameCss}\" style=\"padding-right:5px;\" title=\"${fileName}\" >${fileName}<input type=\"hidden\" id=\"fu_itemName_hidden_{id}\" value=\"{id}\"/><div class=\"${descriptionCss}\">${descriptionText}</div></div></td><td class=\"size\" style=\"padding-left: 10px; width:20%; {{if exceedLimit}}color: red;{{else}}color: #83888D;{{/if}}\"><div>${fileSize}&nbsp;</div></td><td style=\"width:10%;\">&nbsp;<a id=\"fu_item_loading_${id}\" class=\"${loadingCss}\" style=\"display:none;\"></a><a id=\"fu_item_complete_${id}\" class=\"${completeCss}\" style=\"display:none;\"></a><a id=\"fu_item_delete_${id}\" class=\"fu_item_delete ${deleteCss}\" href=\"#\"></a></td></tr></table><div class=\"studioFileUploaderProgressBar\" style=\"margin-top:-41px; float:left; width:${progressBarWidth};height:40px;\" ></div></div>";
    ASC.Controls.FileUploaderGlobalConfig.DefaultProgressTemplate = "<table cellpadding='5' cellspacing='0' width='100%' style='padding:10px 0;' class='describe-text'><tr><td width='100'>${overallProgressText}</td><td><div style=\"margin-top: 1px;\" class=\"${overAllProcessBarCssClass}\"><div></div></div></td><td class='fu_percent' width='20'></td></tr></table>";

    // init image zoom
    StudioManager.initImageZoom();

    //for ie10 css
    if (jq.browser.msie && jq.browser.version == 10) {
        jq("body").addClass("ie10");
    }

    //settings preloaded
    jq.blockUI.defaults.css = {};
    jq.blockUI.defaults.overlayCSS = {};
    jq.blockUI.defaults.fadeOut = 0;
    jq.blockUI.defaults.fadeIn = 0;
    jq.blockUI.defaults.message = '<img alt="" src="' + StudioManager.GetImage('loader_16.gif') + '"/>';

    if (jq("#studio_sidePanel").length) {
        LeftMenuManager.init(StudioManager.getBasePathToModule(), jq(".menu-list .menu-item.sub-list, .menu-list>.menu-item.sub-list>.menu-sub-list>.menu-sub-item"));
        LeftMenuManager.restoreLeftMenu();

        jq(".support-link").on("click", function () {
            jq(".support .expander").trigger("click");
        });
    }
    // init page-menu actions
    LeftMenuManager.bindEvents();

    // init RenderPromoBar
    if (ASC.Resources.Master.SetupInfoNotifyAddress &&
        ASC.Resources.Master.IsAuthenticated == true &&
        ASC.Resources.Master.ApiResponsesMyProfile.response) {

        jq.getScript(
            [
                ASC.Resources.Master.SetupInfoNotifyAddress,
                "&userId=",
                ASC.Resources.Master.ApiResponsesMyProfile.response.id,
                "&page=",
                location.pathname, location.search,
                "&language=",
                ASC.Resources.Master.CurrentCultureName,
                "&admin=",
                ASC.Resources.Master.IsAdmin,
                "&promo=",
                ASC.Resources.Master.ShowPromotions,
                "&version=",
                ASC.Resources.Master.CurrentTenantVersion,
                "&tariff=",
                ASC.Resources.Master.TenantTariff
            ].join(""));
    }

    // Disable auto submit form by enter key
    jQuery(function () {
        var keyStop = {
            //8: ":not(input:text, textarea, input:file, input:password)", // stop backspace = back
            13: "input:text, input:password", // stop enter = submit 
            end: null
        };
        jq(document).bind("keydown", function (event) {
            var selector = keyStop[event.which];

            if (selector !== undefined && jq(event.target).is(selector)) {
                event.preventDefault(); //stop event
            }
            return true;
        });
    });
})();

jq("table.mainPageTable").tlBlock();
setTimeout("jq(window).resize()", 500); //hack for resizing filter

jq(window).bind("resize", function () {
    // hide all popup's
    jq(".studio-action-panel").hide();
});

// init uvOptions
var uvOptions = {
    custom_fields: {
        "premium": ASC.Resources.Master.TenantQuotaIsTrial
    }
};