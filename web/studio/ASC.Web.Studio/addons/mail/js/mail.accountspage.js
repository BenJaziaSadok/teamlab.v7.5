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

window.accountsPage = (function($) {
    var 
        isInit = false,
        _page,
        buttons = [];

    var init = function() {
        if (isInit === false) {
            isInit = true;

            _page = $('#id_accounts_page');

            _page.find('#createNewAccount').click(function() {
                accountsModal.addBox();
                return false;
            });

            buttons = [
                { selector: "#accountActionMenu .activateAccount", handler: _activate },
                { selector: "#accountActionMenu .deactivateAccount", handler: _deactivate },
                { selector: "#accountActionMenu .editAccount", handler: _editAccount },
                { selector: "#accountActionMenu .deleteAccount", handler: _removeAccount}];
        }
    };

    var show = function() {
        if (_checkEmpty())
            _page.hide();
        else
            _page.show();
    };

    var hide = function() {
        _page.hide();
    };

    function clear() {
        var accounts_rows = _page.find('.accounts_list');
        $('#accountActionMenu').hide();
        if (accounts_rows)
            accounts_rows.remove();
    }

    var addAccount = function(accountName, enabled, oauth) {
        accountName = accountName.toLowerCase();
        if (!isContain(accountName)) {
            
            var html = $.tmpl('accountItemTmpl',
            {
                address: accountName,
                enabled: enabled,
                oAuthConnection: oauth
            });

            var $html = $(html);
            $html.actionMenu('accountActionMenu', buttons, _pretreatment);
            $('#id_accounts_page .containerBodyBlock .accounts_list').append($html);
            
        }
        if (TMMail.pageIs('accounts') && !_checkEmpty()) {
            _page.show();
        }
    };

    var deleteAccount = function(id) {
        _page.find('tr[data_id="' + id + '"]').remove();
        if (_checkEmpty() && TMMail.pageIs('accounts'))
            _page.hide();
    };

    var activateAccount = function(accountName, activate) {
        var account_div = _page.find('tr[data_id="' + accountName + '"]');

        if (activate) {
            account_div.removeClass('disabled');
        }
        else {
            account_div.toggleClass('disabled', true);
        }

    };

    var _pretreatment = function(id) {
        if (_page.find('tr[data_id="' + id + '"]').hasClass('disabled')) {
            $("#accountActionMenu .activateAccount").show();
            $("#accountActionMenu .deactivateAccount").hide();
        }
        else {
            $("#accountActionMenu .activateAccount").hide();
            $("#accountActionMenu .deactivateAccount").show();
        }
    };

    var _activate = function(id) {
        accountsModal.activateBox(id, true);
    };

    var _deactivate = function(id) {
        accountsModal.activateBox(id, false);
    };

    var _editAccount = function(id) {
        accountsModal.editBox(id);
    };

    var _removeAccount = function(id) {
        accountsModal.removeBox(id);
    };

    var isContain = function(accountName) {
        var account = _page.find('tr[data_id="' + accountName + '"]');
        return (account.length > 0);
    };

    var _checkEmpty = function() {
        if (_page.find('.accounts_list tr').length) {
            _page.find('.accounts_list').show();
            blankPages.hide();
            return false;
        }
        else {
            blankPages.showEmptyAccounts();
            return true;
        }
    };

    function loadAccounts(accounts) {
        clear();
        var html = $.tmpl('accountsTmpl',
            {
                accounts: accounts
            });

        var $html = $(html);
        $('#id_accounts_page .containerBodyBlock .content-header').after($html);
        $('#id_accounts_page').actionMenu('accountActionMenu', buttons, _pretreatment);
    }

    return {
        init: init,

        show: show,
        hide: hide,

        addAccount: addAccount,
        deleteAccount: deleteAccount,
        activateAccount: activateAccount,
        isContain: isContain,
        clear: clear,
        loadAccounts: loadAccounts
    };

})(jQuery);