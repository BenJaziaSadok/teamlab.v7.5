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

ASC.Projects.AllMilestones = (function() {
    var isInit = false;

    var currentUserId;
    var isAdmin;

    var statusListContainer;
    var milestoneActionContainer;

    var currentProjectId;
    var currentFilter = {};

    var loadListProjectsFlag = false;
    var loadListTagsFlag = false;
    var loadTeamFlag = false;

    var filterMilestoneCount = 0;

    var selectedStatusCombobox;
    var selectedActionCombobox;

    var advansedFilter;

    var descriptionTimeout;
    var overDescriptionPanel = false;

    var basePath = 'sortBy=deadline&sortOrder=ascending';

    //pagination
    var entryCountOnPage;
    var pageCount;
    var cookiePaginationKey;
    var currentPage = 0;
    var allMilestonesCount;

    var getMilestoneActiveTasksLink = function(prjId, milestoneId) {
        return 'tasks.aspx?prjID=' + prjId + '#' + basePath + '&milestone=' + milestoneId + '&status=open';
    };

    var getMilestoneClosedTasksLink = function(prjId, milestoneId) {
        return 'tasks.aspx?prjID=' + prjId + '#' + basePath + '&milestone=' + milestoneId + '&status=closed';
    };

    var getCurrentProjectId = function() {
        return currentProjectId;
    };

    var setCurrentFilter = function(filter) {
        currentFilter = filter;
    };

    var showAdvansedFilter = function() {
        jq("#ProjectsAdvansedFilter").show();
    };

    var hideAdvansedFilter = function() {
        jq("#ProjectsAdvansedFilter").hide();
    };

    var showNewMilestoneButton = function() {
        jq(".addNewButton").removeClass("display-none");
    };

    var hideNewMilestoneButton = function() {
        jq(".addNewButton").addClass("display-none");
    };

    //filter Set
    var onSetFilterMilestones = function(evt, $container) {
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
        setCurrentFilter(filter);
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
        if (!allMilestonesCount) {
            jq('#milestonesList tbody').empty();
            jq('#descriptionPanel').hide();
            jq('#tableForNavigation').hide();
            showOrHideEmptyScreen(0);
        }
        else {
            getMilestones(filter, false);
            showPreloader();
        }
    };

    //filter Reset
    var onResetFilterMilestones = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        setCurrentFilter(filter);
        getMilestones(filter, false);
        showPreloader();
    };

    var createAdvansedFilter = function() {
        var now = new Date();
        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        var inWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        inWeek.setDate(inWeek.getDate() + 7);

        var filters = [];

        // Responsible

        if (currentProjectId) {
            if (ASC.Projects.Common.userInProjectTeam(currentUserId, ASC.Projects.Master.Team)) {
                filters.push({
                    type: "combobox",
                    id: "me_responsible_for_milestone",
                    title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                    filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                    group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                    options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                    hashmask: "person/{0}",
                    groupby: "userid",
                    bydefault: { value: currentUserId }
                });
                //Tasks
                filters.push({
                    type: "combobox",
                    id: "me_tasks",
                    title: ASC.Projects.Resources.ProjectsFilterResource.MyTasks,
                    filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Tasks + ":",
                    group: ASC.Projects.Resources.ProjectsFilterResource.Tasks,
                    hashmask: "person/{0}",
                    groupby: "taskuserid",
                    options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                    bydefault: { value: currentUserId }
                });
            }
            filters.push({
                type: "combobox",
                id: "responsible_for_milestone",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherParticipant,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                hashmask: "person/{0}",
                groupby: "userid",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
            filters.push({
                type: "combobox",
                id: "user_tasks",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherParticipant,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                hashmask: "person/{0}",
                groupby: "userid",
                group: ASC.Projects.Resources.ProjectsFilterResource.Tasks,
                options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
        } else {
            filters.push({
                type: "person",
                id: "me_responsible_for_milestone",
                title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid",
                bydefault: { id: currentUserId }
            });
            filters.push({
                type: "person",
                id: "responsible_for_milestone",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid"
            });
            //Tasks
            filters.push({
                type: "person",
                id: "me_tasks",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyTasks,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Tasks + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.Tasks,
                hashmask: "person/{0}",
                groupby: "taskuserid",
                bydefault: { id: currentUserId }
            });
            filters.push({
                type: "person",
                id: "user_tasks",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Tasks + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.Tasks,
                hashmask: "person/{0}",
                groupby: "taskuserid"
            });
        }

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
        // Status
        filters.push({
            type: "combobox",
            id: "open",
            title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenMilestone,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
            hashmask: "combobox/{0}",
            groupby: "status",
            options:
                [
                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenMilestone, def: true },
                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedMilestone }
                ]
        });
        filters.push({
            type: "combobox",
            id: "closed",
            title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedMilestone,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
            hashmask: "combobox/{0}",
            groupby: "status",
            options:
                [
                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenMilestone },
                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedMilestone, def: true }
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

        var colCount = 3;
        if (currentProjectId) colCount = 2;

        ASC.Projects.ProjectsAdvansedFilter.filter = jq('#ProjectsAdvansedFilter').advansedFilter(
        {
            store: true,
            anykey: true,
            colcount: colCount,
            anykeytimeout: 1000,
            filters: filters,
            sorters:
            [
                { id: "deadline", title: ASC.Projects.Resources.ProjectsFilterResource.ByDeadline, sortOrder: "ascending", def: true },
                { id: "create_on", title: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate, sortOrder: "descending" },
                { id: "title", title: ASC.Projects.Resources.ProjectsFilterResource.ByTitle, sortOrder: "ascending" }
            ]
        }
      )
      .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
      .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

        ASC.Projects.ProjectsAdvansedFilter.init = true;

        // ga-track-events

        //filter
        ASC.Projects.ProjectsAdvansedFilter.filter.one("adv-ready", function () {
            var projectAdvansedFilterContainer = jq("#ProjectsAdvansedFilter .advansed-filter-list");
            projectAdvansedFilterContainer.find("li[data-id='me_responsible_for_milestone'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'me_responsible_for_milestone');
            projectAdvansedFilterContainer.find("li[data-id='responsible_for_milestone'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'responsible_for_milestone');
            projectAdvansedFilterContainer.find("li[data-id='me_tasks'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'me_tasks');
            projectAdvansedFilterContainer.find("li[data-id='user_tasks'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'user_tasks');
            projectAdvansedFilterContainer.find("li[data-id='open'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'open');
            projectAdvansedFilterContainer.find("li[data-id='closed'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'closed');
            projectAdvansedFilterContainer.find("li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'myprojects');
            projectAdvansedFilterContainer.find("li[data-id='project'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'project');
            projectAdvansedFilterContainer.find("li[data-id='tag'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'tag');

            projectAdvansedFilterContainer.find("li[data-id='overdue'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'overdue');
            projectAdvansedFilterContainer.find("li[data-id='today'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'today');
            projectAdvansedFilterContainer.find("li[data-id='upcoming'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'upcoming');
            projectAdvansedFilterContainer.find("li[data-id='deadline'] .inner-text").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'user-period');

            jq("#ProjectsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, 'sort');
            jq("#ProjectsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.milestones, ga_Actions.filterClick, "search_text", "enter");
        });
    };

    var initPageNavigatorControl = function () {
        window.mileListPgNavigator = new ASC.Controls.PageNavigator.init("mileListPgNavigator", "#divForTaskPager", entryCountOnPage, pageCount, 1,
                                                               ASC.Projects.Resources.ProjectsJSResource.PreviousPage, ASC.Projects.Resources.ProjectsJSResource.NextPage);
        mileListPgNavigator.NavigatorParent = '#divForTaskPager';
        mileListPgNavigator.changePageCallback = function(page) {
            currentPage = page - 1;
            showPreloader();
            getMilestones(currentFilter, true);
        };
    };

    var updatePageNavigatorControl = function() {
        jq("#totalCount").text(filterMilestoneCount);
        mileListPgNavigator.drawPageNavigator(currentPage + 1, filterMilestoneCount);

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
        mileListPgNavigator.EntryCountOnPage = newCountOfRows;

        showPreloader();
        getMilestones(currentFilter, false);
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

        var isAdministrator = jq("#milestonesListContainer").attr("data-is-admin");
        isAdmin = isAdministrator === "True";
        currentUserId = Teamlab.profile.id;

        var milestonesCount = jq("#milestonesListContainer").attr("data-milestones-count");
        if (!milestonesCount) milestonesCount = 0;
        allMilestonesCount = parseInt(milestonesCount);

        if (isInit === false) {
            isInit = true;
        }

        currentProjectId = jq.getURLParam('prjID');

        //page navigator
        entryCountOnPage = countOfPage;
        pageCount = entryPageCount;
        cookiePaginationKey = cookieKey;
        jq("#countOfRows").val(entryCountOnPage).tlCombobox();
        initPageNavigatorControl();

        //filter
        ASC.Projects.ProjectsAdvansedFilter.initialisation(currentUserId, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterMilestones;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterMilestones;

        statusListContainer = jq('#statusListContainer');
        milestoneActionContainer = jq('#milestoneActionContainer');

        advansedFilter = jq('#ProjectsAdvansedFilter');

        // waiting data from api
        jq(document).bind("loadApiData", function() {
            if (loadListProjectsFlag && loadListTagsFlag && loadTeamFlag) {
                createAdvansedFilter();
            }
        })

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadProjects, function() {
            loadListProjectsFlag = true;
            jq(document).trigger("loadApiData");
        });

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTags, function() {
            loadListTagsFlag = true;
            jq(document).trigger("loadApiData");
        });

        if (currentProjectId) {
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function() {
                loadTeamFlag = true;
                jq(document).trigger("loadApiData");
            });
        } else {
            loadTeamFlag = true;
            jq(document).trigger("loadApiData");
        }

        //

        // Events

        jq("#countOfRows").change(function(evt) {
            changeCountOfRows(this.value);
        });

        jq('#emptyListMilestone').on('click', '.addFirstElement', function() {
            ASC.Projects.MilestoneAction.showNewMilestonePopup();
        });

        jq("#emptyScreenForFilter").on("click", '.clearFilterButton', function() {
            jq('#ProjectsAdvansedFilter').advansedFilter(null);
            return false;
        });

        // popup
        jq('#questionWindowDeleteMilestone .remove').on('click', function() {
            var milestoneId = jq("#questionWindowDeleteMilestone").attr("milestoneId");
            jq('#milestonesList tr#' + milestoneId).html('<td class="process" colspan="8"></td>');
            deleteMilestone(milestoneId);
            jq("#questionWindowDeleteMilestone").removeAttr("milestoneId");
            jq.unblockUI();
            return false;
        });
        jq('#questionWindowDeleteMilestone .cancel, #questionWindowTasks .button.gray.middle').on('click', function () {
            jq.unblockUI();
            return false;
        });

        jq('body').on('click', function(event) {
            var target = (event.target) ? event.target : event.srcElement;
            var element = jq(target);
            if (!element.is('.entity-menu')) {
                hideMilestoneActionContainer();
            }
            if (!(element.is('span.overdue') || element.is('span.active') || element.is('span.closed'))) {
                hideStatusListContainer();
            }
        });

        jq('#milestonesList').on('click', 'td.status .changeStatusCombobox.canEdit', function(event) {
            hideMilestoneActionContainer();
            var element = (event.target) ? event.target : event.srcElement;
            var status = jq(element).attr('class');
            var currentMilestone = selectedStatusCombobox !== undefined ? selectedStatusCombobox.attr('milestoneId') : -1;
            jq('#milestonesList tr#' + currentMilestone + ' td.status .changeStatusCombobox').removeClass('selected');
            selectedStatusCombobox = jq(this);

            if (statusListContainer.attr('milestoneId') !== selectedStatusCombobox.attr('milestoneId')) {
                statusListContainer.attr('milestoneId', selectedStatusCombobox.attr('milestoneId'));
                showStatusListContainer(status);
            } else {
                toggleStatusListContainer(status);
            }
            return false;
        });

        jq('#statusListContainer li').on('click', function() {
            if (jq(this).is('.selected')) return;
            var milestoneId = jq('#statusListContainer').attr('milestoneId');
            var status = jq(this).attr('class').split(" ")[0];
            if (status == 'closed') {
                var text = jq.trim(jq('#' + milestoneId + ' td.activeTasksCount').text());
                if (text != '' && text != '0') {
                    showQuestionWindow(milestoneId);
                    return;
                }
            }

            Teamlab.updatePrjMilestone({milestoneId: milestoneId}, milestoneId, { status: status }, { success: onUpdateMilestone, error: onChangeStatusError });
        });

        jq('#milestonesList').on('mouseenter', 'tr .title a', function(event) {
            descriptionTimeout = setTimeout(function() {
                var targetObject = event.target;
                jq('#descriptionPanel .value div, #descriptionPanel .param div').hide();
                if (typeof jq(targetObject).attr('projectTitle') != 'undefined') {
                    jq('#descriptionPanel .value .project a').html(jq.htmlEncodeLight(jq(targetObject).attr('projectTitle')));
                    jq('#descriptionPanel .value .project').attr('projectId', jq(targetObject).attr('projectId'));
                    jq('#descriptionPanel .project').show();
                }
                if (typeof jq(targetObject).attr('description') != 'undefined') {
                    var description = jq(targetObject).attr('description');
                    if (description != '') {
                        jq('#descriptionPanel .value .description').html(jq.htmlEncodeLight(description));
                        jq('#descriptionPanel .description').show();
                    }
                }
                if (typeof jq(targetObject).attr('created') != 'undefined') {
                    var date = jq(targetObject).attr('created');
                    jq('#descriptionPanel .value .created').html(date);
                    jq('#descriptionPanel .created').show();
                }
                if (typeof jq(targetObject).attr('createdBy') != 'undefined') {
                    jq('#descriptionPanel .value .createdby').html(jq.htmlEncodeLight(jq(targetObject).attr('createdBy')));
                    jq('#descriptionPanel .value .createdby').attr('createdById', jq(targetObject).attr('createdById'));
                    jq('#descriptionPanel .createdby').show();
                }
                var listParams = jq('#descriptionPanel .param div');
                var listValues = jq('#descriptionPanel .value div');
                showDescriptionPanel(targetObject);
                overDescriptionPanel = true;
                for (var i = 0; i < listValues.length; i++) {
                    var height = jq(listValues[i]).height();
                    jq(listParams[i]).height(height);
                }
            }, 400, this);

        });

        jq('#milestonesList').on('mouseleave', 'tr .title a', function() {
            clearTimeout(descriptionTimeout);
            overDescriptionPanel = false;
            hideDescriptionPanel();
        });

        jq('#removeMilestoneButton').on('click', function() {
            var milestoneId = milestoneActionContainer.attr('milestoneId');
            milestoneActionContainer.hide();
            showQuestionWindowMilestoneRemove(milestoneId);
        });

        jq('#addMilestoneTaskButton').on('click', function() {
            milestoneActionContainer.hide();
            
            var taskParams = {};
            taskParams.milestoneId = parseInt(milestoneActionContainer.attr('milestoneId'));
            taskParams.projectId = parseInt(milestoneActionContainer.attr('projectId'));

            ASC.Projects.TaskAction.showCreateNewTaskForm(taskParams);
        });

        // ga-track-events
            //change status
            jq("#statusListContainer .open").trackEvent(ga_Categories.milestones, ga_Actions.changeStatus, "open");
            jq("#statusListContainer .closed").trackEvent(ga_Categories.milestones, ga_Actions.changeStatus, "closed");

            //responsible
            jq(".responsible span").trackEvent(ga_Categories.milestones, ga_Actions.userClick, "milestone-responsible");

            //actions
            jq("#addMilestoneTaskButton").trackEvent(ga_Categories.milestones, ga_Actions.actionClick, "add-task-in-milestone");

        //end ga-track-events
    };

    var getMilestones = function(filter) {

        filter.Count = entryCountOnPage;
        filter.StartIndex = entryCountOnPage * currentPage;

        if (filter.StartIndex > filterMilestoneCount) {
            filter.StartIndex = 0;
            currentPage = 1;
        }

        Teamlab.getPrjMilestones({}, { filter: filter, success: onGetMilestones });
    };

    var showOrHideEmptyScreen = function(milestonesCount) {
        if (milestonesCount) {
            showAdvansedFilter();
            showNewMilestoneButton();
            jq('#milestonesListContainer .noContentBlock').hide();
        } else {
            jq('#descriptionPanel').hide();
            jq("#tableForNavigation").hide();
            if (allMilestonesCount == 0) {
                jq('#emptyScreenForFilter').hide();
                jq('#emptyListMilestone').show();
                hideNewMilestoneButton();
                hideAdvansedFilter();
            } else {
                jq('#emptyListMilestone').hide();
                showAdvansedFilter();
                jq('#emptyScreenForFilter').show();
            }
        }
    };

    var onGetMilestones = function(params, milestones) {
        var tmplMile, listTmplMiles = new Array();
        var milestonesCount = milestones.length;

        clearTimeout(descriptionTimeout);

        jq('#milestonesList tbody').empty();
        filterMilestoneCount = params.__total != undefined ? params.__total : 0;
        updatePageNavigatorControl();

        hidePreloader();
        showOrHideEmptyScreen(milestonesCount);

        if (milestonesCount) {
            for (var i = 0; i < milestones.length; i++) {
                tmplMile = getMilestoneTemplate(milestones[i]);
                listTmplMiles.push(tmplMile);
            }
            jq.tmpl("projects_milestoneTemplate", listTmplMiles).appendTo('#milestonesList tbody');
        }
    };

    var getMilestoneTemplate = function(milestone) {
        var id = milestone.id;
        var prjId = milestone.projectId;
        var template = {
            id: id,
            isKey: milestone.isKey,
            isNotify: milestone.isNotify,
            title: milestone.title,
            activeTasksCount: milestone.activeTaskCount,
            activeTasksLink: getMilestoneActiveTasksLink(prjId, id),
            closedTasksCount: milestone.closedTaskCount,
            closedTasksLink: getMilestoneClosedTasksLink(prjId, id),
            canEdit: milestone.canEdit,
            projectId: prjId,
            projectTitle: milestone.projectTitle,
            createdById: milestone.createdBy.id,
            createdBy: milestone.createdBy.displayName,
            description: milestone.description,
            created: milestone.displayDateCrtdate
        };

        if (milestone.responsible) {
            template.responsible = milestone.responsible.displayName;
            template.responsibleId = milestone.responsible.id;
        } else {
            template.responsible = null;
            template.responsibleId = null;
        }

        var today = new Date();
        var status = milestone.status == 0
            ? today < milestone.deadline
                ? 'active'
                : 'overdue'
            : 'closed';

        template.status = status;
        template.deadline = milestone.displayDateDeadline;

        return template;
    };

    var showPreloader = function() {
        LoadingBanner.displayLoading();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };

    var showStatusListContainer = function(status) {
        selectedStatusCombobox.addClass('selected');

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

    var toggleStatusListContainer = function(status) {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        } else {
            selectedStatusCombobox.addClass('selected');
        }

        if (status == 'overdue' || status == 'active') {
            status = 'open';
        }
        var currentStatus = statusListContainer.find('li.' + status);
        currentStatus.addClass('selected');
        currentStatus.siblings().removeClass('selected');

        statusListContainer.toggle();
    };

    var hideStatusListContainer = function() {
        if (statusListContainer.is(':visible')) {
            selectedStatusCombobox.removeClass('selected');
        }
        statusListContainer.hide();
        jq("#projectActions").hide();
        jq(".project-title .menu-small").removeClass("active");
    };

    var showQuestionWindow = function(milestoneId) {
        var proj = jq("tr#" + milestoneId + " td.title").find("a").attr("projectid");
        jq("#linkToTasks").attr("href", getMilestoneActiveTasksLink(proj, milestoneId));

        StudioBlockUIManager.blockUI(jq("#questionWindowTasks"), 400, 200, 0);
    };

    var showQuestionWindowMilestoneRemove = function(milestoneId) {
        StudioBlockUIManager.blockUI(jq("#questionWindowDeleteMilestone"), 400, 200, 0);
        jq("#questionWindowDeleteMilestone").attr("milestoneId", milestoneId);
    };

    var showDescriptionPanel = function(obj) {
        var x, y;
        jq('#descriptionPanel').show();

        x = jq(obj).offset().left;
        y = jq(obj).offset().top + 20;

        jq('#descriptionPanel .dropdown-item').show();

        jq('#descriptionPanel').css({ left: x, top: y });
    };

    var hideDescriptionPanel = function() {
        setTimeout(function() {
            if (!overDescriptionPanel) {
                jq('#descriptionPanel').hide(100);
            }
        }, 200);
    };

    jq('#descriptionPanel').on('mouseenter', function() {
        overDescriptionPanel = true;
    });

    jq('#descriptionPanel').on('mouseleave', function() {
        overDescriptionPanel = false;
        hideDescriptionPanel();
    });

    jq('#descriptionPanel .value .project').on('click', function() {
        var projectId = jq(this).attr('projectId');
        var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', projectId);
        path = jq.removeParam('tag', path);
        ASC.Controls.AnchorController.move(path);
        hideDescriptionPanel();
    });

    jq('#milestonesList').on('click', 'td.responsible span', function() {
        var responsibleId = jq(this).attr('responsibleId');
        if (responsibleId != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'responsible_for_milestone', responsibleId);
            path = jq.removeParam('user_tasks', path);
            ASC.Controls.AnchorController.move(path);
        }
    });

    jq('#milestonesList').on('click', 'td.actions .entity-menu', function() {
        hideStatusListContainer();
        var currentMilestone = selectedActionCombobox !== undefined ? selectedActionCombobox.attr('milestoneId') : -1;
        jq('#milestonesList tr#' + currentMilestone + ' td.actions .entity-menu').removeClass('selected');
        selectedActionCombobox = jq(this);

        if (selectedActionCombobox.attr('milestoneId') !== milestoneActionContainer.attr('milestoneId')) {
            milestoneActionContainer.attr('milestoneId', selectedActionCombobox.attr('milestoneId'));
            milestoneActionContainer.attr('projectId', selectedActionCombobox.attr('projectId'));
            showMilestoneActionContainer(selectedActionCombobox);
        }
        else {
            toggleMilestoneActionContainer(selectedActionCombobox);
        }
        // ga-track
        try {
            if (window._gat) {
                window._gaq.push(['_trackEvent', ga_Categories.milestones, ga_Actions.actionClick, "milestone-menu"]);
            }
        } catch (err) {
        }
        return false;
    });

    var showMilestoneActionContainer = function() {
        selectedActionCombobox.addClass('selected');


        var currentStatus = selectedActionCombobox.attr('status');
        if (currentStatus == 'closed') {
            jq('#updateMilestoneButton').hide();
            jq('#addMilestoneTaskButton').hide();
        }
        else {
            jq('#updateMilestoneButton').show();
            jq('#addMilestoneTaskButton').show();
        }
        var top = selectedActionCombobox.offset().top + selectedActionCombobox.innerHeight() - 3;
        var left = selectedActionCombobox.offset().left - milestoneActionContainer.innerWidth() + 29;

        milestoneActionContainer.css({ 'top': top, 'left': left });

        milestoneActionContainer.show();
    };

    var toggleMilestoneActionContainer = function() {
        if (milestoneActionContainer.is(':visible')) {
            selectedActionCombobox.removeClass('selected');
        } else {
            selectedActionCombobox.addClass('selected');
        }

        var currentStatus = selectedActionCombobox.attr('status');
        if (currentStatus == 'closed') {
            jq('#updateMilestoneButton').hide();
            jq('#addMilestoneTaskButton').hide();
        }
        else {
            jq('#updateMilestoneButton').show();
            jq('#addMilestoneTaskButton').show();
        }

        milestoneActionContainer.toggle();
    };

    var hideMilestoneActionContainer = function() {
        if (selectedActionCombobox) {
            selectedActionCombobox.removeClass('selected');
        }
        milestoneActionContainer.hide();
        jq("#projectActions").hide();
        jq(".project-title .menu-small").removeClass("active");
    };

    var removeMilestonesActionsForManager = function() {
        var milestones = jq("#milestonesList tr");
        for (var i = 0; i < milestones.length; i++) {
            var responsibleId = jq(milestones[i]).find(".responsible span").attr("responsibleid");
            if (responsibleId != currentUserId) {
                jq(milestones[i]).find(".entity-menu").remove();
            }
        }
        jq("#emptyListMilestone .emptyScrBttnPnl").remove();
    };

    jq('#updateMilestoneButton').on('click', function() {
        var milestoneId = milestoneActionContainer.attr('milestoneId');
        milestoneActionContainer.hide();

        var milestoneRow = jq('#milestonesList tr#' + milestoneId);

        var milestone =
        {
            id: milestoneRow.attr('id'),
            project: milestoneRow.find('td.title a').attr('projectId'),
            responsible: milestoneRow.find('td.responsible span').attr('responsibleId'),
            deadline: milestoneRow.find('td.deadline span').text(),
            title: milestoneRow.find('td.title a').text(),
            description: milestoneRow.find('td.title a').attr('description'),
            isKey: milestoneRow.attr('isKey'),
            isNotify: milestoneRow.attr('isNotify')
        };

        ASC.Projects.MilestoneAction.onGetMilestoneBeforeUpdate(milestone);
    });

    var onAddMilestone = function(params, milestone) {
        allMilestonesCount++;
        filterMilestoneCount++;
        updatePageNavigatorControl();

        var milestoneTemplate = getMilestoneTemplate(milestone);

        var firstMilestone = jq('#milestonesList tbody tr:first');
        var addedMilestone = jq.tmpl("projects_milestoneTemplate", milestoneTemplate);

        if (firstMilestone.length == 0) {
            showOrHideEmptyScreen(1);
            jq('#milestonesList tbody').append(addedMilestone);
            ASC.Projects.MilestoneAction.unlockMilestoneActionPage();
            jq.unblockUI();
            return;
        }

        addedMilestone.insertBefore(firstMilestone);
        addedMilestone.yellowFade();
        ASC.Projects.MilestoneAction.unlockMilestoneActionPage();
        jq.unblockUI();
    };
    var onUpdateMilestone = function(params, milestone) {
        var milestoneTemplate = getMilestoneTemplate(milestone);

        var updatedMilestone = jq('#milestonesList tr#' + milestone.id);

        var newMilestone = jq.tmpl("projects_milestoneTemplate", milestoneTemplate);

        updatedMilestone.replaceWith(newMilestone);
        newMilestone.yellowFade();
        ASC.Projects.MilestoneAction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var onUpdateMilestoneError = function (params, error) {
        ASC.Projects.MilestoneAction.unlockMilestoneActionPage();
        jq.unblockUI();
    };

    var onChangeStatusError = function (params, error) {
        if (error[0] == "Can not close a milestone with open tasks") {
            showQuestionWindow(params.milestoneId);
        }
    };

    var deleteMilestone = function(milestoneId) {
        var params = {};
        Teamlab.removePrjMilestone(params, milestoneId, { success: onDeleteMilestone });
    };

    var onDeleteMilestone = function(params, milestone) {
        var milestoneId = milestone.id;
        var removedMilestone = jq('#milestonesList tr#' + milestoneId);
        removedMilestone.yellowFade();
        removedMilestone.remove();

        if (currentProjectId == milestone.projectId) {
            ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.milestones, "delete");
        }

        allMilestonesCount--;
        filterMilestoneCount--;
        updatePageNavigatorControl();

        if (jq('#milestonesList tbody tr:first').length == 0) {
            clearTimeout(descriptionTimeout);
            jq('#descriptionPanel').hide();
            jq('#tableForNavigation').hide();

            if (filterMilestoneCount == 0) {
                showOrHideEmptyScreen(0);
            }
            else {
                currentPage--;
                getMilestones(currentFilter, true);
            }
        }
    };

    var onAddTask = function(params, task) {
        if (task.milestone == null) return;
        var milestoneId = task.milestone.id;
        Teamlab.getPrjMilestone({}, milestoneId, { success: onGetMilestoneAfterAddTask });
    };

    var onGetMilestoneAfterAddTask = function(params, milestone) {
        var milestoneTemplate = getMilestoneTemplate(milestone);

        var updatedMilestone = jq('#milestonesList tr#' + milestoneTemplate.id);
        var newMilestone = jq.tmpl("projects_milestoneTemplate", milestoneTemplate);

        if (updatedMilestone.length)
            updatedMilestone.replaceWith(newMilestone);
        newMilestone.yellowFade();
    };

    return {
        init: init,
        getCurrentProjectId: getCurrentProjectId,
        setCurrentFilter: setCurrentFilter,
        onAddMilestone: onAddMilestone,
        onUpdateMilestone: onUpdateMilestone,
        onUpdateMilestoneError: onUpdateMilestoneError,
        onDeleteMilestone: onDeleteMilestone,
        getMilestoneTemplate: getMilestoneTemplate,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader,
        changeCountOfRows: changeCountOfRows,
        removeMilestonesActionsForManager: removeMilestonesActionsForManager,
        onAddTask: onAddTask
    };
})(jQuery);