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

if (typeof ASC === "undefined") {
    ASC = {};
}

if (typeof ASC.CRM === "undefined") {
    ASC.CRM = (function() { return {} })();
}

ASC.CRM.ListContactView = (function() {
    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListContactView.cookieKey && ASC.CRM.ListContactView.cookieKey != "") {
            var cookie = {
                    page: page,
                    countOnPage: countOnPage
                },
                path = '/',
                parts = location.pathname.split('/');
            parts.splice(parts.length - 1, 1);
            path = parts.join('/');

            jq.cookies.set(ASC.CRM.ListContactView.cookieKey, cookie, { path: path });
        }
    };

    var _lockMainActions = function() {
        jq("#contactsHeaderMenu .menuActionDelete").removeClass("unlockAction").unbind("click");
        jq("#contactsHeaderMenu .menuActionAddTag").removeClass("unlockAction").unbind("click");
        jq("#contactsHeaderMenu .menuActionAddTask").removeClass("unlockAction").unbind("click");
        jq("#contactsHeaderMenu .menuActionPermissions").removeClass("unlockAction").unbind("click");
        jq("#contactsHeaderMenu .menuActionSendEmail").removeClass("unlockAction").unbind("click");
    };

    var _checkForLockMainActions = function() {
        if (ASC.CRM.ListContactView.selectedItems.length === 0) {
            _lockMainActions();
            return;
        }

        if (!jq("#contactsHeaderMenu .menuActionDelete").hasClass("unlockAction")) {
            jq("#contactsHeaderMenu .menuActionDelete").addClass("unlockAction").unbind("click").bind("click", function() {
                _showDeletePanel();
            });
        }
        if (!jq("#contactsHeaderMenu .menuActionAddTag").hasClass("unlockAction")) {
            jq("#contactsHeaderMenu .menuActionAddTag").addClass("unlockAction");
        }
        if (!jq("#contactsHeaderMenu .menuActionPermissions").hasClass("unlockAction")) {
            jq("#contactsHeaderMenu .menuActionPermissions").addClass("unlockAction").unbind("click").bind("click", function() {
                _showSetPermissionsPanel({ isBatch: true });
            });
        }
        if (!jq("#contactsHeaderMenu .menuActionAddTask").hasClass("unlockAction")) {
            jq("#contactsHeaderMenu .menuActionAddTask").addClass("unlockAction").unbind("click").bind("click", function () {
                ASC.CRM.ListContactView.showTaskPanel(ASC.CRM.ListContactView.selectedItems, false);
            });
        }

        var unlockSendEmail = false;
        for (var i = 0, len = ASC.CRM.ListContactView.selectedItems.length; i < len; i++) {
            if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                unlockSendEmail = true;
            }
        }

        if (unlockSendEmail) {
            jq("#contactsHeaderMenu .menuActionSendEmail").addClass("unlockAction").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.showSendEmailDialog();
            });
        } else {
            jq("#contactsHeaderMenu .menuActionSendEmail").removeClass("unlockAction").unbind("click");
        }
    };


    var _initPageNavigatorControl = function (countOfRows, visiblePageCount, currentPageNumber) {
        window.contactPageNavigator = new ASC.Controls.PageNavigator.init("contactPageNavigator", "#divForContactPager", countOfRows, visiblePageCount, currentPageNumber,
                                                                        ASC.CRM.Resources.CRMJSResource.Previous, ASC.CRM.Resources.CRMJSResource.Next);

        contactPageNavigator.changePageCallback = function(page) {
            _setCookie(page, contactPageNavigator.EntryCountOnPage);

            var startIndex = contactPageNavigator.EntryCountOnPage * (page - 1);
            ASC.CRM.ListContactView.renderContent(startIndex);
        };
    };

    var _initContactActionMenu = function() {

        jq.dropdownToggle({
            dropdownID: "contactActionMenu",
            switcherSelector: "#companyTable .entity-menu",
            addTop: -2,
            addLeft: 10,
            rightPos: true,
            beforeShowFunction: function (switcherObj, dropdownItem) {
                var contactId = switcherObj.attr("id").split('_')[1];
                if (!contactId) {
                    return;
                }
                _showActionMenu(parseInt(contactId));
            },
            showFunction: function(switcherObj, dropdownItem) {
                jq("#companyTable .entity-menu.active").removeClass("active");
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass("active");
                }
            },
            hideFunction: function() {
                jq("#companyTable .entity-menu.active").removeClass("active");
            }
        });


        jq("#companyTable").unbind("contextmenu").bind("contextmenu", function(event) {
            event.preventDefault();

            var e = ASC.CRM.Common.fixEvent(event),
                target = jq(e.srcElement || e.target),
                contactId = parseInt(target.closest("tr.with-entity-menu").attr("id").split('_')[1]);
            if (!contactId) {
                return false;
            }
            _showActionMenu(contactId);
            jq("#companyTable .entity-menu.active").removeClass("active");

            var $dropdownItem = jq("#contactActionMenu");
            $dropdownItem.show();
            var left = $dropdownItem.children(".corner-top").position().left;
            $dropdownItem.hide();

            if (target.is(".entity-menu")) {
                if ($dropdownItem.is(":hidden")) {
                    target.addClass("active");
                }
                $dropdownItem.css({
                    "top": target.offset().top + target.outerHeight() - 2,
                    "left": target.offset().left - left + 7,
                    "right": "auto"
                });
            } else {
                $dropdownItem.css({
                    "top": e.pageY + 3,
                    "left": e.pageX - left - 5,
                    "right": "auto"
                });
            }
            $dropdownItem.show();
            return true;
        });
    };

    var _initOtherActionMenu = function() {
        if (jq("#exportListToCSV").length == 1) {
            jq("#exportListToCSV").bind("click", exportToCsv);
        }
        if (jq("#openListInEditor").length == 1) {
            jq("#openListInEditor").bind("click", openExportFile);
        }
        jq("#menuCreateNewTask").bind("click", function() {

            ASC.CRM.ListContactView.showTaskPanel(null, true);
        });
    };

    var _initScrolledGroupMenu = function() {
        ScrolledGroupMenu.init({
            menuSelector: "#contactsHeaderMenu",
            menuAnchorSelector: "#mainSelectAll",
            menuSpacerSelector: "#companyListBox .header-menu-spacer",
            userFuncInTop: function() { jq("#contactsHeaderMenu .menu-action-on-top").hide(); },
            userFuncNotInTop: function() { jq("#contactsHeaderMenu .menu-action-on-top").show(); }
        });
    };

    var _renderContactPageNavigator = function(startIndex) {
        var tmpTotal;
        if (startIndex >= ASC.CRM.ListContactView.Total) {
            tmpTotal = startIndex + 1;
        } else {
            tmpTotal = ASC.CRM.ListContactView.Total;
        }
        contactPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListContactView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);
        jq("#tableForContactNavigation").show();
    };

    var _renderSimpleContactPageNavigator = function() {
        jq("#contactsHeaderMenu .menu-action-simple-pagenav").html("");
        var $simplePN = jq("<div></div>"),
            lengthOfLinks = 0;
        if (jq("#divForContactPager .pagerPrevButtonCSSClass").length != 0) {
            lengthOfLinks++;
            jq("#divForContactPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
        }
        if (jq("#divForContactPager .pagerNextButtonCSSClass").length != 0) {
            lengthOfLinks++;
            if (lengthOfLinks === 2) {
                jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
            }
            jq("#divForContactPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
        }
        if ($simplePN.children().length != 0) {
            $simplePN.appendTo("#contactsHeaderMenu .menu-action-simple-pagenav");
            jq("#contactsHeaderMenu .menu-action-simple-pagenav").show();
        } else {
            jq("#contactsHeaderMenu .menu-action-simple-pagenav").hide();
        }
    };

    var _renderCheckedContactsCount = function(count) {
        if (count != 0) {
            jq("#contactsHeaderMenu .menu-action-checked-count > span").text(jq.format(ASC.CRM.Resources.CRMJSResource.ElementsSelectedCount, count));
            jq("#contactsHeaderMenu .menu-action-checked-count").show();
        } else {
            jq("#contactsHeaderMenu .menu-action-checked-count > span").text("");
            jq("#contactsHeaderMenu .menu-action-checked-count").hide();
        }
    };

    var _renderNoContactsEmptyScreen = function() {
        jq("#companyTable tbody tr").remove();
        jq("#mainContactList").hide();

        ASC.CRM.Common.hideExportButtons();
        jq("#emptyContentForContactsFilter").hide();
        jq("#contactsEmptyScreen").show();
    };

    var _renderNoContactsForQueryEmptyScreen = function() {
        jq("#companyTable tbody tr").remove();
        jq("#companyListBox").hide();
        jq("#tableForContactNavigation").hide();
        jq("#mainSelectAll").attr("disabled", true);

        ASC.CRM.Common.hideExportButtons();
        jq("#contactsEmptyScreen").hide();
        jq("#emptyContentForContactsFilter").show();
    };

    var _showActionMenu = function(contactID) {
        var contact = null;

        for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
            if (contactID == ASC.CRM.ListContactView.fullContactList[i].id) {
                contact = ASC.CRM.ListContactView.fullContactList[i];
                break;
            }
        }
        if (contact == null) return;

        jq("#contactActionMenu .addTaskLink").unbind("click").bind("click", function() {
            jq("#contactActionMenu").hide();
            jq("#taskActionMenu").hide();
            jq("#companyTable .entity-menu.active").removeClass("active");

            ASC.CRM.ListContactView.showTaskPanel(contact, false);
        });

        jq("#contactActionMenu .addDealLink").attr("href", jq.format("deals.aspx?action=manage&contactID={0}", contactID));
        jq("#contactActionMenu .addCaseLink").attr("href", jq.format("cases.aspx?action=manage&contactID={0}", contactID));

        jq("#contactActionMenu .addPhoneLink").text(contact.primaryPhone != null ? ASC.CRM.Resources.CRMJSResource.EditPhone : ASC.CRM.Resources.CRMJSResource.AddNewPhone);
        jq("#contactActionMenu .addPhoneLink").unbind("click").bind("click", function() {
            jq("#contactActionMenu").hide();
            jq("#companyTable .entity-menu.active").removeClass("active");
            _showAddPrimaryPhoneInput(contactID, contact.primaryPhone);
        });

        jq("#contactActionMenu .addEmailLink").text(contact.primaryEmail != null ? ASC.CRM.Resources.CRMJSResource.EditEmail : ASC.CRM.Resources.CRMJSResource.AddNewEmail);
        jq("#contactActionMenu .addEmailLink").unbind("click").bind("click", function() {
            jq("#contactActionMenu").hide();
            jq("#companyTable .entity-menu.active").removeClass("active");
            _showAddPrimaryEmailInput(contactID, contact.primaryEmail);
        });

        if (contact.primaryEmail != null && contact.primaryEmail.emailHref != "") {
            jq("#contactActionMenu .sendEmailLink").attr("href", contact.primaryEmail.emailHref);
            jq("#contactActionMenu .sendEmailLink").removeClass("display-none");
        } else {
            jq("#contactActionMenu .sendEmailLink").addClass("display-none");
        }


        jq("#contactActionMenu .editContactLink").attr("href",
                    jq.format("default.aspx?id={0}&action=manage{1}", contactID, !contact.isCompany ? "&type=people" : ""));

        jq("#contactActionMenu .deleteContactLink").unbind("click").bind("click", function() {
            jq("#contactActionMenu").hide();
            jq("#companyTable .entity-menu.active").removeClass("active");
            ASC.CRM.ListContactView.showConfirmationPanelForDelete(contact.displayName, contact.id, contact.isCompany, true);
        });

        jq("#contactActionMenu .showProfileLink").attr("href",
                    jq.format("default.aspx?id={0}{1}", contactID, !contact.isCompany ? "&type=people" : ""));

        //if (ASC.CRM.ListContactView.currentUserIsAdmin || Teamlab.profile.id == contact.createdBy.id) {
        //    jq("#contactActionMenu .setPermissionsLink").show();
        //    jq("#contactActionMenu .setPermissionsLink").unbind("click").bind("click", function() {
        //        jq("#contactActionMenu").hide();
        //        jq("#companyTable .entity-menu.active").removeClass("active");

        //        ASC.CRM.ListContactView.deselectAll();

        //        ASC.CRM.ListContactView.selectedItems.push(createShortContact(contact));
        //        _showSetPermissionsPanel({ isBatch: false });
        //    });
        //} else {
        //    jq("#contactActionMenu .setPermissionsLink").hide();
        //}
    };

    var _showAddPrimaryPhoneInput = function(contactId, primaryPhone) {
        var $phoneInput = jq("#addPrimaryPhone_" + contactId);
        $phoneInput.css("borderColor", "");

        var $phoneElement = jq("#contactItem_" + contactId).find(".primaryPhone");
        if ($phoneElement.length != 0) {
            $phoneElement.remove();
            if (primaryPhone != null) {
                $phoneInput.val(primaryPhone.data);
            }
        } else {
            $phoneInput.val("");
        }
        $phoneInput.show().focus();
        jq.forcePhoneSymbolsOnly($phoneInput);

        $phoneInput.unbind("blur").bind("blur", function () {
            $phoneInput.unbind("blur");
            var text = $phoneInput.val().trim();
            if (text.length == 0) {
                if (primaryPhone == null) {
                    $phoneInput.val("").hide();
                    return;
                } else {
                    _deletePrimaryPhone(contactId, text, primaryPhone);
                }
            } else {
                _addPrimaryPhone(contactId, text, primaryPhone);
            }
        });

        $phoneInput.unbind("keyup").bind("keyup", function(event) {
            if (ASC.CRM.ListContactView.isEnterKeyPressed(event)) {
                var text = $phoneInput.val().trim();
                if (text.length == 0) {
                    if (primaryPhone == null) {
                        $phoneInput.val("").hide();
                        return;
                    } else {
                        _deletePrimaryPhone(contactId, text, primaryPhone);
                    }
                } else {
                    $phoneInput.unbind("blur");
                    _addPrimaryPhone(contactId, text, primaryPhone);
                }
            }
        });
    };

    var _addPrimaryPhone = function(contactId, phoneNumber, oldPrimaryPhone) {
        //            var reg = new RegExp(/(^\+)?(\d+)/);
        //            if (val == "" || !reg.test(val)) {

        if (oldPrimaryPhone == null) {
            var params = { contactId: contactId },
                data = {
                    data: phoneNumber,
                    isPrimary: true,
                    infoType: "Phone",
                    category: "Work"
                };

            Teamlab.addCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_phone,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        } else {
            var params = { contactId: contactId },
                data = {
                    id: oldPrimaryPhone.id,
                    data: phoneNumber,
                    isPrimary: true,
                    infoType: "Phone"
                };

            Teamlab.updateCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_phone,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
    };

    var _deletePrimaryPhone = function(contactId, phoneNumber, oldPrimaryPhone) {
        var params = { contactId: contactId };
        Teamlab.deleteCrmContactInfo(params, contactId, oldPrimaryPhone.id,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.delete_primary_phone,
                before: function(params) {
                    jq("#check_contact_" + params.contactId).hide();
                    jq("#loaderImg_" + params.contactId).show();
                },
                after: function(params) {
                    jq("#check_contact_" + params.contactId).show();
                    jq("#loaderImg_" + params.contactId).hide();
                }
            });
    };

    var _showAddPrimaryEmailInput = function(contactId, primaryEmail) {
        var $emailInput = jq("#addPrimaryEmail_" + contactId);
        $emailInput.css("borderColor", "");

        var $emailElement = jq("#contactItem_" + contactId).find(".primaryEmail");
        if ($emailElement.length != 0) {
            $emailElement.remove();
            if (primaryEmail != null) {
                $emailInput.val(primaryEmail.data);
            }
        } else {
            $emailInput.val("");
        }

        $emailInput.show().focus();

        $emailInput.unbind("blur").bind("blur", function() {
            var text = $emailInput.val().trim();

            if (text.length == 0) {
                if (primaryEmail == null) {
                    $emailInput.unbind("blur");
                    $emailInput.val("").hide();
                    return false;
                } else {
                    $emailInput.unbind("blur");
                    _deletePrimaryEmail(contactId, text, primaryEmail);
                }
            } else {
                if (ASC.CRM.ListContactView.isEmailValid(text)) {
                    $emailInput.unbind("blur");
                    _addPrimaryEmail(contactId, text, primaryEmail);
                } else {
                    $emailInput.css("borderColor", "#CC0000");
                    return false;
                }
            }
        });

        $emailInput.unbind("keyup").bind("keyup", function(event) {
            if (ASC.CRM.ListContactView.isEnterKeyPressed(event)) {
                var text = $emailInput.val().trim();
                if (text.length == 0) {
                    if (primaryEmail == null) {
                        $emailInput.val("").hide();
                        return false;
                    } else {
                        _deletePrimaryEmail(contactId, text, primaryEmail);
                    }
                } else {
                    if (ASC.CRM.ListContactView.isEmailValid(text)) {
                        $emailInput.unbind("blur");
                        _addPrimaryEmail(contactId, text, primaryEmail);
                    } else {
                        $emailInput.css("borderColor", "#CC0000");
                        return false;
                    }
                }
            }
        });
    };

    var _addPrimaryEmail = function(contactId, email, oldPrimaryEmail) {
        if (oldPrimaryEmail == null) {
            var params = { contactId: contactId },
                data = {
                    data: email,
                    isPrimary: true,
                    infoType: "Email",
                    category: "Work"
                };

            Teamlab.addCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_email,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        } else {
            var params = { contactId: contactId },
                data = {
                    id: oldPrimaryEmail.id,
                    data: email,
                    isPrimary: true,
                    infoType: "Email"
                };

            Teamlab.updateCrmContactInfo(params, contactId, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.add_primary_email,
                before: function(params) { jq("#check_contact_" + params.contactId).hide(); jq("#loaderImg_" + params.contactId).show(); },
                after: function(params) { jq("#check_contact_" + params.contactId).show(); jq("#loaderImg_" + params.contactId).hide(); }
            });
        }
    };

    var _deletePrimaryEmail = function(contactId, email, oldPrimaryEmail) {
        var params = { contactId: contactId };
        Teamlab.deleteCrmContactInfo(params, contactId, oldPrimaryEmail.id,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.delete_primary_email,
                before: function(params) {
                    jq("#check_contact_" + params.contactId).hide();
                    jq("#loaderImg_" + params.contactId).show();
                },
                after: function(params) {
                    jq("#check_contact_" + params.contactId).show();
                    jq("#loaderImg_" + params.contactId).hide();
                }
            });
    };

    var _getFilterSettings = function() {
        var settings = {
            sortBy: "displayName",
            sortOrder: "ascending",
            tags: []
        };

        if (ASC.CRM.ListContactView.advansedFilter.advansedFilter == null) return settings;

        var param = ASC.CRM.ListContactView.advansedFilter.advansedFilter();

        jq(param).each(function(i, item) {
            switch (item.id) {
                case "sorter":
                    settings.sortBy = item.params.id;
                    settings.sortOrder = item.params.dsc == true ? "descending" : "ascending";
                    break;
                case "text":
                    settings.filterValue = item.params.value;
                    break;
                case "fromToDate":
                    settings.fromDate = new Date(item.params.from);
                    settings.toDate = new Date(item.params.to);
                    break;
                default:
                    if (item.hasOwnProperty("apiparamname") && item.params.hasOwnProperty("value") && item.params.value != null) {
                        try {
                            var apiparamnames = jq.parseJSON(item.apiparamname),
                                apiparamvalues = jq.parseJSON(item.params.value);
                            if (apiparamnames.length != apiparamvalues.length) {
                                settings[item.apiparamname] = item.params.value;
                            }
                            for (var i = 0, len = apiparamnames.length; i < len; i++) {
                                settings[apiparamnames[i]] = apiparamvalues[i];
                            }
                        } catch (err) {
                            settings[item.apiparamname] = item.params.value;
                        }
                    }
                    break;
            }
        });
        if (!settings.hasOwnProperty("contactStage")) {
            settings.contactStage = "-1";
        }
        if (!settings.hasOwnProperty("contactType")) {
            settings.contactType = "-1";
        }
        return settings;
    };

    var _contactItemFactory = function(contact, selectedIDs) {
        var index = jq.inArray(contact.id, selectedIDs);
        contact.isChecked = index != -1;

        contact.primaryPhone = null;
        contact.primaryEmail = null;
        //contact.nearTask = null;

        for (var j = 0, n = contact.commonData.length; j < n; j++) {
            if (contact.commonData[j].isPrimary) {
                if (contact.commonData[j].infoType == 0) {
                    contact.primaryPhone = {
                        data: contact.commonData[j].data,
                        id: contact.commonData[j].id
                    };
                }
                if (contact.commonData[j].infoType == 1) {
                    contact.primaryEmail = {
                        data: contact.commonData[j].data,
                        id: contact.commonData[j].id,
                        emailHref: _getEmailHref(contact.id)
                    };
                }
            }
        }
    };

    var _getEmailHref = function (contactID) {
        if (typeof (ASC.CRM.ListContactView.basePathMail) == "undefined"){
            ASC.CRM.ListContactView.basePathMail = ASC.CRM.Common.getMailModuleBasePath();
        } 
        return [
                ASC.CRM.ListContactView.basePathMail,
                "#composeto/crm=",
                contactID
                ].join('');
    };

    var _renderTagElement = function(tag) {
        var $tagElem = jq("<a></a>").addClass("dropdown-item")
                        .text(ASC.CRM.Common.convertText(tag.title,false))
                        .bind("click", function() {
                            _addThisTag(this);
                        });
        jq("#addTagDialog ul.dropdown-content").append(jq("<li></li>").append($tagElem));
    };

    var _renderAndInitTagsDialog = function() {
        for (var i = 0, n = ASC.CRM.Data.contactTags.length; i < n; i++) {
            _renderTagElement(ASC.CRM.Data.contactTags[i]);
        }

        jq.dropdownToggle({
            dropdownID: "addTagDialog",
            switcherSelector: "#contactsHeaderMenu .menuActionAddTag.unlockAction",
            addTop: 5,
            addLeft: 0,
            showFunction: function(switcherObj, dropdownItem) {
                jq("#addTagDialog input.textEdit").val("");
            }
        });
    };

    var _initConfirmationPannels = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "deletePanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: ASC.CRM.Resources.CRMCommonResource.ConfirmationDeleteText,
            innerHtmlText:
            ["<div id=\"deleteList\" class=\"containerForListBatchDelete mobile-overflow\">",
                "<dl>",
                    "<dt class=\"listForBatchDelete confirmRemoveCompanies\">",
                        ASC.CRM.Resources.CRMContactResource.Companies,
                        ":",
                    "</dt>",
                    "<dd class=\"listForBatchDelete confirmRemoveCompanies\">",
                    "</dd>",
                    "<dt class=\"listForBatchDelete confirmRemovePersons\">",
                        ASC.CRM.Resources.CRMContactResource.Persons,
                        ":",
                    "</dt>",
                    "<dd class=\"listForBatchDelete confirmRemovePersons\">",
                    "</dd>",
                "</dl>",
            "</div>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMContactResource.DeletingContacts
        }).insertAfter("#mainContactList");

        jq("#deletePanel").on("click", ".crm-actionButtonsBlock .button.blue", function () {
            ASC.CRM.ListContactView.deleteBatchContacts();
        });


        jq.tmpl("blockUIPanelTemplate", {
            id: "setPermissionsPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.SetPermissions,
            innerHtmlText: "",
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            OKBtnClass: "setPermissionsLink",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMCommonResource.SaveChangesProggress
        }).insertAfter("#mainContactList");

        jq("#permissionsContactsPanelInnerHtml").insertBefore("#setPermissionsPanel .containerBodyBlock .crm-actionButtonsBlock").removeClass("display-none");
    };

    var _initSMTPSettingsForm = function (headerTest, questionText) {

        jq.tmpl("blockUIPanelTemplate", {
            id: "smtpSettingsPanel",
            headerTest: headerTest,
            questionText: questionText,
            innerHtmlText: "<div id=\"smtpSettingsContent\"></div>",
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMContactResource.SavingChangesProgress
        }).insertAfter("#mainContactList");

        jq.tmpl("SMTPSettingsFormTemplate", null).appendTo("#smtpSettingsContent");
        jq("#smtpSettingsContent").on("change", "#cbxAuthentication", function () {
            ASC.CRM.ListContactView.changeAuthentication();
        });

        jq("#smtpSettingsPanel").on("click", ".button.blue.middle", function () {
            ASC.CRM.ListContactView.saveSMTPSettings();
        });
    };

    var _initSendEmailDialogs = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "createLinkPanel",
            headerTest: ASC.CRM.Resources.CRMContactResource.GenerateLinks,
            questionText: "",
            innerHtmlText: [
                "<div class=\"headerPanel-splitter bold clearFix\">",
                    "<input type=\"checkbox\" id=\"cbxBlind\" style=\"float:left\" />",
                    "<label for=\"cbxBlind\" style=\"float:left;padding: 3px 0 0 4px;\">",
                        ASC.CRM.Resources.CRMContactResource.BlindLinkInfoText,
                    "</label>",
                "</div>",
                "<div class=\"describe-text headerPanel-splitter\">",
                    ASC.CRM.Resources.CRMContactResource.BatchSizeInfoText,
                "</div>",
                "<div class=\"headerPanel-splitter\">",
                    "<b style=\"padding-right:5px;\">",
                        ASC.CRM.Resources.CRMContactResource.BatchSize,
                    "</b>",
                    "<input maxlength=\"10\" class=\"textEdit\" id=\"tbxBatchSize\" style=\"width:100px;\" />",
                "</div>",
                "<div id=\"linkList\" style=\"display:none;\"></div>"
                ].join(''),
            OKBtn: ASC.CRM.Resources.CRMContactResource.Generate,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMContactResource.Generation
        }).insertAfter("#mainContactList");

    };

    var _addThisTag = function(obj) {
        var params = {
            tagName: jq(obj).text(),
            isNewTag: false
        };
        _addTag(params);
    };

    var _addTag = function(params) {
        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
        }
        params.contactIDs = selectedIDs;

        Teamlab.addCrmTag(params, "contact", params.contactIDs, params.tagName,
        {
            success: ASC.CRM.ListContactView.CallbackMethods.add_tag,
            before: function(par) {
                for (var i = 0, n = par.contactIDs.length; i < n; i++) {
                    jq("#check_contact_" + par.contactIDs[i]).hide();
                    jq("#loaderImg_" + par.contactIDs[i]).show();
                }
            },
            after: function(par) {
                for (var i = 0, n = par.contactIDs.length; i < n; i++) {
                    jq("#check_contact_" + par.contactIDs[i]).show();
                    jq("#loaderImg_" + par.contactIDs[i]).hide();
                }
            }
        });
    };

    var _showSetPermissionsPanel = function(params) {
        if (jq("#setPermissionsPanel div.tintMedium").length > 0) {
            jq("#setPermissionsPanel div.tintMedium span.header-base").remove();
            jq("#setPermissionsPanel div.tintMedium").removeClass("tintMedium").css("padding", "0px");
        }
        jq("#isPrivate").prop("checked", false);
        ASC.CRM.PrivatePanel.changeIsPrivateCheckBox();
        jq("#selectedUsers div.selectedUser[id^=selectedUser_]").remove();
        SelectedUsers.IDs = new Array();
        jq("#setPermissionsPanel .crm-actionButtonsBlock").show();
        jq("#setPermissionsPanel .crm-actionProcessInfoBlock").hide();
        jq("#setPermissionsPanel .setPermissionsLink").unbind("click").bind("click", function() {
            _setPermissions(params);
        });
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#setPermissionsPanel", 600, 500, 0);
    };

    var _setPermissions = function(params) {
        var selectedUsers = SelectedUsers.IDs;
        selectedUsers.push(SelectedUsers.CurrentUserID);

        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
        }

        var data = {
            contactid: selectedIDs,
            isPrivate: jq("#isPrivate").is(":checked"),
            accessList: selectedUsers
        };

        Teamlab.updateCrmContactRights(params, data,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.update_contact_rights,
                before: function() { jq("#setPermissionsPanel .crm-actionButtonsBlock").hide(); jq("#setPermissionsPanel .crm-actionProcessInfoBlock").show(); },
                after: function() { jq("#setPermissionsPanel .crm-actionProcessInfoBlock").hide(); jq("#setPermissionsPanel .crm-actionButtonsBlock").show(); }
            });
    };

    var _showDeletePanel = function() {
        var showCompaniesPanel = false,
            showPersonsPanel = false;
        jq("#deleteList dd.confirmRemoveCompanies, #deleteList dd.confirmRemovePersons").html("");
        for (var i = 0, len = ASC.CRM.ListContactView.selectedItems.length; i < len; i++) {
            if (ASC.CRM.ListContactView.selectedItems[i].isCompany) {
                showCompaniesPanel = true;
                var label = jq("<label></label>").attr("title", ASC.CRM.ListContactView.selectedItems[i].displayName)
                                            .text(ASC.CRM.ListContactView.selectedItems[i].displayName);
                jq("#deleteList dd.confirmRemoveCompanies").append(
                            label.prepend(jq("<input>")
                            .attr("type", "checkbox")
                            .prop("checked", true)
                            .attr("id", "company_" + ASC.CRM.ListContactView.selectedItems[i].id))
                        );
            } else {
                showPersonsPanel = true;
                var label = jq("<label></label>")
                            .attr("title", ASC.CRM.ListContactView.selectedItems[i].displayName)
                            .text(ASC.CRM.ListContactView.selectedItems[i].displayName);
                jq("#deleteList dd.confirmRemovePersons").append(
                            label.prepend(jq("<input>")
                            .attr("type", "checkbox")
                            .prop("checked", true)
                            .attr("id", "person_" + ASC.CRM.ListContactView.selectedItems[i].id))
                        );
            }
        }

        if (showCompaniesPanel) {
            jq("#deleteList dt.confirmRemoveCompanies, #deleteList dd.confirmRemoveCompanies").show();
        } else {
            jq("#deleteList dt.confirmRemoveCompanies, #deleteList dd.confirmRemoveCompanies").hide();
        }
        if (showPersonsPanel) {
            jq("#deleteList dt.confirmRemovePersons, #deleteList dd.confirmRemovePersons").show();
        } else {
            jq("#deleteList dt.confirmRemovePersons, #deleteList dd.confirmRemovePersons").hide();
        }
        jq("#deletePanel .crm-actionButtonsBlock").show();
        jq("#deletePanel .crm-actionProcessInfoBlock").hide();
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#deletePanel", 500, 500, 0);
    };

    var createShortContact = function(contact) {
        var shortContact = {
            id: contact.id,
            isCompany: contact.isCompany,
            primaryEmail: contact.primaryEmail,
            displayName: contact.displayName,
            smallFotoUrl: contact.smallFotoUrl
        };
        return shortContact;
    };

    var exportToCsv = function() {
        var index = window.location.href.indexOf('#'),
            basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href,
            anchor = index >= 0 ? window.location.href.substr(index, window.location.href.length) : "";
        jq("#otherActions").hide();
        window.location.href = basePath + "?action=export" + anchor;
    };

    var openExportFile = function() {
        var index = window.location.href.indexOf('#'),
            basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
        jq("#otherActions").hide();
        window.open(basePath + "?action=export&view=editor");
    };

    var _initSimpleContactActionMenu = function () {
        jq.dropdownToggle({
            dropdownID: "simpleContactActionMenu",
            switcherSelector: "#contactTable .entity-menu",
            addTop: -2,
            addLeft: 10,
            rightPos: true,
            beforeShowFunction: function (switcherObj, dropdownItem) {
                var contactId = switcherObj.attr("id").split('_')[1];
                if (!contactId) {
                    return;
                }
                _showSimpleActionMenu(contactId, switcherObj.attr("data-displayName"), switcherObj.attr("data-email"));
            },
            showFunction: function (switcherObj, dropdownItem) {
                jq("#contactTable .entity-menu.active").removeClass("active");
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass("active");
                }
            },
            hideFunction: function () {
                jq("#contactTable .entity-menu.active").removeClass("active");
            }
        });
    };

    var _showSimpleActionMenu = function (contactID, displayName, email) {

        jq("#simpleContactActionMenu .unlinkContact").unbind("click").bind("click", function () {
            ASC.CRM.ListContactView.removeMember(contactID);
        });

        if (typeof (ASC.CRM.ListContactView.basePathMail) == "undefined") {
            ASC.CRM.ListContactView.basePathMail = ASC.CRM.Common.getMailModuleBasePath();
        }
        var pathCreateEmail = "",
            pathSortEmails = "";

        if (email != "") {
            pathCreateEmail = [
                ASC.CRM.ListContactView.basePathMail,
                "#composeto/crm=",
                contactID,
             ].join('');

            pathSortEmails = [
                ASC.CRM.ListContactView.basePathMail,
                "#inbox/",
                "from=",
                email,
                "/sortorder=descending/"
            ].join('');

            jq("#simpleContactActionMenu .writeEmail").attr("href", pathCreateEmail);
            jq("#simpleContactActionMenu .viewMailingHistory").attr("href", pathSortEmails).removeClass("display-none");
        } else {
            pathCreateEmail = [
               ASC.CRM.ListContactView.basePathMail,
               "#composeto/"
            ].join('');

            jq("#simpleContactActionMenu .writeEmail").attr("href", pathCreateEmail);
            jq("#simpleContactActionMenu .viewMailingHistory").addClass("display-none");
        }
    };

    var _add_new_task_render = function (task) {
        if (typeof (task.contact) === "object" && task.contact != null) {
            var currentContact = null;
            for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                if (task.contact.id == ASC.CRM.ListContactView.fullContactList[i].id) {
                    currentContact = ASC.CRM.ListContactView.fullContactList[i];
                    break;
                }
            }
            if (currentContact != null) {
                currentContact.nearTask = task;

                for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.selectedItems[i].id == currentContact.id) {
                        currentContact.isChecked = true;
                        break;
                    }
                }

                jq("#contactItem_" + task.contact.id).replaceWith(jq.tmpl("contactTmpl", currentContact));

                ASC.CRM.Common.tooltip("#taskTitle_" + task.id, "tooltip", true);
            }
        }
    };
    
    var _preInitPage = function (entryCountOnPage) {
        jq("#mainSelectAll").prop("checked", false);//'cause checkboxes save their state between refreshing the page
        ASC.CRM.ListContactView.selectAll(jq("#mainSelectAll"));

        jq('#tableForContactNavigation select')
            .val(entryCountOnPage)
            .change(function () {
                ASC.CRM.ListContactView.changeCountOfRows(this.value);
            })
            .tlCombobox();
    };
    
    var _initEmptyScreen = function (emptyListImgSrc, emptyFilterListImgSrc) {
        //init emptyScreen for all list
        var buttonHtml = ["<a class='link underline blue plus' href='default.aspx?action=manage'>",
                    ASC.CRM.Resources.CRMContactResource.CreateFirstCompany,
                    "</a><br/>",
                    "<a class='link underline blue plus' href='default.aspx?action=manage&type=people'>",
                    ASC.CRM.Resources.CRMContactResource.CreateFirstPerson,
                    "</a>"].join('');
        
        if (jq.browser.mobile !== true){
            buttonHtml += ["<br/><a class='crm-importLink' href='default.aspx?action=import'>",
                            ASC.CRM.Resources.CRMContactResource.ImportContacts,
                            "</a>"].join('');
        }

        jq.tmpl("emptyScrTmpl",
            {
                ID: "contactsEmptyScreen",
                ImgSrc: emptyListImgSrc,
                Header: ASC.CRM.Resources.CRMContactResource.EmptyContactListHeader,
                Describe: jq.format(ASC.CRM.Resources.CRMContactResource.EmptyContactListDescription,
                    //types
                    "<span class='hintTypes baseLinkAction' >", "</span>",
                    //csv
                    "<span class='hintCsv baseLinkAction' >", "</span>"),
                ButtonHTML: buttonHtml
            }).insertAfter("#mainContactList");

        //init emptyScreen for filter
        jq.tmpl("emptyScrTmpl",
            {
                ID: "emptyContentForContactsFilter",
                ImgSrc: emptyFilterListImgSrc,
                Header: ASC.CRM.Resources.CRMContactResource.EmptyContactListFilterHeader,
                Describe: ASC.CRM.Resources.CRMContactResource.EmptyContactListFilterDescribe,
                ButtonHTML: ["<a class='crm-clearFilterButton' href='javascript:void(0);' ",
                    "onclick='ASC.CRM.ListContactView.advansedFilter.advansedFilter(null);'>",
                    ASC.CRM.Resources.CRMCommonResource.ClearFilter,
                    "</a>"
                ].join('')
            }).insertAfter("#mainContactList");
    };
    
    var _initFilter = function () {
        if (!jq("#contactsAdvansedFilter").advansedFilter) return;

        var tmpDate = new Date(),
            today = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0),
            yesterday = new Date(new Date(today).setDate(tmpDate.getDate() - 1)),
            beginningOfThisMonth = new Date(new Date(today).setDate(1)),

            endOfLastMonth = new Date(new Date(beginningOfThisMonth).setDate(beginningOfThisMonth.getDate() - 1)),
            beginningOfLastMonth = new Date(new Date(endOfLastMonth).setDate(1)),


            todayString = Teamlab.serializeTimestamp(today),
            yesterdayString = Teamlab.serializeTimestamp(yesterday),
            beginningOfThisMonthString = Teamlab.serializeTimestamp(beginningOfThisMonth),
            beginningOfLastMonthString = Teamlab.serializeTimestamp(beginningOfLastMonth),
            endOfLastMonthString = Teamlab.serializeTimestamp(endOfLastMonth);

        ASC.CRM.ListContactView.advansedFilter = jq("#contactsAdvansedFilter")
            .advansedFilter({
                anykey      : false,
                hint        : ASC.CRM.Resources.CRMCommonResource.AdvansedFilterInfoText.format(
                            '<b>',
                            '</b>',
                            '<br/><br/><a href="' + ASC.Resources.Master.FilterHelpCenterLink + '" target="_blank">',
                            '</a>'),
                maxfilters  : 3,
                colcount    : 2,
                maxlength   : "100",
                store       : true,
                inhash      : true,
                filters     : [
                                {
                                    type        : "person",
                                    id          : "my",
                                    apiparamname: "responsibleid",
                                    title       : ASC.CRM.Resources.CRMCommonResource.My,
                                    filtertitle : ASC.CRM.Resources.CRMContactResource.FilterByContactManager,
                                    group       : ASC.CRM.Resources.CRMContactResource.ByContactManager,
                                    groupby     : "responsible",
                                    enable      : true,
                                    bydefault   : { id: Teamlab.profile.id, value: Teamlab.profile.id }
                                },
                                {
                                    type        : "person",
                                    id          : "responsibleID",
                                    apiparamname: "responsibleid",
                                    title       : ASC.CRM.Resources.CRMCommonResource.Custom,
                                    filtertitle : ASC.CRM.Resources.CRMContactResource.FilterByContactManager,
                                    group       : ASC.CRM.Resources.CRMContactResource.ByContactManager,
                                    groupby     : "responsible",
                                    enable      : true
                                },
                                {
                                    type        : "combobox",
                                    id          : "lastMonth",
                                    apiparamname: jq.toJSON(["fromDate", "toDate"]),
                                    title       : ASC.CRM.Resources.CRMCommonResource.LastMonth,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                    group       : ASC.CRM.Resources.CRMCommonResource.FilterByCreationDate,
                                    groupby     : "byDate",
                                    options     :
                                            [
                                            { value: jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.LastMonth, def: true },
                                            { value: jq.toJSON([yesterdayString, yesterdayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Yesterday },
                                            { value: jq.toJSON([todayString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Today },
                                            { value: jq.toJSON([beginningOfThisMonthString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.ThisMonth }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "yesterday",
                                    apiparamname: jq.toJSON(["fromDate", "toDate"]),
                                    title       : ASC.CRM.Resources.CRMCommonResource.Yesterday,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                    group       : ASC.CRM.Resources.CRMCommonResource.FilterByCreationDate,
                                    groupby     : "byDate",
                                    options     :
                                            [
                                            { value: jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.LastMonth },
                                            { value: jq.toJSON([yesterdayString, yesterdayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Yesterday, def: true },
                                            { value: jq.toJSON([todayString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Today },
                                            { value: jq.toJSON([beginningOfThisMonthString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.ThisMonth }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "today",
                                    apiparamname: jq.toJSON(["fromDate", "toDate"]),
                                    title       : ASC.CRM.Resources.CRMCommonResource.Today,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                    group       : ASC.CRM.Resources.CRMCommonResource.FilterByCreationDate,
                                    groupby     : "byDate",
                                    options     :
                                            [
                                            { value: jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.LastMonth },
                                            { value: jq.toJSON([yesterdayString, yesterdayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Yesterday },
                                            { value: jq.toJSON([todayString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Today, def: true },
                                            { value: jq.toJSON([beginningOfThisMonthString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.ThisMonth }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "thisMonth",
                                    apiparamname: jq.toJSON(["fromDate", "toDate"]),
                                    title       : ASC.CRM.Resources.CRMCommonResource.ThisMonth,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                    group       : ASC.CRM.Resources.CRMCommonResource.FilterByCreationDate,
                                    groupby     : "byDate",
                                    options     :
                                            [
                                            { value: jq.toJSON([beginningOfLastMonthString, endOfLastMonthString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.LastMonth },
                                            { value: jq.toJSON([yesterdayString, yesterdayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Yesterday },
                                            { value: jq.toJSON([todayString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.Today },
                                            { value: jq.toJSON([beginningOfThisMonthString, todayString]), classname: '', title: ASC.CRM.Resources.CRMCommonResource.ThisMonth, def: true }
                                            ]
                                },
                                {
                                    type        : "daterange",
                                    id          : "fromToDate",
                                    title       : ASC.CRM.Resources.CRMCommonResource.Custom,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                    group       : ASC.CRM.Resources.CRMCommonResource.FilterByCreationDate,
                                    groupby     : "byDate"
                                },

                                {
                                    type        : "combobox",
                                    id          : "company",
                                    apiparamname: "contactListView",
                                    title       : ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Company,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.Show,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Show,
                                    groupby     : "type",
                                    options     :
                                            [
                                            { value: "company", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Company, def: true },
                                            { value: "person", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Person },
                                            { value: "withopportunity", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_WithOpportunity }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "person",
                                    apiparamname: "contactListView",
                                    title       : ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Person,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.Show,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Show,
                                    groupby     : "type",
                                    options     :
                                            [
                                            { value: "company", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Company },
                                            { value: "person", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Person, def: true },
                                            { value: "withopportunity", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_WithOpportunity }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "withopportunity",
                                    apiparamname: "contactListView",
                                    title       : ASC.CRM.Resources.CRMEnumResource.ContactListViewType_WithOpportunity,
                                    filtertitle : ASC.CRM.Resources.CRMCommonResource.Show,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Show,
                                    groupby     : "type",
                                    options     :
                                            [
                                            { value: "company", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Company },
                                            { value: "person", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_Person },
                                            { value: "withopportunity", classname: '', title: ASC.CRM.Resources.CRMEnumResource.ContactListViewType_WithOpportunity, def: true }
                                            ]
                                },
                                {
                                    type        : "combobox",
                                    id          : "contactStage",
                                    apiparamname: "contactStage",
                                    title       : ASC.CRM.Resources.CRMContactResource.AfterStage,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                    options     : ASC.CRM.Data.contactStages,
                                    defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                    enable      : ASC.CRM.Data.contactStages.length > 0
                                },
                                {
                                    type        : "combobox",
                                    id          : "contactType",
                                    apiparamname: "contactType",
                                    title       : ASC.CRM.Resources.CRMContactResource.ContactType,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                    options     : ASC.CRM.Data.contactTypes,
                                    defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                    enable      : ASC.CRM.Data.contactTypes.length > 0
                                },
                                {
                                    type        : "combobox",
                                    id          : "tags",
                                    apiparamname: "tags",
                                    title       : ASC.CRM.Resources.CRMCommonResource.FilterWithTag,
                                    group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                    options     : ASC.CRM.Data.contactTags,
                                    defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                    multiple    : true,
                                    enable      : ASC.CRM.Data.contactTags.length > 0
                                }
                ],
                sorters: [
                            { id: "displayname", title: ASC.CRM.Resources.CRMCommonResource.Title, dsc: false, visible: true },
                            { id: "firstname", title: ASC.CRM.Resources.CRMContactResource.FirstName, dsc: false, visible: true },
                            { id: "lastname", title: ASC.CRM.Resources.CRMContactResource.LastName, dsc: false, visible: true },
                            { id: "contacttype", title: ASC.CRM.Resources.CRMContactResource.AfterStage, dsc: false },
                            { id: "created", title: ASC.CRM.Resources.CRMCommonResource.CreateDate, dsc: true, def: true }
                ]
            })
            .bind("setfilter", ASC.CRM.ListContactView.changeFilter)
            .bind("resetfilter", ASC.CRM.ListContactView.changeFilter);
    };

    return {
        CallbackMethods:
        {
            get_contacts_by_filter: function(params, contacts) {
                ASC.CRM.ListContactView.Total = params.__total || 0;
                var startIndex = params.__startIndex || 0;
                if (ASC.CRM.ListContactView.Total === 0 &&
                    typeof (ASC.CRM.ListContactView.advansedFilter) != "undefined" &&
                    ASC.CRM.ListContactView.advansedFilter.advansedFilter().length == 1) {
                    ASC.CRM.ListContactView.noContacts = true;
                    ASC.CRM.ListContactView.noContactsForQuery = true;
                } else {
                    ASC.CRM.ListContactView.noContacts = false;
                    if (ASC.CRM.ListContactView.Total === 0) {
                        ASC.CRM.ListContactView.noContactsForQuery = true;
                    } else {
                        ASC.CRM.ListContactView.noContactsForQuery = false;
                    }
                }

                if (ASC.CRM.ListContactView.noContacts) {
                    _renderNoContactsEmptyScreen();
                    LoadingBanner.hideLoading();
                    return false;
                }

                if (ASC.CRM.ListContactView.noContactsForQuery) {
                    _renderNoContactsForQueryEmptyScreen();
                    LoadingBanner.hideLoading();
                    return false;
                }

                jq("#totalContactsOnPage").text(ASC.CRM.ListContactView.Total);

                _renderContactPageNavigator(startIndex);
                _renderSimpleContactPageNavigator();

                if (contacts.length == 0) {//it can happen when select page without elements after deleting
                    jq("#contactsEmptyScreen").hide();
                    jq("#emptyContentForContactsFilter").hide();
                    jq("#companyListBox").show();
                    jq("#companyTable tbody tr").remove();
                    jq("#tableForContactNavigation").show();
                    jq("#mainSelectAll").attr("disabled", true);

                    var startIndex = ASC.CRM.ListContactView.entryCountOnPage * (contactPageNavigator.CurrentPageNumber - 1);
                    while (startIndex >= ASC.CRM.ListContactView.Total && startIndex >= ASC.CRM.ListContactView.entryCountOnPage) {
                        startIndex -= ASC.CRM.ListContactView.entryCountOnPage;
                    }
                    ASC.CRM.ListContactView.renderContent(startIndex);
                    return false;
                }

                jq("#emptyContentForContactsFilter").hide();
                jq("#contactsEmptyScreen").hide();
                jq("#companyListBox").show();
                jq("#mainSelectAll").removeAttr("disabled");
                ASC.CRM.Common.showExportButtons();

                var selectedIDs = new Array();
                for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                    selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
                }

                for (var i = 0, n = contacts.length; i < n; i++) {
                    _contactItemFactory(contacts[i], selectedIDs);
                    ASC.CRM.ListContactView.fullContactList.push(contacts[i]);
                }
                jq("#companyTable tbody").replaceWith(jq.tmpl("contactListTmpl", { contacts: contacts }));

                ASC.CRM.ListContactView.checkFullSelection();

                ASC.CRM.Common.RegisterContactInfoCard();
                ASC.CRM.Common.tooltip(".nearestTask", "tooltip", true);
                window.scrollTo(0, 0);
                ScrolledGroupMenu.fixContentHeaderWidth(jq('#contactsHeaderMenu'));
                LoadingBanner.hideLoading();
            },

            add_primary_phone: function(params, data) {
                jq("#addPrimaryPhone_" + params.contactId).hide();
                jq("<span></span>").attr("title", data.data).attr("class", "primaryPhone").text(data.data).appendTo(jq("#addPrimaryPhone_" + params.contactId).parent());
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId)
                        ASC.CRM.ListContactView.fullContactList[i].primaryPhone = {
                            data: data.data,
                            id: data.id
                        };
                }
            },

            delete_primary_phone: function(params) {
                jq("#addPrimaryPhone_" + params.contactId).val("").hide();
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId) {
                        ASC.CRM.ListContactView.fullContactList[i].primaryPhone = null;
                    }
                }
            },

            add_primary_email: function(params, data) {
                jq("#addPrimaryEmail_" + params.contactId).hide();
                jq("<a></a>").attr("title", data.data)
                    .attr("class", "primaryEmail linkMedium").text(data.data)
                    .attr("href", "mailto:" + data.data).appendTo(jq("#addPrimaryEmail_" + params.contactId).parent());
                jq("#addPrimaryEmailMenu").hide();
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId)
                        ASC.CRM.ListContactView.fullContactList[i].primaryEmail = {
                            data: data.data,
                            id: data.id,
                            emailHref: _getEmailHref(params.contactId)
                        };
                }

                if (jq("#contactItem_" + params.contactId).hasClass("selectedRow")) {
                    for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                        if (ASC.CRM.ListContactView.selectedItems[i].id == params.contactId)
                            ASC.CRM.ListContactView.selectedItems[i].primaryEmail = {
                                data: data.data,
                                id: data.id,
                                emailHref: _getEmailHref(params.contactId)
                            };
                    }
                }
                _checkForLockMainActions();
            },

            delete_primary_email: function(params) {
                jq("#addPrimaryEmail_" + params.contactId).val("").hide();
                for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                    if (ASC.CRM.ListContactView.fullContactList[i].id == params.contactId) {
                        ASC.CRM.ListContactView.fullContactList[i].primaryEmail = null;
                    }
                }
                if (jq("#contactItem_" + params.contactId).hasClass("selectedRow")) {
                    for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                        if (ASC.CRM.ListContactView.selectedItems[i].id == params.contactId) {
                            ASC.CRM.ListContactView.selectedItems[i].primaryEmail = null;
                        }
                    }
                }
                _checkForLockMainActions();
            },

            delete_batch_contacts: function(params, data) {
                var newFullContactList = new Array();
                for (var i = 0, len_i = ASC.CRM.ListContactView.fullContactList.length; i < len_i; i++) {
                    var isDeleted = false;
                    for (var j = 0, len_j = params.contactIDsForDelete.length; j < len_j; j++)
                        if (params.contactIDsForDelete[j] == ASC.CRM.ListContactView.fullContactList[i].id) {
                            isDeleted = true;
                            break;
                        }
                    if (!isDeleted) {
                        newFullContactList.push(ASC.CRM.ListContactView.fullContactList[i]);
                    }

                }
                ASC.CRM.ListContactView.fullContactList = newFullContactList;

                ASC.CRM.ListContactView.Total -= params.contactIDsForDelete.length;
                jq("#totalContactsOnPage").text(ASC.CRM.ListContactView.Total);

                var selectedIDs = new Array();
                for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                    selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
                }

                for (var i = 0, len = params.contactIDsForDelete.length; i < len; i++) {
                    var $objForRemove = jq("#contactItem_" + params.contactIDsForDelete[i]);
                    if ($objForRemove.length != 0) {
                        $objForRemove.remove();
                    }

                    var index = jq.inArray(params.contactIDsForDelete[i], selectedIDs);
                    if (index != -1) {
                        selectedIDs.splice(index, 1);
                        ASC.CRM.ListContactView.selectedItems.splice(index, 1);
                    }
                }
                jq("#mainSelectAll").prop("checked", false);

                _checkForLockMainActions();
                _renderCheckedContactsCount(ASC.CRM.ListContactView.selectedItems.length);

                if (ASC.CRM.ListContactView.Total == 0
                    && (typeof (ASC.CRM.ListContactView.advansedFilter) == "undefined"
                    || ASC.CRM.ListContactView.advansedFilter.advansedFilter().length == 1)) {
                    ASC.CRM.ListContactView.noContacts = true;
                    ASC.CRM.ListContactView.noContactsForQuery = true;
                } else {
                    ASC.CRM.ListContactView.noContacts = false;
                    if (ASC.CRM.ListContactView.Total === 0) {
                        ASC.CRM.ListContactView.noContactsForQuery = true;
                    } else {
                        ASC.CRM.ListContactView.noContactsForQuery = false;
                    }
                }
                PopupKeyUpActionProvider.EnableEsc = true;

                if (ASC.CRM.ListContactView.noContacts) {
                    _renderNoContactsEmptyScreen();
                    jq.unblockUI();
                    return;
                }

                if (ASC.CRM.ListContactView.noContactsForQuery) {
                    _renderNoContactsForQueryEmptyScreen();
                    jq.unblockUI();
                    return;
                }

                if (jq("#companyTable tbody tr").length == 0) {
                    jq.unblockUI();
                    var startIndex = ASC.CRM.ListContactView.entryCountOnPage * (contactPageNavigator.CurrentPageNumber - 1);
                    while (startIndex >= ASC.CRM.ListContactView.Total && startIndex >= ASC.CRM.ListContactView.entryCountOnPage) {
                        startIndex -= ASC.CRM.ListContactView.entryCountOnPage;
                    }
                    ASC.CRM.ListContactView.renderContent(startIndex);
                } else {
                    jq.unblockUI();
                }
            },

            add_tag: function(params, data) {
                jq("#addTagDialog").hide();
                if (params.isNewTag) {
                    var tag = {
                        value: params.tagName,
                        title: params.tagName
                    };
                    ASC.CRM.Data.contactTags.push(tag);
                    _renderTagElement(tag);
                    ASC.CRM.ListContactView.advansedFilter.advansedfilter(
                    {
                        nonetrigger: true,
                        sorters: [],
                        filters: [
                            { id: "tags", type: 'combobox', options: ASC.CRM.Data.contactTags, enable: ASC.CRM.Data.contactTags.length > 0 }
                        ]
                    });
                }
            },

            add_new_task: function(params, task) {
                if (!ASC.CRM.Common.isArray(task)) {
                    _add_new_task_render(task);
                } else {
                    for (var i = 0, n = task.length; i < n; i++) {
                        _add_new_task_render(task[i]);
                    }
                }

                ASC.CRM.Common.RegisterContactInfoCard();
                PopupKeyUpActionProvider.EnableEsc = true;
                jq.unblockUI();
                //taskContactSelector.SelectedContacts = new Array();
            },

            render_simple_content: function(params, contacts) {
                for (var i = 0, n = contacts.length; i < n; i++) {
                    ASC.CRM.Common.contactItemFactory(contacts[i], params);
                }
                jq(window).trigger("getContactsFromApi", [contacts]);
                jq.tmpl("simpleContactTmpl", contacts).prependTo("#contactTable tbody");

                if (typeof (params) != "undefined" && params != null && params.hasOwnProperty("showActionMenu") && params.showActionMenu === true) {
                    jq.tmpl("simpleContactActionMenuTmpl", null).insertAfter("#contactTable");
                    _initSimpleContactActionMenu();
                }

                ASC.CRM.Common.RegisterContactInfoCard();
                LoadingBanner.hideLoading();
            },

            removeMember: function(params, contact) {
                jq("#contactItem_" + params.contactID).remove();
            },

            addMember: function(params, contact) {
                ASC.CRM.Common.contactItemFactory(contact, params);
                jq.tmpl("simpleContactTmpl", contact).prependTo("#contactTable tbody");
                ASC.CRM.Common.RegisterContactInfoCard();
            },

            update_contact_rights: function(params, contacts) {
                for (var i = 0, n = contacts.length; i < n; i++) {
                    for (var j = 0, m = ASC.CRM.ListContactView.fullContactList.length; j < m; j++) {
                        if (contacts[i].id == ASC.CRM.ListContactView.fullContactList[j].id) {
                            var contact_id = contacts[i].id;
                            ASC.CRM.ListContactView.fullContactList[j].isPrivate = contacts[i].isPrivate;
                            jq("#contactItem_" + contact_id).replaceWith(jq.tmpl("contactTmpl", ASC.CRM.ListContactView.fullContactList[j]));
                            if (params.isBatch) {
                                jq("#check_contact_" + contact_id).prop("checked", true);
                            } else {
                                ASC.CRM.ListContactView.selectedItems = [];
                            }

                            if (ASC.CRM.ListContactView.fullContactList[j].nearTask && ASC.CRM.ListContactView.fullContactList[j].nearTask != null) {
                                ASC.CRM.Common.tooltip("#taskTitle_" + ASC.CRM.ListContactView.fullContactList[j].nearTask.id, "tooltip", true);
                            }
                            break;
                        }
                    }
                }
                ASC.CRM.Common.RegisterContactInfoCard();
                PopupKeyUpActionProvider.EnableEsc = true;
                jq.unblockUI();
            }
        },

        fullContactList           : [],
        //isFirstTime: true,
        fromLeftMenu              : false,
        selectedItems             : [],

        currentUserIsAdmin        : false,

        entryCountOnPage          : 0,
        defaultCurrentPageNumber  : 0,
        emailQuotas               : 0,

        noContacts         : false,
        noContactsForQuery : false,
        cookieKey          : "",

        init: function (visiblePageCount,
                        emailQuotas,
                        isAdmin,
                        cookieKey,
                        SMTPSettingsFormHeaderTest,
                        SMTPSettingsFormQuestionTest,
                        emptyListImgSrc,
                        emptyFilterListImgSrc) {

            ASC.CRM.ListContactView.currentUserIsAdmin = isAdmin;
            ASC.CRM.ListContactView.emailQuotas = emailQuotas;
            ASC.CRM.ListContactView.cookieKey = cookieKey;
            ASC.CRM.ListContactView.fromLeftMenu = false;
            ASC.CRM.ListContactView.needToSendApi = true;

            var settings = {
                    page: 1,
                    countOnPage: jq("#tableForContactNavigation select:first>option:first").val()
                },
                key = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + location.pathname + location.search,
                currentAnchor = location.hash,
                cookieKey = encodeURIComponent(key.charAt(key.length - 1) === '/' ? key + 'default.aspx' : key);

            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#'
                ? currentAnchor.substring(1)
                : currentAnchor;

            var cookieAnchor = jq.cookies.get(cookieKey);
            if (currentAnchor == "" || cookieAnchor == currentAnchor) {
                var tmp = ASC.CRM.Common.getPagingParamsFromCookie(ASC.CRM.ListContactView.cookieKey);
                if (tmp != null) {
                    settings = tmp;
                }
            } else {
                _setCookie(settings.page, settings.countOnPage);
            }

            ASC.CRM.ListContactView.entryCountOnPage = settings.countOnPage;
            ASC.CRM.ListContactView.defaultCurrentPageNumber = settings.page;

            _preInitPage(ASC.CRM.ListContactView.entryCountOnPage);

            LoadingBanner.displayLoading();

            _initEmptyScreen(emptyListImgSrc, emptyFilterListImgSrc);

            _initPageNavigatorControl(ASC.CRM.ListContactView.entryCountOnPage, visiblePageCount, ASC.CRM.ListContactView.defaultCurrentPageNumber);

            _initContactActionMenu();

            _renderAndInitTagsDialog();

            _initOtherActionMenu();

            ASC.CRM.ListContactView.initConfirmationPanelForDelete();

            _initConfirmationPannels();

            if (ASC.CRM.ListContactView.currentUserIsAdmin) {
                _initSMTPSettingsForm(SMTPSettingsFormHeaderTest, SMTPSettingsFormQuestionTest);
                _initSendEmailDialogs();
            }
            _initScrolledGroupMenu();

            jq(".companies-menu-item, .persons-menu-item").click(function () {
                ASC.CRM.ListContactView.needToSendApi = true;
                ASC.CRM.ListContactView.fromLeftMenu = true;
            });

            jq(document).click(function(event) {
                jq.dropdownToggle().registerAutoHide(event, "#contactsHeaderMenu .menuActionAddTag", "#addTagDialog");
                jq.dropdownToggle().registerAutoHide(event, "#contactsHeaderMenu .menuActionSendEmail", "#sendEmailDialog");

                jq.dropdownToggle().registerAutoHide(event, "#companyTable .with-entity-menu", "#contactActionMenu", function() {
                    jq("#companyTable .entity-menu.active").removeClass("active");
                });
            });

            _initFilter();

            /*tracking events*/
            ASC.CRM.ListContactView.advansedFilter.one("adv-ready", function () {
                var crmAdvansedFilterContainer = jq("#contactsAdvansedFilter .advansed-filter-list");
                crmAdvansedFilterContainer.find("li[data-id='my'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'me_manager');
                crmAdvansedFilterContainer.find("li[data-id='responsibleID'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'custom_manager');
                crmAdvansedFilterContainer.find("li[data-id='company'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'company');
                crmAdvansedFilterContainer.find("li[data-id='Persons'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'persons');
                crmAdvansedFilterContainer.find("li[data-id='withopportunity'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'with_opportunity');
                crmAdvansedFilterContainer.find("li[data-id='lastMonth'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'last_month');
                crmAdvansedFilterContainer.find("li[data-id='yesterday'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'yesterday');
                crmAdvansedFilterContainer.find("li[data-id='today'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'today');
                crmAdvansedFilterContainer.find("li[data-id='thisMonth'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'this_month');
                crmAdvansedFilterContainer.find("li[data-id='fromToDate'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'from_to_date');
                crmAdvansedFilterContainer.find("li[data-id='contactStage'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'contact_stage');
                crmAdvansedFilterContainer.find("li[data-id='contactType'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'contact_type');
                crmAdvansedFilterContainer.find("li[data-id='tags'] .inner-text").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, 'with_tags');

                jq("#contactsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, "sort");
                jq("#contactsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.contacts, ga_Actions.filterClick, "search_text", "enter");
            });
        },

        onContextMenu: function(event) {
            event.preventDefault();
            return false;
        },

        isEnterKeyPressed: function(event) {
            //Enter key was pressed
            console.log(event);
            return event.keyCode == 13;
        },
        isEmailValid: function(email) {
            var reg = new RegExp(ASC.Resources.Master.EmailRegExpr, "i");
            if (email == "" || !reg.test(email)) { return false; }
            return true;
        },

        filterSortersCorrection: function () {
            var settings = _getFilterSettings();

            if (settings.hasOwnProperty("contactListView") && settings.contactListView === "person") {
                var dsc_created = true,
                    dsc = settings.sortOrder == "descending";

                if (settings.sortBy == "created" ) {
                    dsc_created = dsc;
                }

                if (settings.sortBy == "displayname" ) {
                    settings.sortBy = "created";
                }

                ASC.CRM.ListContactView.advansedFilter.advansedfilter(
                    {
                        nonetrigger: true,
                        filters: [],
                        sorters: [
                                        { id: "displayname", visible: false, selected: false},
                                        { id: "firstname", visible: true, selected: settings.sortBy === "firstname", dsc: settings.sortBy === "firstname" ? dsc : false },
                                        { id: "lastname", visible: true, selected: settings.sortBy === "lastname", dsc: settings.sortBy === "lastname" ? dsc : false },
                                        { id: "contacttype", selected: settings.sortBy === "contacttype", dsc: settings.sortBy === "contacttype" ? dsc : false },
                                        { id: "created", selected: settings.sortBy === "created", dsc: dsc_created }
                        ]
                    });
            } else {
                var dsc_created = true,
                    dsc = settings.sortOrder == "descending";

                if (settings.sortBy == "created") {
                    dsc_created = dsc;
                }

                if (settings.sortBy == "firstname" || settings.sortBy == "lastname") {
                    settings.sortBy = "created";
                }
                ASC.CRM.ListContactView.advansedFilter.advansedfilter(
                    {
                        nonetrigger: true,
                        filters: [],
                        sorters: [
                                        { id: "displayname", visible: true, selected: settings.sortBy === "displayname", dsc: settings.sortBy === "displayname" ? dsc : false },
                                        { id: "firstname", visible: false, selected: false },
                                        { id: "lastname", visible: false, selected: false },
                                        { id: "contacttype", selected: settings.sortBy === "contacttype", dsc: settings.sortBy === "contacttype" ? dsc : false },
                                        { id: "created", selected: settings.sortBy === "created", dsc: dsc_created }
                        ]
                    });
            }
        },

        changeFilter: function () {
            ASC.CRM.ListContactView.needToSendApi = true;

            var defaultStartIndex = 0;
            if (ASC.CRM.ListContactView.defaultCurrentPageNumber != 0) {
                _setCookie(ASC.CRM.ListContactView.defaultCurrentPageNumber, window.contactPageNavigator.EntryCountOnPage);
                defaultStartIndex = (ASC.CRM.ListContactView.defaultCurrentPageNumber - 1) * window.contactPageNavigator.EntryCountOnPage;
                ASC.CRM.ListContactView.defaultCurrentPageNumber = 0;
            } else {
                _setCookie(0, window.contactPageNavigator.EntryCountOnPage);
            }

            ASC.CRM.ListContactView.advansedFilter.one("adv-ready", function () {
                if (ASC.CRM.ListContactView.needToSendApi === true) {
                    ASC.CRM.ListContactView.needToSendApi = false;
                    ASC.CRM.ListContactView.deselectAll();
                    ASC.CRM.ListContactView.renderContent(defaultStartIndex);
                }
            });

            if (ASC.CRM.ListContactView.fromLeftMenu === true) {
                defaultStartIndex = 0;
                _setCookie(0, window.contactPageNavigator.EntryCountOnPage);

                ASC.CRM.ListContactView.fromLeftMenu = false;
                ASC.CRM.ListContactView.filterSortersCorrection();
            } else {
                var param = typeof (arguments[2]) == "object" ? arguments[2] : null;
                if (param != null && param.hasOwnProperty("id") && (param.id == "sorter" || param.id == "text")) {
                    ASC.CRM.ListContactView.needToSendApi = false;
                    ASC.CRM.ListContactView.deselectAll();
                    ASC.CRM.ListContactView.renderContent(defaultStartIndex);
                }
                ASC.CRM.ListContactView.filterSortersCorrection();
            }
        },

        renderContent: function(startIndex) {
            ASC.CRM.ListContactView.fullContactList = new Array();

            LoadingBanner.displayLoading();
            jq("#mainSelectAll").prop("checked", false);

            ASC.CRM.ListContactView.getContacts(startIndex);
        },

        addRecordsToContent: function() {
            if (!ASC.CRM.ListContactView.showMore) { return false; }

            jq("#showMoreContactsButtons .crm-showMoreLink").hide();
            jq("#showMoreContactsButtons .crm-loadingLink").show();

            var startIndex = jq("#companyTable tbody tr").length;

            ASC.CRM.ListContactView.getContacts(startIndex);
        },

        getContacts: function(startIndex) {
            var filter = _getFilterSettings();

            if (typeof startIndex == 'undefined') {
                filter.StartIndex = 0;
            } else {
                filter.StartIndex = startIndex;
            }
            filter.Count = ASC.CRM.ListContactView.entryCountOnPage;

            EventTracker.Track('crm_search_contacts_by_filter');

            Teamlab.getCrmSimpleContacts({}, { filter: filter, success: ASC.CRM.ListContactView.CallbackMethods.get_contacts_by_filter });
        },

        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) { return; }
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListContactView.entryCountOnPage = newCountOfRows;
            contactPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);

            ASC.CRM.ListContactView.renderContent(0);
        },

        selectAll: function(obj) {
            var isChecked = jq(obj).is(":checked"),
                selectedIDs = [];

            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }

            for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                var contact = ASC.CRM.ListContactView.fullContactList[i],
                    index = jq.inArray(contact.id, selectedIDs);
                if (isChecked && index == -1) {
                    ASC.CRM.ListContactView.selectedItems.push(createShortContact(contact));
                    selectedIDs.push(contact.id);
                    jq("#contactItem_" + contact.id).addClass("selectedRow");
                    jq("#check_contact_" + contact.id).prop("checked", true);
                }
                if (!isChecked && index != -1) {
                    ASC.CRM.ListContactView.selectedItems.splice(index, 1);
                    selectedIDs.splice(index, 1);
                    jq("#contactItem_" + contact.id).removeClass("selectedRow");
                    jq("#check_contact_" + contact.id).prop("checked", false);
                }
            }
            _renderCheckedContactsCount(ASC.CRM.ListContactView.selectedItems.length);
            _checkForLockMainActions();
        },

        selectItem: function(obj) {
            var id = parseInt(jq(obj).attr("id").split("_")[2]),
                selectedIDs = [],
                index = 0,
                selectedContact = null;
            for (var i = 0, n = ASC.CRM.ListContactView.fullContactList.length; i < n; i++) {
                if (id == ASC.CRM.ListContactView.fullContactList[i].id) {
                    selectedContact = createShortContact(ASC.CRM.ListContactView.fullContactList[i]);
                }
            }

            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListContactView.selectedItems[i].id);
            }

            index = jq.inArray(id, selectedIDs);

            if (jq(obj).is(":checked")) {
                jq("#contactItem_" + id).addClass("selectedRow");
                if (index == -1) {
                    ASC.CRM.ListContactView.selectedItems.push(selectedContact);
                }
                ASC.CRM.ListContactView.checkFullSelection();
            } else {
                jq("#mainSelectAll").prop("checked", false);
                jq("#contactItem_" + id).removeClass("selectedRow");
                if (index != -1) {
                    ASC.CRM.ListContactView.selectedItems.splice(index, 1);
                }
            }
            _renderCheckedContactsCount(ASC.CRM.ListContactView.selectedItems.length);
            _checkForLockMainActions();
        },

        deselectAll: function() {
            ASC.CRM.ListContactView.selectedItems = new Array();
            jq("#companyTable input:checkbox").prop("checked", false);
            jq("#mainSelectAll").prop("checked", false);
            jq("#companyTable tr.selectedRow").removeClass("selectedRow");
            _renderCheckedContactsCount(0);
            _lockMainActions();
        },

        checkFullSelection: function() {
            var rowsCount = jq("#companyTable tbody tr").length,
                selectedRowsCount = jq("#companyTable input[id^=check_contact_]:checked").length;
            jq("#mainSelectAll").prop("checked", rowsCount == selectedRowsCount);
        },

        deleteBatchContacts: function() {
            var ids = [];
            jq("#deletePanel input:checked").each(function() {
                ids.push(parseInt(jq(this).attr("id").split("_")[1]));
            });
            var params = { contactIDsForDelete: ids };

            Teamlab.removeCrmContact(params, ids,
            {
                success: ASC.CRM.ListContactView.CallbackMethods.delete_batch_contacts,
                before: function() { jq("#deletePanel .crm-actionButtonsBlock").hide(); jq("#deletePanel .crm-actionProcessInfoBlock").show(); },
                after: function() { jq("#deletePanel .crm-actionProcessInfoBlock").hide(); jq("#deletePanel .crm-actionButtonsBlock").show(); }
            });
        },

        initConfirmationPanelForDelete: function () {
            jq.tmpl("blockUIPanelTemplate", {
                id: "confirmationDeleteOneContactPanel",
                headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
                questionText: "",
                innerHtmlText:
                ["<div class=\"confirmationAction\">",
                    "<b></b>",
                "</div>",
                "<div class=\"confirmationNote\">",
                    ASC.CRM.Resources.CRMJSResource.DeleteConfirmNote,
                "</div>"].join(''),
                OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
                CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
                progressText: ASC.CRM.Resources.CRMJSResource.DeleteContactInProgress
            }).appendTo("#studioPageContent .mainPageContent .containerBodyBlock:first");
        },

        showConfirmationPanelForDelete: function(contactName, contactID, isCompany, isListView) {
            if (isCompany == "true" || isCompany == true) {
                jq("#confirmationDeleteOneContactPanel .confirmationAction>b").text(jq.format(ASC.CRM.Resources.CRMJSResource.DeleteCompanyConfirmMessage, Encoder.htmlDecode(contactName)));
            } else {
                jq("#confirmationDeleteOneContactPanel .confirmationAction>b").text(jq.format(ASC.CRM.Resources.CRMJSResource.DeletePersonConfirmMessage, Encoder.htmlDecode(contactName)));
            }
            jq("#confirmationDeleteOneContactPanel .crm-actionButtonsBlock>.button.blue.middle").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.deleteContact(contactID, isListView);
            });
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#confirmationDeleteOneContactPanel", 500, 500, 0);
        },

        deleteContact: function(contactID, isListView) {
            if (isListView === true) {
                var ids = new Array();
                ids.push(contactID);
                var params = { contactIDsForDelete: ids };

                Teamlab.removeCrmContact(params, ids,
                    {
                        success: ASC.CRM.ListContactView.CallbackMethods.delete_batch_contacts,
                        before: function() { jq("#contactActionMenu").hide(); }
                    });
            } else {
                Teamlab.removeCrmContact({}, contactID,
                    {
                        before: function() {
                            jq("#confirmationDeleteOneContactPanel .crm-actionButtonsBlock").hide();
                            jq("#confirmationDeleteOneContactPanel .crm-actionProcessInfoBlock").show();

                            jq("#crm_contactMakerDialog input, #crm_contactMakerDialog select, #crm_contactMakerDialog textarea").attr("disabled", true);
                            jq("#crm_contactMakerDialog .crm-actionProcessInfoBlock span").text(ASC.CRM.Resources.CRMJSResource.DeleteContactInProgress);
                            jq("#crm_contactMakerDialog .crm-actionButtonsBlock").hide();
                            jq("#crm_contactMakerDialog .crm-actionProcessInfoBlock").show();
                        },
                        success: function() {
                            location.href = "default.aspx";
                        }
                    });
            }
        },


        showTaskPanel: function (contact, ShowChangeButton) {

            window.taskContactSelector.ShowChangeButton = ShowChangeButton;
            if (ShowChangeButton == true) {
                jq("#selector_taskContactSelector .crm-editLink").removeClass("display-none");
            } else {
                jq("#selector_taskContactSelector .crm-editLink").addClass("display-none");
            }
            ASC.CRM.TaskActionView.showTaskPanel(0, "contact", 0, contact, { success: ASC.CRM.ListContactView.CallbackMethods.add_new_task });
        },

        addNewTag: function() {
            var newTag = jq("#addTagDialog input").val().trim();
            if (newTag == "") { return false; }

            var params = {
                tagName: newTag,
                isNewTag: true
            };
            _addTag(params);
        },

        showSendEmailDialog: function () {
            if (typeof (ASC.CRM.ListContactView.basePathMail) == "undefined") {
                ASC.CRM.ListContactView.basePathMail = ASC.CRM.Common.getMailModuleBasePath();
            }

            var sendMailByTlHref = [
                ASC.CRM.ListContactView.basePathMail,
                "#composeto/crm=",
            ].join('');


            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                    sendMailByTlHref += ASC.CRM.ListContactView.selectedItems[i].id + ",";
                }
            }
            sendMailByTlHref = sendMailByTlHref.substring(0, sendMailByTlHref.length - 1);
            jq("#sendEmailDialog .sendMailByTl").attr("href", sendMailByTlHref);

            jq.dropdownToggle().toggle("#contactsHeaderMenu .menuActionSendEmail", "sendEmailDialog", 5, 0);
        },

        showCreateLinkPanel: function() {
            var selectedEmails = [];
            jq("#sendEmailDialog").hide();
            jq("#createLinkPanel #linkList").html("");
            jq("#cbxBlind").prop("checked", false);
            jq("#tbxBatchSize").val("10");
            jq.forceIntegerOnly("#tbxBatchSize");
            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++) {
                if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                    selectedEmails.push(ASC.CRM.ListContactView.selectedItems[i].primaryEmail.data);
                }
            }

            jq("#createLinkPanel .crm-actionButtonsBlock a:first").unbind("click").bind("click", function() {
                ASC.CRM.ListContactView.createLink(selectedEmails);
            });
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#createLinkPanel", 500, 500, 0);
        },

        createLink: function(emails) {
            jq("#createLinkPanel #linkList").html("");
            jq("#createLinkPanel .crm-actionButtonsBlock").hide();
            jq("#createLinkPanel .crm-actionProcessInfoBlock").show();
            var blindAttr = jq("#cbxBlind").is(":checked") ? "bcc=" : "cc=",
                batchSize = jq("#tbxBatchSize").val().trim() == "" ? 0 : parseInt(jq("#tbxBatchSize").val().trim()),
                href = "mailto:?",
                linkList = new Array(),
                link = href,
                counter = 0,
                info = "";

            if (ASC.CRM.ListContactView.selectedItems.length - emails.length > 0) {
                info = jq.format(ASC.CRM.Resources.CRMJSResource.GenerateLinkInfo,
                jq.format(ASC.CRM.Resources.CRMJSResource.RecipientsWithoutEmail, ASC.CRM.ListContactView.selectedItems.length - emails.length));
            } else {
                info = jq.format(ASC.CRM.Resources.CRMJSResource.GenerateLinkInfo, "");
            }

            jq("#createLinkPanel #linkList").append(jq("<div></div>").text(info).addClass("headerPanel-splitter"));

            if (emails.length == 1) {
                linkList.push(jq.format("mailto:{0}", emails[0]));
            } else if (batchSize == 1) {
                for (var i = 0, n = emails.length; i < n; i++) {
                    linkList.push(jq.format("mailto:{0}", emails[i]));
                }
            } else {
                for (var i = 0, n = emails.length; i < n; i++) {
                    link += blindAttr + emails[i] + "&";
                    counter++;
                    if (batchSize != 0 && counter == batchSize) {
                        counter = 0;
                        linkList.push(link.substring(0, link.length - 1));
                        link = href;
                    }
                    if (i == emails.length - 1 && emails.length % batchSize != 0) {
                        linkList.push(link.substring(0, link.length - 1));
                    }
                }
            }

            for (var i = 0, n = linkList.length; i < n; i++) {
                counter = i + 1;
                jq("#linkList")
                .append(jq("<a></a>").text(ASC.CRM.Resources.CRMJSResource.Batch + " " + counter).attr("href", linkList[i]));
                if (i != linkList.length - 1)
                    jq("#linkList").append(jq("<span><span/>").addClass("splitter").text(","));
            }
            jq("#createLinkPanel .crm-actionButtonsBlock").show();
            jq("#createLinkPanel .crm-actionProcessInfoBlock").hide();
            jq("#linkList").show();
        },

        initSMTPSettingsPanel: function () {
            jq.forceIntegerOnly("#tbxPort");
            jq("#smtpSettingsContent div.errorBox").remove();
            jq("#smtpSettingsContent div.okBox").remove();
            if (ASC.CRM.Data.smtpSettings != null) {
                jq("#tbxHost").val(ASC.CRM.Data.smtpSettings.Host);
                jq("#tbxPort").val(ASC.CRM.Data.smtpSettings.Port);
                jq("#tbxHostLogin").val(ASC.CRM.Data.smtpSettings.HostLogin);
                jq("#tbxHostPassword").val(ASC.CRM.Data.smtpSettings.HostPassword);
                jq("#tbxSenderDisplayName").val(ASC.CRM.Data.smtpSettings.SenderDisplayName);
                jq("#tbxSenderEmailAddress").val(ASC.CRM.Data.smtpSettings.SenderEmailAddress);
                jq("#cbxEnableSSL").prop("checked", ASC.CRM.Data.smtpSettings.EnableSSL);
                if (ASC.CRM.Data.smtpSettings.RequiredHostAuthentication) {
                    jq("#cbxAuthentication").prop("checked", true);
                    jq("#tbxHostLogin").removeAttr("disabled");
                    jq("#tbxHostPassword").removeAttr("disabled");
                } else {
                    jq("#cbxAuthentication").prop("checked", false);
                    jq("#tbxHostLogin").attr("disabled", true);
                    jq("#tbxHostPassword").attr("disabled", true);
                }
            }
        },

        changeAuthentication: function() {
            if (jq("#cbxAuthentication").is(":checked")) {
                jq("#tbxHostLogin").removeAttr("disabled");
                jq("#tbxHostPassword").removeAttr("disabled");
            } else {
                jq("#tbxHostLogin").attr("disabled", true);
                jq("#tbxHostPassword").attr("disabled", true);
            }
        },

        checkSMTPSettings: function() {

            if (ASC.CRM.Data.smtpSettings == null)
                return false;
            if (ASC.CRM.Data.smtpSettings.RequiredHostAuthentication)
                if (ASC.CRM.Data.smtpSettings.Host === "" ||
                ASC.CRM.Data.smtpSettings.Port === "" ||
                    ASC.CRM.Data.smtpSettings.HostLogin === "" ||
                        ASC.CRM.Data.smtpSettings.HostPassword === "" ||
                            ASC.CRM.Data.smtpSettings.SenderDisplayName === "" ||
                                ASC.CRM.Data.smtpSettings.SenderEmailAddress === "")
                return false;

            if (!ASC.CRM.Data.smtpSettings.RequiredHostAuthentication)
                if (ASC.CRM.Data.smtpSettings.Host === "" ||
                ASC.CRM.Data.smtpSettings.Port === "" ||
                    ASC.CRM.Data.smtpSettings.SenderDisplayName === "" ||
                        ASC.CRM.Data.smtpSettings.SenderEmailAddress === "")
                return false;

            return true;
        },

        saveSMTPSettings: function() {
            jq("#smtpSettingsContent div.errorBox").remove();
            jq("#smtpSettingsContent div.okBox").remove();

            var host = jq("#tbxHost").val().trim(),
                port = jq("#tbxPort").val().trim(),
                authentication = jq("#cbxAuthentication").is(":checked"),
                hostLogin = jq("#tbxHostLogin").val().trim(),
                hostPassword = jq("#tbxHostPassword").val().trim(),
                senderDisplayName = jq("#tbxSenderDisplayName").val().trim(),
                senderEmailAddress = jq("#tbxSenderEmailAddress").val().trim(),
                enableSSL = jq("#cbxEnableSSL").is(":checked"),
                isValid = true;

            if (authentication
                && (host === "" || port === "" || hostLogin === "" || hostPassword === "" || senderDisplayName === "" || senderEmailAddress === "")) {
                isValid = false;
            }
            if (!authentication
                && (host === "" || port === "" || senderDisplayName === "" || senderEmailAddress === "")) {
                isValid = false;
            }

            if (!isValid) {
                jq("#smtpSettingsContent").prepend(
                    jq("<div></div>").addClass("errorBox").text(ASC.CRM.Resources.CRMJSResource.EmptyFieldsOfSettings)
                );
                return;
            }

            AjaxPro.CommonSettingsView.SaveSMTPSettings(host, port, authentication, hostLogin, hostPassword, senderDisplayName, senderEmailAddress, enableSSL, function(res) {
                if (res.error != null) {
                    jq("#smtpSettingsContent").prepend(jq("<div></div>").addClass("errorBox").text(res.error.Message));
                    return;
                }
                ASC.CRM.Data.smtpSettings = {};
                ASC.CRM.Data.smtpSettings.Host = host;
                ASC.CRM.Data.smtpSettings.Port = port;
                ASC.CRM.Data.smtpSettings.RequiredHostAuthentication = authentication;
                ASC.CRM.Data.smtpSettings.HostLogin = hostLogin;
                ASC.CRM.Data.smtpSettings.HostPassword = hostPassword;
                ASC.CRM.Data.smtpSettings.SenderDisplayName = senderDisplayName;
                ASC.CRM.Data.smtpSettings.SenderEmailAddress = senderEmailAddress;
                ASC.CRM.Data.smtpSettings.EnableSSL = enableSSL;

                jq("#smtpSettingsContent div.errorBox").remove();
                jq("#smtpSettingsContent").prepend(
                    jq("<div></div>").addClass("okBox").text(ASC.CRM.Resources.CRMJSResource.SettingsUpdated)
                );
                window.location.href = "sender.aspx";
            });
        },

        renderSimpleContent: function (showUnlinkBtn, showActionMenu) {
            if (typeof window.entityData != "undefined" && window.entityData != null && window.entityData.id != 0) {
                LoadingBanner.displayLoading();
                Teamlab.getCrmEntityMembers({
                    showCompanyLink : window.entityData.type != "company",
                    showUnlinkBtn   : showUnlinkBtn,
                    showActionMenu  : showActionMenu
                },
                window.entityData.type, window.entityData.id, { success: ASC.CRM.ListContactView.CallbackMethods.render_simple_content });
            }
        },

        removeMember: function(contactID) {
            Teamlab.removeCrmEntityMember({ contactID: contactID }, window.entityData.type, window.entityData.id, contactID, {
                before: function (params) {
                    if (jq("#trashImg_" + params.contactID).length == 1) {
                        jq("#trashImg_" + params.contactID).hide();
                        jq("#loaderImg_" + params.contactID).show();
                    } else {
                        jq("#simpleContactActionMenu").hide();
                        jq("#contactTable .entity-menu.active").removeClass("active");
                    }
                },
                after: ASC.CRM.ListContactView.CallbackMethods.removeMember
            });
        },

        addMember: function(contactID) {
            var data =
                    {
                        contactid     : contactID,
                        personid      : contactID,
                        caseid        : window.entityData.id,
                        companyid     : window.entityData.id,
                        opportunityid : window.entityData.id

                    };
            Teamlab.addCrmEntityMember({
                showCompanyLink : window.entityData.type != "company",
                showUnlinkBtn   : false,
                showActionMenu  : true
            },
            window.entityData.type, window.entityData.id, contactID, data, { success: ASC.CRM.ListContactView.CallbackMethods.addMember });
        },
        
        showSenderPage: function () {
            var selectedTargets = new Array();

            for (var i = 0, n = ASC.CRM.ListContactView.selectedItems.length; i < n; i++)
                if (ASC.CRM.ListContactView.selectedItems[i].primaryEmail != null) {
                    var target = {};
                    target.primaryEmail = ASC.CRM.ListContactView.selectedItems[i].primaryEmail.data;
                    target.title = ASC.CRM.ListContactView.selectedItems[i].displayName;
                    target.id = ASC.CRM.ListContactView.selectedItems[i].id;
                    selectedTargets.push(target);
                }

            if (selectedTargets.length > ASC.CRM.ListContactView.emailQuotas) {
                alert(jq.format(ASC.CRM.Resources.CRMJSResource.ErrorEmailRecipientsCount, ASC.CRM.ListContactView.emailQuotas));
                return false;
            }

            ASC.CRM.SmtpSender.setItems(selectedTargets);

            jq("#sendEmailDialog").hide();

            if (!ASC.CRM.ListContactView.checkSMTPSettings()) {
                ASC.CRM.ListContactView.initSMTPSettingsPanel();
                PopupKeyUpActionProvider.EnableEsc = false;
                StudioBlockUIManager.blockUI("#smtpSettingsPanel", 500, 500, 0);
                return false;
            } else {
                window.location.href = "sender.aspx";
            }
        }
    };
})();


ASC.CRM.ContactPhotoUploader = (function() {
    return {
        initPhotoUploader: function(parentDialog, photoImg, data) {
            new AjaxUpload('changeLogo', {
                action: 'ajaxupload.ashx?type=ASC.Web.CRM.Classes.ContactPhotoHandler,ASC.Web.CRM',
                autoSubmit: true,
                data: data,
                onSubmit: function(file, ext) {
                    var tmpDirName = "";
                    if (jq("#uploadPhotoPath").length == 1) {
                        tmpDirName = jq("#uploadPhotoPath").val();
                        if (tmpDirName != "") {
                            tmpDirName = tmpDirName.substr(0, tmpDirName.lastIndexOf("/"))
                        }
                    }
                    this.setData(jQuery.extend({ tmpDirName: tmpDirName }, data));
                },
                onChange: function(file, extension) {
                    if (jQuery.inArray("." + extension, window.imgExst) == -1) {
                        jq("#divLoadPhotoFromPC .fileUploadDscr").hide();
                        jq("#divLoadPhotoFromPC .fileUploadError").text(ASC.CRM.Resources.CRMJSResource.ErrorMessage_NotImageSupportFormat).show();
                        return false;
                    }
                    jq("#divLoadPhotoFromPC .fileUploadError").hide();
                    jq("#divLoadPhotoFromPC .fileUploadDscr").hide();

                    jq(".under_logo .linkChangePhoto").addClass("disable");
                    LoadingBanner.displayLoading();
                    return true;
                },
                onComplete: function(file, response) {
                    var responseObj = jq.evalJSON(response);
                    if (!responseObj.Success) {
                        jq("#divLoadPhotoFromPC .fileUploadDscr").hide();
                        jq("#divLoadPhotoFromPC .fileUploadError").text(responseObj.Message).show();
                        jq(".under_logo .linkChangePhoto").removeClass("disable");
                        LoadingBanner.hideLoading();
                        return;
                    }
                    jq("#divLoadPhotoFromPC .fileUploadError").hide();
                    jq("#divLoadPhotoFromPC .fileUploadDscr").show();
                    PopupKeyUpActionProvider.CloseDialog();
                    if (jq("#uploadPhotoPath").length == 1) {
                        jq("#uploadPhotoPath").val(responseObj.Data);
                    }

                    var now = new Date();
                    photoImg.attr("src", responseObj.Data + '?' + now.getTime());
                    jq(".under_logo .linkChangePhoto").removeClass("disable");
                    LoadingBanner.hideLoading();
                },
                parentDialog: parentDialog,
                isInPopup: true,
                name: "changeLogo"
            });
        }
    };
})();

ASC.CRM.ContactFullCardView = (function() {

    var initSliderControl = function() {
        if (typeof (window.sliderListItems) != "undefined" && window.sliderListItems != null) {
            var colors = [],
                values = [],
                status = 0;
            values[0] = "";

            for (var i = 0, n = window.sliderListItems.items.length; i < n; i++) {
                colors[i] = window.sliderListItems.items[i].color;
                values[i + 1] = window.sliderListItems.items[i].title;
                if (window.sliderListItems.items[i].id == window.sliderListItems.status) {
                    status = i + 1;
                }
            }

            if (jq('#loyaltySliderDetails').length != 0) {
                jq('#loyaltySliderDetails').sliderWithSections({
                    value: status,
                    values: values,
                    max: window.sliderListItems.positionsCount,
                    colors: colors,
                    marginWidth: 1,
                    sliderOptions: {
                        stop: function(event, ui) {
                            if (ui.value != 0) {
                                changeContactStatus(window.sliderListItems.items[ui.value - 1].id);
                            } else {
                                changeContactStatus(0);
                            }
                        }
                    }
                });
            }
        }
    };

    var initChangeContactStatusConfirmationPanel = function (isAdmin) {
        jq.tmpl("blockUIPanelTemplate", {
            id: "changeContactStatusConfirmation",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: ASC.CRM.ContactFullCardView.isCompany ?
                ASC.CRM.Resources.CRMContactResource.ConfirmationChangePersonsStatus :
                ASC.CRM.Resources.CRMContactResource.ConfirmationChangeCompanyStatus,
            innerHtmlText:
            ["<div class=\"noAskingCCSAnymore\">",
                "<input type=\"checkbox\" style=\"float: left;\" id=\"noAskCCSAnymore\"/>",
                "<label style=\"float:left; padding: 2px 0 0 4px;\" for=\"noAskCCSAnymore\">",
                    ASC.CRM.Resources.CRMCommonResource.DontAskAnymore,
                "</label>",
                "<span style=\"height: 20px;margin: 0 0 0 4px;\">",
                    "<div class=\"HelpCenterSwitcher\" ",
                        "onclick=\"jq(this).helper({ BlockHelperID: 'changeContactStatusConfirmation_helpInfo', popup: true});\">",
                    "</div>",
                    "<div class=\"popup_helper\" id=\"changeContactStatusConfirmation_helpInfo\">",
                    isAdmin === true
                    ? ASC.CRM.Resources.CRMContactResource.ContactStatusGroupChangeHelpForAdmin.format(
                            '<a href="settings.aspx?type=contact_stage" target="_blank">',
                            '</a>')
                    : ASC.CRM.Resources.CRMContactResource.ContactStatusGroupChangeHelpForUser,
                    "</div>",
                "</span>",
            "</div>"].join(''),
            OKBtn: ASC.CRM.ContactFullCardView.isCompany ?
                ASC.CRM.Resources.CRMContactResource.OKCompanyStatusGroupChange :
                ASC.CRM.Resources.CRMContactResource.OKPersonStatusGroupChange,
            CancelBtn: ASC.CRM.Resources.CRMContactResource.CancelContactStatusGroupChange,
            progressText: ASC.CRM.Resources.CRMContactResource.LoadingWait
        }).insertAfter("#contactDetailsMenuPanel");

        jq("#changeContactStatusConfirmation").on("click", ".crm-actionButtonsBlock .button.blue", function () {
            if (jq("#noAskCCSAnymore").is(":checked")) {
                Teamlab.updateCRMContactStatusSettings({}, true,
                    function () {
                        ASC.CRM.ContactFullCardView.changeContactStatusGroupAuto = true;
                    });
            }
            jq("#changeContactStatusConfirmation .crm-actionButtonsBlock").hide();
            jq("#changeContactStatusConfirmation .crm-actionProcessInfoBlock").show();

            var statusValue = jq("#changeContactStatusConfirmation").attr("data-statusValue");

            if (ASC.CRM.ContactFullCardView.isCompany === true) {
                Teamlab.updateCrmCompanyContactStatus({}, window.sliderListItems.id,
                        { companyid: window.sliderListItems.id, contactStatusid: statusValue },
                        function () {
                            PopupKeyUpActionProvider.EnableEsc = true;
                            jq.unblockUI();
                        });

            } else {
                Teamlab.updateCrmPersonContactStatus({}, window.sliderListItems.id,
                       { personid: window.sliderListItems.id, contactStatusid: statusValue },
                       function () {
                           PopupKeyUpActionProvider.EnableEsc = true;
                           jq.unblockUI();
                       });
            }
        });

        jq("#changeContactStatusConfirmation").on("click", ".crm-actionButtonsBlock .button.gray", function () {
            if (jq("#noAskCCSAnymore").is(":checked")) {
                Teamlab.updateCRMContactStatusSettings({}, false,
                    function () {
                        ASC.CRM.ContactFullCardView.changeContactStatusGroupAuto = false;
                    });
            }
            jq("#changeContactStatusConfirmation .crm-actionButtonsBlock").hide();
            jq("#changeContactStatusConfirmation .crm-actionProcessInfoBlock").show();

            var statusValue = jq("#changeContactStatusConfirmation").attr("data-statusValue");

            Teamlab.updateCrmContactContactStatus({}, window.sliderListItems.id,
                    { contactid: window.sliderListItems.id, contactStatusid: statusValue },
                    function () {
                        PopupKeyUpActionProvider.EnableEsc = true;
                        jq.unblockUI();
                    });
        });

        jq("#changeContactStatusConfirmation").on("click", ".cancelButton", function () {
            var statusValue = jq("#changeContactStatusConfirmation").attr("data-statusValue");
            Teamlab.updateCrmContactContactStatus({}, window.sliderListItems.id,
                           { contactid: window.sliderListItems.id, contactStatusid: statusValue }, {});
        });

    };

    var initAddTagToContactGroupConfirmationPanel = function (isAdmin) {
        jq.tmpl("blockUIPanelTemplate", {
            id: "addTagToContactGroupConfirmation",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: "",
            innerHtmlText:
            ["<div>",
                (ASC.CRM.ContactFullCardView.isCompany ?
                ASC.CRM.Resources.CRMContactResource.ConfirmationAddTagToCompanyGroup :
                ASC.CRM.Resources.CRMContactResource.ConfirmationAddTagToPersonGroup).replace(/\n/g, "<br/>"),
            "</div>",
            "<div class=\"noAskingATCAnymore clearFix\">",
                "<input type=\"checkbox\" style=\"float: left;\" id=\"noAskingATCAnymore\"/>",
                "<label style=\"float:left; padding: 2px 0 0 4px;\" for=\"noAskingATCAnymore\">",
                    ASC.CRM.Resources.CRMCommonResource.DontAskAnymore,
                "</label>",
                "<span style=\"height: 20px;margin: 0 0 0 4px;\">",
                    "<div class=\"HelpCenterSwitcher\" ",
                        "onclick=\"jq(this).helper({ BlockHelperID: 'addTagToContactGroupConfirmation_helpInfo', popup: true});\">",
                    "</div>",
                    "<div class=\"popup_helper\" id=\"addTagToContactGroupConfirmation_helpInfo\">",
                    isAdmin === true
                    ? ASC.CRM.Resources.CRMContactResource.AddTagToContactGroupHelpForAdmin.format(
                            '<a href="settings.aspx?type=tag" target="_blank">',
                            '</a>')
                    : ASC.CRM.Resources.CRMContactResource.AddTagToContactGroupHelpForUser,
                    "</div>",
                "</span>",
            "</div>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMContactResource.OKAddContactTagToGroup,
            OKBtnClass: "OKAddTagToGroup",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            OtherBtnHtml: ["<a class=\"button gray middle CancelAddTagToGroup\">",
                 ASC.CRM.ContactFullCardView.isCompany ?
                       ASC.CRM.Resources.CRMContactResource.CancelAddContactTagToGroupForCompany :
                       ASC.CRM.Resources.CRMContactResource.CancelAddContactTagToGroupForPerson,
                "</a>"].join(''),
            progressText: ASC.CRM.Resources.CRMContactResource.LoadingWait
        }).insertAfter("#contactDetailsMenuPanel");

        jq("#addTagToContactGroupConfirmation").on("click", ".OKAddTagToGroup", function () {
            if (jq("#noAskingATCAnymore").is(":checked")) {
                Teamlab.updateCRMContactTagSettings({}, true,
                    function () {
                        ASC.CRM.ContactFullCardView.addTagToContactGroupAuto = true;
                    });
            }
            jq("#addTagToContactGroupConfirmation .crm-actionButtonsBlock").hide();
            jq("#addTagToContactGroupConfirmation .crm-actionProcessInfoBlock").show();

            addTagToContactGroup(ASC.CRM.ContactFullCardView.tagParams, ASC.CRM.ContactFullCardView.tagText);
        });

        jq("#addTagToContactGroupConfirmation").on("click", ".CancelAddTagToGroup", function () {
            if (jq("#noAskingATCAnymore").is(":checked")) {
                Teamlab.updateCRMContactTagSettings({}, false,
                    function () {
                        ASC.CRM.ContactFullCardView.addTagToContactGroupAuto = false;
                    });
            }
            jq("#addTagToContactGroupConfirmation .crm-actionButtonsBlock").hide();
            jq("#addTagToContactGroupConfirmation .crm-actionProcessInfoBlock").show();
            addTagToContactOnly(ASC.CRM.ContactFullCardView.tagParams, ASC.CRM.ContactFullCardView.tagText);
        });
    };

    var initMergePanel = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "mergePanel",
            headerTest: ASC.CRM.Resources.CRMContactResource.MergePanelHeaderText,
            questionText: "",
            innerHtmlText:
            ["<div class=\"describe-text\">",
                ASC.CRM.Resources.CRMContactResource.MergePanelDescriptionText,
            "</div>",
            "<ul id=\"listContactsToMerge\">",
            "</ul>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMContactResource.MergePanelButtonStartText,
            OKBtnClass: "OKMergeContacts",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMContactResource.MergePanelProgress
        }).insertAfter("#contactProfile");

        jq("#mergePanel").on("click", ".OKMergeContacts", function () {
            ASC.CRM.ContactFullCardView.mergeContacts();
        });
    };

    var initUploadPhotoPanel = function (contactPhotoFileSizeNote, contactPhotoMedium) {
        jq.tmpl("blockUIPanelTemplate", {
            id: "divLoadPhotoWindow",
            headerTest: ASC.CRM.Resources.CRMContactResource.ChooseProfilePhoto,
            questionText: "",
            innerHtmlText:
            ["<div id=\"divLoadPhotoFromPC\" style=\"margin-top: -20px;\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.LoadPhotoFromPC,
                "</h4>",
                "<div class=\"describe-text\" style=\"margin-bottom: 5px;\">",
                    contactPhotoFileSizeNote,
                "</div>",
                "<div>",
                    "<a id=\"changeLogo\" class=\"button middle gray\">",
                        ASC.CRM.Resources.CRMJSResource.Browse, "...",
                    "</a>",
                    "<span class=\"fileUploadDscr\">",
                        ASC.CRM.Resources.CRMJSResource.NoFileSelected,
                    "</span>",
                    "<span class=\"fileUploadError\"></span>",
                    "<br />",
                "</div>",
            "</div>",
            "<div id=\"divLoadPhotoDefault\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.ChangePhotoToDefault,
                "</h4>",
                "<div id=\"divDefaultImagesHolder\" data-uploadOnly=\"false\">",
                    "<div class=\"ImageHolderOuter\" onclick=\"ASC.CRM.SocialMedia.DeleteContactAvatar();\">",
                        "<img class=\"AvatarImage\" src=",
                        contactPhotoMedium,
                        " alt=\"\" />",
                    "</div>",
                "</div>",
            "</div>",
            "<div id=\"divLoadPhotoFromSocialMedia\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.LoadPhotoFromSocialMedia,
                "</h4>",
                "<div id=\"divImagesHolder\" data-uploadOnly=\"false\">",
                "</div>",
                "<div style=\"clear: both;\">",
                    "<div id=\"divAjaxImageContainerPhotoLoad\">",
                    "</div>",
                "</div>",
            "</div>"].join(''),
            OKBtn: "",
            CancelBtn: "",
            progressText: ""
        }).insertAfter("#contactProfile");
    };

    var contactNetworksFactory = function(item) {
        if (item.infoType == 7) { //Address
            var addressObj = jq.parseJSON(Encoder.htmlDecode(item.data));
            item.data = ASC.CRM.Common.getAddressTextForDisplay(addressObj);
            var query = ASC.CRM.Common.getAddressQueryForMap(addressObj);
            item.href = "http://maps.google.com/maps?q=" + query;
        }
        if (item.infoType == 2) { //Website
            if (item.data.indexOf("://") == -1) {
                item.href = "http://" + item.data;
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 4) { //Twitter
            if (item.data.indexOf("://") == -1) {
                item.href = "https://twitter.com/#!/" + item.data;
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 5) { //LinkedIn
            if (item.data.indexOf("://") == -1) {
                item.href = "http://" + item.data;
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 6) { //Facebook
            if (item.data.indexOf("://") == -1) {
                if (isNaN(item.data)) {
                    item.href = "http://facebook.com/" + item.data;
                } else {
                    item.href = "http://www.facebook.com/#!/profile.php?id=" + item.data;
                }
            }
        }
        if (item.infoType == 8) { //LiveJournal
            if (item.data.indexOf("://") == -1) {
                item.href = "http://" + item.data + ".livejournal.com/";
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 9) { //MySpace
            if (item.data.indexOf("://") == -1) {
                item.href = "http://myspace.com/" + item.data;
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 11) { //Blogger
            if (item.data.indexOf("://") == -1) {
                item.href = "http://" + item.data + ".blogspot.com/";
            } else {
                item.href = item.data;
            }
        }
        if (item.infoType == 14) { //ICQ
            item.href = "http://www.icq.com/people/" + item.data + "/";
        }
        return item;
        //      10  GMail, -mailto
        //      12 Yahoo, -mailto
        //      13  MSN, -mailto

    };

    var showChangeContactStatusConfirmationPanel = function (statusValue) {
        jq("#changeContactStatusConfirmation .crm-actionButtonsBlock").show();
        jq("#changeContactStatusConfirmation .crm-actionProcessInfoBlock").hide();
        jq("#changeContactStatusConfirmation").attr("data-statusValue", statusValue);
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#changeContactStatusConfirmation", 500, 191, 0);
    };

    var showAddTagToContactGroupConfirmationPanel = function (params, text) {
        jq("#addTagToContactGroupConfirmation .crm-actionButtonsBlock").show();
        jq("#addTagToContactGroupConfirmation .crm-actionProcessInfoBlock").hide();
        ASC.CRM.ContactFullCardView.tagText = text
        ASC.CRM.ContactFullCardView.tagParams = params;
        jq("#addTagDialog").hide();
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#addTagToContactGroupConfirmation", 500, 210, 0);
    };

    var changeContactStatus = function (statusValue) {
        switch (ASC.CRM.ContactFullCardView.changeContactStatusGroupAuto) {
            case null:
                if (ASC.CRM.ContactFullCardView.isCompany === false && ASC.CRM.ContactFullCardView.companyID === 0) {
                    Teamlab.updateCrmPersonContactStatus({}, window.sliderListItems.id,
                            { personid: window.sliderListItems.id, contactStatusid: statusValue },
                            function () {
                                PopupKeyUpActionProvider.EnableEsc = true;
                                jq.unblockUI();
                            });
                } else {
                    showChangeContactStatusConfirmationPanel(statusValue);
                }
                break;
            case false:
                Teamlab.updateCrmContactContactStatus({}, window.sliderListItems.id,
                       { contactid: window.sliderListItems.id, contactStatusid: statusValue },
                       function () {
                           PopupKeyUpActionProvider.EnableEsc = true;
                           jq.unblockUI();
                       });
                break;
            case true:
                if (ASC.CRM.ContactFullCardView.isCompany === true) {
                    Teamlab.updateCrmCompanyContactStatus({}, window.sliderListItems.id,
                        { companyid: window.sliderListItems.id, contactStatusid: statusValue }, 
                        function () {
                            PopupKeyUpActionProvider.EnableEsc = true;
                            jq.unblockUI();
                        });
                } else {
                    Teamlab.updateCrmPersonContactStatus({}, window.sliderListItems.id,
                        { personid: window.sliderListItems.id, contactStatusid: statusValue },
                        function () {
                            PopupKeyUpActionProvider.EnableEsc = true;
                            jq.unblockUI();
                        });
                }
                break;
        }
    };

    var addTagToContactOnly = function (params, text) {
        Teamlab.addCrmTag(params, ASC.CRM.TagView.EntityType, ASC.CRM.TagView.EntityID, text,
                {
                    success: function (params, tag) {
                        ASC.CRM.TagView.callback_add_tag(params, tag);
                        PopupKeyUpActionProvider.EnableEsc = true;
                        jq.unblockUI();
                    },
                    before: function () {
                        jq("#tagContainer .adding_tag_loading").show();
                        jq("#addTagDialog").hide();
                    },
                    after: function () {
                        jq("#tagContainer .adding_tag_loading").hide();
                    }
                });
    };

    var addTagToContactGroup = function (params, text) {
        Teamlab.addCrmContactTagToGroup(params,
            ASC.CRM.ContactFullCardView.isCompany === true ? "company" : "person",
            ASC.CRM.ContactFullCardView.contactID,
            text,
            {
                success: function (params, tag) {
                    ASC.CRM.TagView.callback_add_tag(params, tag);
                    PopupKeyUpActionProvider.EnableEsc = true;
                    jq.unblockUI();
                },
                before: function () {
                    jq("#tagContainer .adding_tag_loading").show();
                    jq("#addTagDialog").hide();
                },
                after: function () {
                    jq("#tagContainer .adding_tag_loading").hide();
                }
            });
    };

    var addTagToGroupOrNot = function (params, text) {
        switch (ASC.CRM.ContactFullCardView.addTagToContactGroupAuto) {
            case null:
                if (ASC.CRM.ContactFullCardView.isCompany === false && ASC.CRM.ContactFullCardView.companyID === 0) {
                    addTagToContactOnly(params, text);
                } else {
                    showAddTagToContactGroupConfirmationPanel(params, text);
                }
                break;
            case false:
                addTagToContactOnly(params, text);
                break;
            case true:
                addTagToContactGroup(params, text);
                break;
        }
    };

    var renderContactNetworks = function() {
        if (typeof (window.contactNetworks) != "undefined" && window.contactNetworks.length != 0) {
            var $currentContainer,
                $currentPrimaryContainer,
                isFirst = true;
            for (var i = 0, n = window.contactNetworks.length; i < n; i++) {
                contactNetworksFactory(window.contactNetworks[i]);
                var currentData = window.contactNetworks[i];
                if (currentData.isPrimary) {
                    $currentPrimaryContainer = jq.tmpl("collectionContainerTmpl",
                        { Type: currentData.infoTypeLocalName })
                        .insertBefore("#contactTagsTR").children(".collectionItemsTD");
                    jq.tmpl("collectionTmpl", currentData).appendTo($currentPrimaryContainer);
                    isFirst = true;
                } else {
                    if (isFirst || window.contactNetworks[i - 1].infoType != currentData.infoType) {
                        $currentContainer = jq.tmpl("collectionContainerTmpl",
                            { Type: currentData.infoTypeLocalName })
                            .appendTo("#contactAdditionalTable").children(".collectionItemsTD");
                        jq.tmpl("collectionTmpl", currentData).appendTo($currentContainer);
                        isFirst = false;
                    } else {
                        jq.tmpl("collectionTmpl", currentData).appendTo($currentContainer);
                    }
                }
            }
        }

        if (jq("#contactGeneralList .writeEmail").length == 1) {
                var basePathMail = ASC.CRM.Common.getMailModuleBasePath();

                pathCreateEmail = [
                    basePathMail,
                    "#composeto/crm=",
                    ASC.CRM.ContactFullCardView.contactID
                ].join(''),

                pathSortEmails = [
                    basePathMail,
                    "#inbox/",
                    "from=",
                    jq("#contactGeneralList .writeEmail").attr("data-email"),
                    "/sortorder=descending/"
                ].join('');

            jq("#contactGeneralList .writeEmail").attr("href", pathCreateEmail);

            jq("#contactGeneralList .viewMailingHistory a.button").attr("href", pathSortEmails);
            jq("#contactGeneralList .viewMailingHistory").removeClass("display-none");
        } else {
            jq("#contactGeneralList .viewMailingHistory").remove();
        }
    };

    var renderCustomFields = function() {

        if (typeof (window.customFieldList) != "undefined" && window.customFieldList.length != 0) {
            var sortedList = [],
                subList = {
                    label : "",
                    list  : []
                };

            for (var i = 0, n = window.customFieldList.length; i < n; i++) {
                var field = window.customFieldList[i];
                if (jQuery.trim(field.mask) != "") {
                    field.mask = jq.evalJSON(field.mask);
                }

                if (field.fieldType == 4 || i == n - 1) {
                    if (field.fieldType != 4) {
                        subList.list.push(field);
                    }

                    if ((i != 0 || i == n - 1) && subList.label == "") {
                        if (jq("#contactAdditionalTable").length == 0) {
                            jq.tmpl("customFieldListWithoutLabelTmpl", { list: subList.list }).insertBefore("#contactHistoryTable");
                        } else {
                            jq.tmpl("customFieldListWithoutLabelWithGroupTmpl", { list: subList.list }).appendTo("#contactAdditionalTable");
                        }
                    } else {
                        sortedList.push(subList);
                    }

                    subList = {
                        label: field.label,
                        list: []
                    };
                } else {
                    subList.list.push(field);
                }
            }

            for (var i = 0, n = sortedList.length; i < n; i++) {
                if (sortedList[i].list.length == 0) {
                    sortedList.splice(i, 1);
                    i--;
                    n--;
                }
            }
            jq.tmpl("customFieldListTmpl", sortedList).insertBefore("#contactHistoryTable");
        }
    };

    var showMergePanelComplete = function() {
        jq("#mergePanel .infoPanel").hide();
        jq("#contactDetailsMenuPanel").hide();
        jq(".mainContainerClass .containerHeaderBlock .menu-small.active").removeClass("active");
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#mergePanel", 400, 400, 0);
    };

    return {
        contactID: 0,
        contactDisplayName: "",
        isCompany: undefined,
        init: function (contactID,
                        contactDisplayName,
                        isCompany,
                        companyID,
                        changeContactStatusGroupAuto,
                        addTagToContactGroupAuto,
                        contactPhotoFileSizeNote,
                        contactPhotoMedium,
                        isAdmin) {

            ASC.CRM.ContactFullCardView.contactID = contactID;
            ASC.CRM.ContactFullCardView.contactDisplayName = contactDisplayName;
            ASC.CRM.ContactFullCardView.isCompany = isCompany;
            ASC.CRM.ContactFullCardView.companyID = companyID;
            ASC.CRM.ContactFullCardView.changeContactStatusGroupAuto = changeContactStatusGroupAuto;
            ASC.CRM.ContactFullCardView.addTagToContactGroupAuto = addTagToContactGroupAuto;
            ASC.CRM.ContactFullCardView.tagParams = {};
            ASC.CRM.ContactFullCardView.tagText = "";

            var $avatar = jq("#contactProfile .contact_photo:first");
            ASC.CRM.Common.loadContactFoto($avatar, $avatar, $avatar.attr("data-avatarurl"));


            if (!jq.browser.mobile) {
                initUploadPhotoPanel(contactPhotoFileSizeNote, contactPhotoMedium);

                ASC.CRM.ContactPhotoUploader.initPhotoUploader(
                    jq("#divLoadPhotoWindow"),
                    jq("#contactProfile .contact_photo"),
                    { contactID: jq.getURLParam("id"), uploadOnly: false });
            }

            for (var i = 0, n = window.contactTags.length; i < n; i++) {
                window.contactTags[i] = Encoder.htmlDecode(window.contactTags[i]);
            }
            for (var i = 0, n = window.contactAvailableTags.length; i < n; i++) {
                window.contactAvailableTags[i] = Encoder.htmlDecode(window.contactAvailableTags[i]);
            }

            jq.tmpl("tagViewTmpl",
                    {
                      tags          : window.contactTags,
                      availableTags : window.contactAvailableTags
                    })
                    .appendTo("#contactTagsTR>td:last");

            ASC.CRM.TagView.init("contact", false, {
                addTag: function (params, text) {
                    addTagToGroupOrNot(params, text);
                }
            });

            if (typeof (window.contactResponsibleIDs) != "undefined" && window.contactResponsibleIDs.length != 0) {
                jq("#contactManagerList").html(ASC.CRM.Common.getAccessListHtml(window.contactResponsibleIDs));
            }
            renderContactNetworks();
            renderCustomFields();

            if (jq("#contactAdditionalTable tbody").children().length <= 1) {
                jq("#contactAdditionalTable").remove();
            }

            ASC.CRM.Common.RegisterContactInfoCard();

            jq.registerHeaderToggleClick("#contactProfile .crm-detailsTable", "tr.headerToggleBlock");
            jq("#contactHistoryTable .headerToggle").bind("click", function() {
                ASC.CRM.HistoryView.activate();
            });

            jq("#contactProfile .headerToggle").not("#contactHistoryTable .headerToggle").each(
                function() {
                    jq(this).parents("tr.headerToggleBlock:first").nextUntil(".headerToggleBlock").hide();
                });


            initChangeContactStatusConfirmationPanel(isAdmin);
            initAddTagToContactGroupConfirmationPanel(isAdmin);
            initMergePanel();

            initSliderControl();
            if (jq.trim(jq("#contactManagerList").text()).length == 0) {
                jq("#contactManagerList").parents("tr:first").remove();
            }
        },

        showMergePanel: function() {
            if (typeof (window.contactToMergeSelector) == "undefined") {
                LoadingBanner.displayLoading();
                var _selectorType = ASC.CRM.ContactFullCardView.isCompany === true ? 0 : 1,
                    _entityType = 0,
                    _entityID = 0;

                Teamlab.getCrmContactsByPrefix({},
                {
                    filter: {
                        prefix     : ASC.CRM.ContactFullCardView.contactDisplayName,
                        searchType : _selectorType,
                        entityType : _entityType,
                        entityID   : _entityID
                    },
                    success: function(par, contacts) {
                        var _excludedArrayIDs = [];
                        for (var i = 0, n = contacts.length; i < n; i++) {
                            if (contacts[i].id != ASC.CRM.ContactFullCardView.contactID) {
                                _excludedArrayIDs.push(contacts[i].id);
                            } else {
                                contacts.splice(i, 1);
                                i--;
                                n--;
                            }
                        }

                        var contactsCount = contacts.length;

                        jq.tmpl("listContactsToMergeTmpl", { contacts: contacts, count: contactsCount }).appendTo("#listContactsToMerge");
                        window["contactToMergeSelector"] = new ASC.CRM.ContactSelector.ContactSelector("contactToMergeSelector",
                        {
                            SelectorType: _selectorType,
                            EntityType: _entityType,
                            EntityID: _entityID,
                            ShowOnlySelectorContent: false,
                            DescriptionText: (ASC.CRM.ContactFullCardView.isCompany === true ? ASC.CRM.Resources.CRMContactResource.FindCompanyByName : ASC.CRM.Resources.CRMContactResource.FindEmployeeByName),
                            DeleteContactText: "",
                            AddContactText: "",
                            IsInPopup: true,
                            NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
                            NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
                            NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
                            ShowChangeButton: true,
                            ShowAddButton: false,
                            ShowDeleteButton: false,
                            ShowContactImg: true,
                            ShowNewCompanyContent: false,
                            ShowNewContactContent: false,
                            presetSelectedContactsJson: '',
                            ExcludedArrayIDs: _excludedArrayIDs,
                            HTMLParent: "#listContactsToMerge .contactToMergeSelectorContainer"
                        });


                        jq("#listContactsToMerge input[type='radio']:first").prop("checked", true);

                        LoadingBanner.hideLoading();
                        showMergePanelComplete();
                    }
                });
            } else {
                showMergePanelComplete();
            }
        },

        mergeContacts: function() {
            var fromID = ASC.CRM.ContactFullCardView.contactID,
                toID = jq("#listContactsToMerge input[name=contactToMerge]:checked").val() * 1;

            if (toID == 0) {
                if (typeof (window.contactToMergeSelector.SelectedContacts[0]) == "undefined") {
                    jq("#mergePanel .infoPanel > div").text(ASC.CRM.Resources.CRMJSResource.ErrorContactIsNotSelected);
                    jq("#mergePanel .infoPanel").show();
                    return;
                }
                toID = window.contactToMergeSelector.SelectedContacts[0] * 1;
            }

            jq("#mergePanel .crm-actionButtonsBlock").hide();
            jq("#mergePanel .crm-actionProcessInfoBlock").show();

            Teamlab.mergeCrmContacts({ isCompany: ASC.CRM.ContactFullCardView.isCompany },
                {
                    fromcontactid: fromID,
                    tocontactid: toID
                },
                function(params, contact) {
                    location.href = ["default.aspx?id=", contact.id, params.isCompany === true ? "" : "&type=people"].join("");
                });
        }
    };
})();


ASC.CRM.ContactDetailsView = (function() {
    var _projectList = [];
    var _canCreateProjects = false;

    var initTabs = function (contactsTabVisible, projectsTabVisible, socialMediaTabVisible) {
        window.ASC.Controls.ClientTabsNavigator.init("ContactTabs", {
            tabs: [
            {
                title: ASC.CRM.Resources.CRMCommonResource.Profile,
                selected: false,
                anchor: "profile",
                divID: "profileTab"
            },
            {
                title: ASC.CRM.Resources.CRMTaskResource.Tasks,
                selected: false,
                anchor: "tasks",
                divID: "tasksTab"
            },
            {
                title: ASC.CRM.Resources.CRMContactResource.Persons,
                selected: false,
                anchor: "contacts",
                divID: "contactsTab",
                visible: contactsTabVisible
            },
            {
                title: ASC.CRM.Resources.CRMDealResource.Deals,
                selected: false,
                anchor: "deals",
                divID: "dealsTab"
            },
            {
                title: ASC.CRM.Resources.CRMCommonResource.Documents,
                selected: false,
                anchor: "files",
                divID: "filesTab"
            },
            {
                title: ASC.CRM.Resources.CRMCommonResource.Projects,
                selected: false,
                anchor: "projects",
                divID: "projectsTab",
                visible: projectsTabVisible
            },
            {
                title: "Twitter",
                selected: false,
                anchor: "twitter",
                divID: "socialMediaTab",
                visible: socialMediaTabVisible
            }
            ]
        });
    };

    var initAnchorLinking = function() {
        ASC.Controls.AnchorController.bind(/profile/, ASC.CRM.HistoryView.activate);

        ASC.Controls.AnchorController.bind(/tasks/, ASC.CRM.ListTaskView.activate);

        ASC.CRM.ListContactView.isContentRendered = false;
        ASC.Controls.AnchorController.bind(/contacts/, function() {
            if (ASC.CRM.ListContactView.isContentRendered == false) {
                ASC.CRM.ListContactView.isContentRendered = true;
                ASC.CRM.ListContactView.renderSimpleContent(false, true);
            }
        });

        ASC.Controls.AnchorController.bind(/twitter/, ASC.CRM.SocialMedia.activate);
        ASC.Controls.AnchorController.bind(/deals/, ASC.CRM.DealTabView.activate);
        ASC.Controls.AnchorController.bind(/files/, window.Attachments.loadFiles);

        ASC.CRM.ContactDetailsView.isProjectTabRendered = false;
        ASC.Controls.AnchorController.bind(/projects/, function() {
            if (ASC.CRM.ContactDetailsView.isProjectTabRendered == false) {
                ASC.CRM.ContactDetailsView.isProjectTabRendered = true;
                activateProjectsTab();
            }
        });
    };

    var initProjectSelector = function(projectList) {
        if (jq.browser.mobile === true) {
            var chooseOption = {
                id: 0,
                title: ASC.CRM.Resources.CRMJSResource.LinkWithProject
            };
            projectList.splice(0, 0, chooseOption);

            jq.tmpl("projectSelectorOptionTmpl", projectList).appendTo("#projectsInContactPanel select");
            jq("#projectsInContactPanel select")
                .change(function(evt) {
                    chooseProject(jq(this).children("option:selected"), this.value);
                });
        } else {
            jq.tmpl("projectSelectorItemTmpl", projectList).appendTo("#projectSelectorContainer>.dropdown-content");

            jq.dropdownToggle({
                dropdownID       : "projectSelectorContainer",
                switcherSelector : "#projectsInContactPanel .selectProject>div",
                addTop           : 2
            });

            if (projectList.length > 0) {
                jq("#projectsInContactPanel .selectProject .menuAction").addClass("unlockAction");
                jq("#projectSelectorContainer").removeClass("display-none");
            }
            jq("#projectSelectorContainer").on("click", ".dropdown-content>li", function() {
                jq("#projectSelectorContainer").hide();
                var id = jq(this).attr("data-id");
                chooseProject(jq(this), id);
            });
        }
    };

    var chooseProject = function(element, id) {
        var data = {
            contactid: ASC.Projects.AllProject.contactID,
            projectid: id
        };
        Teamlab.addProjectForCrmContact({ element: element }, id, data, callback_add_project);
    };

    var removeProjectFromList = function(element) {
        element.remove();
        if (jq.browser.mobile === true) {
            jq("#projectsInContactPanel select").val(0).tlCombobox();
        } else {
            if (jq("#projectSelectorContainer .dropdown-item").length == 0) {
                jq("#projectsInContactPanel .selectProject .menuAction").removeClass("unlockAction");
                jq("#projectSelectorContainer").addClass("display-none");
            }
        }
    };

    var addProjectToList = function(project) {
        if (jq.browser.mobile === true) {
            jq.tmpl("projectSelectorOptionTmpl", project).appendTo("#projectsInContactPanel select");
            //jq("#projectsInContactPanel select").val(0).tlCombobox();
        } else {
            jq.tmpl("projectSelectorItemTmpl", project).appendTo("#projectSelectorContainer>.dropdown-content");
            jq("#projectsInContactPanel .selectProject .menuAction:not(.unlockAction)").addClass("unlockAction");
            jq("#projectSelectorContainer.display-none").hide();
            jq("#projectSelectorContainer.display-none").removeClass("display-none");
        }
    };

    var callback_add_project = function(params, project) {
        ASC.Projects.AllProject.addProjectsToSimpleList(project);
        removeProjectFromList(params.element);
    };

    var callback_remove_project = function(params, project) {
        jq(params.element).remove();
        if (jq("#tableListProjects>tbody>tr").length == 0) {
            jq("#projectsInContactPanel").hide();
            jq("#tableListProjectsContainer:not(.display-none)").addClass("display-none");
            jq("#projectsEmptyScreen.display-none").removeClass("display-none");
        }
        addProjectToList(project);
    };

    var callback_get_projects_data = function(params, allProjects, contactProjects) {
        _projectList = allProjects;
        if (typeof (params[0].__count) != "undefined" && params[0].__count != 0) {
            for (var i = 0; i < allProjects.length; i++) {
                if (allProjects[i].canLinkContact === false) {
                    allProjects.splice(i, 1);
                    i--;
                }
            }
            _projectList = allProjects;

            for (var i = 0, n = contactProjects.length; i < n; i++) {
                var idToExclude = contactProjects[i].id;
                for (var j = 0, m = allProjects.length; j < m; j++) {
                    if (allProjects[j].id == idToExclude) {
                        allProjects.splice(j, 1);
                        break;
                    }
                }
            }
            initProjectSelector(allProjects);
        }

        if (typeof (params[1].__count) != "undefined" && params[1].__count != 0) {
            jq("#projectsEmptyScreen:not(.display-none)").addClass("display-none");
            jq("#projectsEmptyScreenWithoutButton:not(.display-none)").addClass("display-none");

            jq("#tableListProjectsContainer.display-none").removeClass("display-none");
            ASC.Projects.AllProject.renderListProjects(contactProjects);
            jq("#projectsInContactPanel").show();
        } else {
            jq("#tableListProjectsContainer:not(.display-none)").addClass("display-none");
            if (_canCreateProjects === true || _projectList.length != 0) {
                jq("#projectsEmptyScreen.display-none").removeClass("display-none");
            } else {
                jq("#projectsEmptyScreenWithoutButton.display-none").removeClass("display-none");
            }
        }
        LoadingBanner.hideLoading();
    };


    var getProjectsData = function(contactID) {
        LoadingBanner.displayLoading();
        var filter = {
            sortBy    : "title",
            sortOrder : "ascending",
            fields    : "id,title,isPrivate,security"
        };

        Teamlab.joint()
            .getPrjProjects({}, { filter: filter })
            .getProjectsForCrmContact({}, contactID)
            .start({}, {
                success: callback_get_projects_data
            });
    };

    var activateProjectsTab = function() {
        jq("#projectsEmptyScreen .emptyScrBttnPnl>a").bind("click", function() {
            jq("#projectsEmptyScreen:not(.display-none)").addClass("display-none");
            jq("#projectsInContactPanel").show();
        });

        ASC.Projects.AllProject.init(true);
        ASC.Projects.AllProject.contactID = parseInt(jq.getURLParam("id"));
        if (!isNaN(ASC.Projects.AllProject.contactID)) {
            jq("#projectsInContactPanel .createNewProject>div").click(function() {
                location.href = [StudioManager.getLocationPathToModule("projects"), "projects.aspx?action=add&contactID=", ASC.Projects.AllProject.contactID].join("");
            });
            getProjectsData(ASC.Projects.AllProject.contactID);
        }
        jq("#tableListProjects").on("click", ".trash>img.trash_delete", function(event) {
            var $trashObj = jq(this);
            $trashObj.hide();
            $trashObj.parent().children(".trash_progress").removeClass("display-none");
            var $line = $trashObj.parents("tr:first"),
                id = $line.attr("id"),
                data = {
                    contactid: ASC.Projects.AllProject.contactID,
                    projectid: id
                };
            Teamlab.removeProjectFromCrmContact({ element: $line }, id, data, callback_remove_project);
        });
    };

    var initAttachments = function () {
        window.Attachments.init();
        window.Attachments.bind("addFile", function(ev, file) {
            var contactID = parseInt(jq.getURLParam("id"));
            if (!isNaN(contactID)) {

                var type = "contact",
                    fileids = [];
                fileids.push(file.id);

                Teamlab.addCrmEntityFiles({}, contactID, type, {
                    entityid   : contactID,
                    entityType : type,
                    fileids    : fileids
                }, function(params, data) {
                    window.Attachments.appendFilesToLayout(data.files);
                    params.fromAttachmentsControl = true;
                    ASC.CRM.HistoryView.addEventToHistoryLayout(params, data);
                });
            }
        });

        window.Attachments.bind("deleteFile", function(ev, fileId) {
            var $fileLinkInHistoryView = jq("#fileContent_" + fileId);
            if ($fileLinkInHistoryView.length != 0) {
                var messageID = $fileLinkInHistoryView.parents("div[id^=eventAttach_]").attr("id").split("_")[1];
                ASC.CRM.HistoryView.deleteFile(fileId, messageID);
            } else {
                Teamlab.removeCrmEntityFiles({ fileId: fileId }, fileId, {
                    success: function(params) {
                        window.Attachments.deleteFileFromLayout(params.fileId);
                    }
                });
            }
        });
    };

    var initContactDetailsMenuPanel = function() {
        jq(document).ready(function() {
            jq.dropdownToggle({
                dropdownID: "contactDetailsMenuPanel",
                switcherSelector: ".mainContainerClass .containerHeaderBlock .menu-small",
                addTop: -2,
                addLeft: -10,
                showFunction: function(switcherObj, dropdownItem) {
                    if (dropdownItem.is(":hidden")) {
                        switcherObj.addClass('active');
                    } else {
                        switcherObj.removeClass('active');
                    }
                },
                hideFunction: function() {
                    jq(".mainContainerClass .containerHeaderBlock .menu-small.active").removeClass("active");
                }
            });
        });
    };

    var initOtherActionMenu = function (isShared) {
        ASC.CRM.Common.removeExportButtons();

        var params = null;
        if (!isShared) {
            params = { taskResponsibleSelectorUserIDs: window.contactResponsibleIDs };
        }

        jq("#menuCreateNewTask").bind("click", function () {
            ASC.CRM.TaskActionView.showTaskPanel(0, "contact", 0, window.contactForInitTaskActionPanel, params);
        });

        ASC.CRM.ListTaskView.bindEmptyScrBtnEvent(params);

        var href = jq("#menuCreateNewDeal").attr("href") + "&contactID=" + jq.getURLParam("id");
        jq("#menuCreateNewDeal").attr("href", href);
    };

    var initEmptyScreens = function(isCompany, imgSrcEmptyPeople, imgSrcEmptyProjects) {
        if (isCompany === true) {
            jq.tmpl("emptyScrTmpl",
                {ID: "emptyPeopleInCompanyPanel",
                ImgSrc: imgSrcEmptyPeople,
                Header: ASC.CRM.Resources.CRMContactResource.EmptyContentPeopleHeader,
                Describe: ASC.CRM.Resources.CRMContactResource.EmptyContentPeopleDescribe,
                ButtonHTML: ["<a class='link-with-entity baseLinkAction' ",
                            "onclick='javascript:jq(\"#peopleInCompanyPanel\").show();jq(\"#emptyPeopleInCompanyPanel\").addClass(\"display-none\");'>",
                            ASC.CRM.Resources.CRMContactResource.AssignContact,
                            "</a>"
                            ].join(''),
                CssClass: "display-none"
            }).insertAfter("#contactListBox");
        }

        jq.tmpl("emptyScrTmpl",
                {
                    ID: "projectsEmptyScreen",
                    ImgSrc: imgSrcEmptyProjects,
                    Header: ASC.CRM.Resources.CRMContactResource.EmptyContactProjectListHeader,
                    Describe: ASC.CRM.Resources.CRMContactResource.EmptyContactProjectListDescription,
                    ButtonHTML: ["<a class='link-with-entity baseLinkAction'>",
                                ASC.CRM.Resources.CRMContactResource.AssignProject,
                                "</a>"
                    ].join(''),
                    CssClass: "display-none"
                }).insertAfter("#tableListProjects");

        jq.tmpl("emptyScrTmpl",
                {
                    ID: "projectsEmptyScreenWithoutButton",
                    ImgSrc: imgSrcEmptyProjects,
                    Header: ASC.CRM.Resources.CRMContactResource.EmptyContactProjectListHeader,
                    Describe: ASC.CRM.Resources.CRMContactResource.EmptyContactProjectListDescriptionWithoutButton,
                    CssClass: "display-none"
                }).insertAfter("#tableListProjects");
    };

    var initPeopleContactSelector = function() {
        window["contactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("contactSelector",
                    {
                        SelectorType: 2,
                        EntityType: 0,
                        EntityID: 0,
                        ShowOnlySelectorContent: true,
                        DescriptionText: ASC.CRM.Resources.CRMContactResource.FindContactByName,
                        DeleteContactText: "",
                        AddContactText: "",
                        IsInPopup: false,
                        NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
                        NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
                        NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
                        ShowChangeButton: false,
                        ShowAddButton: false,
                        ShowDeleteButton: false,
                        ShowContactImg: false,
                        ShowNewCompanyContent: false,
                        ShowNewContactContent: true,
                        presetSelectedContactsJson: '',
                        ExcludedArrayIDs: [],
                        HTMLParent: "#peopleInCompanyPanel"
                    });
        window.contactSelector.SelectItemEvent = ASC.CRM.ContactDetailsView.addPersonToCompany;
        ASC.CRM.ListContactView.removeMember = ASC.CRM.ContactDetailsView.removePersonFromCompany;

        jq(window).bind("getContactsFromApi", function(event, contacts) {
            var contactLength = contacts.length;
            if (contactLength == 0) {
                jq("#emptyPeopleInCompanyPanel.display-none").removeClass("display-none");
            } else {
                jq("#peopleInCompanyPanel").show();
                var contactIDs = [];
                for (var i = 0; i < contactLength; i++) {
                    contactIDs.push(contacts[i].id);
                }
                window.contactSelector.SelectedContacts = contactIDs;
            }
        });
    };

    return {
        init: function (isCompany, canCreateProjects, imgSrcEmptyPeople, imgSrcEmptyProjects, contactsTabVisible, projectsTabVisible, socialMediaTabVisible, isShared) {
            _canCreateProjects = canCreateProjects;
            initTabs(contactsTabVisible, projectsTabVisible, socialMediaTabVisible);
            initEmptyScreens(isCompany, imgSrcEmptyPeople, imgSrcEmptyProjects);

            if (isCompany === true) {
                initPeopleContactSelector()
            }

            initAnchorLinking();
            initAttachments();
            initContactDetailsMenuPanel();
            initOtherActionMenu(isShared);
        },

        removePersonFromCompany: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: parseInt(id) }, window.entityData.type, window.entityData.id, id, {
                before: function (params) {
                    jq("#simpleContactActionMenu").hide();
                    jq("#contactTable .entity-menu.active").removeClass("active");
                },
                after: function(params) {
                    var index = jq.inArray(params.contactID, window.contactSelector.SelectedContacts);
                    if (index != -1) {
                        window.contactSelector.SelectedContacts.splice(index, 1);
                    } else {
                        console.log("Can't find such contact in list");
                    }
                    ASC.CRM.ContactSelector.Cache = {};

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");
                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (window.contactSelector.SelectedContacts.length == 0) {
                            jq("#peopleInCompanyPanel").hide();
                            jq("#emptyPeopleInCompanyPanel.display-none").removeClass("display-none");
                        }
                    }, 500);

                }
            });
        },

        addPersonToCompany: function(obj) {
            if (jq("#contactItem_" + obj.id).length > 0) {
                return false;
            }
            var data =
                {
                    personid  : obj.id,
                    companyid : window.entityData.id
                };
            Teamlab.addCrmEntityMember({
                                            showCompanyLink : window.entityData.type != "company",
                                            showUnlinkBtn   : false,
                                            showActionMenu  : true
                                        },
                                        window.entityData.type, window.entityData.id, obj.id, data, {
                success: function(params, contact) {
                    ASC.CRM.ListContactView.CallbackMethods.addMember(params, contact);
                    //ASC.CRM.ContactSelector.Cache = {};

                    window.contactSelector.SelectedContacts.push(contact.id);
                    jq("#emptyPeopleInCompanyPanel:not(.display-none)").addClass("display-none");
                }
            });
        },
        
        checkSocialMediaError: function () {
            var smErrorMessage = jq("input[id$='_ctrlSMErrorMessage']").val();
            if (smErrorMessage != "" && smErrorMessage !== undefined) {
                ShowErrorMessage(smErrorMessage);
            }
        }
    };
})();


ASC.CRM.ContactActionView = (function () {
    var isInit = false,
        cache = {};
    this.ContactData = null;

    var renderContactNetworks = function() {
        jq("#generalListEdit").on('click', ".not_primary_field", function() {
            ASC.CRM.ContactActionView.choosePrimaryElement(jq(this), jq(this).parent().parent().parent().attr("id") == "addressContainer");
        });

        if (typeof (window.contactNetworks) != "undefined" && window.contactNetworks.length != 0) {
            for (var i = 0, n = window.contactNetworks.length; i < n; i++) {
                var networkItem = window.contactNetworks[i];
                if (networkItem.hasOwnProperty("infoType") && networkItem.hasOwnProperty("data")) {
                    if (networkItem.infoType == 7) { //Address
                        var address = jq.parseJSON(Encoder.htmlDecode(networkItem.data));

                        var $addressJQ = createNewAddress(jq('#addressContainer').children('div:first').clone(), networkItem.isPrimary, networkItem.category, address.street, address.city, address.state, address.zip, address.country);
                        $addressJQ.insertAfter(jq('#addressContainer').children('div:last')).show();
                        continue;
                    }

                    var container_id,
                        $newContact;

                    if (networkItem.infoType == 0) { //Phone
                        container_id = 'phoneContainer';

                        $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(networkItem.data), networkItem.isPrimary);

                        changePhoneCategory(
                                        $newContact.children('table').find('a'),
                                        jq("#phoneCategoriesPanel ul.dropdown-content li").children('a[category=' + networkItem.category + ']').text(),
                                        networkItem.category);
                    } else if (networkItem.infoType == 1) { //Email
                        container_id = 'emailContainer';

                        $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(networkItem.data), networkItem.isPrimary);

                        changeBaseCategory(
                                        $newContact.children('table').find('a'),
                                        jq("#baseCategoriesPanel ul.dropdown-content li").children('a[category=' + networkItem.category + ']').text(),
                                        networkItem.category);
                    } else {
                        container_id = 'websiteAndSocialProfilesContainer';

                        $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, Encoder.htmlDecode(networkItem.data));

                        changeBaseCategory(
                                        $newContact.find('a.social_profile_category'),
                                        jq("#baseCategoriesPanel ul.dropdown-content li").children('a[category=' + networkItem.category + ']').text(),
                                        networkItem.category);

                        ASC.CRM.ContactActionView.changeSocialProfileCategory(
                                        $newContact.find('a.social_profile_type'),
                                        networkItem.infoType,
                                        jq("#socialProfileCategoriesPanel ul.dropdown-content li").children('a[category=' + networkItem.infoType + ']').text(),
                                        jq("#socialProfileCategoriesPanel ul.dropdown-content li").children('a[category=' + networkItem.infoType + ']').attr('categoryName'));
                    }


                    $newContact.insertAfter(jq('#' + container_id).children('div:last')).show();
                    continue;
                }
            }

            var add_new_button_class = "crm-addNewLink";
            if (jq('#emailContainer').children('div').length > 1) {
                jq('#emailContainer').prev('dt').removeClass('crm-withGrayPlus');
            }
            jq('#emailContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#emailContainer').children('div:last').find("." + add_new_button_class).show();

            if (jq('#phoneContainer').children('div').length > 1) {
                jq('#phoneContainer').prev('dt').removeClass('crm-withGrayPlus');
            }
            jq('#phoneContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#phoneContainer').children('div:last').find("." + add_new_button_class).show();

            if (jq('#websiteAndSocialProfilesContainer').children('div').length > 1) {
                jq('#websiteAndSocialProfilesContainer').prev('dt').removeClass('crm-withGrayPlus');
            }
            jq('#websiteAndSocialProfilesContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#websiteAndSocialProfilesContainer').children('div:last').find("." + add_new_button_class).show();

            if (jq('#addressContainer').children('div').length > 1) {
                jq('#addressContainer').prev('dt').removeClass('crm-withGrayPlus');
            }
            jq('#addressContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
            jq('#addressContainer').children('div:last').find("." + add_new_button_class).show();
        }
    };

    var renderContactTags = function () {

        for (var i = 0, n = window.contactActionTags.length; i < n; i++) {
            window.contactActionTags[i] = Encoder.htmlDecode(window.contactActionTags[i]);
        }
        for (var i = 0, n = window.contactActionAvailableTags.length; i < n; i++) {
            window.contactActionAvailableTags[i] = Encoder.htmlDecode(window.contactActionAvailableTags[i]);
        }

        jq.tmpl("tagViewTmpl",
                        {
                            tags: window.contactActionTags,
                            availableTags: window.contactActionAvailableTags
                        })
                        .appendTo("#tagsContainer>div:first");
        ASC.CRM.TagView.init("contact", true);
        if (window.contactActionTags.length > 0) {
            jq("#tagsContainer>div:first").removeClass("display-none");
            jq("#tagsContainer").prev().removeClass("crm-withGrayPlus");
        }

    };

    var renderCustomFields = function() {
        if (typeof (window.customFieldList) != "undefined" && window.customFieldList.length != 0) {
            ASC.CRM.Common.renderCustomFields(customFieldList, "custom_field_", "customFieldRowTmpl", "#generalListEdit");
        }
        jq.registerHeaderToggleClick("#contactProfileEdit", "dt.headerToggleBlock");
        jq("#contactProfileEdit dt.headerToggleBlock").each(
                function() {
                    jq(this).nextUntil("dt.headerToggleBlock").hide();
                });
    };

    var initContactType = function (contactTypeID) {
        if (typeof (window.contactAvailableTypes) != "undefined" && window.contactAvailableTypes.length != 0) {
            var html = "";
            for (var i = 0, n = window.contactAvailableTypes.length; i < n; i++) {
                html += ["<option value='",
                    window.contactAvailableTypes[i].id,
                    "'",
                    window.contactAvailableTypes[i].id == contactTypeID ? " selected='selected'" : "",
                    ">",
                    jq.htmlEncodeLight(window.contactAvailableTypes[i].title),
                    "</option>"
                ].join('');
            }
            jq("#contactTypeContainer select").html(jq("#contactTypeContainer select").html() + html);
            if (contactTypeID != 0) {
                jq("#contactTypeContainer").prev().removeClass("crm-withGrayPlus");
                jq("#contactTypeContainer > div:first").removeClass("display-none");
            }
        }
    };

    var initOtherActionMenu = function() {
        ASC.CRM.Common.removeExportButtons();
        jq("#menuCreateNewTask").bind("click", function() { ASC.CRM.TaskActionView.showTaskPanel(0, "", 0, null, {}); });
    };

    var initConfirmationGotoSettingsPanel = function (isCompany) {
        var view = isCompany === true ? "&view=company" : "&view=person";

        jq.tmpl("blockUIPanelTemplate", {
            id: "confirmationGotoSettingsPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: "",
            innerHtmlText:
            ["<div class=\"confirmationNote\">",
                ASC.CRM.Resources.CRMJSResource.ConfirmGoToCustomFieldPage,
            "</div>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            OKBtnHref: "settings.aspx?type=custom_field" + view,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ""
        }).insertAfter("#otherContactCustomFieldPanel");
    };

    var initUploadPhotoPanel = function (contactPhotoFileSizeNote, contactPhotoMedium) {
        jq.tmpl("blockUIPanelTemplate", {
            id: "divLoadPhotoWindow",
            headerTest: ASC.CRM.Resources.CRMContactResource.ChooseProfilePhoto,
            questionText: "",
            innerHtmlText:
            ["<div id=\"divLoadPhotoFromPC\" style=\"margin-top: -20px;\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.LoadPhotoFromPC,
                "</h4>",
                "<div class=\"describe-text\" style=\"margin-bottom: 5px;\">",
                    contactPhotoFileSizeNote,
                "</div>",
                "<div>",
                    "<a id=\"changeLogo\" class=\"button middle gray\">",
                        ASC.CRM.Resources.CRMJSResource.Browse, "...",
                    "</a>",
                    "<span class=\"fileUploadDscr\">",
                        ASC.CRM.Resources.CRMJSResource.NoFileSelected,
                    "</span>",
                    "<span class=\"fileUploadError\"></span>",
                    "<br />",
                "</div>",
            "</div>",
            "<div id=\"divLoadPhotoDefault\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.ChangePhotoToDefault,
                "</h4>",
                "<div id=\"divDefaultImagesHolder\" data-uploadOnly=\"true\">",
                    "<div class=\"ImageHolderOuter\" onclick=\"ASC.CRM.SocialMedia.DeleteContactAvatar();\">",
                        "<img class=\"AvatarImage\" src=",
                        contactPhotoMedium,
                        " alt=\"\" />",
                    "</div>",
                "</div>",
            "</div>",
            "<div id=\"divLoadPhotoFromSocialMedia\">",
                "<h4>",
                    ASC.CRM.Resources.CRMContactResource.LoadPhotoFromSocialMedia,
                "</h4>",
                "<div id=\"divImagesHolder\" data-uploadOnly=\"true\">",
                "</div>",
                "<div style=\"clear: both;\">",
                    "<div id=\"divAjaxImageContainerPhotoLoad\">",
                    "</div>",
                "</div>",
            "</div>"].join(''),
            OKBtn: "",
            CancelBtn: "",
            progressText: ""
        }).insertAfter("#divSMProfilesWindow");
    };

    var bindEventFindInSocialMediaButton = function (isCompany) {
        if (!isCompany) {
            setInterval(function () {
                var $input1 = jq("#contactProfileEdit .info_for_person input[name=baseInfo_firstName]"),
                    $input2 = jq("#contactProfileEdit .info_for_person input[name=baseInfo_lastName]");

                if ($input1.val().trim() == "" || $input2.val().trim() == "") {
                    jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Enabled").hide();
                    jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Disabled").show();
                } else {
                    jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Disabled").hide();
                    jq("#contactProfileEdit .info_for_person .findInSocialMediaButton_Enabled").show();
                }
            }, 500);
        } else {
            setInterval(function () {
                var $input = jq("#contactProfileEdit .info_for_company input[name=baseInfo_companyName]");
                if ($input.val().trim() == "") {
                    jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Enabled").hide();
                    jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Disabled").show();
                } else {
                    jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Disabled").hide();
                    jq("#contactProfileEdit .info_for_company .findInSocialMediaButton_Enabled").show();
                }
            }, 500);
        }
    };

    var createNewAddress = function($contact, is_primary, category, street, city, state, zip, country) {
        if (jq("#addressContainer").children("div").length != 1) {
            $contact.attr("style", "margin-top: 10px;");
        }

        if (typeof (is_primary) != "undefined") {
            changeAddressPrimaryCategory($contact, is_primary);
        }
        if (typeof (category) != "undefined") {
            var $categoryOption = $contact.find('select.address_category').children('option[category="' + category + '"]');
            if ($categoryOption.length == 1) {
                $categoryOption.attr("selected", "selected");
                changeAddressCategory($contact.find('select.address_category'), category);
            }
        }

        var parts = jq("#addressContainer").children("div:last").attr('selectname').split('_'),
            ind = parts[2] * 1 + 1;
        $contact.find('input, textarea, select').not('.address_category').each(function() {
            if (jq(this).attr('name') != "") {
                var parts = jq(this).attr('name').split('_');
                parts[2] = ind;
                jq(this).attr('name', parts.join('_'));
            }
        });

        parts = $contact.attr('selectname').split('_');
        parts[2] = ind;
        $contact.attr('selectname', parts.join('_'));

        if (street && street != "") $contact.find('textarea.contact_street').val(street);
        if (city && city != "") $contact.find('input.contact_city').val(city);
        if (state && state != "") $contact.find('input.contact_state').val(state);
        if (zip && zip != "") $contact.find('input.contact_zip').val(zip);
        if (country && country != "") $contact.find('select.contact_country').val(country);

        $contact.find('textarea.contact_street').Watermark(ASC.CRM.Resources.CRMJSResource.AddressWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_city').Watermark(ASC.CRM.Resources.CRMJSResource.CityWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_state').Watermark(ASC.CRM.Resources.CRMJSResource.StateWatermark, ASC.CRM.ContactActionView.WatermarkClass);
        $contact.find('input.contact_zip').Watermark(ASC.CRM.Resources.CRMJSResource.ZipCodeWatermark, ASC.CRM.ContactActionView.WatermarkClass);

        $contact.find('select.contact_country').attr('name', $contact.attr('selectname'));
        jq('<option value="" style="display:none;"></option>').prependTo($contact.find('select.contact_country'));

        return $contact;
    };

    var changeAddressPrimaryCategory = function($divAddressObj, isPrimary) {
        var tmpNum = isPrimary ? 1 : 0,
            tmpClass = isPrimary ? "is_primary primary_field" : "is_primary not_primary_field",
            $switcerObj = $divAddressObj.find('.is_primary');

        $switcerObj.attr("class", tmpClass);
        $switcerObj.attr("alt", ASC.CRM.Resources.CRMJSResource.Primary);
        $switcerObj.attr("title", ASC.CRM.Resources.CRMJSResource.Primary);

        var parts = $divAddressObj.attr('selectname').split('_');
        parts[5] = tmpNum;
        $divAddressObj.attr('selectname', parts.join('_'));

        $divAddressObj.find('input, textarea, select').not('.address_category').each(function() {
            var parts = jq(this).attr('name').split('_');
            parts[5] = tmpNum;
            jq(this).attr('name', parts.join('_'));
        });
    };

    var _changeCommunicationPrimaryCategory = function($divObj, isPrimary) {
        var tmpNum = isPrimary ? 1 : 0,
            tmpClass = isPrimary ? "is_primary primary_field" : "is_primary not_primary_field",
            $switcerObj = $divObj.find('.is_primary');

        $switcerObj.attr("class", tmpClass);
        $switcerObj.attr("alt", ASC.CRM.Resources.CRMJSResource.Primary);
        $switcerObj.attr("title", ASC.CRM.Resources.CRMJSResource.Primary);

        var $inputObj = $divObj.find('input.textEdit'),
            parts = $inputObj.attr('name').split('_');

        parts[4] = tmpNum;
        $inputObj.attr('name', parts.join('_'));
    };

    var changeBaseCategory = function(Obj, text, category) {
        jq(Obj).text(text);
        var $inputObj = jq(Obj).parents('tr:first').find('input'),
            parts = $inputObj.attr('name').split('_');
        parts[3] = category;
        $inputObj.attr('name', parts.join('_'));
        jq("#baseCategoriesPanel").hide();
    };

    var changePhoneCategory = function(Obj, text, category) {
        jq(Obj).text(text);
        var $inputObj = jq(Obj).parents('tr:first').find('input'),
            parts = $inputObj.attr('name').split('_');
        parts[3] = category;
        $inputObj.attr('name', parts.join('_'));
        jq("#phoneCategoriesPanel").hide();
    };

    var changeAddressCategory = function(switcerObj, category) {
        jq(switcerObj).parents('table:first').find('input, textarea, select').not('.address_category').each(function() {
            var parts = jq(this).attr('name').split('_');
            parts[3] = category;
            jq(this).attr('name', parts.join('_'));
        });
    };

    var removeAssignedPersonFromCompany = function(id) {
        if (jq("#trashImg_" + id).length == 1) {
            jq("#trashImg_" + id).hide();
            jq("#loaderImg_" + id).show();
        }
        if (typeof (id) == "number") {
            var index = jq.inArray(id, window.assignedContactSelector.SelectedContacts);
            if (index != -1) {
                window.assignedContactSelector.SelectedContacts.splice(index, 1);
            } else {
                console.log("Can't find such contact in list");
            }
            ASC.CRM.ContactSelector.Cache = {};
        } else {
            for (var i = 0, n = ASC.CRM.SocialMedia.selectedPersons.length; i < n; i++) {
                if (ASC.CRM.SocialMedia.selectedPersons[i].id == id) {
                    ASC.CRM.SocialMedia.selectedPersons.splice(i, 1);
                    break;
                }
            }
        }

        jq("#contactItem_" + id).animate({ opacity: "hide" }, 500);

        setTimeout(function() {
            jq("#contactItem_" + id).remove();
            if (jq("#contactTable tr").length == 0) {
                jq("#contactListBox").parent().addClass('hiddenFields');
            }
        }, 500);
    };

    var addAssignedPersonToCompany = function(obj, params) {
        if (jq("#contactItem_" + obj.id).length > 0) return false;

        Teamlab.getCrmContact({}, obj.id, {
            success: function(par, contact) {
                ASC.CRM.Common.contactItemFactory(contact, { showUnlinkBtn: true, showActionMenu: false });
                jq.tmpl("simpleContactTmpl", contact).prependTo("#contactTable tbody");
                jq("#contactListBox").parent().removeClass('hiddenFields');
                ASC.CRM.Common.RegisterContactInfoCard();

                window.assignedContactSelector.SelectedContacts.push(contact.id);
                //ASC.CRM.ContactSelector.Cache = {};
            }
        });

    };

    var validateEmail = function($emailInputObj) {
        var email = $emailInputObj.value.trim(),
            $tableObj = jq($emailInputObj).parents("table:first"),
            reg = new RegExp(ASC.Resources.Master.EmailRegExpr, "i");

        if (email == "" || reg.test(email)) {
            $tableObj.css("borderColor", "");
            $tableObj.parent().children(".requiredErrorText").hide();
            return true;
        } else {
            $tableObj.css("borderColor", "#CC0000");
            $tableObj.parent().children(".requiredErrorText").show();
            $emailInputObj.focus();
            return false;
        }

    };

    var validatePhone = function($phoneInputObj) {
        var phone = $phoneInputObj.value.trim(),
            $tableObj = jq($phoneInputObj).parents("table:first"),
            reg = new RegExp(/(^\+)?(\d+)/);

        if (phone == "" || reg.test(phone)) {
            $tableObj.css("borderColor", "");
            $tableObj.parent().children(".requiredErrorText").hide();
            return true;
        } else {
            $tableObj.css("borderColor", "#CC0000");
            $tableObj.parent().children(".requiredErrorText").show();
            $phoneInputObj.focus();
            return false;
        }

    };

    var disableSubmitForm = function () {
        jq("#contactProfileEdit input, #contactProfileEdit select, #contactProfileEdit textarea").attr("readonly", "readonly").addClass('disabled');
        jq("#contactProfileEdit .input_with_type").addClass('disabled');
        jq(".under_logo .linkChangePhoto").addClass("disable");
    };

    var enableSubmitForm = function () {
        jq("#contactProfileEdit input.disabled, #contactProfileEdit select.disabled, #contactProfileEdit textarea.disabled").removeAttr("readonly").removeClass('disabled');
        jq("#contactProfileEdit .input_with_type.disabled").removeClass('disabled');
        jq(".under_logo .linkChangePhoto.disable").removeClass("disable");
    };

    var validateSubmitForm = function () {
        var isValid = true,
            isEmailValid = true;
        if (jq("#typeAddedContact").val() == "people") {
            if (jq("#contactProfileEdit input[name=baseInfo_firstName]").val().trim() == "") {
                ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_firstName]"));
                isValid = false;
            }

            if (jq("#contactProfileEdit input[name=baseInfo_lastName]").val().trim() == "") {
                ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_lastName]"));
                isValid = false;
            }

            if (typeof (window.companySelector) != "undefined") {
                jq("#companySelectorsContainer input[name=baseInfo_compID]").val(typeof (window.companySelector.SelectedContacts[0]) != 'undefined' ? window.companySelector.SelectedContacts[0] : "");
                jq("#companySelectorsContainer input[name=baseInfo_compName]").val(jq('#contactTitle_companySelector_0').hasClass(ASC.CRM.ContactActionView.WatermarkClass) ? "" : jq('#contactTitle_companySelector_0').val());
            }
        } else {
            if (jq("#contactProfileEdit input[name=baseInfo_companyName]").val().trim() == "") {
                ShowRequiredError(jq("#contactProfileEdit input[name=baseInfo_companyName]"));
                isValid = false;
            }
        }

        if (!isValid) {
            enableSubmitForm();
            return false;
        }

        jq("#emailContainer > div:not(:first) > table input").each(function () {
            if (!validateEmail(this)) {
                isEmailValid = false;
            }
        });

        if (!isEmailValid) {
            enableSubmitForm();
            jq.scrollTo(jq("#emailContainer").position().top - 100, { speed: 400 });
            return false;
        }

        return true;
    };

    var prepareAddressDataForSubmitForm = function () {
        jq('#addressContainer').children('div:first').find('input, textarea, select').attr('name', '');
        jq('#addressContainer').children('div:not(:first)').each(function () {
            var $curObj = jq(this);
            if ($curObj.find('.contact_street').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                && $curObj.find('.contact_city').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                    && $curObj.find('.contact_state').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                        && $curObj.find('.contact_zip').hasClass(ASC.CRM.ContactActionView.WatermarkClass)
                            && $curObj.find('.contact_country').val() == ASC.CRM.Resources.CRMJSResource.ChooseCountry) {
                $curObj.find('input, textarea, select').attr('name', '');
            } else {
                $curObj.addClass("not_empty");
                $curObj.find('.' + ASC.CRM.ContactActionView.WatermarkClass).val(" ");
                if ($curObj.find('.contact_country').val() == ASC.CRM.Resources.CRMJSResource.ChooseCountry) {
                    $curObj.find('.contact_country').val("");
                }
            }
        });

        if ((jq('#addressContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
            !jq('#addressContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                && jq('#addressContainer').children('div.not_empty').length > 0) {
            ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#addressContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), true);
        }
    };

    var prepareEmailAndPhoneDataForSubmitForm = function () {
        jq('#emailContainer').children('div:first').find('input').attr('name', '');
        jq('#phoneContainer').children('div:first').find('input').attr('name', '');

        jq('#emailContainer').children('div:not(:first)').find('input').each(function () {
            if (jq(this).val().trim() != '') {
                jq(this).parents('table:first').parent().addClass("not_empty");
            }
        });
        jq('#phoneContainer').children('div:not(:first)').find('input').each(function () {
            if (jq(this).val().trim() != '') {
                jq(this).parents('table:first').parent().addClass("not_empty");
            }
        });

        if ((jq('#emailContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
                       !jq('#emailContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                           && jq('#emailContainer').children('div.not_empty').length > 0) {
            ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#emailContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), false);
        }

        if ((jq('#phoneContainer').children('div:not(:first)').find('.primary_field').length == 0 ||
            !jq('#phoneContainer').children('div:not(:first)').find('.primary_field').parent().parent().hasClass('not_empty'))
                && jq('#phoneContainer').children('div.not_empty').length > 0) {
            ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#phoneContainer').children('div.not_empty')[0]).children(".actions_for_item").children(".is_primary"), false);
        }
    };

    return {
        init: function (dateMask, contactID, contactTypeID, isShared, _selectorType, isAdmin, contactPhotoFileSizeNote, contactPhotoMedium) {
            if (isInit === false) {
                isInit = true;

                var isCompany = (jq("#typeAddedContact").val() === "company"),

                    $avatar = jq("#contactPhoto .contact_photo:first"),
                    handlerAvatarUrl = $avatar.attr("data-avatarurl");
                if (handlerAvatarUrl != "") {
                    ASC.CRM.Common.loadContactFoto($avatar, $avatar, handlerAvatarUrl);
                }

                ASC.CRM.UserSelectorListView.Init(
                    "_ContactManager",
                    "UserSelectorListView_ContactManager",
                    true,
                    true,
                    ASC.CRM.Resources.CRMContactResource.NotifyContactManager,
                    [],
                    null,
                    [],
                    "#contactActionViewManager");


                if (isCompany === false) {
                    if (window.presetCompanyForPersonJson != null && window.presetCompanyForPersonJson != "") {
                        window.presetCompanyForPersonJson = new Array(jq.parseJSON(window.presetCompanyForPersonJson));
                    }
                    window["companySelector"] = new ASC.CRM.ContactSelector.ContactSelector("companySelector",
                    {
                        SelectorType: _selectorType,
                        EntityType: 0,
                        EntityID: 0,
                        ShowOnlySelectorContent: false,
                        DescriptionText: ASC.CRM.Resources.CRMContactResource.FindCompanyByName,
                        DeleteContactText: "",
                        AddContactText: "",
                        IsInPopup: false,
                        NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
                        NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
                        NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
                        ShowChangeButton: true,
                        ShowAddButton: false,
                        ShowDeleteButton: false,
                        ShowContactImg: true,
                        ShowNewCompanyContent: true,
                        ShowNewContactContent: false,
                        presetSelectedContactsJson: '',
                        ExcludedArrayIDs: [],
                        HTMLParent: "#companySelectorsContainer div:first",
                        presetSelectedContactsJson: window.presetCompanyForPersonJson
                    });

                } else {
                    window["assignedContactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("assignedContactSelector",
                    {
                        SelectorType: _selectorType,
                        EntityType: 0,
                        EntityID: 0,
                        ShowOnlySelectorContent: true,
                        DescriptionText: ASC.CRM.Resources.CRMContactResource.FindContactByName,
                        DeleteContactText: "",
                        AddContactText: "",
                        IsInPopup: false,
                        NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
                        NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
                        NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
                        ShowChangeButton: false,
                        ShowAddButton: false,
                        ShowDeleteButton: false,
                        ShowContactImg: false,
                        ShowNewCompanyContent: false,
                        ShowNewContactContent: true,
                        ShowOnlySelectorContent: true,
                        HTMLParent: "#assignedContactsListEdit dd.assignedContacts",
                        ExcludedArrayIDs: [],
                        presetSelectedContactsJson: window.presetPersonsForCompanyJson,
                    });
                }
                if (isAdmin === true) {
                    initConfirmationGotoSettingsPanel(isCompany);
                }

                ASC.CRM.ListContactView.renderSimpleContent(true, false);
                ASC.CRM.ContactActionView.WatermarkClass = "crm-watermarked";

                renderContactTags();
                renderContactNetworks();

                jq("#generalListEdit").on("click", ".crm-withGrayPlus", function(event) {
                    var container_id = jq(this).next('dd').attr('id');
                    //jq(this).next('dd').find(".crm-addNewLink").show();

                    if (container_id == "addressContainer") {
                        ASC.CRM.ContactActionView.editAddress(jq("#addressContainer > div:first .crm-addNewLink"));
                    } else if (container_id == "tagsContainer" || container_id == "contactTypeContainer") {
                        jq("#" + container_id + " > div:first").removeClass("display-none");
                    } else {
                        ASC.CRM.ContactActionView.editCommunications(jq("#" + container_id).children("div:first").find(".crm-addNewLink"), container_id);
                    }

                    jq(this).removeClass("crm-withGrayPlus");
                });

                jq.dropdownToggle({ dropdownID: 'phoneCategoriesPanel', switcherSelector: '#phoneContainer .input_with_type a', addTop: 2, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'baseCategoriesPanel', switcherSelector: '#emailContainer .input_with_type a', noActiveSwitcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_category', addTop: 2, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'baseCategoriesPanel', switcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_category', noActiveSwitcherSelector: '#emailContainer .input_with_type a', addTop: 2, addLeft: 0 });
                jq.dropdownToggle({ dropdownID: 'socialProfileCategoriesPanel', switcherSelector: '#websiteAndSocialProfilesContainer .input_with_type a.social_profile_type', addTop: 2, addLeft: 0 });

                renderCustomFields();
                initContactType(contactTypeID);

                jq.tmpl("makePublicPanelTemplate",
                    {
                        Title: isCompany ?
                                ASC.CRM.Resources.CRMContactResource.MakePublicPanelTitleForCompany :
                                ASC.CRM.Resources.CRMContactResource.MakePublicPanelTitleForPerson,
                        Description: ASC.CRM.Resources.CRMContactResource.MakePublicPanelDescrForContact,
                        IsPublicItem: isShared,
                        CheckBoxLabel: isCompany ?
                                        ASC.CRM.Resources.CRMContactResource.MakePublicPanelCheckBoxLabelForCompany :
                                        ASC.CRM.Resources.CRMContactResource.MakePublicPanelCheckBoxLabelForPerson
                    }).appendTo("#makePublicPanel");

                jq("input.textEditCalendar").mask(dateMask);
                jq("input.textEditCalendar").datepickerWithButton();

                initOtherActionMenu();

                ASC.CRM.ListContactView.initConfirmationPanelForDelete();

                if (!jq.browser.mobile) {
                    initUploadPhotoPanel(contactPhotoFileSizeNote, contactPhotoMedium);

                    ASC.CRM.ContactPhotoUploader.initPhotoUploader(
                    jq("#divLoadPhotoWindow"),
                    jq("#contactProfileEdit .contact_photo"),
                    { contactID: contactID, uploadOnly: true });
                }


                if (typeof (window.assignedContactSelector) != "undefined") {
                    if (window.assignedContactSelector.SelectedContacts.length == 0) {
                        jq("#contactListBox").parent().addClass('hiddenFields');
                    }
                    window.assignedContactSelector.SelectItemEvent = addAssignedPersonToCompany;
                    ASC.CRM.ListContactView.removeMember = removeAssignedPersonFromCompany;
                }

                if (!isNaN(contactID) && jq("#deleteContactButton").length == 1) {
                    var contactName = jq("#deleteContactButton").attr("contactName");
                    jq("#deleteContactButton").unbind("click").bind("click", function() {
                        ASC.CRM.ListContactView.showConfirmationPanelForDelete(contactName, contactID, isCompany, false);
                    });
                }

                bindEventFindInSocialMediaButton(isCompany);
                
                jq("[id$='saveContactButton']").bind("click", function () { ASC.CRM.SocialMedia.EnsureLinkedInAccounts(); });
            }
        },

        editCommunicationsEvent: function(evt, container_id) {
            evt = evt || window.event;
            var $target = jq(evt.target || evt.srcElement);
            ASC.CRM.ContactActionView.editCommunications($target, container_id);
        },

        editCommunications: function($target, container_id) {
            var add_new_button_class = "crm-addNewLink",
                delete_button_class = "crm-deleteLink",
                primary_class = "primary_field";

            if ($target.length == 0 && container_id == "overviewContainer" || $target.hasClass(add_new_button_class)) {
                var $lastVisibleDiv = jq('#' + container_id).children('div:visible:last');
                if ($lastVisibleDiv.length != 0) {
                    $lastVisibleDiv.find("." + add_new_button_class).hide();
                }

                var is_primary = jq('#' + container_id).find(".primary_field").length == 0 ? true : undefined,
                    $newContact = ASC.CRM.ContactActionView.createNewCommunication(container_id, "", is_primary);

                $newContact.insertAfter(jq('#' + container_id).children('div:last')).show()
                $newContact.find('input.textEdit').focus();

            } else if ($target.hasClass(delete_button_class)) {
                var $divHTML = $target.parent().parent();
                if (jq('#' + container_id).children('div').length == 2) {
                    $divHTML.parent().prev('dt').addClass("crm-withGrayPlus");
                }

                $divHTML.remove();
                if ($divHTML.find('.' + primary_class).length == 1 && jq('#' + container_id).children('div:not(:first)').length >= 1) {
                    ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#' + container_id).children('div:not(:first)')[0]).find('.is_primary'), false);
                }
                jq('#' + container_id).children('div:not(:first)').find("." + add_new_button_class).hide();
                jq('#' + container_id).children('div:last').find("." + add_new_button_class).show();
            }
        },

        createNewCommunication: function(container_id, text, is_primary) {
            var $contact = jq('#' + container_id).children('div:first').clone();

            if (container_id != "overviewContainer") {
                var $lastInputElement = jq('#' + container_id).children('div:last').find('input.textEdit');
                if ($lastInputElement.length != 0) {
                    var parts = $lastInputElement.attr('name').split('_'),
                        ind = parts[2] * 1 + 1;

                    parts = $contact.find('input.textEdit').attr('name').split('_');
                    parts[2] = ind;
                    $contact.find('input.textEdit').attr('name', parts.join('_'));
                }

                if (text && text != "") {
                    $contact.children('table').find('input').val(text);
                }

                if (container_id === "phoneContainer") {
                    jq.forcePhoneSymbolsOnly($contact.children('table').find('input'));
                }

                if (typeof (is_primary) != "undefined") {
                    var $isPrimaryElem = $contact.children(".actions_for_item").children(".is_primary");
                    if ($isPrimaryElem.length != 0) {
                        _changeCommunicationPrimaryCategory($contact, is_primary);
                    }
                }
            } else {
                if (text && text != "") {
                    $contact.children('textarea').val(text);
                }
            }
            return $contact;
        },

        editAddressEvent: function(evt) {
            evt = evt || window.event;
            var $target = jq(evt.target || evt.srcElement);
            ASC.CRM.ContactActionView.editAddress($target);
        },

        editAddress: function($target) {
            var add_new_button_class = "crm-addNewLink",
                delete_button_class = "crm-deleteLink",
                primary_class = "primary_field";
            if ($target.hasClass(add_new_button_class)) {
                var $lastVisibleDiv = jq('#addressContainer').children('div:visible:last');
                if ($lastVisibleDiv.length != 0) {
                    $lastVisibleDiv.find("." + add_new_button_class).hide();
                }
                var is_primary = jq("#addressContainer").find(".primary_field").length == 0 ? true : undefined,
                    $newContact = createNewAddress(jq('#addressContainer').children('div:first').clone(), is_primary).insertAfter(jq('#addressContainer').children('div:last')).show();
                $newContact.find('textarea').focus();
            } else if ($target.hasClass(delete_button_class)) {
                var $divHTML = $target.parent().parent();

                if (jq('#addressContainer').children('div').length == 2) {
                    $divHTML.parent().prev('dt').addClass("crm-withGrayPlus");
                }
                $divHTML.remove();

                if ($divHTML.find('.' + primary_class).length == 1 && jq('#addressContainer').children('div:not(:first)').length >= 1) {
                    ASC.CRM.ContactActionView.choosePrimaryElement(jq(jq('#addressContainer').children('div:not(:first)')[0]).find('.is_primary'), true);
                }
                jq('#addressContainer').children('div:not(:first)').find("." + add_new_button_class).hide();
                jq('#addressContainer').children('div:last').find("." + add_new_button_class).show();
            }
        },

        changeSocialProfileCategory: function(Obj, category, text, categoryName) {
            var $divObj = jq(Obj).parents('table:first').parent();
            jq(Obj).text(text);
            $inputObj = jq(Obj).parents('tr:first').find('input');
            var parts = $inputObj.attr('name').split('_');
            parts[1] = categoryName;
            $inputObj.attr('name', parts.join('_'));

            var $findProfileObj = $divObj.find('.find_profile'),
                isShown = false,
                func = "",
                title = "",
                description = " ";
            switch (category) {
                case 4:
                    isShown = window.twitterSearchEnabled;
                    title = ASC.CRM.Resources.CRMJSResource.FindTwitter;
                    description = ASC.CRM.Resources.CRMJSResource.ContactTwitterDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindTwitterProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5)
                    break;
                case 5:
                    if (!window.linkedinSearchEnabled || jq("#typeAddedContact").val() === "company") {
                        isShown = false;
                    } else {
                        isShown = true;
                    }
                    title = ASC.CRM.Resources.CRMJSResource.FindLinkedIn;
                    description = ASC.CRM.Resources.CRMJSResource.ContactLinkedInDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindLinkedInProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5)
                    break;
                case 6:
                    isShown = window.facebokSearchEnabled;
                    title = ASC.CRM.Resources.CRMJSResource.FindFacebook;
                    description = ASC.CRM.Resources.CRMJSResource.ContactFacebookDescription;
                    func = (function(p1, p2) { return function() { ASC.CRM.SocialMedia.FindFacebookProfiles(jq(this), jq("#typeAddedContact").val(), p1, p2); } })(-3, 5);
                    break;
            }

            if (isShown) {
                $findProfileObj.unbind('click').click(func);
                $findProfileObj.attr('title', title).show();
            } else {
                $findProfileObj.hide();
            }

            $divObj.children(".text-medium-describe").text(description);
            jq("#socialProfileCategoriesPanel").hide();
        },

        choosePrimaryElement: function(switcerObj, isAddress) {
            if (!isAddress) {
                var $divObj = jq(switcerObj).parent().parent();
                _changeCommunicationPrimaryCategory($divObj, true);

                jq(switcerObj).parents('dd:first').children('div:not(:first)').not($divObj).each(function() {
                    _changeCommunicationPrimaryCategory(jq(this), false);
                });
            } else {
                var $divAddressObj = jq(switcerObj).parent().parent();
                changeAddressPrimaryCategory($divAddressObj, true);

                jq(switcerObj).parents('dd:first').children('div:not(:first)').not($divAddressObj).each(function() {
                    changeAddressPrimaryCategory(jq(this), false);
                });
            }

            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("class", "is_primary not_primary_field");
            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("alt", ASC.CRM.Resources.CRMJSResource.CheckAsPrimary);
            jq(switcerObj).parents('dd:first').find('.is_primary').not(switcerObj).attr("title", ASC.CRM.Resources.CRMJSResource.CheckAsPrimary);
        },


        submitForm: function (buttonUnicId) {
            try {

                disableSubmitForm();
                HideRequiredError();

                if (validateSubmitForm() === true) {
                    jq("#crm_contactMakerDialog .crm-actionButtonsBlock").hide();
                    jq("#crm_contactMakerDialog .crm-actionProcessInfoBlock").show();

                    prepareAddressDataForSubmitForm();
                    prepareEmailAndPhoneDataForSubmitForm();

                    jq('#overviewContainer').children('div:first').find('textarea').attr('name', '');
                    jq('#websiteAndSocialProfilesContainer').children('div:first').find('input').attr('name', '');
                    jq('#websiteAndSocialProfilesContainer').children('div:not(:first)').find('input').each(function () {
                        if (jq(this).val().trim() != '') {
                            jq(this).parents('table:first').parent().addClass("not_empty");
                        }
                    });


                    jq("#isPublicContact").val(jq("#isPublic").is(":checked"));

                    jq("#notifyContactManagers").val(jq("#cbxNotify_ContactManager").is(":checked"));
                    jq("#selectedContactManagers").val(window.SelectedUsers_ContactManager.IDs.join(","));


                    var $checkboxes = jq("#generalListEdit input[type='checkbox'][id^='custom_field_']");
                    if ($checkboxes) {
                        for (var i = 0, n = $checkboxes.length; i < n; i++) {
                            if (jq($checkboxes[i]).is(":checked")) {
                                var id = $checkboxes[i].id.replace('custom_field_', '');
                                jq("#generalListEdit input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
                            }
                        }
                    }

                    ASC.CRM.TagView.prepareTagDataForSubmitForm(jq("input[name='baseInfo_assignedTags']"));

                    if (jq("#typeAddedContact").val() == "people") {
                        return true;
                    } else if (ASC.CRM.SocialMedia.selectedPersons.length == 0) {
                        jq("#assignedContactsListEdit input[name='baseInfo_assignedContactsIDs']").val(window.assignedContactSelector.SelectedContacts);
                        return true;
                    } else {
                        var data = [];
                        for (var i = 0, n = ASC.CRM.SocialMedia.selectedPersons.length; i < n; i++) {
                            data.push({
                                Key: ASC.CRM.SocialMedia.selectedPersons[i].Key,
                                Value: ASC.CRM.SocialMedia.selectedPersons[i].Value
                            });
                        }

                        Teamlab.addCrmPerson({ buttonId: buttonUnicId }, data,
                            {
                                success: function (params, persons) {
                                    var personIds = [];
                                    for (var i = 0, n = persons.length; i < n; i++) {
                                        if (persons[i]) {
                                            personIds.push(persons[i].id);
                                        }
                                    }
                                    personIds = personIds.concat(window.assignedContactSelector.SelectedContacts);
                                    jq("#assignedContactsListEdit input[name='baseInfo_assignedContactsIDs']").val(personIds);
                                    __doPostBack(params.buttonId, '');
                                    return true;
                                },
                                error: function (params) {
                                     return false;
                                }
                            });
                    }
                }
                return false;

            } catch (e) {
                console.log(e);
                return false;
            }
        },

        showBaseCategoriesPanel: function(switcherUI) {
            jq("#baseCategoriesPanel a.dropdown-item").unbind('click').click(function() {
                changeBaseCategory(switcherUI, jq(this).text(), jq(this).attr("category"));

            });
        },

        showPhoneCategoriesPanel: function(switcherUI) {
            jq("#phoneCategoriesPanel a.dropdown-item").unbind('click').click(function() {
                changePhoneCategory(switcherUI, jq(this).text(), jq(this).attr("category"));
            });
        },

        showSocialProfileCategoriesPanel: function(switcherUI) {
            jq("#socialProfileCategoriesPanel a.dropdown-item").unbind('click').click(function() {
                ASC.CRM.ContactActionView.changeSocialProfileCategory(switcherUI, jq(this).attr("category") * 1, jq(this).text(), jq(this).attr("categoryName"));
            });
        },

        changeAddressCategory: function(obj) {
            changeAddressCategory(obj, jq(obj).find('option:selected').attr("category"));
        },

        showAssignedContactPanel: function() {
            jq('#assignedContactsListEdit .assignedContacts').removeClass('hiddenFields');
            jq('#assignedContactsListEdit .assignedContactsLink').addClass('hiddenFields');
        },

        showGotoAddSettingsPanel: function () {
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#confirmationGotoSettingsPanel", 500, 500, 0);
        }
    };
})();



/*
* --------------------------------------------------------------------
* jQuery-Plugin - sliderWithSections
* --------------------------------------------------------------------
*/

jQuery.fn.sliderWithSections = function (settings) {
    //accessible slider options
    var options = jQuery.extend({
        value: null,
        colors: null,
        values: null,
        defaultColor: '#E1E1E1',
        liBorderWidth: 1,
        sliderOptions: null,
        max: 0,
        marginWidth: 1,
        slide: function (e, ui) {
        }
    }, settings);


    //plugin-generated slider options (can be overridden)
    var sliderOptions = {
        step: 1,
        min: 0,
        orientation: 'horizontal',
        max: options.max,
        range: false, //multiple select elements = true
        slide: function (e, ui) { //slide function
            var thisHandle = jQuery(ui.handle);
            thisHandle.attr('aria-valuetext', options.values[ui.value]).attr('aria-valuenow', ui.value);

            if (ui.value != 0) {
                thisHandle.find('.ui-slider-tooltip .ttContent').html(options.values[ui.value]);
                thisHandle.removeClass("ui-slider-tooltip-hide");
            } else {
                thisHandle.addClass("ui-slider-tooltip-hide");
            }

            var liItems = jQuery(this).children('ol.ui-slider-scale').children('li');

            for (var i = 0; i < sliderOptions.max; i++) {
                if (i < ui.value) {
                    var color = options.colors != null && options.colors[i] ? options.colors[i] : 'transparent';
                    jQuery(liItems[i]).css('background-color', color);
                } else {
                    jQuery(liItems[i]).css('background-color', options.defaultColor);
                }
            }

            options.slide(e, ui);
        },

        value: options.value
    };

    //slider options from settings
    options.sliderOptions = (settings) ? jQuery.extend(sliderOptions, settings.sliderOptions) : sliderOptions;


    //create slider component div
    var sliderComponent = jQuery('<div></div>'),
        $tooltip = jQuery('<a href="#" tabindex="0" ' +
        'class="ui-slider-handle" ' +
        'role="slider" ' +
        'aria-valuenow="' + options.value + '" ' +
        'aria-valuetext="' + options.values[options.value] + '"' +
        '><span class="ui-slider-tooltip ui-widget-content ui-corner-all"><span class="ttContent"></span>' +
        '<span class="ui-tooltip-pointer-down ui-widget-content"><span class="ui-tooltip-pointer-down-inner"></span></span>' +
        '</span></a>')
        .data('handleNum', options.value)
        .appendTo(sliderComponent);
    sliderComponent.find('.ui-slider-tooltip .ttContent').html(options.values[options.value]);
    if (options.values[options.value] == "") {
        sliderComponent.children(".ui-slider-handle").addClass("ui-slider-tooltip-hide");
    }

    var scale = sliderComponent.append('<ol class="ui-slider-scale ui-helper-reset" role="presentation" style="width: 100%; height: 100%;"></ol>').find('.ui-slider-scale:eq(0)');

    //var widthVal = (1 / sliderOptions.max * 100).toFixed(2) + '%';
    var sliderWidth = jQuery(this).css('width').replace('px', '') * 1,
        widthVal = ((sliderWidth - options.marginWidth * (sliderOptions.max - 1) - 2 * options.liBorderWidth * sliderOptions.max) / sliderOptions.max).toFixed(4);
    for (var i = 0; i <= sliderOptions.max; i++) {
        var style = (i == sliderOptions.max || i == 0) ? 'display: none;' : '',
            liStyle = (i == sliderOptions.max) ? 'display: none;' : '',
            color = 'transparent';

        if (i < options.value) {
            color = options.colors != null && options.colors[i] ? options.colors[i] : 'transparent';
        } else {
            color = options.defaultColor;
        }

        scale.append('<li style="left:' + leftVal(i, sliderWidth) + '; background-color:' + color + '; height: 100%; width:' + widthVal + 'px;' + liStyle + '"></li>');
    };

    function leftVal (i, sliderWidth) {
        var widthVal = ((sliderWidth - options.marginWidth * (sliderOptions.max - 1) - 2 * options.liBorderWidth * sliderOptions.max) / sliderOptions.max);
        return ((widthVal + 2 * options.liBorderWidth + options.marginWidth) * i).toFixed(4) + 'px';
    }

    //inject and return
    sliderComponent.appendTo(jQuery(this)).slider(options.sliderOptions).attr('role', 'application');
    sliderComponent.find('.ui-tooltip-pointer-down-inner').each(function () {
        var bWidth = jQuery('.ui-tooltip-pointer-down-inner').css('borderTopWidth'),
            bColor = jQuery(this).parents('.ui-slider-tooltip').css('backgroundColor');
        jQuery(this).css('border-top', bWidth + ' solid ' + bColor);
    });

    return this;
};

jq(document).ready(function() {
    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintTypes",
        dropdownID: "files_hintTypesPanel",
        fixWinSize: false
    });

    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintCsv",
        dropdownID: "files_hintCsvPanel",
        fixWinSize: false
    });
});