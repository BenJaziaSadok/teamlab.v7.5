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

(function ($) {

    // action panel will be drawn under its parent element and
    // its right corner will be shifted on this value from parent right corner
    var right_shift_width = 8;

    var _menu_click = function(e) {
        e.stopPropagation();
    };

    var methods = {

        init: function(options) {

            return this.each(function() {

                var $this = $(this),
                    ap_data = $this.data('actionPanel');

                // If the plugin hasn't been initialized yet
                if (!ap_data) {
                    $(this).data('actionPanel', {
                        target: $this,
                        options: options
                    });

                    ap_data = $this.data('actionPanel');

                    ap_data['hide'] = function () {
                        // remove html if exists
                        ap_data['$html'] && ap_data['$html'].remove();
                        $this.removeClass('active');
                        $this.off('.actionPanel').on('click.actionPanel', methods._show);
                        dropdown.unregHide(ap_data['hide']);
                        dropdown.unregScroll(ap_data['scroll']);
                    };

                    $this.on('click.actionPanel', methods._show);
                    $this.on('remove', methods._destroy);
                }
                else {
                    ap_data['options'] = options;
                }

                if (true === options.show)
                    methods._show.apply(this);
            });
        },

        _destroy: function() {
            var $this = $(this),
                ap_data = $this.data('actionPanel');

            ap_data['hide']();
            $this.off('.actionPanel');
            $this.removeData('actionPanel');
        },

        destroy: function() {
            return this.forEach(methods._destroy);
        },

        _show: function (event) {
            // handle click event if it exists (method was called as callback on click event)
            event && dropdown.onClick(event);

            var $this = $(this);

            if ($this.attr('disabled')) return;

            var ap_data = $this.data('actionPanel');

            $this.off('.actionPanel').on('click.actionPanel', function (e) {
                dropdown.onClick(e);
                ap_data['hide']();
            });
            dropdown.regHide(ap_data['hide']);

            $this.addClass('active');

            var html = '<div class="actionPanel';
            if (ap_data.options.css)
                html += ' ' + ap_data.options.css;
            html += '"><div class="popup-corner right"></div><div class="actionPanelContent"></div></div>';
            var $html = $(html);
            $.each(ap_data.options.buttons, function (index, value) {
                var $add = $('<div class="action ' +
                (value.css_class != undefined ? value.css_class : "") +
                (value.disabled != undefined && true == value.disabled ? '' : ' active') +
                '" isActive="' + (value.disabled != undefined ? !value.disabled : "true") +
                '" title="' + value.text + '">' + value.text + '</div>');
                if (!value.disabled) {
                    $add.click(function(e) {
                        ap_data['hide']();
                        return value.handler(e, value);
                    });
                }
                $html.find('.actionPanelContent').append($add);
            });

            $html.data('actionPanel', { target: $this });

            //ToDo: dirty trick - refactore
            var $body = $(document.body);
            $body.append($html);
            $html.css({ opacity: 0 });
            $html.show();

            var offset = $this.offset();
            var popup_corner = $html.find('.popup-corner');
            var $this_width = $this.width();

            var x = offset.left - $html.width() + $this_width - right_shift_width;

            $html.css({ left: x, top: methods._getY(ap_data.options.horizontal_target ? $this.find(ap_data.options.horizontal_target) : $this, $html, popup_corner) });
            $html.click(_menu_click);

            var arrow = $this.find('.arrow-down');
            if(0 == arrow.length)
                arrow = $this.find('.down_arrow');

            if (0 != arrow.length) {
                var right = $html[0].offsetWidth - arrow.offset().left - Math.ceil(arrow[0].offsetWidth / 2) - Math.ceil(popup_corner[0].offsetWidth / 2) + x;
                // right minus magic 1px for some browsers
                if (!$.browser.mozilla)
                    right = right - 1;
                popup_corner.css('right', right);
            } else {
                popup_corner.css('right', ($this_width + popup_corner.width()) / 2 + right_shift_width);
            }

            ap_data['$html'] = $html;
            $html.css({ opacity: 1 });

            ap_data['scroll'] = function() {
                $html.css({ top: methods._getY($this, $html, popup_corner) });
            };
            dropdown.regScroll(ap_data['scroll']);
        },
        
        _getY: function($target, $html, popup_corner) {
            var y = $target.offset().top + $target.height();
            if (y + $html[0].offsetHeight > $(document).height()) {
                y = $target.offset.top - $html[0].offsetHeight;
                popup_corner.addClass('bottom');
                $html.css('margin', '0 0 5px 0');
            }
            return y;
        },

        hide: function() {
            return this.each(function () {
                var $this = $(this);
                if (!$this.is('.active'))
                    return;
                $this.data('actionPanel')['hide']();
            });
        }
    };

    $.fn.actionPanel = function(method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }

    };

})(jQuery);