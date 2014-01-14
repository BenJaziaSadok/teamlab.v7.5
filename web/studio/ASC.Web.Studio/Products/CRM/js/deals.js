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

ASC.CRM.myFilter = {
    filterId: 'dealsAdvansedFilter',
    idFilterByContact: 'contactID',
    idFilterByParticipant: 'participantID',

    type: 'custom-contact',
    hiddenContainerId: 'hiddenBlockForContactSelector',
    containerId: 'selector_contactSelectorForFilter',

    onSelectContact: function(item, params) {
        var input = jq("#contactTitle_" + window.contactSelectorForFilter.ObjName + "_0");
        jq("#infoContent_" + window.contactSelectorForFilter.ObjName + "_0").show();
        jq("#selectorContent_" + window.contactSelectorForFilter.ObjName + "_0").hide();
        window.contactSelectorForFilter.setContact(input, item.id, item.displayName, item.smallFotoUrl);
        window.contactSelectorForFilter.showInfoContent(input);
        window.contactSelectorForFilter.SelectedContacts = [];
        window.contactSelectorForFilter.SelectedContacts.push(item.id);
        //ASC.CRM.ContactSelector.Cache = {};
        if (ASC.CRM.myFilter.filterId) {
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter(ASC.CRM.myFilter.idFilterByContact, { id: item.id, displayName: item.displayName, smallFotoUrl: item.smallFotoUrl, value: jq.toJSON([item.id, false]) });
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
        }
    },

    onSelectParticipant: function(item, params) {
        var input = jq("#contactTitle_" + window.contactSelectorForFilter.ObjName + "_0");
        jq("#infoContent_" + window.contactSelectorForFilter.ObjName + "_0").show();
        jq("#selectorContent_" + window.contactSelectorForFilter.ObjName + "_0").hide();
        window.contactSelectorForFilter.setContact(input, item.id, item.displayName, item.smallFotoUrl);
        window.contactSelectorForFilter.showInfoContent(input);
        window.contactSelectorForFilter.SelectedContacts = [];
        window.contactSelectorForFilter.SelectedContacts.push(item.id);
        //ASC.CRM.ContactSelector.Cache = {};
        if (ASC.CRM.myFilter.filterId) {
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter(ASC.CRM.myFilter.idFilterByParticipant, { id: item.id, displayName: item.displayName, smallFotoUrl: item.smallFotoUrl, value: jq.toJSON([item.id, true]) });
            jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
        }
    },


    createFilterByContact: function(filter) {
        var o = document.createElement('div');
        o.innerHTML = [
      '<div class="default-value">',
        '<span class="title">',
          filter.title,
        '</span>',
        '<span class="selector-wrapper">',
          '<span class="contact-selector"></span>',
        '</span>',
        '<span class="btn-delete"></span>',
      '</div>'
    ].join('');
        return o;
    },
    customizeFilterByContact: function($container, $filteritem, filter) {
        window.contactSelectorForFilter.SelectItemEvent = ASC.CRM.myFilter.onSelectContact;

        if (ASC.CRM.myFilter.containerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo($filteritem.find('span.contact-selector:first'));
        }
    },
    destroyFilterByContact: function($container, $filteritem, filter) {
        if (ASC.CRM.myFilter.containerId && ASC.CRM.myFilter.hiddenContainerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo(jq('#' + ASC.CRM.myFilter.hiddenContainerId));
            window.contactSelectorForFilter.changeContact('contactSelectorForFilter_0');
        }
    },


    createFilterByParticipant: function(filter) {
        var o = document.createElement('div');
        o.innerHTML = [
      '<div class="default-value">',
        '<span class="title">',
          filter.title,
        '</span>',
        '<span class="selector-wrapper">',
          '<span class="contact-selector"></span>',
        '</span>',
        '<span class="btn-delete"></span>',
      '</div>'
    ].join('');
        return o;
    },
    customizeFilterByParticipant: function($container, $filteritem, filter) {
        window.contactSelectorForFilter.SelectItemEvent = ASC.CRM.myFilter.onSelectParticipant;

        if (ASC.CRM.myFilter.containerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo($filteritem.find('span.contact-selector:first'));
        }
    },
    destroyFilterByParticipant: function($container, $filteritem, filter) {
        if (ASC.CRM.myFilter.containerId && ASC.CRM.myFilter.hiddenContainerId) {
            jq('#' + ASC.CRM.myFilter.containerId).appendTo(jq('#' + ASC.CRM.myFilter.hiddenContainerId));
            window.contactSelectorForFilter.changeContact('contactSelectorForFilter_0');
        }
    },

    processFilter: function($container, $filteritem, filtervalue, params) {
        if (params && params.id && isFinite(params.id)) {
            var input = jq("#contactTitle_" + window.contactSelectorForFilter.ObjName + "_0");
            window.contactSelectorForFilter.setContact(input, params.id,
                                params.hasOwnProperty("displayName") ? params.displayName : "",
                                params.hasOwnProperty("smallFotoUrl") ? params.smallFotoUrl : "");
            window.contactSelectorForFilter.showInfoContent(input);
            window.contactSelectorForFilter.SelectedContacts = [];
            window.contactSelectorForFilter.SelectedContacts.push(params.id);
        }
    }
};

ASC.CRM.ListDealView = (function() {

    //Teamlab.bind(Teamlab.events.getException, onGetException);

    function onGetException(params, errors) {
        console.log('deals.js ', errors);
        LoadingBanner.hideLoading();
    };

    var _setCookie = function(page, countOnPage) {
        if (ASC.CRM.ListDealView.cookieKey && ASC.CRM.ListDealView.cookieKey != "") {
            var cookie = {
                page: page,
                countOnPage: countOnPage
            };
            jq.cookies.set(ASC.CRM.ListDealView.cookieKey, cookie, { path: location.pathname });
        }
    };

    var _getDeals = function(startIndex) {
        var filters = ASC.CRM.ListDealView.getFilterSettings(startIndex);

        //EventTracker.Track('crm_search_opportunities_by_filter');

        Teamlab.getCrmOpportunities({ startIndex: startIndex || 0 }, { filter: filters, success: callback_get_opportunities_by_filter });
    };

    var _resizeFilter = function() {
        var visible = jq("#dealFilterContainer").is(":hidden") == false;
        if (ASC.CRM.ListDealView.isFilterVisible == false && visible) {
            ASC.CRM.ListDealView.isFilterVisible = true;
            if (ASC.CRM.ListDealView.advansedFilter)
                jq("#dealsAdvansedFilter").advansedFilter("resize");
        }
    };

    var _changeFilter = function() {
        ASC.CRM.ListDealView.deselectAll();

        var defaultStartIndex = 0;
        if (ASC.CRM.ListDealView.defaultCurrentPageNumber != 0) {
            _setCookie(ASC.CRM.ListDealView.defaultCurrentPageNumber, window.dealPageNavigator.EntryCountOnPage);
            defaultStartIndex = (ASC.CRM.ListDealView.defaultCurrentPageNumber - 1) * window.dealPageNavigator.EntryCountOnPage;
            ASC.CRM.ListDealView.defaultCurrentPageNumber = 0;
        } else {
            _setCookie(0, window.dealPageNavigator.EntryCountOnPage);
        }
        _renderContent(0 || defaultStartIndex);
    };

    var _renderContent = function(startIndex) {
        ASC.CRM.ListDealView.dealList = new Array();
        ASC.CRM.ListDealView.bidList = new Array();

        LoadingBanner.displayLoading();
        jq("#mainSelectAllDeals").prop("checked", false);

        _getDeals(startIndex);
    };

    var _initPageNavigatorControl = function (countOfRows, currentPageNumber, visiblePageCount) {
        window.dealPageNavigator = new ASC.Controls.PageNavigator.init("dealPageNavigator", "#divForDealPager", countOfRows, visiblePageCount, currentPageNumber,
                                                                        ASC.CRM.Resources.CRMJSResource.Previous, ASC.CRM.Resources.CRMJSResource.Next);

        window.dealPageNavigator.changePageCallback = function(page) {
            _setCookie(page, window.dealPageNavigator.EntryCountOnPage);

            var startIndex = window.dealPageNavigator.EntryCountOnPage * (page - 1);
            _renderContent(startIndex);
        };
    };

    var _renderDealPageNavigator = function(startIndex) {
        var tmpTotal;
        if (startIndex >= ASC.CRM.ListDealView.Total) {
            tmpTotal = startIndex + 1;
        } else {
            tmpTotal = ASC.CRM.ListDealView.Total;
        }
        window.dealPageNavigator.drawPageNavigator((startIndex / ASC.CRM.ListDealView.entryCountOnPage).toFixed(0) * 1 + 1, tmpTotal);
    };

    var _renderSimpleDealsPageNavigator = function() {
        jq("#dealHeaderMenu .menu-action-simple-pagenav").html("");
        var $simplePN = jq("<div></div>"),
            lengthOfLinks = 0;
        if (jq("#divForDealPager .pagerPrevButtonCSSClass").length != 0) {
            lengthOfLinks++;
            jq("#divForDealPager .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
        }
        if (jq("#divForDealPager .pagerNextButtonCSSClass").length != 0) {
            lengthOfLinks++;
            if (lengthOfLinks === 2) {
                jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
            }
            jq("#divForDealPager .pagerNextButtonCSSClass").clone().appendTo($simplePN);
        }
        if ($simplePN.children().length != 0) {
            $simplePN.appendTo("#dealHeaderMenu .menu-action-simple-pagenav");
            jq("#dealHeaderMenu .menu-action-simple-pagenav").show();
        } else {
            jq("#dealHeaderMenu .menu-action-simple-pagenav").hide();
        }
    };

    var _renderCheckedDealsCount = function(count) {
        if (count != 0) {
            jq("#dealHeaderMenu .menu-action-checked-count > span").text(jq.format(ASC.CRM.Resources.CRMJSResource.ElementsSelectedCount, count));
            jq("#dealHeaderMenu .menu-action-checked-count").show();
        } else {
            jq("#dealHeaderMenu .menu-action-checked-count > span").text("");
            jq("#dealHeaderMenu .menu-action-checked-count").hide();
        }
    };

    var _renderNoDealsEmptyScreen = function() {
        jq("#dealTable tbody tr").remove();
        jq("#dealList").hide();
        jq("#dealList:not(.display-none)").addClass("display-none");
        jq("#dealFilterContainer").hide();
        ASC.CRM.Common.hideExportButtons();
        jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
        jq("#dealsEmptyScreen.display-none").removeClass("display-none");
    };

    var _renderNoDealsForQueryEmptyScreen = function() {
        jq("#dealTable tbody tr").remove();
        jq("#dealList").hide();
        jq("#dealList:not(.display-none)").addClass("display-none");
        ASC.CRM.Common.hideExportButtons();
        jq("#mainSelectAllDeals").attr("disabled", true);
        jq("#dealsEmptyScreen:not(.display-none)").addClass("display-none");
        jq("#emptyContentForDealsFilter.display-none").removeClass("display-none");
    };

    var callback_get_opportunities_by_filter = function(params, opportunities) {
        ASC.CRM.ListDealView.Total = params.__total || 0;
        var startIndex = params.__startIndex || 0,
            selectedIDs = new Array();

        for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
        }
        for (var i = 0, n = opportunities.length; i < n; i++) {
            ASC.CRM.ListDealView._dealItemFactory(opportunities[i], selectedIDs);
        }
        ASC.CRM.ListDealView.dealList = opportunities;
        jq(window).trigger("getDealsFromApi", [params, opportunities]);

        if (ASC.CRM.ListDealView.Total === 0 &&
                    typeof (ASC.CRM.ListDealView.advansedFilter) != "undefined" &&
                    ASC.CRM.ListDealView.advansedFilter.advansedFilter().length == 1) {
            ASC.CRM.ListDealView.noDeals = true;
            ASC.CRM.ListDealView.noDealsForQuery = true;
        } else {
            ASC.CRM.ListDealView.noDeals = false;
            if (ASC.CRM.ListDealView.Total === 0) {
                ASC.CRM.ListDealView.noDealsForQuery = true;
            } else {
                ASC.CRM.ListDealView.noDealsForQuery = false;
            }
        }

        if (ASC.CRM.ListDealView.noDeals) {
            _renderNoDealsEmptyScreen();
            LoadingBanner.hideLoading();
            return false;
        }

        if (ASC.CRM.ListDealView.noDealsForQuery) {
            _renderNoDealsForQueryEmptyScreen();

            jq("#dealFilterContainer").show();
            _resizeFilter();

            LoadingBanner.hideLoading();
            return false;
        }

        if (opportunities.length == 0) {//it can happen when select page without elements after deleting
            jq("dealsEmptyScreen:not(.display-none)").addClass("display-none");
            jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
            jq("#dealTable tbody tr").remove();
            jq("#tableForDealsNavigation").show();
            jq("#mainSelectAllDeals").attr("disabled", true);
            ASC.CRM.Common.hideExportButtons();
            LoadingBanner.hideLoading();
            return false;
        }

        jq("#totalDealsOnPage").text(ASC.CRM.ListDealView.Total);
        jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
        jq("#dealsEmptyScreen:not(.display-none)").addClass("display-none");
        ASC.CRM.Common.showExportButtons();

        jq("#dealFilterContainer").show();
        _resizeFilter();

        jq("#mainSelectAllDeals").removeAttr("disabled");

        jq("#dealList").show();
        jq("#dealList.display-none").removeClass("display-none");
        jq("#dealTable tbody").replaceWith(jq.tmpl("dealListTmpl", { opportunities: ASC.CRM.ListDealView.dealList }));

        ASC.CRM.ListDealView.checkFullSelection();

        ASC.CRM.Common.RegisterContactInfoCard();
        for (var i = 0, n = ASC.CRM.ListDealView.dealList.length; i < n; i++) {
            ASC.CRM.Common.tooltip("#dealTitle_" + ASC.CRM.ListDealView.dealList[i].id, "tooltip");
        }

        _renderDealPageNavigator(startIndex);
        _renderSimpleDealsPageNavigator();

        if (ASC.CRM.ListDealView.bidList.length == 0) {
            jq("#dealList .showTotalAmount").hide();
        } else {
            jq("#dealList .showTotalAmount").show();
        }

        window.scrollTo(0, 0);
        ScrolledGroupMenu.fixContentHeaderWidth(jq('#dealHeaderMenu'));
        LoadingBanner.hideLoading();
    };
   

    var callback_add_tag = function(params, tag) {
        jq("#addTagDealsDialog").hide();
        if (params.isNewTag) {
            var tag = {
                value: params.tagName,
                title: params.tagName
            };
            ASC.CRM.Data.dealTags.push(tag);
            _renderTagElement(tag);

            ASC.CRM.ListDealView.advansedFilter = ASC.CRM.ListDealView.advansedFilter.advansedFilter(
            {
                nonetrigger: true,
                sorters: [],
                filters: [
                    { id: "tags", type: 'combobox', options: ASC.CRM.Data.dealTags, enable: ASC.CRM.Data.dealTags.length > 0 }
                ]
            });
        }
    };

    var callback_delete_batch_opportunities = function(params, data) {
        var newDealsList = new Array();
        for (var i = 0, len_i = ASC.CRM.ListDealView.dealList.length; i < len_i; i++) {
            var isDeleted = false;
            for (var j = 0, len_j = params.dealsIDsForDelete.length; j < len_j; j++)
                if (params.dealsIDsForDelete[j] == ASC.CRM.ListDealView.dealList[i].id) {
                isDeleted = true;
                break;
            }
            if (!isDeleted)
                newDealsList.push(ASC.CRM.ListDealView.dealList[i]);

        }
        ASC.CRM.ListDealView.dealList = newDealsList;

        ASC.CRM.ListDealView.Total -= params.dealsIDsForDelete.length;
        jq("#totalDealsOnPage").text(ASC.CRM.ListDealView.Total);

        var selectedIDs = new Array();
        for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
        }

        for (var i = 0, len = params.dealsIDsForDelete.length; i < len; i++) {
            var $objForRemove = jq("#dealItem_" + params.dealsIDsForDelete[i]);
            if ($objForRemove.length != 0)
                $objForRemove.remove();

            var index = jq.inArray(params.dealsIDsForDelete[i], selectedIDs);
            if (index != -1) {
                selectedIDs.splice(index, 1);
                ASC.CRM.ListDealView.selectedItems.splice(index, 1);
            }
        }
        jq("#mainSelectAllDeals").prop("checked", false);

        _checkForLockMainActions();
        _renderCheckedDealsCount(ASC.CRM.ListDealView.selectedItems.length);

        if (ASC.CRM.ListDealView.Total == 0
            && (typeof (ASC.CRM.ListDealView.advansedFilter) == "undefined"
            || ASC.CRM.ListDealView.advansedFilter.advansedFilter().length == 1)) {
            ASC.CRM.ListDealView.noDeals = true;
            ASC.CRM.ListDealView.noDealsForQuery = true;
        } else {
            ASC.CRM.ListDealView.noDeals = false;
            if (ASC.CRM.ListDealView.Total === 0) {
                ASC.CRM.ListDealView.noDealsForQuery = true;
            } else {
                ASC.CRM.ListDealView.noDealsForQuery = false;
            }
        }

        PopupKeyUpActionProvider.EnableEsc = true;
        if (ASC.CRM.ListDealView.noDeals) {
            _renderNoDealsEmptyScreen();
            jq.unblockUI();
            return false;
        }

        if (ASC.CRM.ListDealView.noDealsForQuery) {
            _renderNoDealsForQueryEmptyScreen();

            jq.unblockUI();
            return false;
        }

        if (jq("#dealTable tbody tr").length == 0) {
            jq.unblockUI();

            var startIndex = ASC.CRM.ListDealView.entryCountOnPage * (dealPageNavigator.CurrentPageNumber - 1);
            if (startIndex >= ASC.CRM.ListDealView.Total) { startIndex -= ASC.CRM.ListDealView.entryCountOnPage; }
            _renderContent(startIndex);
        } else {
            jq.unblockUI();
        }
    };

    var callback_update_opportunity_rights = function(params, opportunities) {
        for (var i = 0, n = opportunities.length; i < n; i++) {
            for (var j = 0, m = ASC.CRM.ListDealView.dealList.length; j < m; j++) {
                var opportunity_id = opportunities[i].id;
                if (opportunity_id == ASC.CRM.ListDealView.dealList[j].id) {
                    ASC.CRM.ListDealView.dealList[j].isPrivate = opportunities[i].isPrivate;
                    jq("#dealItem_" + opportunity_id).replaceWith(
                        jq.tmpl("dealTmpl", ASC.CRM.ListDealView.dealList[j])
                    );
                    if (params.isBatch) {
                        jq("#checkDeal_" + opportunity_id).prop("checked", true);
                    } else {
                        ASC.CRM.ListDealView.selectedItems = new Array();
                    }
                    break;
                }
            }
        }
        ASC.CRM.Common.RegisterContactInfoCard();
        PopupKeyUpActionProvider.EnableEsc = true;
        jq.unblockUI();
    };

    var _showActionMenu = function(dealID) {
        var deal = null;
        for (var i = 0, n = ASC.CRM.ListDealView.dealList.length; i < n; i++) {
            if (dealID == ASC.CRM.ListDealView.dealList[i].id) {
                deal = ASC.CRM.ListDealView.dealList[i];
                break;
            }
        }
        if (deal == null) return;

        jq("#dealActionMenu .editDealLink").attr("href", jq.format("deals.aspx?id={0}&action=manage", dealID));

        jq("#dealActionMenu .deleteDealLink").unbind("click").bind("click", function() {
            jq("#dealActionMenu").hide();
            jq("#dealTable .entity-menu.active").removeClass("active");
            ASC.CRM.ListDealView.showConfirmationPanelForDelete(deal.title, dealID, true);
        });

        jq("#dealActionMenu .showProfileLink").attr("href", jq.format("deals.aspx?id={0}", dealID));

        if (ASC.CRM.ListDealView.currentUserIsAdmin || Teamlab.profile.id == deal.createdBy.id) {
            jq("#dealActionMenu .setPermissionsLink").show();
            jq("#dealActionMenu .setPermissionsLink").unbind("click").bind("click", function() {
                jq("#dealcaseActionMenu").hide();
                jq("#dealTable .entity-menu.active").removeClass("active");

                ASC.CRM.ListDealView.deselectAll();
                ASC.CRM.ListDealView.selectedItems.push(_createShortDeal(deal));

                _showSetPermissionsPanel({ isBatch: false });
            });
        } else {
            jq("#dealActionMenu .setPermissionsLink").hide();
        }

        if (ASC.CRM.ListDealView.currentUserIsAdmin && jq("#dealActionMenu .createProject").length == 1) {
            var basePathForLink = StudioManager.getLocationPathToModule("projects") + "projects.aspx?action=add&opportunityID=";
            jq("#dealActionMenu .createProject").attr("href", basePathForLink + dealID);
            jq("#dealActionMenu .createProject").unbind("click").bind("click", function() {
                jq("#dealTable .entity-menu.active").removeClass("active");
                jq("#dealActionMenu").hide();
            });
        }
    };

    var _lockMainActions = function() {
        jq("#dealHeaderMenu .menuActionDelete").removeClass("unlockAction").unbind("click");
        jq("#dealHeaderMenu .menuActionAddTag").removeClass("unlockAction").unbind("click");
        jq("#dealHeaderMenu .menuActionPermissions").removeClass("unlockAction").unbind("click");
    };

    var _checkForLockMainActions = function() {
        if (ASC.CRM.ListDealView.selectedItems.length === 0) {
            _lockMainActions();
            return;
        }
        if (!jq("#dealHeaderMenu .menuActionDelete").hasClass("unlockAction")) {
            jq("#dealHeaderMenu .menuActionDelete").addClass("unlockAction").unbind("click").bind("click", function() {
                _showDeletePanel();
            });
        }

        if (!jq("#dealHeaderMenu .menuActionAddTag").hasClass("unlockAction")) {
            jq("#dealHeaderMenu .menuActionAddTag").addClass("unlockAction");
        }

        if (!jq("#dealHeaderMenu .menuActionPermissions").hasClass("unlockAction")) {
            jq("#dealHeaderMenu .menuActionPermissions").addClass("unlockAction").unbind("click").bind("click", function() {
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
        jq("#addTagDealsDialog ul.dropdown-content").append(jq("<li></li>").append($tagElem));
    };

    var _renderAndInitTagsDialog = function() {
        for (var i = 0, n = ASC.CRM.Data.dealTags.length; i < n; i++) {
            _renderTagElement(ASC.CRM.Data.dealTags[i]);
        }
        jq.dropdownToggle({
            dropdownID: "addTagDealsDialog",
            switcherSelector: "#dealHeaderMenu .menuActionAddTag.unlockAction",
            addTop: 5,
            addLeft: 0,
            showFunction: function(switcherObj, dropdownItem) {
                jq("#addTagDealsDialog input.textEdit").val("");
            }
        });
    };

    var _initDealActionMenu = function() {
        jq.dropdownToggle({
            dropdownID: "dealActionMenu",
            switcherSelector: "#dealTable .entity-menu",
            addTop: -2,
            addLeft: 10,
            rightPos: true,
            beforeShowFunction: function (switcherObj, dropdownItem) {
                var dealId = switcherObj.attr("id").split('_')[1];
                if (!dealId) return;
                _showActionMenu(parseInt(dealId));
            },
            showFunction: function(switcherObj, dropdownItem) {
                jq("#dealTable .entity-menu.active").removeClass("active");
                if (dropdownItem.is(":hidden")) {
                    switcherObj.addClass("active");
                }
            },
            hideFunction: function() {
                jq("#dealTable .entity-menu.active").removeClass("active");
            }
        });


        jq("#dealTable").unbind("contextmenu").bind("contextmenu", function(event) {
            event.preventDefault();

            var e = ASC.CRM.Common.fixEvent(event),
                target = jq(e.srcElement || e.target),
                dealId = parseInt(target.closest("tr.with-entity-menu").attr("id").split('_')[1]);
            if (!dealId) {
                return false;
            }
            _showActionMenu(dealId);
            jq("#dealTable .entity-menu.active").removeClass("active");

            var $dropdownItem = jq("#dealActionMenu");
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
            menuSelector: "#dealHeaderMenu",
            menuAnchorSelector: "#mainSelectAllDeals",
            menuSpacerSelector: "#dealList .header-menu-spacer",
            userFuncInTop: function() { jq("#dealHeaderMenu .menu-action-on-top").hide(); },
            userFuncNotInTop: function() { jq("#dealHeaderMenu .menu-action-on-top").show(); }
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

    var _initConfirmationPannels = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "deleteDealsPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.Confirmation,
            questionText: ASC.CRM.Resources.CRMCommonResource.ConfirmationDeleteText,
            innerHtmlText: ["<div id=\"deleteDealsList\" class=\"containerForListBatchDelete mobile-overflow\">",
                                        "<dl>",
                                            "<dt class=\"listForBatchDelete\">",
                                                ASC.CRM.Resources.CRMDealResource.Deals,
                                                ":",
                                            "</dt>",
                                            "<dd class=\"listForBatchDelete\">",
                                            "</dd>",
                                        "</dl>",
                                    "</div>"].join(''),

            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMDealResource.DeletingDeals
        }).insertAfter("#dealList");

        jq("#deleteDealsPanel").on("click", ".crm-actionButtonsBlock .button.blue", function () {
            ASC.CRM.ListDealView.deleteBatchDeals();
        });

        jq.tmpl("blockUIPanelTemplate", {
            id: "setPermissionsDealsPanel",
            headerTest: ASC.CRM.Resources.CRMCommonResource.SetPermissions,
            innerHtmlText: "",
            OKBtn: ASC.CRM.Resources.CRMCommonResource.OK,
            OKBtnClass: "setPermissionsLink",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ASC.CRM.Resources.CRMCommonResource.SaveChangesProggress
        }).insertAfter("#dealList");

        jq("#permissionsDealsPanelInnerHtml").insertBefore("#setPermissionsDealsPanel .containerBodyBlock .crm-actionButtonsBlock").removeClass("display-none");
    };

    var _addThisTag = function(obj) {
        var params = {
            tagName: jq(obj).text(),
            isNewTag: false
        };

        _addTag(params);
    };

    var _addTag = function(params) {
        var selectedIDs = [];
        for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
        }
        params.selectedIDs = selectedIDs;

        Teamlab.addCrmTag(params, "opportunity", params.selectedIDs, params.tagName,
        {
            success: callback_add_tag,
            before: function(par) {
                for (var i = 0, n = par.selectedIDs.length; i < n; i++) {
                    jq("#checkDeal_" + par.selectedIDs[i]).hide();
                    jq("#loaderImg_" + par.selectedIDs[i]).show();
                }
            },
            after: function(par) {
                for (var i = 0, n = par.selectedIDs.length; i < n; i++) {
                    jq("#loaderImg_" + par.selectedIDs[i]).hide();
                    jq("#checkDeal_" + par.selectedIDs[i]).show();
                }
            }
        });
    };

    var _showDeletePanel = function() {
        jq("#deleteDealsList dd.listForBatchDelete").html("");
        for (var i = 0, len = ASC.CRM.ListDealView.selectedItems.length; i < len; i++) {
            var label = jq("<label></label>")
                            .attr("title", ASC.CRM.ListDealView.selectedItems[i].title)
                            .text(ASC.CRM.ListDealView.selectedItems[i].title);
            jq("#deleteDealsList dd.listForBatchDelete").append(
                            label.prepend(jq("<input>")
                            .attr("type", "checkbox")
                            .prop("checked", true)
                            .attr("id", "deal_" + ASC.CRM.ListDealView.selectedItems[i].id))
                        );

        }
        jq("#deleteDealsPanel .crm-actionButtonsBlock").show();
        jq("#deleteDealsPanel .crm-actionProcessInfoBlock").hide();
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#deleteDealsPanel", 500, 500, 0);
    };

    var _showSetPermissionsPanel = function(params) {
        if (jq("#setPermissionsDealsPanel div.tintMedium").length > 0) {
            jq("#setPermissionsDealsPanel div.tintMedium span.header-base").remove();
            jq("#setPermissionsDealsPanel div.tintMedium").removeClass("tintMedium").css("padding", "0px");
        }
        jq("#isPrivate").prop("checked", false);
        ASC.CRM.PrivatePanel.changeIsPrivateCheckBox();
        jq("#selectedUsers div.selectedUser[id^=selectedUser_]").remove();
        SelectedUsers.IDs = new Array();
        jq("#setPermissionsDealsPanel .crm-actionButtonsBlock").show();
        jq("#setPermissionsDealsPanel .crm-actionProcessInfoBlock").hide();
        jq("#setPermissionsDealsPanel .setPermissionsLink").unbind("click").bind("click", function() {
            _setPermissions(params);
        });
        PopupKeyUpActionProvider.EnableEsc = false;
        StudioBlockUIManager.blockUI("#setPermissionsDealsPanel", 600, 500, 0);
    };

    var _setPermissions = function(params) {
        var selectedUsers = SelectedUsers.IDs,
            selectedIDs = [];
        selectedUsers.push(SelectedUsers.CurrentUserID);

        for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
            selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
        }

        var data = {
            opportunityid: selectedIDs,
            isPrivate: jq("#isPrivate").is(":checked"),
            accessList: selectedUsers
        };

        Teamlab.updateCrmOpportunityRights(params, data,
        {
            success: callback_update_opportunity_rights,
            before: function() {
                jq("#setPermissionsDealsPanel .crm-actionButtonsBlock").hide();
                jq("#setPermissionsDealsPanel .crm-actionProcessInfoBlock").show();
            },
            after: function() {
                jq("#setPermissionsDealsPanel .crm-actionProcessInfoBlock").hide();
                jq("#setPermissionsDealsPanel .crm-actionButtonsBlock").show();
            }
        });
    };

    var _createShortDeal = function(deal) {
        var shortDeal = {
            id: deal.id,
            isClosed: deal.isClosed,
            isPrivate: deal.isPrivate,
            title: deal.title
        };
        return shortDeal;
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
        jq("#mainSelectAllDeals").prop("checked", false);//'cause checkboxes save their state between refreshing the page

        jq("#tableForDealNavigation select:first")
            .val(entryCountOnPage)
            .change(function () {
                ASC.CRM.ListDealView.changeCountOfRows(this.value);
            })
            .tlCombobox();
    };
    
    var _initEmptyScreen = function (emptyListImgSrc, emptyFilterListImgSrc) {
        //init emptyScreen for all list
        var buttonHTML = ["<a class='link underline blue plus' href='deals.aspx?action=manage'>",
            ASC.CRM.Resources.CRMDealResource.CreateFirstDeal,
            "</a>"].join('');
        
        if (jq.browser.mobile != true) {
            buttonHTML += ["<br/><a class='crm-importLink' href='deals.aspx?action=import'>",
                ASC.CRM.Resources.CRMDealResource.ImportDeals,
                "</a>"].join('');
        }

        jq.tmpl("emptyScrTmpl",
            {
                ID: "dealsEmptyScreen",
                ImgSrc: emptyListImgSrc,
                Header: ASC.CRM.Resources.CRMDealResource.EmptyContentDealsHeader,
                Describe: jq.format(ASC.CRM.Resources.CRMDealResource.EmptyContentDealsDescribe, "<span class='hintStages baseLinkAction'>", "</span>"),
                ButtonHTML: buttonHTML,
                CssClass: "display-none"
            }).insertAfter("#dealList");

        //init emptyScreen for filter
        jq.tmpl("emptyScrTmpl",
            {
                ID: "emptyContentForDealsFilter",
                ImgSrc: emptyFilterListImgSrc,
                Header: ASC.CRM.Resources.CRMDealResource.EmptyContentDealsFilterHeader,
                Describe: ASC.CRM.Resources.CRMDealResource.EmptyContentDealsFilterDescribe,
                ButtonHTML: ["<a class='crm-clearFilterButton' href='javascript:void(0);' onclick='ASC.CRM.ListDealView.advansedFilter.advansedFilter(null);'>",
                    ASC.CRM.Resources.CRMCommonResource.ClearFilter,
                    "</a>"].join(''),
                CssClass: "display-none"
            }).insertAfter("#dealList");

    };
    
    var _initFilter = function () {
        if (!jq("#dealsAdvansedFilter").advansedFilter) return;

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

        ASC.CRM.ListDealView.advansedFilter = jq("#dealsAdvansedFilter")
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
                                apiparamname: "responsibleID",
                                title       : ASC.CRM.Resources.CRMCommonResource.My,
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByResponsible,
                                groupby     : "responsible",
                                filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByResponsible,
                                enable      : true,
                                bydefault   : { id: Teamlab.profile.id, value: Teamlab.profile.id }
                            },

                            {
                                type        : "person",
                                id          : "responsibleID",
                                apiparamname: "responsibleID",
                                title       : ASC.CRM.Resources.CRMDealResource.CustomResponsibleFilter,
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByResponsible,
                                groupby     : "responsible",
                                filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByResponsible
                            },

                            {
                                type        : "combobox",
                                id          : "lastMonth",
                                apiparamname: jq.toJSON(["fromDate", "toDate"]),
                                title       : ASC.CRM.Resources.CRMCommonResource.LastMonth,
                                filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
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
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
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
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
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
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
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
                                title       : ASC.CRM.Resources.CRMDealResource.CustomDateFilter,
                                filtertitle : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                group       : ASC.CRM.Resources.CRMCommonResource.FilterByDate,
                                groupby     : "byDate"
                            },

                            {
                                type        : ASC.CRM.myFilter.type,
                                id          : ASC.CRM.myFilter.idFilterByParticipant,
                                apiparamname: jq.toJSON(["contactID", "contactAlsoIsParticipant"]),
                                title       : ASC.CRM.Resources.CRMDealResource.FilterByParticipant,
                                group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                groupby     : "contact",
                                hashmask    : "",
                                create      : ASC.CRM.myFilter.createFilterByParticipant,
                                customize   : ASC.CRM.myFilter.customizeFilterByParticipant,
                                destroy     : ASC.CRM.myFilter.destroyFilterByParticipant,
                                process     : ASC.CRM.myFilter.processFilter
                            },
                            {
                                type        : ASC.CRM.myFilter.type,
                                id          : ASC.CRM.myFilter.idFilterByContact,
                                apiparamname: jq.toJSON(["contactID", "contactAlsoIsParticipant"]),
                                title       : ASC.CRM.Resources.CRMDealResource.FilterByContact,
                                group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                groupby     : "contact",
                                hashmask    : "",
                                create      : ASC.CRM.myFilter.createFilterByContact,
                                customize   : ASC.CRM.myFilter.customizeFilterByContact,
                                destroy     : ASC.CRM.myFilter.destroyFilterByContact,
                                process     : ASC.CRM.myFilter.processFilter
                            },
                            {
                                type        : "combobox",
                                id          : "tags",
                                apiparamname: "tags",
                                title       : ASC.CRM.Resources.CRMCommonResource.FilterWithTag,
                                group       : ASC.CRM.Resources.CRMCommonResource.Other,
                                options     : ASC.CRM.Data.dealTags,
                                defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                enable      : ASC.CRM.Data.dealTags.length > 0,
                                multiple    : true
                            },

                            {
                                type        : "combobox",
                                id          : "opportunityStagesID",
                                apiparamname: "opportunityStagesID",
                                title       : ASC.CRM.Resources.CRMDealResource.ByStage,
                                group       : ASC.CRM.Resources.CRMDealResource.FilterByStageOrStageType,
                                groupby     : "stageType",
                                options     : ASC.CRM.Data.dealMilestones,
                                defaulttitle: ASC.CRM.Resources.CRMCommonResource.Choose,
                                enable      : ASC.CRM.Data.dealMilestones.length > 0
                            },
                            {
                                type        : "combobox",
                                id          : "stageTypeOpen",
                                apiparamname: "stageType",
                                title       : ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_Open,
                                group       : ASC.CRM.Resources.CRMDealResource.FilterByStageOrStageType,
                                filtertitle : ASC.CRM.Resources.CRMDealResource.ByStageType,
                                groupby     : "stageType",
                                options     :
                                        [
                                        { value: "Open", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_Open, def: true },
                                        { value: "ClosedAndWon", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndWon },
                                        { value: "ClosedAndLost", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndLost }
                                        ]
                            },
                            {
                                type        : "combobox",
                                id          : "stageTypeClosedAndWon",
                                apiparamname: "stageType",
                                title       : ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndWon,
                                filtertitle : ASC.CRM.Resources.CRMDealResource.ByStageType,
                                group       : ASC.CRM.Resources.CRMDealResource.FilterByStageOrStageType,
                                groupby     : "stageType",
                                options     :
                                        [
                                        { value: "Open", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_Open },
                                        { value: "ClosedAndWon", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndWon, def: true },
                                        { value: "ClosedAndLost", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndLost }
                                        ]
                            },
                            {
                                type        : "combobox",
                                id          : "stageTypeClosedAndLost",
                                apiparamname: "stageType",
                                title       : ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndLost,
                                filtertitle : ASC.CRM.Resources.CRMDealResource.ByStageType,
                                group       : ASC.CRM.Resources.CRMDealResource.FilterByStageOrStageType,
                                groupby     : "stageType",
                                options     :
                                        [
                                        { value: "Open", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_Open },
                                        { value: "ClosedAndWon", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndWon },
                                        { value: "ClosedAndLost", classname: '', title: ASC.CRM.Resources.CRMEnumResource.DealMilestoneStatus_ClosedAndLost, def: true }
                                        ]
                            }
                ],
                sorters: [
                            { id: "title", title: ASC.CRM.Resources.CRMCommonResource.Title, dsc: false, def: false },
                            { id: "responsible", title: ASC.CRM.Resources.CRMCommonResource.Responsible, dsc: false, def: false },
                            { id: "stage", title: ASC.CRM.Resources.CRMDealResource.DealMilestone, dsc: false, def: true },
                            { id: "bidvalue", title: ASC.CRM.Resources.CRMDealResource.ExpectedValue, dsc: false, def: false },
                            { id: "dateandtime", title: ASC.CRM.Resources.CRMDealResource.Estimated, dsc: false, def: false }
                ]
            })
            .bind("setfilter", ASC.CRM.ListDealView.setFilter)
            .bind("resetfilter", ASC.CRM.ListDealView.resetFilter);
    };

    return {
        contactID: 0,

        dealList: [],
        bidList: [],
        selectedItems: [],

        isFilterVisible: false,

        currentUserIsAdmin: false,

        entryCountOnPage: 0,
        defaultCurrentPageNumber: 0,

        noDeals: false,
        noDealsForQuery: false,

        currencyDecimalSeparator: '.',
        cookieKey: "",

        init: function (contactID, visiblePageCount, cookieKey, currencyDecimalSeparator, isAdmin, _selectorType, emptyListImgSrc, emptyFilterListImgSrc) {
            ASC.CRM.ListDealView.currencyDecimalSeparator = currencyDecimalSeparator;
            ASC.CRM.ListDealView.cookieKey = cookieKey;
            ASC.CRM.ListDealView.currentUserIsAdmin = isAdmin;

            var settings = {
                    page: 1,
                    countOnPage: jq("#tableForDealNavigation select:first>option:first").val()
                },
                key = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + location.pathname + location.search,
                currentAnchor = location.hash,
                cookieKey = encodeURIComponent(key);

            currentAnchor = currentAnchor && typeof currentAnchor === 'string' && currentAnchor.charAt(0) === '#'
                ? currentAnchor.substring(1)
                : currentAnchor;

            var cookieAnchor = jq.cookies.get(cookieKey);
            if (currentAnchor == "" || cookieAnchor == currentAnchor) {
                var tmp = ASC.CRM.Common.getPagingParamsFromCookie(ASC.CRM.ListDealView.cookieKey);
                if (tmp != null) {
                    settings = tmp;
                }
            } else {
                _setCookie(settings.page, settings.countOnPage);
            }

            ASC.CRM.ListDealView.entryCountOnPage = settings.countOnPage;
            ASC.CRM.ListDealView.defaultCurrentPageNumber = settings.page;

            _preInitPage(ASC.CRM.ListDealView.entryCountOnPage);
            _initEmptyScreen(emptyListImgSrc, emptyFilterListImgSrc);

            jq.tmpl("dealExtendedListTmpl", { contactID: 0 }).prependTo("#dealList");

            _initPageNavigatorControl(ASC.CRM.ListDealView.entryCountOnPage, ASC.CRM.ListDealView.defaultCurrentPageNumber, visiblePageCount);

            _renderAndInitTagsDialog();

            _initDealActionMenu();

            _initScrolledGroupMenu();

            _initOtherActionMenu();

            ASC.CRM.ListDealView.initConfirmationPanelForDelete();

            _initConfirmationPannels();

            window["contactSelectorForFilter"] = new ASC.CRM.ContactSelector.ContactSelector("contactSelectorForFilter",
                        {
                            SelectorType: _selectorType,
                            EntityType: 0,
                            EntityID: 0,
                            ShowOnlySelectorContent: false,
                            DescriptionText: "",
                            DeleteContactText: "",
                            AddContactText: "",
                            IsInPopup: false,
                            NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
                            NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
                            NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
                            ShowChangeButton: true,
                            ShowAddButton: false,
                            ShowDeleteButton: false,
                            ShowContactImg: false,
                            ShowNewCompanyContent: false,
                            ShowNewContactContent: false,
                            presetSelectedContactsJson: '',
                            ExcludedArrayIDs: [],
                            HTMLParent: "#hiddenBlockForContactSelector"
                        });

            jq(window).bind("afterResetSelectedContact", function(event, obj, objName) {
                if (objName === "contactSelectorForFilter" && ASC.CRM.myFilter.filterId) {
                    jq('#' + ASC.CRM.myFilter.filterId).advansedFilter('resize');
                }
            });
            
            _initFilter();

            /*tracking events*/
            ASC.CRM.ListDealView.advansedFilter.one("adv-ready", function () {
                var crmAdvansedFilterContainer = jq("#dealsAdvansedFilter .advansed-filter-list");
                crmAdvansedFilterContainer.find("li[data-id='my'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'me_manager');
                crmAdvansedFilterContainer.find("li[data-id='responsibleID'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'custom_manager');
                crmAdvansedFilterContainer.find("li[data-id='company'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'company');
                crmAdvansedFilterContainer.find("li[data-id='Persons'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'persons');
                crmAdvansedFilterContainer.find("li[data-id='withopportunity'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'with_opportunity');
                crmAdvansedFilterContainer.find("li[data-id='lastMonth'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'last_month');
                crmAdvansedFilterContainer.find("li[data-id='yesterday'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'yesterday');
                crmAdvansedFilterContainer.find("li[data-id='today'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'today');
                crmAdvansedFilterContainer.find("li[data-id='thisMonth'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'this_month');
                crmAdvansedFilterContainer.find("li[data-id='fromToDate'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'from_to_date');
                crmAdvansedFilterContainer.find("li[data-id='opportunityStagesID'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'opportunity_stages');
                crmAdvansedFilterContainer.find("li[data-id='stageTypeOpen'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'stage_type_open');
                crmAdvansedFilterContainer.find("li[data-id='stageTypeClosedAndWon'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'stage_type_closed_and_won');
                crmAdvansedFilterContainer.find("li[data-id='stageTypeClosedAndLost'] .inner-text").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'stage_type_closed_and_lost');

                jq("#dealsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.deals, ga_Actions.filterClick, 'sort');
                jq("#dealsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.deals, ga_Actions.filterClick, "search_text", "enter");
            });
        },

        setFilter: function(evt, $container, filter, params, selectedfilters) { _changeFilter(); },
        resetFilter: function(evt, $container, filter, selectedfilters) { _changeFilter(); },

        getFilterSettings: function(startIndex) {
            startIndex = startIndex || 0;

            var settings = {
                startIndex: startIndex,
                count: ASC.CRM.ListDealView.entryCountOnPage
            };

            if (!ASC.CRM.ListDealView.advansedFilter) return settings;

            var param = ASC.CRM.ListDealView.advansedFilter.advansedFilter();

            jq(param).each(function(i, item) {
                switch (item.id) {
                    case "sorter":
                        settings.sortBy = item.params.id;
                        settings.sortOrder = item.params.dsc == true ? 'descending' : 'ascending';
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
            return settings;
        },

        _dealItemFactory : function(deal, selectedIDs) {
            var tmpDate = new Date(),
                nowDate = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);

            deal.isOverdue = false;

            switch (deal.stage.stageType) {
                case 1:
                    deal.closedStatusString = ASC.CRM.Resources.CRMJSResource.SuccessfullyClosed + ": " + deal.actualCloseDateString;
                    deal.classForTitle = "linkHeaderMedium gray-text";
                    break;
                case 2:
                    deal.closedStatusString = ASC.CRM.Resources.CRMJSResource.UnsuccessfullyClosed + ": " + deal.actualCloseDateString;
                    deal.classForTitle = "linkHeaderMedium gray-text";
                    break;
                case 0:
                    deal.closedStatusString = "";
                    if (deal.expectedCloseDateString != "" && deal.expectedCloseDate.getTime() < nowDate.getTime()) {
                        deal.isOverdue = true;
                        deal.classForTitle = "linkHeaderMedium red-text";
                    } else {
                        deal.classForTitle = "linkHeaderMedium";
                    }
                    break;
            }

            deal.bidNumberFormat = ASC.CRM.ListDealView.numberFormat(deal.bidValue,
                                  { before: deal.bidCurrency.symbol, thousands_sep: " ", dec_point: ASC.CRM.ListDealView.currencyDecimalSeparator });

            if (typeof (deal.bidValue) != "undefined" && deal.bidValue != 0) {
                if (typeof (deal.perPeriodValue) == "undefined")// || deal.perPeriodValue == 0)
                    deal.perPeriodValue = 0;

                var isExist = false;
                for (var j = 0, len = ASC.CRM.ListDealView.bidList.length; j < len; j++)
                    if (ASC.CRM.ListDealView.bidList[j].bidCurrencyAbbreviation == deal.bidCurrency.abbreviation) {
                        ASC.CRM.ListDealView.bidList[j].bidValue += deal.bidValue * (deal.perPeriodValue != 0 ? deal.perPeriodValue : 1);
                        isExist = true;
                        break;
                    }

                if (!isExist) {
                    ASC.CRM.ListDealView.bidList.push(
                                      {
                                          bidValue: deal.bidValue * (deal.perPeriodValue != 0 ? deal.perPeriodValue : 1),
                                          bidCurrencyAbbreviation: deal.bidCurrency.abbreviation,
                                          bidCurrencySymbol: deal.bidCurrency.symbol,
                                          isConvertable: deal.bidCurrency.isConvertable
                                      });
                }
            }

            var index = jq.inArray(deal.id, selectedIDs);
            deal.isChecked = index != -1;
        },

        expectedValue: function(bidType, perPeriodPalue) {
            switch (bidType) {
                case 1:
                    return ASC.CRM.Resources.CRMJSResource.BidType_PerHour + " " + jq.format(ASC.CRM.Resources.CRMJSResource.PerPeriodHours, perPeriodPalue);
                case 2:
                    return ASC.CRM.Resources.CRMJSResource.BidType_PerDay + " " + jq.format(ASC.CRM.Resources.CRMJSResource.PerPeriodDays, perPeriodPalue);
                case 3:
                    return ASC.CRM.Resources.CRMJSResource.BidType_PerWeek + " " + jq.format(ASC.CRM.Resources.CRMJSResource.PerPeriodWeeks, perPeriodPalue);
                case 4:
                    return ASC.CRM.Resources.CRMJSResource.BidType_PerMonth + " " + jq.format(ASC.CRM.Resources.CRMJSResource.PerPeriodMonths, perPeriodPalue);
                case 5:
                    return ASC.CRM.Resources.CRMJSResource.BidType_PerYear + " " + jq.format(ASC.CRM.Resources.CRMJSResource.PerPeriodYears, perPeriodPalue);
                default:
                    return "";
            }
        },

        changeCountOfRows: function(newValue) {
            if (isNaN(newValue)) return;
            var newCountOfRows = newValue * 1;
            ASC.CRM.ListDealView.entryCountOnPage = newCountOfRows;
            dealPageNavigator.EntryCountOnPage = newCountOfRows;

            _setCookie(1, newCountOfRows);
            _renderContent(0);
        },

        numberFormat: function(_number, _cfg) {
            function obj_merge(obj_first, obj_second) {
                var obj_return = {};
                for (key in obj_first) {
                    if (typeof obj_second[key] !== 'undefined') {
                        obj_return[key] = obj_second[key];
                    } else {
                        obj_return[key] = obj_first[key];
                    }
                }
                return obj_return;
            }
            function thousands_sep(_num, _sep) {
                if (_num.length <= 3) { return _num; }
                var _count = _num.length,
                    _num_parser = '',
                    _count_digits = 0;
                for (var _p = (_count - 1); _p >= 0; _p--) {
                    var _num_digit = _num.substr(_p, 1);
                    if (_count_digits % 3 == 0 && _count_digits != 0 && !isNaN(parseFloat(_num_digit))) _num_parser = _sep + _num_parser;
                    _num_parser = _num_digit + _num_parser;
                    _count_digits++;
                }
                return _num_parser;
            }
            if (typeof _number !== 'number') {
                _number = parseFloat(_number);
                if (isNaN(_number)) return false;
            }
            var _cfg_default = { before: '', after: '', decimals: 2, dec_point: '.', thousands_sep: ',' };
            if (_cfg && typeof _cfg === 'object') {
                _cfg = obj_merge(_cfg_default, _cfg);
            } else {
                _cfg = _cfg_default;
            }
            _number = _number.toFixed(_cfg.decimals);
            if (_number.indexOf('.') != -1) {
                var _number_arr = _number.split('.'),
                    _number = thousands_sep(_number_arr[0], _cfg.thousands_sep) + _cfg.dec_point + _number_arr[1];
            } else {
                var _number = thousands_sep(_number, _cfg.thousands_sep);
            }
            return [_cfg.before, _number, _cfg.after].join('');
        },

        showExchangeRatePopUp: function() {
            if (ASC.CRM.ListDealView.bidList.length == 0) return;
            ASC.CRM.ExchangeRateView.init(ASC.CRM.ListDealView.bidList, ASC.CRM.ListDealView.currencyDecimalSeparator);
            jq("#ExchangeRateTabs>a:first").click();
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI('#exchangeRatePopUp', 550, 600, 0);
        },

        selectAll: function(obj) {
            var isChecked = jq(obj).is(":checked"),
                selectedIDs = new Array();

            for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
            }

            for (var i = 0, len = ASC.CRM.ListDealView.dealList.length; i < len; i++) {
                var deal = ASC.CRM.ListDealView.dealList[i],
                    index = jq.inArray(deal.id, selectedIDs);
                if (isChecked && index == -1) {
                    ASC.CRM.ListDealView.selectedItems.push(_createShortDeal(deal));
                    selectedIDs.push(deal.id);
                    jq("#dealItem_" + deal.id).addClass("selectedRow");
                    jq("#checkDeal_" + deal.id).prop("checked", true);
                }
                if (!isChecked && index != -1) {
                    ASC.CRM.ListDealView.selectedItems.splice(index, 1);
                    selectedIDs.splice(index, 1);
                    jq("#dealItem_" + deal.id).removeClass("selectedRow");
                    jq("#checkDeal_" + deal.id).prop("checked", false);
                }
            }
            _renderCheckedDealsCount(ASC.CRM.ListDealView.selectedItems.length);
            _checkForLockMainActions();
        },

        selectItem: function(obj) {
            var id = parseInt(jq(obj).attr("id").split("_")[1]),
                selectedDeal = null,
                selectedIDs = [],
                index = 0;

            for (var i = 0, n = ASC.CRM.ListDealView.dealList.length; i < n; i++) {
                if (id == ASC.CRM.ListDealView.dealList[i].id) {
                    selectedDeal = _createShortDeal(ASC.CRM.ListDealView.dealList[i]);
                }
            }

            for (var i = 0, n = ASC.CRM.ListDealView.selectedItems.length; i < n; i++) {
                selectedIDs.push(ASC.CRM.ListDealView.selectedItems[i].id);
            }
            index = jq.inArray(id, selectedIDs);

            if (jq(obj).is(":checked")) {
                jq(obj).addClass("selectedRow");
                if (index == -1) {
                    ASC.CRM.ListDealView.selectedItems.push(selectedDeal);
                }
                ASC.CRM.ListDealView.checkFullSelection();
            } else {
                jq("#mainSelectAllDeals").prop("checked", false);
                jq(obj).removeClass("selectedRow");
                if (index != -1) {
                    ASC.CRM.ListDealView.selectedItems.splice(index, 1);
                }
            }
            _renderCheckedDealsCount(ASC.CRM.ListDealView.selectedItems.length);
            _checkForLockMainActions();
        },

        deselectAll: function() {
            ASC.CRM.ListDealView.selectedItems = [];
            _renderCheckedDealsCount(0);
            jq("#dealTable input:checkbox").prop("checked", false);
            jq("#mainSelectAllDeals").prop("checked", false);
            jq("#dealTable tr.selectedRow").removeClass("selectedRow");
            _lockMainActions();
        },

        checkFullSelection: function() {
            var rowsCount = jq("#dealTable tbody tr").length,
                selectedRowsCount = jq("#dealTable input[id^=checkDeal_]:checked").length;
            jq("#mainSelectAllDeals").prop("checked", rowsCount == selectedRowsCount);
        },

        deleteBatchDeals: function() {
            var ids = [],
                params = null;
            jq("#deleteDealsPanel input:checked").each(function() {
                ids.push(parseInt(jq(this).attr("id").split("_")[1]));
            });
            params = { dealsIDsForDelete: ids };

            Teamlab.removeCrmOpportunity(params, ids,
                {
                    success: callback_delete_batch_opportunities,
                    before: function(params) {
                        jq("#deleteDealsPanel .crm-actionButtonsBlock").hide();
                        jq("#deleteDealsPanel .crm-actionProcessInfoBlock").show();
                    },
                    after: function(params) {
                        jq("#deleteDealsPanel .crm-actionProcessInfoBlock").hide();
                        jq("#deleteDealsPanel .crm-actionButtonsBlock").show();
                    }
                });
        },

        initConfirmationPanelForDelete: function (title, dealID, isListView) {
            jq.tmpl("blockUIPanelTemplate", {
                id: "confirmationDeleteOneDealPanel",
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
                progressText: ASC.CRM.Resources.CRMJSResource.DeleteDealInProgress
            }).appendTo("#studioPageContent .mainPageContent .containerBodyBlock:first");

        },

        showConfirmationPanelForDelete: function(title, dealID, isListView) {
            jq("#confirmationDeleteOneDealPanel .confirmationAction>b").text(jq.format(ASC.CRM.Resources.CRMJSResource.DeleteDealConfirmMessage, Encoder.htmlDecode(title)));

            jq("#confirmationDeleteOneDealPanel .crm-actionButtonsBlock>.button.blue.middle").unbind("click").bind("click", function() {
                ASC.CRM.ListDealView.deleteDeal(dealID, isListView);
            });
            PopupKeyUpActionProvider.EnableEsc = false;
            StudioBlockUIManager.blockUI("#confirmationDeleteOneDealPanel", 500, 500, 0);
        },

        deleteDeal: function(dealID, isListView) {
            if (isListView === true) {
                var ids =[];
                ids.push(dealID);
                var params = { dealsIDsForDelete: ids };
                Teamlab.removeCrmOpportunity(params, ids, callback_delete_batch_opportunities);
            } else {
                var contact_id = jq.trim(jq.getURLParam("contact_id"));

                Teamlab.removeCrmOpportunity({ contact_id: contact_id }, dealID, {
                    before: function() {
                        jq("#confirmationDeleteOneDealPanel .crm-actionButtonsBlock").hide();
                        jq("#confirmationDeleteOneDealPanel .crm-actionProcessInfoBlock").show();

                        jq("#crm_dealMakerDialog input, #crm_dealMakerDialog select, #crm_dealMakerDialog textarea").attr("disabled", true);
                        jq("#crm_dealMakerDialog .crm-actionProcessInfoBlock span").text(ASC.CRM.Resources.CRMJSResource.DeleteDealInProgress);
                        jq("#crm_dealMakerDialog .crm-actionButtonsBlock").hide();
                        jq("#crm_dealMakerDialog .crm-actionProcessInfoBlock").show();
                    },
                    success: function(params, opportunity) {
                        if (params.contact_id != "") {
                            location.href = jq.format("default.aspx?id={0}#deals", params.contact_id);
                        } else {
                            location.href = "deals.aspx";
                        }
                    }
                });
            }
        },

        addNewTag: function() {
            var newTag = jq("#addTagDealsDialog input").val().trim();
            if (newTag == "") {
                return false;
            }

            var params = {
                tagName: newTag,
                isNewTag: true
            };
            _addTag(params);
        }
    };
})();


ASC.CRM.DealActionView = (function() {

    var initFields = function(CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask) {
        if (typeof (window.customFieldList) != "undefined" && window.customFieldList.length != 0) {
            ASC.CRM.Common.renderCustomFields(customFieldList, "custom_field_", "customFieldRowTmpl", "#crm_dealMakerDialog .deal_info dl:last");
        }
        jq.registerHeaderToggleClick("#crm_dealMakerDialog .deal_info", "dt.headerToggleBlock");
        jq("#crm_dealMakerDialog .deal_info dt.headerToggleBlock").each(
                function() {
                    jq(this).nextUntil("dt.headerToggleBlock").hide();
                });

        jq("input.textEditCalendar").mask(dateMask);
        jq("input.textEditCalendar").datepickerWithButton();

        jq.forceIntegerOnly("#perPeriodValue, #probability");
        jq("#probability").focusout(function(e) {
            var probability = jq.trim(jq("#probability").val());
            if (probability != "" && probability * 1 > 100) {
                jq("#probability").val(100);
            }
        });

        jq("#bidValue").keypress(function(event) {
            // Backspace, Del,
            var controlKeys = [8, 9];
            // IE doesn't support indexOf
            var isControlKey = controlKeys.join(",").match(new RegExp(event.which));
            // Some browsers just don't raise events for control keys. Easy.
            if ((!event.which || // Control keys in most browsers. e.g. Firefox tab is 0
                (48 <= event.which && event.which <= 57) || // Always 0 through 9
                    isControlKey) ||
                        (jq(this).val().length - jq(this).val().replace(CurrencyDecimalSeparator, '').length < 1 &&
                            event.which == KeyCodeCurrencyDecimalSeparator * 1)) {
                return;
            } else {
                event.preventDefault();
            }
        });

        jq("#bidValue").unbind('paste').bind('paste', function(e) {
            var oldValue = this.value,
                $obj = this;
            setTimeout(
                function() {
                    var text = jq($obj).val();
                    if (isNaN(text)) {
                        jq($obj).val(oldValue);
                    } else if (CurrencyDecimalSeparator != '.') {
                        text.replace('.', CurrencyDecimalSeparator);
                        jq($obj).val(text);
                    }
                }, 0);
            return true;
        });


        for (var i = 0, n = window.dealMilestones.length; i < n; i++) {
            var dealMilestone = window.dealMilestones[i];

            jq("<option>")
                .attr("value", dealMilestone.id)
                .text(dealMilestone.title)
                .appendTo(jq("#dealMilestone"));
        }
        jq("#probability").val(window.dealMilestones[jq("#dealMilestone")[0].selectedIndex].probability);
    };

    var initOtherActionMenu = function() {
        ASC.CRM.Common.removeExportButtons();
        jq("#menuCreateNewTask").bind("click", function () { ASC.CRM.TaskActionView.showTaskPanel(0, "", 0, null, {}); });
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
            OKBtnHref: "settings.aspx?type=custom_field&view=opportunity",
            CancelBtn: ASC.CRM.Resources.CRMCommonResource.Cancel,
            progressText: ""
        }).insertAfter("#otherDealsCustomFieldPanel");
    };

    var initDealMembersAndClientSelectors = function (_selectorTypeClient, _selectorTypeMember) {
        window["dealClientSelector"] = new ASC.CRM.ContactSelector.ContactSelector("dealClientSelector",
        {
            SelectorType: _selectorTypeClient,
            EntityType: 0,
            EntityID: 0,
            ShowOnlySelectorContent: false,
            DescriptionText: ASC.CRM.Resources.CRMCommonResource.FindContactByName,
            DeleteContactText: ASC.CRM.Resources.CRMCommonResource.DeleteParticipant,
            AddContactText: "",
            IsInPopup: false,
            NewCompanyTitleWatermark: ASC.CRM.Resources.CRMContactResource.CompanyName,
            NewContactFirstNameWatermark: ASC.CRM.Resources.CRMContactResource.FirstName,
            NewContactLastNameWatermark: ASC.CRM.Resources.CRMContactResource.LastName,
            ShowChangeButton: window.hasDealTargetClient ? false : true,
            ShowAddButton: true,
            ShowDeleteButton: false,
            ShowContactImg: true,
            ShowNewCompanyContent: true,
            ShowNewContactContent: true,
            HTMLParent: "#dealClientContainer",
            ExcludedArrayIDs: window.dealMembersIDs,
            presetSelectedContactsJson: window.presetClientContactsJson
        });

        window["dealMemberSelector"] = new ASC.CRM.ContactSelector.ContactSelector("dealMemberSelector",
        {
            SelectorType: _selectorTypeMember,
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
            ShowChangeButton: true,
            ShowAddButton: true,
            ShowDeleteButton: true,
            ShowContactImg: true,
            ShowNewCompanyContent: true,
            ShowNewContactContent: true,
            HTMLParent: "#dealMembersBody",
            ExcludedArrayIDs: window.dealClientIDs,
            presetSelectedContactsJson: window.presetMemberContactsJson
        });

        if (window.showMembersPanel === true) {
            jq("#dealMembersHeader").show();
            jq("#dealMembersBody").show();
        }

        jq(window).bind("editContactInSelector", function (event, $itemObj, objName) {
            if (objName == "dealMemberSelector")
                window.dealClientSelector.ExcludedArrayIDs = window.dealMemberSelector.SelectedContacts.slice(0);
            if (objName == "dealClientSelector")
                window.dealMemberSelector.ExcludedArrayIDs = [];
        });


        jq(window).bind("deleteContactFromSelector", function (event, $itemObj, objName) {
            if (objName == "dealMemberSelector") {
                window.dealClientSelector.ExcludedArrayIDs = window.dealMemberSelector.SelectedContacts.slice(0);
                if (jq("div[id^='item_dealMemberSelector_']").length == 1) {
                    jq("#dealMembersHeader").hide();
                    jq("#dealMembersBody").hide();
                    jq("#item_dealClientSelector_0").removeClass("hasMembers");
                }
            }
            if (objName == "dealClientSelector") {
                window.dealMemberSelector.ExcludedArrayIDs = [];
            }
        });

        jq(window).bind("contactSelectorIsReady", function (event, objName) {
            if (objName == "dealClientSelector") {
                jq("#selectorContent_dealClientSelector_0 .crm-addNewLink").parent().remove();
                jq("#newContactContent_dealClientSelector_0 .crm-addNewLink").remove();
                jq("#infoContent_dealClientSelector_0 .crm-addNewLink").removeAttr("onclick");

                jq("#infoContent_dealClientSelector_0 .crm-addNewLink").click(function () {
                    jq("#item_dealClientSelector_0").addClass("hasMembers");
                    if (jq("div[id^='item_dealMemberSelector_']").length == 0) {
                        window.dealMemberSelector.AddNewSelector(jq(this));
                    }
                    jq("#dealMembersHeader").show();
                    jq("#dealMembersBody").show();
                });
            }
            if (objName == "dealMemberSelector") {
                if (window.dealMemberSelector.SelectedContacts.length != 0) {
                    jq("#item_dealClientSelector_0").addClass("hasMembers");
                }
            }
        });

        jq("#infoContent_dealClientSelector_0 .crm-editLink").bind('click', function () {
            window.dealMemberSelector.ExcludedArrayIDs = [];
        });
        
        window.dealClientSelector.SelectItemEvent = ASC.CRM.DealActionView.chooseMainContact;
        window.dealMemberSelector.SelectItemEvent = ASC.CRM.DealActionView.chooseMemberContact;
    };

    var renderDealInfo = function(currencyDecimalSeparator) {

        jq("#nameDeal").val(window.targetDeal.title);
        jq("#descriptionDeal").val(window.targetDeal.description);
        jq("#bidValue").val(window.targetDeal.bid_value.toString().replace(/[\.\,]/g, currencyDecimalSeparator));

        jq(jq.format("#bidCurrency option[value={0}]", window.targetDeal.bid_currency)).prop("selected", "selected");

        if (window.targetDeal.bid_type > 0) {
            jq(jq.format("#bidType [value={0}]", window.targetDeal.bid_type)).prop("selected", "selected");
            jq("#bidType").nextAll().show();

            jq("#perPeriodValue").val(window.targetDeal.per_period_value);
            jq("#bidType").change();
        }

        jq("#probability").val(window.targetDeal.deal_milestone_probability);
        jq("#expectedCloseDate").val(window.targetDeal.expected_close_date);

        jq(jq.format("#dealMilestone option[value={0}]", window.targetDeal.deal_milestone)).prop("selected", "selected");

        jq("#contactID").val("contact_id");

        jq("#deleteDealButton").unbind("click").bind("click", function() {
            ASC.CRM.ListDealView.showConfirmationPanelForDelete(jq.htmlEncodeLight(window.targetDeal.title), window.targetDeal.id, false);
        });
    };

    return {

        init: function (CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask, today, _selectorTypeClient, _selectorTypeMember, isAdmin) {

            initFields(CurrencyDecimalSeparator, KeyCodeCurrencyDecimalSeparator, dateMask);
            initOtherActionMenu();
            ASC.CRM.ListDealView.initConfirmationPanelForDelete();
            if (isAdmin === true) {
                initConfirmationGotoSettingsPanel();
            }

            var dealID = parseInt(jq.getURLParam("id"));
            if (!isNaN(dealID) && typeof (window.targetDeal) === "object") {
                renderDealInfo(CurrencyDecimalSeparator);
            } else {
                jq("#expectedCloseDate").val(today);
            }

            initDealMembersAndClientSelectors(_selectorTypeClient, _selectorTypeMember);
        },

        chooseMainContact: function(obj, params) {
            window.dealClientSelector.setContact(params.input, obj.id, obj.displayName, obj.smallFotoUrl);
            window.dealClientSelector.showInfoContent(params.input);
            window.dealMemberSelector.ExcludedArrayIDs = [obj.id];
            //ASC.CRM.ContactSelector.Cache = {};
        },

        chooseMemberContact: function(obj, params) {
            window.dealMemberSelector.setContact(params.input, obj.id, obj.displayName, obj.smallFotoUrl);
            window.dealMemberSelector.showInfoContent(params.input);
            window.dealClientSelector.ExcludedArrayIDs.push(obj.id);
            //ASC.CRM.ContactSelector.Cache = {};
        },

        selectBidTypeEvent: function(selectObj) {
            var idx = selectObj.selectedIndex;
            if (idx != 0) {
                jq(selectObj).nextAll().show();
            } else {
                jq(selectObj).nextAll().hide();
            }

            var elems = jq(selectObj).nextAll('span.splitter'),
                resourceValue = "";
            switch (idx) {
                case 1:
                    resourceValue = ASC.CRM.Resources.CRMJSResource.PerPeriodHours;
                    break;
                case 2:
                    resourceValue = ASC.CRM.Resources.CRMJSResource.PerPeriodDays;
                    break;
                case 3:
                    resourceValue = ASC.CRM.Resources.CRMJSResource.PerPeriodWeeks;
                    break;
                case 4:
                    resourceValue = ASC.CRM.Resources.CRMJSResource.PerPeriodMonths;
                    break;
                case 5:
                    resourceValue = ASC.CRM.Resources.CRMJSResource.PerPeriodYears;
                    break;
            }

            var labels = resourceValue.split("{0}");

            jq(elems).each(function(index) {
                jq(this).text(jq.trim(labels[index]));
            });

        },

        submitForm: function() {
            try {
                var isValid = true;

                if (jq.trim(jq("#nameDeal").val()) == "") {
                    ShowRequiredError(jq("#nameDeal"));
                    isValid = false;
                } else {
                    RemoveRequiredErrorClass(jq("#nameDeal"));
                }

                if (window.userSelector.SelectedUserId == null || window.userSelector.SelectedUserId == "") {
                     if (jq.browser.mobile === true) {
                        ShowRequiredError(jq("#AdvUserSelectorContainer select:first"));
                    } else {
                        ShowRequiredError(jq("#crm_dealMakerDialog .inputUserName:first"));
                    }

                    isValid = false;
                } else {
                    if (jq.browser.mobile === true) {
                        RemoveRequiredErrorClass(jq("#AdvUserSelectorContainer select:first"));
                    } else {
                        RemoveRequiredErrorClass(jq("#crm_dealMakerDialog .inputUserName:first"));
                    }
                }

                if (!isValid)
                    return false;

                var dealMilestoneProbability = jq.trim(jq("#probability").val());

                if (dealMilestoneProbability == "") {
                    dealMilestoneProbability = 0;
                    jq("#probability").val(0);
                } else {
                    dealMilestoneProbability = dealMilestoneProbability * 1;
                    if (dealMilestoneProbability > 100) {
                        jq("#probability").val(100);
                    }
                }

                jq("#crm_dealMakerDialog input, #crm_dealMakerDialog select, #crm_dealMakerDialog textarea")
                    .prop("readonly", "readonly")
                    .addClass('disabled');

                jq("#responsibleID").val(window.userSelector.SelectedUserId);

                if (window.dealClientSelector.SelectedContacts.length > 0) {
                    jq("#selectedContactID").val(window.dealClientSelector.SelectedContacts[0]);
                } else {
                    jq("#selectedContactID").val(0);
                }

                jq("#selectedMembersID").val(window.dealMemberSelector.SelectedContacts.join(","));

                if (jq.getURLParam("id") != null) {
                    jq("#crm_dealMakerDialog .crm-actionProcessInfoBlock span").text(ASC.CRM.Resources.CRMJSResource.EditingDealProgress);
                } else {
                    jq("#crm_dealMakerDialog .crm-actionProcessInfoBlock span").text(ASC.CRM.Resources.CRMJSResource.AddNewDealProgress);
                }

                jq("#crm_dealMakerDialog .crm-actionButtonsBlock").hide();
                jq("#crm_dealMakerDialog .crm-actionProcessInfoBlock").show();

                if (!jq("#isPrivate").is(":checked")) {
                    window.SelectedUsers.IDs = new Array();
                    jq("#cbxNotify").removeAttr("checked");
                }

                jq("#isPrivateDeal").val(jq("#isPrivate").is(":checked"));
                jq("#notifyPrivateUsers").val(jq("#cbxNotify").is(":checked"));
                jq("#selectedPrivateUsers").val(window.SelectedUsers.IDs.join(","));

                var $checkboxes = jq("#crm_dealMakerDialog .deal_info input[type='checkbox'][id^='custom_field_']");
                if ($checkboxes) {
                    for (var i = 0; i < $checkboxes.length; i++) {
                        if (jq($checkboxes[i]).is(":checked")) {
                            var id = $checkboxes[i].id.replace('custom_field_', '');
                            jq("#crm_dealMakerDialog .deal_info input[name='customField_" + id + "']").val(jq($checkboxes[i]).is(":checked"));
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

ASC.CRM.DealFullCardView = (function() {

    var renderCustomFields = function() {
        if (typeof (window.customFieldList) != "undefined" && window.customFieldList.length != 0) {
            var sortedList = ASC.CRM.Common.sortCustomFieldList(window.customFieldList);
            jq.tmpl("customFieldListTmpl", sortedList).insertBefore("#dealHistoryTable");
        }
    };

    return {
        init: function() {
            jq.tmpl("tagViewTmpl",
                    { tags: window.dealTags,
                      availableTags: window.dealAvailableTags
                    })
                    .appendTo("#dealTagsTD");
            ASC.CRM.TagView.init("opportunity", false);

            if (typeof (window.dealResponsibleIDs) != "undefined" && window.dealResponsibleIDs.length != 0) {
                jq("#dealProfile .dealAccessList").html(ASC.CRM.Common.getAccessListHtml(window.dealResponsibleIDs));
            }
            renderCustomFields();

            ASC.CRM.Common.RegisterContactInfoCard();

            jq.registerHeaderToggleClick("#dealProfile .crm-detailsTable", "tr.headerToggleBlock");
            jq("#dealHistoryTable .headerToggle").bind("click", function() {
                ASC.CRM.HistoryView.activate();
            });

            jq("#dealProfile .crm-detailsTable .headerToggle").not("#dealHistoryTable .headerToggle").each(
                function() {
                    jq(this).parents("tr.headerToggleBlock:first").nextUntil(".headerToggleBlock").hide();
                });

            jq.dropdownToggle({
                dropdownID: 'dealMilestoneDropDown',
                switcherSelector: '#dealMilestoneSwitcher',
                addTop: 0,
                showFunction: function(switcherObj, dropdownItem) {
                    var left = parseInt(dropdownItem.css("left")) + jq("#dealMilestoneSwitcher").width() - 12;
                    dropdownItem.css("left", left);
                }
            });
        },

        changeDealMilestone: function(dealID, milestoneID) {
            Teamlab.updateCrmOpportunityMilestone({}, dealID, milestoneID, function(params, deal) {
                jq("#dealMilestoneDropDown").hide();
                var dealMilestone = deal.stage;
                jq("#dealMilestoneSwitcher").text(dealMilestone.title);
                jq("#dealMilestoneProbability").text(dealMilestone.successProbability + "%");

                if (dealMilestone.stageType != 0) { //closed = not open
                    jq("#closeDealDate").text(ASC.CRM.Resources.CRMJSResource.ActualCloseDate + ":");
                    jq("#closeDealDate").next().next().text(deal.actualCloseDateString);
                } else { //opened
                    if (deal.expectedCloseDateString != "") {
                        jq("#closeDealDate").text(ASC.CRM.Resources.CRMJSResource.ExpectedCloseDate + ":");
                        jq("#closeDealDate").next().next().text(deal.expectedCloseDateString);
                    } else {
                        jq("#closeDealDate").text(ASC.CRM.Resources.CRMJSResource.ExpectedCloseDate + ":");
                        jq("#closeDealDate").next().next().text(ASC.CRM.Resources.CRMJSResource.NoCloseDate);
                    }
                }
            });
        }
    };
})();

ASC.CRM.DealDetailsView = (function() {

    var initTabs = function () {
        window.ASC.Controls.ClientTabsNavigator.init("DealTabs", {
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
                title: ASC.CRM.Resources.CRMDealResource.PeopleInDeal,
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
        ASC.Controls.AnchorController.bind(/contacts/, function () {
            ///////////////
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
            var dealID = jq.getURLParam("id") * 1,
                type = "opportunity",
                fileids = [];
            fileids.push(file.id);

            Teamlab.addCrmEntityFiles({}, dealID, type, {
                entityid: dealID,
                entityType: type,
                fileids: fileids
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

    var initDealDetailsMenuPanel = function() {
        jq(document).ready(function() {
            jq.dropdownToggle({
                dropdownID: "dealDetailsMenuPanel",
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

        jq("#dealDetailsMenuPanel .createProject").unbind("click").bind("click", function() {
            jq(".mainContainerClass .containerHeaderBlock .menu-small.active").removeClass("active");
            jq("#dealDetailsMenuPanel").hide();
        });
    };

    var initOtherActionMenu = function() {
        ASC.CRM.Common.removeExportButtons();

        var params = {};
        if (window.dealResponsibleIDs.length != 0) {
            params.taskResponsibleSelectorUserIDs = window.dealResponsibleIDs;
        }

        jq("#menuCreateNewTask").bind("click", function () { ASC.CRM.TaskActionView.showTaskPanel(0, window.entityData.type, window.entityData.id, null, params); });

        ASC.CRM.ListTaskView.bindEmptyScrBtnEvent(params);
    };

    var initEmptyScreens = function(imgSrcEmptyParticipants, imgSrcEmptyTasks) {
        jq.tmpl("emptyScrTmpl",
                { ID: "emptyDealParticipantPanel",
                    ImgSrc: imgSrcEmptyParticipants,
                    Header: ASC.CRM.Resources.CRMDealResource.EmptyPeopleInDealContent,
                    Describe: ASC.CRM.Resources.CRMDealResource.EmptyPeopleInDealDescript,
                    ButtonHTML: ["<a class='link dotline blue plus' ",
                            "onclick='javascript:jq(\"#dealParticipantPanel\").show();jq(\"#emptyDealParticipantPanel\").addClass(\"display-none\");'>",
                            ASC.CRM.Resources.CRMCommonResource.AddParticipant,
                            "</a>"
                            ].join(''),
                    CssClass: "display-none"
                }).insertAfter("#contactListBox");
    };

    var initParticipantsContactSelector = function() {
        window["dealContactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("dealContactSelector",
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
                    HTMLParent: "#dealParticipantPanel"
                });

        window.dealContactSelector.SelectItemEvent = ASC.CRM.DealDetailsView.addMemberToDeal;
        ASC.CRM.ListContactView.removeMember = ASC.CRM.DealDetailsView.removeMemberFromDeal;

        jq(window).bind("getContactsFromApi", function(event, contacts) {
            var contactLength = contacts.length;
            if (contactLength == 0) {
                jq("#emptyDealParticipantPanel.display-none").removeClass("display-none");
            } else {
                jq("#dealParticipantPanel").show();
                var contactIDs = [];
                for (var i = 0; i < contactLength; i++) {
                    contactIDs.push(contacts[i].id);
                }
                dealContactSelector.SelectedContacts = contactIDs;
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
            initDealDetailsMenuPanel();
            initOtherActionMenu();
        },

        removeMemberFromDeal: function(id) {
            Teamlab.removeCrmEntityMember({ contactID: parseInt(id) }, window.entityData.type, window.entityData.id, id, {
                before: function(params) {
                    jq("#simpleContactActionMenu").hide();
                    jq("#contactTable .entity-menu.active").removeClass("active");
                },
                after: function(params) {
                    var index = jq.inArray(params.contactID, window.dealContactSelector.SelectedContacts);
                    if (index != -1) {
                        window.dealContactSelector.SelectedContacts.splice(index, 1);
                    } else {
                        console.log("Can't find such contact in list");
                    }
                    ASC.CRM.ContactSelector.Cache = {};

                    jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                    ASC.CRM.HistoryView.removeOptionFromContact(params.contactID);

                    //ASC.CRM.Common.changeCountInTab("delete", "contacts");

                    setTimeout(function() {
                        jq("#contactItem_" + params.contactID).remove();
                        if (window.dealContactSelector.SelectedContacts.length == 0) {
                            jq("#dealParticipantPanel").hide();
                            jq("#emptyDealParticipantPanel.display-none").removeClass("display-none");
                        }
                    }, 500);
                }
            });
        },

        addMemberToDeal: function(obj) {
            if (jq("#contactItem_" + obj.id).length > 0) return false;
            var data =
            {
                contactid: obj.id,
                opportunityid: window.entityData.id
            };
            Teamlab.addCrmEntityMember({
                                        showCompanyLink: true,
                                        showUnlinkBtn: false,
                                        showActionMenu: true
                                    },
                                    window.entityData.type, window.entityData.id, obj.id, data, {
                    success: function(params, contact) {
                        ASC.CRM.ListContactView.CallbackMethods.addMember(params, contact);

                        window.dealContactSelector.SelectedContacts.push(contact.id);
                        //ASC.CRM.ContactSelector.Cache = {};
                        jq("#emptyDealParticipantPanel:not(.display-none)").addClass("display-none");
                        ASC.CRM.HistoryView.appendOptionToContact({ value: contact.id, title: contact.displayName });
                    }
                });
        }
    };
})();

ASC.CRM.ExchangeRateView = (function() {
    var _currenciesAndRates = null;

    var convertAmount = function () {
        var amount = jq("#amount").val().trim();
        if (amount != "") {
            jq("#introducedAmount").text(amount + " " + jq("#fromCurrency").val());
            jq("#conversionResult").text(amount * 1 * ASC.CRM.ExchangeRateView.Rate + " " + jq("#toCurrency").val());
        } else {
            jq("#introducedAmount").text("");
            jq("#conversionResult").text("");
        }
    };

    var renderBaseControl = function () {
        jq.tmpl("blockUIPanelTemplate", {
            id: "exchangeRatePopUp",
            headerTest: ASC.CRM.Resources.CRMCommonResource.ConversionRates
        }).appendTo("body");

        jq.tmpl("exchangeRateViewTmpl", { ratesPublisherDisplayDate: ASC.CRM.Data.ratesPublisherDisplayDate })
            .appendTo("#exchangeRatePopUp .containerBodyBlock:first");


        window.ASC.Controls.ClientTabsNavigator.init("ExchangeRateTabs", {
            tabs: [
            {
                title: ASC.CRM.Resources.CRMDealResource.TotalAmount,
                selected: true,
                anchor: "",
                divID: "totalAmountTab"
            },
            {
                title: ASC.CRM.Resources.CRMCommonResource.MoneyCalculator,
                selected: false,
                anchor: "",
                divID: "converterTab"
            },
            {
                title: ASC.CRM.Resources.CRMCommonResource.SummaryTable,
                selected: false,
                anchor: "",
                divID: "exchangeTab"
            }]
        });
    };

    var renderTotalAmount = function(bidList, currencyDecimalSeparator) {
        jq("#totalOnPage .diferrentBids").html("");
        jq("#totalOnPage .totalBid").html("");
        var sum = 0,
            tmpBidNumberFormat,
            isTotalBidAndExchangeRateShow = false;

        for (var i = 0, len = bidList.length; i < len; i++) {

            if (bidList[i].bidCurrencyAbbreviation != ASC.CRM.Data.defaultCurrency.Abbreviation) {
                isTotalBidAndExchangeRateShow = true;
            }

            tmpBidNumberFormat = ASC.CRM.ListDealView.numberFormat(bidList[i].bidValue,
                { before: bidList[i].bidCurrencySymbol, thousands_sep: " ", dec_point: currencyDecimalSeparator });

            jq.tmpl("bidFormat", { number: tmpBidNumberFormat, abbreviation: bidList[i].bidCurrencyAbbreviation }).appendTo("#totalOnPage .diferrentBids");

            if (!bidList[i].isConvertable) {
                jq.tmpl("bidFormat", { number: tmpBidNumberFormat, abbreviation: bidList[i].bidCurrencyAbbreviation }).appendTo("#totalOnPage .totalBid");
            }

            var tmp_rate = ASC.CRM.ExchangeRateView.ExchangeRates[bidList[i].bidCurrencyAbbreviation];
            if (bidList[i].isConvertable && typeof (tmp_rate) != "undefined") {
                sum += bidList[i].bidValue / tmp_rate;
            }
        }

        tmpBidNumberFormat = ASC.CRM.ListDealView.numberFormat(sum, { before: ASC.CRM.Data.defaultCurrency.Symbol, thousands_sep: " ", dec_point: currencyDecimalSeparator });

        jq.tmpl("bidFormat", { number: tmpBidNumberFormat, abbreviation: ASC.CRM.Data.defaultCurrency.Abbreviation }).prependTo("#totalOnPage .totalBid");

        if (isTotalBidAndExchangeRateShow == true) {
            jq("#totalOnPage .totalBidAndExchangeRateLink").show();
        } else {
            jq("#totalOnPage .totalBidAndExchangeRateLink").hide();
        }
        jq("#totalOnPage").show();
    };

    var getDataAndInit = function (bidList, currencyDecimalSeparator) {
        LoadingBanner.displayLoading();

        Teamlab.getCrmCurrencySummaryTable({}, ASC.CRM.Data.defaultCurrency.Abbreviation, function (params, tableData) {
            _currenciesAndRates = tableData;

            var html = "",
                tmp_cur = {};

            for (var i = 0, n = _currenciesAndRates.length; i < n; i++) {
                tmp_cur = _currenciesAndRates[i];
                html += [
                    "<option value=\"",
                    tmp_cur.abbreviation,
                    "\"",
                    tmp_cur.abbreviation == ASC.CRM.Data.defaultCurrency.Abbreviation ? " selected=\"selected\">" : ">",
                    jq.format("{0} - {1}", tmp_cur.abbreviation, tmp_cur.title),
                    "</option>"].join('');
            }

            jq("#fromCurrency").html(html);
            jq("#toCurrency").html(html);
            jq("#exchangeRateContent select:first").html(html);

            jq.tmpl("ratesTableTmpl", { currencyRates: _currenciesAndRates }).appendTo("#ratesTable");

            jq("#introducedFromCurrency").text(ASC.CRM.Data.defaultCurrency.Title + ":");
            jq("#introducedToCurrency").text(ASC.CRM.Data.defaultCurrency.Title + ":");

            jq("#conversionRate").text(jq.format("1 {0} = 1 {0}", ASC.CRM.Data.defaultCurrency.Abbreviation));

            for (var i = 0, n = _currenciesAndRates.length; i < n; i++) {
                ASC.CRM.ExchangeRateView.ExchangeRates[_currenciesAndRates[i].abbreviation] = _currenciesAndRates[i].rate;
                ASC.CRM.ExchangeRateView.ExchangeRatesNames[_currenciesAndRates[i].abbreviation] = _currenciesAndRates[i].title;
            }
            renderTotalAmount(bidList, currencyDecimalSeparator);

            jq.forceIntegerOnly("#amount", convertAmount);

            jq("#amount").keyup(function (event) {
                convertAmount();
            });
            LoadingBanner.hideLoading();
        });
    };

    return {
        Rate : 1,
        ExchangeRates : {},
        ExchangeRatesNames : {},


        init: function (bidList, currencyDecimalSeparator) {
            ASC.CRM.ExchangeRateView.Rate = 1;

            if (jq("#exchangeRatePopUp").length == 0) {
                renderBaseControl();
                getDataAndInit(bidList, currencyDecimalSeparator);
            } else {
                renderTotalAmount(bidList, currencyDecimalSeparator);
            }

        },

        changeCurrency: function() {
            var fromcurrency = jq("#fromCurrency").val(),
                tocurrency = jq("#toCurrency").val(),
                data = {
                    amount: 1,
                    fromcurrency: fromcurrency,
                    tocurrency: tocurrency
                };
            LoadingBanner.displayLoading();
            Teamlab.getCrmCurrencyConvertion({}, data, function(params, currencyRate) {
                ASC.CRM.ExchangeRateView.Rate = currencyRate;
                jq("#conversionRate").text("1 " + fromcurrency + " = " + currencyRate + " " + tocurrency);
                jq("#introducedFromCurrency").text(ASC.CRM.ExchangeRateView.ExchangeRatesNames[fromcurrency] + ":");
                jq("#introducedToCurrency").text(ASC.CRM.ExchangeRateView.ExchangeRatesNames[tocurrency] + ":");
                convertAmount();
                LoadingBanner.hideLoading();
            });
        },

        updateSummaryTable: function(newCurrency) {
            LoadingBanner.displayLoading();
            Teamlab.getCrmCurrencySummaryTable({}, newCurrency, function (params, tableData) {
                var $ratesList = jq("#ratesTable td.rateValue");
                for (var i = 0, n = $ratesList.length; i < n; i++) {
                    var $ratesItem = jq($ratesList[i]),
                        key = $ratesItem.attr("id").toLowerCase(),
                        rate = "";
                    
                    for (var j = 0, m = tableData.length; j < m; j++) {
                        if (tableData[j].abbreviation.toLowerCase() == key) {
                            rate = tableData[j].rate;
                            break;
                        }
                    }
                    $ratesItem.text(rate);
                    
                }
                LoadingBanner.hideLoading();
            });
        }


    };
})();


ASC.CRM.DealTabView = (function () {

    var _onGetDealsComplete = function () {
        jq("#dealTable tbody tr").remove();

        if (ASC.CRM.DealTabView.Total == 0) {
            jq("#dealList:not(.display-none)").addClass("display-none");
            jq("#emptyContentForDealsFilter.display-none").removeClass("display-none");
            LoadingBanner.hideLoading();
            return false;
        }

        jq("#dealButtonsPanel").show();
        jq("#dealList.display-none").removeClass("display-none");

        jq.tmpl("dealTmpl", ASC.CRM.DealTabView.dealList).prependTo("#dealTable tbody");

        if (ASC.CRM.ListDealView.bidList.length == 0) {
            jq("#dealList .showTotalAmount").hide();
        } else {
            jq("#dealList .showTotalAmount").show();
        }
        LoadingBanner.hideLoading();
    };

    var _onGetMoreDealsComplete = function () {
        if (ASC.CRM.DealTabView.dealList.length == 0) {
            return false;
        }

        jq.tmpl("dealTmpl", ASC.CRM.DealTabView.dealList).appendTo("#dealTable tbody");
        ASC.CRM.Common.RegisterContactInfoCard();

        if (ASC.CRM.ListDealView.bidList.length == 0) {
            jq("#dealList .showTotalAmount").hide();
        } else {
            jq("#dealList .showTotalAmount").show();
        }
    };

    var _addRecordsToContent = function () {
        if (!ASC.CRM.DealTabView.showMore) return false;
        ASC.CRM.DealTabView.dealList = [];
        var startIndex = jq("#dealTable tbody tr").length;

        jq("#showMoreDealsButtons .crm-showMoreLink").hide();
        jq("#showMoreDealsButtons .crm-loadingLink").show();

        _getDealsForContact(startIndex);
    };

    var _getApiFilter = function (startIndex) {
        return {
            contactID: ASC.CRM.DealTabView.contactID,
            count: ASC.CRM.DealTabView.entryCountOnPage,
            startIndex: startIndex,
            contactAlsoIsParticipant: true
        };
    };

    var _getDealsForContact = function (startIndex) {
        var filters = _getApiFilter(startIndex);
        Teamlab.getCrmOpportunities({ startIndex: startIndex }, { filter: filters, success: callback_get_opportunities_for_contact });
    };


    var callback_get_opportunities_for_contact = function (params, opportunities) {
        for (var i = 0, n = opportunities.length; i < n; i++) {
            ASC.CRM.ListDealView._dealItemFactory(opportunities[i], []);
        }
        ASC.CRM.DealTabView.dealList = opportunities;
        jq(window).trigger("getDealsFromApi", [params, opportunities]);

        ASC.CRM.DealTabView.Total = params.__total || 0;
        if (typeof (params.__nextIndex) == "undefined") {
            ASC.CRM.DealTabView.showMore = false;
        }

        if (!params.__startIndex) {
            _onGetDealsComplete();
        } else {
            _onGetMoreDealsComplete();
        }

        for (var i = 0, n = ASC.CRM.DealTabView.dealList.length; i < n; i++) {
            ASC.CRM.Common.tooltip("#dealTitle_" + ASC.CRM.DealTabView.dealList[i].id, "tooltip");
        }

        jq("#showMoreDealsButtons .crm-loadingLink").hide();
        if (ASC.CRM.DealTabView.showMore) {
            jq("#showMoreDealsButtons .crm-showMoreLink").show();
        } else {
            jq("#showMoreDealsButtons .crm-showMoreLink").hide();
        }
    };

    var callback_get_dealstab_data = function (params, allDeals, contactDeals) {
        if (typeof (params[0].__count) != "undefined" && params[0].__count != 0) {

            for (var i = 0, n = contactDeals.length; i < n; i++) {
                var idToExclude = contactDeals[i].id;
                for (var j = 0, m = allDeals.length; j < m; j++) {
                    if (allDeals[j].id == idToExclude) {
                        allDeals.splice(j, 1);
                        break;
                    }
                }
            }
            initDealsSelector(allDeals);
        }

        if (typeof (params[1].__count) != "undefined" && params[1].__count != 0) {
            jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
            jq("#dealList.display-none").removeClass("display-none");
            callback_get_opportunities_for_contact(params[1], contactDeals);
            jq("#dealsInContactPanel").show();
        } else {
            jq("#dealList:not(.display-none)").addClass("display-none");
            jq("#emptyContentForDealsFilter.display-none").removeClass("display-none");
        }
        LoadingBanner.hideLoading();
    };

    var callback_add_deal = function (params, deal) {
        jq("#dealList.display-none").removeClass("display-none");
        ASC.CRM.ListDealView._dealItemFactory(deal, []);

        jq.tmpl("dealTmpl", deal).prependTo("#dealTable tbody");
        removeDealFromList(params.element);

        jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
    };

    var _getDealTabViewData = function () {
        LoadingBanner.displayLoading();

        var filterForContactDeals = _getApiFilter(0),
            filterForLinkDeals = {
                startIndex: 0,
                sortBy:	"title",
                sortOrder:	"ascending",
                stageType:	"Open"
        };

        Teamlab.joint()
            .getCrmOpportunities({}, { filter: filterForLinkDeals })
            .getCrmOpportunities({}, { filter: filterForContactDeals })
            .start({}, {
                success: callback_get_dealstab_data
            });
    };

    var initDealsSelector = function (dealList) {
        if (jq.browser.mobile === true) {
            var chooseOption = {
                id: 0,
                title: ASC.CRM.Resources.CRMJSResource.LinkWithDeal
            };
            dealList.splice(0, 0, chooseOption);

            jq.tmpl("dealSelectorOptionTmpl", dealList).appendTo("#dealsInContactPanel select");
            jq("#dealsInContactPanel select")
                .change(function (evt) {
                    chooseDeal(jq(this).children("option:selected"), this.value);
                });
        } else {
            jq.tmpl("dealSelectorItemTmpl", dealList).appendTo("#dealSelectorContainer>.dropdown-content");

            jq.dropdownToggle({
                dropdownID: "dealSelectorContainer",
                switcherSelector: "#dealsInContactPanel .selectDeal>div",
                addTop: 2
            });

            if (dealList.length > 0) {
                jq("#dealsInContactPanel .selectDeal .menuAction").addClass("unlockAction");
                jq("#dealSelectorContainer").removeClass("display-none");
            }
            jq("#dealSelectorContainer").on("click", ".dropdown-content>li", function () {
                jq("#dealSelectorContainer").hide();
                var id = jq(this).attr("data-id");
                chooseDeal(jq(this), id);
            });
        }
    };

    var chooseDeal = function (element, id) {
        Teamlab.addCrmDealForContact({ element: element },  ASC.CRM.DealTabView.contactID, id, callback_add_deal);
    };

    var removeDealFromList = function (element) {
        element.remove();
        if (jq.browser.mobile === true) {
            jq("dealsInContactPanel select").val(0).tlCombobox();
        } else {
            if (jq("#dealSelectorContainer .dropdown-item").length == 0) {
                jq("#dealsInContactPanel .selectDeal .menuAction").removeClass("unlockAction");
                jq("#dealSelectorContainer").addClass("display-none");
            }
        }
    };

    return {
        contactID: 0,
        dealList: [],
        bidList:[],

        isTabActive: false,
        showMore: true,

        entryCountOnPage: 0,
        currentPageNumber: 1,
        currencyDecimalSeparator: '.',

        initTab: function (contactID, rowsCount, currencyDecimalSeparator, imgSrcEmptyScr) {
            ASC.CRM.DealTabView.contactID = contactID;
            ASC.CRM.DealTabView.entryCountOnPage = rowsCount;
            ASC.CRM.DealTabView.currencyDecimalSeparator = currencyDecimalSeparator;

            ASC.CRM.ListDealView.currencyDecimalSeparator = currencyDecimalSeparator;
            ASC.CRM.ListDealView.bidList = [];

            jq.tmpl("emptyScrTmpl",
                    {
                        ID: "emptyContentForDealsFilter",
                        ImgSrc: imgSrcEmptyScr,
                        Header: ASC.CRM.Resources.CRMDealResource.EmptyContentDealsHeader,
                        Describe: jq.format(ASC.CRM.Resources.CRMDealResource.EmptyContentDealsDescribe,
                                            "<span class='hintStages baseLinkAction'>", "</span>"),
                        ButtonHTML: ["<a class='link-with-entity baseLinkAction'>",
                                    ASC.CRM.Resources.CRMDealResource.LinkOrCreateDeal,
                                    "</a>"
                        ].join(''),
                        CssClass: "display-none"
                    }).insertAfter("#dealList");
            jq.tmpl("dealExtendedListTmpl", { contactID: contactID }).appendTo("#dealList");

            jq("#emptyContentForDealsFilter .emptyScrBttnPnl>a").bind("click", function () {
                jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
                jq("#dealsInContactPanel").show();
            });

            jq("#showMoreDealsButtons .crm-showMoreLink").bind("click", function () {
                _addRecordsToContent();
            });
        },

        activate: function () {
            if (ASC.CRM.DealTabView.isTabActive == false) {
                ASC.CRM.DealTabView.isTabActive = true;

                jq("#emptyContentForDealsFilter .emptyScrBttnPnl>a").bind("click", function () {
                    jq("#emptyContentForDealsFilter:not(.display-none)").addClass("display-none");
                    jq("#dealsInContactPanel").show();
                });

                jq("#dealsInContactPanel .createNewDeal>div").click(function () {
                    location.href = "deals.aspx?action=manage&contactID=" + ASC.CRM.DealTabView.contactID;
                });
                _getDealTabViewData();
            }
        }

        //addDealToList : function (deal) {
        //    if (jq.browser.mobile === true) {
        //        jq.tmpl("dealSelectorOptionTmpl", deal).appendTo("#dealsInContactPanel select");
        //        //jq("#projectsInContactPanel select").val(0).tlCombobox();
        //    } else {
        //        jq.tmpl("dealSelectorItemTmpl", deal).appendTo("#dealSelectorContainer>.dropdown-content");
        //        jq("#dealsInContactPanel .selectDeal .menuAction:not(.unlockAction)").addClass("unlockAction");
        //        jq("#dealSelectorContainer.display-none").hide();
        //        jq("#dealSelectorContainer.display-none").removeClass("display-none");
        //    }
        //},

    };
})();


jq(document).ready(function() {
    jq.dropdownToggle({
        switcherSelector: ".noContentBlock .hintStages",
        dropdownID: "files_hintStagesPanel",
        fixWinSize: false
    });
});