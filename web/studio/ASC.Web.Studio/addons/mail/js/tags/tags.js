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

window.tagsManager = (function($) {
    var 
        isInit = false,
        tags = [],
        events = $({});

    var init = function() {
        if (isInit === false) {
            isInit = true;

            serviceManager.bind(Teamlab.events.removeMailTag, _onDeleteMailTag);
            serviceManager.bind(Teamlab.events.updateMailTag, _onUpdateMailTag);
            serviceManager.bind(Teamlab.events.createMailTag, _onCreateMailTag);
            serviceManager.bind(Teamlab.events.getMailTags, _onGetMailTags);

            tagsPanel.init();
            tagsColorsPopup.init();
            tagsDropdown.init();
            tagsModal.init();
            tagsPage.init();
        }
    };

    function convertServerTag(server_tag) {
        var tag = {};
        tag.id = server_tag.id;
        tag.name = TMMail.ltgt(server_tag.name);
        tag.short_name = cutTagName(tag.name);
        tag.style = server_tag.style;
        if (0 > tag.id)
            tag.style = Math.abs(tag.id) % 16 + 1;
        tag.addresses = server_tag.addresses;
        tag.lettersCount = server_tag.lettersCount;
        return tag;
    }

    function _onGetMailTags(params, tags_arr) {
        tags = $.map(tags_arr, convertServerTag);
        events.trigger('refresh', [tags]);
    }

    var _onUpdateMailTag = function (params, tag) {
        tag = convertServerTag(tag);
        $.each(tags, function(i) {
            if (tag.id == tags[i].id) {
                tags[i] = tag;
                events.trigger('update', tag);
                return false; //break
            }
        });
    };

    var _onCreateMailTag = function (params, tag) {
        tag = convertServerTag(tag);
        var mail_tags = $.grep(tags, function(item) { return item.id > 0 ? true : false; });
        tags.splice(mail_tags.length, 0, tag);
        var prev_tag_id = mail_tags.length > 0 ? mail_tags[mail_tags.length - 1].id : undefined;
        events.trigger('create', [tag, prev_tag_id]);
    };

    var _onErrorCreateMailTag = function(params, errors) {
        events.trigger('error', { message: errors[0], comment: '' });
    };

    var _onDeleteMailTag = function(params, id) {
        tags = $.grep(tags, function(tag) {
            return tag.id != id;
        });
        events.trigger('delete', id);
    };

    var getTag = function(tagid) {
        var res = undefined;
        $.each(tags, function(i, v) {
            if (v.id == tagid) {
                res = v;
                return false;
            }
        });
        return res;
    };

    var getTagByName = function(name) {
        name = TMMail.ltgt(name);
        for (var i = 0; i < tags.length; i++) {
            if (tags[i].name.toLowerCase() == name.toLowerCase()) {
                return tags[i];
            }
        }
        return undefined;
    };

    var getAllTags = function() {
        return tags;
    };

    var createTag = function(tag) {
        if (getTagByName(tag.name)) {
            events.trigger('error', { message: MailScriptResource.ErrorTagNameAlreadyExists.replace('%1', tag.name), comment: '' });
            return;
        }

        if (!tag.addresses)
            tag.addresses = [];

        serviceManager.createTag(tag.name, tag.style, tag.addresses, {}, { error: _onErrorCreateMailTag });
    };

    var updateTag = function(tag) {
        var findTag = getTagByName(tag.name);
        if (findTag && tag.id != findTag.id) {
            events.trigger('error', { message: MailScriptResource.ErrorTagNameAlreadyExists.replace('%1', tag.name), comment: '' });
            return;
        }

        serviceManager.updateTag(tag.id, tag.name, tag.style, tag.addresses, {}, { error: _onErrorCreateMailTag });
    };

    var deleteTag = function(id) {
        serviceManager.deleteTag(id, {}, {}, ASC.Resources.Master.Resource.LoadingProcessing);
    };

    var getVacantStyle = function() {
        var mail_tags = $.grep(tags, function(item) { return item.id > 0 ? true : false; });
        if (mail_tags.length > 0)
            return (parseInt(mail_tags[mail_tags.length - 1].style)) % 16 + 1;
        return 1;
    };

    var increment = function(id) {
        var tag = getTag(id);
        if (tag) {
            tag.lettersCount += 1;
            events.trigger('increment', [tag]);
        }
    };

    var decrement = function(id) {
        var tag = getTag(id);
        if (tag) {
            if (tag.lettersCount > 0)
                tag.lettersCount -= 1;
            events.trigger('decrement', [tag]);
        }
    };

    var cutTagName = function(tagName) {
        var hardcoded_tag_name_for_view_length = 25
        var last_slash_index = tagName.lastIndexOf('/');
        var resultName = '';
        if (-1 == last_slash_index || tagName.length < hardcoded_tag_name_for_view_length) {
            resultName = tagName;
        } else {
            if ((tagName.length - last_slash_index) < hardcoded_tag_name_for_view_length) {
                var length_of_befin = hardcoded_tag_name_for_view_length - (tagName.length - last_slash_index) - 3;
                resultName = tagName.substr(0, length_of_befin) + '...' + tagName.substr(last_slash_index);
            } else {
                var length_of_tag_end = hardcoded_tag_name_for_view_length - 3;
                resultName = '...' + tagName.substr(tagName.length - length_of_tag_end, length_of_tag_end);
            }
        }
        return resultName;
    };

    return {
        init: init,
        getTag: getTag,
        getTagByName: getTagByName,
        getAllTags: getAllTags,

        createTag: createTag,
        updateTag: updateTag,
        deleteTag: deleteTag,
        getVacantStyle: getVacantStyle,

        increment: increment,
        decrement: decrement,

        events: events
    };
})(jQuery);