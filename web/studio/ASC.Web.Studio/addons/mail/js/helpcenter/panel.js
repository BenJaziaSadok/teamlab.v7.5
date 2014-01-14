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

window.helpPanel = (function($) {
    var 
        isInit = false,
        _panel_content;

    var init = function() {
        if (isInit === false) {
            isInit = true;
            _panel_content = $('#studio_sidePanel .help-center');
        }
    };

    var unmarkSettings = function() {
        var
            $sections = $(_panel_content).find('.menu-sub-list').children();

        if ($(_panel_content).hasClass('active'))
            $(_panel_content).toggleClass('active', false);

        for (var i = 0, n = $sections.length; i < n; i++) {
            var $item = $($sections[i]);
            if ($item.hasClass('active'))
                $item.toggleClass('active', false);
        }
    };

    var selectItem = function(number) {
        if (number == 'all') $(_panel_content).toggleClass('active', true);
        else {
            var $sections = $(_panel_content).find('.menu-sub-list').children();
            $($sections[number]).toggleClass('active', true);
        }
       
    };

    return {
        init: init,
        selectItem: selectItem,
        unmarkSettings: unmarkSettings
    };

})(jQuery);