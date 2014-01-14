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

window.popup = (function($) {
    var _el,
        _popup_queue = [],
        _onUnblock;

    var init = function() {
        _el = $('#commonPopup');
    };

    var _show = function(size) {
        var margintop = jq(window).scrollTop() - 135;
        margintop = margintop + 'px';

        $.blockUI({ message: _el,
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: size,

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': '-261px',
                'margin-top': margintop,
                'background-color': 'White'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0
        });

        var cancelButton = _el.find('.popupCancel .cancelButton');
        cancelButton.attr('onclick', '');

        _el.find('.containerBodyBlock .buttons .cancel').add(cancelButton).bind('click', function() {
            hide();
            return false;
        });
    };

    var hide = function() {
        if (_el.is(':visible')) {
            $.unblockUI({ onUnblock: _onUnblock });
            setTimeout(_process_queue, 0);
        }
    };

    var _process_queue = function() {
        if (_el.is(':visible'))
            return;
        var item = _popup_queue.pop();
        if (item) {
            _el.find('div.containerHeaderBlock:first td:first').html(item.header);
            _el.find('div.containerBodyBlock:first').html(item.body);
            _onUnblock = item.onUnblock;
            _show(item.size);
        }
    };

    var addBig = function (header, body, onUnblock, to_begining) {
        addPopup(header, body, '523px', onUnblock, to_begining);
    };

    var addSmall = function(header, body, onUnblock, to_begining) {
        addPopup(header, body, '350px', onUnblock, to_begining);
    };

    var addPopup = function(header, body, size, onUnblock, to_begining) {
        if (!_el.is(':visible') || header != _el.find('div.containerHeaderBlock:first td:first')[0].innerHTML ||
            body != _el.find('div.containerBodyBlock:first')[0].innerHTML) {
            if(to_begining){
                _popup_queue.unshift({ header: header, body: body, onUnblock: onUnblock, size: size });
            } else {
                _popup_queue.push({ header: header, body: body, onUnblock: onUnblock, size: size });
            }
            _process_queue();
        }
    };

    return {
        init: init,
        hide: hide,
        addBig: addBig,
        addSmall: addSmall
    };

})(jQuery);