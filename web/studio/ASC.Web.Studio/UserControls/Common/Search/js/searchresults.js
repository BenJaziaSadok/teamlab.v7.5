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

var SearchResults = new function () {
    this._containerId = null;
    this._target = null;
    this.ShowAll = function (obj, control, product, item) {
        jq(obj).html('<img src=' + StudioManager.GetImage('loader_12.gif') + ' />');
        jq(obj).css('border-bottom', '0px');
        this._containerId = control;
        this._target = item;
        SearchController.GetAllData(product, jq("#searchTextHidden").val() || jq('#studio_search').val(), function (result) {
            SearchResults.ShowAllCallback(result.value);
        });
    };

    this.ShowAllCallback = function (result) {
        jq("#oper_" + SearchResults._target + " > span").remove();
        jq('#' + SearchResults._containerId).html(result);
    };
    this.Toggle = function (element, th) {
        var elem = jq('#' + element);
        if (elem.css('display') == 'none') {
            elem.css('display', 'block');
            jq('#' + th).attr("src", StudioManager.GetImage('collapse_down_dark.png'));
        } else {
            elem.css('display', 'none');
            jq('#' + th).attr("src", StudioManager.GetImage('collapse_right_dark.png'));
        }
    };
};