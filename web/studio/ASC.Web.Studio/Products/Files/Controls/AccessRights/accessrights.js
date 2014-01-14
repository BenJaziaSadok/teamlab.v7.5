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

window.ASC.Files.Share = (function () {
    var isInit = false;
    var objectID;
    var objectTitle;
    var sharingInfo = [];
    var linkInfo;
    var sharingManager = null;
    var shareLink;
    var shortenLink = "";
    var needUpdate = false;

    var clip = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;

            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.GetSharedInfo, onGetSharedInfo);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.GetSharedInfoShort, onGetSharedInfoShort);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.SetAceObject, onSetAceObject);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.UnSubscribeMe, onUnSubscribeMe);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.GetShortenLink, onGetShortenLink);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.SendLinkToEmail, onSendLinkToEmail);

            sharingManager = new SharingSettingsManager(undefined, null);
            sharingManager.OnSave = setAceObject;
        }
    };

    var getAceString = function (aceStatus) {
        if (aceStatus == "owner") {
            return ASC.Files.FilesJSResources.AceStatusEnum_Owner;
        }
        aceStatus = parseInt(aceStatus);
        switch (aceStatus) {
            case ASC.Files.Constants.AceStatusEnum.Read:
                return ASC.Files.FilesJSResources.AceStatusEnum_Read;
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                return ASC.Files.FilesJSResources.AceStatusEnum_ReadWrite;
            case ASC.Files.Constants.AceStatusEnum.Restrict:
                return ASC.Files.FilesJSResources.AceStatusEnum_Restrict;
            default:
                return "";
        }
    };

    var updateClip = function () {
        if (jq.browser.mobile) {
            return;
        }

        if (ASC.Files.Share.clip) {
            ASC.Files.Share.clip.destroy();
        }

        if (jq("#shareLinkCopy:visible").length != 0) {
            var linkButton = "shareLinkCopy";
            var url = jq("#shareLink").val();
        } else if (jq("#embeddedCopy:visible").length != 0) {
            linkButton = "embeddedCopy";
            url = jq("#shareEmbedded").val();
        } else {
            return;
        }

        var offsetLink = jq("#" + linkButton).offset();
        var offsetDiff = jq(".blockUI #studio_sharingSettingsDialog").offset() || { left: 0, top: 0 };

        ASC.Files.Share.clip = new ZeroClipboard.Client();
        ASC.Files.Share.clip.setText(url);
        ASC.Files.Share.clip.glue(linkButton, linkButton,
            {
                zIndex: 670,
                left: offsetLink.left - offsetDiff.left + "px",
                top: offsetLink.top - offsetDiff.top + "px"
            });
        ASC.Files.Share.clip.addEventListener("onComplete", function () {
            jq("#shareEmbedded, #embeddedCopy, #shareLink, #shareLinkCopy").yellowFade();
        });
    };

    var updateSocialLink = function (url) {
        var linkPanel = jq("#shareViaSocPanel");
        var link = encodeURIComponent(url);

        linkPanel.find(".google").attr("href", ASC.Files.Constants.URL_SHARE_GOOGLE_PLUS.format(link));
        linkPanel.find(".facebook").attr("href", ASC.Files.Constants.URL_SHARE_FACEBOOK.format(link, objectTitle, "", ""));
        linkPanel.find(".twitter").attr("href", ASC.Files.Constants.URL_SHARE_TWITTER.format(link));
    };

    var addShareMail = function () {
        var tmp = jq("#shareMailPanel .recipientMail:first").clone(false);
        jq("#shareMailPanel .recipientMail .textEdit").each(function (i, e) {
            e = jq(e);
            if (e.val().trim() == "") {
                jq(e).closest(".recipientMail").remove();
            }
        });

        tmp.find(".textEdit").val("");
        jq("#shareLinkMailAdd").before(tmp);

        if (jq("#shareMailPanel .recipientMail").length >= 4) {
            jq("#shareLinkMailAdd").addClass("display-none");
        }
    };

    var deleteShareMail = function (obj) {
        var target = obj.target || obj.srcElement;
        var parent = jq(target).parent();
        if (jq("#shareMailPanel .recipientMail").length > 1) {
            parent.remove();
        } else {
            parent.find(".textEdit").val("");
        }
        jq("#shareLinkMailAdd").removeClass("display-none");
    };

    var showEmbeddedPanel = function () {
        jq("#studio_sharingSettingsDialog").removeClass("outside-panel");
        jq("#studio_sharingSettingsDialog").addClass("embedded-panel");
        jq("#shareSidePanel .active").removeClass("active");
        jq("#shareSideEmbedded").addClass("active");
        jq("#shareMailPanel").hide();
        jq(".share-link-close").show();

        jq("#sharingLinkItem label:has(input[value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "'])").hide();

        var embeddedAccess =
            (linkInfo === ASC.Files.Constants.AceStatusEnum.Restrict
                ? ASC.Files.Constants.AceStatusEnum.Restrict
                : ASC.Files.Constants.AceStatusEnum.Read);
        jq("#sharingLinkItem input[value='" + embeddedAccess + "']").prop("checked", true);

        updateClip();
    };

    var showOutsidePanel = function () {
        jq("#studio_sharingSettingsDialog").removeClass("embedded-panel");
        jq("#studio_sharingSettingsDialog").addClass("outside-panel");
        jq("#shareSidePanel .active").removeClass("active");
        jq("#shareSideOutside").addClass("active");

        if (ASC.Files.Utility.CanWebEdit(objectTitle, true) && ASC.Resources.Master.TenantTariffDocsEdition
            && !ASC.Files.Utility.MustConvert(objectTitle)) {
            jq("#sharingLinkItem label:has(input[value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "'])").show();
        }

        jq("#sharingLinkItem input[value='" + linkInfo + "']").prop("checked", true);
        updateClip();
    };

    var showMainPanel = function () {
        jq("#studio_sharingSettingsDialog").removeClass("outside-panel embedded-panel");
        jq("#shareSidePanel .active").removeClass("active");
        jq("#shareSidePortal").addClass("active");
    };

    var showShareLink = function (ace) {
        jq(".share-link-close").show();
        jq("#shareLinkPanel, #shareEmbeddedPanel").removeClass("display-none");
        jq("#shareMailPanel").hide();
        jq("#getShortenLink").hide();
        jq("#sharingLinkDeny").hide();

        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Read:
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                if (shortenLink == "") {
                    jq("#getShortenLink").show();
                }
                break;
        }
    };

    var changeShareLinkAce = function (event) {
        var url = shareLink;

        if (shortenLink != "") {
            url = shortenLink;
        }

        var ace = parseInt(jq("#sharingLinkItem input:checked").val());
        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Restrict:
                url = "";
                jq("#shareLinkPanel, #shareEmbeddedPanel").addClass("display-none");
                jq("#sharingLinkDeny").show();
                jq(".share-link-close").show();
                break;
            case ASC.Files.Constants.AceStatusEnum.Read:
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                showShareLink(ace);
                break;
        }

        jq("#shareLink").val(url);

        updateSocialLink(url);

        updateClip();
        linkInfo = ace;

        if (typeof event != "undefined") {
            saveShareLinkAccess();
        }
    };

    var renderGetLink = function (arrayActions) {
        jq("#studio_sharingSettingsDialog").addClass("with-link");

        if (jq("#studio_sharingSettingsDialog #shareSelectorBody").length == 0) {
            jq("#shareSelectorBody").prependTo("#studio_sharingSettingsDialog .containerBodyBlock");

            if (jq.browser.mobile) {
                jq("#shareLinkCopy").remove();
                jq("#shareLink").attr("readonly", "false").attr("readonly", "").removeAttr("readonly");
            } else {
                jq("#shareLinkBody").on("mousedown", "#shareLink, #shareEmbedded", function () {
                    jq(this).select();
                    return false;
                });
            }
            jq("#shareLinkPanel").on(jq.browser.webkit ? "keydown" : "keypress", "#shareLink", function () {
                return false;
            });

            for (var i in arrayActions) {
                if (typeof arrayActions[i] == "object") {
                    var htmlOpt =
                        "<label><input type=\"radio\" name=\"linkShareOpt\" value=\"{0}\" {2}>{1}</label>".format(
                            arrayActions[i].id,
                            arrayActions[i].name,
                            arrayActions[i].defaultAction ? "checked=\"checked\"" : ""
                        );
                    jq("#sharingLinkItem").append(htmlOpt);
                }
            }

            jq("#shareSideEmbedded").on("click", showEmbeddedPanel);
            jq("#shareSideOutside").on("click", showOutsidePanel);
            jq("#shareSidePortal").on("click", showMainPanel);
            jq("#shareMailPanel").on("click", ".recipientMail .baseLinkAction", deleteShareMail);
            jq("#shareLinkPanel").on("click", "#shareLinkMailAdd", addShareMail);
            jq("#shareLinkPanel").on("click", "#getShortenLink", getShortenLink);
            jq("#shareLinkPanel").on("click", "#shareSendLinkToEmail", sendLinkToEmail);
            jq("#shareEmbeddedPanel").on("click", ".embedded-size-item", setEmbeddedSize);
            jq("#shareEmbeddedPanel").on("change", ".embedded-size-custom input", setEmbeddedSize);

            jq("#shareViaSocPanel").on("click", "li a", function () {
                window.open(jq(this).attr("href"), "new", "height=600,width=1020,fullscreen=0,resizable=0,status=0,toolbar=0,menubar=0,location=1");
                return false;
            });

            jq("#shareLinkPanel").on("click", "#shareViaMail", function () {
                jq("#shareMailPanel, .share-link-close").toggle();
            });

            jq("#sharingLinkItem input").on("change", changeShareLinkAce);
            jq("#sharingSettingsItems").on("scroll", updateClip);
        }

        jq("#sharingLinkItem input[value='" + linkInfo + "']").prop("checked", true);

        updateClip();

        if (ASC.Files.Utility.CanWebEdit(objectTitle, true) && ASC.Resources.Master.TenantTariffDocsEdition
            && !ASC.Files.Utility.MustConvert(objectTitle)) {
            jq("#sharingLinkItem label:has(input[value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "'])").show();
        } else {
            jq("#sharingLinkItem label:has(input[value='" + ASC.Files.Constants.AceStatusEnum.ReadWrite + "'])").hide();
        }

        shortenLink = "";

        changeShareLinkAce();
    };

    var renderEmbeddedPanel = function () {
        if (ASC.Files.Utility.CanWebView(objectTitle) && ASC.Resources.Master.TenantTariffDocsEdition) {
            jq("#shareSideEmbedded").show();
            setEmbeddedSize();
        } else {
            jq("#shareSideEmbedded").hide();
            showMainPanel();
        }
    };

    var setEmbeddedSize = function () {
        jq(".embedded-size-item").removeClass("selected");
        var target = jq(this);
        if (target.is(".embedded-size-custom input")) {
            var heightTmp = jq("#embeddedSizeTemplate .embedded-size-custom input[name='height']").val();
            heightTmp = Math.abs(heightTmp) || 0;
            if (heightTmp) {
                var height = heightTmp + "px";
            } else {
                jq("#embeddedSizeTemplate .embedded-size-custom input[name='height']").val("");
            }
            var widthTmp = jq("#embeddedSizeTemplate .embedded-size-custom input[name='width']").val();
            widthTmp = Math.abs(widthTmp) || 0;
            if (widthTmp) {
                var width = widthTmp + "px";
            } else {
                jq("#embeddedSizeTemplate .embedded-size-custom input[name='width']").val("");
            }
        } else if (target.is(".embedded-size-item")) {
            target.addClass("selected");
            jq("#embeddedSizeTemplate .embedded-size-custom input").val("");
            if (target.hasClass("embedded-size-8x6")) {
                height = "800px";
                width = "600px";
            } else if (target.hasClass("embedded-size-6x4")) {
                height = "600px";
                width = "400px";
            }
        } else {
            jq(".embedded-size-item:first").addClass("selected");
        }

        generateEmbeddedString(height, width);
    };

    var generateEmbeddedString = function (height, width) {
        height = height || "100%";
        width = width || "100%";
        var embeddedString = '<iframe src="{0}" height="{1}" width="{2}" frameborder="0" scrolling="no" allowtransparency></iframe>';

        var url = shareLink + "&action=embedded";

        embeddedString = embeddedString.format(url, height, width);

        jq("#shareEmbedded").val(embeddedString);
        updateClip();
    };

    var unSubscribeMe = function (entryType, entryId) {
        if (ASC.Files.Folders.folderContainer != "forme") {
            return;
        }
        var list = new Array();

        var textFolder = "";
        var textFile = "";
        var strHtml = "<label title=\"{0}\"><input type=\"checkbox\" entryType=\"{1}\" entryId=\"{2}\" checked=\"checked\">{0}</label>";

        if (entryType && entryId) {
            list.push({ entryType: entryType, entryId: entryId });

            var entryRowTitle = ASC.Files.UI.getEntryTitle(entryType, entryId);

            if (entryType == "file") {
                textFile += strHtml.format(entryRowTitle, entryType, entryId);
            } else {
                textFolder += strHtml.format(entryRowTitle, entryType, entryId);
            }
        } else {
            jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):has(.checkbox input:checked)").each(function () {
                var entryRowData = ASC.Files.UI.getObjectData(this);
                var entryRowType = entryRowData.entryType;
                var entryRowId = entryRowData.entryId;

                list.push({ entryType: entryRowType, entryId: entryRowId });

                entryRowTitle = entryRowData.title;
                if (entryRowType == "file") {
                    textFile += strHtml.format(entryRowTitle, entryRowType, entryRowId);
                } else {
                    textFolder += strHtml.format(entryRowTitle, entryRowType, entryRowId);
                }
            });
        }

        if (list.length == 0) {
            return;
        }

        jq("#confirmUnsubscribeList dd.confirm-remove-files").html(textFile);
        jq("#confirmUnsubscribeList dd.confirm-remove-folders").html(textFolder);

        jq("#confirmUnsubscribeList .confirm-remove-folders, #confirmUnsubscribeList .confirm-remove-files").show();
        if (textFolder == "") {
            jq("#confirmUnsubscribeList .confirm-remove-folders").hide();
        }
        if (textFile == "") {
            jq("#confirmUnsubscribeList .confirm-remove-files").hide();
        }

        ASC.Files.UI.blockUI(jq("#filesConfirmUnsubscribe"), 420, 0, -150);

        PopupKeyUpActionProvider.EnterAction = "jq(\"#unsubscribeConfirmBtn\").click();";
    };

    var updateForParent = function () {
        if (needUpdate) {
            ASC.Files.Share.getSharedInfo(ASC.Files.UI.parseItemId(objectID).entryType, ASC.Files.UI.parseItemId(objectID).entryId, objectTitle, false, true);
        } else {
            onGetSharedInfoShort(null);
        }
    };

    //request

    var getSharedInfo = function (entryType, entryId, objTitle, asFlat, asResult) {
        objectID = entryType + "_" + entryId;
        objectTitle = objTitle;

        ASC.Files.ServiceManager.request("get",
            "json",
            !asResult ? ASC.Files.TemplateManager.events.GetSharedInfo : ASC.Files.TemplateManager.events.GetSharedInfoShort,
            { showLoading: true, asFlat: asFlat === true },
            "sharedinfo" + (!asResult ? "" : "short") + "?objectId=" + encodeURIComponent(objectID));
    };

    var setAceObject = function (data) {
        var dataItems = data.items;
        var aceWrapperList = new Array();

        for (var i = 0; i < dataItems.length; i++) {
            var dataItem = dataItems[i];
            var change = true;
            for (var j = 0; j < sharingInfo.length && change; j++) {
                var dataItemOld = sharingInfo[j];
                if (dataItemOld.id === dataItem.id) {
                    if (dataItemOld.selectedAction.id == dataItem.selectedAction.id) {
                        change = false;
                    } else {
                        break;
                    }
                }
            }

            if (change) {
                aceWrapperList.push(
                    {
                        id: dataItem.id,
                        is_group: dataItem.isGroup,
                        ace_status: dataItem.selectedAction.id
                    });
            }
        }

        //remove
        for (j = 0; j < sharingInfo.length; j++) {
            dataItemOld = sharingInfo[j];
            change = true;
            for (i = 0; i < dataItems.length; i++) {
                dataItem = dataItems[i];
                if (dataItemOld.id === dataItem.id) {
                    change = false;
                    break;
                }
            }
            if (change) {
                aceWrapperList.push(
                    {
                        id: dataItemOld.id,
                        is_group: dataItemOld.isGroup,
                        ace_status: ASC.Files.Constants.AceStatusEnum.None
                    });
            }
        }

        needUpdate = aceWrapperList.length;
        if (!needUpdate) {
            return;
        }

        var notify = jq("#shareMessageSend").prop("checked") == true;
        if (notify) {
            aceWrapperList[0].message = Encoder.htmlEncode((jq("#shareMessage:visible").val() || "").trim());
        }

        var dataXml = ASC.Files.Common.jsonToXml({ ace_wrapperList: { entry: aceWrapperList } });

        ASC.Files.ServiceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.SetAceObject,
            { showLoading: true },
            dataXml,
            "setaceobject?objectId=" + encodeURIComponent(objectID)
                + "&notify=" + notify);
    };

    var saveShareLinkAccess = function () {
        var aceWrapperList =
            [{
                id: ASC.Files.Constants.ShareLinkId,
                is_group: true,
                ace_status: linkInfo
            }];

        if (!aceWrapperList.length) {
            return;
        }

        var dataXml = ASC.Files.Common.jsonToXml({ ace_wrapperList: { entry: aceWrapperList } });

        ASC.Files.ServiceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.SetAceObject,
            { showLoading: true, clearData: false },
            dataXml,
            "setaceobject?objectId=" + encodeURIComponent(objectID));
    };

    var confirmUnSubscribe = function () {
        if (jq("#filesConfirmUnsubscribe:visible").length == 0) {
            return;
        }

        PopupKeyUpActionProvider.CloseDialog();

        var listChecked = jq("#confirmUnsubscribeList input:checked");
        if (listChecked.length == 0) {
            return;
        }

        var data = {};
        data.entry = new Array();

        var list = new Array();
        for (var i = 0; i < listChecked.length; i++) {
            var entryConfirmType = jq(listChecked[i]).attr("entryType");
            var entryConfirmId = jq(listChecked[i]).attr("entryId");
            var entryConfirmObj = ASC.Files.UI.getEntryObject(entryConfirmType, entryConfirmId);
            ASC.Files.UI.blockObject(entryConfirmObj, true, ASC.Files.FilesJSResources.DescriptRemove, true);
            data.entry.push(entryConfirmType + "_" + entryConfirmId);
            list.push({ entryId: entryConfirmId, entryType: entryConfirmType });
        }
        ASC.Files.UI.updateMainContentHeader();

        ASC.Files.ServiceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.UnSubscribeMe,
            { parentFolderID: ASC.Files.Folders.currentFolder.id, showLoading: true, list: list },
            { stringList: data },
            "removeace");
    };

    var getShortenLink = function () {
        var fileId = ASC.Files.UI.parseItemId(objectID).entryId;

        ASC.Files.ServiceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.GetShortenLink,
            {},
            "shorten?fileId=" + encodeURIComponent(fileId));

        jq("#getShortenLink").hide();

        updateClip();
    };

    var sendLinkToEmail = function () {
        var fileId = ASC.Files.UI.parseItemId(objectID).entryId;
        var message = jq("#shareMailText").val().trim();

        var stringList = new Array();

        jq("#shareMailPanel .recipientMail .textEdit").each(function () {
            var mail = jq(this).val();
            if (mail) {
                mail = mail.trim();
                if (jq.isValidEmail(mail)) {
                    stringList.push(mail);
                }
            }
        });

        if (!stringList.length) {
            jq("#shareMailPanel .recipientMail .textEdit:first").focus();
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_EmptyField, true);
            return;
        }

        Encoder.EncodeType = "!entity";
        var dataResult = [
            {
                key: Encoder.htmlEncode(message),
                value: { entry: stringList }
            }
        ];
        Encoder.EncodeType = "entity";

        var dataXml = ASC.Files.Common.jsonToXml({ stringListHash: { entry: dataResult } });

        ASC.Files.ServiceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.SendLinkToEmail,
            {},
            dataXml,
            "sendlinktoemail?fileId=" + encodeURIComponent(fileId));
    };

    //event handler

    var onGetSharedInfo = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined" || typeof jsonData == "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }
        sharingInfo = jsonData;

        var translateItems = [];
        var linkAccess;
        linkInfo = ASC.Files.Constants.AceStatusEnum.None;
        jq(jsonData).each(function (i) {
            var item = jsonData[i];
            if (item.id === ASC.Files.Constants.ShareLinkId) {
                shareLink = item.title;
                linkInfo = item.ace_status;
                linkAccess = item.ace_status;
            } else {
                translateItems.push(
                    {
                        "id": item.id,
                        "name": item.title,
                        "isGroup": item.is_group,
                        "canEdit": !(item.locked || item.owner),
                        "hideRemove": item.disable_remove,
                        "selectedAction": {
                            "id": item.ace_status,
                            "name": getAceString(item.owner ? "owner" : item.ace_status),
                            "defaultAction": false
                        }
                    });
            }
        });
        var arrayActions = [
            {
                "id": ASC.Files.Constants.AceStatusEnum.Read,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.Read),
                "defaultAction": true
            },
            {
                "id": ASC.Files.Constants.AceStatusEnum.ReadWrite,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.ReadWrite),
                "defaultAction": false
            },
            {
                "id": ASC.Files.Constants.AceStatusEnum.Restrict,
                "name": getAceString(ASC.Files.Constants.AceStatusEnum.Restrict),
                "defaultAction": false
            }
        ];

        var translateData = {
            "actions": arrayActions,
            "items": translateItems
        };

        sharingInfo = translateItems;

        sharingManager.UpdateSharingData(translateData);

        jq("#studio_sharingSettingsDialog").removeClass("with-link");
        var width = 600;
        if (linkAccess) {
            renderGetLink(arrayActions);

            renderEmbeddedPanel();

            showMainPanel();

            if (ASC.Files.Constants.YOUR_DOCS) {
                jq("#sharingSettingsDialogBody").css("border-left", "none");

                showOutsidePanel();
            } else {
                width = 830;
            }
        }

        sharingManager.ShowDialog(width, params.asFlat);
        PopupKeyUpActionProvider.EnterAction = "jq('#sharingSettingsSaveButton:visible, .share-link-close:visible, #shareSendLinkToEmail:visible').click();";
        if (params.asFlat) {
            PopupKeyUpActionProvider.CloseDialogAction = "ASC.Files.Share.updateForParent();";
        }

        jq("body").css("overflow", "");

        updateClip();

        if (ASC.Files.Folders) {
            var accessHead = jq(".share-container-head-corporate");
            var shareHead = jq(".share-container-head");

            if (ASC.Files.Folders.folderContainer == "corporate") {
                jq("#studio_sharingSettingsDialog #shareMessagePanel").hide();
                shareHead.hide();
                if (!accessHead.length) {
                    accessHead = jq("<span class=\"share-container-head-corporate\">{0}</span>".format(ASC.Files.FilesJSResources.AccessSettingsTitle));
                    shareHead.after(accessHead);
                }
                accessHead.show();
            } else {
                jq("#studio_sharingSettingsDialog #shareMessagePanel").show();
                shareHead.show();
                accessHead.hide();
            }
        }
    };

    var onGetSharedInfoShort = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined" || typeof jsonData == "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        if (jsonData) {
            var data =
                {
                    needUpdate: true,
                    sharingSettings: jsonData
                };
        } else {
            data = { needUpdate: false };
        }

        var message = JSON.stringify(data);
        window.parent.postMessage(message, "*");
    };

    var onSetAceObject = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        var itemId = ASC.Files.UI.parseItemId(objectID);
        var entryObj = ASC.Files.UI.getEntryObject(itemId.entryType, itemId.entryId);

        entryObj.toggleClass("share-open", jsonData === true);

        if (params.clearData) {
            objectID = null;
        }
    };

    var onUnSubscribeMe = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        var list = params.list;
        var foldersCountChange = false;

        for (var i = 0; i < list.length; i++) {
            if (!foldersCountChange && list[i].entryType == "folder") {
                foldersCountChange = true;
            }

            ASC.Files.Marker.removeNewIcon(list[i].entryType, list[i].entryId);
            ASC.Files.UI.getEntryObject(list[i].entryType, list[i].entryId).remove();
        }

        if (foldersCountChange && ASC.Files.Tree) {
            ASC.Files.Tree.resetNode(params.parentFolderID);
        }

        ASC.Files.UI.checkEmptyContent();
    };

    var onGetShortenLink = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }
        if (jsonData == null || jsonData == "") {
            return;
        }

        jq("#shareLink").val(jsonData);

        var ace = parseInt(jq("#sharingLinkItem input:checked").val());
        switch (ace) {
            case ASC.Files.Constants.AceStatusEnum.Read:
            case ASC.Files.Constants.AceStatusEnum.ReadWrite:
                shortenLink = jsonData;
                break;
        }

        updateSocialLink(jsonData);
        updateClip();
    };

    var onSendLinkToEmail = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        sharingManager.SaveAndCloseDialog();
    };

    return {
        init: init,

        updateForParent: updateForParent,
        getSharedInfo: getSharedInfo,

        setAceObject: setAceObject,
        unSubscribeMe: unSubscribeMe,
        confirmUnSubscribe: confirmUnSubscribe,

        clip: clip
    };
})();

jq(document).ready(function () {
    (function ($) {
        ASC.Files.Share.init();
        $(function () {

            jq("#buttonUnsubscribe, #mainUnsubscribe").click(function () {
                if (jq(this).is("#mainUnsubscribe") && !jq(this).hasClass("unlockAction")) {
                    return;
                }
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Share.unSubscribeMe();
            });

            jq("#unsubscribeConfirmBtn").click(function () {
                ASC.Files.Share.confirmUnSubscribe();
            });

            jq("#filesMainContent").on("click", ".share-action", function () {
                var entryData = ASC.Files.UI.getObjectData(this);
                var entryId = entryData.entryId;
                var entryType = entryData.entryType;
                var entryTitle = entryData.title;
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Share.getSharedInfo(entryType, entryId, entryTitle);
                return false;
            });

            jq("#studio_sharingSettingsDialog .containerBodyBlock").addClass("clearFix");
        });
    })(jQuery);
});