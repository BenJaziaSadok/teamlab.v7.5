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

ASC.Projects.projectNavPanel = (function() {
    var projectModulesNames = {
        tasks: "tasksModule",
        milestones: "milestonesModule",
        messages: "messagesModule",
        timeTraking: "timetrackingModule",
        docs: "tmdocsModule",
        team: "projectteamModule",
        contacts: "contactsModule"
    };

    var init = function() {

        if (isNeedResize()) resizeTitle();

        var projectId = jq.getURLParam("PrjID");
        jq('#questionWindowDelProj .remove').bind('click', function() {
            var projectId = jq.getURLParam("prjID");
            deleteProject(projectId);
            return false;
        });
        jq('#questionWindowDelProj .cancel, .projectDescriptionPopup .cancel').bind('click', function() {
            jq.unblockUI();
            return false;
        });
        jq('#deleteProject').click(function() {
            jq("#projectActions").hide();
            jq(".project-title .menu-small").removeClass("active");
            showQuestionWindow();
            return false;
        });
        jq("#viewDescription").click(function() {
            jq("#projectActions").hide();
            jq(".project-title .menu-small").removeClass("active");
            showProjectDescription();
            return false;
        });
        jq("#followProject").click(function() {
            if (!jq(this).attr("data-followed")) {
                Teamlab.subscribeProject({ followed: true }, projectId, { success: onSubscribeProject });
            } else {
                Teamlab.subscribeProject({ followed: false }, projectId, { success: onSubscribeProject });
            }
            return false;
        });
        jq(window).resize(function() {
            if (isNeedResize) resizeTitle();
        });
        jq("body").click(function () {
            jq(".project-title .menu-small").removeClass("active");
        });
        jq(".gant-chart-link").trackEvent("gantt-chart", ga_Actions.actionClick, "gantt-btn");
    };

    var resizeTitle = function() {
        var windowWidth = jq(window).width(),
            blockWidth = parseInt(jq(".mainPageLayout").css("min-width"), 10),
            newWidth = (windowWidth < blockWidth) ? blockWidth: windowWidth;
            jq(".project-title #essenceTitle").css("max-width", newWidth - getConstantWidth() + "px").addClass("truncated-text");
    };

    var getConstantWidth = function () {
        var paddings = 24 * 4;
        var magicValue = 130;
        return paddings + magicValue + jq(".mainPageTableSidePanel").width() + jq(".gant-chart-link").width();
    };

    var isNeedResize = function() {
        var title = jq(".project-title #essenceTitle");
        var text = jq.trim(title.text());
        var words = text.split(" ");
        if (words.length < 3 && title.width() > jq(window).width() - getConstantWidth()) {
            return true;
        }
        return false;
    };

    var changeModuleItemsCount = function(moduleName, actionOrCount) {
        var currentCount;
        var countContainer = jq("#" + moduleName);
        if (!countContainer) return;

        var text = jq.trim(countContainer.text());
        if (text == "") currentCount = 0;
        else {
            text = text.substr(1, text.length - 2);
            currentCount = parseInt(text);
        }

        if (typeof (actionOrCount) == "string") {
            if (actionOrCount == "add") {
                currentCount++;
                countContainer.text(" (" + currentCount + ")");
            } else if (actionOrCount == "delete") {
                currentCount--;
                if (currentCount != 0) {
                    countContainer.text(" (" + currentCount + ")");
                } else {
                    countContainer.empty();
                }
            }
        } else {
            countContainer.text(" (" + actionOrCount + ")");
        }
    };

    var changeCommonProjectTime = function(time) {
        var countContainer = jq("#" + projectModulesNames.timeTraking);
        if (!countContainer) return;

        var currentTime = { hours: 0, minutes: 0 };
        var text = jq.trim(countContainer.text());
        if (text != "") {
            text = text.substr(1, text.length - 2);
            text = text.split(":");
            currentTime.hours = parseInt(text[0], 10);
            currentTime.minutes = parseInt(text[1], 10);
        }

        currentTime.hours += time.hours;
        currentTime.minutes += time.minutes;

        if (currentTime.minutes > 59) {
            currentTime.hours += 1
            currentTime.minutes -= 60;
        }
        if (currentTime.minutes < 0) {
            currentTime.hours -= 1
            currentTime.minutes += 60;
        }

        if (currentTime.hours == 0 && currentTime.minutes == 0) countContainer.empty();

        if (currentTime.minutes < 10) {
            countContainer.text(" (" + currentTime.hours + ":0" + currentTime.minutes + ")");
        } else {
            countContainer.text(" (" + currentTime.hours + ":" + currentTime.minutes + ")");
        }

    };

    var onSubscribeProject = function(params) {
        jq("#projectActions").hide();
        jq(".project-title .menu-small").removeClass("active");
        var followLink = jq("#followProject");
        var currentText = jq.trim(followLink.attr('title'));
        var newText = followLink.attr("data-text");
        followLink.attr('title', newText);
        followLink.attr("data-text", currentText);
        if (params.followed) {
            followLink.attr("data-followed", "followed");
            followLink.removeClass("unsubscribed").addClass("subscribed");
        } else {
            followLink.removeAttr("data-followed");
            followLink.removeClass("subscribed").addClass("unsubscribed");
        }
    };

    var deleteProject = function(projectId) {
        var params = {};
        jq("#questionWindowDelProj").find('.middle-button-container').addClass('display-none');
        jq("#questionWindowDelProj").find('.pm-ajax-info-block').removeClass('display-none');
        Teamlab.removePrjProject(params, projectId, { success: onDeleteProject, error: onDeleteProjectError });
    };

    var onDeleteProject = function() {
        document.location.replace("projects.aspx");
    };

    var onDeleteProjectError = function() {
        jq("#questionWindowDelProj").find('.middle-button-container').removeClass('display-none');
        jq("#questionWindowDelProj").find('.pm-ajax-info-block').addClass('display-none');
        jq.unblockUI();
    };
    var showQuestionWindow = function() {
        StudioBlockUIManager.blockUI(jq("#questionWindowDelProj"), 400, 300, 0);
    };
    var showProjectDescription = function() {     
        var prjInfoDescr = jq("#prjInfoDescription");
        if (prjInfoDescr.length) {
            var description = prjInfoDescr.attr("data-description").trim();
            if (description.length) {
                var newText = jq.linksParser(description.replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>').replace('&amp;', '&'));
                jq("#prjInfoDescription").html(newText);
            }
        }
        StudioBlockUIManager.blockUI(jq('.projectDescriptionPopup'), 560, 300, 0);
    };
    return {
        init: init,
        resizeTitle: resizeTitle,
        isNeedResize: isNeedResize,
        changeModuleItemsCount: changeModuleItemsCount,
        projectModulesNames: projectModulesNames,
        changeCommonProjectTime: changeCommonProjectTime
    };
})(jQuery);

jq(document).ready(function() {
    ASC.Projects.projectNavPanel.init();
});