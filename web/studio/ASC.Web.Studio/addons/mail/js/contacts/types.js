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

window.contactTypes = (function($) {
    var _initFlag = false;
    var _types = [];
    var events = $({});
    var init = function() {
        if (!_initFlag) {
            _initFlag = true;
            update();
        }
    };

    var _onGetTypes = function(params, contactTypes)
    {
        _types = [];
        $.each(contactTypes, function(index, value) {
            _types.push({id: value.id, title: value.title, color: value.color});
        });
        events.trigger('update');
    };

    var update = function() {
        serviceManager.getCrmContactStatus({}, {success: _onGetTypes});
    };

    var getTypes = function() {
        return _types;
    };

    return {
        init: init,
        update: update,
        getTypes: getTypes,
        events: events
    };
})(jQuery);