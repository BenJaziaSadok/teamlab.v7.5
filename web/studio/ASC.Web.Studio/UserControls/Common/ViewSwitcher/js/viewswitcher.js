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
    ASC.Controls.AnchorController.bind('onupdate', function () {
        var anchor = ASC.Controls.AnchorController.getAnchor();
        var $tabObj = jq(".viewSwitcher>ul>.viewSwitcherTab_" + anchor).get(0);
        if ($tabObj) {
            viewSwitcherToggleTabs(jq($tabObj).attr("id"));
        }
    },
        {
            'once': true
        }
    );

})();

function viewSwitcherDropdownRegisterAutoHide (event, switcherID, dropdownID) {

    if (!jq((event.target) ? event.target : event.srcElement)
        .parents()
        .addBack()
        .is("'#" + switcherID + ", #" + dropdownID + "'"))
        jq("#" + dropdownID).hide();
}

function viewSwitcherToggleTabs (tabID) {

    var tab = jq('#' + tabID);

    tab.parent().children().each(
        function () {
            var child = jq(this);
            viewSwitcherToggleCurrentTab(child, tabID);
        }
    );
}

function viewSwitcherToggleCurrentTab (tab, tabID) {
    var hideFlag = true;
    var currentTabID = tab.attr('id');

    if (tab.hasClass('viewSwitcherTabSelected')) {
        tab.addClass('viewSwitcherTab');
        tab.removeClass('viewSwitcherTabSelected');

    }

    if (currentTabID == tabID) {
        tab.addClass('viewSwitcherTabSelected');
        tab.removeClass('viewSwitcherTab');
        hideFlag = false;
    }

    if (currentTabID == undefined || currentTabID == '' || currentTabID == null)
        currentTabID = '';

    var currentDivID = currentTabID.replace(/_ViewSwitcherTab$/gi, '');

    if (currentDivID != '') {
        if (hideFlag) {
            jq('#' + currentDivID).hide();
        } else {
            jq('#' + currentDivID).show();
        }
    }
}