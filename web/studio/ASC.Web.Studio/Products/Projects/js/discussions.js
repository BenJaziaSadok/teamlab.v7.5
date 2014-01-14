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

ASC.Projects.Discussions = (function() {
    var isInit = false;

    var myGuid;
    var currentProjectId;

    var allDiscCount;
    var filterDiscCount = 0;

    var basePath = 'sortBy=create_on&sortOrder=descending';

    var loadListProjectsFlag = false;
    var loadListTagsFlag = false;
    var loadTeamFlag = false;

    var advansedFilter;
    var showAdvansedFilter = function() {
        advansedFilter.css('visibility', 'visible');
    };
    var hideAdvansedFilter = function() {
        advansedFilter.css('visibility', 'hidden');
    };

    var discussionsList;

    //pagination
    var entryCountOnPage;
    var pageCount;
    var cookiePaginationKey;
    var currentPage = 0;

    var currentFilter = {};

    var setCurrentFilter = function(filter) {
        currentFilter = filter;
    };

    var showPreloader = function() {
        LoadingBanner.displayLoading();
    };

    var hidePreloader = function() {
        LoadingBanner.hideLoading();
    };

    //filter Set
    var onSetFilterDiscussions = function(evt, $container) {
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
        ASC.Projects.Discussions.setCurrentFilter(filter);
        ASC.Projects.Discussions.getDiscussions(filter);
        ASC.Projects.Discussions.showPreloader();
        if (path !== hash) {
            ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
            location.hash = path;
        }
    };

    //filter Reset
    var onResetFilterDiscussions = function(evt, $container) {
        currentPage = 0;
        var path = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'anchor', currentProjectId);
        ASC.Projects.ProjectsAdvansedFilter.hashFilterChanged = true;
        ASC.Controls.AnchorController.move(path);
        var filter = ASC.Projects.ProjectsAdvansedFilter.makeData($container, 'data', currentProjectId);
        ASC.Projects.Discussions.setCurrentFilter(filter);
        ASC.Projects.Discussions.getDiscussions(filter);
        ASC.Projects.Discussions.showPreloader();
    };

    var createAdvansedFilter = function() {
        var now = new Date();
        var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        var lastWeek = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
        lastWeek.setDate(lastWeek.getDate() - 7);

        var filters = [];
        //Author

        if (currentProjectId) {
            if (ASC.Projects.Common.userInProjectTeam(Teamlab.profile.id, ASC.Projects.Master.Team)) {
                filters.push({
                    type: "combobox",
                    id: "me_author",
                    title: ASC.Projects.Resources.ProjectsFilterResource.MyDiscussions,
                    filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Author + ":",
                    group: ASC.Projects.Resources.ProjectsFilterResource.Author,
                    hashmask: "person/{0}",
                    groupby: "userid",
                    options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                    bydefault: { value: Teamlab.profile.id }
                });
            }
            filters.push({
                type: "combobox",
                id: "author",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherParticipant,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Author + ":",
                hashmask: "person/{0}",
                groupby: "userid",
                group: ASC.Projects.Resources.ProjectsFilterResource.ByParticipant,
                options: ASC.Projects.ProjectsAdvansedFilter.getTeamForFilter(),
                defaulttitle: ASC.Projects.Resources.ProjectsFilterResource.Select
            });
        } else {
            filters.push({
                type: "person",
                id: "me_author",
                title: ASC.Projects.Resources.ProjectsFilterResource.MyDiscussions,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Author + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.Author,
                hashmask: "person/{0}",
                groupby: "userid",
                bydefault: { id: Teamlab.profile.id }
            });
            filters.push({
                type: "person",
                id: "author",
                title: ASC.Projects.Resources.ProjectsFilterResource.OtherUsers,
                filtertitle: ASC.Projects.Resources.ProjectsFilterResource.Author + ":",
                group: ASC.Projects.Resources.ProjectsFilterResource.Author,
                hashmask: "person/{0}",
                groupby: "userid"
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
        //Creation date
        filters.push({
            type: "daterange",
            id: "today2",
            title: ASC.Projects.Resources.ProjectsFilterResource.Today,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate,
            hashmask: "created/{0}/{1}",
            groupby: "created",
            bydefault: { from: today.getTime(), to: today.getTime() }
        });
        filters.push({
            type: "daterange",
            id: "recent",
            title: ASC.Projects.Resources.ProjectsFilterResource.Recent,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate,
            hashmask: "created/{0}/{1}",
            groupby: "created",
            bydefault: { from: lastWeek.getTime(), to: today.getTime() }
        });
        filters.push({
            type: "daterange",
            id: "created",
            title: ASC.Projects.Resources.ProjectsFilterResource.CustomPeriod,
            filtertitle: " ",
            group: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate,
            hashmask: "created/{0}/{1}",
            groupby: "created"
        });
        //Followed
        filters.push({
            type: "flag",
            id: "followed",
            title: ASC.Projects.Resources.ProjectsFilterResource.FollowDiscussions,
            group: ASC.Projects.Resources.ProjectsFilterResource.Other,
            hashmask: "followed"
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
                        { id: "comments", title: ASC.Projects.Resources.ProjectsFilterResource.ByComments, sortOrder: "descending", def: true },
                        { id: "create_on", title: ASC.Projects.Resources.ProjectsFilterResource.ByCreateDate, sortOrder: "descending" },
                        { id: "title", title: ASC.Projects.Resources.ProjectsFilterResource.ByTitle, sortOrder: "ascending" }

                    ]
            }
        ).bind('setfilter', ASC.Projects.ProjectsAdvansedFilter.onSetFilter)
         .bind('resetfilter', ASC.Projects.ProjectsAdvansedFilter.onResetFilter);

        ASC.Projects.ProjectsAdvansedFilter.init = true;

        // ga-track-events

        //filter
        ASC.Projects.ProjectsAdvansedFilter.filter.one("adv-ready", function () {
            var projectsAdvansedFilter = jq("#ProjectsAdvansedFilter .advansed-filter-list");
            projectsAdvansedFilter.find("li[data-id='me_author'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'me-author');
            projectsAdvansedFilter.find("li[data-id='author'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'author');

            projectsAdvansedFilter.find("li[data-id='myprojects'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'myprojects');
            projectsAdvansedFilter.find("li[data-id='project'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'project');
            projectsAdvansedFilter.find("li[data-id='tag'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'tag');

            projectsAdvansedFilter.find("li[data-id='followed'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'followed');

            projectsAdvansedFilter.find("li[data-id='today2'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'today');
            projectsAdvansedFilter.find("li[data-id='recent'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'recent-7-days');
            projectsAdvansedFilter.find("li[data-id='created'] .inner-text").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'user-period');

            jq("#ProjectsAdvansedFilter .btn-toggle-sorter").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, 'sort');
            jq("#ProjectsAdvansedFilter .advansed-filter-input").trackEvent(ga_Categories.discussions, ga_Actions.filterClick, "search_text", "enter");
            //end ga-track-events
        })
    };

    var initPageNavigatorControl = function () {
        window.discListPgNavigator = new ASC.Controls.PageNavigator.init("discListPgNavigator", "#divForTaskPager", entryCountOnPage, pageCount, 1,
                ASC.Projects.Resources.ProjectsJSResource.PreviousPage, ASC.Projects.Resources.ProjectsJSResource.NextPage);
        discListPgNavigator.NavigatorParent = '#divForTaskPager';
        discListPgNavigator.changePageCallback = function(page) {
            currentPage = page - 1;
            LoadingBanner.displayLoading();
            getDiscussions(currentFilter, true);
        };
    };

    var updatePageNavigatorControl = function() {
        jq("#totalCount").text(filterDiscCount);
        discListPgNavigator.drawPageNavigator(currentPage + 1, filterDiscCount);

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
        discListPgNavigator.EntryCountOnPage = newCountOfRows;

        LoadingBanner.displayLoading();
        getDiscussions(currentFilter, false);
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

    var resize = function() {
        var windowWidth = jq(window).width() - 24*2,
            mainBlockWidth = parseInt(jq(".mainPageLayout").css("min-width")),
            newWidth = (windowWidth < mainBlockWidth) ? mainBlockWidth : windowWidth;
       
        jq("#discussionsList .describe-list").each(
                function() {
                    jq(this).css("max-width", (newWidth - 24 * 2 - 120 - jq(".mainPageTableSidePanel").width()) + "px");
                }
            );
    };

    var init = function (countOfPage, cookieKey, entryPageCount) {
        if (isInit === false) {
            isInit = true;
        }

        var discCount = jq("#discussionsList").attr("data-discussions-count");
        if (!discCount) discCount = 0;
        allDiscCount = parseInt(discCount);

        myGuid = Teamlab.profile.id;
        if (allDiscCount != 0) {
            //page navigator
            entryCountOnPage = countOfPage;
            pageCount = entryPageCount;
            cookiePaginationKey = cookieKey;
            jq("#countOfRows").val(entryCountOnPage).tlCombobox();
            initPageNavigatorControl();
        } else {
            jq('#emptyListDiscussion').show();
            return;
        }

        ASC.Projects.ProjectsAdvansedFilter.initialisation(myGuid, basePath);
        ASC.Projects.ProjectsAdvansedFilter.onSetFilter = onSetFilterDiscussions;
        ASC.Projects.ProjectsAdvansedFilter.onResetFilter = onResetFilterDiscussions;

        jq("#emptyScreenForFilter").on("click", ".clearFilterButton", function() {
            jq('#ProjectsAdvansedFilter').advansedFilter(null);
            return false;
        });

        advansedFilter = jq('#ProjectsAdvansedFilter');
        discussionsList = jq('#discussionsList');

        currentProjectId = jq.getURLParam('prjID');

        if (allDiscCount != 0) {
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
        }

        jq("#discussionsList").on("click", ".name-list.project", function() {
            var projectId = jq(this).attr('data-projectId');
            var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'project', projectId);
            path = jq.removeParam('tag', path);
            ASC.Controls.AnchorController.move(path);
        });

        jq("#discussionsList").on("click", ".name-list.author", function() {
            var authorId = jq(this).attr('data-authorId');
            if (authorId != "4a515a15-d4d6-4b8e-828e-e0586f18f3a3") {
                var path = jq.changeParamValue(ASC.Controls.AnchorController.getAnchor(), 'author', authorId);
                ASC.Controls.AnchorController.move(path);
            }
        });

        jq(window).resize(function() {
            resize();
        });

        jq("#countOfRows").change(function(evt) {
            changeCountOfRows(this.value);
        });

    };

    var getDiscussions = function(filter) {
        filter.Count = entryCountOnPage;
        filter.StartIndex = entryCountOnPage * currentPage;

        if (filter.StartIndex > filterDiscCount) {
            filter.StartIndex = 0;
            currentPage = 1;
        }

        Teamlab.getPrjDiscussions({}, { filter: filter, success: onGetDiscussions });
    };

    var showOrHideEmptyScreen = function(discussionCount) {

        if (discussionCount) {
            jq(".noContentBlock").hide();
            showAdvansedFilter();
            return;
        }
        if (allDiscCount == 0) {
            hideAdvansedFilter();
            jq('#emptyScreenForFilter').hide();
            jq('#emptyListDiscussion').show();

        } else {
            jq("#tableForNavigation").hide();
            if (filterDiscCount == 0) {
                jq('#emptyListDiscussion').hide();
                showAdvansedFilter();
                jq('#emptyScreenForFilter').show();
            }
        }
    };

    var onGetDiscussions = function(params, discussions) {
        filterDiscCount = params.__total != undefined ? params.__total : 0;
        updatePageNavigatorControl();

        hidePreloader();
        discussionsList.empty();

        var discussionCount = discussions.length;
        showOrHideEmptyScreen(discussionCount);
        if (discussionCount) {
            showDiscussions(discussions);
        } else {

        }
    };

    var showDiscussions = function(discussions) {
        var templates = [];
        for (var i = 0; i < discussions.length; i++) {
            var discussion = discussions[i];
            var template = getDiscussionTemplate(discussion);
            templates.push(template);
        }

        discussionsList.empty();

        for (var j = 0; j < templates.length; j++) {
            try {
                jq.tmpl("projects_discussionTemplate", templates[j]).appendTo(discussionsList);
            } catch (e) {

            }
        }

        resize();
        discussionsList.show();

        jq('.content-list p').filter(function(index) { return jq(this).html() == "&nbsp;"; }).remove();

    };

    var getDiscussionTemplate = function(discussion) {
        var discussionId = discussion.id;
        var prjId = discussion.projectId;

        var template =
        {
            createdDate: discussion.displayDateCrtdate,
            createdTime: discussion.displayTimeCrtdate,
            title: discussion.title,
            discussionUrl: getDiscussionUrl(prjId, discussionId),
            authorAvatar: discussion.createdBy.avatar,
            authorId: discussion.createdBy.id,
            authorName: discussion.createdBy.displayName,
            authorPost: discussion.createdBy.title,
            projectId: prjId,
            text: discussion.text,
            hasPreview: discussion.text.search('class="asccut"') > 0,
            commentsCount: discussion.commentsCount,
            commentsUrl: getCommentsUrl(prjId, discussionId),
            writeCommentUrl: getWriteCommentUrl(prjId, discussionId)
        };
        if (!currentProjectId) {
            template.projectTitle = discussion.projectTitle;
            template.projectUrl = getProjectUrl(discussion.projectId);
        }
        return template;
    };

    var getDiscussionUrl = function(prjId, discussionId) {
        return 'messages.aspx?prjID=' + prjId + '&id=' + discussionId;
    };

    var getProjectUrl = function(prjId) {
        return 'projects.aspx?prjID=' + prjId;
    };

    var getCommentsUrl = function(prjId, discussionId) {
        return 'messages.aspx?prjID=' + prjId + '&id=' + discussionId + '#comments';
    };

    var getWriteCommentUrl = function(prjId, discussionId) {
        return 'messages.aspx?prjID=' + prjId + '&id=' + discussionId + '#addcomment';
    };

    return {
        init: init,
        setCurrentFilter: setCurrentFilter,
        getDiscussions: getDiscussions,
        showPreloader: showPreloader,
        hidePreloader: hidePreloader,
        changeCountOfRows: changeCountOfRows
    };
})();

ASC.Projects.DiscussionDetails = (function () {
    var projectFolderId, projectName, fileContainer, privateFlag, projectTeam;
    var discussionId = jq.getURLParam("id");
    var projId = jq.getURLParam("prjID");
    var currentUserId;
    var participantsCount;
    var isCommentEdit = false;
    var subscribeButton = jq('#changeSubscribeButton');

    var init = function () {
        currentUserId = Teamlab.profile.id;
        participantsCount = jq("#discussionParticipantsTable .discussionParticipant[id!=currentLink]").length;
        privateFlag = jq("#discussionParticipantsContainer").attr("data-private") === "True" ? true : false;

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function () {
            projectTeam = ASC.Projects.Master.Team;
        });

        fileContainer = jq("#discussionFilesContainer");
        if (fileContainer.length && window.Attachments) {
            initAttachmentsControl();
        }

        if (jq("#discussionParticipantsContainer [guid=" + currentUserId + "][id!=currentLink]").length) {
            jq("#currentLink").remove();
        }

        var commentsCount = jq("#mainContainer div[id^=container_] div[id^=comment_] table").length;

        if (participantsCount > 0) {
            jq("#switcherDiscussionParticipants").show();
        }
        jq("#add_comment_btn").wrap("<span class='addcomment-button icon-link plus'></span>");
        if (commentsCount > 0) {
            jq("#hideCommentsButton").show();
            jq("#commentsContainer").css("marginTop", "15px");
        } else {
            jq("#commentsContainer").css("marginTop", "-6px");
        }
        jq("#commentsContainer").show();

        if (jq('#discussionActions .dropdown-content li a').length == 0 && discussionId != null) {
            jq('.menu-small').addClass("visibility-hidden");
        }

        var hash = ASC.Controls.AnchorController.getAnchor();
        if (hash == "addcomment" && CommentsManagerObj) {
            ASC.Projects.Common.showCommentBox();
        }

        jq("#manageParticipantsButton, .manage-participants-button .dottedLink").click(function () {
            jq("#discussionActions").hide();
            jq(".project-title .menu-small").removeClass("active");
            jq("#discussionParticipantsTable span.userLink").each(function () {
                var userId = jq(this).closest("tr").not(".hidden").attr("guid");
                discussionParticipantsSelector.SelectUser(userId);
            });

            discussionParticipantsSelector.IsFirstVisit = true;
            discussionParticipantsSelector.ShowDialog();
        });

        if (window.discussionParticipantsSelector) {
            discussionParticipantsSelector.OnOkButtonClick = function () {
                var participants = [];
                var participantsIds = [];

                var users = discussionParticipantsSelector.GetSelectedUsers();

                var isSubscribed = subscribeButton.attr("subscribed") === "1";
                for (var i = 0; i < users.length; i++) {
                    var userId = users[i].ID;
                    if (userId == currentUserId && isSubscribed) {
                        Teamlab.subscribeToPrjDiscussion({}, discussionId, { success: onChangeSubscribe });
                        continue;
                    }
                    var userName = users[i].Name;
                    var userDepartment = users[i].Group.Name;
                    var userTitle = users[i].Title;

                    participants.push({ id: userId, displayName: userName, department: userDepartment, title: userTitle, descriptionFlag: true });
                    participantsIds.push(userId);
                }

                var data = {};
                data.projectId = jq.getURLParam("prjID");
                data.participants = participantsIds.join(",");
                data.notify = false;

                Teamlab.updatePrjDiscussion({ participants: participants }, discussionId, data, {
                    before: function () { LoadingBanner.displayLoading(); },
                    success: onChangeDiscussionParticipants,
                    error: onDiscussionError,
                    after: function () { LoadingBanner.hideLoading(); }
                });
            };
        }
        subscribeButton.click(function () {
            Teamlab.subscribeToPrjDiscussion({}, discussionId, { success: onChangeSubscribe, error: onDiscussionError });
        });

        jq.switcherAction("#switcherDiscussionParticipants", "#discussionParticipantsContainer");
        jq.switcherAction("#switcherFilesButton", "#discussionFilesContainer");
        jq.switcherAction("#switcherCommentsButton", "#discussionCommentsContainer");

        jq("#createTaskOnDiscussion").click(function () {
            jq("body").click();
            LoadingBanner.displayLoading();
            Teamlab.addPrjTaskByMessage({}, projId, discussionId, {
                success: function (params, task) {
                    location.href = "tasks.aspx?prjID=" + projId + "&ID=" + task.id;
                }
            });
        });

        jq("#discussionParticipantsTable").on("mouseenter", ".discussionParticipant.gray", function () {
            jq(this).helper({
                BlockHelperID: "hintSubscribersPrivateProject",
                addLeft: 45,
                addTop: 12
            });
        });

        jq("#discussionParticipantsTable").on("mouseleave", ".discussionParticipant.gray", function () {
            jq("#hintSubscribersPrivateProject").hide();
        });

        jq("#addFirstCommentButton").click(function () {
            jq("#commentsContainer").show();
            jq("#add_comment_btn").click();
        });

        jq("#deleteDiscussionButton").click(function () {
            jq("#discussionActions").hide();
            jq(".project-title .menu-small").removeClass("active");
            showQuestionWindow();
        });

        jq("#questionWindow .remove").bind("click", function () {
            deleteDiscussion();
            return false;
        });
        jq("#questionWindow .cancel").bind("click", function () {
            jq.unblockUI();
            return false;
        });

        jq(document).on("click", "#btnCancel, #cancel_comment_btn", function () {
            isCommentEdit = false;
        });
        jq(document).on("click", "a[id^=edit_]", function () {
            isCommentEdit = true;
        });
        jq("#btnAddComment").click(function () {
            if (!isCommentEdit) {
                var text = jq("iframe[id^=CommentsFckEditor]").contents().find("iframe").contents().find("#fckbodycontent").text();
                if (jq.trim(text) != "") {
                    commentsCount = jq("#mainContainer div[id^=container_] div[id^=comment_] table").length;
                    updateTabTitle("comments", commentsCount + 1);
                    jq("#hideCommentsButton").show();
                    jq("#commentsContainer").css("marginTop", "15px");
                }
            } else {
                isCommentEdit = false;
            }
        });
    };

    var initAttachmentsControl = function () {

        projectFolderId = parseInt(jq("#discussionFilesContainer").attr("data-projectfolderid"));
        projectName = Encoder.htmlEncode(jq("#discussionFilesContainer").attr("data-projectName").trim());

        var canEditFiles = fileContainer.attr("data-canEdit") === "True" ? true : false;

        if (!canEditFiles) {
            Attachments.banOnEditing();
        }

        ProjectDocumentsPopup.init(projectFolderId, projectName);
        Attachments.init();
        Attachments.setFolderId(projectFolderId);
        Attachments.loadFiles();

        var id = jq.getURLParam("id");

        var filesCount = parseInt(jq('#discussionTabs div[container=discussionFilesContainer]').attr('count'));
        if (filesCount > 0) {
            jq("#hideFilesButton").show();
            jq("#discussionFilesContainer").css("marginBottom", "18px");
        }

        Attachments.bind("addFile", function (ev, file) {
            if (file.attachFromPrjDocFlag || file.isNewFile) {
                Teamlab.addPrjEntityFiles(null, id, "message", [file.id], { error: onDiscussionError });
            }
            filesCount++;
            updateTabTitle('files', filesCount);
            jq("#hideFilesButton").show();
            jq("#discussionFilesContainer").css("marginBottom", "18px");
        });

        Attachments.bind("deleteFile", function (ev, fileId) {
            Teamlab.removePrjEntityFiles({}, id, "message", fileId, { error: onDiscussionError });
            Attachments.deleteFileFromLayout(fileId);
            filesCount--;
            updateTabTitle('files', filesCount);
            if (filesCount == 0) {
                jq("#hideFilesButton").hide();
                jq("#discussionFilesContainer").css("marginBottom", "0px");
            }
        });
    };
    var removeComment = function () {
        var commentsCount = jq("#mainContainer div[id^=container_] div[id^=comment_] table").length;
        updateTabTitle("comments", commentsCount);
        if (commentsCount == 0) {
            jq("#mainContainer").empty();
            jq("#mainContainer").hide();
            jq("#commentsTitle").empty();
            jq("#hideCommentsButton").hide();
            jq("#commentsContainer").css("marginTop", "-6px");
        }
    };

    var onDiscussionError = function () {
        window.location.reload();
    };
    var onDeleteDiscussionError = function () {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
    };

    var onChangeDiscussionParticipants = function (params, discussion) {
        jq("#discussionParticipantsContainer .discussionParticipant").not(".hidden").remove();

        showDiscussionParticipants(params.participants);

        participantsCount = jq("#discussionParticipantsTable .discussionParticipant").not(".hidden").length;
        updateTabTitle("participants", participantsCount);
        if (participantsCount == 0) {
            jq(".manage-participants-button").show();
            jq("#switcherDiscussionParticipants").hide();
        } else {
            jq("#discussionParticipantsContainer").show();
            jq("#switcherDiscussionParticipants").show();
        }
    };

    var showDiscussionParticipants = function (participants) {
        var newListParticipants = [];
        var notSeePartisipant = [];
        if (privateFlag) {
            for (var i = 0; i < participants.length; i++) {
                var addedFlag = false;
                for (var j = 0; j < projectTeam.length; j++) {
                    if ((participants[i].id == projectTeam[j].id) && projectTeam[j].canReadMessages) {
                        newListParticipants.push(participants[i]);
                        addedFlag = true;
                    }
                }
                if (!addedFlag) notSeePartisipant.push(participants[i]);
            }
        } else {
            newListParticipants = participants;
        }
        jq.tmpl("projects_subscribedUser", newListParticipants).appendTo("#discussionParticipantsTable");

        if (notSeePartisipant.length) {
            jq.tmpl("projects_subscribedUser", notSeePartisipant).addClass("gray").appendTo("#discussionParticipantsTable");
        }
    };

    var onChangeSubscribe = function (params, discussion) {
        var subscribed = subscribeButton.attr("subscribed") === "1";
        var currentLink = jq("#discussionParticipantsContainer [guid=" + currentUserId + "]");
        if (!currentLink.length) {
            currentLink = jq("#currentLink");
        }
        if (subscribed) {
            currentLink.hide();
            currentLink.addClass('hidden');
            subscribeButton.attr('subscribed', '0');
            subscribeButton.removeClass('subscribed').addClass('unsubscribed');
            if (window.discussionParticipantsSelector) {
                discussionParticipantsSelector.DisableUser(currentUserId);
            }
        } else {
            currentLink.show();
            currentLink.removeClass("hidden");
            subscribeButton.attr("subscribed", "1");
            subscribeButton.removeClass('unsubscribed').addClass('subscribed');
            if (window.discussionParticipantsSelector) {
                discussionParticipantsSelector.SelectUser(currentUserId);
            }
        }
        participantsCount = jq("#discussionParticipantsTable .discussionParticipant").not(".hidden").length;
        updateTabTitle("participants", participantsCount);
    };

    var updateTabTitle = function (tabTitle, count) {
        var container;
        switch (tabTitle) {
            case "comments":
                container = "discussionCommentsContainer";
                break;
            case "participants":
                container = "discussionParticipantsContainer";
                break;
            case "files":
                container = "discussionFilesContainer";
                break;
        }
        if (!container) return;

        var tab = jq("#discussionTabs div[container=" + container + "] span:first");
        var oldTitle = tab.text();
        var ind = oldTitle.lastIndexOf("(");
        var newTitle = oldTitle;
        if (ind > -1 && count == 0) {
            newTitle = oldTitle.slice(0, ind);
        }
        else if (ind > -1 && count != 0) {
            newTitle = oldTitle.slice(0, ind) + "(" + count + ")";
        }
        else {
            if (count > 0)
                newTitle = oldTitle + " (" + count + ")";
        }
        tab.text(newTitle);
    }

    var showQuestionWindow = function () {
        var margintop = jq(window).scrollTop();
        margintop = margintop + "px";
        jq.blockUI({
            message: jq("#questionWindow"),
            css: {
                left: "50%",
                top: "25%",
                opacity: "1",
                border: "none",
                padding: "0px",
                width: "400px",

                cursor: "default",
                textAlign: "left",
                position: "absolute",
                "margin-left": "-200px",
                "margin-top": margintop,
                "background-color": "White"
            },
            overlayCSS: {
                backgroundColor: "#AAA",
                cursor: "default",
                opacity: "0.3"
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0,

            onBlock: function () {
            }
        });
    };

    var deleteDiscussion = function () {
        var params = {};
        Teamlab.removePrjDiscussion(params, discussionId, { success: onDeleteDiscussion, error: ASC.Projects.DiscussionDetails.onDeleteDiscussionError });
    };

    var onDeleteDiscussion = function (params, discussion) {
        window.location.replace("messages.aspx?prjID=" + discussion.projectId);
    };

    return {
        init: init
    };
})(jQuery);

ASC.Projects.DiscussionAction = (function () {
    var fckId;
    var projectId, id;
    var isMobile;
    var loadListTeamFlag = false;
    var projectFolderId, projectName, currentUserId, privateFlag;
    var newFilesToAttach = [];
    var filesFromProject = [];
    var projectTeam = [];

    var init = function () {
        currentUserId = Teamlab.profile.id;
        projectId = jq.getURLParam("prjID");
        fckId = jq("#discussionTextContainer").attr("data-fckId");
        id = jq.getURLParam("id");
        isMobile = jq.browser.mobile || !!(jq.browser.msie && (jq.browser.version == 10 || jq.browser.version == 11));
        privateFlag = jq("#discussionParticipantsContainer").attr("data-private") === "True" ? true : false;

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadProjects, function () {
            if (jq('#discussionProject').length)
                initProjectsCombobox();
        });

        if (projectId)
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function () {
                loadListTeamFlag = true;
                if (jq.getURLParam("action") != "edit") {
                    getTeam({}, projectId);
                } else {
                    projectTeam = ASC.Projects.Master.Team;
                }
            });

        if (jq("#discussionFilesContainer").length)
            initAttachmentsControl();

        jq('[id$=discussionTitle]').focus();

        discussionParticipantsSelector.OnOkButtonClick = function () {
            jq('#discussionParticipantsContainer .discussionParticipant').remove();

            var participants = discussionParticipantsSelector.GetSelectedUsers();
            var partisipantsTmpls = []
            for (var i = 0; i < participants.length; i++) {
                var partisipant = {};
                partisipant.id = participants[i].ID;
                partisipant.displayName = participants[i].Name;
                partisipant.department = participants[i].Group.Name;
                partisipant.title = participants[i].Title;
                partisipant.descriptionFlag = false;
                partisipantsTmpls.push(partisipant);
            }
            showDiscussionParticipants(partisipantsTmpls);
        };

        jq('#discussionParticipantsContainer').on('click', ".discussionParticipant .delMember span", function () {
            var userId = jq(this).closest('tr').attr('guid');
            if (userId != currentUserId) {
                jq(this).closest('tr').remove();
                discussionParticipantsSelector.DisableUser(userId);
            }
        });

        jq('#discussionParticipantsContainer .discussionParticipant').each(function () {
            var userId = jq(this).attr('guid');
            if (userId == currentUserId) {
                jq(this).find('.delMember span').remove();
            }
        });

        jq('#addDiscussionParticipantButton span').click(function () {
            discussionParticipantsSelector.ClearSelection();

            jq('#discussionParticipantsContainer .discussionParticipant').each(function () {
                var userId = jq(this).attr('guid');
                discussionParticipantsSelector.SelectUser(userId);
            });

            discussionParticipantsSelector.IsFirstVisit = true;
            discussionParticipantsSelector.ShowDialog();
        });


        jq('#hideDiscussionPreviewButton').click(function () {
            jq('#discussionPreviewContainer').hide();
        });

        jq('#discussionProject').change(function () {
            if (jq(this).val() != -1) {
                jq('#discussionProject option[value=-1]').remove();
                jq('#discussionProjectContainer').removeClass('requiredFieldError');
                jq('#discussionParticipantsContainer .discussionParticipant').remove();
                jq('#addDiscussionParticipantOfProject').removeClass('disable');
                privateFlag = jq('#discussionProject option[value=' + jq(this).val() + ']').attr("data-private") == "true" ? true : false;
            }
            jq("#discussionTitleContainer input").focus();
        });

        jq('#discussionTitleContainer input').keyup(function () {
            if (jq.trim(jq(this).val()) != '') {
                jq('#discussionTitleContainer').removeClass('requiredFieldError');
            }
        });

        jq('#discussionPreviewButton').click(function () {
            var discussion =
        {
            title: jq('#discussionTitleContainer input').val(),
            authorName: jq(this).attr('authorName'),
            authorTitle: jq(this).attr('authorTitle'),
            authorPageUrl: jq(this).attr('authorPageUrl'),
            authorAvatarUrl: jq(this).attr('authorAvatarUrl'),
            createOn: formatDate(new Date()),
            content: isMobile ? ASC.Controls.HtmlHelper.Text2EncodedHtml(jq('[id$=discussionContent]').val())
                              : FCKeditorAPI.GetInstance(fckId).GetHTML(true)
        };

            jq('#discussionPreviewContainer .discussionContainer').remove();
            jq.tmpl("projects_discussionActionTemplate", discussion).prependTo('#discussionPreviewContainer');
            jq('#discussionPreviewContainer').show();
        });

        jq('#discussionCancelButton').click(function () {
            var projectId = jq.getURLParam('prjID');
            var discussionId = jq.getURLParam("id");
            if (discussionId != "") {
                window.location.replace('messages.aspx?prjID=' + projectId + '&id=' + id);
            } else {
                window.location.replace('messages.aspx?prjID=' + projectId);
            }

        });

        jq('#addDiscussionParticipantOfProject').click(function () {
            if (!jq('#addDiscussionParticipantOfProject').hasClass('disable')) {

                var currentProjectId = jq('#discussionProject').val();

                if (currentProjectId == -1) {
                    jq('#discussionProjectContainer').addClass('requiredFieldError');
                }
                jq('#errorAllParticipantsProject').hide();

                getTeam({}, currentProjectId);

            };
        });

        jq('#discussionActionButton').click(function () {
            jq('#discussionProjectContainer').removeClass('requiredFieldError');
            jq('#discussionTitleContainer').removeClass('requiredFieldError');
            jq('#discussionTextContainer').removeClass('requiredFieldError');

            var projectid = projectId ? projectId : jq('#discussionProject').val();
            var title = jq.trim(jq('#discussionTitleContainer input').val());
            var content = isMobile ?
                ASC.Controls.HtmlHelper.Text2EncodedHtml(jq('[id$=discussionContent]').val()) :
                FCKeditorAPI.GetInstance(fckId).GetHTML(true);

            var isError = false;
            if (projectid == -1) {
                jq('#discussionProjectContainer').addClass('requiredFieldError');
                isError = true;
            }

            if (title == '') {
                jq('#discussionTitleContainer').addClass('requiredFieldError');
                isError = true;
            }

            var tmp = document.createElement("DIV");
            tmp.innerHTML = content;

            if (tmp.textContent == "" || tmp.innerText == "") {
                jq('#discussionTextContainer').addClass('requiredFieldError');
                isError = true;
            }

            if (isError) {
                var scroll = jq('#pageHeader').offset().top;
                jq('body, html').animate({
                    scrollTop: scroll
                }, 500);
                return;
            }

            var discussion =
                {
                    projectid: projectid,
                    title: title,
                    content: content
                };

            var discussionId = jq(this).attr('discussionId');
            if (discussionId != -1) {
                discussion.messageid = discussionId;
            }

            var participants = [];
            jq('#discussionParticipantsContainer .discussionParticipant').each(function () {
                participants.push(jq(this).attr('guid'));
            });
            discussion.participants = participants.join(',');

            lockDiscussionActionPageElements();
            if (discussionId == -1) {
                addDiscussion(discussion);
            }
            else {
                updateDiscussion(discussion);
            }
        });

        jq.switcherAction("#switcherParticipantsButton", "#discussionParticipantsContainer");
        jq.switcherAction("#switcherFilesButton", "#discussionFilesContainer");

        jq("#discussionParticipants").on("mouseenter", ".discussionParticipant.gray", function () {
            jq(this).helper({
                BlockHelperID: "hintSubscribersPrivateProject",
                addLeft: 45,
                addTop: 12
            });
        });
        jq("#discussionParticipants").on("mouseleave", ".discussionParticipant.gray", function () {
            jq("#hintSubscribersPrivateProject").hide();
        });
    };

    var initProjectsCombobox = function () {
        var projectsCombpbox = jq('#discussionProject');
        var allprojects = ASC.Projects.Master.Projects;
        for (var i = 0; i < allprojects.length; i++) {
            var option = document.createElement('option');
            option.setAttribute('value', allprojects[i].id);
            option.setAttribute('data-private', allprojects[i].isPrivate);
            if (projectId && projectId == allprojects[i].id) {
                option.setAttribute('selected', 'selected');
            }
            option.appendChild(document.createTextNode(allprojects[i].title));
            if (allprojects[i].canCreateMessage) {
                projectsCombpbox.append(option);
            }
        }
    };

    var addDiscussion = function (discussion) {
        var params = {};
        Teamlab.addPrjDiscussion(params, discussion.projectid, discussion, { success: onAddDiscussion, error: onAddDiscussionError });
    };

    var onAddDiscussion = function (params, discussion) {
        ASC.Projects.DiscussionAction.attachFiles(discussion);
    };

    var onAddDiscussionError = function () {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
        unlockDiscussionActionPageElements();
    };

    var onUpdateDiscussionError = function () {
        if (this.__errors[0] == "Access denied.") {
            window.location.replace("messages.aspx");
        }
        unlockDiscussionActionPageElements();
    };

    var updateDiscussion = function (discussion) {
        var params = {};
        Teamlab.updatePrjDiscussion(params, discussion.messageid, discussion, { success: onUpdateDiscussion, error: onUpdateDiscussionError });
    };

    var onUpdateDiscussion = function (params, discussion) {
        window.location.replace('messages.aspx?prjID=' + discussion.projectId + '&id=' + discussion.id);
    };
    var lockDiscussionActionPageElements = function () {
        jq('#discussionProject').attr('disabled', 'disabled').addClass('disabled');
        jq('#discussionTitleContainer input').attr('readonly', 'readonly').addClass('disabled');
        jq('iframe[id^=ctl00_ctl00]').contents().find('iframe').contents().find('#fckbodycontent').attr('readonly', 'readonly').addClass('disabled');
        jq('#discussionButtonsContainer').hide();
        jq('#discussionActionsInfoContainer').show();
    }

    var unlockDiscussionActionPageElements = function () {
        jq('#discussionProject').removeAttr('disabled').removeClass('disabled');
        jq('#discussionTitleContainer input').removeAttr('readonly').removeClass('disabled');
        jq('iframe[id^=ctl00_ctl00]').contents().find('iframe').contents().find('#fckbodycontent').removeAttr('readonly').removeClass('disabled');
        jq('#discussionActionsInfoContainer').hide();
        jq('#discussionButtonsContainer').show();
    }

    var showDiscussionParticipants = function (participants) {
        var newListParticipants = [];
        var notSeePartisipant = [];
        if (privateFlag) {
            for (var i = 0; i < participants.length; i++) {
                var addedFlag = false;
                for (var j = 0; j < projectTeam.length; j++) {
                    if ((participants[i].id == projectTeam[j].id) && projectTeam[j].canReadMessages) {
                        newListParticipants.push(participants[i]);
                        addedFlag = true;
                    }
                }
                if (!addedFlag) notSeePartisipant.push(participants[i]);
            }
        } else {
            newListParticipants = participants;
        }
        jq.tmpl("projects_subscribedUser", newListParticipants).appendTo("#discussionParticipants");

        if (notSeePartisipant.length) {
            jq.tmpl("projects_subscribedUser", notSeePartisipant).addClass("gray").appendTo("#discussionParticipants");
        }
    };

    var initAttachmentsControl = function () {
        projectFolderId = parseInt(jq("#discussionFilesContainer").attr("data-projectfolderid"));
        projectName = jq("#discussionFilesContainer").attr("data-projectName").trim();
        var action = jq.getURLParam("action");
        if (action == "edit")
            loadAttachmentsForEditingDiscussion();

        //        if (projectId && action == "add")
        //            loadAttachmentsForCreatingDiscussion();
    };

    var loadAttachmentsForEditingDiscussion = function () {
        var discussionId = jq.getURLParam("id");
        ProjectDocumentsPopup.init(projectFolderId, projectName);
        Attachments.setFolderId(projectFolderId);
        Attachments.loadFiles();

        Attachments.bind("addFile", function (ev, file) {
            Teamlab.addPrjEntityFiles(null, discussionId, "message", [file.id], function () { });
        });
        Attachments.bind("deleteFile", function (ev, fileId) {
            Teamlab.removePrjEntityFiles({}, discussionId, "message", fileId, function () { });
            Attachments.deleteFileFromLayout(fileId);
        });
    };

    var loadAttachmentsForCreatingDiscussion = function () {
        var uploadWithAttach = false;
        Attachments.setFolderId(projectFolderId, uploadWithAttach);
        Attachments.setCreateNewEntityFlag(true);

        ProjectDocumentsPopup.init(projectFolderId, projectName);

        Attachments.bind("addFile", function (ev, file) {
            addFileToList(file);
        });
        Attachments.bind("deleteFile", function (ev, fileId) {
            removeFileFromList(fileId);
        });
        window.onbeforeunload = function (evt) {
            for (var i = 0; i < newFilesToAttach.length; i++) {
                Teamlab.removeDocFile({}, newFilesToAttach[i]);
            }
            return;
        };
    };

    var addFileToList = function (file) {
        if (file.fromProjectDocs) {
            filesFromProject.push(file.id);
        } else {
            newFilesToAttach.push(file.id);
        }
    };

    var removeFileFromList = function (fileId) {

        for (var i = 0; i < filesFromProject.length; i++) {
            if (fileId == filesFromProject[i]) {
                filesFromProject.splice(i, 1);
                Attachments.deleteFileFromLayout(fileId);
                break;
            }
        }
        for (var i = 0; i < newFilesToAttach.length; i++) {
            if (fileId == newFilesToAttach[i]) {
                newFilesToAttach.splice(i, 1);
                Teamlab.removeDocFile({}, fileId);
            }
        }

        Attachments.deleteFileFromLayout(fileId);
    };

    var attachFiles = function (discussion) {
        var filesIds = newFilesToAttach.concat(filesFromProject);
        if (filesIds.length) {
            Teamlab.addPrjEntityFiles(null, discussion.id, "message", filesIds, function () {
                window.location.replace('messages.aspx?prjID=' + discussion.projectId + '&id=' + discussion.id);
            });
        } else {
            window.location.replace('messages.aspx?prjID=' + discussion.projectId + '&id=' + discussion.id);
        }
    };

    var getTeam = function (params, projId) {
        if (loadListTeamFlag) {
            onGetTeam({}, ASC.Projects.Master.Team);
        } else {
            Teamlab.getPrjTeam(params, projId, { before: function () { LoadingBanner.displayLoading(); }, success: onGetTeam, after: function () { LoadingBanner.hideLoading(); } });
        }
    };

    var onGetTeam = function (params, team) {
        projectTeam = team;
        var count = team ? team.length : 0;
        var newParticipants = [];
        if (count > 0) {
            var existParticipants = jq('#discussionParticipantsContainer .discussionParticipant');
            var existCount = existParticipants.length;
            for (var i = 0; i < count; i++) {
                var existFlag = false;
                for (var j = 0; j < existCount; j++) {
                    if (jq(existParticipants[j]).attr('guid') == team[i].id) {
                        existFlag = true;
                        break;
                    }
                }
                if (!existFlag) {
                    team[i].descriptionFlag = false;
                    newParticipants.push(team[i]);
                }
            }
            showDiscussionParticipants(newParticipants);
        }
        jq('#addDiscussionParticipantOfProject').addClass('disable');
    };

    var formatDate = function (date) {
        var dateArray =
            ['0' + date.getDate(), '0' + (date.getMonth() + 1), '0' + date.getFullYear(), '0' + date.getHours(), '0' + date.getMinutes()];
        for (var i = 0; i < dateArray.length; i++) {
            dateArray[i] = dateArray[i].slice(-2);
        }
        var shortDate = dateArray[0] + '.' + dateArray[1] + '.' + dateArray[2] + ' ' + dateArray[3] + ':' + dateArray[4];
        return shortDate;
    };

    return {
        init: init,
        attachFiles: attachFiles
    };
})();