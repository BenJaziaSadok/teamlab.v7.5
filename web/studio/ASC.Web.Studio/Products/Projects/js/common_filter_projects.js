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

ASC.Projects.ProjectsAdvansedFilter = (function() {
    var myGuid,
    anchorMoving = false,
    init = false,
    firstload = true,
    hashFilterChanged = true;
    var basePath = "";
    var baseSortBy = "";
    var massNameFilters = {

        team_member: "team_member",
        me_team_member: "me_team_member",

        me_responsible_for_milestone: "me_responsible_for_milestone",
        responsible_for_milestone: "responsible_for_milestone",

        me_project_manager: "me_project_manager",
        project_manager: "project_manager",

        me_author: "me_author",
        author: "author",

        user: "user",

        me_tasks_responsible: "me_tasks_responsible",
        tasks_responsible: "tasks_responsible",

        me_tasks_creator: "me_tasks_creator",
        tasks_creator: "tasks_creator",

        me_tasks: "me_tasks",
        user_tasks: "user_tasks",

        noresponsible: "noresponsible",

        group: "group",
        followed: "followed",
        tag: "tag",
        text: "text",

        project: "project",
        myprojects: "myprojects",

        milestone: "milestone",
        mymilestones: "mymilestones",

        status: "status",
        open: "open",
        closed: "closed",
        paused: "paused",

        payment_status: "payment_status",
        not_chargeable: "notChargeable",
        not_billed: "notBilled",
        billed: "billed",

        overdue: "overdue",
        today: "today",
        upcoming: "upcoming",
        recent: "recent",
        deadlineStart: "deadlineStart",
        deadlineStop: "deadlineStop",
        createdStart: "createdStart",
        createdStop: "createdStop",
        periodStart: "periodStart",
        periodStop: "periodStop",

        entity: "entity",
        project_entity: "project_entity",
        milestone_entity: "milestone_entity",
        discussion_entity: "discussion_entity",
        team_entity: "team_entity",
        task_entity: "task_entity",
        subtask_entity: "subtask_entity",
        time_entity: "time_entity",
        comment_entity: "comment_entity",

        sortBy: "sortBy",
        sortOrder: "sortOrder"
    };
    var massFilters = [
    "team_member",
    "project_manager",
    "responsible_for_milestone",
    "tasks_responsible",
    "tasks_creator",
    "author",
    "user",

    "noresponsible",

    "user_tasks",
    "group",
    "followed",
    "tag",
    "text",

    "project",
    "myprojects",

    "mymilestones",
    "milestone",

    "status",
    "payment_status",

    "overdue",
    "today",
    "upcoming",
    "recent",
    "deadlineStart",
    "deadlineStop",
    "createdStart",
    "createdStop",
    "periodStart",
    "periodStop",

    "entity",

    "sortBy",
    "sortOrder"];

    var onMovedHash = function() {
        if (!ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged) {
            setFilterByUrl();
        } else {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = false;
        }
    };

    var initialisation = function(userGuid, bsPath) {
        myGuid = userGuid;
        basePath = bsPath;
        var res = /sortBy=(.+)\&sortOrder=(.+)/ig.exec(basePath);
        if (res && res.length == 3) {
            baseSortBy = res[1];
        }
        ASC.Controls.AnchorController.bind(/^(.+)*$/, onMovedHash);
    };

    var getUrlParam = function(name, str) {
        var regexS = "[#&]" + name + "=([^&]*)";
        var regex = new RegExp(regexS);
        var tmpUrl = "#";
        if (str) {
            tmpUrl += str;
        } else {
            tmpUrl += ASC.Controls.AnchorController.getAnchor();
        }
        var results = regex.exec(tmpUrl);
        if (results == null)
            return "";
        else
            return results[1];
    };

    var coincidesWithFilter = function(filter) {
        var hash = ASC.Controls.AnchorController.getAnchor();

        var sortOrder = getUrlParam(massNameFilters.sortOrder, hash);
        var sortBy = getUrlParam(massNameFilters.sortBy, hash);

        if (sortBy == "" && sortOrder == "") {
            hash = basePath + "&" + hash;
        }

        for (var i = 0; i < massFilters.length; i++) {
            var paramName = massFilters[i];
            var filterParam = getUrlParam(paramName, filter);
            var hashParam = getUrlParam(paramName, hash);
            if (filterParam != hashParam) {
                return false;
            }
        }
        return true;
    };

    var setFilterByUrl = function() {
        if (ASC.Projects.ProjectsAdvansedFilter.firstload) {
            ASC.Projects.ProjectsAdvansedFilter.firstload = false;
        }

        var hash = ASC.Controls.AnchorController.getAnchor();
        if (hash == "") {
            location.hash = basePath;
            return;
        }
        var team_member = getUrlParam(massNameFilters.team_member),
            project_manager = getUrlParam(massNameFilters.project_manager),
            responsible_for_milestone = getUrlParam(massNameFilters.responsible_for_milestone),
            tasks_responsible = getUrlParam(massNameFilters.tasks_responsible),
            tasks_creator = getUrlParam(massNameFilters.tasks_creator),
            author = getUrlParam(massNameFilters.author),
            user = getUrlParam(massNameFilters.user),

            user_tasks = getUrlParam(massNameFilters.user_tasks),
            noresponsible = getUrlParam(massNameFilters.noresponsible),

            group = getUrlParam(massNameFilters.group),

            followed = getUrlParam(massNameFilters.followed),
            tag = getUrlParam(massNameFilters.tag),
            text = decodeURIComponent(getUrlParam(massNameFilters.text)),

            project = getUrlParam(massNameFilters.project),
            myprojects = getUrlParam(massNameFilters.myprojects),

            mymilestones = getUrlParam(massNameFilters.mymilestones),
            milestone = getUrlParam(massNameFilters.milestone),

            status = getUrlParam(massNameFilters.status),
            payment_status = getUrlParam(massFilters.payment_status),

            overdue = getUrlParam(massNameFilters.overdue),
            deadlineStart = getUrlParam(massNameFilters.deadlineStart),
            deadlineStop = getUrlParam(massNameFilters.deadlineStop),
            createdStart = getUrlParam(massNameFilters.createdStart),
            createdStop = getUrlParam(massNameFilters.createdStop),
            periodStart = getUrlParam(massNameFilters.periodStart),
            periodStop = getUrlParam(massNameFilters.periodStop),

            entity = getUrlParam(massNameFilters.entity),

            sortBy = getUrlParam(massNameFilters.sortBy),
            sortOrder = getUrlParam(massNameFilters.sortOrder);

        filters = [];
        sorters = [];

        // Responsible
        if (team_member.length > 0) {
            filters.push({ type: "person", id: massNameFilters.team_member, isset: true, params: { id: team_member} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_team_member, reset: true });
            filters.push({ type: "person", id: massNameFilters.team_member, reset: true });
        }

        if (project_manager.length > 0) {
            filters.push({ type: "person", id: massNameFilters.project_manager, isset: true, params: { id: project_manager} });
        } else {
            filters.push({ type: "person", id: massNameFilters.me_project_manager, reset: true });
            filters.push({ type: "person", id: massNameFilters.project_manager, reset: true });
        }

        if (tasks_responsible.length > 0) {
            if (jq.getURLParam("prjID")) {
                filters.push({ type: "combobox", id: massNameFilters.tasks_responsible, isset: true, params: { value: tasks_responsible} });
            } else {
                filters.push({ type: "person", id: massNameFilters.tasks_responsible, isset: true, params: { id: tasks_responsible} });
            }
        } else {
            filters.push({ type: "person", id: massNameFilters.tasks_responsible, reset: true });
            filters.push({ type: "person", id: massNameFilters.me_tasks_responsible, reset: true });
        }

        if (tasks_creator.length > 0) {
            filters.push({ type: "person", id: massNameFilters.tasks_creator, isset: true, params: { id: tasks_creator} });
        } else {
            filters.push({ type: "person", id: massNameFilters.tasks_creator, reset: true });
            filters.push({ type: "person", id: massNameFilters.me_tasks_creator, reset: true });
        }

        if (responsible_for_milestone.length > 0) {
            if (jq.getURLParam("prjID")) {
                filters.push({ type: "combobox", id: massNameFilters.responsible_for_milestone, isset: true, params: { value: responsible_for_milestone} });
            } else {
                filters.push({ type: "person", id: massNameFilters.responsible_for_milestone, isset: true, params: { id: responsible_for_milestone} });
            }
        } else {
            filters.push({ type: "person", id: massNameFilters.me_responsible_for_milestone, reset: true });
            filters.push({ type: "person", id: massNameFilters.responsible_for_milestone, reset: true });
        }

        if (author.length > 0) {
            if (jq.getURLParam("prjID")) {
                filters.push({ type: "combobox", id: massNameFilters.author, isset: true, params: { value: author} });
            } else {
                filters.push({ type: "person", id: massNameFilters.author, isset: true, params: { id: author} });
            }
        } else {
            filters.push({ type: "person", id: massNameFilters.me_author, reset: true });
            filters.push({ type: "person", id: massNameFilters.author, reset: true });
        }

        if (user.length > 0) {
            filters.push({ type: "person", id: massNameFilters.user, isset: true, params: { id: user} });
        } else {
            filters.push({ type: "person", id: massNameFilters.user, reset: true });
        }

        if (noresponsible.length > 0) {
            filters.push({ type: "flag", id: "noresponsible", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "noresponsible", reset: true });
        }

        if (group.length > 0) {
            filters.push({ type: "group", id: massNameFilters.group, isset: true, params: { id: group} });
        } else {
            filters.push({ type: "group", id: massNameFilters.group, reset: true });
        }

        //Tasks
        if (user_tasks.length > 0) {
            if (jq.getURLParam("prjID")) {
                filters.push({ type: "combobox", id: massNameFilters.user_tasks, isset: true, params: { value: user_tasks} });
            } else {
                filters.push({ type: "person", id: massNameFilters.user_tasks, isset: true, params: { id: user_tasks} });
            }
        } else {
            filters.push({ type: "person", id: massNameFilters.me_tasks, reset: true });
            filters.push({ type: "person", id: massNameFilters.user_tasks, reset: true });
        }

        // Milestone
        if (mymilestones.length > 0) {
            filters.push({ type: "flag", id: "mymilestones", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "mymilestones", reset: true });
        }
        if (milestone.length > 0) {
            filters.push({ type: "combobox", id: "milestone", params: { value: milestone} });
        } else {
            filters.push({ type: "combobox", id: "milestone", reset: true });
        }

        // Project
        if (project.length > 0) {
            filters.push({ type: "combobox", id: "project", isset: true, params: { value: project} });
        } else {
            filters.push({ type: "combobox", id: "project", reset: true });
        }
        if (myprojects.length > 0) {
            filters.push({ type: "flag", id: "myprojects", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "myprojects", reset: true });
        }

        // Tag
        if (tag.length > 0) {
            filters.push({ type: "combobox", id: "tag", isset: true, params: { value: tag} });
        } else {
            filters.push({ type: "combobox", id: "tag", reset: true });
        }

        // Status
        if (status.length > 0) {
            filters.push({ type: "combobox", id: status, isset: true, params: { value: status} });
        } else {
            filters.push({ type: "combobox", id: "open", reset: true });
            filters.push({ type: "combobox", id: "paused", reset: true });
            filters.push({ type: "combobox", id: "closed", reset: true });
        }

        // Payment status
        if (payment_status.length > 0) {
            filters.push({ type: "combobox", id: status, isset: true, params: { value: payment_status} });
        } else {
            filters.push({ type: "combobox", id: "notChargeable", reset: true });
            filters.push({ type: "combobox", id: "notBilled", reset: true });
            filters.push({ type: "combobox", id: "billed", reset: true });
        }

        // due date
        if (overdue.length > 0) {
            filters.push({ type: "flag", id: "overdue", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "overdue", reset: true });
        }
        if (deadlineStart.length > 0 && deadlineStop.length > 0) {
            filters.push({ type: "daterange", id: "deadline", isset: true, params: { from: deadlineStart, to: deadlineStop} });
        } else {
            filters.push({ type: "daterange", id: "today", reset: true });
            filters.push({ type: "daterange", id: "upcoming", reset: true });
            filters.push({ type: "daterange", id: "deadline", reset: true });
        }
        if (createdStart.length > 0 && createdStop.length > 0) {
            filters.push({ type: "daterange", id: "created", isset: true, params: { from: createdStart, to: createdStop} });
        } else {
            filters.push({ type: "daterange", id: "today2", reset: true });
            filters.push({ type: "daterange", id: "recent", reset: true });
            filters.push({ type: "daterange", id: "created", reset: true });
        }
        if (periodStart.length > 0 && periodStop.length > 0) {
            filters.push({ type: "daterange", id: "period", isset: true, params: { from: periodStart, to: periodStop} });
        } else {
            filters.push({ type: "daterange", id: "today3", reset: true });
            filters.push({ type: "daterange", id: "yesterday", reset: true });
            filters.push({ type: "daterange", id: "currentweek", reset: true });
            filters.push({ type: "daterange", id: "previousweek", reset: true });
            filters.push({ type: "daterange", id: "currentmonth", reset: true });
            filters.push({ type: "daterange", id: "previousmonth", reset: true });
            filters.push({ type: "daterange", id: "currentyear", reset: true });
            filters.push({ type: "daterange", id: "previousyear", reset: true });
        }

        // Text
        if (text.length > 0) {
            filters.push({ type: "text", id: "text", isset: true, params: { value: text} });
        } else {
            filters.push({ type: "text", id: "text", reset: true, params: { value: null} });
        }

        // Other
        if (followed.length > 0) {
            filters.push({ type: "flag", id: "followed", isset: true, params: {} });
        } else {
            filters.push({ type: "flag", id: "followed", reset: true });
        }

        // Entity
        if (entity.length > 0) {
            filters.push({ type: "combobox", id: entity.toLowerCase() + "_entity", isset: true, params: { value: entity.toLowerCase()} });
        } else {
            filters.push({ type: "combobox", id: "project_entity", reset: true });
            filters.push({ type: "combobox", id: "milestone_entity", reset: true });
            filters.push({ type: "combobox", id: "discussion_entity", reset: true });
            filters.push({ type: "combobox", id: "team_entity", reset: true });
            filters.push({ type: "combobox", id: "task_entity", reset: true });
            filters.push({ type: "combobox", id: "subtask_entity", reset: true });
            filters.push({ type: "combobox", id: "comment_entity", reset: true });
        }

        // Sorters
        if (sortBy.length > 0 && sortOrder.length > 0) {
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: sortOrder });
        } else if (sortBy.length > 0) {
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: "descending" });
        } else if (sortOrder.length > 0) {
            sortBy = baseSortBy;
            sorters.push({ type: "sorter", id: sortBy, selected: true, sortOrder: sortOrder });
        }

        jq("#ProjectsAdvansedFilter").advansedFilter({ filters: filters, sorters: sorters });
    };

    var makeData = function($container, type, prjId) {
        var data = {}, anchor = "", filters = $container.advansedFilter();
        var projectId = prjId;
        if (projectId) {
            data.projectId = projectId;
        }
        for (var filterInd = 0; filterInd < filters.length; filterInd++) {
            switch (filters[filterInd].id) {
                case "me_team_member":
                case "team_member":
                    data.participant = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "team_member", data.participant);
                    break;
                case "me_project_manager":
                case "project_manager":
                    data.manager = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "project_manager", data.manager);
                    break;
                case "me_responsible_for_milestone":
                case "responsible_for_milestone":
                    if (filters[filterInd].params.id) {
                        data.milestoneResponsible = filters[filterInd].params.id;
                    } else {
                        data.milestoneResponsible = filters[filterInd].params.value;
                    }
                    anchor = jq.changeParamValue(anchor, "responsible_for_milestone", data.milestoneResponsible);
                    break;
                case "me_tasks_responsible":
                case "tasks_responsible":
                    if (filters[filterInd].params.id) {
                        data.participant = filters[filterInd].params.id;
                    } else {
                        data.participant = filters[filterInd].params.value;
                    }
                    anchor = jq.changeParamValue(anchor, "tasks_responsible", data.participant);
                    break;
                case "me_tasks_creator":
                case "tasks_creator":
                    data.creator = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "tasks_creator", data.creator);
                    break;
                case "me_author":
                case "author":
                    if (filters[filterInd].params.id) {
                        data.participant = filters[filterInd].params.id;
                    } else {
                        data.participant = filters[filterInd].params.value;
                    }
                    anchor = jq.changeParamValue(anchor, "author", data.participant);
                    break;
                case "user":
                    data.user = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "user", data.user);
                    break;
                case "group":
                    data.departament = filters[filterInd].params.id;
                    anchor = jq.changeParamValue(anchor, "group", data.departament);
                    break;
                case "mymilestones":
                    data.mymilestones = "true";
                    anchor = jq.changeParamValue(anchor, "mymilestones", "true");
                    break;
                case "milestone":
                    data.milestone = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "milestone", data.milestone);
                    break;
                case "noresponsible":
                    data.participant = "00000000-0000-0000-0000-000000000000";
                    anchor = jq.changeParamValue(anchor, "noresponsible", "true");
                    break;
                case "myprojects":
                    data.myprojects = "true";
                    anchor = jq.changeParamValue(anchor, "myprojects", "true");
                    break;
                case "project":
                    data.projectId = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "project", data.projectId);
                    break;
                case "me_tasks":
                case "user_tasks":
                    if (filters[filterInd].params.id) {
                        data.taskResponsible = filters[filterInd].params.id;
                    } else {
                        data.taskResponsible = filters[filterInd].params.value;
                    }
                    anchor = jq.changeParamValue(anchor, "user_tasks", data.taskResponsible);
                    break;
                case "followed":
                    data.follow = "true";
                    anchor = jq.changeParamValue(anchor, "followed", "true");
                    break;
                case "tag":
                    data.tag = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "tag", data.tag);
                    break;
                case "open":
                case "paused":
                case "closed":
                case "status":
                    data.status = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "status", data.status);
                    break;
                case "notChargeable":
                case "notBilled":
                case "billed":
                case "payment_status":
                    data.status = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "payment_status", data.status);
                    break;
                case "overdue":
                    data.status = "open";
                    data.deadlineStop = Teamlab.serializeTimestamp(new Date());
                    anchor = jq.changeParamValue(anchor, "overdue", "true");
                    break;
                case "today":
                case "upcoming":
                case "deadline":
                    data.deadlineStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.deadlineStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "deadlineStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "deadlineStop", filters[filterInd].params.to);
                    break;
                case "today2":
                case "recent":
                case "created":
                case "previousweek2":
                case "previousmonth2":
                case "period2":
                    data.createdStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.createdStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "createdStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "createdStop", filters[filterInd].params.to);
                    break;
                case "today3":
                case "yesterday":
                case "currentweek":
                case "previousweek":
                case "currentmonth":
                case "previousmonth":
                case "currentyear":
                case "previousyear":
                case "period":
                    data.periodStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from));
                    data.periodStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
                    anchor = jq.changeParamValue(anchor, "periodStart", filters[filterInd].params.from);
                    anchor = jq.changeParamValue(anchor, "periodStop", filters[filterInd].params.to);
                    break;
                case "text":
                    data.FilterValue = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "text", data.FilterValue);
                    break;
                case "project_entity":
                case "milestone_entity":
                case "discussion_entity":
                case "team_entity":
                case "task_entity":
                case "subtask_entity":
                case "timespend_entity":
                case "comment_entity":
                case "entity":
                    data.entity = filters[filterInd].params.value;
                    anchor = jq.changeParamValue(anchor, "entity", data.entity);
                    break;
                case "sorter":
                    data.sortBy = filters[filterInd].params.id;
                    data.sortOrder = filters[filterInd].params.sortOrder;
                    anchor = jq.changeParamValue(anchor, "sortBy", data.sortBy); ;
                    anchor = jq.changeParamValue(anchor, "sortOrder", data.sortOrder);
                    break;
            }
        }
        if (type == "anchor") {
            return anchor;
        } else {
            return data;
        }
    };

    var visibleFilterItem = function(type, filterId, visible) {
        jq("#ProjectsAdvansedFilter").advansedFilter({ filters: [{ type: type, id: filterId, visible: visible}] });
    };

    var initFilterComboboxItem = function(data, filterId) {
        jq("#ProjectsAdvansedFilter").advansedFilter({ filters: [{ type: "combobox", id: filterId, options: data}] });
    };

    var getProjectsForFilter = function() {
        var currentUserProjects = [];
        var otherProjects = [];

        var projects = ASC.Projects.Master.Projects;
        if (!projects) return [];
        var projectsCount = projects.length;
        for (var i = 0; i < projectsCount; i++) {
            var prj = { 'value': projects[i].id, 'title': projects[i].title, 'canCreateTask': projects[i].canCreateTask, 'canCreateMilestone': projects[i].canCreateMilestone };
            if (projects[i].isInTeam) {
                currentUserProjects.push(prj);
            } else {
                otherProjects.push(prj);
            }
        }
        if (currentUserProjects.length > 0) {
            currentUserProjects[currentUserProjects.length - 1]['classname'] = 'separator';
        }

        return currentUserProjects.concat(otherProjects);
    };

    var getMilestonesForFilter = function() {
        var milestones = ASC.Projects.Master.Milestones;
        if (!milestones) return [];
        var milestonesForFilter = [];
        milestonesForFilter.push({ 'value': 0, 'title': ASC.Projects.Resources.ProjectsJSResource.NoMilestone });

        for (var i = 0, n = milestones.length; i < n; i++) {
            milestonesForFilter.push({ 'value': milestones[i].id, 'title': milestones[i].displayDateDeadline + " " + milestones[i].title });
        }

        return milestonesForFilter;
    };

    var getTagsForFilter = function() {
        var tags = ASC.Projects.Master.Tags;
        if (!tags) return [];
        var filterTags = [];
        for (var i = 0, n = tags.length; i < n; i++) {
            filterTags.push({ 'value': tags[i].id, 'title': tags[i].title });
        }
        return filterTags;
    };

    var getTeamForFilter = function() {
        var team = ASC.Projects.Master.Team;
        var filterTeam = [];
        for (var i = 0, n = team.length; i < n; i++) {
            filterTeam.push({ 'value': team[i].id, 'title': team[i].displayName });
        }
        return filterTeam;
    };

    return {
        getUrlParam: getUrlParam,
        basePath: basePath,
        hashFilterChanged: hashFilterChanged,
        massNameFilters: massNameFilters,
        anchorMoving: anchorMoving,
        initialisation: initialisation,
        firstload: firstload,
        coincidesWithFilter: coincidesWithFilter,
        init: init,
        setFilterByUrl: setFilterByUrl,
        makeData: makeData,

        getProjectsForFilter: getProjectsForFilter,
        getTagsForFilter: getTagsForFilter,
        getMilestonesForFilter: getMilestonesForFilter,
        getTeamForFilter: getTeamForFilter,

        visibleFilterItem: visibleFilterItem,

        onSetFilter: function() {
        },
        onResetFilter: function() {
        }
    };
})(jQuery);
