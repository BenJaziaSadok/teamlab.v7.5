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

window.accountsManager = (function($) {
    var 
        isInit = false,
        firstLoad = true,
        mailboxList = [],
        get_accounts_handler;

    var init = function() {
        if (isInit === false) {
            isInit = true;

            get_accounts_handler = serviceManager.bind(window.Teamlab.events.getMailAccounts, onGetMailAccounts);
            serviceManager.bind(window.Teamlab.events.removeMailMailbox, _onRemoveMailbox);
            serviceManager.bind(window.Teamlab.events.updateMailMailbox, _onUpdateMailMailbox);
            serviceManager.bind(window.Teamlab.events.setMailMailboxState, _onSetMailboxState);

            accountsModal.init();
            accountsPage.init();
        }
    };

    var onGetMailAccounts = function(params, accounts) {
        accountsPage.clear();
        $.each(accounts, function(index, value) {
            var account = {};
            account.name = TMMail.ltgt(value.name);
            account.email = TMMail.ltgt(value.address);
            account.enabled = value.enabled;
            account.id = value.id;
            account.oauth = value.oAuthConnection;
            addAccount(account);
        });

        accountsPage.loadAccounts(accounts);
    };

    var _onUpdateMailMailbox = function(params, mailbox) {
        accountsModal.hide();
        for (var i = 0; i < mailboxList.length; i++) {
            if (mailboxList[i].email == params.email.toLowerCase()) {
                mailboxList[i].name = params.name;
                break;
            }
        }
    };

    var _onRemoveMailbox = function(params, email) {
        accountsPage.deleteAccount(email);
        for (var i = 0; i < mailboxList.length; i++) {
            if (mailboxList[i].email == email.toLowerCase()) {
                mailboxList.splice(i, 1);
                break;
            }
        }
        mailBox.markFolderAsChanged(TMMail.sysfolders.inbox.id);
        mailBox.markFolderAsChanged(TMMail.sysfolders.sent.id);
        mailBox.markFolderAsChanged(TMMail.sysfolders.drafts.id);
        mailBox.markFolderAsChanged(TMMail.sysfolders.trash.id);
        mailBox.markFolderAsChanged(TMMail.sysfolders.spam.id);
    };

    var _onSetMailboxState = function(params, email) {
        accountsPage.activateAccount(params.email, params.enabled);
        for (var i = 0; i < mailboxList.length; i++) {
            if (mailboxList[i].email == params.email.toLowerCase()) {
                mailboxList[i].enabled = params.enabled;
                break;
            }
        }
    };

    var getAccountList = function() {
        return mailboxList;
    };

    var getAccountByAddress = function(email) {
        var mailBox = undefined;
        for (var i = 0; i < mailboxList.length; i++) {
            if (mailboxList[i].email == email.toLowerCase()) {
                mailBox = mailboxList[i];
                break;
            }
        }
        return mailBox;
    };

    var addAccount = function(account) {
        account.email = account.email.toLowerCase();
        for (var i = 0; i < mailboxList.length; i++) {
            if (mailboxList[i].email == account.email) return;
        }
        mailboxList.push(account);
    };

    function any() {
        return mailboxList.length > 0;
    }

    return {
        init: init,
        getAccountList: getAccountList,
        getAccountByAddress: getAccountByAddress,
        addAccount: addAccount,
        any: any
    };

})(jQuery);
