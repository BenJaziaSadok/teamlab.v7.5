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

window.fromSenderFilter = (function($) {
    var type = 'from-sender-filter';

    var create = function(filter) {
        var o = document.createElement('div');
        o.innerHTML = [
          '<div class="default-value">',
            '<span class="title">',
              (TMMail.pageIs('sent') || TMMail.pageIs('drafts')) ? MailScriptResource.FilterToMailAddress : MailScriptResource.FilterFromSender,
            '</span>',
            '<span class="selector-wrapper"></span>',
            '<span class="btn-delete"></span>',
          '</div>'
        ].join('');
        return o;
    };

    var customize = function($container, $filteritem, filter) {
        var $html = $('<div class="input-filter"><input type="text"/></div>');
        $filteritem.find('.selector-wrapper:first').html($html);
        $filteritem.on('keydown', 'input', function (e) {
            if (e.keyCode != 13)
                return;

            MailFilter.setFrom($(this).val());
            mailBox.updateAnchor();
        });
    };

    var destroy = function($container, $filteritem, filter) {
    };

    var process = function($container, $filteritem, filtervalue, params) {
        $filteritem.find('.selector-wrapper:first input').val(params.value);
    };

    return {
        type: type,
        create: create,
        customize: customize,
        destroy: destroy,
        process: process
    };
})(jQuery);