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


var FeedProductsColection = {
        community: ASC.Resources.Master.FeedResource.CommunityProduct,
        projects: ASC.Resources.Master.FeedResource.ProjectsProduct,
        crm: ASC.Resources.Master.FeedResource.CrmProduct,
        documents: ASC.Resources.Master.FeedResource.DocumentsProduct
    };

var FeedTextsColection = {
    blog: {
        createdText: ASC.Resources.Master.FeedResource.BlogCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.BlogCommentedText,
        location: ASC.Resources.Master.FeedResource.BlogModule
    },
    news: {
        createdText: ASC.Resources.Master.FeedResource.NewsCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.EventCommentedText,
        extraText: ASC.Resources.Master.FeedResource.EventsModule
    },
    order: {
        createdText: ASC.Resources.Master.FeedResource.OrderCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.EventCommentedText,
        extraText: ASC.Resources.Master.FeedResource.EventsModule
    },
    advert: {
        createdText: ASC.Resources.Master.FeedResource.AdvertCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.EventCommentedText,
        extraText: ASC.Resources.Master.FeedResource.EventsModule
    },
    poll: {
        createdText: ASC.Resources.Master.FeedResource.PollCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.EventCommentedText,
        extraText: ASC.Resources.Master.FeedResource.EventsModule
    },
    bookmark: {
        createdText: ASC.Resources.Master.FeedResource.BookmarkCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.BookmarkCommentedText
    },
    forumPost: {
        createdText: ASC.Resources.Master.FeedResource.ForumPostCreatedText,
        location: ASC.Resources.Master.FeedResource.ForumModule
    },
    forumTopic: {
        createdText: ASC.Resources.Master.FeedResource.ForumTopicCreatedText,
        location: ASC.Resources.Master.FeedResource.ForumModule
    },
    forumPoll: {
        createdText: ASC.Resources.Master.FeedResource.ForumPollCreatedText,
        location: ASC.Resources.Master.FeedResource.ForumModule
    },
    company: {
        createdText: ASC.Resources.Master.FeedResource.CompanyCreatedText
    },
    person: {
        createdText: ASC.Resources.Master.FeedResource.PersonCreatedText
    },
    crmTask: {
        createdText: ASC.Resources.Master.FeedResource.CrmTaskCreatedText,
        extraText: ASC.Resources.Master.FeedResource.Responsible,
        extraText2: ASC.Resources.Master.FeedResource.Contact
    },
    deal: {
        createdText: ASC.Resources.Master.FeedResource.DealCreatedText,
        extraText2: ASC.Resources.Master.FeedResource.Contact
    },
    cases: {
        createdText: ASC.Resources.Master.FeedResource.CaseCreatedText
    },
    project: {
        createdText: ASC.Resources.Master.FeedResource.ProjectCreatedText,
        extraText: ASC.Resources.Master.FeedResource.ProjectManager
    },
    participant: {
        createdText: ASC.Resources.Master.FeedResource.ParticipantCreatedText
    },
    milestone: {
        createdText: ASC.Resources.Master.FeedResource.MilestoneCreatedText,
        extraText: ASC.Resources.Master.FeedResource.Project
    },
    task: {
        createdText: ASC.Resources.Master.FeedResource.TaskCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.TaskCommentedText,
        extraText: ASC.Resources.Master.FeedResource.Project,
        extraText2: ASC.Resources.Master.FeedResource.Responsibles
    },
    discussion: {
        createdText: ASC.Resources.Master.FeedResource.DiscussionCreatedText,
        commentedText: ASC.Resources.Master.FeedResource.DiscussionCommentedText
    },
    file: {
        createdText: ASC.Resources.Master.FeedResource.FileCreatedText,
        createdExtraText: ASC.Resources.Master.FeedResource.FileCreatedInFolderText,
        updatedText: ASC.Resources.Master.FeedResource.FileUpdatedText,
        updatedExtraText: ASC.Resources.Master.FeedResource.FileUpdatedInFolderText,
        extraText: ASC.Resources.Master.FeedResource.Size
    },
    sharedFile: {
        createdText: ASC.Resources.Master.FeedResource.SharedFileCreatedText,
        createdExtraText: ASC.Resources.Master.FeedResource.SharedFileCreatedInFolderText,
        extraText: ASC.Resources.Master.FeedResource.Size
    },
    folder: {
        createdText: ASC.Resources.Master.FeedResource.FolderCreatedText,
        createdExtraText: ASC.Resources.Master.FeedResource.FolderCreatedInFolderText,
    },
    sharedFolder: {
        createdText: ASC.Resources.Master.FeedResource.SharedFolderCreatedText
    }
};

var FeedReader = new function () {
    var guestId = "712d9ec3-5d2b-4b13-824f-71f00191dcca";
    // for develop set "/asc"
    var developUrlPrefix = "";

    this.HasNewFeeds = false;
    this.DataReaded = false;

    this.GetNewFeedsCount = function () {
        Teamlab.getNewFeedsCount({}, { filter: {}, success: FeedReader.OnGetNewFeedsCount });
    };

    this.OnGetNewFeedsCount = function (params, newsCount) {
        if (!newsCount) {
            return;
        }
        if (newsCount > 100) {
            newsCount = ">100";
        }

        var $feedLink = jq(".top-item-box.feed");
        $feedLink.addClass("has-led");
        $feedLink.find(".inner-label").text(newsCount);

        FeedReader.HasNewFeeds = true;
    };

    this.GetDropFeeds = function () {
        var filter = {
            onlyNew: true,
        };

        Teamlab.getFeeds({}, { filter: filter, success: FeedReader.OnGetDropFeeds });
    };

    this.OnGetDropFeeds = function (params, response) {
        var feeds = response.feeds,
            $dropFeedsBox = jq("#drop-feeds-box"),
            $loader = $dropFeedsBox.find(".loader"),
            $feedsReadedMsg = $dropFeedsBox.find(".feeds-readed-msg"),
            $seeAllBtn = $dropFeedsBox.find(".see-all-btn"),
            dropFeedsList = $dropFeedsBox.find(".list");

        if (!feeds.length) {
            $loader.hide();
            $feedsReadedMsg.show();
            $seeAllBtn.css("display", "inline-block");
            return;
        }

        dropFeedsList.empty();

        for (var i = 0; i < feeds.length; i++) {
            try {
                var template = getFeedTemplate(feeds[i]);
                jq.tmpl("dropFeedTmpl", template).appendTo(dropFeedsList);
            } catch (e) {
                console.log(e);
            }
        }

        $loader.hide();
        jq(dropFeedsList).removeClass("display-none");
        $seeAllBtn.css("display", "inline-block");
    };

    var getFeedTemplate = function (feed) {
        var template = feed;

        template.byGuest = template.author.UserInfo.ID == guestId;
        template.productText = getFeedProductText(template);

        if (!template.location) {
            template.location = getFeedLocation(template);
        }

        resolveAdditionalFeedData(template);
        template.actionText = getFeedActionText(template);
        
        template.author.ProfileUrl = developUrlPrefix + template.author.ProfileUrl;
        template.itemUrl = developUrlPrefix + template.itemUrl;

        return template;
    };

    function getFeedProductText(template) {
        var productsCollection = FeedProductsColection;
        if (!productsCollection) {
            return null;
        }

        return productsCollection[template.product];
    }

    function getFeedLocation(template) {
        var textsColection = FeedTextsColection;
        if (!textsColection) {
            return null;
        }

        var itemTexts = textsColection[template.item];
        if (!itemTexts) {
            return null;
        }

        return itemTexts.location;
    }

    function getFeedActionText(template) {
        var textsCollection = FeedTextsColection;
        if (!textsCollection) {
            return null;
        }

        var itemTexts = textsCollection[template.item];
        if (!itemTexts) {
            return null;
        }

        switch (template.action) {
            case 0:
                return template.extraAction ? itemTexts.createdExtraText : itemTexts.createdText;
            case 1:
                return template.extraAction ? itemTexts.updatedExtraText : itemTexts.updatedText;
            case 2:
                return itemTexts.commentedText;
            default:
                return template.extraAction ? itemTexts.createdExtraText : itemTexts.createdText;
        }
    }

    function resolveAdditionalFeedData(template) {
        switch (template.item) {
            case "blog":
                template.itemClass = "blogs";
                break;
            case "news":
            case "order":
            case "advert":
            case "poll":
                template.itemClass = "events";
                break;
            case "forum":
            case "forumPoll":
                template.itemClass = "forum";
                break;
            case "bookmark":
                template.itemClass = "bookmarks";
                break;
            case "company":
            case "person":
                template.itemClass = "group";
                break;
            case "crmTask":
                template.itemClass = "tasks";
                break;
            case "deal":
                template.itemClass = "opportunities";
                break;
            case "cases":
                template.itemClass = "cases";
                break;
            case "project":
                template.itemClass = "projects";
                break;
            case "participant":
                template.itemClass = "projects";
                break;
            case "milestone":
                template.itemClass = "milestones";
                break;
            case "task":
                template.itemClass = "tasks";
                break;
            case "discussion":
                template.itemClass = "discussions";
                break;
            case "file":
            case "sharedFile":
                template.itemClass = "documents";
                template.extraAction = template.extra2;
                template.extraActionUrl = template.extra3;
                break;
            case "folder":
            case "sharedFolder":
                template.itemClass = "documents";
                template.extraAction = template.extra;
                template.extraActionUrl = template.extra2;
                break;
        }
    }
};

jq(document).ready(function () {
    jq.dropdownToggle({
        switcherSelector: ".studio-top-panel .feedActiveBox",
        dropdownID: "studio_dropFeedsPopupPanel",
        addTop: 5,
        addLeft: -405
    });

    jq(".studio-top-panel .feedActiveBox").on("mouseup", function (event) {
        if (event.which == 2 && FeedReader.HasNewFeeds) {
            FeedReader.HasNewFeeds = false;
            jq(".studio-top-panel .feedActiveBox .inner-label").remove();
        }
    });

    jq(".studio-top-panel .feedActiveBox").on("click", function (event) {
        if (event.which == 2 && FeedReader.HasNewFeeds) {
            FeedReader.HasNewFeeds = false;
            jq(".studio-top-panel .feedActiveBox .inner-label").remove();
            return true;
        }
        if (event.which != 1) {
            return true;
        }

        if (FeedReader.HasNewFeeds) {
            if (!FeedReader.DataReaded) {
                FeedReader.GetDropFeeds();
                Teamlab.readFeeds({}, {
                    filter: {},
                    success: function (params, readed) {
                        if (readed) {
                            FeedReader.DataReaded = true;
                            jq(".studio-top-panel .feedActiveBox .inner-label").remove();
                        }
                    }
                });
            }
            event.preventDefault();
        } else {
            event.stopPropagation();
        }
        
        return true;
    });
    
    setTimeout(function() {
        FeedReader.GetNewFeedsCount();
    }, 3000);
});