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

window.crmFilter = (function($) {
    var isInit = false;
    var filter;
    var events = $({});

    var init = function() {
        if (!isInit) {
            isInit = true;

            $('#crmFilter').advansedFilter({
                maxfilters: -1,
                sorters: [
                    { id: 'displayname', title: MailScriptResource.FilterByTitle, sortOrder: 'ascending', def: true },
                    { id: 'contacttype', title: MailScriptResource.FilterByContactTypes, sortOrder: 'ascending' }
                ],
                filters: [
                    {
                        type: "combobox",
                        id: "company",
                        apiparamname: "contactListView",
                        title: MailScriptResource.FilterCompany,
                        filtertitle: MailScriptResource.FilterShowGroup,
                        group: MailScriptResource.FilterShowGroup,
                        groupby: 'type',
                        options:
                            [
                                { value: "company", classname: '', title: MailScriptResource.FilterCompany, def: true },
                                { value: "person", classname: '', title: MailScriptResource.FilterPerson },
                                { value: "withopportunity", classname: '', title: MailScriptResource.FilterWithOpportunity }
                            ]
                    },
                    {
                        type: "combobox",
                        id: "person",
                        apiparamname: "contactListView",
                        title: MailScriptResource.FilterPerson,
                        filtertitle: MailScriptResource.FilterShowGroup,
                        group: MailScriptResource.FilterShowGroup,
                        groupby: 'type',
                        options:
                            [
                                { value: "company", classname: '', title: MailScriptResource.FilterCompany },
                                { value: "person", classname: '', title: MailScriptResource.FilterPerson, def: true },
                                { value: "withopportunity", classname: '', title: MailScriptResource.FilterWithOpportunity }
                            ]
                    },
                    {
                        type: "combobox",
                        id: "withopportunity",
                        apiparamname: "contactListView",
                        title: MailScriptResource.FilterWithOpportunity,
                        filtertitle: MailScriptResource.FilterShowGroup,
                        group: MailScriptResource.FilterShowGroup,
                        groupby: 'type',
                        options:
                            [
                                { value: "company", classname: '', title: MailScriptResource.FilterCompany },
                                { value: "person", classname: '', title: MailScriptResource.FilterPerson },
                                { value: "withopportunity", classname: '', title: MailScriptResource.FilterWithOpportunity, def: true }
                            ]
                    },
                    {
                        type: "combobox",
                        id: "types",
                        apiparamname: "contactType",
                        title: MailScriptResource.FilterContactType,
                        group: MailScriptResource.FilterAnotherGroup,
                        options: [],
                        defaulttitle: MailScriptResource.FilterChoose,
                        enable: false
                    },
                    {
                        type: "combobox",
                        id: "tags",
                        apiparamname: "tags",
                        title: MailScriptResource.FilterWithTags,
                        group: MailScriptResource.FilterAnotherGroup,
                        options: [],
                        defaulttitle: MailScriptResource.FilterChoose,
                        multiple: true,
                        enable: false
                    }
                ]
            }).bind('setfilter', _onSetFilter).bind('resetfilter', _onResetFilter).bind('resetallfilters', _onResetAllFilters);

            // filter object initialization should follow after advansed filter plugin call - because
            // its replace target element with new markup
            filter = $('#crmFilter');

            tagsManager.events.bind('refresh', _onUpdateTags);
            tagsManager.events.bind('delete', _onUpdateTags);
            tagsManager.events.bind('create', _onUpdateTags);
            tagsManager.events.bind('update', _onUpdateTags);

            contactTypes.events.bind('update', _onUpdateTypes);
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
        _onUpdateTags();
        contactTypes.update();
    };

    var _onUpdateTags = function(e) {
        var tags = [];
        $.each(tagsManager.getAllTags(), function(index, value) {
            if (0 > value.id)
                tags.push({ value: value.name, classname: 'to', title: value.name });
        });
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'tags', options: tags, enable: tags.length > 0}] });
    };

    var _onUpdateTypes = function(e) {
        var types = [];
        $.each(contactTypes.getTypes(), function(index, value) {
            types.push({ value: value.id, classname: 'colorFilterItem color_' + value.color.replace('#', ''), title: value.title });
        });
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'types', options: types, enable: types.length > 0}] });
        events.trigger('ready');
    };

    var clear = function() {
        filter.advansedFilter(null);
    };

    var setCompany = function() {
        _showItem('company', { value: 'company' });
    };

    var setPerson = function() {
        _showItem('person', { value: 'person' });
    };

    var setOpportunity = function() {
        _showItem('withopportunity', { value: 'withopportunity' });
    };

    var setType = function(value) {
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'types', params: { value: value}}] });
    };
    var setTags = function(tags) {
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'tags', params: { value: tags}}] });
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

        setCompany: setCompany,
        setPerson: setPerson,
        setOpportunity: setOpportunity,
        setType: setType,
        setTags: setTags,
        setSearch: setSearch,
        setSort: setSort,

        events: events
    };
})(jQuery);