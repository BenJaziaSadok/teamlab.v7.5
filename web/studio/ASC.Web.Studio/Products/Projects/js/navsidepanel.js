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

ASC.Projects.navSidePanel = (function() {
    var currentProjectId;
    var init = function() {
        initSidePanelFilters();
        currentProjectId = jq.getURLParam('prjID');

        jq("#createNewTask").click(function() {
            var taskParams = {};
            var taskRespFromHash = jq.getURLParam("tasks_responsible");
            if (currentProjectId && taskRespFromHash && ASC.Projects.Common.userInProjectTeam(taskRespFromHash, ASC.Projects.Master.Team)) {
                taskParams.responsibles = [{id: taskRespFromHash}];
            }
            ASC.Projects.TaskAction.showCreateNewTaskForm(taskParams);
            jq("#createNewButton").hide();
            return false;
        });
        jq("#createNewMilestone").click(function() {
            ASC.Projects.MilestoneAction.showNewMilestonePopup();
            jq("#createNewButton").hide();
            return false;
        });
        jq("#createNewTimer").click(function() {
            var currentCategory = jq(".menu-list").find(".menu-item.currentCategory").attr("id");
            var taskId = jq.getURLParam("ID");
            if (currentProjectId) {
                if ((currentCategory == "menuTasks" || document.location.href.indexOf("timetracking.aspx") > 0) && taskId) {
                    ASC.Projects.Common.showTimer('timer.aspx?prjID=' + currentProjectId + '&ID=' + taskId);
                } else {
                    ASC.Projects.Common.showTimer('timer.aspx?prjID=' + currentProjectId);
                }
            } else {
                ASC.Projects.Common.showTimer('timer.aspx');
            }
            jq("#createNewButton").hide();
            return false;
        });
        jq("#myProjectsConteiner .expander").click(function(event) {
            var menuItem = jq(this).closest(".menu-sub-item");
            if (jq(menuItem).hasClass("open")) {
                jq(menuItem).removeClass("open");
            } else {
                jq(menuItem).addClass("open");
            }
            event.stopPropagation();
        });
        jq(".page-menu #menuMyProjects").click(function() {
            jq(".active").removeClass("active");
            jq(this).closest("div").addClass("active");
        });
    };

    var initSidePanelFilters = function() {
        var currentCategory = jq(".menu-list").find(".menu-item.active").attr("id");
        var hash = document.location.hash;
        var flag = false;
        var date = new Date();
        var deadlineStart = date.getTime();
        date.setDate(date.getDate() + 7);
        var deadlineStop = date.getTime();

        var createdStop = date.getTime();
        date.setDate(date.getDate() - 7);
        var createdStart = date.getTime();
        var path = "";
        var pageProjectsFlag = location.href.indexOf("projects.aspx");
        var pageActionFlag = jq.getURLParam('action');
        if ((!currentProjectId && !pageActionFlag) && pageProjectsFlag > 0) {
            var hash = document.location.hash;
            if (hash.indexOf("team_member=" + Teamlab.profile.id) > 0) {
                if (jq("#myProjectsConteiner").length) {
                    jq(".page-menu #menuMyProjects").closest("div").addClass("active");
                } else {
                    jq(".page-menu #menuMyProjects").closest("li").addClass("active");
                }

                flag = true;
            } else if (hash.indexOf("followed=true") > 0) {
                jq(".page-menu #menuFollowedProjects").parent("li").addClass("active");
                flag = true;
            } else {
                if (hash.indexOf("status=open") > 0) {
                    jq(".page-menu #menuActiveProjects").parent("li").addClass("active");
                    flag = true;
                }
            }
            if (flag) {
                jq("#menuProjects").removeClass("active");
            }
            jq(".page-menu #menuMyProjects").attr("href", "#team_member=" + Teamlab.profile.id + "&status=open");
            jq(".page-menu #menuFollowedProjects").attr("href", "#followed=true&status=open");
            jq(".page-menu #menuActiveProjects").attr("href", "#status=open");
        } else {
            jq(".page-menu #menuMyProjects").attr("href", "projects.aspx#team_member=" + Teamlab.profile.id + "&status=open");
            jq(".page-menu #menuFollowedProjects").attr("href", "projects.aspx#followed=true&status=open");
            jq(".page-menu #menuActiveProjects").attr("href", "projects.aspx#status=open");
        }
        path = "#user_tasks=" + Teamlab.profile.id + "&deadlineStart=" + deadlineStart + "&deadlineStop=" + deadlineStop;
        if (currentCategory == "menuMilestones") {
            flag = false;
            if (hash.indexOf("user_tasks=" + Teamlab.profile.id) > 0 && hash.indexOf("status=open") > 0) {
                jq(".page-menu #menuMyMilestones").parent("li").addClass("active");
                flag = true;
            } else if (hash.indexOf("user_tasks=" + Teamlab.profile.id) > 0 && hash.indexOf("&deadlineStart=") > 0) {
                jq(".page-menu #menuUpcomingMilestones").parent("li").addClass("active");
                flag = true;
            }
            if (flag) {
                jq("#menuMilestones").removeClass("active");
            }
            jq('.page-menu #menuMyMilestones').attr('href', "#user_tasks=" + Teamlab.profile.id + "&status=open");
            jq('.page-menu #menuUpcomingMilestones').attr('href', path);
        } else {
            jq('.page-menu #menuMyMilestones').attr('href', "milestones.aspx#user_tasks=" + Teamlab.profile.id + "&status=open");
            jq('.page-menu #menuUpcomingMilestones').attr('href', "milestones.aspx" + path);
        }

        path = "#tasks_responsible=" + Teamlab.profile.id + "&deadlineStart=" + deadlineStart + "&deadlineStop=" + deadlineStop;
        if (currentCategory == "menuTasks" && jq("#menuTasks").hasClass("open")) {
            flag = false;
            if (hash.indexOf("tasks_responsible=" + Teamlab.profile.id) > 0 && hash.indexOf("status=open") > 0) {
                jq(".page-menu #menuMyTasks").parent("li").addClass("active");
                flag = true;
            } else if (hash.indexOf("tasks_responsible=" + Teamlab.profile.id) > 0 && hash.indexOf("&deadlineStart=") > 0) {
                jq(".page-menu #menuUpcomingTasks").parent("li").addClass("active");
                flag = true;
            }
            if (flag) {
                jq("#menuTasks").removeClass("active");
            }
            jq('.page-menu #menuMyTasks').attr('href', "#tasks_responsible=" + Teamlab.profile.id + "&status=open");
            jq('.page-menu #menuUpcomingTasks').attr('href', path);
        } else {
            jq('.page-menu #menuMyTasks').attr('href', "tasks.aspx#tasks_responsible=" + Teamlab.profile.id + "&status=open");
            jq('.page-menu #menuUpcomingTasks').attr('href', "tasks.aspx" + path);
        }

        path = "#createdStart=" + createdStart + "&createdStop=" + createdStop;
        if (currentCategory == "menuMessages" && jq("#menuMessages").hasClass("open")) {
            flag = false;
            if (hash.indexOf("author=" + Teamlab.profile.id) > 0) {
                jq(".page-menu #menuMyDiscussions").parent("li").addClass("active");
                flag = true;
            } else if (hash.indexOf("createdStart=") > 0) {
                jq(".page-menu #menuLatestDiscussion").parent("li").addClass("active");
                flag = true;
            }
            if (flag) {
                jq("#menuMessages").removeClass("active");
            }
            jq('.page-menu #menuLatestDiscussion').attr('href', path);
            jq('.page-menu #menuMyDiscussions').attr('href', "#author=" + Teamlab.profile.id);
        } else {
            jq('.page-menu #menuLatestDiscussion').attr('href', "messages.aspx" + path);
            jq('.page-menu #menuMyDiscussions').attr('href', "messages.aspx#author=" + Teamlab.profile.id);
        }
    };
    return {
        init: init
    };
})(jQuery);