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

(function($) {
    window.dropdown = {
        unregHide: function(hide) {
            $(window).unbind('.dropdown', hide);
            $('iframe').each(function() {
                try {
                    if ($(this)[0].contentWindow.document)
                        $($(this)[0].contentWindow.document).off(".dropdown", hide);
                } catch(e) {
                }
            });
        },

        regHide: function (hide) {
            dropdown.unregHide(hide);
            setTimeout(function() {
                $(window).on("contextmenu.dropdown click.dropdown resize.dropdown", hide);
                $('iframe').each(function() {
                    try {
                        if ($(this)[0].contentWindow.document)
                            $($(this)[0].contentWindow.document).on("contextmenu.dropdown click.dropdown", hide);
                    } catch(e) {
                    }
                });
            }, 0);
        },

        unregScroll: function (scroll) {
            $(window).off('.dropdown', scroll);
        },

        regScroll: function (scroll) {
            $(window).on('scroll.dropdown', scroll);
        },

        onClick: function (event) {
            event.preventDefault();
            event.stopPropagation();
            // initiate global event for other dropdowns close
            $(window).click();
        }
    };
})(jQuery);