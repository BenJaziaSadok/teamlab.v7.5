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

window.contactsPanel = (function($) {
    var
        isInit = false,
        _panel_content;

    var init = function() {
        if (isInit === false) {
            isInit = true;
            _panel_content = $('#customContactPanel');
        }
    }

    var unmarkContacts = function() {
        var
            $contacts = _panel_content.children();

        for (var i = 0, n = $contacts.length; i < n; i++) {
            var $contact = $($contacts[i]);
            if ($contact.hasClass('active'))
                $contact.toggleClass('active', false);
        }
    };

    var selectContact = function(id)
    {
        var $account = (_panel_content.find('[id="' + id + '"]')).parent();
        if($account != undefined) $account.toggleClass('active', true);
    };

    return {
        init: init,
        unmarkContacts: unmarkContacts,
        selectContact: selectContact
    };

})(jQuery);