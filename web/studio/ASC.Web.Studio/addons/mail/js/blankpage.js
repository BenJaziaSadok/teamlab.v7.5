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

window.blankPages = (function($) {
    var _page;

    var init = function() {
        _page = $('#blankPage');
    };

    var showEmptyAccounts = function() {
        _showPage(MailScriptResource.EmptyScrAccountsHeader,
            MailScriptResource.EmptyScrAccountsDescription,
            'accounts',
            MailScriptResource.EmptyScrAccountsButton,
            'addFirstElement',
            function() {
                ASC.Controls.AnchorController.move('#accounts');
                accountsModal.addBox(); return false;
            }

        );
    };

    var showEmptyTags = function() {
        _showPage(MailScriptResource.EmptyScrTagsHeader,
            MailScriptResource.EmptyScrTagsDescription,
            'tags',
            MailScriptResource.EmptyScrTagsButton,
            'addFirstElement',
            function() {
                tagsModal.showCreate();
                return false;
            }
        );
    };

    var showNoLettersFilter = function() {
        _showPage(MailScriptResource.NoLettersFilterHeader,
            MailScriptResource.NoLettersFilterDescription,
            'filter',
            MailScriptResource.ResetFilterButton,
            'clearFilterButton',
            function() { folderFilter.reset(); return false; }
        );
    };

    var showEmptyFolder = function() {
        var header, description, img_class, btn_text, btn_class, btn_handler;
        btn_handler = function() {
            messagePage.compose();
            return false;
        };
        btn_class = 'addFirstElement';
        if (TMMail.pageIs('inbox')) {
            header = MailScriptResource.EmptyInboxHeader;
            description = MailScriptResource.EmptyInboxDescription;
            img_class = 'inbox';
            btn_text = MailScriptResource.EmptyInboxButton;
        } else if (TMMail.pageIs('sent')) {
            header = MailScriptResource.EmptySentHeader;
            description = MailScriptResource.EmptySentDescription;
            img_class = 'sent';
            btn_text = MailScriptResource.EmptySentButton;
        } else if (TMMail.pageIs('drafts')) {
            header = MailScriptResource.EmptyDraftsHeader;
            description = MailScriptResource.EmptyDraftsDescription;
            img_class = 'drafts';
            btn_text = MailScriptResource.EmptyDraftsButton;
        } else if (TMMail.pageIs('trash')) {
            header = MailScriptResource.EmptyTrashHeader;
            description = MailScriptResource.EmptyTrashDescription;
            img_class = 'trash';
        } else if (TMMail.pageIs('spam')) {
            header = MailScriptResource.EmptySpamHeader;
            description = MailScriptResource.EmptySpamDescription;
            img_class = 'spam';
        }
        _showPage(header, description, img_class, btn_text, btn_class, btn_handler);
    };

    var showEmptyCrmContacts = function() {
        _showPage(MailScriptResource.EmptyScrCrmHeader,
            MailScriptResource.EmptyScrCrmDescription,
            'contacts',
            MailScriptResource.EmptyScrCrmButton,
            'addFirstElement',
            null,
            '/products/crm/'
        );
    };

    var showNoCrmContacts = function() {
        _showPage(MailScriptResource.ResetCrmContactsFilterHeader,
            MailScriptResource.ResetCrmContactsFilterDescription,
            'filter',
            MailScriptResource.ResetFilterButton,
            'clearFilterButton',
            function() { contactsPage.resetFilter(); return false; }
        );
    };

    var showNoTlContacts = function() {
        _showPage(MailScriptResource.ResetTlContactsFilterHeader,
            MailScriptResource.ResetTlContactsFilterDescription,
            'filter',
            MailScriptResource.ResetFilterButton,
            'clearFilterButton',
            function() { contactsPage.resetFilter(); return false; }
        );
    };

    var _showPage = function(header, description, img_class, btn_text, btn_class, btn_handler, btn_href) {
        _page.find('.header-base-big').html(header);
        _page.find('.emptyScrDscr').html(description);
        _page.find('.img').attr('class', 'img ' + img_class);
        if (btn_text) {
            _page.find('.emptyScrBttnPnl').html('<a href="' + (btn_href || '#') + '" class="' + btn_class + '">' + btn_text + '</a>');
            if (null != btn_handler) {
                _page.find('.emptyScrBttnPnl a').click(btn_handler);
            }
            _page.find('.emptyScrBttnPnl').show();
        } else
            _page.find('.emptyScrBttnPnl').hide();
        _page.show();
    };

    var hide = function() {
        _page.hide();
    };

    return {
        init: init,
        showEmptyAccounts: showEmptyAccounts,
        showNoLettersFilter: showNoLettersFilter,
        showEmptyFolder: showEmptyFolder,
        showEmptyCrmContacts: showEmptyCrmContacts,
        showNoCrmContacts: showNoCrmContacts,
        showNoTlContacts: showNoTlContacts,
        showEmptyTags: showEmptyTags,
        hide: hide
    };
})(jQuery);