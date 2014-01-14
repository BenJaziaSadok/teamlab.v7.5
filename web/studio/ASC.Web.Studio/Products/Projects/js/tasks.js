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

ASC.Projects.TasksManager = (function() {
    var isInit = false,

        currentUserId,
        currentProjectId,

        currentFilter,

        filteredTasks = [],

        loadListProjectsFlag = false,
        loadListTagsFlag = false,
        loadListMilestonesFlag = false,
        loadTeamFlag = false,

        taskDescriptionTimeout = 0,
        overTaskDescriptionPanel = false,

        selectedStatusCombobox = undefined,
        statusListContainer = undefined,

        projectParticipants = undefined,

        basePath = 'sortBy=deadline&sortOrder=ascending',

        allTaskCount,
        filterTaskCount = 0,

    //pagination
        cookiePaginationKey,
        currentPage = 0,
        entryCountOnPage,
        pageCount;

    var getCurrentUserId = function() {
        return currentUserId;
    };

    var getCurrentProjectId = function() {
        return currentProjectId;
    };

    var onSetFilterTasks = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        var hash = ASC.Controls.AnchorController.getAnchor();
        if (ASC.Projects.ProjectsAdvansedFilter.firstload && hash.length) {
            if (!ASC.Projects.ProjectsAdvansedFilter.coincidesWithFilter(path)) {
                ASC.Projects.ProjectsAdvansedFilter.setFilterByUrl();
                return;
            }
        }
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            ASC.Projects.ProjectsAdvansedFilter.firstload = false;
        }

        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        currentFilter = filter;

        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
        if (!allTaskCount) {
            emptyScreenList();
            LoadingBanner.hideLoading();
        }
        else {
            LoadingBanner.displayLoading();
            getTasks(filter, false);
        }
    };

    var onResetFilterTasks = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        currentFilter = filter;
        getTasks(filter, false);
        LoadingBanner.displayLoading();
    };

    var createAdvansedFilter = function() {
        var now = new Date(),
            today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0),
            inWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        inWeek.setDate(inWeek.getDate() + 7);

        var filters = [];

        // Responsible
        if (currentProjectId) {
            if (ASC.Projects.Common.userInProjectTeam(currentUserId, ASC.Projects.Master.Team)) {
                filters.push({
                    type: "combobox",
                    id: "me_tasks_responsible",
                    title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                    filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                    group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                    options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                    hashmask: "person/{0}",
                    groupby: "userid",
                    bydefault: { value: currentUserId }
                });
            }
            filters.push({
                type: "combobox",
                id: "tasks_responsible",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherParticipant,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                hashmask: "person/{0}",
                groupby: "userid",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
        } else {
            filters.push({
                type: "person",
                id: "me_tasks_responsible",
                title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid",
                bydefault: { id: currentUserId }
            });
            filters.push({
                type: "person",
                id: "tasks_responsible",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid"
            });
        }
        filters.push({
            type: "group",
            id: "group",
            title: ASC.Projects.Resources.ProjectsFilterResource.Groups,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Group + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
            hashmask: "group/{0}",
            groupby: "userid"
        });
        filters.push({
            type: "flag",
            id: "noresponsible",
            title: ASC.Projects.Resources.ProjectsFilterResource.NoResponsible,
            group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
            hashmask: "noresponsible",
            groupby: "userid"
        });

        // Creator
        filters.push({
            type: "person",
            id: "me_tasks_creator",
            title: ASC.Projects.Resources.ProjectsFilterResource.Me,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByCreator + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByCreator,
            hashmask: "person/{0}",
            groupby: "creatorid",
            bydefault: { id: currentUserId }
        });
        filters.push({
            type: "person",
            id: "tasks_creator",
            title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByCreator + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByCreator,
            hashmask: "person/{0}",
            groupby: "creatorid"
        });
        //Projects
        if (!currentProjectId) {
            filters.push({
                type: "flag",
                id: "myprojects",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyProjects,
                group: ASC.Projects.Resources.ProjectsFilterResource.ByProject,
                hashmask: "myprojects",
                groupby: "projects"
            });
            filters.push({
                type: "combobox",
                id: "project",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherProjects,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByProject + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByProject,
                options: ASC.Projects.ProjectsAdvansedFilter.getProjectsForFilter(),
                hashmask: "project/{0}",
                groupby: "projects",
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
            filters.push({
                type: "combobox",
                id: "tag",
                title: ASC.Projects.Resources.ProjectsFilterResource.ByTag,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Tag + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByProject,
                options: ASC.Projects.ProjectsAdvansedFilter.getTagsForFilter(),
                groupby: "projects",
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
        }
        //Milestones
        var milestones = ASC.Projects.ProjectsAdvansedFilter.getMilestonesForFilter();
        if (milestones.length > 1) {
            filters.push({
                type: "flag",
                id: "mymilestones",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyMilestones,
                group: ASC.Projects.Resources.ProjectsFilterResource.ByMilestone,
                hashmask: "mymilestones",
                groupby: "milestones"
            });
            filters.push({
                type: "combobox",
                id: "milestone",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherMilestones,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByMilestone + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByMilestone,
                hashmask: "milestone/{0}",
                groupby: "milestones",
                options: milestones,
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
        }
        // Status
        filters.push({
            type: "combobox",
            id: "open",
            title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenTask,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
            hashmask: "combobox/{0}",
            groupby: "status",
            options:
                [
                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenTask, def: true },
                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedTask }
                ]
        });
        filters.push({
            type: "combobox",
            id: "closed",
            title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedTask,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
            hashmask: "combobox/{0}",
            groupby: "status",
            options:
                [
                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenTask },
                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedTask, def: true }
                ]
        });
        //Due date
        filters.push({
            type: "flag",
            id: "overdue",
            title: ASC.Projects.Resources.ProjectsFilterResource.Overdue,
            group: ASC.Projects.Resources.ProjectsFilterResource.DueDate,
            hashmask: "overdue",
            groupby: "deadline"
        });
        filters.push({
            type: "daterange",
            id: "today",
            title: ASC.Projects.Resources.ProjectsFilterResource.Today,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.DueDate,
            hashmask: "deadline/{0}/{1}",
            groupby: "deadline",
            bydefault: { from: today.getTime(), to: today.getTime() }
        });
        filters.push({
            type: "daterange",
            id: "upcoming",
            title: ASC.Projects.Resources.ProjectsFilterResource.UpcomingMilestones,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.DueDate,
            hashmask: "deadline/{0}/{1}",
            groupby: "deadline",
            bydefault: { from: today.getTime(), to: inWeek.getTime() }
        });
        filters.push({
            type: "daterange",
            id: "deadline",
            title: ASC.Projects.Resources.ProjectsFilterResource.CustomPeriod,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.DueDate,
            hashmask: "deadline/{0}/{1}",
            groupby: "deadline"
        });

        var colCount = 2;
        if (!currentProjectId && milestones.length > 1) colCount = 3;

        ASC.Projects.ProjectsAdvansedFilter.filter = jq('#ProjectsAdvansedFilter').advansedFilter(
        {
            store: true,
            anykey: true,
            colcount: colCount,
            maxlength: "100",
            anykeytimeout: 1000,
            filters: filters,
            sorters:
            [
                { id: "deadline", title: ASC.Projects.Resources.ProjectsFilterResource.ByDeadline, sortOrder: "ascending", def: true },
                { id: "priority", title: ASC.Projects.Resources.ProjectsFilterResource.ByPriority, sortOrder: "descending" },
                { id: "create_on", title: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate, sortOrder: "descending" },
                { id: "start_date", title: ASC.Projects.Resources.ProjectsFilterResource.ByStartDate, sortOrder: "descending" },
                { id: "title", title: ASC.Projects.Resources.ProjectsFilterResource.ByTitle, sortOrder: "ascending" }
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

        ASC.Projects.ProjectsAdvansedFilter.init = true;

        //filter
        ASC.Projects.ProjectsAdvansedFilter.filter.one("adv-ready", function () {
            var projectAdvansedFilterContainer = jq("#ProjectsAdvansedFilter .advansed-filter-list");
            projectAdvansedFilterContainer.find("li[data-id='me_tasks_responsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'me-tasks-responsible');
            projectAdvansedFilterContainer.find("li[data-id='tasks_responsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'tasks-responsible');
            projectAdvansedFilterContainer.find("li[data-id='group'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'group');
            projectAdvansedFilterContainer.find("li[data-id='noresponsible'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'noresponsible');

            projectAdvansedFilterContainer.find("li[data-id='open'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'open');
            projectAdvansedFilterContainer.find("li[data-id='closed'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'closed');

            projectAdvansedFilterContainer.find("li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'my-projects');
            projectAdvansedFilterContainer.find("li[data-id='project'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'project');
            projectAdvansedFilterContainer.find("li[data-id='tag'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'tag');

            projectAdvansedFilterContainer.find("li[data-id='overdue'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'overdue');
            projectAdvansedFilterContainer.find("li[data-id='today'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'today');
            projectAdvansedFilterContainer.find("li[data-id='upcoming'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'upcoming');
            projectAdvansedFilterContainer.find("li[data-id='deadline'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'user-period');

            projectAdvansedFilterContainer.find("li[data-id='mymilestones'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'my-milestones');
            projectAdvansedFilterContainer.find("li[data-id='milestone'] .inner-text").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'milestone');

            jq("#ProjectsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, 'sort');
            jq("#ProjectsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.tasks, ga_Actions.filterClick, "search_text", "enter");
        });
    };

    var initPageNavigatorControl = function () {
        window.taskListPgNavigator = new ASC.Controls.PageNavigator.init("taskListPgNavigator", "#divForTaskPager", entryCountOnPage, pageCount, 1,
            ASC.Projects.Resources.ProjectsJSResource.PreviousPage, ASC.Projects.Resources.ProjectsJSResource.NextPage);
        window.taskListPgNavigator.NavigatorParent = '#divForTaskPager';
        window.taskListPgNavigator.changePageCallback = function(page) {
            currentPage = page - 1;
            LoadingBanner.displayLoading();
            getTasks(currentFilter, true);
        };
    };

    var updatePageNavigatorControl = function() {
        jq("#totalCount").text(filterTaskCount);
        window.taskListPgNavigator.drawPageNavigator(currentPage + 1, filterTaskCount);

        ASC.Projects.Common.setPaginationCookie(entryCountOnPage, cookiePaginationKey);

        jq("#tableForNavigation").show();
        renderSimplePageNavigator();
    };

    var changeCountOfRows = function(newValue) {
        if (isNaN(newValue)) {
            return;
        }
        var newCountOfRows = newValue * 1;
        entryCountOnPage = newCountOfRows;
        window.taskListPgNavigator.EntryCountOnPage = newCountOfRows;

        LoadingBanner.displayLoading();
        getTasks(currentFilter, false);
    };

    var renderSimplePageNavigator = function() {
        jq(".simplePageNavigator").html("");
        var $simplePN = jq("<div></div>");
        var lengthOfLinks = 0;
        if (jq("#tableForNavigation .pagerPrevButtonCSSClass").length != 0) {
            lengthOfLinks++;
            jq("#tableForNavigation .pagerPrevButtonCSSClass").clone().appendTo($simplePN);
        }
        if (jq("#tableForNavigation .pagerNextButtonCSSClass").length != 0) {
            lengthOfLinks++;
            if (lengthOfLinks === 2) {
                jq("<span style='padding: 0 8px;'>&nbsp;</span>").clone().appendTo($simplePN);
            }
            jq("#tableForNavigation .pagerNextButtonCSSClass").clone().appendTo($simplePN);
        }
        if ($simplePN.children().length != 0) {
            $simplePN.appendTo(".simplePageNavigator");
            jq(".simplePageNavigator").show();
        }
        else {
            jq(".simplePageNavigator").hide();
        }
    };

    var init = function (countOfPage, cookieKey, entryPageCount) {
        if (isInit === false) {
            isInit = true;
        }
        var taskCount = jq("#SubTasksBody").attr("data-task-count");
        if (!taskCount) taskCount = 0;
        allTaskCount = parseInt(taskCount);

        currentUserId = Teamlab.profile.id;
        currentProjectId = jq.getURLParam('prjID');

        ASC.Projects.SubtasksManager.init();
        ASC.Projects.SubtasksManager.onAddSubtaskHandler = onAddSubtask;
        ASC.Projects.SubtasksManager.onRemoveSubtaskHandler = onRemoveSubtask;
        ASC.Projects.SubtasksManager.onChangeTaskStatusHandler = onUpdateSubtaskStatus;

        //page navigator
        entryCountOnPage = countOfPage;
        pageCount = entryPageCount;
        cookiePaginationKey = cookieKey;
        jq("#countOfRows").val(entryCountOnPage).tlCombobox();
        initPageNavigatorControl();

        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(currentUserId, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterTasks;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterTasks;

        LoadingBanner.displayLoading();

        statusListContainer = jq('#statusListContainer');

        // waiting data from api
        jq(document).bind("loadApiData", function() {
            if (loadListProjectsFlag && loadListTagsFlag && loadListMilestonesFlag && loadTeamFlag) {
                createAdvansedFilter();
            }
        });

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadProjects, function() {
            loadListProjectsFlag = true;
            jq(document).trigger("loadApiData");
        });

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTags, function() {
            loadListTagsFlag = true;
            jq(document).trigger("loadApiData");
        });

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadMilestones, function() {
            loadListMilestonesFlag = true;
            updateMilestonesListForMovePanel(ASC.Projects.Master.Milestones);
            jq(document).trigger("loadApiData");
        });

        if (currentProjectId) {
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function() {
                projectParticipants = ASC.Projects.Master.Team;
                loadTeamFlag = true;
                jq(document).trigger("loadApiData");
            });
        } else {
            loadTeamFlag = true;
            jq(document).trigger("loadApiData");
        }

        //

        jq("#countOfRows").change(function(evt) {
            changeCountOfRows(this.value);
        });

        jq('#questionWindow .end').on('click', function() {
            closeTask(jq(this).attr('taskid'));
            jq.unblockUI();
            return false;
        });

        var $taskListContainer = jq('.taskList');
        $taskListContainer.on('click', '.changeStatusCombobox.canEdit', function(event) {
            var status = jq(this).find('span:first').attr('class');
            var current = jq(this).attr('taskid');
            jq('.taskList .task[taskid=' + current + '] .changeStatusCombobox').removeClass('selected');
            selectedStatusCombobox = jq(this);

            if (statusListContainer.attr('taskid') !== selectedStatusCombobox.attr('taskid')) {
                statusListContainer.attr('taskid', selectedStatusCombobox.attr('taskid'));
            }

            showStatusListContainer(status);

            return false;
        });

        jq('#statusListContainer li').on('click', function() {
            if (jq(this).is('.selected')) return;
            var taskid = jq('#statusListContainer').attr('taskid');
            var status = jq(this).attr('class').split(" ")[0];
            if (status == jq('.taskList .task[taskid=' + taskid + '] .changeStatusCombobox span').attr('class')) return;
            if (status == 'closed') {
                if (jq('.taskList .subtask[taskid=' + taskid + ']').length &&
                jq('.taskList .subtask[taskid=' + taskid + ']').length != jq('.taskList .subtask.closed[taskid=' + taskid + ']').length) {
                    popupWindow(taskid);
                } else {
                    closeTask(taskid);
                }
            } else {
                jq('.taskList .task[taskid=' + taskid + '] .check').html('');
                jq('.taskList .task[taskid=' + taskid + '] .check').append('<div class="taskProcess"></div>');
                updateTaskStatus({}, taskid, 1);
            }
            ASC.Projects.TasksManager.resize();
        });

        $taskListContainer.on('click', '.task .user', function(event) {
            var userid = jq(this).attr('userId');
            if (userid != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
                var path;
                if (jq(this).hasClass('not')) {
                    path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'noresponsible', true);
                    path = jq.removeParam('tasks_responsible', path);
                } else {
                    path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_responsible', userid);
                    path = jq.removeParam('noresponsible', path);
                }
                path = jq.removeParam('group', path);
                ASC.Controls.AnchorController.move(path);
            }
            event.stopPropagation();
        });

        jq('#othersListPopup').on('click', '.user', function() {
            var userid = jq(this).attr('userId');
            if (userid != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
                var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_responsible', userid);
                path = jq.removeParam('noresponsible', path);
                ASC.Controls.AnchorController.move(path);
            }
        });

        jq('#taskDescrPanel .project .value').on('click', function() {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', jq(this).attr('projectid'));
            path = jq.removeParam('milestone', path);
            path = jq.removeParam('myprojects', path);
            ASC.Controls.AnchorController.move(path);
        });

        jq('#taskDescrPanel .milestone .value').on('click', function() {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'milestone', jq(this).attr('milestone'));
            ASC.Controls.AnchorController.move(path);
        });

        jq('.addTask').on('click', function() {
            showNewTaskPopup();
            return false;
        });

        jq("#emptyTaskListScreen .addFirstElement").click(function() {
            showNewTaskPopup();
            return false;
        });

        jq(window).resize(function() {
            ASC.Projects.TasksManager.resize();
        });

        jq("#emptyScreenForFilter").on('click', '.clearFilterButton', function() {
            ASC.Controls.AnchorController.move(basePath);
            return false;
        });

        $taskListContainer.on('click', '.task .other', function(event) {
            jq('#othersListPopup').html(jq('.taskList .task .others[taskid="' + jq(this).attr('taskid') + '"]').html());
            showActionsPanel('othersPanel', this);
            event.stopPropagation();
        });

        jq('#moveTaskPanel .blue').on('click', function() {
            var data = {},
                taskId = parseInt(jq("#moveTaskPanel").attr("taskid"), 10),
                task = getFilteredTaskById(taskId);

            data.newMilestoneID = jq('#moveTaskPanel .milestonesList input:checked').attr('value');

            if(task.milestoneId == parseInt(data.newMilestoneID, 10)){
                jq.unblockUI();
                return false;
            }

            if (task.links) {
                MoveTaskQuestionPopup.setParametrs("#moveTaskPanel", task.links,
                                                    function () {
                                                        Teamlab.updatePrjTask({}, taskId, data, { success: onUpdateTask });
                                                    },
                                                    function () {
                                                        jq.unblockUI();
                                                        StudioBlockUIManager.blockUI(jq('#moveTaskPanel'), 550, 300, 0);
                                                    });
                MoveTaskQuestionPopup.showDialog();
            } else {
                Teamlab.updatePrjTask({}, taskId, data, { success: onUpdateTask });
                jq.unblockUI();
            }
        });

        jq('#taskActionPanel #ta_accept').on('click', function() {
            jq('.studio-action-panel').hide();

            var taskId = jq('#taskActionPanel').attr('objid');
            var taskRow = jq('.task[taskid=' + taskId + ']');
            var taskLink = taskRow.find(".taskName a");

            var data = {};
            data.title = jq.trim(taskRow.find(".taskName a").text());

            var deadline = taskLink.attr("data-deadline");
            if (deadline) {
                data.deadline = new Date(deadline);
                data.deadline.setHours(0);
                data.deadline.setMinutes(0);
                data.deadline = Teamlab.serializeTimestamp(data.deadline);
            }
            var description = taskLink.attr("description");
            if (description) {
                data.description = description;
            }
            var milestoneId = taskLink.attr("milestoneid");
            if (milestoneId) {
                data.milestoneid = milestoneId;
            }

            data.priority = taskRow.find(".high_priority").length ? 1 : 0;
            data.responsibles = [currentUserId];

            Teamlab.updatePrjTask({}, taskId, data, {success: onUpdateTask});
            return false;
        });

        jq('#taskActionPanel #ta_edit').on('click', function() {
            jq(".studio-action-panel").hide();
            var taskId = jq('#taskActionPanel').attr('objid');
            ASC.Projects.TaskAction.showUpdateTaskForm(taskId);
            return false;
        });

        jq('#taskActionPanel #ta_subtask').on('click', function() {
            ASC.Projects.SubtasksManager.hideSubtaskFields();
            var taskid = jq('#taskActionPanel').attr('objid');
            var subtaskCont = jq('.subtasks[taskid=' + taskid + ']');

            if (!jq(subtaskCont).is(':visible')) {
                separateSubtasks(taskid);
            }

            ASC.Projects.SubtasksManager.addFirstSubtask(subtaskCont.find(".quickAddSubTaskLink"));

            jq('.studio-action-panel').hide();
            jq('.taskList .task').removeClass('menuopen');
            return false;
        });

        jq('#taskActionPanel #ta_move').on('click', function() {
            jq('.studio-action-panel').hide();
            if (!currentProjectId) {
                getMilestonesForMovePanel({}, jq(this).attr('projectid'));
            } else {
                showMoveToMilestonePanel();
            }
            return false;
        });

        jq('#taskActionPanel #ta_remove').on('click', function() {
            var taskId = jq('#taskActionPanel').attr('objid');
            jq('.studio-action-panel').hide();
            showQuestionWindowTaskRemove(taskId);
            return false;
        });

        jq('#taskActionPanel #ta_mesres').on('click', function() {
            var taskId = jq('#taskActionPanel').attr('objid');
            notifyTaskResponsible({}, taskId);
            jq('.studio-action-panel').hide();
            return false;
        });

        jq('#taskActionPanel #ta_time').on('click', function() {
            jq('.studio-action-panel').hide();
            var taskId = jq('#taskActionPanel').attr('objid');
            var projectId = jq('#taskActionPanel #ta_time').attr('projectid');
            var user = jq('#taskActionPanel #ta_time').attr('userid');
            if (!user) {
                user = currentUserId;
            }
            ASC.Projects.Common.showTimer('timer.aspx?prjID=' + projectId + '&taskId=' + taskId + '&userID=' + user);
            return false;
        });

        $taskListContainer.on('mouseenter', '.task .taskName a', function(event) {
            taskDescriptionTimeout = setTimeout(function() {
                var targetObject = event.target;
                jq('#taskDescrPanel .descr .value .readMore').hide();
                jq('#taskDescrPanel .date, #taskDescrPanel .startdate, #taskDescrPanel .milestone, #taskDescrPanel .descr, #taskDescrPanel .createdby, #taskDescrPanel .descr, #taskDescrPanel .closed, #taskDescrPanel .descr, #taskDescrPanel .closedby').hide();
                if (jq(targetObject).attr('status') == 2) {
                    if (typeof jq(targetObject).attr('updated') != 'undefined') {
                        if (jq(targetObject).attr('updated').length) {
                            jq('#taskDescrPanel .closed .value').html(jq(targetObject).attr('updated').substr(0, 10));
                            jq('#taskDescrPanel .closed').show();
                        }
                        if (jq(targetObject).attr('createdby').length) {
                            jq('#taskDescrPanel .closedby .value').html(jq(targetObject).attr('createdby'));
                            jq('#taskDescrPanel .closedby').show();
                        }
                    }
                } else {
                    if (typeof jq(targetObject).attr('created') != 'undefined') {
                        if (jq(targetObject).attr('created').length) {
                            jq('#taskDescrPanel .date .value').html(jq(targetObject).attr('created').substr(0, 10));
                            jq('#taskDescrPanel .date').show();
                        }
                    }
                    if (typeof jq(targetObject).attr('createdby') != 'undefined') {
                        jq('#taskDescrPanel .createdby .value').html(Encoder.htmlEncode(jq(targetObject).attr('createdby')));
                        jq('#taskDescrPanel .createdby').show();
                    }
                }
                if (jq(targetObject).attr('data-start').length) {
                    jq('#taskDescrPanel .startdate .value').html(jq(targetObject).attr('data-start'));
                    jq('#taskDescrPanel .startdate').show();
                }
                if (jq('#taskDescrPanel .project').length) {
                    jq('#taskDescrPanel .project .value').html('<span class="descr_milestone">' + Encoder.htmlEncode(jq(targetObject).attr('project')) + '</span>');
                    jq('#taskDescrPanel .project .value').attr('projectid', jq(targetObject).attr('projectid'));
                }
                if (typeof jq(targetObject).attr('milestone') != 'undefined') {
                    jq('#taskDescrPanel .milestone .value').html('<span class="descr_milestone">' + Encoder.htmlEncode(jq(targetObject).attr('milestone')) + '</span>');
                    jq('#taskDescrPanel .milestone .value').attr('projectid', jq(targetObject).attr('projectid'));
                    jq('#taskDescrPanel .milestone .value').attr('milestone', jq(targetObject).attr('milestoneid'));
                    jq('#taskDescrPanel .milestone').show();
                    jq('#taskDescrPanel .milestone .value').removeClass('deadline_active').removeClass('deadline_late');
                    if (jq('.taskList .task[taskid=' + jq(targetObject).attr('taskid') + '] .deadline').length) jq('#taskDescrPanel .milestone .value').addClass('deadline_active');
                }
                var description = jq(targetObject).attr('description');
                if (jq.trim(description) != '') {
                    jq('#taskDescrPanel .descr .value div').html(jq.linksParser(jq.htmlEncodeLight(jq(targetObject).attr('description'))));
                    if (description.indexOf("\n") > 2 || description.length > 80) {
                        var link = "tasks.aspx?prjID=" + jq(targetObject).attr('projectid') + "&id=" + jq(targetObject).attr('taskid');
                        jq('#taskDescrPanel .descr .value .readMore').attr("href", link);
                        jq('#taskDescrPanel .descr .value .readMore').show();
                    }
                    jq('#taskDescrPanel .descr').show();
                }

                showActionsPanel('taskDescrPanel', targetObject);
                overTaskDescriptionPanel = true;
            }, 400, this);
        });

        $taskListContainer.on('mouseleave', '.task .taskName a', function() {
            clearTimeout(taskDescriptionTimeout);
            overTaskDescriptionPanel = false;
            hideDescriptionPanel();
        });

        jq('#taskDescrPanel').on('mouseenter', function() {
            overTaskDescriptionPanel = true;
        });

        jq('#taskDescrPanel').on('mouseleave', function() {
            overTaskDescriptionPanel = false;
            hideDescriptionPanel();
        });

        $taskListContainer.on('click', '.task .entity-menu', function() {
            ASC.Projects.SubtasksManager.hideSubtaskActionPanel();
            ASC.Projects.SubtasksManager.hideSubtaskFields();

            jq('.taskList .task').removeClass('menuopen');
            if (jq('#taskActionPanel:visible').length) jq(this).closest(".task").removeClass('menuopen'); else jq(this).closest(".task").addClass('menuopen');
            showActionsPanel('taskActionPanel', this);

            // ga-track
            try {
                if (window._gat) {
                    window._gaq.push(['_trackEvent', ga_Categories.tasks, ga_Actions.actionClick, "task-menu"]);
                }
            } catch (err) {
            }
            return false;
        });

        $taskListContainer.on('click', '.subtasksCount span.expand', function() {
            hideTaskActionPanel();
            ASC.Projects.SubtasksManager.hideSubtaskFields();

            var taskId = jq(this).attr('taskid');
            if (showOrHideListSubtasks(taskId)) {
                jq(this).attr('class', 'collaps');
            }
            return false;
        });

        $taskListContainer.on('click', '.subtasksCount span.collaps', function() {
            hideTaskActionPanel();
            ASC.Projects.SubtasksManager.hideSubtaskFields();

            var taskId = jq(this).attr('taskid');
            if (showOrHideListSubtasks(taskId)) {
                jq(this).attr('class', 'expand');
            }
            return false;
        });

        $taskListContainer.on('click', '.task', function(event) {
            hideTaskActionPanel();
            ASC.Projects.SubtasksManager.hideSubtaskFields();

            var elt = (event.target) ? event.target : event.srcElement;
            if (jq(elt).is('a')) {
                return undefined;
            }
            var taskid = jq(jq(this).find('.taskName')).attr('taskid');
            jq(this).find(".expand").attr("class", "collaps");
            showOrHideListSubtasks(taskid);

            return false;
        });

        $taskListContainer.on('click', '.subtasksCount span.add', function(event) {
            hideTaskActionPanel();
            ASC.Projects.SubtasksManager.hideSubtaskFields();
            event.stopPropagation();

            var taskid = jq(this).attr('taskid');
            var subtaskCont = jq('.subtasks[taskid=' + taskid + ']');

            showOrHideListSubtasks(taskid, true);

            ASC.Projects.SubtasksManager.addFirstSubtask(subtaskCont.find(".quickAddSubTaskLink"));
        });

        jq('body').bind("click", function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            var $elt = jq(elt);

            if (
              $elt.is('.studio-action-panel') ||
			  $elt.is('#taskName') ||
			  $elt.is('.subtask-name-input') ||
			  $elt.is('.choose') ||
			  $elt.is('.choose > *') ||
			  $elt.is('.combobox-title') ||
			  $elt.is('.combobox-title-inner-text') ||
			  $elt.is('.option-item')
			) {
                isHide = false;
            }

            if (isHide) {
                hideStatusListContainer();
            }
        });

        jq('#questionWindowTaskRemove .cancel, #questionWindow .cancel, #remindAboutTask .ok, #moveTaskPanel .gray').on('click', function() {
            jq.unblockUI();
            return false;
        });

        jq('#questionWindowTaskRemove .remove').on('click', function() {
            var taskId = jq('#questionWindowTaskRemove').attr('taskId');
            removeTask({ 'taskId': taskId }, taskId);
            return false;
        });


        // ga-track-events

        //show next
        jq("#showNextTasks").trackEvent(ga_Categories.tasks, ga_Actions.next, 'next-tasks');
        
        //change status
        jq("#statusListContainer .open").trackEvent(ga_Categories.tasks, ga_Actions.changeStatus, "open");
        jq("#statusListContainer .closed").trackEvent(ga_Categories.tasks, ga_Actions.changeStatus, "closed");

        //responsible
        jq(".user span").trackEvent(ga_Categories.tasks, ga_Actions.userClick, "tasks-responsible");

        //actions in menu
        jq("#ta_accept").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "accept");
        jq("#ta_mesres").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "notify-responsible");
        jq("#ta_move").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "move-in-milestone");
        jq("#ta_time").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "time-track");
        jq("#ta_subtask").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "add-subtask");
        jq("#ta_remove").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "remove");
        jq("#ta_edit").trackEvent(ga_Categories.tasks, ga_Actions.actionClick, "edit");

        //actions
        jq(".subtasksCount .add").trackEvent(ga_Categories.tasks, ga_Actions.quickAction, "add-subtask");
        //end ga-track-events


    };

    var showNewTaskPopup = function() {
        jq('.studio-action-panel').hide();
        ASC.Projects.TaskAction.showCreateNewTaskForm();
    };

    var resize = function() {

        var mainWidth = parseInt(jq(".mainPageLayout").css("min-width")),
            taskNameWidth = mainWidth - jq(".mainPageTableSidePanel").width()- 24 * 4
            - jq(".taskList .check").outerWidth(true)
            - jq(".taskList .deadline").outerWidth(true)
            - jq(".taskList .user").outerWidth(true)
            - jq(".taskList .entity-menu").outerWidth(true)
            - jq(".taskList .subtasksCount").outerWidth(true),            
            dif = 0;
        if (jq(window).width() > mainWidth + 24 * 2) {
            dif = jq(window).width() - mainWidth - 24 * 2;
        }

        jq("#SubTasksBody .taskList .taskPlace .taskName").each(
        function () {           
            jq(this).css("max-width", taskNameWidth + dif + "px");
        }
    );

        
    };

    var showOrHideListSubtasks = function(taskId, addFlag) {
        ASC.Projects.SubtasksManager.hideSubtaskActionPanel();
        var taskListContainer = jq("#SubTasksBody .taskList");
        var subtasks = jq('.subtasks[taskid=' + taskId + ']');
        var subtasksCount = subtasks.find(".subtask").length;
        if (jq(subtasks).is(":visible")) {
            taskListContainer.find('.task[taskid=' + taskId + ']').removeClass('borderbott');
            subtasks.hide();
            return true;
        };

        if (!subtasksCount && !addFlag) return false;

        taskListContainer.find('.task[taskid=' + taskId + ']').addClass('borderbott');
        if (subtasksCount) {
            separateSubtasks(taskId);
        }
        subtasks.show();
        ASC.Projects.TasksManager.resize();
        return true;
    };

    var getTasks = function(filter) {

        filter.Count = entryCountOnPage;
        filter.StartIndex = entryCountOnPage * currentPage;

        if (filter.StartIndex > filterTaskCount) {
            filter.StartIndex = 0;
            currentPage = 1;
        }
        Teamlab.getPrjTasks({}, { filter: filter, success: onGetTasks });
    };

    var getFilteredTaskById = function (taskId) {
        for (var i = 0, max = filteredTasks.length; i < max; i++){
            if (filteredTasks[i].id == taskId) {
                return filteredTasks[i];
            }
        }
    };

    var onGetTasks = function(params, tasks) {
        filteredTasks = tasks;
        jq('#SubTasksBody').height('auto');
        jq('#SubTasksBody .taskSaving').hide();
        clearTimeout(taskDescriptionTimeout);
        overTaskDescriptionPanel = false;
        hideDescriptionPanel();

        jq('#SubTasksBody .taskList').html('');

        jq('#showNextTaskProcess').hide();

        jq('#SubTasksBody .taskList').height('auto');
        if (tasks.length) {
            jq.tmpl("projects_taskListItemTmpl", tasks).appendTo('.taskList');
        }

        if (!currentProjectId) {
            jq('#SubTasksBody .choose.project span').html(jq('#SubTasksBody .choose.project').attr('choose'));
            jq('#SubTasksBody .choose.project').attr('value', '');
        }

        LoadingBanner.hideLoading();

        filterTaskCount = params.__total != undefined ? params.__total : 0;
        updatePageNavigatorControl();
        emptyScreenList(tasks.length);
        ASC.Projects.TasksManager.resize();
    };

    var onAddTask = function(params, task) {
        allTaskCount++;
        filterTaskCount++;
        jq('.taskList .noContentBlock').remove();
        jq.tmpl("projects_taskListItemTmpl", task).prependTo(".taskList");
        jq('#SubTasksBody .taskSaving').hide();
        jq('.taskList .task:first').yellowFade();
        resize();
        updatePageNavigatorControl();
        emptyScreenList(true);
    };

    var onUpdateTask = function(params, task) {
        var taskId = task.id;
        jq('.taskList .task[taskid=' + taskId + ']:first').remove();
        jq.tmpl("projects_taskListItemTmpl", task).insertBefore('.taskList .subtasks[taskid=' + taskId + ']');
        jq('.taskList .subtasks[taskid=' + taskId + ']:first').remove();
        jq('.taskList .task[taskid=' + taskId + ']').yellowFade();
        if (task.subtasks.length && jq('.taskList .subtasks:visible[taskid=' + taskId + ']').length) {
            jq('.taskList .task[taskid=' + taskId + ']').addClass('borderbott');
        }
        resize();
        jq.unblockUI();
    };

    var closeTask = function(taskId) {
        jq('.taskProcess').remove();
        jq('.taskList .task[taskid=' + taskId + '] .check').html('');
        jq('.taskList .task[taskid=' + taskId + '] .check').append('<div class="taskProcess"></div>');

        updateTaskStatus({}, taskId, 2);
    };

    var updateTaskStatus = function(params, taskId, status) {
        Teamlab.updatePrjTask(params, taskId, { "status": status }, { success: onUpdateTaskStatus });
    };

    var onUpdateTaskStatus = function(params, task) {
        var status = task.status;
        var taskId = task.id;

        if (status == 1) {
            jq('.taskList .subtasks[taskid=' + taskId + ']:first').remove();
            jq('.taskList .task[taskid=' + taskId + ']:first').replaceWith(jq.tmpl("projects_taskListItemTmpl", task));
            setTimeout(function() { jq('.taskList .task[taskid=' + taskId + ']').yellowFade(); }, 0);
        } else {
            jq('.taskList .subtasks[taskid=' + taskId + ']:first').remove();
            jq('.taskList .task[taskid=' + taskId + ']:first').replaceWith(jq.tmpl("projects_taskListItemTmpl", task));
            jq('.taskList .subtasks[taskid=' + taskId + ']').hide();
            setTimeout(function() { jq('.taskList .task.closed[taskid=' + taskId + ']').yellowFade(); }, 0);
        }
        ASC.Projects.TasksManager.resize();
    };

    var notifyTaskResponsible = function(params, taskId) {
        Teamlab.notifyPrjTaskResponsible(params, taskId, { success: showRemindTaskPopup });

    };

    var removeTask = function(params, taskId) {
        Teamlab.removePrjTask(params, taskId, { success: onRemoveTask, error: onErrorRemoveTask });
    };

    var onRemoveTask = function(params, task) {
        var taskId = task.id;
        jq('.taskList .task[taskid=' + taskId + ']').remove();
        jq('.taskList .subtasks[taskid=' + taskId + ']').remove();

        if (currentProjectId == task.projectId) {
            ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.tasks, "delete");
        }

        allTaskCount--;
        filterTaskCount--;
        updatePageNavigatorControl();
        if (typeof task != 'undefined') {
            emptyScreenList(task.length);
        } else {
            emptyScreenList(0);
        }
        jq('.taskList .task[taskid=' + taskId + ']').html('<div class="taskProcess"></div>');
        jq('#questionWindowTaskRemove').removeAttr('taskId');
        jq.unblockUI();
    };

    var onErrorRemoveTask = function(param, error) {
        var removePopupErrorBox = jq("#questionWindowTaskRemove .errorBox");
        removePopupErrorBox.text(error[0]);
        removePopupErrorBox.removeClass("display-none");
        jq("#questionWindowTaskRemove .middle-button-container").css('marginTop', '8px');
        setTimeout(function() {
            removePopupErrorBox.addClass("display-none");
            jq("#questionWindowTaskRemove .middle-button-container").css('marginTop', '32px');
        }, 3000);
    };

    var onAddSubtask = function (params, subtask) {
        changeCountTaskSubtasks(subtask.taskid, 'add');
    };

    var onRemoveSubtask = function (params, subtask) {
        var taskId = params.taskId;
        var subtasksCont = jq('.taskList .subtasks[taskid=' + taskId + ']');
        if (subtask.status == 1) {
            changeCountTaskSubtasks(taskId, 'delete');
        }
        if (!subtasksCont.find('.subtask').length) {
            showOrHideListSubtasks(taskId);
        }
    };

    var onUpdateSubtaskStatus = function (params, subtask) {
        if (subtask.status == 2) {
            changeCountTaskSubtasks(params.taskId, 'delete');
        } else {
            changeCountTaskSubtasks(params.taskId, 'add');
        }
    };

    var changeCountTaskSubtasks = function(taskid, action) {
        var currentCount;
        var text;
        var task = jq(".task[taskid='" + taskid + "']");
        var subtasksCounterContainer = jq(task).find('.subtasksCount');
        var subtasksCounter = jq(subtasksCounterContainer).find('.dottedNumSubtask');

        if (subtasksCounter.length) {
            text = jq.trim(jq(subtasksCounter).text());
            text = text.substr(1, text.length - 1);
        } else {
            text = "";
        }

        if (text == "") {
            currentCount = 0;
        } else {
            currentCount = parseInt(text);
        }

        if (action == "add") {
            currentCount++;
            if (currentCount == 1) {
                jq(subtasksCounterContainer).find('.add').remove();
                subtasksCounter = '<span class="collaps" taskid="' + taskid + '"><span class="dottedNumSubtask">+' + currentCount + '</span></span>';
                jq(subtasksCounterContainer).append(subtasksCounter);
            } else {
                jq(subtasksCounter).text('+' + currentCount);
            }
        }
        else if (action == "delete") {
            currentCount--;
            if (currentCount != 0) {
                jq(subtasksCounter).text('+' + currentCount);
            }
            else {
                jq(subtasksCounter).remove();
                var hoverText = jq(subtasksCounterContainer).attr('data');
                jq(subtasksCounterContainer).append('<span class="add" taskid="' + taskid + '">+ ' + hoverText + '</span>');
                jq(task).find('.subtasks').hide();
            }
        }
    };

    var updateMilestonesListForMovePanel = function(milestones) {
        jq('#moveTaskPanel .milestonesList .ms').remove();
        jq.tmpl("projects_milestoneForMoveTaskPanelTmpl", milestones).prependTo("#moveTaskPanel .milestonesList");
    };

    var getMilestonesForMovePanel = function(params, projectId) {
        Teamlab.getPrjMilestones(params, null, { filter: { status: 'open', projectId: projectId }, success: onGetMilestonesForMovePanel });
    };

    var onGetMilestonesForMovePanel = function(params, milestones) {
        updateMilestonesListForMovePanel(milestones);
        showMoveToMilestonePanel();
    };

    var compareDates = function(data) {
        var currentDate = new Date();
        if (currentDate > data) {
            return true;
        }
        else return false;
    };

    var emptyScreenList = function(isItems) {
        var emptyScreen = (allTaskCount == 0) ? '#emptyTaskListScreen' : '#emptyScreenForFilter';

        if (isItems === undefined) {
            var tasks = jq('.taskList .task');
            if (tasks.length != 0) {
                isItems = true;
            }
        }

        if (isItems) {
            jq('.noContentBlock').hide();
            jq('#ProjectsAdvansedFilter').show();
            jq('#tableForNavigation').show();
        } else {
            if (filterTaskCount == undefined || filterTaskCount == 0) {
                jq(emptyScreen).show();
                jq('#tableForNavigation').hide();
                if (emptyScreen == '#emptyTaskListScreen') {
                    jq('#ProjectsAdvansedFilter').hide();
                }
            }
            else {
                if (allTaskCount != 0) {
                    currentPage--;
                    getTasks(currentFilter, false);
                }
            }
        }
    };

    var separateSubtasks = function(taskid) {
        var subtasksCont = jq('.subtasks[taskid="' + taskid + '"]');
        var closedSubtasks = jq(subtasksCont).find('.subtask.closed');
        jq(jq(subtasksCont).find('.st_separater')).after(closedSubtasks);
        subtasksCont.show();
    };

    // show popup methods

    var popupWindow = function(taskId) {
        jq('#questionWindow .end').attr('taskid', taskId);

        StudioBlockUIManager.blockUI(jq('#questionWindow'), 480, 200, 0);
        PopupKeyUpActionProvider.EnterAction = PopupKeyUpActionProvider.EnterAction = "jq('#questionWindow .end').click();";
    };

    var showRemindTaskPopup = function() {
        StudioBlockUIManager.blockUI(jq('#remindAboutTask'), 300, 100, 0);
        PopupKeyUpActionProvider.EnterAction = PopupKeyUpActionProvider.EnterAction = "jq('#remindAboutTask .ok').click();";
    };

    var showQuestionWindowTaskRemove = function(taskId) {

        StudioBlockUIManager.blockUI(jq('#questionWindowTaskRemove'), 400, 200, 0);
        PopupKeyUpActionProvider.EnterAction = "jq('#questionWindowTaskRemove .remove').click();";

        jq('#questionWindowTaskRemove').attr('taskId', taskId);
    };

    var showMoveToMilestonePanel = function() {
        var taskId = jq('#taskActionPanel').attr('objid');
        jq('#moveTaskPanel').attr('taskid', taskId);

        var taskId = jq('#moveTaskPanel').attr('taskid');
        jq('#moveTaskTitles').text(jq(".taskList .task[taskid=" + taskId + "] .taskName a").text());
        var milestoneid = jq('.taskList .task[taskid=' + taskId + ']').attr('milestoneid');

        if (typeof milestoneid != 'undefined') {
            jq('#moveTaskPanel input#ms_' + milestoneid).prop('checked', true);
        } else {
            jq('#moveTaskPanel input#ms_0').prop('checked', true);
        }

        StudioBlockUIManager.blockUI(jq('#moveTaskPanel'), 550, 300, 0);
        PopupKeyUpActionProvider.EnterAction = "jq('#moveTaskPanel .blue').click();";
    };

    var showActionsPanel = function(panelId, obj) {
        var objid = '',
            objidAttr = '';
        var x, y;
        if (typeof jq(obj).attr('projectid') != 'undefined') {
            jq('#taskActionPanel #ta_move').attr('projectid', jq(obj).attr('projectid'));
            jq('#taskActionPanel #ta_time').attr('projectid', jq(obj).attr('projectid'));
        }
        if (typeof jq(obj).attr('userid') != 'undefined') {
            jq('#taskActionPanel #ta_time').attr('userid', jq(obj).attr('userid'));
        }
        if (panelId == 'taskActionPanel') {
            objid = jq(obj).attr('taskid');
        }
        if (objid.length) {
            objidAttr = '[objid=' + objid + ']';
        }
        if (jq('#' + panelId + ':visible' + objidAttr).length && panelId != 'taskDescrPanel' && panelId != 'subTaskDescrPanel') {
            jq('body').off('click');
            jq('.studio-action-panel, .filter-list').hide();
            jq('#statusListContainer').hide();
            jq('.changeStatusCombobox').removeClass('selected');

        } else {
            jq('.studio-action-panel, .filter-list').hide();
            jq('#statusListContainer').hide();
            jq('.changeStatusCombobox').removeClass('selected');

            jq('#' + panelId).show();
            if (panelId == 'taskDescrPanel') {
                x = jq(obj).offset().left + 10;
                y = jq(obj).offset().top + 20;
                jq('#' + panelId).attr('objid', jq(obj).attr('taskid'));
            } else if (panelId == 'othersPanel') {
                x = jq(obj).offset().left - 133;
                y = jq(obj).offset().top + 26;
            } else {
                x = jq(obj).offset().left - 131;
                y = jq(obj).offset().top + 16;
                jq('#' + panelId).attr('objid', objid);
                jq('#taskActionPanel .dropdown-item').show();

                var task = jq('.task[taskid=' + objid + ']');

                var taskUser = jq(task).find(".user");
                if (task.length) { //if it`s tasks menu        
                    if (jq(task).hasClass('closed')) {
                        jq('#taskActionPanel .dropdown-item').hide();
                        jq('#taskActionPanel #ta_time').show();
                        jq('#taskActionPanel #ta_remove').show();
                    } else if (taskUser.length == 1) {
                        if (jq(taskUser).hasClass("not") || jq(taskUser).attr('data-userId') == currentUserId) {
                            jq('#taskActionPanel #ta_mesres').hide();
                        }
                        if (!jq(taskUser).hasClass("not")) {
                            jq('#taskActionPanel #ta_accept').hide();
                        }
                    } else {
                        jq('#taskActionPanel #ta_mesres').show();
                    }
                }

                if (jq('.task[taskid=' + objid + ']').length) {
                    if (jq(obj).attr('canDelete') != "true") {
                        jq('#taskActionPanel #ta_remove').hide();

                        if (jq(obj).attr('canEdit') == "false" || Teamlab.profile.isVisitor) {
                            jq('#taskActionPanel #ta_edit').hide();
                            jq('#taskActionPanel #ta_move').hide();
                            jq('#taskActionPanel #ta_mesres').hide();
                            if (jq(obj).attr("data-cancreatesubtask") == "false")
                                jq("#taskActionPanel #ta_subtask").hide();
                        }
                    }
                }
            }

            if (typeof y == 'undefined')
                y = jq(obj).offset().top + 29;
            jq('#' + panelId).css({ left: x, top: y });

            jq('body').click(function(event) {
                var elt = (event.target) ? event.target : event.srcElement;
                var isHide = true;
                if (jq(elt).is('[id="' + panelId + '"]') || (elt.id == obj.id && obj.id.length) || jq(elt).is('.entity-menu') || jq(elt).is('.other')) {
                    isHide = false;
                }

                if (isHide)
                    jq(elt).parents().each(function() {
                        if (jq(this).is('[id="' + panelId + '"]')) {
                            isHide = false;
                            return false;
                        }
                    });

                if (isHide) {
                    hideTaskActionPanel();
                }
            });
        }
    };

    var hideTaskActionPanel = function () {
        jq('.studio-action-panel').hide();
        jq('.taskList .task').removeClass('menuopen');
    };

    var openedCount = function(items) {
        var c = 0;
        for (var i = 0; i < items.length; i++) {
            if (items[i].status != 2) c++;
        }
        return c;
    };

    var hideDescriptionPanel = function() {
        setTimeout(function() {
            if (!overTaskDescriptionPanel) jq('#taskDescrPanel').hide(100);
        }, 200);
    };

    var showStatusListContainer = function(status) {
        selectedStatusCombobox.addClass('selected');
        jq('.studio-action-panel, .filter-list').hide();
        jq('.task.menuopen').removeClass('menuopen');
        var top = selectedStatusCombobox.offset().top + 25;
        var left = selectedStatusCombobox.offset().left + 9;
        statusListContainer.css({ left: left, top: top });

        if (status == 'overdue' || status == 'active') {
            status = 'open';
        }
        var currentStatus = statusListContainer.find('li.' + status);
        currentStatus.addClass('selected');
        currentStatus.siblings().removeClass('selected');

        statusListContainer.show();
    };

    var hideStatusListContainer = function() {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        }
        statusListContainer.hide();
    };

    return {
        init: init,
        getCurrentUserId: getCurrentUserId,
        getCurrentProjectId: getCurrentProjectId,
        openedCount: openedCount,
        showActionsPanel: showActionsPanel,
        compareDates: compareDates,
        onAddTask: onAddTask,
        onUpdateTask: onUpdateTask,
        onRemoveTask: onRemoveTask,
        resize: resize,
        changeCountOfRows: changeCountOfRows
    };
})(jQuery);
