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

/*******************************************************************************
ListCasesView.ascx
*******************************************************************************/
ASC.CRM.ListCasesView = (function() {

    //Teamlab.bind(Teamlab.events.getException, _onGetException);

    function _onGetException(params, errors) {
        console.log('cases.js ', errors);
        LoadingBanner.hideLoading();
    };

    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListCasesView.cookieKey && ASC.CRM.ListCasesView.cookieKey != "") {
            var cookie = {
                page        : page,
                countOnPage : countOnPage
            };
            jq.cookies.set(ASC.CRM.ListCasesView.cookieKey, cookie, { path: location.pathname });
        }
    };

    var _getFilterSettings = function(startIndex) {
        startIndex = startIndex || 0;
        var settings = {
            startIndex : startIndex,
            count      : ASC.CRM.ListCasesView.entryCountOnPage
        };

        if (!ASC.CRM.ListCasesView.advansedFilter) return settings;

        var param = ASC.CRM.ListCasesView.advansedFilter.advansedFilter();

        jq(param).each(function(i, item) {
            switch (item.id) {
                case "sorter":
                    settings.sortBy = item.params.id;
                    settings.sortOrder = item.params.dsc == true ? 'descending' : 'ascending';
                    break;
                case "text":
                    settings.filterValue = item.params.value;
                    break;
                default:
                    if (item.hasOwnProperty("apiparamname") && item.params.hasOwnProperty("value") && item.params.value != null) {
                        settings[item.apiparamname] = item.params.value;
                    }
                    break;
            }
        });
        return settings;
    };

    var _changeFilter = function () {
        ASC.CRM.ListCasesView.deselectAll();

        var defaultStartIndex = 0;
        if (ASC.CRM.ListCasesView.defaultCurrentPageNumber != 0) {
            _setCookie(ASC.CRM.ListCasesView.defaultCurrentPageNumber, window.casesPageNavigator.EntryCountOnPage);
            defaultStartIndex = (ASC.CRM.ListCasesView.defaultCurrentPageNumber - 1) * window.casesPageNavigator.EntryCountOnPage;
            ASC.CRM.ListCasesView.defaultCurrentPageNumber = 0;
        } else {
            _setCookie(0, window.casesPageNavigator.EntryCountOnPage);
        }

        _renderContent(defaultStartIndex);
    };

    var _renderContent = function(startIndex) {
        ASC.CRM.ListCasesView.casesList = new Array();

        LoadingBanner.displayLoading();
        jq("#mainSelectAllCases").prop("checked", false);

        _getCases(startIndex);
    };

    var _initPageNavigatorControl = function (countOfRows, currentPageNumber, visiblePageCount) {
        window.casesPageNavigator = new ASC.Controls.PageNavigator.init("casesPageNavigator", "#divForCasesPager", countOfRows, visiblePageCount, currentPageNumber,
                                                                        ASC.CRM.Resources.CRMJSResource.Previous, ASC.CRM.Resources.CRMJSResource.Next);

        window.casesPageNavigator.changePageCallback = function(page) {
            _setCookie(page, window.casesPageNavigator.EntryCountOnPage);

            var startIndex = window.casesPageNavigator.EntryCountOnPage * (page - 1);
            _renderContent(startIndex);
        };
    };

    var _renderCasesPageNavigator = function(startIndex) {
        var tmpTotal;
        if (startIndex >= ASC.CRM.ListCasesView.Total) {
            tmpTotal = startIndex + 1;
        } else {
            tmpTotal = ASC.CRM.ListCasesView.Total;
        }
        window.casesPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListCasesView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);
        jq("#tableForCasesNavigation").show();
    };

    var _renderSimpleCasesPageNavigator = function() {
        jq("#casesHeaderMenu .menu-action-simple-pagenav").html("");
        var $simplePN = jq("<div></div>"),
            lengthOfLinks = 0;
        if (jq("#divForCasesPager .pagerPrevButtonCSSClass").length != 0) {
            lengthOfLinks++;
            jq("#divForCasesPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
        }
        if (jq("#divForCasesPager .pagerNextButtonCSSClass").length != 0) {
            lengthOfLinks++;
            if (lengthOfLinks === 2) {
                jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
            }
            jq("#divForCasesPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
        }
        if ($simplePN.children().length != 0) {
            $simplePN.appendTo("#casesHeaderMenu .menu-action-simple-pagenav");
            jq("#casesHeaderMenu .menu-action-simple-pagenav").show();
        } else {
            jq("#casesHeaderMenu .menu-action-simple-pagenav").hide();
        }
    };

    var _renderCheckedCasesCount = function(count) {
        if (count != 0) {
            jq("#casesHeaderMenu .menu-action-checked-count > span").text(jq.format(ASC.CRM.Resources.CRMJSResource.ElementsSelectedCount, count));
            jq("#casesHeaderMenu .menu-action-checked-count").show();
        } else {
            jq("#casesHeaderMenu .menu-action-checked-count > span").text("");
            jq("#casesHeaderMenu .menu-action-checked-count").hide();
        }
    };

    var _renderNoCasesEmptyScreen = function() {
        jq("#caseTable tbody tr").remove();
        jq("#caseList").hide();
        jq("#caseFilterContainer").hide();
        ASC.CRM.Common.hideExportButtons();
        jq("#emptyContentForCasesFilter").hide();
        jq("#casesEmptyScreen").show();
    };

    var _renderNoCasesForQueryEmptyScreen = function() {
        jq("#caseTable tbody tr").remove();
        jq("#caseList").hide();
        ASC.CRM.Common.hideExportButtons();
        jq("#mainSelectAllCases").attr("disabled", true);
        jq("#casesEmptyScreen").hide();
        jq("#emptyContentForCasesFilter").show();
    };

    var _showActionMenu = function(caseID) {
        var caseItem = null;
        for (var i = 0, n = ASC.CRM.ListCasesView.casesList.length; i < n; i++) {
            if (caseID == ASC.CRM.ListCasesView.casesList[i].id) {
                caseItem = ASC.CRM.ListCasesView.casesList[i];
                break;
            }
        }
        if (caseItem == null) return;

        jq("#caseActionMenu .editCaseLink").attr("href", jq.format("cases.aspx?id={0}&action=manage", caseID));

        jq("#caseActionMenu .deleteCaseLink").unbind("click").bind("click", function() {
            jq("#caseActionMenu").hide();
            jq("#caseTable .entity-menu.active").removeClass("active");

            ASC.CRM.ListCasesView.showConfirmationPanelForDelete(caseItem.title, caseID, true);
        });

        jq("#caseActionMenu .showProfileLink").attr("href", jq.format("cases.aspx?id={0}", caseID));

        if (ASC.CRM.ListCasesView.currentUserIsAdmin || Teamlab.profile.id == caseItem.createdBy.id) {
            jq("#caseActionMenu .setPermissionsLink").show();
            jq("#caseActionMenu .setPermissionsLink").unbind("click").bind("click", function() {
                jq("#caseActionMenu").hide();
                jq("#caseTable .entity-menu.active").removeClass("active");

                ASC.CRM.ListCasesView.deselectAll();
                ASC.CRM.ListCasesView.selectedItems.push(_createShortCase(caseItem));

                _showSetPermissionsPanel({ isBatch: false });
            });
        } else {
            jq("#caseActionMenu .setPermissionsLink").hide();
        }
    };

    var _getCases = function(startIndex) {
        var filters = _getFilterSettings(startIndex);
        Teamlab.getCrmCases({ startIndex: startIndex || 0 },
        {
            filter: filters,
            success: callback_get_cases_by_filter
        });
    };

    var _resizeFilter = function() {
        var visible = jq("#caseFilterContainer").is(":hidden") == false;
        if (ASC.CRM.ListCasesView.isFilterVisible == false && visible) {
            ASC.CRM.ListCasesView.isFilterVisible = true;
            if (ASC.CRM.ListCasesView.advansedFilter) {
                jq("#casesAdvansedFilter").advansedFilter("resize");
            }
        }
    };

    var _caseItemFactory = function(caseItem, selectedIDs) {
        var index = jq.inArray(caseItem.id, selectedIDs);
        caseItem.isChecked = index != -1;
    };

    var callback_get_cases_by_filter = function(params, cases) {
        ASC.CRM.ListCasesView.Total = params.__total || 0;
        var startIndex = params.__startIndex || 0;

        if (ASC.CRM.ListCasesView.Total === 0 &&
                    typeof (ASC.CRM.ListCasesView.advansedFilter) != "undefined" &&
                    ASC.CRM.ListCasesView.advansedFilter.advansedFilter().length == 1) {
            ASC.CRM.ListCasesView.noCases = true;
            ASC.CRM.ListCasesView.noCasesForQuery = true;
        } else {
            ASC.CRM.ListCasesView.noCases = false;
            if (ASC.CRM.ListCasesView.Total === 0) {
                ASC.CRM.ListCasesView.noCasesForQuery = true;
            } else {
                ASC.CRM.ListCasesView.noCasesForQuery = false;
            }
        }

        if (ASC.CRM.ListCasesView.noCases) {
            _renderNoCasesEmptyScreen();
            LoadingBanner.hideLoading();
            return false;
        }

        if (ASC.CRM.ListCasesView.noCasesForQuery) {
            _renderNoCasesForQueryEmptyScreen();

            jq("#caseFilterContainer").show();
            _resizeFilter();
            LoadingBanner.hideLoading();
            return false;
        }

        if (cases.length == 0) {//it can happen when select page without elements after deleting
            jq("casesEmptyScreen").hide();
            jq("#emptyContentForCasesFilter").hide();
            jq("#caseTable tbody tr").remove();
            jq("#tableForCasesNavigation").show();
            jq("#mainSelectAllCases").attr("disabled", true);
            ASC.CRM.Common.hideExportButtons();
            LoadingBanner.hideLoading();
            return false;
        }

        jq("#totalCasesOnPage").text(ASC.CRM.ListCasesView.Total);

        jq("#emptyContentForCasesFilter").hide();
        jq("#casesEmptyScreen").hide();
        ASC.CRM.Common.showExportButtons();
        jq("#caseFilterContainer").show();
        _resizeFilter();
        jq("#mainSelectAllCases").removeAttr("disabled");
        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
        }

        for (var i = 0, n = cases.length; i < n; i++) {
            _caseItemFactory(cases[i], selectedIDs);
        }
        ASC.CRM.ListCasesView.casesList = cases;


        jq("#caseList").show();
        jq("#caseTable tbody").replaceWith(jq.tmpl("caseListTmpl", { cases: ASC.CRM.ListCasesView.casesList }));

        ASC.CRM.ListCasesView.checkFullSelection();

        _renderCasesPageNavigator(startIndex);
        _renderSimpleCasesPageNavigator();

        window.scrollTo(0, 0);
        ScrolledGroupMenu.fixContentHeaderWidth(jq('#casesHeaderMenu'));
        LoadingBanner.hideLoading();
    };

    var callback_add_tag = function(params, tag) {
        jq("#addTagCasesDialog").hide();
        if (params.isNewTag) {
            var tag = {
                value: params.tagName,
                title: params.tagName
            };
            window.caseTags.push(tag);
            _renderTagElement(tag);

            ASC.CRM.ListCasesView.advansedFilter = ASC.CRM.ListCasesView.advansedFilter.advansedFilter(
            {
                nonetrigger: true,
                sorters: [],
                filters: [
                    { id: "tags", type: 'combobox', options: ASC.CRM.Data.caseTags, enable: ASC.CRM.Data.caseTags.length > 0 }
                ]

            });
        }
    };

    var callback_delete_batch_cases = function(params, data) {
        var newCasesList = new Array();
        for (var i = 0, len_i = ASC.CRM.ListCasesView.casesList.length; i < len_i; i++) {
            var isDeleted = false;
            for (var j = 0, len_j = params.casesIDsForDelete.length; j < len_j; j++)
                if (params.casesIDsForDelete[j] == ASC.CRM.ListCasesView.casesList[i].id) {
                isDeleted = true;
                break;
            }
            if (!isDeleted) {
                newCasesList.push(ASC.CRM.ListCasesView.casesList[i]);
            }

        }
        ASC.CRM.ListCasesView.casesList = newCasesList;

        ASC.CRM.ListCasesView.Total -= params.casesIDsForDelete.length;
        jq("#totalCasesOnPage").text(ASC.CRM.ListCasesView.Total);

        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
        }

        for (var i = 0, len = params.casesIDsForDelete.length; i < len; i++) {
            var $objForRemove = jq("#caseItem_" + params.casesIDsForDelete[i]);
            if ($objForRemove.length != 0) {
                $objForRemove.remove();
            }

            var index = jq.inArray(params.casesIDsForDelete[i], selectedIDs);
            if (index != -1) {
                selectedIDs.splice(index, 1);
                ASC.CRM.ListCasesView.selectedItems.splice(index, 1);
            }
        }
        jq("#mainSelectAllCases").prop("checked", false);

        _checkForLockMainActions();
        _renderCheckedCasesCount(ASC.CRM.ListCasesView.selectedItems.length);

        if (ASC.CRM.ListCasesView.Total == 0
            && (typeof (ASC.CRM.ListCasesView.advansedFilter) == "undefined"
            || ASC.CRM.ListCasesView.advansedFilter.advansedFilter().length == 1)) {
            ASC.CRM.ListCasesView.noCases = true;
            ASC.CRM.ListCasesView.noCasesForQuery = true;
        } else {
            ASC.CRM.ListCasesView.noCases = false;
            if (ASC.CRM.ListCasesView.Total === 0) {
                ASC.CRM.ListCasesView.noCasesForQuery = true;
            } else {
                ASC.CRM.ListCasesView.noCasesForQuery = false;
            }
        }
        PopupKeyUpActionProvider.EnableEsc = true;
        if (ASC.CRM.ListCasesView.noCases) {
            _renderNoCasesEmptyScreen();
            jq.unblockUI();
            return false;
        }

        if (ASC.CRM.ListCasesView.noCasesForQuery) {
            _renderNoCasesForQueryEmptyScreen();
            jq.unblockUI();
            return false;
        }

        if (jq("#caseTable tbody tr").length == 0) {
            jq.unblockUI();

            var startIndex = ASC.CRM.ListCasesView.entryCountOnPage * (window.casesPageNavigator.CurrentPageNumber - 1);
            if (startIndex >= ASC.CRM.ListCasesView.Total) {
                startIndex -= ASC.CRM.ListCasesView.entryCountOnPage;
            }
            _renderContent(startIndex);
        } else {
            jq.unblockUI();
        }
    };

    var callback_update_case_rights = function(params, cases) {
        for (var i = 0, n = cases.length; i < n; i++) {
            for (var j = 0, m = ASC.CRM.ListCasesView.casesList.length; j < m; j++) {
                var case_id = cases[i].id;
                if (case_id == ASC.CRM.ListCasesView.casesList[j].id) {
                    ASC.CRM.ListCasesView.casesList[j].isPrivate = cases[i].isPrivate;
                    jq("#caseItem_" + case_id).replaceWith(
                        jq.tmpl("caseTmpl", ASC.CRM.ListCasesView.casesList[j])
                    );
                    if (params.isBatch) {
                        jq("#checkCase_" + case_id).prop("checked", true);
                    } else {
                        ASC.CRM.ListCasesView.selectedItems = new Array();
                    }
                    break;
                }
            }
        }
        ASC.CRM.Common.RegisterContactInfoCard();
        PopupKeyUpActionProvider.EnableEsc = true;
        jq.unblockUI();
    };

    var _lockMainActions = function() {
        jq("#casesHeaderMenu .menuActionDelete").removeClass("unlockAction").unbind("click");
        jq("#casesHeaderMenu .menuActionAddTag").removeClass("unlockAction").unbind("click");
        jq("#casesHeaderMenu .menuActionPermissions").removeClass("unlockAction").unbind("click");
    };

    var _checkForLockMainActions = function() {
        if (ASC.CRM.ListCasesView.selectedItems.length === 0) {
            _lockMainActions();
            return;
        }
        if (!jq("#casesHeaderMenu .menuActionDelete").hasClass("unlockAction")) {
            jq("#casesHeaderMenu .menuActionDelete").addClass("unlockAction").unbind("click").bind("click", function() {
                _showDeletePanel();
            });
        }

        if (!jq("#casesHeaderMenu .menuActionAddTag").hasClass("unlockAction")) {
            jq("#casesHeaderMenu .menuActionAddTag").addClass("unlockAction");
        }

        if (!jq("#casesHeaderMenu .menuActionPermissions").hasClass("unlockAction")) {
            jq("#casesHeaderMenu .menuActionPermissions").addClass("unlockAction").unbind("click").bind("click", function() {
                _showSetPermissionsPanel({ isBatch: true });
            });
        }
    };

    var _renderTagElement = function(tag) {
        var $tagElem = jq("<a></a>").addClass("dropdown-item")
                        .text(ASC.CRM.Common.convertText(tag.title, false))
                        .bind("click", function() {
                            _addThisTag(this);
                        });
        jq("#addTagCasesDialog ul.dropdown-content").append(jq("<li></li>").append($tagElem));
    };

    var _renderAndInitTagsDialog = function() {
        for (var i = 0, n = ASC.CRM.Data.caseTags.length; i < n; i++) {
            _renderTagElement(ASC.CRM.Data.caseTags[i]);
        }
        jq.dropdownToggle({
            dropdownID: "addTagCasesDialog",
            switcherSelector: "#casesHeaderMenu .menuActionAddTag.unlockAction",
            addTop: 5,
            addLeft: 0,
            showFunction: function(switcherObj, dropdownItem) {
                jq("#addTagCasesDialog input.textEdit").val("");
            }
        });
    };

    var _initCaseActionMenu = function() {
        jq.dropdownToggle({
            dropdownID: "caseActionMenu",
            switcherSelector: "#caseTable .entity-menu",
            addTop: -2,
            addLeft: 10,
            rightPos: true,
            beforeShowFunction: function (switcherObj, dropdownItem) {
                var caseId = switcherObj.attr("id").split('_')[1];
                if (!caseId) { return; }
                _showActionMenu(parseInt(caseId));
            },
            showFunction: function(switcherObj, dropdownItem) {
                jq("#caseTable .entity-menu.active").removeClass("active");
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass("active");
                }
            },
            hideFunction: function() {
                jq("#caseTable .entity-menu.active").removeClass("active");
            }
        });


        jq("#caseTable").unbind("contextmenu").bind("contextmenu", function(event) {
            event.preventDefault();

            var e = ASC.CRM.Common.fixEvent(event),
                target = jq(e.srcElement || e.target),
                caseId = parseInt(target.closest("tr.with-entity-menu").attr("id").split('_')[1]);
            if (!caseId) {
                return false;
            }
            _showActionMenu(caseId);
            jq("#caseTable .entity-menu.active").removeClass("active");

            var $dropdownItem = jq("#caseActionMenu");
            $dropdownItem.show();
            var left = $dropdownItem.children(".corner-top").position().left;
            $dropdownItem.hide();
            if (target.is(".entity-menu")) {
                if ($dropdownItem.is(":hidden")) {
                    target.addClass('active');
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

    var _initScrolledGroupMenu = function() {
        ScrolledGroupMenu.init({
            menuSelector: "#casesHeaderMenu",
            menuAnchorSelector: "#mainSelectAllCases",
            menuSpacerSelector: "#caseList .header-menu-spacer",
            userFuncInTop: function() { jq("#casesHeaderMenu .menu-action-on-top").hide(); },
            userFuncNotInTop: function() { jq("#casesHeaderMenu .menu-action-on-top").show(); }
        });
    };

    var _initOtherActionMenu = function() {
        if (jq("#exportListToCSV").length == 1) {
            jq("#exportListToCSV").bind("click", _exportToCsv);
        }
        if (jq("#openListInEditor").length == 1) {
            jq("#openListInEditor").bind("click", _openExportFile);
        }

        jq("#menuCreateNewTask").bind("click", function() { ASC.CRM.TaskActionView.showTaskPanel(0, "", 0, null, {}); });
    };

    var _initConfirmationPannels = function() {
        jq.tmpl("blockUIPanelTemplate", {
            id: "deleteCasesPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: ASC.CRM.Resources.CRMCommonResource.ConfirmationDeleteText,
            innerHtmlText: ["<div id=\"deleteCasesList\" class=\"containerForListBatchDelete mobile-overflow\">",
                                        "<dl>",
                                            "<dt class=\"listForBatchDelete\">",
                                                ASC.CRM.Resources.CRMCommonResource.CasesModuleName,
                                                ":",
                                            "</dt>",
                                            "<dd class=\"listForBatchDelete\">",
                                            "</dd>",
                                        "</dl>",
                                    "</div>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMCasesResource.DeletingCases
        }).insertAfter("#caseList");

        jq("#deleteCasesPanel").on("click", ".crm-actionButtonsBlock .button.blue", function() {
            ASC.CRM.ListCasesView.deleteBatchCases();
        });

        jq.tmpl("blockUIPanelTemplate", {
            id: "setPermissionsCasesPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.SetPermissions,
            innerHtmlText: "",
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            OKBtnClass: "setPermissionsLink",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMCommonResource.SaveChangesProggress
        }).insertAfter("#caseList");

        jq("#permissionsCasesPanelInnerHtml").insertBefore("#setPermissionsCasesPanel .containerBodyBlock .crm-actionButtonsBlock").removeClass("display-none");
        
    };

    var _addThisTag = function(obj) {
        var params = {
            tagName  : jq(obj).text(),
            isNewTag : false
        };
        _addTag(params);
    };

    var _addTag = function(params) {
        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
        }
        params.selectedIDs = selectedIDs;

        Teamlab.addCrmTag(params, "case", params.selectedIDs, params.tagName,
        {
            success: callback_add_tag,
            before: function(params) {
                for (var i = 0, n = params.selectedIDs.length; i < n; i++) {
                    jq("#checkCase_" + params.selectedIDs[i]).hide();
                    jq("#loaderImg_" + params.selectedIDs[i]).show();
                }
            },
            after: function(params) {
                for (var i = 0, n = params.selectedIDs.length; i < n; i++) {
                    jq("#loaderImg_" + params.selectedIDs[i]).hide();
                    jq("#checkCase_" + params.selectedIDs[i]).show();
                }
            }
        });
    };

    var _showDeletePanel = function() {
        jq("#deleteCasesList dd.listForBatchDelete").html("");
        for (var i = 0, len = ASC.CRM.ListCasesView.selectedItems.length; i < len; i++) {

            var label = jq("<label></label>")
                            .attr("title", ASC.CRM.ListCasesView.selectedItems[i].title)
                            .text(ASC.CRM.ListCasesView.selectedItems[i].title);
            jq("#deleteCasesList dd.listForBatchDelete").append(
                            label.prepend(jq("<input>")
                            .attr("type", "checkbox")
                            .prop("checked", true)
                            .attr("id", "case_" + ASC.CRM.ListCasesView.selectedItems[i].id))
                        );

        }
        jq("#deleteCasesPanel .crm-actionButtonsBlock").show();
        jq("#deleteCasesPanel .crm-actionProcessInfoBlock").hide();
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#deleteCasesPanel", 500, 500, 0);
    };

    var _showSetPermissionsPanel = function(params) {
        if (jq("#setPermissionsCasesPanel div.tintMedium").length > 0) {
            jq("#setPermissionsCasesPanel div.tintMedium span.header-base").remove();
            jq("#setPermissionsCasesPanel div.tintMedium").removeClass("tintMedium").css("padding", "0px");
        }
        jq("#isPrivate").prop("checked", false);
        ASC.CRM.PrivatePanel.changeIsPrivateCheckBox();
        jq("#selectedUsers div.selectedUser[id^=selectedUser_]").remove();
        SelectedUsers.IDs = new Array();
        jq("#setPermissionsCasesPanel .crm-actionButtonsBlock").show();
        jq("#setPermissionsCasesPanel .crm-actionProcessInfoBlock").hide();
        jq("#setPermissionsCasesPanel .setPermissionsLink").unbind("click").bind("click", function() {
            _setPermissions(params);
        });
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#setPermissionsCasesPanel", 600, 500, 0);
    };

    var _setPermissions = function(params) {
        var selectedUsers = SelectedUsers.IDs;
        selectedUsers.push(SelectedUsers.CurrentUserID);

        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
        }

        var data = {
            casesid: selectedIDs,
            isPrivate: jq("#isPrivate").is(":checked"),
            accessList: selectedUsers
        };

        Teamlab.updateCrmCaseRights(params, data,
            {
                success: callback_update_case_rights,
                before: function() {
                    jq("#setPermissionsCasesPanel .crm-actionButtonsBlock").hide();
                    jq("#setPermissionsCasesPanel .crm-actionProcessInfoBlock").show();
                },
                after: function() {
                    jq("#setPermissionsCasesPanel .crm-actionProcessInfoBlock").hide();
                    jq("#setPermissionsCasesPanel .crm-actionButtonsBlock").show();
                }
            });
    };

    var _createShortCase = function(caseItem) {
        var shortCase = {
            id        : caseItem.id,
            isClosed  : caseItem.isClosed,
            isPrivate : caseItem.isPrivate,
            title     : caseItem.title
        };
        return shortCase;
    };

    var _exportToCsv = function() {
        var index = window.location.href.indexOf('#'),
            basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href,
            anchor = index >= 0 ? window.location.href.substr(index, window.location.href.length) : "";
        jq("#otherActions").hide();
        window.location.href = basePath + "?action=export" + anchor;
    };

    var _openExportFile = function() {
        var index = window.location.href.indexOf('#'),
            basePath = index >= 0 ? window.location.href.substr(0, index) : window.location.href;
        jq("#otherActions").hide();
        window.open(basePath + "?action=export&view=editor");
    };

    var _preInitPage = function (entryCountOnPage) {
        jq("#mainSelectAllCases").prop("checked", false);//'cause checkboxes save their state between refreshing the page

        jq("#tableForCasesNavigation select:first")
            .val(entryCountOnPage)
            .change(function () {
                ASC.CRM.ListCasesView.changeCountOfRows(this.value);
            })
            .tlCombobox();
    };

    var _initFilter = function () {
        if (!jq("#casesAdvansedFilter").advansedFilter) return;

        ASC.CRM.ListCasesView.advansedFilter = jq("#casesAdvansedFilter")
            .advansedFilter({
                anykey      : false,
                hint        : ASC.CRM.Resources.CRMCommonResource.AdvansedFilterInfoText.format(
                            '<b>',
                            '</b>',
                            '<br/><br/><a href="' + ASC.Resources.Master.FilterHelpCenterLink + '" target="_blank">',
                            '</a>'),
                maxfilters  : 3,
                maxlength   : "100",
                store       : true,
                inhash      : true,
                filters     : [
                            {
                                type        : "combobox",
                                id          : "opened",
                                apiparamname: "isClosed",
                                title       : ASC.CRM.Resources.CRMJSResource.CaseStatusOpened,
                                filtertitle : ASC.CRM.Resources.CRMCasesResource.CasesByStatus,
                                group       : ASC.CRM.Resources.CRMCasesResource.CasesByStatus,
                                groupby     : "caseStatus",
                                options     :
                                        [
                                        { value: false, classname: '', title: ASC.CRM.Resources.CRMJSResource.CaseStatusOpened, def: true },
                                        { value: true, classname: '', title: ASC.CRM.Resources.CRMJSResource.CaseStatusClosed }
                                        ]
                            },
                            {
                                type        : "combobox",
                                id          : "closed",
                                apiparamname: "isClosed",
                                title       : ASC.CRM.Resources.CRMJSResource.CaseStatusClosed,
                                filtertitle : ASC.CRM.Resources.CRMCasesResource.CasesByStatus,
                                group       : ASC.CRM.Resources.CRMCasesResource.CasesByStatus,
                                groupby     : "caseStatus",
                                options     :
                                        [
                                        { value: false, classname: '', title: ASC.CRM.Resources.CRMJSResource.CaseStatusOpened },
                                        { value: true, classname: '', title: ASC.CRM.Resources.CRMJSResource.CaseStatusClosed, def: true }
                                        ]
                            },
                            {
                                type        : "combobox",
                                id          : "tags",
                                apiparamname: "tags",
                                title       : ASC.CRM.Resources.CRMCommonResource.FilterWithTag,
                                group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                options     : ASC.CRM.Data.caseTags,
                                defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                enable      : ASC.CRM.Data.caseTags.length > 0,
                                multiple    : true
                            }
                            ],

                sorters     : [
                            { id: "title", title: ASC.CRM.Resources.CRMCommonResource.Title, dsc: false, def: true }
                            ]
            })
            .bind("setfilter", ASC.CRM.ListCasesView.setFilter)
            .bind("resetfilter", ASC.CRM.ListCasesView.resetFilter);
    };

    var _initEmptyScreen = function (emptyListImgSrc, emptyFilterListImgSrc) {
        //init emptyScreen for all list
        var buttonHtml = ["<a class='link underline blue plus' href='cases.aspx?action=manage'>",
            ASC.CRM.Resources.CRMCasesResource.CreateFirstCase,
            "</a>"].join('');
        
        if (jq.browser.mobile !== true) {
            buttonHtml += ["<br/><a class='crm-importLink' href='cases.aspx?action=import'>",
                ASC.CRM.Resources.CRMCasesResource.ImportCases,
                "</a>"].join('');
        }

        jq.tmpl("emptyScrTmpl",
            {
                ID: "casesEmptyScreen",
                ImgSrc: emptyListImgSrc,
                Header: ASC.CRM.Resources.CRMCasesResource.EmptyContentCasesHeader,
                Describe: ASC.CRM.Resources.CRMCasesResource.EmptyContentCasesDescribe,
                ButtonHTML: buttonHtml
            }).insertAfter("#caseList");

        //init emptyScreen for filter
        jq.tmpl("emptyScrTmpl",
            {
                ID: "emptyContentForCasesFilter",
                ImgSrc: emptyFilterListImgSrc,
                Header: ASC.CRM.Resources.CRMCasesResource.EmptyContentCasesFilterHeader,
                Describe: ASC.CRM.Resources.CRMCasesResource.EmptyContentCasesFilterDescribe,
                ButtonHTML: ["<a class='crm-clearFilterButton' href='javascript:void(0);'",
                    "onclick='ASC.CRM.ListCasesView.advansedFilter.advansedFilter(null);'>",
                    ASC.CRM.Resources.CRMCommonResource.ClearFilter,
                    "</a>"].join('')
            }).insertAfter("#caseList");
    };

    return {
        casesList       : new Array(),
        selectedItems   : new Array(),

        //isFirstTime     : true,
        isFilterVisible : false,

        currentUserIsAdmin : false,

        entryCountOnPage   : 0,
        defaultCurrentPageNumber : 0,
        noCases            : false,
        noCasesForQuery    : false,
        cookieKey          : "",

        init: function (visiblePageCount, cookieKey, anchor, isAdmin, emptyListImgSrc, emptyFilterListImgSrc) {
            ASC.CRM.ListCasesView.cookieKey = cookieKey;
            ASC.CRM.ListCasesView.currentUserIsAdmin = isAdmin;

            var settings = {
                page: 1,
                countOnPage: jq("#tableForCasesNavigation select:first>option:first").val()
            },
                key = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + location.pathname + location.search,
                currentAnchor = location.hash,
                cookieKey = encodeURIComponent(key);

            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#'
                ? currentAnchor.substring(1)
                : currentAnchor;

            var cookieAnchor = jq.cookies.get(cookieKey);
            if (currentAnchor == "" || cookieAnchor == currentAnchor) {
                var tmp = ASC.CRM.Common.getPagingParamsFromCookie(ASC.CRM.ListCasesView.cookieKey);
                if (tmp != null) {
                    settings = tmp;
                }
            } else {
                _setCookie(settings.page, settings.countOnPage);
            }

            ASC.CRM.ListCasesView.entryCountOnPage = settings.countOnPage;
            ASC.CRM.ListCasesView.defaultCurrentPageNumber = settings.page;

            _preInitPage(ASC.CRM.ListCasesView.entryCountOnPage);
            _initEmptyScreen(emptyListImgSrc, emptyFilterListImgSrc);

            _initPageNavigatorControl(ASC.CRM.ListCasesView.entryCountOnPage, ASC.CRM.ListCasesView.defaultCurrentPageNumber, visiblePageCount);

            _renderAndInitTagsDialog();

            _initCaseActionMenu();

            _initScrolledGroupMenu();

            _initOtherActionMenu();

            ASC.CRM.ListCasesView.initConfirmationPanelForDelete();

            _initConfirmationPannels();

            _initFilter();
            /*tracking events*/

            ASC.CRM.ListCasesView.advansedFilter.one("adv-ready", function () {
                var crmAdvansedFilterContainer = jq("#casesAdvansedFilter .advansed-filter-list");
                crmAdvansedFilterContainer.find("li[data-id='opened'] .inner-text").trackEvent(ga_Categories.cases, ga_Actions.filterClick, 'opened_status');
                crmAdvansedFilterContainer.find("li[data-id='closed'] .inner-text").trackEvent(ga_Categories.cases, ga_Actions.filterClick, 'closed_status');
                crmAdvansedFilterContainer.find("li[data-id='tags'] .inner-text").trackEvent(ga_Categories.cases, ga_Actions.filterClick, 'with_tags');

                jq("#casesAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.cases, ga_Actions.filterClick, "sort");
                jq("#casesAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.cases, ga_Actions.filterClick, "search_text", "enter");
            });
        },

        setFilter: function(evt, $container, filter, params, selectedfilters) { _changeFilter(); },
        resetFilter: function(evt, $container, filter, selectedfilters) { _changeFilter(); },

        selectAll: function(obj) {
            var isChecked = jq(obj).is(":checked"),
                selectedIDs = new Array();

            for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
            }

            for (var i = 0, len = ASC.CRM.ListCasesView.casesList.length; i < len; i++) {
                var caseItem = ASC.CRM.ListCasesView.casesList[i],
                    index = jq.inArray(caseItem.id, selectedIDs);
                if (isChecked && index == -1) {
                    ASC.CRM.ListCasesView.selectedItems.push(_createShortCase(caseItem));
                    selectedIDs.push(caseItem.id);
                    jq("#caseItem_" + caseItem.id).addClass("selectedRow");
                    jq("#checkCase_" + caseItem.id).prop("checked", true);
                }
                if (!isChecked && index != -1) {
                    ASC.CRM.ListCasesView.selectedItems.splice(index, 1);
                    selectedIDs.splice(index, 1);
                    jq("#caseItem_" + caseItem.id).removeClass("selectedRow");
                    jq("#checkCase_" + caseItem.id).prop("checked", false);
                }
            }
            _renderCheckedCasesCount(ASC.CRM.ListCasesView.selectedItems.length);
            _checkForLockMainActions();
        },

        selectItem: function(obj) {
            var id = parseInt(jq(obj).attr("id").split("_")[1]),
                selectedCase = null,
                selectedIDs = new Array();

            for (var i = 0, n = ASC.CRM.ListCasesView.casesList.length; i < n; i++) {
                if (id == ASC.CRM.ListCasesView.casesList[i].id) {
                    selectedCase = _createShortCase(ASC.CRM.ListCasesView.casesList[i]);
                }
            }

            for (var i = 0, n = ASC.CRM.ListCasesView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListCasesView.selectedItems[i].id);
            }

            var index = jq.inArray(id, selectedIDs);

            if (jq(obj).is(":checked")) {
                jq(obj).addClass("selectedRow");
                if (index == -1) {
                    ASC.CRM.ListCasesView.selectedItems.push(selectedCase);
                }
                ASC.CRM.ListCasesView.checkFullSelection();
            } else {
                jq("#mainSelectAllCases").prop("checked", false);
                jq(obj).removeClass("selectedRow");
                if (index != -1) {
                    ASC.CRM.ListCasesView.selectedItems.splice(index, 1);
                }
            }
            _renderCheckedCasesCount(ASC.CRM.ListCasesView.selectedItems.length);
            _checkForLockMainActions();
        },

        deselectAll: function() {
            ASC.CRM.ListCasesView.selectedItems = new Array();
            _renderCheckedCasesCount(0);
            jq("#caseTable input:checkbox").prop("checked", false);
            jq("#mainSelectAllCases").prop("checked", false);
            jq("#caseTable tr.selectedRow").removeClass("selectedRow");
            _lockMainActions();
        },

        checkFullSelection: function() {
            var rowsCount = jq("#caseTable tbody tr").length,
                selectedRowsCount = jq("#caseTable input[id^=checkCase_]:checked").length;
            jq("#mainSelectAllCases").prop("checked", rowsCount == selectedRowsCount);
        },

        deleteBatchCases: function() {
            var ids = new Array();
            jq("#deleteCasesPanel input:checked").each(function() {
                ids.push(parseInt(jq(this).attr("id").split("_")[1]));
            });
            var params = { casesIDsForDelete: ids };

            Teamlab.removeCrmCase(params, ids,
                {
                    success: callback_delete_batch_cases,
                    before: function(params) {
                        jq("#deleteCasesPanel .crm-actionButtonsBlock").hide();
                        jq("#deleteCasesPanel .crm-actionProcessInfoBlock").show();
                    },
                    after: function(params) {
                        jq("#deleteCasesPanel .crm-actionProcessInfoBlock").hide();
                        jq("#deleteCasesPanel .crm-actionButtonsBlock").show();
                    }
                });
        },

        initConfirmationPanelForDelete: function () {
            jq.tmpl("blockUIPanelTemplate", {
                id: "confirmationDeleteOneCasePanel",
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
                progressText: ASC.CRM.Resources.CRMJSResource.DeleteCaseInProgress
            }).appendTo("#studioPageContent .mainPageContent .containerBodyBlock:first");
        },

        showConfirmationPanelForDelete: function(title, caseID, isListView) {
            jq("#confirmationDeleteOneCasePanel .confirmationAction>b").text(jq.format(ASC.CRM.Resources.CRMJSResource.DeleteCaseConfirmMessage, Encoder.htmlDecode(title)));

            jq("#confirmationDeleteOneCasePanel .crm-actionButtonsBlock>.button.blue.middle").unbind("click").bind("click", function() {
                ASC.CRM.ListCasesView.deleteCase(caseID, isListView);
            });
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#confirmationDeleteOneCasePanel", 500, 500, 0);
        },

        deleteCase: function(caseID, isListView) {
            if (isListView === true) {
                var ids = new Array();
                ids.push(caseID);
                var params = { casesIDsForDelete: ids };
                Teamlab.removeCrmCase(params, ids, callback_delete_batch_cases);
            } else {
                Teamlab.removeCrmCase({}, caseID,
                    {
                        before: function() {
                            jq("#confirmationDeleteOneCasePanel .crm-actionButtonsBlock").hide();
                            jq("#confirmationDeleteOneCasePanel .crm-actionProcessInfoBlock").show();

                            jq("#crm_caseMakerDialog input, #crm_caseMakerDialog select, #crm_caseMakerDialog textarea").attr("disabled", true);
                            jq("#crm_caseMakerDialog .crm-actionProcessInfoBlock span").text(ASC.CRM.Resources.CRMJSResource.DeleteCaseInProgress);
                            jq("#crm_caseMakerDialog .crm-actionButtonsBlock").hide();
                            jq("#crm_caseMakerDialog .crm-actionProcessInfoBlock").show();
                        },
                        success: function() {
                            location.href = "cases.aspx";
                        }
                    });
            }
        },

        addNewTag: function() {
            var newTag = jq("#addTagCasesDialog input").val().trim();
            if (newTag == "") {
                return false;
            }

            var params = {
                tagName: newTag,
                isNewTag: true
            };
            _addTag(params);
        },

        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) {
                return;
            }
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListCasesView.entryCountOnPage = newCountOfRows;
            casesPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);
            _renderContent(0);
        }
    };
})();


ASC.CRM.CasesActionView = (function() {

    var initFields = function(dateMask) {
        if (typeof (window.casesEditCustomFieldList) != "undefined" && window.casesEditCustomFieldList.length != 0) {
            ASC.CRM.Common.renderCustomFields(window.casesEditCustomFieldList, "custom_field_", "customFieldRowTmpl", "#crm_caseMakerDialog dl.customFieldsContainer");
        }
        jq.registerHeaderToggleClick("#crm_caseMakerDialog .case_info", "dt.headerToggleBlock");
        jq("#crm_caseMakerDialog .case_info dt.headerToggleBlock").each(
                function() {
                    jq(this).nextUntil("dt.headerToggleBlock").hide();
                });

        jq("input.textEditCalendar").mask(dateMask);
        jq("input.textEditCalendar").datepickerWithButton();
    };

    var initOtherActionMenu = function() {
        ASC.CRM.Common.removeExportButtons();
        jq("#menuCreateNewTask").bind("click", function() { ASC.CRM.TaskActionView.showTaskPanel(0, "", 0, null, {}); });
    };

    var initConfirmationGotoSettingsPanel = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "confirmationGotoSettingsPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: "",
            innerHtmlText:
            ["<div class=\"confirmationNote\">",
                ASC.CRM.Resources.CRMJSResource.ConfirmGoToCustomFieldPage,
            "</div>"].join(''),
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            OKBtnHref: "settings.aspx?type=custom_field&view=case",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ""
        }).insertAfter("#otherCasesCustomFieldPanel");
    };

    var initCasesMembersSelector = function (_selectorType) {
        if (window.casesActionSelectedContacts == "") {
            var casesContactSelectorReady = function (event, objName) {
                if (objName == "casesContactSelector") {
                    jq("#selector_" + window.casesContactSelector.ObjName).children("div:first").children("div[id^='item_']").remove();
                    jq("#membersCasesSelectorsContainer").prev('dt').addClass("crm-withGrayPlus");
                    jq(window).unbind("contactSelectorIsReady", casesContactSelectorReady);
                }
            };
            jq(window).bind("contactSelectorIsReady", casesContactSelectorReady);
        }

        window["casesContactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("casesContactSelector",
        {
            SelectorType: _selectorType,
            EntityType: 0,
            EntityID: 0,
            ShowOnlySelectorContent: false,
            DescriptionText: ASC.CRM.Resources.CRMCommonResource.FindContactByName,
            DeleteContactText: ASC.CRM.Resources.CRMCommonResource.DeleteParticipant,
            AddContactText: ASC.CRM.Resources.CRMCommonResource.AddParticipant,
            IsInPopup: false,
            NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
            NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
            NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
            ShowChangeButton: false,
            ShowChangeButton: true,
            ShowAddButton: true,
            ShowDeleteButton: true,
            ShowContactImg: true,
            ShowNewCompanyContent: true,
            ShowNewContactContent: true,
            HTMLParent: "#membersCasesSelectorsContainer",
            ExcludedArrayIDs: [],
            presetSelectedContactsJson: window.casesActionSelectedContacts
        });

        jq(window).bind("deleteContactFromSelector", function (event, $itemObj, objName) {
            if (jq("#selector_" + window.casesContactSelector.ObjName).children("div:first").children("div[id^='item_']").length == 1) {
                jq("#membersCasesSelectorsContainer").prev('dt').addClass("crm-withGrayPlus");
            }
        });
    };

    return {
        init: function (dateMask, _selectorType, isAdmin) {
            initFields(dateMask);
            initOtherActionMenu();
            ASC.CRM.ListCasesView.initConfirmationPanelForDelete();
            if (isAdmin === true) {
                initConfirmationGotoSettingsPanel();
            }
            jq.tmpl("tagViewTmpl",
                        {
                            tags          : window.casesActionTags,
                            availableTags : window.casesActionAvailableTags
                        })
                        .appendTo("#tagsContainer>div:first");
            ASC.CRM.TagView.init("cases", true);
            if (window.casesActionTags.length > 0) {
                jq("#tagsContainer>div:first").removeClass("display-none");
                jq("#tagsContainer").prev().removeClass("crm-withGrayPlus");
            }

            initCasesMembersSelector(_selectorType);

            jq("#crm_caseMakerDialog").on("click", ".crm-withGrayPlus", function (event) {
                var container_id = jq(this).next('dd').attr('id');
                if (container_id == "membersCasesSelectorsContainer") {
                    window.casesContactSelector.AddNewSelector(jq(this));
                } else if (container_id == "tagsContainer") {
                    jq("#tagsContainer > div:first").removeClass("display-none");
                }
                jq(this).removeClass("crm-withGrayPlus");
            });

            var caseID = parseInt(jq.getURLParam("id"));
            if (!isNaN(caseID) && jq("#deleteCaseButton").length == 1) {
                var caseTitle = jq("#deleteCaseButton").attr("caseTitle");
                jq("#deleteCaseButton").unbind("click").bind("click", function() {
                    ASC.CRM.ListCasesView.showConfirmationPanelForDelete(caseTitle, caseID, false);
                });
            }
        },

        submitForm: function() {
            try {
                var title = jq("#caseTitle").val().trim();
                if (title == "") {
                    AddRequiredErrorText(jq("#caseTitle"), ASC.CRM.Resources.CRMJSResource.ErrorEmptyCaseTitle);
                    ShowRequiredError(jq("#caseTitle"));
                    return false;
                }

                jq("#crm_caseMakerDialog .crm-actionButtonsBlock").hide();
                jq("#crm_caseMakerDialog .crm-actionProcessInfoBlock").show();

                jq("#membersCasesSelectorsContainer input[name=memberID]").val(window.casesContactSelector.SelectedContacts.join(","));

                if (!jq("#isPrivate").is(":checked")) {
                    window.SelectedUsers.IDs = new Array();
                    jq("#notifyPrivate").removeAttr("checked");
                }

                jq("#isPrivateCase").val(jq("#isPrivate").is(":checked"));
                jq("#notifyPrivateUsers").val(jq("#cbxNotify").is(":checked"));
                jq("#selectedUsersCase").val(window.SelectedUsers.IDs.join(","));


                ASC.CRM.TagView.prepareTagDataForSubmitForm(jq("#crm_caseMakerDialog input[name='baseInfo_assignedTags']"));

                var $checkboxes = jq("#crm_caseMakerDialog input[type='checkbox'][id^='custom_field_']");
                if ($checkboxes) {
                    for (var i = 0, n = $checkboxes.length; i < n; i++) {
                        if (jq($checkboxes[i]).is(":checked")) {
                            var id = $checkboxes[i].id.replace('custom_field_', '');
                            jq("#crm_caseMakerDialog input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
                        }
                    }
                }
                return true;
            } catch (e) {
                console.log(e);
                return false;
            }
        },

        showGotoAddSettingsPanel: function () {
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#confirmationGotoSettingsPanel", 500, 500, 0);
        }
    };
})();


ASC.CRM.CasesFullCardView = (function() {

    var renderCustomFields = function() {
        if (typeof (window.casesCustomFieldList) != "undefined" && window.casesCustomFieldList.length != 0) {
            var sortedList = ASC.CRM.Common.sortCustomFieldList(window.casesCustomFieldList);
            jq.tmpl("customFieldListTmpl", sortedList).insertBefore("#caseHistoryTable");
        }
    };

    var callback_change_case_status = function (params, caseitem) {
        jq("#caseDetailsMenuPanel").hide();
        jq(".mainContainerClass .containerHeaderBlock .menu-small.active").removeClass("active");

        jq("#caseStatusSwitcher").text(params.isClosed === true
                ? ASC.CRM.Resources.CRMJSResource.CaseStatusClosed
                : ASC.CRM.Resources.CRMJSResource.CaseStatusOpened);
        if (params.isClosed === true) {
            jq("#closeCase").hide();
            jq("#openCase").show();
        } else {
            jq("#openCase").hide();
            jq("#closeCase").show();
        }
    };

    return {
        init: function() {
            jq.tmpl("tagViewTmpl",
                    {
                        tags         : window.caseTags,
                        availableTags: window.caseAvailableTags
                    })
                    .appendTo("#caseTagsTD");
            ASC.CRM.TagView.init("case", false);

            if (typeof (window.caseResponsibleIDs) != "undefined" && window.caseResponsibleIDs.length != 0) {
                jq("#caseProfile .caseAccessList").html(ASC.CRM.Common.getAccessListHtml(window.caseResponsibleIDs));
            }
            renderCustomFields();

            jq.registerHeaderToggleClick("#caseProfile .crm-detailsTable", "tr.headerToggleBlock");
            jq("#caseHistoryTable .headerToggle").bind("click", function() {
                ASC.CRM.HistoryView.activate();
            });

            jq("#caseProfile .crm-detailsTable .headerToggle").not("#caseHistoryTable .headerToggle").each(
                function() {
                    jq(this).parents("tr.headerToggleBlock:first").nextUntil(".headerToggleBlock").hide();
                });
        },

        changeCaseStatus: function(isClosed) {
            var caseID = parseInt(jq.getURLParam("id")),
                data = {
                    caseid   : caseID,
                    isClosed : isClosed == 1
                };
            Teamlab.updateCrmCase(data, caseID, data, callback_change_case_status);
        }
    };
})();


ASC.CRM.CasesDetailsView = (function() {

    var initTabs = function () {
        window.ASC.Controls.ClientTabsNavigator.init("CaseTabs", {
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
                title: ASC.CRM.Resources.CRMCasesResource.PeopleInCase,
                selected: false,
                anchor: "contacts",
                divID: "contactsTab"
            },
            {
                title: ASC.CRM.Resources.CRMCommonResource.Documents,
                selected: false,
                anchor: "files",
                divID: "filesTab"
            }]
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

        ASC.Controls.AnchorController.bind(/files/, window.Attachments.loadFiles);
    };

    var initAttachments = function () {
        window.Attachments.init();
        window.Attachments.bind("addFile", function(ev, file) {
            //ASC.CRM.Common.changeCountInTab("add", "files");
            var caseID = jq.getURLParam("id") * 1;
            var type = "case";
            var fileids = [];
            fileids.push(file.id);

            Teamlab.addCrmEntityFiles({}, caseID, type, {
                entityid   : caseID,
                entityType : type,
                fileids    : fileids
            },
            {
                success: function(params, data) {
                    window.Attachments.appendFilesToLayout(data.files);
                    params.fromAttachmentsControl = true;
                    ASC.CRM.HistoryView.addEventToHistoryLayout(params, data);
                }
            });
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
                        //ASC.CRM.Common.changeCountInTab("delete", "files");
                    }
                });
            }
        });
    };

    var initCaseDetailsMenuPanel = function() {
        jq(document).ready(function() {
            jq.dropdownToggle({
                dropdownID: "caseDetailsMenuPanel",
                switcherSelector: ".mainContainerClass .containerHeaderBlock .menu-small",
                addTop: -2,
                addLeft: -10,
                showFunction: function(switcherObj, dropdownItem) {
                    if (dropdownItem.is(":hidden")) {
                        switcherObj.addClass("active");
                    } else {
                        switcherObj.removeClass("active");
                    }
                },
                hideFunction: function() {
                    jq(".mainContainerClass .containerHeaderBlock .menu-small.active").removeClass("active");
                }
            });
        });
    };

    var initOtherActionMenu = function() {
        ASC.CRM.Common.removeExportButtons();

        var params = {};
        if (window.caseResponsibleIDs.length != 0) {
            params.taskResponsibleSelectorUserIDs = window.caseResponsibleIDs;
        }
        jq("#menuCreateNewTask").bind("click", function() {
            ASC.CRM.TaskActionView.showTaskPanel(0, window.entityData.type, window.entityData.id, null, params);
        });
        ASC.CRM.ListTaskView.bindEmptyScrBtnEvent(params);
    };

    var initEmptyScreens = function(imgSrcEmptyParticipants) {
        jq.tmpl("emptyScrTmpl",
                { ID: "emptyCaseParticipantPanel",
                    ImgSrc: imgSrcEmptyParticipants,
                    Header: ASC.CRM.Resources.CRMCasesResource.EmptyPeopleInCaseContent,
                    Describe: ASC.CRM.Resources.CRMCasesResource.EmptyPeopleInCaseDescript,
                    ButtonHTML: ["<a class='link dotline blue plus' ",
                            "onclick='javascript:jq(\"#caseParticipantPanel\").show();jq(\"#emptyCaseParticipantPanel\").addClass(\"display-none\");'>",
                            ASC.CRM.Resources.CRMCommonResource.AddParticipant,
                            "</a>"
                            ].join(''),
                    CssClass: "display-none"
                }).insertAfter("#contactListBox");
    };

    var initParticipantsContactSelector = function() {
        window["casesContactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("casesContactSelector",
                {
                    SelectorType: -1,
                    EntityType: 0,
                    EntityID: 0,
                    ShowOnlySelectorContent: true,
                    DescriptionText: ASC.CRM.Resources.CRMCommonResource.FindContactByName,
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
                    ShowNewCompanyContent: true,
                    ShowNewContactContent: true,
                    presetSelectedContactsJson: '',
                    ExcludedArrayIDs: [],
                    HTMLParent: "#caseParticipantPanel"
                });
        window.casesContactSelector.SelectItemEvent = ASC.CRM.CasesDetailsView.addMemberToCase;
        ASC.CRM.ListContactView.removeMember = ASC.CRM.CasesDetailsView.removeMemberFromCase;

        jq(window).bind("getContactsFromApi", function(event, contacts) {
            var contactLength = contacts.length;
            if (contactLength == 0) {
                jq("#emptyCaseParticipantPanel.display-none").removeClass("display-none");
            } else {
                jq("#caseParticipantPanel").show();
                var contactIDs = [];
                for (var i = 0; i < contactLength; i++) {
                    contactIDs.push(contacts[i].id);
                }
                casesContactSelector.SelectedContacts = contactIDs;
            }
        });
    };

    return {
        init: function(imgSrcEmptyParticipants) {
            initTabs();
            initEmptyScreens(imgSrcEmptyParticipants);
            initParticipantsContactSelector();

            initAnchorLinking();
            initAttachments();
            initCaseDetailsMenuPanel();
            initOtherActionMenu();
        },

        removeMemberFromCase: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: parseInt(id) }, window.entityData.type, window.entityData.id, id, {
                before: function(params) {
                    jq("#simpleContactActionMenu").hide();
                    jq("#contactTable .entity-menu.active").removeClass("active");
                },
                after: function(params) {
                    var index = jq.inArray(params.contactID, window.casesContactSelector.SelectedContacts);
                    if (index != -1) {
                        window.casesContactSelector.SelectedContacts.splice(index, 1);
                    } else {
                        console.log("Can't find such contact in list");
                    }
                    ASC.CRM.ContactSelector.Cache = {};

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    ASC.CRM.HistoryView.removeOptionFromContact(params.contactID);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");

                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (window.casesContactSelector.SelectedContacts.length == 0) {
                            jq("#caseParticipantPanel").hide();
                            jq("#emptyCaseParticipantPanel.display-none").removeClass("display-none");
                        }
                    }, 500);
                }
            });
        },

        addMemberToCase: function(obj, params) {
            if (jq("#contactItem_" + obj.id).length > 0) {
                return false;
            }
            var data =
            {
                contactid : obj.id,
                caseid    : entityData.id
            };
            Teamlab.addCrmEntityMember({
                                            showCompanyLink : true,
                                            showUnlinkBtn: false,
                                            showActionMenu: true
                                        },
                                        entityData.type, entityData.id, obj.id, data, {
                    success: function(par, contact) {
                        ASC.CRM.ListContactView.CallbackMethods.addMember(par, contact);

                        window.casesContactSelector.SelectedContacts.push(contact.id);
                        //ASC.CRM.ContactSelector.Cache = {};

                        jq("#emptyCaseParticipantPanel:not(.display-none)").addClass("display-none");
                        ASC.CRM.HistoryView.appendOptionToContact({ value: contact.id, title: contact.displayName });
                    }
                });
        }
    };
})();
