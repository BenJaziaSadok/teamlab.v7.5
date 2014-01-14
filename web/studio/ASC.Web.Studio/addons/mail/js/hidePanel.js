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
    var _more_text_width = 100;
    var methods = {
        init: function(options) {

            return this.each(function() {

                var $this = $(this);

                $this.empty();

                var max_width = $this.width() - _more_text_width;
                var used_width = 0;

                $.each(options.items, function(index, value) {
                    var $html = $(options.item_to_html(value, used_width > 0));

                    $html.css({ 'opacity': 0 });
                    $this.append($html);

                    if (used_width + $html.outerWidth() < max_width) {
                        used_width += $html.outerWidth();
                        $html.css({ 'opacity': 1 });
                    }
                    else {
                        $html.remove();
                        var more_text = MailScriptResource.More.replace('%1', options.items.length - index);
                        var more = $('<div class="more_lnk"><span class="gray">' + more_text + '</span></div>');
                        var buttons = [];
                        $.each(options.items, function(index2, value) {
                            if (index2 >= index)
                                buttons.push({ 'text': value, 'disabled': true });
                        });
                        more.find('.gray').actionPanel({ 'buttons': buttons});
                        $this.append(more);
                        return false;
                    }
                });
            });
        },

        destroy: function() {

            return this.each(function() {
                var $this = $(this);
                $this.empty();
            })

        }
    };

    $.fn.hidePanel = function(method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }

    };

})(jQuery);