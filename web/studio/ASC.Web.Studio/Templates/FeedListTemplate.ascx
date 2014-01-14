<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Import Namespace="Resources" %>

<script id="feedTmpl" type="text/x-jquery-tmpl">
    <div id="feed_${id}" class="item">
        {{if byGuest}}
        <img src="${authorAvatar}" class="avatar"/>
        {{else}}
        <a href="${author.ProfileUrl}" target="_blank"><img src="${authorAvatar}" class="avatar"/></a>
        {{/if}}
        <div class="content-box">
            <div class="title">
                <a href="${itemUrl}" target="_blank"
                   data-extra="${extra}" data-hintName="${hintName}" 
                   data-extra2="${extra2}" data-hintName2="${hintName2}"
                   data-extra3="${extra3}" data-hintName3="${hintName3}"
                   data-extra4="${extra4}" data-hintName4="${hintName4}">${title}</a>
                {{if isNew}}
                <span title="<%= FeedResource.NewFeedIndicator %>"><%= FeedResource.NewFeedIndicator %></span>
                {{/if}}
            </div>
            <div class="description">
                <span class="menu-item-icon ${itemClass}" />
                <span class="product">${productText}.</span>
                {{if location}}
                <span class="location" title="${location}">${location}.</span>
                {{/if}}
                <span class="action">${actionText}</span>
                {{if extraAction}}
                <span class="extra-action">"<a href="${extraActionUrl}" target="_blank">${extraAction}</a>"</span>
                {{/if}}
                {{if groupedFeeds.length}}
                <span class="grouped-feeds-count">
                    ${ASC.Resources.Master.FeedResource.OtherFeedsCountMsg.format(groupedFeeds.length)}
                </span>
                {{/if}}
            </div>
            <div class="date">
                {{if isToday}}
                <span><%= FeedResource.TodayAt + " " %>${displayCreatedTime}</span>
                {{else isYesterday}}
                <span><%= FeedResource.YesterdayAt + " " %>${displayCreatedTime}</span>
                {{else}}
                <span>${displayCreatedDate}</span>
                <span class="time">${displayCreatedTime}</span>
                {{/if}}
            </div>
            <div class="author"> 
                <span><%= FeedResource.Author %>:</span>
                {{if byGuest}}
                <span class="guest">${author.DisplayName}</span>
                {{else}}
                <a href="${author.ProfileUrl}" target="_blank">${author.DisplayName}</a>
                {{/if}}
            </div>
            <div class="body">
                {{html description}}
                {{if hasPreview}}
                <div class="show-all-btn control-btn"><%= FeedResource.ShowAll %></div>
                {{/if}}
            </div>
            {{if groupedFeeds.length}}
                <div class="control-btn show-grouped-feeds-btn">
                    <%= FeedResource.ShowGroupedFeedsBtn %>
                </div>
                <div class="control-btn hide-grouped-feeds-btn">
                    <%= FeedResource.HideGroupedFeedsBtn %>
                </div>
                <div class="grouped-feeds-box">
                    {{each groupedFeeds}}
                    <div><a class="title" href="${ItemUrl}" target="_blank">${Title}</a></div>
                    {{/each}}
                </div>
            {{/if}}
            {{if canComment}}
            <div id="write-comment-btn-${id}" class="control-btn write-comment-btn">
                <%= FeedResource.WriteComment %>
            </div>
            <div class="comment-errorr-msg-box">
                <span class="red-text"><%= FeedResource.CommentErrorMsg %></span>
            </div>
            {{if comments && comments.length}}
                <div style="margin-top: 5px;"></div>
            {{/if}}
            <div class="comments-box">
                <div id="comment-form-${id}" class="comment-form">
                    <textarea></textarea>
                    <a id="publish-comment-btn-${id}" class="publish-comment-btn button" href="#" 
                       data-id="${id}" 
                       data-entity="${item}" 
                       data-entityid="${itemId}" 
                       data-commentapiurl="${commentApiUrl}"><%= FeedResource.PublishCommentBtn %></a>
                    <a id="cancel-comment-btn-${id}" class="cancel-comment-btn button gray" href="#" data-id="${id}"><%= FeedResource.CancelCommentBtn %></a>
                </div>
                {{each comments}}
                <div class="comment">
                    <a href="${author.ProfileUrl}" target="_blank"><img class="comment-avatar" src="${authorAvatar}"/></a>
                    <div class="comment-content-box">
                        <div class="comment-author">
                            <a href="${author.ProfileUrl}" target="_blank">${author.DisplayName}<span>,</span></a>
                            {{if author.UserInfo.Title}}
                            <span>${author.UserInfo.Title},</span>
                            {{/if}}
                            <span>${formattedDate}</span>
                        </div>
                        <div class="comment-body">{{html description}}</div>
                        <div class="reply-comment-btn" data-commentid="${id}"><%= FeedResource.ReplyCommentBtn %></div>
                    </div>
                </div>
                {{/each}}
            </div>
            {{/if}}
        </div>
    </div>
</script>

<script id="feedCommentTmpl" type="text/x-jquery-tmpl">
    <div class="comment">
        <a href="${authorLink}" target="_blank"><img class="comment-avatar" src="${authorAvatar}"/></a>
        <div class="comment-content-box">
            <div class="comment-author">
                <a href="${authorLink}" target="_blank">${authorName}<span>,</span></a>
                {{if authorPosition}}
                    <span>${authorPosition},</span>
                {{/if}}
                <span>${date}</span>
            </div>
            <div class="comment-body">{{html description}}</div>
            <div class="reply-comment-btn" data-commentid="${id}"><%= FeedResource.ReplyCommentBtn %></div>
        </div>
    </div>
</script>