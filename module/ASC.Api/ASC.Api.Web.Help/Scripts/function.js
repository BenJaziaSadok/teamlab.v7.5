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

$(function() {
    $(".prettyprint code").each(function(e, t) {
        var type = "";
        switch ($(t).attr("data-mediatype")) {
            case "application/json":
                $(t).html($(t).text().replace(/\n/g, '<br/>'));
                type = "language-js";
                break;
            case "text/xml":
                $(t).html($(t).html().replace(/\n/g, '<br/>'));
                type = "language-xml";
                break;
            default:
        }
        $(t).addClass(type);

    });
    $("a.param-name").hover(function(e) {
        $($(this).attr("href")).toggleClass('highlite');
    });
    prettyPrint();
    $('.tabs li:first').each(function(e, t) {
        $(t).addClass('active');
    });
    $('.tab-content div:first').each(function(e, t) {
        $(t).addClass('active');
    });
    $('.tabs').tabs();
});