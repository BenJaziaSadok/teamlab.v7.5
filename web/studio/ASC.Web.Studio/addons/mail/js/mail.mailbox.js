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

window.mailBox = (function($) {
    var is_init = false,
        current_message_id = -1,
        current_folder_id = -1,
        keep_selection_on_reload = false,
        page_is_loaded = false,
        last_selected_concept = null,
        max_width_of_switching = 1424,
        min_width_of_switching = 1024,
        max_displayed_tags_count = 3;

    var selection = new TMContainers.IdMap();
    var current_selection = new TMContainers.IdMap();

    var selection_concept = {
        unread: { displayAcc: ASC.Mail.Resources.MailResource.OverallConceptUnread_Acc, displayGen: ASC.Mail.Resources.MailResource.OverallConceptUnread_Gen },
        read: { displayAcc: ASC.Mail.Resources.MailResource.OverallConceptRead_Acc, displayGen: ASC.Mail.Resources.MailResource.OverallConceptRead_Gen },
        important: { displayAcc: ASC.Mail.Resources.MailResource.OverallConceptImportant_Acc, displayGen: ASC.Mail.Resources.MailResource.OverallConceptImportant_Gen },
        with_attachments: { displayAcc: ASC.Mail.Resources.MailResource.OverallConceptWithAttachments_Acc, displayGen: ASC.Mail.Resources.MailResource.OverallConceptWithAttachments_Gen },
        all: { displayAcc: ASC.Mail.Resources.MailResource.OverallConceptAll, displayGen: ASC.Mail.Resources.MailResource.OverallConceptAll }
    };

    function init() {
        if (is_init === false) {
            is_init = true;

            ASC.Controls.AnchorController.bind(TMMail.anchors.common,               onInboxPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.sysfolders,           onSysFolderPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.compose,              onCompose);
            ASC.Controls.AnchorController.bind(TMMail.anchors.composeto,            onComposeTo);
            ASC.Controls.AnchorController.bind(TMMail.anchors.reply,                onReplyPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.replyAll,             onReplyAllPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.forward,              onForwardPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.draftitem,            onDraftItemPage);

            ASC.Controls.AnchorController.bind(TMMail.anchors.message,              onMessagePage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.next_message,         onNextMessagePage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.prev_message,         onPrevMessagePage);

            ASC.Controls.AnchorController.bind(TMMail.anchors.conversation,         onConversationPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.next_conversation,    onNextConversationPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.prev_conversation,    onPrevConversationPage);

            ASC.Controls.AnchorController.bind(TMMail.anchors.accounts,             onAccountsPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.teamlab,              onTeamLabContactsPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.crm,                  onCrmContactsPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.tags,                 onTagsPage);
            ASC.Controls.AnchorController.bind(TMMail.anchors.helpcenter,           onHelpPage);

            // ToDo: why here??? refactoring needed
            wysiwygEditor.init();
            messagePage.init();

            tagsManager.events.bind('delete', onDeleteTag);
            tagsManager.events.bind('update', onUpdateTag);

            serviceManager.bind(window.Teamlab.events.getMailFilteredConversations, onGetMailConversations);
            serviceManager.bind(window.Teamlab.events.getMailMessagesModifyDate,    onGetMailMessagesModifyDate);
            serviceManager.bind(window.Teamlab.events.removeMailFolderMessages,     onRemoveMailFolderMessages);
            serviceManager.bind(window.Teamlab.events.restoreMailMessages,          onRestoreMailMessages);
            serviceManager.bind(window.Teamlab.events.restoreMailConversations,     onRestoreMailMessages);
            serviceManager.bind(window.Teamlab.events.moveMailMessages,             onMoveMailMessages);
            serviceManager.bind(window.Teamlab.events.moveMailConversations,        onMoveMailConversations);
            serviceManager.bind(window.Teamlab.events.removeMailMessages,           onRemoveMailMessages);
            serviceManager.bind(window.Teamlab.events.removeMailConversations,      onRemoveMailMessages);
            serviceManager.bind(window.Teamlab.events.markMailMessages,             onMarkMailMessages);

            serviceManager.bind(window.Teamlab.events.getNextMailConversationId, onGetNextPrevConversationId);
            serviceManager.bind(window.Teamlab.events.getPrevMailConversationId, onGetNextPrevConversationId);

            serviceManager.bind(window.Teamlab.events.getNextMailMessageId, onGetNextPrevMessageId);
            serviceManager.bind(window.Teamlab.events.getPrevMailMessageId, onGetNextPrevMessageId);

            $('#createNewMailBtn').trackEvent(ga_Categories.leftPanel, ga_Actions.buttonClick, "create-new-Email");
            $('#check_email_btn').trackEvent(ga_Categories.leftPanel, ga_Actions.buttonClick, "check_email");

            $(document).on("click", '.menu-list a.menu-item-label', messagePage.onLeaveMessage);
            $('#createNewMailBtn').click(messagePage.onLeaveMessage);
            $('#check_email_btn').click(messagePage.onLeaveMessage);

            $(window).resize(function() {
                if (TMMail.pageIs('sysfolders')) changeTagStyle();
                resizeActionMenuWidth();
            });

            $(window).scroll(stickActionMenuToTheTop);
        }
    }

    function getScrolledGroupOptions() {
        var options = undefined;
        if (TMMail.pageIs('sysfolders')) {
            options = {
                menuSelector: "#MessagesListGroupButtons",
                menuAnchorSelector: "#SelectAllMessagesCB",
                menuSpacerSelector: "#itemContainer .contentMenuWrapper.messagesList .header-menu-spacer",
                userFuncInTop: function() { $("#MessagesListGroupButtons .menu-action-on-top").hide(); },
                userFuncNotInTop: function() { $("#MessagesListGroupButtons .menu-action-on-top").show(); }
            };
        } else if (TMMail.pageIs('teamlab') || TMMail.pageIs('crm')) {
            options = {
                menuSelector: "#ContactsListGroupButtons",
                menuAnchorSelector: "#SelectAllContactsCB",
                menuSpacerSelector: "#id_contacts_page .contentMenuWrapper .header-menu-spacer",
                userFuncInTop: function() { $("#ContactsListGroupButtons .menu-action-on-top").hide(); },
                userFuncNotInTop: function() { $("#ContactsListGroupButtons .menu-action-on-top").show(); }
            };
        } else if (TMMail.pageIs('message') || TMMail.pageIs('conversation')) {
            options = {
                menuSelector: "#MessageGroupButtons",
                menuAnchorSelector: "#MessageGroupButtons .btnReply",
                menuSpacerSelector: "#itemContainer .messageHeader .contentMenuWrapper .header-menu-spacer",
                userFuncInTop: function() { $("#MessageGroupButtons .menu-action-on-top").hide(); },
                userFuncNotInTop: function() { $("#MessageGroupButtons .menu-action-on-top").show(); }
            };
        }

        return options;
    }

    function resizeActionMenuWidth() {
        var options = getScrolledGroupOptions();
        if (options != undefined && $(options.menuSelector).is(":visible")) {
            window.ScrolledGroupMenu.resizeContentHeaderWidth(options.menuSelector);
        }
    }

    function stickActionMenuToTheTop() {
        var options = getScrolledGroupOptions();
        if (options != undefined && $(options.menuSelector).is(":visible")) {
            window.ScrolledGroupMenu.stickMenuToTheTop(options);
        }
    }

    function getTagWidth() {
        var width;
        var current_widow_width = $(window).width();
        if (current_widow_width >= max_width_of_switching) width = 160;
        else if (current_widow_width <= min_width_of_switching) width = 30;
        else width = 30 + (current_widow_width - min_width_of_switching) * 0.3;
        return width;
    }

    function changeTagStyle() {
        var max_width = "max-width: " + getTagWidth() + "px";
        $("#itemContainer").find(".tag.tagArrow").attr("style", max_width);
    }

    function unmarkAllPanels(skip_filter_panels) {
        if (true !== skip_filter_panels){
            tagsPanel.unmarkAllTags();
            accountsPanel.unmark();
        }

        accountsPanel.updateAnchors();

        contactsPanel.unmarkContacts();
        folderPanel.unmarkFolders();
        settingsPanel.unmarkSettings();
        helpPanel.unmarkSettings();
    }

    function getMessagesAddresses() {
        var addresses = [];
        selection.Each(function(message_id) {
            var address = $('tr[messageid="' + message_id + '"]').find('span.author').attr('email');
            var email = TMMail.parseEmailFromFullAddress(address);
            if (TMMail.reEmailStrict.test(email) && !TMMail.in_array(address, addresses))
                addresses.push(address);
        });
        return addresses;
    }

    // Makes checkboxes checked or not depending on ids from _Selection
    // Note: No need to call updateSelectionComboCheckbox after this
    function updateSelectionView() {
        var have_one_unchecked = false;
        current_selection.Clear();
        $('.messages:visible .message').each(function() {
            var row = $(this);
            var message_id = row.attr('messageid');
            var $checkbox = row.find('input[type="checkbox"]');
            if (selection.HasId(message_id)) {
                current_selection.AddId(message_id);
                $checkbox.prop('checked', true);
                row.addClass('selected');
            }
            else {
                $checkbox.prop('checked', false);
                row.removeClass('selected');
                have_one_unchecked = true;
            }
        });
        setSelectionComboCheckbox(!have_one_unchecked);
        updateOverallSelectionLine();
    }

    function updateSelectionComboCheckbox(messages_table) {
        // Update checked state
        var unchecked_found = false;
        messages_table = messages_table || $('.messages:visible');
        messages_table.find('.message input[type="checkbox"]').each(function() {
            if (!current_selection.HasId($(this).attr('messageid'))) {
                unchecked_found = true;
                return false;
            }
            return true;
        });
        setSelectionComboCheckbox(!unchecked_found);
    }

    function setSelectionComboCheckbox(checked) {
        $('#SelectAllMessagesCB').prop('checked', checked);
    }

    function updateOverallSelectionLine() {
        var selected_num = selection.Count();
        $('#OverallSelectionNumber').toggle(selected_num != 0);
        $('#OverallSelectionNumberText').text(selected_num);
        $('#OverallSelectedNumberCategory').html(last_selected_concept && last_selected_concept != selection_concept.all ? (last_selected_concept.displayGen + '&nbsp;') : '');

        $('#OverallDeselectAll').toggle(selected_num != 0);
    }

    function overallDeselectAll() {
        selection.Clear();
        last_selected_concept = null;
        updateSelectionView();
        commonLettersButtonsState();
    }

    function hideContentDivs(skip_folder_filter) {
        $('#itemContainer .messages').hide();
        $('#bottomNavigationBar').hide();
        $('#itemContainer .contentMenuWrapper').hide();
        $('#itemContainer .itemWrapper').remove();
        $('#itemContainer .mailContentWrapper').remove();
        $('#itemContainer .simpleWrapper').remove();
        if (true !== skip_folder_filter)
            folderFilter.hide();
    }

    function markFolderAsChanged(folder_id) {
        var folder_name = TMMail.GetSysFolderNameById(folder_id, null);
        var $visible_table_anchor = $('.messages:visible').attr('anchor');
        if (folder_name != null) {
            $('.messages[anchor^="' + folder_name + '"]').attr('changed', 'true');
            $('.messages[anchor^="' + folder_name + '"][anchor!="' + $visible_table_anchor + '"]').remove();
        }
        filterCache.drop(folder_id);
    }

    function getConversationMessages() {
        return $('.itemWrapper:visible .message-wrap');
    }

    function setTag(tag_id, message_ids) {
        if (!TMMail.pageIs('sysfolders') && !message_ids) {
            
            if (TMMail.pageIs('conversation')) {
                var chain_message_id = mailBox.currentMessageId;
                setMessagesTag(tag_id, [chain_message_id]);

                var messages = getConversationMessages();
                var i, len = messages.length;
                for (i = 0; i < len; i++) {
                    var message_id = $(messages[i]).attr('message_id');
                    messagePage.setTag(message_id, tag_id);
                }
            } else
                setCurrentMessageTag(tag_id);

            // Google Analytics
            window.ASC.Mail.ga_track(
                TMMail.pageIs('message') ? ga_Categories.message : ga_Categories.createMail
                , ga_Actions.buttonClick
                , "set_tag");
        }
        else {
            // Google Analytics
            window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.buttonClick, "set_tag");

            setMessagesTag(tag_id, message_ids);
        }

        folderFilter.update();
    }

    function setCurrentMessageTag(tag_id) {
        var message_id = mailBox.currentMessageId;
        if (TMMail.pageIs('conversation')) {
            message_id = messagePage.getActualConversationLastMessageId();
        }

        messagePage.setTag(message_id, tag_id);

        if (mailBox.currentMessageId < 1)
            return;

        tagsManager.increment(tag_id);
        setTagToCachedMessageLine(tag_id, message_id);
        serviceManager.setTag([message_id], tag_id, { tag_id: tag_id, message_id: message_id });
    }

    function setMessagesTag(tag_id, message_ids, from_message) {
        if (typeof (message_ids) === 'undefined')
            message_ids = mailBox._Selection.GetIds();

        var ids = [], i, len;
        for (i = 0, len = message_ids.length; i < len; i++) {
            if (!messageHasTag(tag_id, message_ids[i])) {
                setTagToCachedMessageLine(tag_id, message_ids[i]);
                tagsManager.increment(tag_id);
                ids.push(message_ids[i]);
            }
        }

        if (ids.length > 0) {
            if (from_message == undefined || from_message == false) {
                serviceManager.setConverstationsTag(ids, tag_id, { ids: ids, tag_id: tag_id });
            } else {
                serviceManager.setTag(ids, tag_id, { tag_id: tag_id, ids: ids });
            }
        }
    }

    function unsetTag(tag_id, message_ids) {
        tagsDropdown.hide();

        if (!TMMail.pageIs('sysfolders') && !message_ids[0])
            unsetCurrentMessageTag(tag_id);
        else
            unsetMessagesTag(tag_id, message_ids);

        folderFilter.update();
    }

    function unsetConversationsTag(tag_id) {
        tagsDropdown.hide();
        var message_ids = [];
        if (TMMail.pageIs('sysfolders')) message_ids = mailBox._Selection.GetIds();
        else if (TMMail.pageIs('conversation')) message_ids = messagePage.getCurrentConversationIds();
        else message_ids.push(mailBox.currentMessageId);
        unsetMessagesTag(tag_id, message_ids, true);
    }

    function unsetMessagesTag(tag_id, message_ids, from_conversation) {
        if (typeof (message_ids) === 'undefined')
            message_ids = mailBox._Selection.GetIds();

        var ids = [];
        for (var i = 0; i < message_ids.length; i++) {
            if (messageHasTag(tag_id, message_ids[i])) {
                var messages;
                if (TMMail.pageIs('conversation')) {
                    messages = getConversationMessages();
                    var found_tags = messages.find('.tagDelete[tagid="' + tag_id + '"]');
                    if (found_tags.length == 1) {
                        var k, len = messages.length;
                        for (k = 0; k < len; k++) {
                            var message_id = $(messages[k]).attr('message_id');
                            unsetTagFromCachedMessageLine(tag_id, message_id);
                        }
                        tagsManager.decrement(tag_id);
                    }
                } else {
                    unsetTagFromCachedMessageLine(tag_id, message_ids[i]);
                    tagsManager.decrement(tag_id);
                }
                ids.push(message_ids[i]);
            }
            messagePage.unsetTag(message_ids[i], tag_id);
        }
        if (ids.length > 0) {
            if (from_conversation == undefined || from_conversation == false) {
                serviceManager.unsetTag(ids, tag_id, { ids: ids, tag_id: tag_id });
            } else {
                serviceManager.unsetConverstationsTag(ids, tag_id, { ids: ids, tag_id: tag_id });
            }
        }

        if ($.inArray(tag_id.toString(), MailFilter.getTags()) >= 0) {
            markFolderAsChanged(current_folder_id);
            updateAnchor();
        }
    }

    function unsetCurrentMessageTag(tag_id) {
        messagePage.unsetTag(mailBox.currentMessageId, tag_id);
        unsetTagFromCachedMessageLine(tag_id, mailBox.currentMessageId);

        if (mailBox.currentMessageId < 1)
            return;

        tagsManager.decrement(tag_id);
        serviceManager.unsetTag([mailBox.currentMessageId], tag_id, { ids: [mailBox.currentMessageId], tag_id: tag_id });
    }

    // __________________________________________ cached messages tags panels _________________________________________
    function getMessageTagInfo(message_id) {
        var $el = $('.message[messageid="' + message_id + '"] .subject a');
        if (0 == $el.length)
            return undefined;
        var tags_str = $el.attr('_tags');
        return { '$el': $el, 'tags_str': tags_str };
    }

    function updateTagsLine($el) {
        $el.find('.tag').add($el.find('.more-tags')).remove();
        var tags_str = $el.attr('_tags');
        if ('' == tags_str)
            return;
        var tags = [];
        var tags_ids = tags_str.split(','), tag, i;

        // Search and remove old deleted crm tags
        for (i = 0; i < tags_ids.length; i++) {
            tag = tagsManager.getTag(tags_ids[i]);
            if (tag == undefined) {
                mailBox.unsetTag(tags[i], [message_id]);
            } else {
                tags.push(tag);
            }
        }

        var $tags_markup = $.tmpl('messageItemTagsTmpl', {
            tags: tags,
            max_displayed_tags_count: max_displayed_tags_count
        });

        $el.prepend($tags_markup);
        processTagsMore($el);
    }

    function setTagToCachedMessageLine(tag_id, message_id) {
        var info = getMessageTagInfo(message_id);
        if (!info)
            return;
        if ('' === info.tags_str)
            info.tags_str = '' + tag_id;
        else
            info.tags_str += ',' + tag_id;
        info.$el.attr('_tags', info.tags_str);
        updateTagsLine(info.$el, message_id);
    }

    function unsetTagFromCachedMessageLine(tag_id, message_id) {
        var info = getMessageTagInfo(message_id);
        if (!info)
            return;
        var tags = info.tags_str.split(',');
        var new_tags_str = '';
        $.each(tags, function(index, value) {
            if (value != tag_id) {
                if ('' != new_tags_str) new_tags_str += ',';
                new_tags_str += value;
            }
        });
        info.$el.attr('_tags', new_tags_str);
        updateTagsLine(info.$el, message_id);
    }

    function messageHasTag(tag_id, message_id) {
        if (TMMail.pageIs('message') || TMMail.pageIs('conversation')) {

            var tags_panel = $('#itemContainer .head[message_id=' + message_id + '] .tags');
            if (tags_panel) {
                var del_el = tags_panel.find('.value .itemTags .tagDelete[tagid="' + tag_id + '"]');
                if (del_el.length) 
                    return true;
            }
        }
        else {
            var info = getMessageTagInfo(message_id);
            if (!info)
                return false;
            var tags = info.tags_str.split(',');
            for (var i = 0; i < tags.length; i++) {
                if (tags[i] == tag_id)
                    return true;
            }
        }
        return false;
    }

    function messageTagsIds(message_id) {
        var info = getMessageTagInfo(message_id);
        if (!info)
            return [];
        return info.tags_str.split(',');
    }

    function getSelectedMessagesUsedTags() {
        var res = [];
        var selected_ids = current_selection.GetIds();
        for (var i = 0; i < selected_ids.length; i++) {
            var message_tags_ids = messageTagsIds(selected_ids[i]);
            if (0 == i)
                res = message_tags_ids;
            else {
                var temp = [];
                $.each(res, function(index, value) {
                    if (-1 != $.inArray(value, message_tags_ids))
                        temp.push(value);
                });
                res = temp;
                if (0 == res.length)
                    return [];
            }
        }
        res = $.map(res, function(v) { return parseInt(v); });
        return res;
    }

    function onDeleteTag(e, tag_id) {
        var els = $('.message .subject a[_tags*="' + tag_id + '"]');
        $.each(els, function(index, el) {
            var message_id = $(el).closest('.message[messageid]').attr('messageid');
            if (messageHasTag(tag_id, message_id))
                unsetTagFromCachedMessageLine(tag_id, message_id);
        });
    }

    function onUpdateTag(e, tag) {
        var els = $('.message .subject a[_tags*="' + tag.id + '"]');
        $.each(els, function(index, el) {
            var $el = $(el);
            var message_id = $el.closest('.message[messageid]').attr('messageid');
            if (messageHasTag(tag.id, message_id))
                updateTagsLine($el, message_id);
        });
    }

    function setMessagesReadUnread(message_ids, as_read) {
        var status = as_read === true ? 'read' : 'unread';
        serviceManager.markMessages(message_ids, status, { status: status, messageIds: message_ids, folderId: MailFilter.getFolder() });
        serviceManager.updateFolders(ASC.Resources.Master.Resource.LoadingProcessing);
    }

    function setConversationReadUnread(message_ids, as_read) {
        var status = as_read === true ? 'read' : 'unread';
        serviceManager.markConversations(message_ids, status, { status: status, messageIds: message_ids, folderId: MailFilter.getFolder() });
        serviceManager.updateFolders(ASC.Resources.Master.Resource.LoadingProcessing);
    }

    function updateMessageImportance(id, importance) {
        var status = importance === true ? 'important' : 'normal';
        serviceManager.markMessages([id], status, { status: status, messageIds: [id], folderId: MailFilter.getFolder() });
    }

    function updateConversationImportance(id, importance) {
        var status = importance === true ? 'important' : 'normal';
        serviceManager.markConversations([id], status);
    }

    // Removes messages from the specified folder to trash. And removes them completely if we delete them from trash.
    function deleteMessages(ids, from_folder, from_conversation) {
        ASC.Mail.hintManager.hide();
        if (from_folder == TMMail.sysfolders.trash.id) {
            serviceManager.deleteMessages(ids, { fromFolder: from_folder, fromConversation: from_conversation }, {}, window.MailScriptResource.DeletionMessage);
            serviceManager.updateFolders();
        } else {
            moveMessages(ids, from_folder, TMMail.sysfolders.trash.id, from_conversation);
        }
    }

    // Removes conversations from the specified folder to trash, or removes them completely if we delete them from trash.
    function deleteConversations(ids, from_folder) {
        ASC.Mail.hintManager.hide();
        if (from_folder == TMMail.sysfolders.trash.id ||
            from_folder == TMMail.sysfolders.spam.id) {
            serviceManager.deleteConversations(ids, { fromFolder: from_folder }, {}, window.MailScriptResource.DeletionMessage);
            serviceManager.updateFolders();
        } else {
            moveConversations(ids, from_folder, TMMail.sysfolders.trash.id);
        }
    };

    function deleteConversation(id, from_folder) {
        return deleteConversations([id], from_folder);
    }

    function deleteMessage(id, from_folder, from_conversation) {
        deleteMessages([id], from_folder, from_conversation);
    }

    function moveMessages(ids, from_folder, to_folder, from_conversation) {
        ASC.Mail.hintManager.hide();
        serviceManager.moveMailMessages(ids, to_folder, { fromFolder: from_folder, toFolder: to_folder, fromConversation: from_conversation }, {}, window.MailScriptResource.MovingMessages);
        serviceManager.updateFolders();
    }

    function moveMessage(id, from_folder, to_folder, from_conversation) {
        return moveMessages([id], from_folder, to_folder, from_conversation);
    }

    function moveConversations(ids, from_folder, to_folder) {
        ASC.Mail.hintManager.hide();
        serviceManager.moveMailConversations(ids, to_folder, { fromFolder: from_folder, toFolder: to_folder }, {}, window.MailScriptResource.MovingMessages);
        serviceManager.updateFolders();
    }

    function moveConversation(id, from_folder, to_folder) {
        return moveConversations([id], from_folder, to_folder);
    }

    function restoreMessages(ids) {
        ASC.Mail.hintManager.hide();
        var folder_ids_for_update = new TMContainers.IdMap();

        for (var i = 0; i < ids.length; i++) {
            var prevfolderid = $('div[messageid="' + ids[i].messageId + '"]').attr('prevfolderid');
            folder_ids_for_update.AddId(prevfolderid);
        }
        serviceManager.restoreMailMessages(ids, { folders: folder_ids_for_update }, {}, window.MailScriptResource.RestoringMessages);
        serviceManager.updateFolders();
    }

    function restoreConversations(ids) {
        ASC.Mail.hintManager.hide();
        serviceManager.restoreMailConversations(ids, {
            folders: [
                TMMail.sysfolders.inbox.id,
                TMMail.sysfolders.sent.id,
                TMMail.sysfolders.drafts.id,
                TMMail.sysfolders.trash.id,
                TMMail.sysfolders.spam.id
            ]
        }, {}, window.MailScriptResource.RestoringMessages);
        serviceManager.updateFolders();
    }

    function hidePages() {
        tagsPage.hide();
        accountsPage.hide();
        contactsPage.hide();
        blankPages.hide();
        helpPage.hide();
    }

    function onRemoveMailFolderMessages(params, folder) {
        if ((folder == TMMail.sysfolders.trash.id && TMMail.pageIs('trash'))
            || (folder == TMMail.sysfolders.spam.id && TMMail.pageIs('spam'))
            || (MailFilter.getFolder() == folder && TMMail.pageIs('message'))) {
            serviceManager.updateFolders();
        } else {
            markFolderAsChanged(folder);
        }
        TMMail.showCompleteActionHint(TMMail.action_types.clear_folder, null, null, folder);
    }

    function showFolder(id, $html) {
        messagePage.hide();
        hidePages();

        if (!accountsManager.any()) {
            folderFilter.hide();
            blankPages.showEmptyAccounts();
            return;
        }

        $('#itemContainer').height('auto');

        var cur_scroll = $(window).scrollTop();
        hideContentDivs(true);

        if ($html.find('[messageid]').length) {
            $html.show();
            blankPages.hide();
            folderFilter.show();
            changeTagStyle();
            $('#itemContainer .contentMenuWrapper').show();
            itemsHandler();
            folderFilter.update();
            redrawNavigation($html.attr('has_next') ? true : false, $html.attr('has_prev') ? true : false);
        } else {
            $('#MessagesListGroupButtons').hide();
            if (MailFilter.isBlank()) {
                folderFilter.hide();
                blankPages.showEmptyFolder();
            }
            else {
                folderFilter.show();
                blankPages.showNoLettersFilter();
            }
        }

        $(window).scrollTop(cur_scroll);

        TMMail.setPageHeaderFolderName(id);
    }

    function onGetMailConversations(params, messages) {
        if (undefined == messages.length)
            return;

        hideLoadingMask();

        var has_next = (true === MailFilter.getPrevFlag()) || params.__total > MailFilter.getPageSize();
        var has_prev = (false === MailFilter.getPrevFlag() && null != MailFilter.getFromDate() && undefined != MailFilter.getFromDate()) || (true === MailFilter.getPrevFlag() && params.__total > MailFilter.getPageSize());

        if ((0 == messages.length && MailFilter.getFromDate()) || (false == has_prev && true == MailFilter.getPrevFlag())) {
            MailFilter.setFromDate(null);
            MailFilter.setPrevFlag(false);
            MailFilter.setFromMessage(0);
            keep_selection_on_reload = true;
            var new_anchor = "#" + TMMail.GetSysFolderNameById(MailFilter.getFolder()) + MailFilter.toAnchor(true);
            ASC.Controls.AnchorController.move(new_anchor);
        }
        if (params.folder_id != MailFilter.getFolder())
            return;
        var filter = MailFilter.toAnchor(true);
        var folder = TMMail.GetSysFolderNameById(MailFilter.getFolder());
        var anchor = folder + filter;
        anchor = anchor.replace(/'/g, "%27");
        if ($('#itemContainer .messages[anchor="' + anchor + '"]').length) {
            $('#itemContainer .messages[anchor="' + anchor + '"]').remove();
        }

        var potential_alerts_flag = false;

        var tags_ids = $.map(tagsManager.getAllTags(), function(value) { return value.id; });

        $.each(messages, function(i, m) {
            if (params.folder_id == TMMail.sysfolders.sent.id || params.folder_id == TMMail.sysfolders.drafts.id) {
                m.sender = m.to;
            } else {
                m.sender = m.from;
            }

            m.sender = m.sender ? m.sender.replace(/^\s+|\s+$/g, "") : '';
            m.author = m.sender.split('<')[0].replace(/\"/g, " ").replace(/^\s+|\s+$/g, "");

            m.displayDate = window.ServiceFactory.getDisplayDate(window.ServiceFactory.serializeDate(m.receivedDate));
            m.displayTime = window.ServiceFactory.getDisplayTime(window.ServiceFactory.serializeDate(m.receivedDate));

            if ("mail-daemon@teamlab.com" == m.sender)
                potential_alerts_flag = true;

            // remove tags wich doesn't exist
            m.tagIds = $.grep(m.tagIds, function(v) {
                if (0 > $.inArray(v, tags_ids)) {
                    mailBox.unsetTag(v, m.id);
                    return false;
                }
                return true;
            });

            // add tags objects array
            m.tags = [];
            $.each(m.tagIds, function (index, id) {
                var tag = tagsManager.getTag(id);
                if (tag != undefined)
                    m.tags.push(tag);
            });

            m.subject = TMMail.ltgt(m.subject) || '';

            if (m.folder == TMMail.sysfolders.drafts.id)
                m.anchor = '#draftitem/' + m.id;
            else
                m.anchor = '#conversation/' + m.id;
        });

        if (true === potential_alerts_flag)
            mailAlerts.check();

        var html = $.tmpl(
            'messagesTmpl',
            {
                messages: messages,
                has_next: has_next,
                has_prev: has_prev,
                max_displayed_tags_count: max_displayed_tags_count
            });
        // HTML is set here
        var $html = $(html);
        $html.hide();
        $html.attr('anchor', anchor);

        processTagsMore($html);

        if ($html.find('[messageid]').length) {
            $('#itemContainer .contentMenuWrapper.messagesList').after($html);
        }

        if (TMMail.pageIs('sysfolders'))
            showFolder(params.folder_id, $html);

        stickActionMenuToTheTop();
        updateGroupButtonsComposition();
        updateView();
        updateSelectionView();
        commonLettersButtonsState();
    }

    function processTagsMore($html) {
        $html.find('.more-tags').unbind('.processTagsMore').bind('click.processTagsMore', function (event) {
            var $this = $(this);
            $this.unbind('.processTagsMore');
            var tags_ids = $this.parent().attr('_tags').split(',');
            var buttons = [];
            for (var i = max_displayed_tags_count; i < tags_ids.length; i++) {
                var tag = tagsManager.getTag(tags_ids[i]);
                if (tag)
                    buttons.push({ 'text': TMMail.ltgt(tag.name), 'disabled': true });
            }
            $this.actionPanel({ 'buttons': buttons, 'show': true });
            event.preventDefault();
        });
    }

    function onGetMailMessagesModifyDate(params, date) {
        if (TMMail.messages_modify_date == date)
            return;

        // don't update tags list at first time modify date recieved
        if (TMMail.messages_modify_date - (new Date(0)) != 0)
            serviceManager.getTags();

        TMMail.messages_modify_date = date;
    }

    function redrawNavigation(has_next, has_prev) {
        PagesNavigation.RedrawFolderNavigationBar(window.mailPageNavigator,
            TMMail.option('MessagesPageSize'),
            onChangePageSize,
            has_next,
            has_prev);
        PagesNavigation.RedrawPrevNextControl();
    }

    function onChangePageSize(page_size) {
        if (isNaN(page_size) || page_size < 1) return;
        TMMail.option('MessagesPageSize', page_size);
        MailFilter.setPageSize(page_size);

        keep_selection_on_reload = true;
        updateAnchor(true);
    }

    function actionPanelSelect(selector_suffix) {
        $('.messages:visible .message' + selector_suffix).each(function() {
            selection.AddId($(this).attr('messageid'));
        });

        updateSelectionView();
        commonLettersButtonsState();
        hidePages();
    }

    function actionPanelSelectAll() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'all_select');

        $('.messages:visible .message').each(function() {
            selection.AddId($(this).attr('messageid'));
        });

        last_selected_concept = selection_concept.all;
        updateSelectionView();
        commonLettersButtonsState();
        hidePages();
    }

    function actionPanelSelectNone() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'none_select');

        $('.messages:visible .message').each(function() {
            selection.RemoveId($(this).attr('messageid'));
        });

        last_selected_concept = null;
        updateSelectionView();
        commonLettersButtonsState();
        hidePages();
    }

    function actionPanelSelectUnread() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'unread_select');

        actionPanelSelectNone();
        last_selected_concept = selection_concept.unread;
        actionPanelSelect('.new');
    }

    function actionPanelSelectRead() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'read_select');

        actionPanelSelectNone();
        last_selected_concept = selection_concept.read;
        actionPanelSelect(':not(.new)');
    }

    function actionPanelSelectImportant() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'important_select');

        actionPanelSelectNone();
        last_selected_concept = selection_concept.important;
        actionPanelSelect(':has(.icon-important)');
    }

    function actionPanelSelectWithAttachments() {
        // google analitics track
        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.actionClick, 'whith_atachments_select');

        actionPanelSelectNone();
        last_selected_concept = selection_concept.with_attachments;
        actionPanelSelect(':has(.icon-attachment)');
    }

    function itemsHandler() {
        var $messages = $('.messages:visible .message');

        // set message importance flag click handler
        $messages.find('.importance').unbind('click').bind('click', function() {
            var $this = $(this);
            var icon = $this.find('[class^="icon-"], [class*=" icon-"]');
            var newimportance = icon.is('.icon-unimportant');
            mailBox.updateConversationImportance($this.parent().attr('messageid'), newimportance);
            icon.toggleClass('icon-unimportant').toggleClass('icon-important');
        });

        // _Selection checkbox clicked
        $messages.find('.checkbox').unbind('click').bind('click', function() {
            var $this = $(this);
            var parent = $this.parent();
            var message_id = parent.attr('messageid');
            if (parent.is('.selected')) {
                selection.RemoveId(message_id);
                current_selection.RemoveId(message_id);
                $this.find('input').prop('checked', false);
            } else {
                selection.AddId(message_id);
                current_selection.AddId(message_id);
                $this.find('input').prop('checked', true);
            }
            parent.toggleClass('selected');
            var messages_table = parent.parent();
            updateSelectionComboCheckbox(messages_table);
            last_selected_concept = null;
            updateOverallSelectionLine();
            commonLettersButtonsState();
        });
    }

    function groupButtonsMenuHandlers() {
        // Delete (group button)
        $('#MessagesListGroupButtons .menuActionDelete').click(function() {
            if ($(this).hasClass('unlockAction')) {
                var messages = current_selection.GetIds();

                if (messages.length > 0) {
                    deleteConversations(messages, current_folder_id);
                }
            }
        });

        // Spam (group button)
        $('#MessagesListGroupButtons .menuActionSpam').click(function() {
            if ($(this).hasClass('unlockAction')) {
                var messages = current_selection.GetIds();

                if (messages.length > 0) {
                    moveConversations(messages, current_folder_id, TMMail.sysfolders.spam.id);
                }
            }
        });

        // NotSpam (group button)
        $('#MessagesListGroupButtons .menuActionNotSpam').click(function() {
            if ($(this).hasClass('unlockAction')) {
                var messages = current_selection.GetIds();

                if (messages.length > 0) {
                    restoreConversations(messages);
                }
            }
        });

        // Read/Unread (group button)
        $('#MessagesListGroupButtons .menuActionRead').click(function() {
            if ($(this).hasClass('unlockAction')) {
                var message_ids = current_selection.GetIds();

                if (message_ids.length > 0) {
                    var read = ($(this).attr('do') == 'read') ? true : false;
                    setConversationReadUnread(message_ids, read);
                }
            }
        });

        // Restore (group button)
        $('#MessagesListGroupButtons .menuActionRestore').click(function() {
            if ($(this).hasClass('unlockAction')) {
                var messages = current_selection.GetIds();
                restoreConversations(messages);
            }
        });

        // Add tag (group button)
        $('#MessagesListGroupButtons .menuActionAddTag').click(function() {
            if ($(this).hasClass('unlockAction'))
                tagsDropdown.show($(this));
        });

        // Select all
        $('#SelectAllMessagesCB').unbind('click');
        $('#SelectAllMessagesCB').bind('click', function(e) {
            if (e.target.checked) {
                actionPanelSelectAll();
            } else {
                actionPanelSelectNone();
            }
            e.stopPropagation();
            $('#SelectAllMessagesDropdown').parent().actionPanel('hide');
        });

        // _Selection combo-checkbox dropdown
        $('#SelectAllMessagesDropdown').parent().actionPanel({ buttons: [
            { text: window.MailScriptResource.AllLabel, handler: actionPanelSelectAll },
            { text: window.MailScriptResource.FilterUnread, handler: actionPanelSelectUnread },
            { text: window.MailScriptResource.FilterRead, handler: actionPanelSelectRead },
            { text: window.MailScriptResource.ImportantLabel, handler: actionPanelSelectImportant },
            { text: window.MailScriptResource.WithAttachments, handler: actionPanelSelectWithAttachments },
            { text: window.MailScriptResource.NoneLabel, handler: actionPanelSelectNone }
        ], css: 'stick-over'});

        $('#OverallDeselectAll').click(function() {
            overallDeselectAll();
        });
    }

    function updateGroupButtonsComposition(folder_id) {
        $('#MessagesListGroupButtons').show();

        if (folder_id === undefined)
            folder_id = MailFilter.getFolder();

        // Update selection combo-checkbox disabled state
        var $combo_checkbox = $('#SelectAllMessagesCB');
        if ($('.messages:visible .message').length == 0) {
            $combo_checkbox.attr('disabled', 'true');
            $('#SelectAllMessagesDropdown').attr('disabled', 'true');
        } else {
            $combo_checkbox.removeAttr('disabled');
            $('#SelectAllMessagesDropdown').removeAttr('disabled');
        }
        // Spam / Not spam
        $('#MessagesListGroupButtons .menuActionNotSpam').toggle(folder_id == TMMail.sysfolders.spam.id);
        $('#MessagesListGroupButtons .menuActionSpam').toggle(folder_id == TMMail.sysfolders.inbox.id);

        // Read / unread
        $('#MessagesListGroupButtons .menuActionRead').toggle(folder_id != TMMail.sysfolders.drafts.id);

        // Restore
        $('#MessagesListGroupButtons .menuActionRestore').toggle(folder_id == TMMail.sysfolders.trash.id);

        updateOverallSelectionLine();
    }

    function commonLettersButtonsState() {
        if (current_selection.Count() != 0) {
            $('#MessagesListGroupButtons .menuAction').addClass('unlockAction');
        } else {
            $('#MessagesListGroupButtons .menuAction').removeClass('unlockAction');
        }

        /* read/unread */

        var read_unread = false;

        $('.messages .message.new').each(function() {
            var $this = $(this);
            var messageid = $this.attr('messageid');
            if (current_selection.HasId(messageid)) {
                read_unread = true;
                return false;
            }
            return true;
        });

        var $read_btn = $('#MessagesListGroupButtons .menuActionRead');
        if (read_unread) {
            $read_btn.attr('do', 'read');
            $read_btn.html('<span title="' + $read_btn.attr('read') + '">' + $read_btn.attr('read') + '</span>');
        } else {
            $read_btn.attr('do', 'unread');
            $read_btn.html('<span title="' + $read_btn.attr('unread') + '">' + $read_btn.attr('unread') + '</span>');
        }
    }

// ReSharper disable UnusedParameter
    function onMarkMailMessages(params, ids) {
// ReSharper restore UnusedParameter
        hidePages();

        var $all_main_tables = $('.messages');
        var tables_idx = $all_main_tables.length;
        while (tables_idx--) {
            var $main_table = $($all_main_tables[tables_idx]);
            var table_anchor = $main_table.attr('anchor');
            if (MailFilter.anchorHasMarkStatus(params.status, table_anchor)) {
                $main_table.attr('changed', 'true');
                // Visible table must be updated right now
                if ($main_table.is(':visible')) {
                    updateAnchor();
                }
            }
            // No need to update a table that has no marking status in filter (anchor)
            else {
                markMessageDivsInTable($main_table, params.status, params.messageIds);
            }
        }

        if (!TMMail.pageIs('message')) {
            commonLettersButtonsState();
        }

        if (params.status == 'read' || params.status == 'unread') {
            serviceManager.getMailFolders();
        }
    }

    // prev next ids responces handlers
    // if zero returned - move to folder anchor
    function onGetNextPrevId (method, id) {
        if (0 != id)
            method.apply(TMMail, [id]);
        else {
            var filter = MailFilter.toAnchor(false);
            var folder = TMMail.GetSysFolderNameById(MailFilter.getFolder());
            ASC.Controls.AnchorController.move(folder + filter);
        }
    };

    function onGetNextPrevConversationId (params, id) {
        onGetNextPrevId(TMMail.moveToConversation, id);
    };

    function onGetNextPrevMessageId (params, id) {
        onGetNextPrevId(TMMail.moveToMessage, id);
    };

    function markMessageDivsInTable ($table, status, message_ids) {
        var 
          $messages = $table.find('.message'),
          div_index = $messages.length;
        while (div_index--) {
            var $message = $($messages[div_index]);
            var message_id = $message.attr('messageid');
            var message_index = message_ids.length;
            while (message_index--) {
                if (message_id === message_ids[message_index]) {
                    setStatusToMessage($message, status);
                    break;
                }
            }
        }
    }

    function setStatusToMessage(el, status) {
        if (status == 'read') {
            el.removeClass('new');
        }
        else if (status == 'unread') {
            el.addClass('new');
        }
        else if (status == 'important') {
            el.find('[class^="icon-"], [class*=" icon-"]').attr('class', 'icon-important');
        }
        else if (status == 'normal') {
            el.find('[class^="icon-"], [class*=" icon-"]').attr('class', 'icon-unimportant');
        }
    }

    function onRestoreMailMessages(params, ids) {
        // TODO: add a check on the inclusion of chains (is_conversation param)
        if (ids.length == 1) {
            var restore_folder_id = undefined;
            if (TMMail.pageIs('trash') || TMMail.pageIs('spam'))
                restore_folder_id = parseInt($('#itemContainer .messages .message[messageid="' + ids[0] + '"]').attr('prevfolderid'));
            else if (TMMail.pageIs('conversation') || TMMail.pageIs('message'))
                restore_folder_id = parseInt($('#itemContainer .message-wrap[message_id="' + ids[0] + '"]').attr('restore_folder_id'));

            if ($.isNumeric(restore_folder_id))
                TMMail.showCompleteActionHint(TMMail.action_types.restore, true, ids.length, restore_folder_id);
        } else {
            TMMail.showCompleteActionHint(TMMail.action_types.restore, true, ids.length);
        }

        $.each(params.folders, function(index, value) {
            markFolderAsChanged(value);
        });

        updateView();
        updateAnchor(true);
        // Clear checkboxes
        selection.RemoveIds(ids);
        last_selected_concept = null;
        // Update buttons depending on checkboxes state
        commonLettersButtonsState();
    }

    function onRemoveMailMessages(params, ids) {
        // Mark the source folder to be updated (unnecessary to update right now)
        markFolderAsChanged(params.fromFolder);
        if (params.fromConversation == undefined || !params.fromConversation) {
            if (moveNextIfPossible()) {
                messagePage.conversation_deleted = true;
                return;
            }
            updateView();
            updateAnchor(true);
            selection.RemoveIds(ids); // Clear checkboxes
            last_selected_concept = null;
            commonLettersButtonsState(); // Update buttons depending on checkboxes state
            // TODO: add a check on the inclusion of chains (is_conversation param)
            TMMail.showCompleteActionHint(TMMail.action_types.delete_messages, true, ids.length);
        } else {
            for (var i = 0; i < ids.length; i++) {
                var deleted_message = $('#itemContainer div.message-wrap[message_id=' + ids[i] + ']');
                if (deleted_message != undefined) {
                    var conversation_messages = $('#itemContainer div.message-wrap');

                    if (conversation_messages.length > 1)
                        deleted_message.remove();
                    else {
                        params.fromConversation = false;
                        onRemoveMailMessages(params, ids);
                    }
                }
            }
        }
    }

    function onMoveMailMessages(params, ids) {
        // Mark the source folder to be updated (unnecessary to update right now)
        markFolderAsChanged(params.fromFolder);
        // Mark the destination folder to be updated (unnecessary to update right now)
        markFolderAsChanged(params.toFolder);

        // Clear checkboxes
        selection.RemoveIds(ids);
        last_selected_concept = null;

        // if not conversation or single message in conversation
        if (!params.fromConversation || $('#itemContainer div.message-wrap').length <= 1) {
            updateAnchor(true);
            // Update buttons depending on checkboxes state
            commonLettersButtonsState();
            return;
        }

        $.each(ids, function(i, v) {
            var deleted_message = $('#itemContainer div.message-wrap[message_id=' + v + ']');
            deleted_message && deleted_message.remove();
        });

        // if main conversation message was removed - fix anchor
        if (-1 != $.inArray(+mailBox.currentMessageId, ids)) {
            mailBox.currentMessageId = messagePage.getActualConversationLastMessageId();
            TMMail.moveToConversation(mailBox.currentMessageId, true);
        }

        TMMail.showCompleteActionHint(TMMail.action_types.move, false, ids.length, params.toFolder);
    }

    function onMoveMailConversations(params, ids) {
        // Mark the source folder to be updated (unnecessary to update right now)
        markFolderAsChanged(params.fromFolder);
        // Mark the destination folder to be updated (unnecessary to update right now)
        markFolderAsChanged(params.toFolder);
        if (moveNextIfPossible()) {
            messagePage.conversation_moved = true;
            messagePage.dst_folder_id = params.toFolder;
            return;
        }
        // Clear checkboxes
        selection.RemoveIds(ids);
        last_selected_concept = null;

        updateAnchor(true);
        // Update buttons depending on checkboxes state
        commonLettersButtonsState();

        TMMail.showCompleteActionHint(TMMail.action_types.move, true, ids.length, params.toFolder);
    }

    function moveNextIfPossible() {
        var next_message_link = $('.itemWrapper .pagerNextButtonCSSClass:visible');
        if ((TMMail.pageIs('conversation') || TMMail.pageIs('message')) && next_message_link.length > 0) {
            var next_conversation_link = next_message_link.prop('href');
            if (next_conversation_link != undefined && next_conversation_link.length > 0) {
                window.open(next_conversation_link, '_self');
                return true;
            }
        }

        return false;
    }

    // Default request if no anchor is entered
    function onInboxPage() {
        if (TMMail.isInvalidPage()) TMMail.moveToInbox();
    }

    function onSysFolderPage(folder_name, params) {
        ASC.Mail.hintManager.hide();
        current_folder_id = TMMail.GetSysFolderIdByName(folder_name, TMMail.sysfolders.inbox.id);

        if (current_folder_id != folderPanel.getMarkedFolder())
            unmarkAllPanels(current_folder_id == MailFilter.getFolder());

        // check al least one account exists
        // if not - shows stub page
        if (!accountsManager.any()) {
            hidePages();
            folderFilter.hide();
            messagePage.hide();
            hideContentDivs();
            blankPages.showEmptyAccounts();
            folderPanel.markFolder(current_folder_id);
            TMMail.setPageHeaderFolderName(current_folder_id);
            hideLoadingMask();
            return;
        }

        folderPanel.markFolder(current_folder_id);

        $(document).off('click', 'a[href]');

        // checks weather page size value in anchor is correct - replace anchor if not
        if (PagesNavigation.FixAnchorPageSizeIfNecessary(MailFilter.getPageSize()))
            return;

        // store page size setting for next session
        TMMail.option('MessagesPageSize', MailFilter.getPageSize());

        if (!keep_selection_on_reload) {
            overallDeselectAll();
        } else {
            keep_selection_on_reload = false;
        }

        var anchor = ASC.Controls.AnchorController.getAnchor().replace(/'/g, "%27");
        var cache = $('#itemContainer .messages[anchor="' + anchor + '"][changed!="true"]');

        var filter = MailFilter.toAnchor(false, {}, true);
        var folder = TMMail.GetSysFolderNameById(MailFilter.getFolder());

        MailFilter.fromAnchor(folder_name, params);

        // if no cash exists - get filtered conversations
        if (0 == cache.length) {
            serviceManager.updateFolders(ASC.Resources.Master.Resource.LoadingProcessing);
            return;
        }

        hidePages();

        if (current_folder_id != folderPanel.getMarkedFolder())
            unmarkAllPanels();

        messagePage.hide();
        hideContentDivs();
        $('#itemContainer .contentMenuWrapper').show();
        folderFilter.show();
        cache.show();
        stickActionMenuToTheTop();
        updateGroupButtonsComposition();
        updateSelectionView();
        commonLettersButtonsState();

        if (anchor.indexOf("page_size") > -1)
            anchor = anchor.substring(0, anchor.indexOf("page_size"));
        else
            if (anchor.indexOf("from_date") > -1)
                anchor = anchor.substring(0, anchor.indexOf("from_date"));

        if (anchor != folder + filter) {
            updateView();
        }

        redrawNavigation(cache.attr('has_next') ? true : false, cache.attr('has_prev') ? true : false);
        folderPanel.markFolder(current_folder_id);
        TMMail.setPageHeaderFolderName(current_folder_id);
        itemsHandler();
    }

    function updateView() {
        markTags();
        accountsPanel.mark();
        accountsPanel.updateAnchors();
        folderFilter.update();

        folderFilter.setUnread(MailFilter.getUnread());
        folderFilter.setImportance(MailFilter.getImportance());
        folderFilter.setAttachments(MailFilter.getAttachments());
        folderFilter.setFrom(MailFilter.getFrom());
        folderFilter.setTo(MailFilter.getTo());
        folderFilter.setPeriod(MailFilter.getPeriod());
        folderFilter.setSearch(MailFilter.getSearch());
        folderFilter.setTags(MailFilter.getTags());
        folderFilter.setSort(MailFilter.getSort(), MailFilter.getSortOrder());

    }

    function markTags() {
        tagsPanel.unmarkAllTags();
        $.each(MailFilter.getTags(), function(index, value) {
            tagsPanel.markTag(value);
        });
    }

    function onCompose() {
        mailBox.currentMessageId = -1;
        messagePage.onCompose();
    }

    function onComposeTo(params) {
        mailBox.currentMessageId = -1;
        if (params) {
            var emails = TMMail.getParamsValue(params, /email=([^\/]+)/);
            if (emails) messagePage.onComposeTo(emails.split(','));
            else {
                var ids = TMMail.getParamsValue(params, /crm=([^\/]+)/);
                if (ids) {
                    serviceManager.getCrmContactsById({}, { contactids: ids.split(',') }, { success: onGetCrmContacts },
                        ASC.Resources.Master.Resource.LoadingProcessing);
                } else messagePage.onComposeTo();
            }
        }
        else messagePage.onComposeTo();
    }

    function onGetCrmContacts(params, resp) {
        var addresses = [];
        for (var i = 0; i < resp.length; i++) {
            if (resp[i].email.data) {
                var address = '';
                if (resp[i].displayName != '') address = '"' + resp[i].displayName + '" ';
                address += '<' + resp[i].email.data + '>';
                addresses.push(address);
            }
        }
        messagePage.onComposeTo(addresses);
    }

    function onReplyPage(id) {
        mailBox.currentMessageId = -1;
        messagePage.reply(id);
    }

    function onReplyAllPage(id) {
        mailBox.currentMessageId = -1;
        messagePage.replyAll(id);
    }

    function onForwardPage(id) {
        mailBox.currentMessageId = -1;
        messagePage.forward(id);
    }

    function onDraftItemPage(id) {
        messagePage.edit(id);
    }

    function onMessagePage(id) {
        mailBox.currentMessageId = id;
        setUnreadInCache(id);
        messagePage.view(id, false);
    }

    function onNextMessagePage (id) {
        serviceManager.getNextMessageId(id);
    };

    function onPrevMessagePage (id) {
        serviceManager.getPrevMessageId(id);
    };

    function onConversationPage (id) {
        mailBox.currentMessageId = id;
        setUnreadInCache(id);
        messagePage.conversation(id, false);
    };

    function onNextConversationPage (id) {
        var cached_id = filterCache.getNextConversation(MailFilter, id);
        if (0 == cached_id)
            serviceManager.getNextConversationId(id);
        else
            onGetNextPrevConversationId({}, cached_id);
    };

    function onPrevConversationPage (id) {
        var cached_id = filterCache.getPrevConversation(MailFilter, id);
        if (0 == cached_id)
            serviceManager.getPrevConversationId(id);
        else
            onGetNextPrevConversationId({}, cached_id);
    };

    function onTagsPage () {
        messagePage.hide();
        hidePages();
        unmarkAllPanels();
        TMMail.setPageHeaderTitle(window.MailScriptResource.Tags);
        hideContentDivs();
        tagsPage.show();
        settingsPanel.selectItem('tagsSettings');
        hideLoadingMask();
    };

    function onAccountsPage() {
        messagePage.hide();
        unmarkAllPanels();
        hidePages();
        TMMail.setPageHeaderTitle(window.MailScriptResource.AccountsLabel);
        hideContentDivs();
        accountsPage.show();
        settingsPanel.selectItem('accountsSettings');
        hideLoadingMask();
    }

    function onTeamLabContactsPage() {
        TMMail.setPageHeaderTitle(window.MailScriptResource.TeamLabContactsLabel);
        if (TMMail.availability.People) contactsPage.show('teamlab', 1);
        else TMMail.moveToInbox();
    }

    function onCrmContactsPage() {
        TMMail.setPageHeaderTitle(window.MailScriptResource.CRMContactsLabel);
        if (TMMail.availability.CRM) contactsPage.show('crm', 1);
        else TMMail.moveToInbox();
    }

    function onHelpPage(helpId) {
        TMMail.setPageHeaderTitle(window.MailScriptResource.HelpCenterLabel);
        messagePage.hide();
        unmarkAllPanels();
        hidePages();
        hideContentDivs();
        helpPanel.selectItem(helpId);
        helpPage.show(helpId);
    }

    function updateAnchor(include_paging_info, keep_selection) {
        if (keep_selection) keep_selection_on_reload = true;
        var filter = MailFilter.toAnchor(include_paging_info),
            folder = TMMail.GetSysFolderNameById(MailFilter.getFolder()),
            anchor = ASC.Controls.AnchorController.getAnchor(),
            new_anchor = (folder + filter).replace(/'/g, "%27");
        if (anchor != new_anchor) ASC.Controls.AnchorController.move(new_anchor);
    }

    function setUnreadInCache(message_id) {
        $('#itemContainer .messages .message[messageid="' + message_id + '"]').removeClass('new');
    }

    function setImportanceInCache(message_id) {
        var importance = $('#itemContainer .messages .message[messageid="' + message_id + '"] .importance');
        if (importance.length > 0) {
            var flag = $(importance).find('[class^="icon-"], [class*=" icon-"]');
            flag.toggleClass('icon-unimportant').toggleClass('icon-important');
        }
    }

    function hideLoadingMask() {
        if (!page_is_loaded) {
            //remove loading mask element
            $('#loading-mask').remove();
            $('body').css('overflow', 'auto');
            page_is_loaded = true;
        }
    }

    function keepSelection(value) {
        keep_selection_on_reload = value;
    }

    return {
        currentMessageId: current_message_id,

        init: init,
        setMessagesReadUnread: setMessagesReadUnread,
        setConversationReadUnread: setConversationReadUnread,
        moveMessage: moveMessage,
        moveConversation: moveConversation,
        deleteMessage: deleteMessage,
        deleteConversation: deleteConversation,
        updateMessageImportance: updateMessageImportance,
        updateConversationImportance: updateConversationImportance,
        setTag: setTag,
        unsetTag: unsetTag,
        unsetConversationsTag: unsetConversationsTag,
        updateAnchor: updateAnchor,
        hideContentDivs: hideContentDivs,
        hidePages: hidePages,
        unmarkAllPanels: unmarkAllPanels,
        markFolderAsChanged: markFolderAsChanged,
        getMessagesAddresses: getMessagesAddresses,
        onChangePageSize: onChangePageSize,
        restoreConversations: restoreConversations,
        restoreMessages: restoreMessages,
        getSelectedMessagesUsedTags: getSelectedMessagesUsedTags,
        _Selection: current_selection,
        groupButtonsMenuHandlers: groupButtonsMenuHandlers,
        hideLoadingMask: hideLoadingMask,
        keepSelection: keepSelection,
        setImportanceInCache: setImportanceInCache,
        getConversationMessages: getConversationMessages
    };

})(jQuery);