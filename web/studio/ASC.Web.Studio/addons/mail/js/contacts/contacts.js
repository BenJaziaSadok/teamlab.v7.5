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

/*
Copyright (c) Ascensio System SIA 2013. All rights reserved.
http://www.teamlab.com
*/
window.contactsManager = (function($) {
    var _initFlag = false,
    _teamlab_contacts = [];

    var init = function() {
        if (!_initFlag) {
            _initFlag = true;
            update();
        }
    };

    var _onGetFullTLContacts = function(params, contacts) {
        var count = contacts.length;
        for (var i = 0; i < count; i++) {
            if (contacts[i].email != undefined && contacts[i].email != '')
                _teamlab_contacts.push({ 'firstName': contacts[i].firstName, 'lastName': contacts[i].lastName, 'email': contacts[i].email });
        }
    };

    var update = function() {
        serviceManager.getProfiles({}, { success: _onGetFullTLContacts });
    };

    var getTLContacts = function() {
        return _teamlab_contacts;
    };

    var getTLContactsByEmail = function(email) {
        var result = null;
        var count = _teamlab_contacts.length;
        for (var i = 0; i < count; i++) {
            if (_teamlab_contacts[i].email.toLowerCase() == email.toLowerCase()) {
                result = _teamlab_contacts[i];
                break;
            }
        }
        return result;
    };

    return {
        init: init,
        update: update,
        getTLContacts: getTLContacts,
        getTLContactsByEmail: getTLContactsByEmail
    };
})(jQuery);