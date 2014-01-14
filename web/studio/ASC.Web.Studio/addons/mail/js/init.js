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

(function($) {
    var addNewMailbox = (function() {
        if ($.browser.msie && $.browser.version < 8) {
            return function($textfield, mailbox) {
                if ($textfield.length > 0) {
                    if ($textfield.is('input')) {
                        $textfield.val(mailbox);
                    } else if ($textfield.is('textarea')) {
                        var val = $textfield.val().split(',');
                        val[val.length - 1] = mailbox;
                        $textfield.val(val.join(',') + ', ');
                        $textfield.focus();
                    }
                }
            };
        }
        return function($textfield, mailbox) {
            if ($textfield.length > 0) {
                if ($textfield.is('input')) {
                    $textfield.val(mailbox);
                } else if ($textfield.is('textarea')) {
                    var val = $textfield.val().split(/\s*,\s*/);
                    val[val.length - 1] = mailbox;
                    $textfield.val(val.join(', ') + ', ');
                    $textfield.focus();
                }
            }
        };
    })();

    var _is_init = false;

    function _onGetMailFolders() {
        if (_is_init) return;

        _is_init = true;

        filterCache.init();
        mailBox.init();
        folderFilter.init();
        contactsPage.init();
        contactsPanel.init();
        contactsManager.init();
        CrmLinkPopup.init();

        var current_anchor = ASC.Controls.AnchorController.getAnchor();
        ASC.Controls.AnchorController.move(current_anchor != "" ? current_anchor : TMMail.sysfolders.inbox.name);

        $('#createNewMailBtn').click(function(e) {
            if (e.isPropagationStopped())
                return;
            CreateNewMail();
        });

        $('#check_email_btn').click(function(e) {
            if (e.isPropagationStopped())
                return;

            if (!accountsManager.any()) return;

            if (!TMMail.pageIs('inbox')) {
                mailBox.unmarkAllPanels();
                ASC.Controls.AnchorController.move(TMMail.sysfolders.inbox.name);
            }
            serviceManager.updateFolders(ASC.Resources.Master.Resource.LoadingProcessing);
        });

        $('#settingsLabel').click(function() {
            var $settingsPanel = $(this).parents('.menu-item.sub-list');
            if ($settingsPanel.hasClass('open'))
                $settingsPanel.removeClass('open');
            else $settingsPanel.addClass('open');
        });

        $('#foldersContainer').find('a[folderid="1"]').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "inbox");
        $('#foldersContainer').find('a[folderid="2"]').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "sent");
        $('#foldersContainer').find('a[folderid="3"]').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "drafts");
        $('#foldersContainer').find('a[folderid="4"]').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "trash");
        $('#foldersContainer').find('a[folderid="5"]').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "spam");
        $('#teamlab').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "teamlab-contacts");
        $('#crm').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "crm-contacts");
        $('#accountsSettings').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "accounts-settings");
        $('#tagsSettings').trackEvent(ga_Categories.leftPanel, ga_Actions.quickAction, "tags-settings");
        $('#MessagesListGroupButtons .menuActionDelete').trackEvent(ga_Categories.folder, ga_Actions.buttonClick, "delete");
        $('#MessagesListGroupButtons .menuActionSpam').trackEvent(ga_Categories.folder, ga_Actions.buttonClick, "spam");
        $('#MessagesListGroupButtons .menuActionRead').trackEvent(ga_Categories.folder, ga_Actions.buttonClick, "read");
        $('#MessagesListGroupButtons .menuActionNotSpam').trackEvent(ga_Categories.folder, ga_Actions.buttonClick, "not-spam");
        $('#MessagesListGroupButtons .menuActionRestore').trackEvent(ga_Categories.folder, ga_Actions.buttonClick, "not-spam");


        mailBox.groupButtonsMenuHandlers();

        if (accountsManager.getAccountList().length > 0 && window.blankModal != undefined)
            window.blankModal.close();
    }


    $(function() {
        TMMail.init(service_ckeck_time, crm_available, tl_available);
        window.MailResource = ASC.Mail.Resources.MailResource;
        window.MailScriptResource = ASC.Mail.Resources.MailScriptResource;
        window.MailAttachmentsResource = ASC.Mail.Resources.MailAttachmentsResource;
        window.MailActionCompleteResource = ASC.Mail.Resources.MailActionCompleteResource;

        blankPages.init();
        popup.init();
        mailAlerts.init();
        folderPanel.init();
        MailFilter.init();
        tagsManager.init();
        accountsManager.init();
        settingsPanel.init();
        helpPanel.init();
        helpPage.init();
        accountsPanel.init();

        serviceManager.bind(Teamlab.events.getMailFolders, _onGetMailFolders);
        serviceManager.getState();
    });

    var CreateNewMail = function() {
        var _lastFolderId = MailFilter.getFolder();

        if (_lastFolderId < 1 ||
                _lastFolderId == TMMail.sysfolders.sent.id ||
                _lastFolderId == TMMail.sysfolders.drafts.id ||
                mailBox._Selection.Count() == 0) {
            messagePage.compose();
        }
        else {
            var selectedAddresses = mailBox.getMessagesAddresses();

            messagePage.setToEmailAddresses(selectedAddresses);

            messagePage.composeTo();
        }

    };

    $(function() {
        $('#itemContainer').click(function(evt) {
            var $target = $(evt.target);
            if ($target.hasClass('mailbox-helper-element')) {
                var mailbox = $target.attr('mailboxid');
                if (mailbox) {
                    var $textfield = $target.parents('td.container:first').find('div.textfield .text-field');
                    if ($textfield.length > 0) {
                        addNewMailbox($textfield, mailbox);
                        $textfield.keydown();
                    }
                }
            }
        });
    });
})(jQuery); 