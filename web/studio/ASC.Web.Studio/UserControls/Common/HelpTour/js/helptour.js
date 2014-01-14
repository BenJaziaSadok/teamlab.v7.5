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

var HelpTourSettings = new function () {
    this.Mask = "<h4>{0}</h4><p>{1}</p>";
    this.Module = "";
    this.Items = [];
    this.Step = -1;

    var setStep = function (step) {
        window.UserHelpTourUsage.SetStep(HelpTourSettings.Module, step);
    };

    var showHelpTour = function () {
        jq(HelpTourSettings.Items).each(function (i, item) {
            item.text = HelpTourSettings.Mask.format(item.header, item.body);
        });

        jq.sets({
            start: HelpTourSettings.Step,
            elems: HelpTourSettings.Items
        });
    };

    this.HelpTourCall = function () {
        if (jq(".dashboard-center-box").length !== 0) {
            jq("[id^=helperSet]").hide();
        }

        jq("#menuSettings, #menuProjects").addClass("open");

        showHelpTour();

        jq(".neverShow").on('click', function () {
            setStep(-1);
            return true;
        });

        jq("body").on("click", ".close-tour", function () {
            setStep(-1);
            return true;
        });

        jq(".nextHelp").on('click', function () {
            var id = jq(this).closest("div[id^=helperSet]").attr("id");
            var num = id.substr(id.indexOf("-") + 1);
            setStep(parseInt(num) + 1);
            return true;
        });
    };
};

jq(document).ready(function () {
    if (jq("[blank-page]").length) {
        jq('#content .close').on('click', function() {
            HelpTourSettings.HelpTourCall();
        });
    } else {
        setTimeout(HelpTourSettings.HelpTourCall, 3000);
    }
});