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

window.tlFilter = (function($) {
    var isInit = false;
    var filter;
    var events = $({});

    var init = function() {
        if (!isInit) {
            isInit = true;

            $('#tlFilter').advansedFilter({
                maxfilters: -1,
                sorters: [
                    { id: 'displayName', title: MailScriptResource.FilterByTitle, sortOrder: 'ascending', def: true }
                ],
                filters: [
                    {
                        type: "combobox",
                        id: "group",
                        apiparamname: "group",
                        title: FilterByGroupLocalize,
                        group: MailScriptResource.FilterShowGroup,
                        options: [],
                        defaulttitle: MailScriptResource.FilterChoose
                    }
                ]
            }).bind('setfilter', _onSetFilter).bind('resetfilter', _onResetFilter).bind('resetallfilters', _onResetAllFilters);

            // filter object initialization should follow after advanced filter plugin call - because
            // its replace target element with new markup
            filter = $('#tlFilter');

            tlGroups.events.bind('update', _onUpdateGroups);
        };
    };

    var _sort_first_time = true; //initialization raises onSetFilter event with default sorter - it's a workaround

    var _onSetFilter = function(evt, $container, filter_item, value, selectedfilters) {
        events.trigger('set', [filter_item, value, selectedfilters]);
    };

    var _onResetFilter = function(evt, $container, filter_item, selectedfilters) {
        events.trigger('reset', [filter_item, selectedfilters]);
    };

    var _onResetAllFilters = function(evt, $container, filterid, selectedfilters) {
        events.trigger('resetall', [selectedfilters]);
    };

    var _showItem = function(id, params) {
        if (!params) params = {};
        filter.advansedFilter({ filters: [{ id: id, params: params}] });
    };

    var _hideItem = function(id) {
        filter.advansedFilter({ filters: [{ id: id, reset: true}] });
    };

    var show = function() {
        filter.parent().show();
    };

    var hide = function() {
        filter.parent().hide();
    };

    var update = function() {
        filter.advansedFilter('resize');
        tlGroups.update();
    };

    var _onUpdateGroups = function(e) {
        var groups = [];
        $.each(tlGroups.getGroups(), function(index, value) {
            groups.push({ value: value.id, classname: '', title: value.name });
        });
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'group', options: groups, enable: groups.length > 0}] });
        events.trigger('ready');
    };

    var clear = function() {
        filter.advansedFilter(null);
    };

    var setGroup = function(value) {
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'group', params: { value: value}}] });
    };

    var setSearch = function(text) {
        filter.advansedFilter({ filters: [{ type: 'text', id: 'text', params: { value: text}}] });
    };

    var setSort = function(sort, order) {
        filter.advansedFilter({ sorters: [{ id: sort, selected: true, dsc: 'descending' == order}] });
    };

    return {
        init: init,

        clear: clear,
        update: update,

        show: show,
        hide: hide,

        setGroup: setGroup,
        setSearch: setSearch,
        setSort: setSort,

        events: events
    };
})(jQuery);