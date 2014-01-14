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

window.tlGroups = (function($) {
    var _initFlag = false;
    var _groups = [];
    var events = $({});
    var init = function() {
        if (!_initFlag) {
            _initFlag = true;
            update();
        }
    };

    var _onGroupsResponce = function(params, groups) {
        _groups = groups;
        events.trigger('update');
    };

    var update = function() {
        serviceManager.getTLGroups({}, { success: _onGroupsResponce });
    };

    var getGroups = function() {
        return _groups;
    };

    return {
        init: init,
        update: update,
        getGroups: getGroups,
        events: events
    };
})(jQuery);