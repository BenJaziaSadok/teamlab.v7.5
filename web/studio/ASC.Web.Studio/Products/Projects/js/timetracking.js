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

ASC.Projects.TimeSpendActionPage = (function() {
    var basePath = 'sortBy=date&sortOrder=descending';
    var isTask = false;

    var overTimeDescrPanel = false;
    var timeDescribeTimeout = 0;

    var currentProjectId;
    var currentUserId;

    var loadListProjectsFlag = false;
    var loadListTagsFlag = false;
    var loadListMilestonesFlag = false;
    var loadTeamFlag = false;

    var currentFilter;
    var currentTimesCount;
    var lastTimerId = null;

    var totalTimeContainer;
    var selectedStatusCombobox;

    var listActionButtons = [];
    var counterSelectedItems;

    var showCheckboxFlag = false;

    var onSetFilterTime = function(evt, $container) {
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

        lastTimerId = 0;
        LoadingBanner.displayLoading();
        getTimes({ mode: 'onset' }, filter);

        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }

        if (path != basePath) {
            getTotalTimeByFilter({ mode: 'onset' }, filter);
        } else {
            totalTimeContainer.css("visibility", "hidden");
        }
    };

    var onResetFilterTime = function(evt, $container) {
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        currentFilter = filter;

        lastTimerId = 0;
        getTimes({ mode: 'onreset' }, filter);

        if (path != basePath) {
            getTotalTimeByFilter({ mode: 'onset' }, filter);
        } else {
            totalTimeContainer.css("visibility", "hidden");
        }

        LoadingBanner.displayLoading();
    };

    var createAdvansedFilter = function() {
        var now = new Date();

        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);

        var startWeek = new Date(today);
        startWeek.setDate(today.getDate() - today.getDay() + 1);

        var endWeek = new Date(today);
        endWeek.setDate(today.getDate() - today.getDay() + 7);

        var startPreviousWeek = new Date(startWeek);
        startPreviousWeek.setDate(startWeek.getDate() - 7);

        var endPreviousWeek = new Date(startWeek);
        endPreviousWeek.setDate(startWeek.getDate() - 1);

        var startPreviousMonth = new Date(today);
        startPreviousMonth.setMonth(today.getMonth() - 1);
        startPreviousMonth.setDate(1);

        var endPreviousMonth = new Date(startPreviousMonth);
        endPreviousMonth.setMonth(startPreviousMonth.getMonth() + 1);
        endPreviousMonth.setDate(endPreviousMonth.getDate() - 1);


        startPreviousWeek = startPreviousWeek.getTime();
        endPreviousWeek = endPreviousWeek.getTime();
        startPreviousMonth = startPreviousMonth.getTime();
        endPreviousMonth = endPreviousMonth.getTime();

        var ttfilters = [];
        //Projects
        if (!currentProjectId) {
            ttfilters.push({
                type: "flag",
                id: "myprojects",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyProjects,
                group: ASC.Projects.Resources.ProjectsFilterResource.ByProject,
                hashmask: "myprojects",
                groupby: "projects"
            });
            ttfilters.push({
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
            ttfilters.push({
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
            ttfilters.push({
                type: "flag",
                id: "mymilestones",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyMilestones,
                group: ASC.Projects.Resources.ProjectsFilterResource.ByMilestone,
                hashmask: "mymilestones",
                groupby: "milestones"
            });
            ttfilters.push({
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
        // Responsible

        if (currentProjectId) {
            if (ASC.Projects.Common.userInProjectTeam(Teamlab.profile.id, ASC.Projects.Master.Team)) {
                ttfilters.push({
                    type: "combobox",
                    id: "me_tasks_responsible",
                    title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                    filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                    group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                    hashmask: "person/{0}",
                    groupby: "userid",
                    options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                    bydefault: { value: currentUserId }
                });
            }
            ttfilters.push({
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
            ttfilters.push({
                type: "person",
                id: "me_tasks_responsible",
                title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid",
                bydefault: { id: currentUserId }
            });
            ttfilters.push({
                type: "person",
                id: "tasks_responsible",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
                hashmask: "person/{0}",
                groupby: "userid"
            });
        }

        ttfilters.push({
            type: "group",
            id: "group",
            title: ASC.Projects.Resources.ProjectsFilterResource.Groups,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Group + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByResponsible,
            hashmask: "group/{0}",
            groupby: "userid"
        });
        //Payment status
        ttfilters.push({
            type: "combobox",
            id: "notChargeable",
            title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotChargeable,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus,
            hashmask: "combobox/{0}",
            groupby: "payment_status",
            options:
                [
                    { value: "notChargeable", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotChargeable, def: true },
                    { value: "notBilled", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotBilled },
                    { value: "billed", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusBilled }
                ]
        },
        {
            type: "combobox",
            id: "notBilled",
            title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotBilled,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus,
            hashmask: "combobox/{0}",
            groupby: "payment_status",
            options:
                [
                    { value: "notChargeable", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotChargeable },
                    { value: "notBilled", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotBilled, def: true },
                    { value: "billed", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusBilled }
                ]
        },
        {
            type: "combobox",
            id: "billed",
            title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusBilled,
            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus + ":",
            group: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatus,
            hashmask: "combobox/{0}",
            groupby: "payment_status",
            options:
                [
                    { value: "notChargeable", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotChargeable },
                    { value: "notBilled", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusNotBilled },
                    { value: "billed", title: ASC.Projects.Resources.ProjectsFilterResource.PaymentStatusBilled, def: true }
                ]
        });
        //Create date
        ttfilters.push({
            type: "daterange",
            id: "previousweek2",
            title: ASC.Projects.Resources.ProjectsFilterResource.PreviousWeek,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.TimePeriod,
            hashmask: "period/{0}/{1}",
            groupby: "period",
            bydefault: { from: startPreviousWeek, to: endPreviousWeek }
        });
        ttfilters.push({
            type: "daterange",
            id: "previousmonth2",
            title: ASC.Projects.Resources.ProjectsFilterResource.PreviousMonth,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.TimePeriod,
            hashmask: "period/{0}/{1}",
            groupby: "period",
            bydefault: { from: startPreviousMonth, to: endPreviousMonth }
        });
        ttfilters.push({
            type: "daterange",
            id: "period2",
            title: ASC.Projects.Resources.ProjectsFilterResource.CustomPeriod,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.TimePeriod,
            hashmask: "period/{0}/{1}",
            groupby: "period"
        });

        var colCount = 3;
        if (currentProjectId) colCount = 2;

        ASC.Projects.ProjectsAdvansedFilter.filter = jq('#ProjectsAdvansedFilter').advansedFilter(
          {
              store: true,
              anykey: true,
              nykeytimeout: 1000,
              colcount: colCount,
              filters: ttfilters,
              sorters: [
                 { id: "date", title: ASC.Projects.Resources.ProjectsFilterResource.ByDate, sortOrder: "descending", def: true },
                 { id: "hours", title: ASC.Projects.Resources.ProjectsFilterResource.ByHours, sortOrder: "ascending" },
                 { id: "note", title: ASC.Projects.Resources.ProjectsFilterResource.ByNote, sortOrder: "ascending" }
             ]
          }
        )
          .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
          .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

        ASC.Projects.ProjectsAdvansedFilter.init = true;

        //filter
        ASC.Projects.ProjectsAdvansedFilter.filter.one("adv-ready", function () {
            var projectAdvansedFilterContainer = jq("#ProjectsAdvansedFilter .advansed-filter-list");
            projectAdvansedFilterContainer.find("li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'my_projects');
            projectAdvansedFilterContainer.find("li[data-id='project'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'other_projects');
            projectAdvansedFilterContainer.find("li[data-id='tag'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'with_tag');
            projectAdvansedFilterContainer.find("li[data-id='notChargeable'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'not_chargeable');
            projectAdvansedFilterContainer.find("li[data-id='notBilled'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'not_billed');
            projectAdvansedFilterContainer.find("li[data-id='billed'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'billed');
            projectAdvansedFilterContainer.find("li[data-id='mymilestones'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'my_milestones');
            projectAdvansedFilterContainer.find("li[data-id='milestone'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'other_milestones');
            projectAdvansedFilterContainer.find("li[data-id='previousweek2'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'previous_week');
            projectAdvansedFilterContainer.find("li[data-id='previousmonth2'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'previous_month');
            projectAdvansedFilterContainer.find("li[data-id='period2'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'custom_period');
            projectAdvansedFilterContainer.find("li[data-id='me_tasks_responsible'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'me-tasks-responsible');
            projectAdvansedFilterContainer.find("li[data-id='tasks_responsible'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'tasks-responsible');
            projectAdvansedFilterContainer.find("li[data-id='group'] .inner-text").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'group');

            jq("#ProjectsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, 'sort');
            jq("#ProjectsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.timeTrack, ga_Actions.filterClick, "search_text", "enter");
        });
    };

    var init = function() {
        currentProjectId = jq.getURLParam("prjID");
        currentUserId = Teamlab.profile.id;
        showCheckboxFlag = jq("#timeTrakingGroupActionMenu").length ? true : false;

        totalTimeContainer = jq(".total-time-forFilter");

        listActionButtons = jq("#timeTrakingGroupActionMenu .menuAction");
        listActionButtons.splice(0, 1);

        counterSelectedItems = jq("#timeTrakingGroupActionMenu .menu-action-checked-count");

        jq("body").append(jq.tmpl("projects_timeTrackingDescribePanelTmpl", {}));

        Teamlab.bind(Teamlab.events.removePrjTime, onRemoveTime);
        Teamlab.bind(Teamlab.events.updatePrjTime, onUpdateTime);

        var taskid = jq.getURLParam("id");
        if (taskid != null && typeof taskid != 'undefined') {
            isTask = true;
            Teamlab.getPrjTime({}, taskid, {
                success: function (data, times) {
                    if (times.length) {
                        jq.each(times, function(i, time) {
                            times[i].showCheckbox = showCheckboxFlag;
                        });
                        jq.tmpl("projects_timeTrackingTmpl", times).appendTo('#timeSpendsList tbody');
                        showTotalCountForTask(times);
                    }
                    showEmptyScreen(times.length);
                }
            });

        } else {
            //filter
            ASC.Projects.ProjectsAdvansedFilter.initialisation(currentUserId, basePath);
            ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterTime;
            ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterTime;

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
        }

        var options = {
            menuSelector: "#timeTrakingGroupActionMenu",
            menuAnchorSelector: "#selectAllTimers",
            menuSpacerSelector: "#mainContent .header-menu-spacer",
            userFuncInTop: function() { jq("#timeTrakingGroupActionMenu .menu-action-on-top").hide(); },
            userFuncNotInTop: function() { jq("#timeTrakingGroupActionMenu .menu-action-on-top").show(); }
        };
        ScrolledGroupMenu.init(options);

        // discribe panel
        jq("#timeSpendsList").on("mouseenter", ".pm-ts-noteColumn a", function(event) {
            timeDescribeTimeout = setTimeout(function() {
                var targetObject = event.target;
                var describePanel = jq('#timeTrackingDescrPanel');
                var created = describePanel.find('.created');
                var createdby = describePanel.find('.createdby');

                jq(created, createdby).hide();

                var createdAttr = jq(targetObject).attr('created');
                var createdAttrBy = jq(targetObject).attr('createdby');

                if (typeof createdAttr != 'undefined' && jq.trim(createdAttr) != "") {
                    created.find('.value').html(createdAttr);
                    created.show();
                }

                if (typeof createdAttrBy != 'undefined') {
                    createdby.find('.value').html(Encoder.htmlEncode(createdAttrBy));
                    createdby.show();
                }

                if (createdAttr.length || createdAttrBy.length) {
                    showTimeDescribePanel(targetObject);
                    overTimeDescrPanel = true;
                }
            }, 500, this);
        });
        jq('#timeSpendsList').on('mouseleave', '.pm-ts-noteColumn a', function() {
            clearTimeout(timeDescribeTimeout);
            overTimeDescrPanel = false;
            hideDescrPanel();
        });
        jq('#timeTrackingDescrPanel').on('mouseenter', function() {
            overTimeDescrPanel = true;
        });

        jq('#timeTrackingDescrPanel').on('mouseleave', function() {
            overTimeDescrPanel = false;
            hideDescrPanel();
        });

        jq('#timeSpendsList').on('click', ".entity-menu", function() {
            jq('#timeSpendsList .entity-menu').removeClass('show');
            if (jq('.studio-action-panel:visible').length) jq(this).removeClass('show');
            else jq(this).addClass('show');
            showActionsPanel('timeActionPanel', this);
            return false;
        });

        jq('#emptyScreenForTimers .addFirstElement').click(function() {
            if (isTask) {
                var taskId = jq.getURLParam("ID");
                ASC.Projects.Common.showTimer('timer.aspx?prjID=' + currentProjectId + '&ID=' + taskId);
            } else {
                if (currentProjectId != null) {
                    ASC.Projects.Common.showTimer('timer.aspx?prjID=' + currentProjectId);
                } else {
                    ASC.Projects.Common.showTimer('timer.aspx');
                }
            }
        });

        jq('#timeActionPanel #ta_edit').click(function() {
            var id = jq(this).attr('timeid');
            var prjId = jq(this).attr('prjid');
            var record = jq('.timeSpendRecord[timeid=' + id + ']');
            var taskId = jq(record).attr('taskid');
            var taskTitle = jq(record).find('.pm-ts-noteColumn').find('a').text();
            var recordNote = jq(record).find('.pm-ts-noteColumn').find('span').text();
            var date = jq(record).attr('date');
            var separateTime = jq.trim(jq(record).find('.pm-ts-hoursColumn').text()).split(' ');
            var responsible = jq(record).find('.pm-ts-personColumn').find('span').attr('userid');
            jq('#timeActionPanel').hide();

            ASC.Projects.TimeTrakingEdit.showPopup(prjId, taskId, taskTitle, id, { hours: parseInt(separateTime[0], 10), minutes: parseInt(separateTime[1], 10) }, date, recordNote, responsible);
        });

        jq('#timeActionPanel #ta_remove').on('click', function() {
            var id = jq(this).attr('timeid');
            jq('#timeActionPanel').hide();
            Teamlab.removePrjTime({ timeid: id }, { timeids: [id] }, { error: onUpdatePrjTimeError });
        });

        jq('#showNext').bind('click', function() {
            jq('#showNext').hide();
            jq('#showNextProcess').show();
            getTimes({ mode: 'next' }, currentFilter);
        });

        jq("#timeSpendsList").on("click", ".pm-ts-personColumn span", function() {
            var userid = jq(this).attr('userid');
            if (!isTask && userid != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
                var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'tasks_responsible', userid);
                path = jq.removeParam('noresponsible', path);
                ASC.Controls.AnchorController.move(path);
            }
        });
        jq('#emptyScreenForFilter').on('click', '.clearFilterButton', function() {
            lastTimerId = 0;
            jq('#ProjectsAdvansedFilter').advansedFilter(null);
            return false;
        });

        jq('#timeSpendsList').on('click', '.changeStatusCombobox.canEdit', function(event) {
            var status = jq(this).find('span:first').attr('class');

            if (selectedStatusCombobox)
                selectedStatusCombobox.removeClass('selected');
            selectedStatusCombobox = jq(this);

            jq('#statusListContainer').attr('data-timeid', jq(this).attr('data-timeid'));
            showStatusListContainer(status);

            return false;
        });

        jq('#statusListContainer li').on('click', function() {
            if (jq(this).is('.selected')) return;
            var timeId = jq('#statusListContainer').attr('data-timeid');
            var status = jq(this).attr('class').split(" ")[0];

            if (status == jq('.timeSpendRecord[timeid=' + timeId + '] .changeStatusCombobox span').attr('class')) return;

            changePaymentStatus(status, timeId);
        });

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is(".studio-action-panel") || jq(elt).is('.entity-menu')) {
                isHide = false;
            }

            if (isHide) {
                jq('.studio-action-panel').hide();
                jq('.entity-menu').removeClass('show');
                if (selectedStatusCombobox)
                    selectedStatusCombobox.removeClass('selected');
            }
        });

        // group menu

        jq('#deleteTimersButton').on('click', function() {
            jq.unblockUI();
            removeChackedTimers();
            return false;
        });

        jq("#selectAllTimers").change(function() {
            var checkboxes = jq("#timeSpendsList input");
            var rows = jq("#timeSpendsList tr");

            if (jq(this).is(":checked")) {
                checkboxes.attr("checked", "checked");
                rows.addClass("checked-row")
                unlockActionButtons();
            } else {
                checkboxes.removeAttr("checked");
                rows.removeClass("checked-row");
                lockActionButtons();
            }
        });

        jq("#mainContent").on("click", ".unlockAction", function() {
            var actionType = jq(this).attr("data-status");
            var status;
            switch (actionType) {
                case "not-chargeable":
                    status = 0;
                    break;
                case "not-billed":
                    status = 1;
                    break;
                case "billed":
                    status = 2;
                    break;
                default:
                    showQuestionWindow();
                    return;
            }
            changePaymentSatusByCheckedTimers(actionType, status);
        });

        jq("#timeSpendsList").on("change", "input", function() {
            var input = jq(this);
            var timerId = input.attr("data-timeId");

            if (input.is(":checked")) {
                jq("tr[timeid=" + timerId + "]").addClass("checked-row");
            } else {
                jq("tr[timeid=" + timerId + "]").removeClass("checked-row");
                jq("#selectAllTimers").removeAttr("checked");
            }

            var countCheckedTimers = getCountCheckedTimers();

            if (countCheckedTimers > 0) {
                if (countCheckedTimers == 1) {
                    unlockActionButtons();
                } else {
                    changeSelectedItemsCounter();
                }
            } else {
                lockActionButtons();
            }
        });

        jq("#deselectAllTimers").click(function() {
            var checkboxes = jq("#timeSpendsList input");
            var rows = jq("#timeSpendsList tr");

            jq("#selectAllTimers").removeAttr("checked");
            checkboxes.removeAttr("checked");
            rows.removeClass("checked-row");
            lockActionButtons();
        });

        //ga-track-events
        //show next
        jq("#showNext").trackEvent(ga_Categories.timeTrack, ga_Actions.next, 'next-times');
        //responsible
        jq("td[id^=person_]").trackEvent(ga_Categories.timeTrack, ga_Actions.userClick, "tasks-responsible");

        //end ga-track-events
    };
    var getCountCheckedTimers = function() {
        var timers = jq("#timeSpendsList input:checked");
        return timers.length;
    };

    var getTimersIds = function() {
        var timers = jq("#timeSpendsList input:checked");

        var timersIds = [];

        for (var i = 0; i < timers.length; i++) {
            timersIds.push(jq(timers[i]).attr("data-timeId"));
        }
        return timersIds;
    };

    var changeSelectedItemsCounter = function() {
        var checkedInputCount = jq("#timeSpendsList input:checked").length;
        counterSelectedItems.find("span").text(checkedInputCount + " " + ASC.Projects.Resources.ProjectsJSResource.GroupMenuSelectedItems);
    };

    var unlockActionButtons = function() {
        jq(listActionButtons).addClass("unlockAction");
        changeSelectedItemsCounter();
        counterSelectedItems.show();
    };

    var lockActionButtons = function() {
        jq(listActionButtons).removeClass("unlockAction");
        counterSelectedItems.hide();
    };

    var showQuestionWindow = function() {
        StudioBlockUIManager.blockUI(jq("#questionWindowTimerRemove"), 400, 300, 0);
        PopupKeyUpActionProvider.EnterAction = "jq('#deleteTimersButton').click();";
    };

    var removeChackedTimers = function() {
        var timersIds = getTimersIds();

        LoadingBanner.displayLoading();
        Teamlab.removePrjTime({}, { timeids: timersIds });
    };

    var changePaymentSatusByCheckedTimers = function(textStatus, status) {
        var timersIds = getTimersIds();

        LoadingBanner.displayLoading();
        Teamlab.changePaymentStatus({ textStatus: textStatus }, { status: status, timeIds: timersIds }, onChangePaymentStatus);
    };

    var changePaymentStatus = function(textStatus, timeId) {
        var status = 0;
        switch (textStatus) {
            case "not-billed":
                status = 1;
                break;
            case "billed":
                status = 2;
                break;
        }
        LoadingBanner.displayLoading();
        Teamlab.changePaymentStatus({ textStatus: textStatus }, { status: status, timeIds: [timeId] }, onChangePaymentStatus);
    };

    var onChangePaymentStatus = function(params, times) {
        for (var i = 0; i < times.length; i++) {
            jq('.timeSpendRecord[timeid=' + times[i].id + '] .changeStatusCombobox span:first').attr('class', params.textStatus);
        }
        LoadingBanner.hideLoading();
    };

    var showStatusListContainer = function(status) {
        var statusListContainer = jq("#statusListContainer");
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

    var hideDescrPanel = function() {
        setTimeout(function() {
            if (!overTimeDescrPanel) jq('#timeTrackingDescrPanel').hide(100);
        }, 200);
    };

    var showTotalCountForTask = function(times) {
        var timesCount = times.length;
        if (!timesCount) return;
        var totalTime = 0;
        var billedTime = 0;
        for (var i = 0; i < timesCount; i++) {
            totalTime += times[i].hours;

            if (times[i].paymentStatus == 2) {
                billedTime += times[i].hours;
            }
        }

        var taskTotalTime = jq.timeFormat(totalTime).split(":");
        totalTimeContainer.find(".total-count .hours").text(taskTotalTime[0]);
        totalTimeContainer.find(".total-count .minutes").text(taskTotalTime[1]);

        var taskBilledTime = jq.timeFormat(billedTime).split(":");
        totalTimeContainer.find(".billed-count .hours").text(taskBilledTime[0]);
        totalTimeContainer.find(".billed-count .minutes").text(taskBilledTime[1]);

        totalTimeContainer.addClass("float-none");
        totalTimeContainer.css("visibility", "visible");
    };

    var showTimeDescribePanel = function(targetObject) {
        var x = jq(targetObject).offset().left + 10;
        var y = jq(targetObject).offset().top + 20;
        jq('#timeTrackingDescrPanel').css({ left: x, top: y });
        jq('#timeTrackingDescrPanel').show();

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="#timeTrackingDescrPanel"]')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="#timeTrackingDescrPanel"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('.studio-action-panel').hide();
            }
        });
    };

    var showEmptyScreen = function(isItems) {
        if (isTask) {
            if (isItems) {
                jq('#emptyScreenForTimers').hide();
                jq("#timeTrakingGroupActionMenu").show();
            } else {
                jq('#emptyScreenForTimers').show();
                jq("#timeTrakingGroupActionMenu").hide();
                totalTimeContainer.css("visibility", "hidden");
            }
        } else {
            var oldRecord = jq('#timeSpendsList .timeSpendRecord');
            if (oldRecord.length) {
                if (!isItems) {
                    jq('#emptyScreenForFilter').show();
                    jq('#ProjectsAdvansedFilter').css("visibility", "visible");
                    jq("#timeTrakingGroupActionMenu").hide();
                    totalTimeContainer.css("visibility", "hidden");
                }
            } else {
                if (ASC.Controls.AnchorController.getAnchor() == basePath) {
                    if (!isItems) {
                        jq('#ProjectsAdvansedFilter').hide();
                        jq("#timeTrakingGroupActionMenu").hide();
                        totalTimeContainer.css("visibility", "hidden");
                        jq('#emptyScreenForTimers').show();
                    } else {
                        jq('#emptyScreenForFilter').hide();
                        jq('#ProjectsAdvansedFilter').css("visibility", "visible");
                        jq("#timeTrakingGroupActionMenu").show();
                    }
                } else {
                    if (!isItems) {
                        jq('#emptyScreenForFilter').show();
                        jq("#timeTrakingGroupActionMenu").hide();
                        totalTimeContainer.css("visibility", "hidden");
                        jq('#ProjectsAdvansedFilter').css("visibility", "visible");
                    } else {
                        jq('#emptyScreenForFilter').hide();
                        jq('#ProjectsAdvansedFilter').css("visibility", "visible");
                        jq("#timeTrakingGroupActionMenu").show();
                    }
                }
            }
        }
    };

    var getTimes = function(params, filter) {
        if (params.mode != 'next') {
            currentTimesCount = 0;
        }
        filter.Count = 31;
        filter.StartIndex = currentTimesCount;
        if (lastTimerId) {
            filter.lastId = lastTimerId;
        }

        Teamlab.getPrjTime(params, null, { filter: filter, success: onGetTimes });
    };

    var getTotalTimeByFilter = function(params, filter) {
        if (params.mode != 'next') {
            currentTimesCount = 0;
        }
        filter.Count = 31;
        filter.StartIndex = currentTimesCount;

        Teamlab.getTotalTimeByFilter(params, { filter: filter, success: onGetTotalTime });

        Teamlab.getTotalBilledTimeByFilter(params, { filter: filter, success: onGetTotalBilledTime });
    };

    var onGetTimes = function(params, data) {
        var count = data.length;
        currentTimesCount += count;
        showEmptyScreen(count);

        if (count > 0) {
            lastTimerId = data[count - 1].id;
        }

        clearTimeout(timeDescribeTimeout);
        overTimeDescrPanel = false;
        hideDescrPanel();

        if (params.mode != 'next') {
            jq('#timeSpendsList tbody').html('');
        }

        jq.each(data, function(i, time) {
            data[i].showCheckbox = showCheckboxFlag;
        });

        jq.tmpl("projects_timeTrackingTmpl", data).appendTo('#timeSpendsList tbody');

        jq('#showNextProcess').hide();
        if (data.length > 30) {
            delete data[data.length - 1];
            jq('#showNext').show();
        } else {
            jq('#showNext').hide();
        }

        LoadingBanner.hideLoading();
    };

    var onGetTotalTime = function(params, time) {
        if (time != 0) {
            var fotmatedTime = jq.timeFormat(time).split(":");
            totalTimeContainer.find(".total-count .hours").text(fotmatedTime[0]);
            totalTimeContainer.find(".total-count .minutes").text(fotmatedTime[1]);

            totalTimeContainer.find(".billed-count .hours").text("0");
            totalTimeContainer.find(".billed-count .minutes").text("00");

            totalTimeContainer.css("visibility", "visible");
        } else {
            totalTimeContainer.css("visibility", "hidden");
        }
    };

    var onGetTotalBilledTime = function(params, time) {
        var billedTimeContainer = totalTimeContainer.find(".billed-count");
        if (time != 0) {
            var fotmatedTime = jq.timeFormat(time).split(":");
            billedTimeContainer.find(".hours").text(fotmatedTime[0]);
            billedTimeContainer.find(".minutes").text(fotmatedTime[1]);
        } else {
            billedTimeContainer.find(".hours").text("0");
            billedTimeContainer.find(".minutes").text("00");
        }
    };

    var onUpdateTime = function(params, time) {
        time.showCheckbox = showCheckboxFlag;
        jq('#timeSpendsList .timeSpendRecord[timeid=' + time.id + ']').replaceWith(jq.tmpl("projects_timeTrackingTmpl", time));
        if (!params.oldTime || !currentProjectId) return;

        var updatedTimeText = jq.timeFormat(time.hours).split(":");
        var updatedTime = { hours: parseInt(updatedTimeText[0], 10), minutes: parseInt(updatedTimeText[1], 10) };

        var difference = { hours: updatedTime.hours - params.oldTime.hours, minutes: updatedTime.minutes - params.oldTime.minutes }

        ASC.Projects.projectNavPanel.changeCommonProjectTime(difference);

    };

    var onRemoveTime = function(params, data) {
        var totalTime = 0;
        for (var i = 0; i < data.length; i++) {
            var timer = jq("#timeSpendRecord" + data[i].id);
            totalTime += data[i].hours;
            timer.animate({ opacity: "hide" }, 500);
            setTimeout(function() {
                timer.remove();
                showEmptyScreen(jq("#timeSpendsList .timeSpendRecord").length);
            }, 500);
        }
        var timeText = jq.timeFormat(totalTime).split(":");
        ASC.Projects.projectNavPanel.changeCommonProjectTime({ hours: -parseInt(timeText[0], 10), minutes: -parseInt(timeText[1], 10) });

        LoadingBanner.hideLoading();
    };
    
    var onUpdatePrjTimeError = function(params, data) {
        jq("div.entity-menu[timeid=" + params.timeid + "]").hide();
    };
    
    var showActionsPanel = function(panelId, obj) {
        var objid = '',
            objidAttr = '',
            y = 0;
        if (typeof jq(obj).attr('timeid') != 'undefined') {
            jq('#timeActionPanel .dropdown-item').attr('timeid', jq(obj).attr('timeid')).attr('prjid', jq(obj).attr('prjid')).attr('userid', jq(obj).attr('userid'));
        }
        if (panelId == 'timeActionPanel') objid = jq(obj).attr('timeid');
        if (objid.length) objidAttr = '[objid=' + objid + ']';
        if (jq('#' + panelId + ':visible' + objidAttr).length) {
            jq("body").unbind("click");
            jq('.studio-action-panel').hide();
        } else {
            jq('.studio-action-panel').hide();
            jq('#' + panelId).show();

            x = jq(obj).offset().left - 131;
            jq('#' + panelId).attr('objid', objid);
            y = jq(obj).offset().top + 18;
            jq('#' + panelId).css({ left: x, top: y });

            jq('body').click(function(event) {
                var elt = (event.target) ? event.target : event.srcElement;
                var isHide = true;
                if (jq(elt).is('[id="' + panelId + '"]') || (elt.id == obj.id && obj.id.length) || jq(elt).is('.entity-menu')) {
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
                    jq('.studio-action-panel').hide();
                    jq('.entity-menu').removeClass('show');
                }
            });

        }
    };

    return {
        init: init
    };
})(jQuery);
