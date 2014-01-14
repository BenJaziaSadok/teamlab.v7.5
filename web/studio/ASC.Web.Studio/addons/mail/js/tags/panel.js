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

window.tagsPanel = (function($) {
    var 
        isInit = false,
        firstLoad = true,
        _panel_content,
        _panel_max_h;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            _panel_content = $('#id_tags_panel_content');

            _panel_max_h = _panel_content.parent().css("max-height").replace(/[^-\d\.]/g, '');
            $('#tags_panel').hover(_expand_tags_panel, _collapse_tags_panel);

            tagsManager.events.bind('refresh', _onRefreshTags);
            tagsManager.events.bind('delete', _onDeleteTag);
            tagsManager.events.bind('update', _onUpdateTag);
            tagsManager.events.bind('increment', _onIncrement);
            tagsManager.events.bind('decrement', _onDecrement);
        }
    };

    var _expand_tags_panel = function() {
        _panel_content.parent().stop().animate({ "max-height": _panel_content.height() }, 200, function() {
            $('#tags_panel .more').css({ 'visibility': 'hidden' });
        });
    };

    var _collapse_tags_panel = function() {
        _panel_content.parent().stop().animate({ "max-height": _panel_max_h }, 200, function() {
            $('#tags_panel .more').css({ 'visibility': 'visible' });
        });
    };

    var _getTag$html = function(tag) {
        var html;
        if ($.inArray(tag.id.toString(), MailFilter.getTags()) >= 0)
            html = '<div class="tag tagArrow tagArrow tag' + tag.style + '" labelid="' + tag.id + '" title="' + tag.name + '"><span class="square tag' + tag.style + '"></span>';
        else
            html = '<div class="tag inactive" labelid="' + tag.id + '" title="' + tag.name + '"><span class="square tag' + tag.style + '"></span>';

        html += '<div class="name">' + tag.short_name + '</div></div>';

        var $html = $(html);

        $html.click(function(e) {
            if (e.isPropagationStopped())
                return;

            // google analytics
            window.ASC.Mail.ga_track(ga_Categories.leftPanel, ga_Actions.filterClick, 'tag');

            var tagid = $(this).attr('labelid');

            if (MailFilter.toggleTag(tagid))
                markTag(tagid);
            else
                unmarkTag(tagid);

            mailBox.updateAnchor();
        });

        return $html;
    };

    var _onRefreshTags = function(e, tags) {
        _panel_content.find('.tag[labelid]').remove();
        $.each(tags, function(index, tag) {
            if (0 >= tag.lettersCount)
                return;
            var $html = _getTag$html(tag);
            _panel_content.append($html);
        });
        _updatePanel();
    };

    var unmarkAllTags = function() {
        _panel_content.find('.tag').removeClass().addClass('tag inactive');
    };

    var unmarkTag = function (tagid) {
        try {
            _panel_content.find('.tag[labelid="' + tagid + '"]').removeClass().addClass('tag inactive');
        }
        catch (err) { }
    };

    var markTag = function (tagid) {
        try {
            var tag = tagsManager.getTag(tagid);
            var css = 'tagArrow tag' + tag.style;
            _panel_content.find('.tag[labelid="' + tagid + '"]').removeClass('inactive').addClass(css);
        }
        catch (err) { }
    };

    var _onUpdateTag = function(e, tag) {
        /*if (0 >= tag.lettersCount) {
            _deleteTag(tag.id);
            return;
        }
        if (0 == _panel_content.find('.tag[labelid="' + tag.id + '"]').length) {
            _insertTag(tag);
            return;
        }*/
        var tag_div = _panel_content.find('.tag[labelid="' + tag.id + '"]');
        tag_div.find('.square').removeClass().addClass('square tag' + tag.style);
        tag_div.find('.name').html(TMMail.ltgt(tag.name));
        _updatePanel();
    };

    var _deleteTag = function(id) {
        _panel_content.find('.tag[labelid="' + id + '"]').remove();
        _updatePanel();
    };
    var _onDeleteTag = function(e, id) {
        _deleteTag(id);
    };

    var _insertTag = function(tag) {
        var $html = _getTag$html(tag);
        var tags = _panel_content.find('.tag[labelid]');
        var insert_flag = false;
        $.each(tags, function(index, value) {
            var id = parseInt($(value).attr('labelid'));
            if ((tag.id > 0 && (id > tag.id || id < 0)) || (tag.id < 0 && id < tag.id)) {
                $(value).before($html);
                insert_flag = true;
                return false;
            }
        });
        if (!insert_flag)
            _panel_content.append($html);
        _updatePanel();
    };
    var _onIncrement = function(e, tag) {
        if (0 == _panel_content.find('.tag[labelid="' + tag.id + '"]').length)
            _insertTag(tag);
    };

    var _onDecrement = function(e, tag) {
        if (0 >= tag.lettersCount)
            _onDeleteTag(e, tag.id);
    };

    var _updatePanel = function() {
        if (0 == $('#tags_panel .tag').length) {
            $('#tags_panel').hide();
            return;
        }
        $('#tags_panel').show();
        if (_panel_max_h < _panel_content.height())
            $('#tags_panel .more').show();
        else
            $('#tags_panel .more').hide();
    };

    return {
        init: init,
        unmarkAllTags: unmarkAllTags,
        unmarkTag: unmarkTag,
        markTag: markTag
    };
})(jQuery);