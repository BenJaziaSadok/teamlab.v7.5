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

window.tagsColorsPopup = (function($) {
    var _callback;
    var _obj;
    var _panel;
    var _corner_left = 14;

    var init = function() {
        _panel = $('#tagsColorsPanel');

        _panel.find('div[colorstyle]').bind('click', function(e) {
            var style = $(this).attr('colorstyle');
            _callback(_obj, style);
        });
    };

    var show = function(obj, callback_func) {
        _callback = callback_func;
        _obj = obj;
        var $obj = $(obj);
        x = $obj.offset().left - _corner_left + $obj.width()/2;
        y = $obj.offset().top + $obj.height();
        _panel.css({ left: x, top: y, display: 'block' });

        $('body').bind('click.tagsColorsPopup', function(event) {
            var elt = (event.target) ? event.target : event.srcElement;
            if (!($(elt).is('.square') || $(elt).is('.square *') || $(elt).is('.leftRow span')))
                hide();
        });
    };

    var hide = function(obj) {
        _panel.hide();
        $('body').unbind("click.tagsColorsPopup");
    };

    return {
        init: init,
        show: show,
        hide: hide
    };
})(jQuery);