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

/*******************************************************************************/
if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Projects === "undefined")
    ASC.Projects = {};

ASC.Projects.Common = (function() { // Private Section
    this.prjId = jq.getURLParam("prjID");
    return {// Public Section
        filterParamsForListProjects: { sortBy: "title", sortOrder: "ascending", status: "open", fields: "id,title,security,isPrivate,status,responsible" },
        filterParamsForListMilestones: { sortBy: "deadline", sortOrder: "descending", status: "open", fields: "id,title,deadline" },
        filterParamsForListTasks: { sortBy: "deadline", sortOrder: "ascending" },
        events: { loadTags: "loadTags", loadProjects: "loadProjects", loadTeam: "loadTeam", loadMilestones: "loadMilestones" },
        initApiData: function() {
            if (typeof (ASC.Projects.Master) != 'undefined') {
                if (typeof (ASC.Projects.Master.Projects) != 'undefined' && ASC.Projects.Master.Projects != null) {
                    ASC.Projects.Master.Projects = Teamlab.create('prj-projects', null, ASC.Projects.Master.Projects.response);
                    jq(document).trigger(ASC.Projects.Common.events.loadProjects);
                } else {
                    Teamlab.getPrjProjects({}, {
                        filter: ASC.Projects.Common.filterParamsForListProjects,
                        success: function(param, projects) {
                            ASC.Projects.Master.Projects = projects;
                            jq(document).trigger(ASC.Projects.Common.events.loadProjects);
                        }
                    });
                }
                if (typeof (ASC.Projects.Master.Tags) != 'undefined' && ASC.Projects.Master.Tags != null) {
                    ASC.Projects.Master.Tags = ASC.Projects.Master.Tags.response;
                    jq(document).trigger(ASC.Projects.Common.events.loadTags);
                } else {
                    Teamlab.getPrjTags({}, {
                        success: function(params, tags) {
                            ASC.Projects.Master.Tags = tags;
                            jq(document).trigger(ASC.Projects.Common.events.loadTags);
                        }
                    });
                }
                if (typeof (ASC.Projects.Master.Team) != 'undefined' && ASC.Projects.Master.Team != null) {
                    ASC.Projects.Master.TeamWithBlockedUsers = Teamlab.create('prj-projectpersons', null, ASC.Projects.Master.Team.response);
                    ASC.Projects.Master.Team = ASC.Projects.Common.removeBlockedUsersFromTeam(ASC.Projects.Master.TeamWithBlockedUsers);
                    jq(document).trigger(ASC.Projects.Common.events.loadTeam);
                } else {
                    ASC.Projects.Common.updateProjectTeam();
                }
                if (typeof (ASC.Projects.Master.Milestones) != 'undefined' && ASC.Projects.Master.Milestones != null) {
                    ASC.Projects.Master.Milestones = Teamlab.create('prj-milestones', null, ASC.Projects.Master.Milestones.response);
                    jq(document).trigger(ASC.Projects.Common.events.loadMilestones);
                } else {
                    var filter = ASC.Projects.Common.filterParamsForListMilestones;
                    if (prjId) {
                        filter.projectId = prjId;
                    }
                    Teamlab.getPrjMilestones({}, {
                        filter: filter,
                        success: function(params, milestones) {
                            ASC.Projects.Master.Milestones = milestones;
                            jq(document).trigger(ASC.Projects.Common.events.loadMilestones);
                        }
                    });
                }
            } else {
                ASC.Projects.Master = {};
            }
        },
        bind: function(eventName, handler) {
            jq(document).bind(eventName, handler);
        },
        updateProjectTeam: function () {
            if (prjId)
                Teamlab.getPrjTeam({}, prjId, {
                    success: function (params, team) {
                        ASC.Projects.Master.TeamWithBlockedUsers = team;
                        ASC.Projects.Master.Team = ASC.Projects.Common.removeBlockedUsersFromTeam(ASC.Projects.Master.TeamWithBlockedUsers);
                        jq(document).trigger(ASC.Projects.Common.events.loadTeam);
                    }
                });
        },
        removeBlockedUsersFromTeam: function(team) {
            var newTeam = [];
            if (!team) return newTeam;
            for (var i = 0; i < team.length; i++) {
                if (team[i].status == 1)
                    newTeam.push(team[i]);
            }
            return newTeam;
        },

        showTimer: function(url) {
            var width = 288;
            var height = 618;

            if (jq.browser.safari) {
                height = 560;
            } else if (jq.browser.opera) {
                height = 600;
            }

            if (jq.browser.msie) {
                width = 284;
                height = 600;
            }
            var hWnd = null;
            var isExist = false;

            try {
                hWnd = window.open('', "displayTimerWindow", "width=" + width + ",height=" + height + ",resizable=yes");
            } catch (err) {
            }
            try {
                isExist = typeof hWnd.ASC === 'undefined' ? false : true;
            } catch (err) {
                isExist = true;
            }

            if (!isExist) {
                hWnd = window.open(url, "displayTimerWindow", "width=" + width + ",height=" + height + ",resizable=yes");
                isExist = true;
            }

            if (!isExist) {
                return undefined;
            }
            try {
                hWnd.focus();
            } catch (err) {
            }
        },

        excludeVisitors: function(users) {
            var usersWithoutVisitors = []
            for (var i = 0; i < users.length; i++) {
                var user = users[i];
                if (!user.isVisitor)
                    usersWithoutVisitors.push(user);
            }
            return usersWithoutVisitors;
        },

        showCommentBox: function() {
            if (typeof (FCKeditorAPI) != "undefined" && FCKeditorAPI != null) {
                CommentsManagerObj.AddNewComment();
            } else {
                setTimeout("ASC.Projects.Common.showCommentBox();", 500);
            }
        },
        setPaginationCookie: function(countOnPage, cookieKey) {
            if (cookieKey && cookieKey != "") {
                var cookie = {
                    countOnPage: countOnPage
                };
                jq.cookies.set(cookieKey, cookie, { path: location.pathname });
            }
        },
        userInProjectTeam: function(userId, team) {
            if (!team) return false;
            for (var i = 0; i < team.length; i++) {
                if (team[i].id == userId)
                    return team[i];
            }
            return false;
        },
        currentUserIsModuleAdmin: function(){
            return Teamlab.profile.isAdmin || ASC.Projects.Master.IsModuleAdmin;
        },
        linkTypeEnum: {
            start_start: 0,
            end_end: 1,
            start_end: 2,
            end_start: 3
        },
        getPossibleTypeLink: function (firstTaskStart, firstTaskDeadline, secondTaskStart, secondTaskDeadline, relatedTaskObject) {
            var possibleTypeLinks = [-1, -1, -1, -1];
            possibleTypeLinks[0] = ASC.Projects.Common.linkTypeEnum.start_start; // possible for all tasks

            if (firstTaskDeadline && secondTaskDeadline) {
                possibleTypeLinks[1] = ASC.Projects.Common.linkTypeEnum.end_end; // possible for tasks with deadline

                if (firstTaskDeadline <= secondTaskStart) {
                    possibleTypeLinks[3] = ASC.Projects.Common.linkTypeEnum.end_start;
                } else if (secondTaskDeadline <= firstTaskStart) {
                    possibleTypeLinks[2] = ASC.Projects.Common.linkTypeEnum.start_end;
                } else {
                    relatedTaskObject.invalidLink = true;
                    if (firstTaskStart <= secondTaskStart) {
                        possibleTypeLinks[3] = ASC.Projects.Common.linkTypeEnum.end_start;
                    } else {
                        possibleTypeLinks[2] = ASC.Projects.Common.linkTypeEnum.start_end;
                    }
                }
            } else {
                if (secondTaskDeadline) {
                    possibleTypeLinks[2] = ASC.Projects.Common.linkTypeEnum.start_end;
                    if (secondTaskDeadline > firstTaskStart) {
                        relatedTaskObject.invalidLink = true;
                    }
                } else if (firstTaskDeadline) {
                    possibleTypeLinks[3] = ASC.Projects.Common.linkTypeEnum.end_start;
                    if (firstTaskDeadline < secondTaskStart) {
                        relatedTaskObject.invalidLink = true;
                    }
                }
            }
            return possibleTypeLinks;
        },
        initMobileBanner: function () {
            var data = {};
                Teamlab.isMobileAppUser({}, data, {
                    success: function (params, isShow) {
                        if (!isShow) {
                            jq(".mobileApp-banner").removeClass("display-none");
                        }
                    }
                })
                jq(".mobileApp-banner_btn.app-store").trackEvent("mobileApp-banner", ga_Actions.actionClick, "app-store");
                jq(".mobileApp-banner_btn.google-play").trackEvent("mobileApp-banner", ga_Actions.actionClick, "google-play");
        },
        emptyGuid: "00000000-0000-0000-0000-000000000000",
        defaultPageURL: "projects.aspx"
    };

})();

var MoveTaskQuestionPopup = (function () {
    var isInit = false,
        links = [],
        firstBlockId = "";
    var successFunc = function () { };
    var cancelFunc = function () { };

    var init = function () {
        if (isInit) return;
        isInit = true;

        jq("#removeTaskLinksQuestionPopup .one-move").click(function () {
            for (var j = 0; j < links.length; ++j) {
                var data = { dependenceTaskId: links[j].dependenceTaskId, parentTaskId: links[j].parentTaskId };
                Teamlab.removePrjTaskLink({}, links[j].dependenceTaskId, data, { success: function () { } });
            }
            successFunc();
        });
        jq("#removeTaskLinksQuestionPopup .cancel").click(function () {
            jq.unblockUI();
            cancelFunc();
        });
    };

    var setParametrs = function (firstBlockId, tasklinks, success, cancel) {
        links = tasklinks;
        firstBlockId = firstBlockId;
        successFunc = success;
        cancelFunc = cancel;
    };

    var showDialog = function () {
        jq.unblockUI();
        StudioBlockUIManager.blockUI(jq("#removeTaskLinksQuestionPopup"), "auto", 200, 0, "absolute");
    };

    return {
        init: init,
        setParametrs: setParametrs,
        showDialog: showDialog
    }
})();

jq(document).ready(function () {
    if (jq(".mobileApp-banner").length) {
        ASC.Projects.Common.initMobileBanner();
    }
    ASC.Projects.Common.initApiData();
    MoveTaskQuestionPopup.init();
});


// Google Analytics const
var ga_Categories = {
    projects: "projects",
    milestones: "projects_milestones",
    tasks: "projects_tasks",
    subtask: "projects_subtask",
    discussions: "projects_discussions",
    timeTrack: "projects_time-track",
    dashboard: "projects_dashboard",
    projectTemplate: "projects_template"
};

var ga_Actions = {
    filterClick: "filter-click",
    createNew: "create-new",
    remove: "remove",
    edit: "edit",
    view: "view",
    changeStatus: "change-status",
    next: "next",
    userClick: "user-click",
    actionClick: "action-click",
    quickAction: "quick-action"
};
// end Google Analytics