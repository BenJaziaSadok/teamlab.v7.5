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
    $("#method_nav").treeview({
        collapsed: true,
        animated: "medium",
        unique: false,
        persist: "location"
    });
    $("#method_nav").show();
    $(".tip").twipsy({ live: false, placement: 'below', offset: 10 });
});

