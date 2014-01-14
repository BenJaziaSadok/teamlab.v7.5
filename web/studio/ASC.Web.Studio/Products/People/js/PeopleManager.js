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

var PeopleManager = new function() {
    this.DefaultHeader = '';
    this.GroupHeader = '';
    this.SelectedCount = '';
    this.UserLimit = '';
};


// Google Analytics const
var ga_Categories = {
    people: "people_list"
};

var ga_Actions = {
    filterClick: "filter-click",
    createNew: "create-new",
    remove: "remove",
    edit: "edit",
    view: "view",
    changeStatus: "change-status",
    next: "next",
    userClick: "user-click",
    actionClick: "action-click",
    quickAction: "quick-action"
};
// end Google Analytic
