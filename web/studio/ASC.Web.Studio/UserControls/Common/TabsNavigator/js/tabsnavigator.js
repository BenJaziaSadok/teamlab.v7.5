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

if (typeof(ASC) == "undefined") {
    ASC = {};
}
if (typeof(ASC.Controls) == "undefined") {
    ASC.Controls = {};
}

ASC.Controls.TabsNavigator = new function () {
    (function () {
        ASC.Controls.AnchorController.bind('onupdate', function () {
            var anchor = ASC.Controls.AnchorController.getAnchor();
            try {
                var $tabObj = jq(".tabsNavigationLinkBox>.tabsNavigationLink_" + anchor + ":first");
                if ($tabObj) {
                    ASC.Controls.TabsNavigator.toggleTabs(jq($tabObj).attr("id"), "");
                }
            } catch (e) {
            }
        },
            { 'once': true }
        );
    })();

    var toggleCurrentTab = function (tab, tabID) {
        var hideFlag = true;
        var currentTabID = tab.attr("id");

        if (tab.hasClass("selectedTab")) {
            tab.removeClass("selectedTab");
        }
        if (currentTabID == tabID) {
            tab.addClass("selectedTab");
            hideFlag = false;
        }

        if (currentTabID == undefined || currentTabID == '' || currentTabID == null) {
            currentTabID = '';
        }
        var currentDivID = currentTabID.replace(/_tab$/gi, '');

        if (currentDivID != '' && jq("#" + currentDivID).length == 1) {
            if (hideFlag) {
                jq("#" + currentDivID + ":not(.display-none)").addClass("display-none");
            } else {
                jq("#" + currentDivID + ".display-none").removeClass("display-none");
            }
        }
    };

    this.toggleTabs = function (tabID, anchor) {
        var tab = jq("#" + tabID);
        tab.parent().children('a').each(
            function () {
                var child = jq(this);
                toggleCurrentTab(child, tabID);
            }
        );
        if (typeof(anchor) == "string" && anchor != "") {
            ASC.Controls.AnchorController.move(anchor);
        }
    };
};