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

window.settingsPanel = (function($) {
    var
        isInit = false,
        _panel_content;

    var init = function() {
        if (isInit === false) {
            isInit = true;
            _panel_content = $('#settingsContainer');
        }
    };

    var unmarkSettings = function() {
        var
            $settings = _panel_content.children();

        for (var i = 0, n =  $settings.length; i < n; i++) {
            var $item = $( $settings[i]);
            if ($item.hasClass('active'))
                $item.toggleClass('active', false);
        }
    };

    var selectItem = function(id)
    {
        var $item = (_panel_content.find('[id="' + id + '"]')).parent();
        if($item != undefined) $item.toggleClass('active', true);
    };

    return {
        init: init,
        unmarkSettings: unmarkSettings,
        selectItem: selectItem
    };

})(jQuery);