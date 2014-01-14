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

window.ASC.Files.ThirdParty = (function () {
    var isInit = false;
    var thirdPartyList = {
        BoxNet: { key: "BoxNet", customerTitle: "Box folder", providerTitle: "box.com" },
        DropBox: { key: "DropBox", customerTitle: "DropBox folder", providerTitle: "dropbox.com" },
        Google: { key: "Google", customerTitle: "Google folder", providerTitle: "google.com" },
        SkyDrive: { key: "SkyDrive", customerTitle: "SkyDrive folder", providerTitle: "skydrive.com" }
    };

    var init = function () {
        if (isInit === false) {
            isInit = true;

            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.GetThirdParty, onGetThirdParty);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.SaveThirdParty, onSaveThirdParty);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.DeleteThirdParty, onDeleteThirdParty);
            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.ChangeAccessToThirdparty, onChangeAccessToThirdparty);

            jq.dropdownToggle({
                switcherSelector: "#files_addThirdParty_btn, #topAddThirdParty img",
                dropdownID: "addThirdPartyPanel",
                anchorSelector: "#topAddThirdParty img"
            });

            jq(document).click(function (event) {
                jq.dropdownToggle().registerAutoHide(event, ".account-row .menu-small", "#thirdPartyActionPanel");
            });
        }
    };

    var isThirdParty = function (entryObject) {
        if (entryObject == null) {
            return ASC.Files.ThirdParty.getThirdPartyItem() != null;
        }

        entryObject = jq(entryObject);
        if (!entryObject.is(".file-row")) {
            entryObject = entryObject.closest(".file-row");
        }
        return entryObject.hasClass("third-party-entry");
    };

    var getThirdPartyItem = function (entryData) {
        var thirdPartyKey =
            entryData == null
                ? ASC.Files.Folders.currentFolder.provider_key
                : entryData.provider_key;

        return execThirdPartyItem(thirdPartyKey);
    };

    var execThirdPartyItem = function (thirdPartyKey) {
        switch (thirdPartyKey) {
            case thirdPartyList.BoxNet.key:
                return thirdPartyList.BoxNet;
            case thirdPartyList.DropBox.key:
                return thirdPartyList.DropBox;
            case thirdPartyList.Google.key:
                return thirdPartyList.Google;
            case thirdPartyList.SkyDrive.key:
                return thirdPartyList.SkyDrive;
            default:
                return null;
        }
    };

    var showSettingThirdParty = function () {
        getThirdParty();
        ASC.Files.UI.hideAllContent();
        jq("#treeSetting").addClass("currentCategory open");
        jq(".settings-link-thirdparty").addClass("active");
        jq("#settingThirdPartyPanel").show();

        ASC.Files.UI.setDocumentTitle(ASC.Files.FilesJSResources.TitleSettingsThirdParty);

        if (ASC.Files.Folders.eventAfter != null && typeof ASC.Files.Folders.eventAfter == "function") {
            ASC.Files.Folders.eventAfter();
            ASC.Files.Folders.eventAfter = null;
        }
    };

    var setToken = function (token, source) {
        jq("#thirdPartyGetToken span").show();

        jq("#thirdPartyGetTokenButton").hide();
        jq("#thirdPartyGetToken input:hidden").val(token);
    };

    var showEditorDialog = function (thirdParty) {
        jq("#thirdPartyEditor div[id$=\"InfoPanel\"]").hide();

        jq("#thirdPartyTitle").val(thirdParty.customerTitle);
        ASC.Files.UI.checkCharacter(jq("#thirdPartyTitle"));

        jq("#thirdPartyPanel input").removeAttr("disabled");

        jq("#thirdPartyPass").val("");
        jq("#thirdPartyCorporate").removeAttr("checked");

        jq("#thirdPartyGetToken input:hidden").val("");

        if (thirdParty.getTokenUrl) {
            jq("#thirdPartyNamePass").hide();
            jq("#thirdPartyGetToken").show();

            jq("#thirdPartyGetToken span").hide();

            jq("#thirdPartyGetTokenButton")
                .show()
                .text(ASC.Files.FilesJSResources.ThirdPartyGetToken.format(thirdParty.providerTitle))
                .unbind("click")
                .click(function () {
                    OAuthCallback = ASC.Files.ThirdParty.setToken;
                    OAuthPopup(thirdParty.getTokenUrl);
                    return false;
                });
        } else {
            jq("#thirdPartyNamePass").show();
            jq("#thirdPartyGetToken").hide();
        }

        jq("#thirdPartyEditor .middle-button-container").show();
        jq("#thirdPartyEditor .ajax-info-block").hide();

        jq("#submitThirdParty").unbind("click").click(function () {
            ASC.Files.ThirdParty.submitEditor(thirdParty);
            return false;
        });

        if (thirdParty.id) {
            jq("#thirdPartyNamePass, #thirdPartyGetToken").hide();
            jq("#thirdPartyCorporate").prop("checked", ASC.Files.Folders.folderContainer == "corporate");

            jq("#thirdPartyDialogCaption").text(ASC.Files.FilesJSResources.ThirdPartyEditorCaption.format(thirdParty.providerTitle));
        } else {
            jq("#thirdPartyDialogCaption").text(ASC.Files.FilesJSResources.ThirdPartyEditorCaptionAppend.format(thirdParty.providerTitle));
        }

        ASC.Files.UI.blockUI(jq("#thirdPartyEditor"), 400, 300);
        PopupKeyUpActionProvider.EnterAction = "jq(\"#submitThirdParty\").click();";
        PopupKeyUpActionProvider.CloseDialogAction = "jq(\"#thirdPartyPass\").val(\"\");";

        jq("#thirdPartyTitle")
            .removeClass(
                thirdPartyList.BoxNet.key
                    + " " + thirdPartyList.DropBox.key
                    + " " + thirdPartyList.Google.key
                    + " " + thirdPartyList.SkyDrive.key)
            .addClass(thirdParty.key)
            .focus();
    };

    var submitEditor = function (thirdParty) {
        var folderTitle = jq("#thirdPartyTitle").val().trim();
        folderTitle = ASC.Files.Common.replaceSpecCharacter(folderTitle);
        var login = jq("#thirdPartyName").val().trim();
        var password = jq("#thirdPartyPass").val().trim();
        var token = jq("#thirdPartyGetToken input:hidden").val();
        var corporate = (jq("#thirdPartyCorporate").prop("checked") === true);

        var infoBlock = jq("#thirdPartyEditor div[id$=\"InfoPanel\"]");
        infoBlock.hide();

        if (folderTitle == ""
            || !thirdParty.id
                && (!thirdParty.getTokenUrl && (login == "" || password == ""))) {
            infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_FieldsIsEmpty);
            return;
        }
        if (!thirdParty.id && (thirdParty.getTokenUrl && token == "")) {
            infoBlock.show().find("div").text(ASC.Files.FilesJSResources.ErrorMassage_MustLogin);
            return;
        }

        jq("#thirdPartyPass").val("");

        jq("#thirdPartyPanel input").attr("disabled", "disabled");
        jq("#thirdPartyEditor .middle-button-container").hide();
        jq("#thirdPartyEditor .ajax-info-block").show();

        if (thirdParty.folderId) {
            ASC.Files.UI.blockObjectById("folder", thirdParty.folderId, true, ASC.Files.FilesJSResources.DescriptChangeInfo);
        }

        saveProvider(thirdParty.id, thirdParty.key, folderTitle, login, password, token, corporate, thirdParty.folderId);
    };

    var showChangeDialog = function (folderData) {
        var thirdPartyItem = ASC.Files.ThirdParty.getThirdPartyItem(folderData);
        var thirdParty =
            {
                getTokenUrl: thirdPartyItem.getTokenUrl,
                key: thirdPartyItem.key,
                providerTitle: thirdPartyItem.providerTitle,

                id: folderData.provider_id,
                customerTitle: folderData.title,
                folderId: folderData.id
            };

        ASC.Files.ThirdParty.showEditorDialog(thirdParty);
    };

    var showDeleteDialog = function (providerId, providerKey, providerTitle, customerTitle, folderData) {
        providerId = providerId || (folderData ? folderData.provider_id : null);
        if (providerId == null) {
            return;
        }

        var folderId = folderData ? folderData.entryId : null;

        providerTitle = providerTitle || ASC.Files.ThirdParty.getThirdPartyItem(folderData).providerTitle;
        jq("#thirdPartyDeleteDescr").html(ASC.Files.FilesJSResources.ConfirmDeleteThirdParty.format(customerTitle, providerTitle));

        jq("#deleteThirdParty").unbind("click").click(function () {
            ASC.Files.UI.blockObjectById("folder", folderId, true, ASC.Files.FilesJSResources.DescriptRemove);
            PopupKeyUpActionProvider.CloseDialog();
            ASC.Files.ThirdParty.deleteProvider(providerId, providerKey, customerTitle, folderId);
            return false;
        });

        ASC.Files.UI.blockUI(jq("#thirdPartyDelete"), 400, 300);
        PopupKeyUpActionProvider.EnterAction = "jq(\"#deleteThirdParty\").click();";
    };

    var addNewBoxNetAccount = function () {
        var thirdParty = thirdPartyList.BoxNet;
        var accountPanel = jq("#account_" + thirdParty.key + "_0");
        if (accountPanel.length > 0) {
            return false;
        }
        addNewThirdParty(thirdParty);
    };

    var addNewDropBoxAccount = function () {
        var thirdParty = thirdPartyList.DropBox;
        var accountPanel = jq("#account_" + thirdParty.key + "_0");
        if (accountPanel.length > 0) {
            return false;
        }
        OAuthCallback = function (token, source) {
            addNewThirdParty(thirdParty, token);
        };
        OAuthPopup(thirdParty.getTokenUrl);
        return false;
    };

    var addNewGoogleAccount = function () {
        var thirdParty = thirdPartyList.Google;
        var accountPanel = jq("#account_" + thirdParty.key + "_0");
        if (accountPanel.length > 0) {
            return false;
        }
        OAuthCallback = function (token, source) {
            addNewThirdParty(thirdParty, token);
        };
        OAuthPopup(thirdParty.getTokenUrl);
        return false;
    };

    var addNewSkyDriveAccount = function () {
        var thirdParty = thirdPartyList.SkyDrive;
        var accountPanel = jq("#account_" + thirdParty.key + "_0");
        if (accountPanel.length > 0) {
            return false;
        }
        OAuthCallback = function (token, source) {
            addNewThirdParty(thirdParty, token);
        };
        OAuthPopup(thirdParty.getTokenUrl);
        return false;
    };

    var addNewThirdParty = function (thirdParty, token) {
        var data = {
            corporate: false,
            customer_title: thirdParty.customerTitle,
            provider_id: 0,
            provider_title: thirdParty.providerTitle,
            provider_key: thirdParty.key,
            isNew: true,
            max_name_length: ASC.Files.Constants.MAX_NAME_LENGTH,
            canCorporate: (ASC.Files.Constants.USER_ADMIN && !ASC.Files.Constants.YOUR_DOCS)
        };

        var xmlData = ASC.Files.Common.jsonToXml({ third_partyList: { entry: data } });
        var htmlXml = ASC.Files.TemplateManager.translateFromString(xmlData);
        jq("#thirdPartyAccountList").append(htmlXml);

        jq("#emptyThirdPartyContainer").hide();
        jq("#thirdPartyAccountContainer").show();

        var accountPanel = jq("#account_" + thirdParty.key + "_0");
        ASC.Files.UI.checkCharacter(jq(accountPanel).find(".account-input-folder"));

        if (thirdParty.key == thirdPartyList.BoxNet.key) {
            jq(accountPanel).find(".account-log-pass-container").show();
            jq(accountPanel).find(".account-settings-container").show();
            jq(accountPanel).find(".account-input-login").focus();
        } else {
            jq(accountPanel).find(".account-settings-container").show();
            jq(accountPanel).find(".account-hidden-token").val(token);
            jq(accountPanel).find(".account-input-folder").focus();
        }
        jq(accountPanel).yellowFade();
    };

    var editThirdPartyAccount = function (obj) {
        var account = jq(obj).parents(".account-row");
        ASC.Files.UI.checkCharacter(jq(account).find(".account-input-folder"));
        var customerTitle = jq(account).find(".account-hidden-customer-title").val().trim();
        jq(account).find(".account-input-folder").val(customerTitle);
        jq(account).find(".account-settings-container").show();
    };

    var cancelThirdPartyAccount = function (obj) {
        var account = jq(obj).parents(".account-row");
        var providerId = parseInt(jq(account).find(".account-hidden-provider-id").val());
        if (providerId == 0) {
            jq(account).remove();
            if (jq("#thirdPartyAccountList .account-row").length == 0) {
                jq("#thirdPartyAccountContainer").hide();
                jq("#emptyThirdPartyContainer").show();
            }
            return false;
        }
        jq(account).find(".account-settings-container").hide();
    };

    var deleteThirdPartyAccount = function (obj) {
        var account = jq(obj).parents(".account-row");
        var providerId = parseInt(jq(account).find(".account-hidden-provider-id").val());
        var providerKey = jq(account).find(".account-hidden-provider-key").val().trim();
        var providerTitle = jq(account).find(".account-hidden-provider-title").val().trim();
        var customerTitle = jq(account).find(".account-hidden-customer-title").val().trim();
        ASC.Files.ThirdParty.showDeleteDialog(providerId, providerKey, providerTitle, customerTitle);
    };

    var saveThirdPartyAccount = function (obj) {
        var account = jq(obj).parents(".account-row");
        var providerId;
        var providerIdTmp = parseInt(jq(account).find(".account-hidden-provider-id").val());
        if (providerIdTmp != 0) {
            providerId = providerIdTmp;
        }
        var providerKey = jq(account).find(".account-hidden-provider-key").val().trim();
        var customerTitle = jq(account).find(".account-input-folder").val().trim();
        var login = jq(account).find(".account-input-login").val().trim();
        var password = jq(account).find(".account-input-pass").val().trim();
        var token = jq(account).find(".account-hidden-token").val().trim();
        var corporate = (jq(account).find(".account-cbx-corporate").prop("checked") === true);

        var thirdParty = execThirdPartyItem(providerKey);

        if (customerTitle == "" || !providerId &&
            (!thirdParty.getTokenUrl && (login == "" || password == ""))) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_FieldsIsEmpty, true);
            return;
        }
        if (!providerId && (thirdParty.getTokenUrl && token == "")) {
            ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.ErrorMassage_MustLogin, true);
            return;
        }

        jq(account).find("input").attr("disabled", true);
        jq(account).find("a.button.account-save-link").addClass("disable");

        saveProvider(providerId, providerKey, customerTitle, login, password, token, corporate/*, jq(account).attr("id") bug 14206  */);
    };

    var changeAccessToThirdpartySettings = function (obj) {
        var enable = jq(obj).prop("checked");
        changeAccessToThirdparty(enable === true);
    };

    var showThirdPartyNewAccount = function () {
        ASC.Files.UI.blockUI(jq("#thirdPartyNewAccount"), 500, 300);
    };

    var showThirdPartyActionsPanel = function (event) {
        var e = ASC.Files.Common.fixEvent(event);
        var target = jq(e.srcElement || e.target);

        jq("#accountEditLinkContainer").unbind("click").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            editThirdPartyAccount(target);
        });

        jq("#accountDeleteLinkContainer").unbind("click").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            deleteThirdPartyAccount(target);
        });

        var dropdownItem = jq("#thirdPartyActionPanel");

        dropdownItem.css(
            {
                "top": target.offset().top + target.outerHeight(),
                "left": "auto",
                "right": jq(window).width() - target.offset().left - target.width() - 2,
                "margin": "5px -8px 0 0"
            });

        dropdownItem.toggle();
        return true;
    };

    //request

    var getThirdParty = function () {
        ASC.Files.ServiceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.GetThirdParty,
            { showLoading: true },
            "thirdparty");
    };

    var saveProvider = function (providerId, providerKey, folderTitle, login, password, token, corporate, folderId) {
        var params =
            {
                providerId: providerId,
                providerKey: providerKey,
                folderTitle: folderTitle,
                login: login,
                password: password,
                token: token,
                corporate: corporate,
                folderId: folderId
            };

        var data = {
            third_party: {
                auth_data:
                    {
                        login: login,
                        password: password,
                        token: token
                    },
                corporate: corporate,
                customer_title: folderTitle,
                provider_id: providerId,
                provider_key: providerKey
            }
        };

        ASC.Files.ServiceManager.request("post",
            "json",
            ASC.Files.TemplateManager.events.SaveThirdParty,
            params,
            data,
            "thirdparty", "save");
    };

    var deleteProvider = function (providerId, providerKey, customerTitle, folderId) {
        var params =
            {
                providerId: providerId,
                providerKey: providerKey,
                folderId: folderId,
                customerTitle: customerTitle,
                parentId: ASC.Files.Folders.currentFolder.id
            };

        ASC.Files.ServiceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.DeleteThirdParty,
            params,
            "thirdparty", "delete?providerId=" + providerId);
    };

    var changeAccessToThirdparty = function (enable) {
        ASC.Files.ServiceManager.request("get",
            "json",
            ASC.Files.TemplateManager.events.ChangeAccessToThirdparty,
            {},
            "thirdparty?enable=" + (enable === true));
    };

    //event handler

    var onGetThirdParty = function (jsonData, params, errorMessage) {
        LoadingBanner.hideLoading();
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        if (jsonData.length > 0) {
            for (var i = 0; i < jsonData.length; i++) {
                var thirdPartyItem = execThirdPartyItem(jsonData[i].provider_key);
                jsonData[i].provider_key = thirdPartyItem.key;
                jsonData[i].provider_title = thirdPartyItem.providerTitle;
                jsonData[i].isNew = false;
                jsonData[i].max_name_length = ASC.Files.Constants.MAX_NAME_LENGTH;
                jsonData[i].canCorporate = (ASC.Files.Constants.USER_ADMIN && !ASC.Files.Constants.YOUR_DOCS);
            }
            var jsonResult = { third_partyList: { entry: jsonData } };
            var xmlData = ASC.Files.Common.jsonToXml(jsonResult);
            var htmlXML = ASC.Files.TemplateManager.translateFromString(xmlData);
            jq("#thirdPartyAccountList").html(htmlXML);
            jq("#emptyThirdPartyContainer").hide();
            jq("#thirdPartyAccountContainer").show();
        } else {
            jq("#thirdPartyAccountList .account-row").remove();
            jq("#thirdPartyAccountContainer").hide();
            jq("#emptyThirdPartyContainer").show();
        }
    };

    var onSaveThirdParty = function (jsonData, params, errorMessage) {
        var infoBlock = jq("#thirdPartyEditor div[id$=\"InfoPanel\"]");

        jq("#thirdPartyPass").val("");

        if (typeof errorMessage != "undefined") {
            if (jq("#settingThirdPartyPanel:visible").length > 0) {
                ASC.Files.UI.displayInfoPanel(errorMessage, true);

                var panel = (typeof params.folderId != "undefined" ?
                    jq("#" + params.folderId) :
                    jq("#account_" + params.providerKey + "_" + (params.providerId || 0)));

                panel.find("input").removeAttr("disabled");
                panel.find("a.button.account-save-link").removeClass("disable");
            } else {
                infoBlock.show().find("div").text(errorMessage);
                jq("#thirdPartyPanel input").removeAttr("disabled");
                jq("#thirdPartyEditor .middle-button-container").show();
                jq("#thirdPartyEditor .ajax-info-block").hide();
            }
            return;
        }

        PopupKeyUpActionProvider.CloseDialog();

        var folderTitle = params.folderTitle;

        var folderObj = null;
        var folderId = params.folderId;

        if (jq("#settingThirdPartyPanel:visible").length > 0) {
            params.providerId = params.providerId ? params.providerId : 0;
            var data = {
                corporate: params.corporate,
                customer_title: jsonData.title,
                provider_id: jsonData.provider_id,
                provider_title: execThirdPartyItem(jsonData.provider_key).providerTitle,
                provider_key: jsonData.provider_key,
                isNew: false,
                max_name_length: ASC.Files.Constants.MAX_NAME_LENGTH,
                canCorporate: (ASC.Files.Constants.USER_ADMIN && !ASC.Files.Constants.YOUR_DOCS)
            };

            var xmlData = ASC.Files.Common.jsonToXml({ third_partyList: { entry: data } });
            var htmlXml = ASC.Files.TemplateManager.translateFromString(xmlData);
            var accountPanel = jq("#account_" + params.providerKey + "_" + params.providerId);

            if (accountPanel.length) {
                jq(accountPanel).replaceWith(htmlXml);
            } else {
                jq("#thirdPartyAccountList").append(htmlXml);

                jq("#emptyThirdPartyContainer").hide();
                jq("#thirdPartyAccountContainer").show();
            }
        } else {
            var folderPlaceId = params.corporate ? ASC.Files.Constants.FOLDER_ID_COMMON_FILES : ASC.Files.Constants.FOLDER_ID_MY_FILES;
            if (folderPlaceId == ASC.Files.Folders.currentFolder.id) {
                var stringData = ASC.Files.Common.jsonToXml({ folder: jsonData });
                var htmlXML = ASC.Files.TemplateManager.translateFromString(stringData);

                ASC.Files.EmptyScreen.hideEmptyScreen();

                folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
                if (folderObj) {
                    folderObj.replaceWith(htmlXML);
                    folderObj = ASC.Files.UI.getEntryObject("folder", folderId);
                } else {
                    jq("#filesMainContent").prepend(htmlXML);
                    folderObj = jq("#filesMainContent .new-folder:first");
                }

                folderObj.yellowFade().removeClass("new-folder");

                ASC.Files.UI.resetSelectAll();

                folderTitle = ASC.Files.UI.getObjectTitle(folderObj);

            } else {
                if (folderId) {
                    ASC.Files.UI.getEntryObject("folder", folderId).remove();
                    ASC.Files.UI.checkEmptyContent();
                }
            }

            if (folderObj && folderObj.is(":visible")) {
                ASC.Files.UI.addRowHandlers(folderObj);
            }
        }

        ASC.Files.Tree.resetNode(ASC.Files.Constants.FOLDER_ID_COMMON_FILES);
        ASC.Files.Tree.resetNode(ASC.Files.Constants.FOLDER_ID_MY_FILES);

        ASC.Files.UI.displayInfoPanel(
            folderId
                ? ASC.Files.FilesJSResources.InfoChangeThirdParty.format(folderTitle)
                : ASC.Files.FilesJSResources.InfoSaveThirdParty.format(folderTitle,
                    params.corporate ? ASC.Files.FilesJSResources.CorporateFiles : ASC.Files.FilesJSResources.MyFiles));

        if (!params.providerId) {
            ASC.Files.Anchor.navigationSet(jsonData.id);
        }
    };

    var onDeleteThirdParty = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        var providerId = params.providerId;
        var providerKey = params.providerKey;
        var parentId = params.parentId;
        var folderId = params.folderId;
        var folderTitle = params.customerTitle;
        var folderObj = ASC.Files.UI.getEntryObject("folder", folderId);

        if (folderObj != null) {
            folderObj.remove();

            ASC.Files.UI.checkEmptyContent();
        }

        var accountPanel = jq("#account_" + providerKey + "_" + providerId);
        if (accountPanel.length > 0) {
            jq(accountPanel).remove();
            if (jq("#thirdPartyAccountList .account-row").length == 0) {
                jq("#thirdPartyAccountContainer").hide();
                jq("#emptyThirdPartyContainer").show();
            }
        }

        ASC.Files.Tree.resetNode(parentId);

        ASC.Files.UI.displayInfoPanel(ASC.Files.FilesJSResources.InfoRemoveThirdParty.format(folderTitle));
    };

    var onChangeAccessToThirdparty = function (jsonData, params, errorMessage) {
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        jq("#cbxEnableSettings").prop("checked", jsonData === true);
    };

    return {
        init: init,
        thirdPartyList: thirdPartyList,

        isThirdParty: isThirdParty,
        getThirdPartyItem: getThirdPartyItem,

        setToken: setToken,

        showEditorDialog: showEditorDialog,
        showChangeDialog: showChangeDialog,
        showDeleteDialog: showDeleteDialog,

        addNewBoxNetAccount: addNewBoxNetAccount,
        addNewDropBoxAccount: addNewDropBoxAccount,
        addNewGoogleAccount: addNewGoogleAccount,
        addNewSkyDriveAccount: addNewSkyDriveAccount,
        editThirdPartyAccount: editThirdPartyAccount,
        cancelThirdPartyAccount: cancelThirdPartyAccount,
        deleteThirdPartyAccount: deleteThirdPartyAccount,
        saveThirdPartyAccount: saveThirdPartyAccount,

        submitEditor: submitEditor,
        deleteProvider: deleteProvider,

        showSettingThirdParty: showSettingThirdParty,
        changeAccessToThirdpartySettings: changeAccessToThirdpartySettings,
        showThirdPartyNewAccount: showThirdPartyNewAccount,

        showThirdPartyActionsPanel: showThirdPartyActionsPanel
    };
})();

(function ($) {
    ASC.Files.ThirdParty.init();

    $(function () {
        jq("#thirdPartyEditor div[id$=\"InfoPanel\"]")
            .removeClass("infoPanel")
            .addClass("errorBox")
            .css("margin", "10px 16px 0");

        ASC.Files.ThirdParty.thirdPartyList.BoxNet.customerTitle = ASC.Files.FilesJSResources.FolderTitleBoxNet;
        ASC.Files.ThirdParty.thirdPartyList.BoxNet.providerTitle = ASC.Files.FilesJSResources.TypeTitleBoxNet;

        ASC.Files.ThirdParty.thirdPartyList.DropBox.customerTitle = ASC.Files.FilesJSResources.FolderTitleDropBox;
        ASC.Files.ThirdParty.thirdPartyList.DropBox.providerTitle = ASC.Files.FilesJSResources.TypeTitleDropBox;
        ASC.Files.ThirdParty.thirdPartyList.DropBox.getTokenUrl = ASC.Files.Constants.URL_OAUTH_DROPBOX;

        ASC.Files.ThirdParty.thirdPartyList.Google.customerTitle = ASC.Files.FilesJSResources.FolderTitleGoogle;
        ASC.Files.ThirdParty.thirdPartyList.Google.providerTitle = ASC.Files.FilesJSResources.TypeTitleGoogle;
        ASC.Files.ThirdParty.thirdPartyList.Google.getTokenUrl = ASC.Files.Constants.URL_OAUTH_GOOGLE;

        ASC.Files.ThirdParty.thirdPartyList.SkyDrive.customerTitle = ASC.Files.FilesJSResources.FolderTitleSkyDrive;
        ASC.Files.ThirdParty.thirdPartyList.SkyDrive.providerTitle = ASC.Files.FilesJSResources.TypeTitleSkyDrive;
        ASC.Files.ThirdParty.thirdPartyList.SkyDrive.getTokenUrl = ASC.Files.Constants.URL_OAUTH_SKYDRIVE;

        jq("#addThirdpartyBox").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showEditorDialog(ASC.Files.ThirdParty.thirdPartyList.BoxNet);
            return false;
        });

        jq("#addThirdpartyDropBox").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showEditorDialog(ASC.Files.ThirdParty.thirdPartyList.DropBox);
            return false;
        });

        jq("#addThirdpartyGoogle").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showEditorDialog(ASC.Files.ThirdParty.thirdPartyList.Google);
            return false;
        });

        jq("#addThirdpartySkydrive").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showEditorDialog(ASC.Files.ThirdParty.thirdPartyList.SkyDrive);
            return false;
        });

        jq("#thirdpartyBoxNet, .empty-container-add-account.BoxNet").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            PopupKeyUpActionProvider.CloseDialog();
            if (jq("#settingThirdPartyPanel:visible").length == 0) {
                ASC.Files.Folders.eventAfter = ASC.Files.ThirdParty.addNewBoxNetAccount;
                ASC.Files.Anchor.move("setting=thirdparty");
                return false;
            }
            ASC.Files.ThirdParty.addNewBoxNetAccount();
            return false;
        });

        jq("#thirdpartyDropBox, .empty-container-add-account.DropBox").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            PopupKeyUpActionProvider.CloseDialog();
            if (jq("#settingThirdPartyPanel:visible").length == 0) {
                ASC.Files.Folders.eventAfter = ASC.Files.ThirdParty.addNewDropBoxAccount;
                ASC.Files.Anchor.move("setting=thirdparty");
                return false;
            }
            ASC.Files.ThirdParty.addNewDropBoxAccount();
            return false;
        });

        jq("#thirdpartyGoogle, .empty-container-add-account.Google").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            PopupKeyUpActionProvider.CloseDialog();
            if (jq("#settingThirdPartyPanel:visible").length == 0) {
                ASC.Files.Folders.eventAfter = ASC.Files.ThirdParty.addNewGoogleAccount;
                ASC.Files.Anchor.move("setting=thirdparty");
                return false;
            }
            ASC.Files.ThirdParty.addNewGoogleAccount();
            return false;
        });

        jq("#thirdpartySkyDrive, .empty-container-add-account.SkyDrive").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            PopupKeyUpActionProvider.CloseDialog();
            if (jq("#settingThirdPartyPanel:visible").length == 0) {
                ASC.Files.Folders.eventAfter = ASC.Files.ThirdParty.addNewSkyDriveAccount;
                ASC.Files.Anchor.move("setting=thirdparty");
                return false;
            }
            ASC.Files.ThirdParty.addNewSkyDriveAccount();
            return false;
        });

        jq("#thirdPartyAccountList").on("click", "a.account-cancel-link", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.cancelThirdPartyAccount(this);
            return false;
        });

        jq("#thirdPartyAccountList").on("click", "a.account-save-link:not(.disable)", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.saveThirdPartyAccount(this);
            return false;
        });

        jq("#cbxEnableSettings").on("change", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.changeAccessToThirdpartySettings(this);
            return false;
        });

        jq("#thirdPartyConnectAccount").on("click", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.ThirdParty.showThirdPartyNewAccount();
            return false;
        });

        jq("#thirdPartyAccountList").on("click", ".menu-small", ASC.Files.ThirdParty.showThirdPartyActionsPanel);

        jq("#thirdPartyAccountList").on("keyup", ".account-input-login, .account-input-pass, .account-input-folder", function (event) {
            if (!e) {
                var e = event;
            }
            e = ASC.Files.Common.fixEvent(e);
            var code = e.keyCode || e.which;

            if (code == ASC.Files.Common.keyCode.enter) {
                var accountRow = jq(this).closest(".account-row");
                if (jq(this).is(".account-input-login")) {
                    accountRow.find(".account-input-pass").focus();
                } else if (jq(this).is(".account-input-pass")) {
                    accountRow.find(".account-input-folder").focus();
                } else {
                    accountRow.find(".account-save-link").click();
                }
            }
        });

    });
})(jQuery);