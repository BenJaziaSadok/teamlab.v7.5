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

/*
Copyright (c) Ascensio System SIA 2013. All rights reserved.
http://www.teamlab.com
*/
window.TariffLimitExceed = (function () {
    var showLimitExceedUsers = function () {
        StudioBlockUIManager.blockUI("#tariffLimitExceedUsersPanel", 500, 300, 0);
    };
    return {
        showLimitExceedUsers: showLimitExceedUsers
    };
})();