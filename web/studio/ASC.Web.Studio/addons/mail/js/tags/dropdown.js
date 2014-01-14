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

window.tagsDropdown = (function($) {
    var _popup;
    var popup_id = '#addTagsPanel';
    var isInit = false;
    var _arrow;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            _popup = $(popup_id);

            _bind_inputs();

            tagsManager.events.bind('delete', _onDeleteTag);
            tagsManager.events.bind('create', _onCreateTag);
            tagsManager.events.bind('error', _onError);
        }
    };

    var _onDeleteTag = function() {
        hide();
    };

    var _onCreateTag = function(e, tag) {
        if (_popup.is(":visible"))
            _setTag(tag.id);
        hide();
    };

    // shows add tags panel near obj element depending on current page
    var showAddTagsPanel = function(obj) {
        if ($(popup_id + ':visible').length) {
            hide();
            return;
        }

        var tagContent = _getTagContent();

        $('#tagsPanelContent .existsTags').html(tagContent);

        $(popup_id + ' #createnewtag').val('');
        var $tag = $(popup_id + ' .tag.inactive');
        $tag.bind('click', function() {
            _setTag($(this).attr('tag_id'));
        });

        $tag = $(popup_id + ' .tag.tagArrow');
        $tag.bind('click', function() {
            mailBox.unsetConversationsTag($(this).attr('tag_id'));
        });



        if ((TMMail.pageIs('message') || TMMail.pageIs('conversation')) &&
            messagePage.getMessageFolder(messagePage.getActualConversationLastMessageId()) != TMMail.sysfolders.trash.id) {
            _showMarkRecipientsCheckbox();
        } else {
            _hideMarkRecipientsCheckbox();
        }


        _setTagColor(undefined, tagsManager.getVacantStyle());

        _popup.find('.square').click(function(e) {
            tagsColorsPopup.show(this, _setTagColor);
        });

        _popup.find('#error_message').hide();

        _arrow = $(obj).find('.down_arrow').offset() != null ? $(obj).find('.down_arrow') : $(obj).find('.arrow-down');
        _popup.css({ left: _arrow.offset().left - 11, top: _arrow.offset().top + 8 }).show();

        $(popup_id).find('.popup-corner').removeClass('bottom');

        dropdown.regHide(_filteredHide);
        dropdown.regScroll(_onScroll);

        $(popup_id).find('input[placeholder]').placeholder();
    };

    var _onScroll = function () {
        _popup.css({ top: _arrow.offset().top + 8 });
    };

    var _showMarkRecipientsCheckbox = function() {
        $(popup_id + ' #markallrecipients').prop('checked', false);
        $(popup_id + ' #markallrecipients').show();
        $(popup_id + ' #markallrecipientsLabel').show();
        $(popup_id + ' #markallrecipients').unbind('click').bind('click', _manageCRMTags);
        var fromAddress = messagePage.getFromAddress(messagePage.getActualConversationLastMessageId());
        fromAddress = TMMail.parseEmailFromFullAddress(fromAddress);
        if (accountsManager.getAccountByAddress(fromAddress))
            $(popup_id + ' #markallrecipientsLabel').text(MailScriptResource.MarkAllRecipientsLabel);
        else $(popup_id + ' #markallrecipientsLabel').text(MailScriptResource.MarkAllSendersLabel);
    };


    var _hideMarkRecipientsCheckbox = function() {
        $(popup_id + ' #markallrecipients').prop('checked', false);
        $(popup_id + ' #markallrecipients').hide();
        $(popup_id + ' #markallrecipientsLabel').hide();
    };


    var _manageCRMTags = function(e) {
        if (e.target.checked) {
            $('#tagsPanelContent .tag').each(function() {
                if ($(this).attr('tag_id') < 0) {
                    $(this).addClass("disabled");
                    $(this).unbind("click");
                }
            });
        } else {
            $('#tagsPanelContent .tag').each(function() {
                if ($(this).attr('tag_id') < 0) {
                    $(this).removeClass("disabled");
                    var used_tags_ids = _getUsedTagsIds();
                    var tagId = parseInt($(this).attr("tag_id"));
                    if (-1 == $.inArray(tagId, used_tags_ids))
                        $(this).bind('click', function() {
                            _setTag($(this).attr('tag_id'));
                        });
                    else
                        $(this).bind('click', function() {
                            mailBox.unsetConversationsTag($(this).attr('tag_id'));
                        });
                }
            });
        }
    };


    var _getTagContent = function() {
        var res = '';
        var tags = tagsManager.getAllTags();
        var used_tags_ids = _getUsedTagsIds();
        for (var i = 0; i < tags.length; i++) {
            tags[i].used = ($.inArray(tags[i].id, used_tags_ids) > -1);
        }
        res = $.tmpl('tagInPanelTmpl', tags);
        return res;
    };

    var _setTag = function(id) {
        if (checked()) {
            var tag = tagsManager.getTag(id);
            var addressesString = "";

            addressesString = messagePage.getFromAddress(messagePage.getActualConversationLastMessageId());
            addressesString = TMMail.parseEmailFromFullAddress(addressesString);
            if (accountsManager.getAccountByAddress(addressesString))
                addressesString = messagePage.getToAddresses(messagePage.getActualConversationLastMessageId());

            var addresses = addressesString.split(',');

            for (var i = 0; i < addresses.length; i++) {
                var address = addresses[i];
                address = TMMail.parseEmailFromFullAddress(address);
                var tag_already_added = 0 < $.grep(tag.addresses, function(val) { return address.toLowerCase() == val.toLowerCase(); }).length;
                if (!tag_already_added)
                    tag.addresses.push(address);
            }
            tagsManager.updateTag(tag);
        }

        hide();

        mailBox.setTag(id);
    };


    // Get a list of tags that are set for all selected messages, or for a particular message.
    var _getUsedTagsIds = function() {
        if (TMMail.pageIs('sysfolders'))
            return mailBox.getSelectedMessagesUsedTags();
        else
            return messagePage.getTags();
    };

    var _setTagColor = function(obj, style) {
        _popup.find('a.square').removeClass().addClass('square tag' + style);
        _popup.find('a.square').attr('colorstyle', style);
    };

    // Rerurns flag value. Flag: Is needed to set tag for all messages sended from that user.
    var checked = function() {
        return $(popup_id + ' input#markallrecipients').prop('checked');
    };

    var _create_label = function() {
        var tag_name = $.trim(TMMail.ltgt(_popup.find('input[type="text"]').val()));
        if (tag_name.length == 0) {
            _setErrorMessage(MailScriptResource.TagNameCantBeEmpty);
            return false;
        }

        var tag = { name: tag_name, style: _popup.find('.actionPanelSection .square').attr('colorstyle'), addresses: [] };
        tagsManager.createTag(tag);

        return false;
    };

    var _bind_inputs = function() {
        _popup.find('input[type="button"]').click(_create_label);

        _popup.find('input[type="text"]').keypress(function(e) {
            if (e.which != 13) return;
            return _create_label();
        });
    };

    var hide = function() {
        _popup.hide();
        tagsColorsPopup.hide();
        dropdown.unregHide(_filteredHide);
        dropdown.unregHide(_onScroll);
    };

    var _filteredHide = function(event) {
        var elt = (event.target) ? event.target : event.srcElement;
        if (!($(elt).is(popup_id + ' *') || $(elt).is(popup_id) || $(elt).is('div[colorstyle]')))
            hide();
    };

    var _onError = function(e, error) {
        _setErrorMessage(error.message + (error.comment ? ': ' + error.comment : ''));
    };

    var _setErrorMessage = function(error_message) {
        _popup.find('#error_message').show().html(error_message);
    };

    return {
        init: init,
        show: showAddTagsPanel,
        hide: hide,
        checked: checked
    };
})(jQuery);