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

window.accountsPanel = (function ($) {
    var 
        is_init = false,
        $panel,
        $content,
        $more,
        max_height;

    // Should be initialized after AccountsManager - to allow it handle accounts changes primarily.
    // Reason: Account list generate with AccountsManager
    var init = function () {
        if (is_init === false) {
            is_init = true;

            $panel = $('#accountsPanel');
            $content = $panel.find('> .content');
            $list = $content.find('> ul');
            $more = $panel.find('.more');

            max_height = $content.css("max-height").replace(/[^-\d\.]/g, '');
            $panel.hover(expand, collapse);

            serviceManager.bind(window.Teamlab.events.getMailAccounts, update);
            serviceManager.bind(window.Teamlab.events.removeMailMailbox, update);
            serviceManager.bind(window.Teamlab.events.updateMailMailbox, update);
            serviceManager.bind(window.Teamlab.events.setMailMailboxState, update);
            serviceManager.bind(window.Teamlab.events.createMailMailboxSimple, update);
            serviceManager.bind(window.Teamlab.events.createMailMailboxOAuth, update);
            serviceManager.bind(window.Teamlab.events.createMailMailbox, update);
        }
    };

    var expand = function() {
        $content.stop().animate({ "max-height": $list.height() }, 200, function() {
            $more.css({ 'visibility': 'hidden' });
        });
    };

    var collapse = function() {
        $content.stop().animate({ "max-height": max_height }, 200, function() {
            $more.css({ 'visibility': 'visible' });
        });
    };

    function update() {
        var accounts = accountsManager.getAccountList();

        if (accounts.length < 2) {
            $panel.hide();
            return;
        }

        var info = [];
        var selected = (MailFilter.getTo() || '').toLowerCase();
        var folder_page = TMMail.pageIs('sysfolders');

        $.each(accounts, function (index, acc) {
            var marked = selected == acc.email.toLowerCase() && folder_page;
            info.push({
                email: acc.email,
                id: TMMail.strHash(acc.email.toLowerCase()),
                marked: marked
            });
        });

        $list.html($.tmpl("accountsPanelTmpl", info));
        updateAnchors();

        setTimeout(function(){
            $panel.show();
            if (max_height < $list.height())
                $more.show();
            else
                $more.hide();
        }, 0);
    };

    // unselect selected account row
    function unmark() {
        $list.find('.tag.tagArrow').removeClass('tag tagArrow');
    };

    // select account if any is in filter
    function mark(){
        var filter_to = MailFilter.getTo();
        if(undefined == filter_to){
            unmark();
            return;
        }

        // skip action if account allready marked
        var id = TMMail.strHash(filter_to.toLowerCase());
        var current = $list.find('.tag.tagArrow');
        if(id == current.prop('id'))
            return;

        current.removeClass('tag tagArrow');
        $list.find('#'+id).addClass('tag tagArrow');
    };

    function updateAnchors(){
        var accounts = accountsManager.getAccountList();
        if (accounts.length < 2)
            return;

        var folder_page = TMMail.pageIs('sysfolders');
        var filter_to = (MailFilter.getTo() || '').toLowerCase();
        $.each(accounts, function(index, acc){
            var href;
            var marked = filter_to == acc.email.toLowerCase();
            if (folder_page)
                href = '#' + TMMail.GetSysFolderNameById(MailFilter.getFolder()) + MailFilter.toAnchor(false, { to: marked ? '' : acc.email }, true);
            else
                href = '#inbox/to=' + encodeURIComponent(acc.email) + '/';
            $list.find('#'+TMMail.strHash(acc.email.toLowerCase())).prop('href', href);
        });
    };

    return {
        init: init,
        unmark: unmark,
        mark: mark,
        updateAnchors: updateAnchors
    };

})(jQuery);
