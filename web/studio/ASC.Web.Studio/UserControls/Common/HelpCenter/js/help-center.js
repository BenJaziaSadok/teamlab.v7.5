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

var showHelpPage = function (helpId) {
    var hideAll = !!jq("#contentHelp-" + helpId).length;

    jq(".help-center").addClass("currentCategory open").toggleClass("active", !hideAll);
    jq(".help-center .menu-sub-item").removeClass("active");

    jq("[id^='contentHelp-']").toggleClass("display-none", hideAll);
    if (hideAll) {
        jq("#contentHelp-" + helpId).removeClass("display-none");
        jq(jq(".help-center .menu-sub-item")[helpId]).addClass("active");
    }
};

jq(function () {
    ASC.Controls.AnchorController.bind(/^help(?:=(\d+))?/, showHelpPage);
});
