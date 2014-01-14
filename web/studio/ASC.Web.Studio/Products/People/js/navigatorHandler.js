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

;
jq(document).ready(function() {
    setTimeout(function() {
        window.peoplePageNavigator.NavigatorParent = '#tableForPeopleNavigation td:first';

        window.peoplePageNavigator.changePageCallback = function(page) {
            ASC.People.PeopleController.moveToPage(page);
        };

        jq("#tableForPeopleNavigation select").tlCombobox();
    }, 0);

    function onRenderProfiles(evt, params, data) {
        var usersCount = params.__total || params.__count;
        window.peoplePageNavigator.drawPageNavigator(params.page, usersCount);
        jq("#totalUsers").text(usersCount);
    }

    jq(window).bind('people-render-profiles', onRenderProfiles);
});
