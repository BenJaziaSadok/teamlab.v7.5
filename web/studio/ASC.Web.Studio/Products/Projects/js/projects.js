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

if (typeof ASC === "undefined")
    ASC = {};
if (typeof ASC.Projects === "undefined")
    ASC.Projects = {};

ASC.Projects.AllProject = (function() {
    var isInit = false;
    var overProjDescrPanel = false;
    var projDescribeTimeout = 0;
    var basePath = 'sortBy=create_on&sortOrder=ascending';
    var moduleLocationPath = StudioManager.getLocationPathToModule("projects");
    var linkViewProject = moduleLocationPath + 'tasks.aspx?prjID=';
    var linkViewMilestones = moduleLocationPath + 'milestones.aspx?prjID=';
    var linkViewTasks = moduleLocationPath + 'tasks.aspx?prjID=';
    var linkViewParticipants = moduleLocationPath + 'projectTeam.aspx?prjID=';

    var currentUserId;
    var currentFilter;

    var isSimpleView;
    var filterProjCount = 0;

    //pagination
    var entryCountOnPage;
    var pageCount;
    var cookiePaginationKey;
    var currentPage = 0;

    //filter Set

    var onSetFilterProjects = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor');
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
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data');
        currentFilter = filter;
        ASC.Projects.AllProject.showPreloader();
        getProjects({ mode: 'onset' }, filter);
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };


    // filter Reset
    var onResetFilterProjects = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor');
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data');
        currentFilter = filter;
        getProjects({ mode: 'onreset' }, filter);
        ASC.Projects.AllProject.showPreloader();
    };

    var createAdvansedFilter = function() {
        ASC.Projects.ProjectsAdvansedFilter.filter = jq('#ProjectsAdvansedFilter').advansedFilter(
            {
                store: true,
                anykey: true,
                colcount: 2,
                anykeytimeout: 1000,
                filters:
                    [
                // Team member
                        {
                        type: "person",
                        id: "me_team_member",
                        title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                        filtertitle: ASC.Projects.Resources.ProjectsFilterResource.TeamMember + ":",
                        group: ASC.Projects.Resources.ProjectsFilterResource.TeamMember,
                        hashmask: "person/{0}",
                        groupby: "userid",
                        bydefault: { id: currentUserId }
                    },
                        {
                            type: "person",
                            id: "team_member",
                            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.TeamMember + ":",
                            title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                            group: ASC.Projects.Resources.ProjectsFilterResource.TeamMember,
                            hashmask: "person/{0}",
                            groupby: "userid"
                        },
                // Project manager
                        {
                        type: "person",
                        id: "me_project_manager",
                        title: ASC.Projects.Resources.ProjectsFilterResource.Me,
                        filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ProjectMenager + ":",
                        group: ASC.Projects.Resources.ProjectsFilterResource.ProjectMenager,
                        hashmask: "person/{0}",
                        groupby: "managerid",
                        bydefault: { id: currentUserId }
                    },
                        {
                            type: "person",
                            id: "project_manager",
                            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ProjectMenager + ":",
                            title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                            group: ASC.Projects.Resources.ProjectsFilterResource.ProjectMenager,
                            hashmask: "person/{0}",
                            groupby: "managerid"
                        },
                //Status
                        {
                        type: "combobox",
                        id: "open",
                        title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenProject,
                        filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
                        group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
                        hashmask: "combobox/{0}",
                        groupby: "status",
                        options:
                                [
                                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenProject, def: true },
                                    { value: "paused", title: ASC.Projects.Resources.ProjectsFilterResource.StatusSuspend },
                                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedProject }
                                ]
                    },
                        {
                            type: "combobox",
                            id: "paused",
                            title: ASC.Projects.Resources.ProjectsFilterResource.StatusSuspend,
                            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
                            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
                            hashmask: "combobox/{0}",
                            groupby: "status",
                            options:
                                [
                                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenProject },
                                    { value: "paused", title: ASC.Projects.Resources.ProjectsFilterResource.StatusSuspend, def: true },
                                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedProject }
                                ]
                        },
                        {
                            type: "combobox",
                            id: "closed",
                            title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedProject,
                            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.ByStatus + ":",
                            group: ASC.Projects.Resources.ProjectsFilterResource.ByStatus,
                            hashmask: "combobox/{0}",
                            groupby: "status",
                            options:
                                [
                                    { value: "open", title: ASC.Projects.Resources.ProjectsFilterResource.StatusOpenProject },
                                    { value: "paused", title: ASC.Projects.Resources.ProjectsFilterResource.StatusSuspend },
                                    { value: "closed", title: ASC.Projects.Resources.ProjectsFilterResource.StatusClosedProject, def: true }
                                ]
                        },
                // Other
                        {
                        type: "flag",
                        id: "followed",
                        title: ASC.Projects.Resources.ProjectsFilterResource.FollowProjects,
                        group: ASC.Projects.Resources.ProjectsFilterResource.Other,
                        hashmask: "followed"
                    },
                        {
                            type: "combobox",
                            id: "tag",
                            title: ASC.Projects.Resources.ProjectsFilterResource.ByTag,
                            filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Tag + ":",
                            group: ASC.Projects.Resources.ProjectsFilterResource.Other,
                            hashmask: "combobox/{0}",
                            options: ASC.Projects.ProjectsAdvansedFilter.getTagsForFilter(),
                            defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
                        }
                    ],
                sorters:
                    [
                        { id: "title", title: ASC.Projects.Resources.ProjectsFilterResource.ByTitle, sortOrder: "ascending", def: true },
                        { id: "create_on", title: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate, sortOrder: "descending" }
                    ]
            }
        )
            .bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
            .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

        ASC.Projects.ProjectsAdvansedFilter.init = true;

        if (!isSimpleView) {
            //filter
            ASC.Projects.ProjectsAdvansedFilter.filter.one("adv-ready", function () {
                var projectAdvansedFilterContainer = jq("#ProjectsAdvansedFilter .advansed-filter-list");
                projectAdvansedFilterContainer.find("li[data-id='me_team_member'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'me_team_member');
                projectAdvansedFilterContainer.find("li[data-id='team_member'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'team_member');
                projectAdvansedFilterContainer.find("li[data-id='me_project_manager'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'me_project_manager');
                projectAdvansedFilterContainer.find("li[data-id='project_manager'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'project_manager');
                projectAdvansedFilterContainer.find("li[data-id='open'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'open');
                projectAdvansedFilterContainer.find("li[data-id='closed'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'closed');
                projectAdvansedFilterContainer.find("li[data-id='paused'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'paused');
                projectAdvansedFilterContainer.find("li[data-id='followed'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'followed');
                projectAdvansedFilterContainer.find("li[data-id='tag'] .inner-text").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'tag');
                jq("#ProjectsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.projects, ga_Actions.filterClick, 'sort');
                jq("#ProjectsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.projects, ga_Actions.filterClick, "search_text", "enter");
            });
        }
    };

    var initPageNavigatorControl = function () {
        window.projListPgNavigator = new ASC.Controls.PageNavigator.init("projListPgNavigator", "#divForTaskPager", entryCountOnPage, pageCount, 1,
                                                                       ASC.Projects.Resources.ProjectsJSResource.PreviousPage, ASC.Projects.Resources.ProjectsJSResource.NextPage);
        projListPgNavigator.NavigatorParent = '#divForTaskPager';
        projListPgNavigator.changePageCallback = function(page) {
            currentPage = page - 1;
            LoadingBanner.displayLoading();
            getProjects({ mode: 'next' }, currentFilter);
        };
    };

    var updatePageNavigatorControl = function() {
        jq("#totalCount").text(filterProjCount);
        projListPgNavigator.drawPageNavigator(currentPage + 1, filterProjCount);

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
        projListPgNavigator.EntryCountOnPage = newCountOfRows;

        LoadingBanner.displayLoading();
        getProjects({ mode: 'onreset' }, currentFilter);
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
    var renderListProjects = function(listProjects) {
        onGetListProject({}, listProjects);
    };

    var addProjectsToSimpleList = function(projectItem) {
        $projectsTable = jq('#tableListProjects');
        $projectsTableBody = $projectsTable.find('tbody');

        clearTimeout(projDescribeTimeout);
        overProjDescrPanel = false;
        hideDescrPanel();

        projectItem = getProjTmpl(projectItem);
        jq.tmpl("projects_projectTmpl", projectItem).prependTo($projectsTableBody);
        $projectsTable.show();
    };

    var init = function(isSimpleViewFlag, countOfPage, cookieKey, entryPageCount) {
        if (isInit === false) {
            isInit = true;
        }
        isSimpleView = isSimpleViewFlag;
        currentUserId = Teamlab.profile.id;

        jq("body").append(jq.tmpl("projects_projectDescribePanelTmpl", {}));

        Teamlab.bind(Teamlab.events.getPrjProjects, onGetListProject);

        if (!isSimpleView) {
            showPreloader();
            //page navigator
            entryCountOnPage = countOfPage;
            cookiePaginationKey = cookieKey;
            pageCount = entryPageCount;
            
            if (jq("#tableListProjects").length) {
                jq("#countOfRows").val(entryCountOnPage).tlCombobox();
                initPageNavigatorControl();
            }
            // filter
            ASC.Projects.ProjectsAdvansedFilter.initialisation(currentUserId, basePath);
            ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterProjects;
            ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterProjects;

            // waiting data from api
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTags, function() {
                createAdvansedFilter();
            });
            //
        }
        if (!isSimpleView) {
            jq('#tableListProjects').on('click', "td.responsible span.userLink", function() {
                var responsibleId = jq(this).attr('id');
                if (responsibleId != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
                    var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project_manager', responsibleId);
                    path = jq.removeParam('team_member', path);
                    ASC.Controls.AnchorController.move(path);
                }
            });

            jq('#emptyScreenForFilter .clearFilterButton').click(function() {
                ASC.Controls.AnchorController.move(basePath);
                jq('#ProjectsAdvansedFilter').advansedFilter(null);
                return false;
            });
            // popup handlers
            jq('#questionWindowMilestone .button.gray.middle, #questionWindowTasks .button.gray.middle').bind('click', function() {
                jq.unblockUI();
                return false;
            });
        }
        // discribe panel
        jq("#tableListProjects").on("mouseenter", ".nameProject a", function(event) {
            projDescribeTimeout = setTimeout(function() {
                var targetObject = event.target;

                var describePanel = jq('#projectDescrPanel');
                var describe = describePanel.find('.descr');
                var readMore = describe.find('.readMore');
                var created = describePanel.find('.created');

                jq(created, describe, readMore).hide();

                var createdAttr = jq(targetObject).attr('created');

                if (typeof createdAttr != 'undefined' && jq.trim(createdAttr) != "") {
                    created.find('.value').html(createdAttr);
                    created.show();
                } else {
                    created.hide();
                }
                var description = jq(targetObject).siblings('.description').text();
                if (jq.trim(description) != '') {
                    describe.show();
                    describe.find('.value div').html(jq.linksParser(description.replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>').replace('&amp;', '&')));
                    if (description.indexOf("\n") > 2 || description.length > 80) {
                        var link = "projects.aspx?prjID=" + jq(targetObject).attr('projectid');
                        readMore.attr("href", link).show();
                    } else {
                        readMore.hide();
                    }
                } else {
                    describe.hide();
                }
                if (jq(targetObject).attr('created').length || jq.trim(description) != '') {
                    showProjDescribePanel(targetObject);
                    overProjDescrPanel = true;
                }
            }, 500, this);
        });
        jq('#tableListProjects').on('mouseleave', '.nameProject a', function() {
            clearTimeout(projDescribeTimeout);
            overProjDescrPanel = false;
            hideDescrPanel();
        });

        jq('#projectDescrPanel').on('mouseenter', function() {
            overProjDescrPanel = true;
        });

        jq('#projectDescrPanel').on('mouseleave', function() {
            overProjDescrPanel = false;
            hideDescrPanel();
        });

        /*--------events--------*/

        jq("#countOfRows").change(function(evt) {
            changeCountOfRows(this.value);
        });

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            var $elt = jq(elt);

            if ($elt.is('#containerStatusList')) isHide = false;
            if (isHide) {
                jq('#containerStatusList').hide();
                jq('.statusContainer').removeClass('openList');
            }
        });

        jq(document).on('click', 'td.action .canEdit', function(event) {
            showListStatus('containerStatusList', this);
            return false;
        });

        // ga-track-events
        if (!isSimpleView) {           
                //change status
                jq("#statusList .open").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "open");
                jq("#statusList .closed").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "closed");
                jq("#statusList .paused").trackEvent(ga_Categories.projects, ga_Actions.changeStatus, "paused");

                //PM
                jq(".responsible .userLink").trackEvent(ga_Categories.projects, ga_Actions.userClick, "project-manager");
            
            //end ga-track-events
        }
    };

    var hideDescrPanel = function() {
        setTimeout(function() {
            if (!overProjDescrPanel) jq('#projectDescrPanel').hide(100);
        }, 200);
    };

    var getProjTmpl = function(proj) {
        var projTmpl = {};
        projTmpl.title = proj.title;
        projTmpl.id = proj.id;
        projTmpl.created = proj.displayDateCrtdate;
        projTmpl.createdBy = proj.createdBy ? proj.createdBy.displayName : "";
        projTmpl.projLink = linkViewProject + projTmpl.id;
        projTmpl.description = proj.description;
        projTmpl.milestones = proj.milestoneCount;
        projTmpl.linkMilest = linkViewMilestones + projTmpl.id + '#sortBy=deadline&sortOrder=ascending&status=open';
        projTmpl.tasks = proj.taskCount;
        projTmpl.linkTasks = linkViewTasks + projTmpl.id + '#sortBy=deadline&sortOrder=ascending&status=open';
        projTmpl.responsible = proj.responsible.displayName;
        projTmpl.responsibleId = proj.responsible.id;
        projTmpl.participants = proj.participantCount ? proj.participantCount - 1 : "";
        projTmpl.linkParticip = linkViewParticipants + projTmpl.id;
        projTmpl.privateProj = proj.isPrivate;
        projTmpl.canEdit = proj.canEdit;
        projTmpl.isSimpleView = isSimpleView;
        projTmpl.canLinkContact = proj.canLinkContact;

        if (proj.status == 0) {
            projTmpl.status = 'open';
        }
        else {
            projTmpl.status = 'closed';
        }
        if (proj.status == 2) projTmpl.status = 'paused';

        return projTmpl;
    };

    var showPreloader = function() {
        LoadingBanner.displayLoading();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };

    var showProjDescribePanel = function(targetObject) {
        var x = jq(targetObject).offset().left + 10;
        var y = jq(targetObject).offset().top + 20;
        jq('#projectDescrPanel').css({ left: x, top: y });
        jq('#projectDescrPanel').show();

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="#projectDescrPanel"]')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="#projectDescrPanel"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('.studio-action-panel').hide();
            }
        });
    };

    var getProjects = function(params, filter) {
        filter.Count = entryCountOnPage;
        filter.StartIndex = entryCountOnPage * currentPage;

        if (filter.StartIndex > filterProjCount) {
            filter.StartIndex = 0;
            currentPage = 1;
        }

        Teamlab.getPrjProjects(params, { filter: filter });
    };

    var onGetListProject = function(params, listProj) {
        $projectsTable = jq('#tableListProjects');
        $projectsTableBody = $projectsTable.find('tbody');
        if (typeof (isSimpleView) != "undefined" && isSimpleView == false) {
            filterProjCount = params.__total != undefined ? params.__total : 0;
            updatePageNavigatorControl();
        }

        clearTimeout(projDescribeTimeout);
        overProjDescrPanel = false;
        hideDescrPanel();

        var listTmplProj = new Array(),
            projTmpl;

        $projectsTableBody.empty();

        if (listProj.length != 0) {
            for (var i = 0; i < listProj.length; i++) {
                projTmpl = getProjTmpl(listProj[i]);
                listTmplProj.push(projTmpl);
            }
            $projectsTableBody.append(jq.tmpl("projects_projectTmpl", listTmplProj));

            jq("#emptyScreenForFilter").hide();
            $projectsTable.show();
        }
        else {
            jq('#tableForNavigation').hide();
            $projectsTable.hide();
            jq("#emptyScreenForFilter").show();
        }
        hidePreloader();
    };

    var changeStatus = function(item) {
        if (!jq(item).hasClass('current')) {
            var projId = jq(item).parents('#containerStatusList').attr('objid').split('_')[1];
            var newStatus = jq(item).attr('class').split(" ")[0];
            if (newStatus == 'closed') {
                var flag = showQuestionWindow(projId);
                if (flag) return;
            }
            var newtitle = jq(item).text().trim();
            var data = { id: projId, status: newStatus };
            Teamlab.updatePrjProjectStatus({}, projId, data);

            changeCboxStatus(newStatus, projId, newtitle);
        }
    };

    var changeCboxStatus = function(status, projId, title) {
        jq('#statusCombobox_' + projId + ' span:first-child').attr('class', status);
        jq('#statusCombobox_' + projId + ' span:first-child').attr('title', title);
        if (status != 'open') {
            jq('tr#' + projId).addClass('noActiveProj');
        } else {
            jq('tr#' + projId).removeClass('noActiveProj');
        }
    };

    var showQuestionWindow = function(projId) {
        var proj = jq('tr#' + projId);
        var tasks = jq.trim(jq(proj).find('td.taskCount').text());
        var popupId;
        if (!tasks.length) {
            var milestones = jq.trim(jq(proj).find('td.milestoneCount').text());
            if (milestones.length) {
                popupId = '#questionWindowMilestone';
                var milUrl = linkViewMilestones + projId + '#sortBy=deadline&sortOrder=ascending&status=open';
                jq('#linkToMilestines').attr('href', milUrl);
            }
            else {
                return false;
            }
        } else {
            popupId = '#questionWindowTasks';
            var tasksUrl = linkViewTasks + projId + '#sortBy=deadline&sortOrder=ascending&status=open';
            jq('#linkToTasks').attr('href', tasksUrl);
        }

        var margintop = jq(window).scrollTop();
        margintop = margintop + 'px';
        jq.blockUI({ message: jq(popupId),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '400px',

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-200px',
                'margin-top': margintop,
                'background-color': 'White'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0
        });
        return true;
    };


    var showListStatus = function(panelId, obj, event) {
        var objid = '';
        var x, y;
        objid = jq(obj).attr('id');

        x = jq(obj).offset().left + 9;
        y = jq(obj).offset().top + 25;

        jq('#containerStatusList').attr('objid', objid);
        jq(obj).parents('tr').addClass('openList');

        jq('#containerStatusList').css({ left: x, top: y });

        var status = jq(obj).children('span').attr('class');
        jq('#containerStatusList #statusList li').show();
        jq('#containerStatusList #statusList li').removeClass('current');
        switch (status) {
            case 'closed':
                {
                    jq('#containerStatusList #statusList .closed').addClass('current');
                    jq('#containerStatusList #statusList .paused').hide();
                    break;
                }

            case 'paused':
                {
                    jq('#containerStatusList #statusList .paused').addClass('current');
                    break;
                }

            default:
                {
                    jq('#containerStatusList #statusList .open').addClass('current');
                    break;
                }
        }

        jq('#containerStatusList').show();

        jq('body').click(function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            var isHide = true;
            if (jq(elt).is('[id="containerStatusList"]')) {
                isHide = false;
            }

            if (isHide)
                jq(elt).parents().each(function() {
                    if (jq(this).is('[id="containerStatusList"]')) {
                        isHide = false;
                        return false;
                    }
                });

            if (isHide) {
                jq('#containerStatusList').hide();
                jq('#tableListProjects tr').removeClass('openList');
            }
        });
    };

    return {
        init: init,
        changeStatus: changeStatus,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader,
        changeCountOfRows: changeCountOfRows,
        renderListProjects: renderListProjects,
        addProjectsToSimpleList: addProjectsToSimpleList
    };
})(jQuery);

