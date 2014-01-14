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

window.folderFilter = (function($) {
    var isInit = false,
        filter,
        skip_tags_hide = false, // skip tags filter hide, if user removed all tags from filter control
        events = $({}),
        prev_search = '';

    var init = function() {
        if (!isInit) {
            isInit = true;

            $('#FolderFilter').advansedFilter({
                anykey: true,
                anykeytimeout: 1000,
                maxfilters: -1,
                sorters: [
                    { id: 'date', title: MailScriptResource.FilterByDate, sortOrder: 'descending', def: true }
                ],
                filters: [
                {
                    type: 'flag',
                    id: 'unread',
                    title: MailScriptResource.FilterUnread,
                    group: MailScriptResource.FilterShowGroup
                },
                {
                    type: 'flag',
                    id: 'read',
                    title: MailScriptResource.FilterRead,
                    group: MailScriptResource.FilterShowGroup
                },
                {
                    type: 'flag',
                    id: 'important',
                    title: MailScriptResource.FilterImportant,
                    group: MailScriptResource.FilterShowGroup
                },
                {
                    type: 'flag',
                    id: 'attachments',
                    title: MailScriptResource.FilterWithAttachments,
                    group: MailScriptResource.FilterShowGroup
                },
                {
                    type: fromSenderFilter.type,
                    id: 'from',
                    title: MailScriptResource.FilterFromSender,
                    group: MailScriptResource.FilterAnotherGroup,
                    create: fromSenderFilter.create,
                    customize: fromSenderFilter.customize,
                    destroy: fromSenderFilter.destroy,
                    process: fromSenderFilter.process
                },
                {
                    type: 'daterange',
                    id: 'period',
                    title: MailScriptResource.FilterByPeriod,
                    group: MailScriptResource.FilterAnotherGroup
                },
                {
                    type: 'combobox',
                    id: 'to',
                    title: MailScriptResource.FilterToMailAddress,
                    group: MailScriptResource.FilterAnotherGroup,
                    options: []
                },
                {
                    type: 'combobox',
                    id: 'tag',
                    title: MailScriptResource.FilterWithTags,
                    group: MailScriptResource.FilterAnotherGroup,
                    options: [],
                    multiple: true,
                    defaulttitle: MailScriptResource.ChooseTag
                }
            ]
            }).bind('setfilter', _onSetFilter).bind('resetfilter', _onResetFilter).bind('resetallfilters', _onResetAllFilters);

            // filter object initialization should follow after advansed filter plugin call - because
            // its replace target element with new markup
            filter = $('#FolderFilter');

            filter.find('div.btn-show-filters:first').bind('click', _onShowFilters);
        }
    };

    function _onShowFilters() {
        var with_tags_filter_link = filter.find('li.filter-item[data-id="tag"]');
        if (with_tags_filter_link) {
            if ($('#id_tags_panel_content .tag').length > 0) {
                with_tags_filter_link.show();
            } else {
                with_tags_filter_link.hide();
            }
        }
    }

    var _sort_first_time = true; //initialization raises onSetFilter event with default sorter - it's a workaround

    var _onSetFilter = function(evt, $container, filter_item, value, selectedfilters) {
        switch (filter_item.id) {
            case 'unread':
                _toggleUnread(true);
                break;
            case 'read':
                _toggleUnread(false);
                break;
            case 'important':
                MailFilter.setImportance(true);
                break;
            case 'attachments':
                MailFilter.setAttachments(true);
                break;
            case 'to':
                MailFilter.setTo(value.value);
                break;
            case 'from':
                MailFilter.setFrom(value.value);
                break;
            case 'period':
                MailFilter.setPeriod({ from: value.from, to: value.to });
                var to_date_container = $container.find('span.to-daterange-selector:first span.datepicker-container:first');
                if (to_date_container) 
                    to_date_container.datepicker("option", "maxDate", new Date()); // not select future dates
                break;
            case 'text':
                MailFilter.setSearch(value.value);
                prev_search = value.value;
                break;

            case 'tag':
                if (null == value.value) {
                    MailFilter.removeAllTags();
                    skip_tags_hide = true;
                    break;
                }

                $.each(value.value, function (i, v_new) {
                    MailFilter.addTag(v_new);
                });

                $.each(MailFilter.getTags(), function (i, v) {
                    var is_set = false;
                    $.each(value.value, function(j, v_new) {
                        if (v == v_new)
                            is_set = true;
                    });
                    if (!is_set)
                        MailFilter.removeTag(v);
                });
                break;

            case 'sorter': //ToDo refactore
                if (_sort_first_time) {
                    _sort_first_time = false;
                    return;
                }
                if (MailFilter.getSort() == value.id && MailFilter.getSortOrder() == value.sortOrder)
                    return;
                MailFilter.setSort(value.id);
                MailFilter.setSortOrder(value.sortOrder);
                break;
            default:
                return;
        }

        window.ASC.Mail.ga_track(ga_Categories.folder, ga_Actions.filterClick, filter_item.id);

        mailBox.updateAnchor();
    };

    var _onResetFilter = function(evt, $container, filter_item, selectedfilters) {
        switch (filter_item.id) {
            case 'unread':
            case 'read':
                _toggleUnread(undefined);
                break;
            case 'important':
                MailFilter.setImportance(false);
                break;
            case 'attachments':
                MailFilter.setAttachments(false);
                break;
            case 'to':
                MailFilter.setTo('');
                break;
            case 'from':
                MailFilter.setFrom('');
                break;
            case 'period':
                MailFilter.setPeriod({ from: 0, to: 0 });
                break;
            case 'text':
                MailFilter.setSearch('');
                break;
            case 'tag':
                MailFilter.removeAllTags();
                break;
            case 'sorter': //ToDo refactore
                return undefined;
                break;
            default:
                return;
        }
        mailBox.updateAnchor();
    };

    var reset = function() {
        MailFilter.setImportance(false);
        MailFilter.setAttachments(false);
        MailFilter.setTo('');
        MailFilter.setFrom('');
        MailFilter.setPeriod({ from: 0, to: 0 });
        MailFilter.setSearch('');
        MailFilter.removeAllTags();
        _toggleUnread(undefined);
        mailBox.updateAnchor();
    };

    var _onResetAllFilters = function(evt, $container, filterid, selectedfilters) {
        reset();
    };

    var _toggleUnread = function(flag) {
        if (flag === MailFilter.getUnread())
            MailFilter.setUnread(undefined);
        else if (!flag === MailFilter.getUnread()) {
            if (flag)
                _hideItem('read');
            else
                _hideItem('unread');
            MailFilter.setUnread(flag);
        }
        else {
            MailFilter.setUnread(flag);
        }
    };

    var _showItem = function(id) {
        filter.advansedFilter({ filters: [{ id: id, params: {}}] });
    };

    var _hideItem = function(id) {
        filter.advansedFilter({ filters: [{ id: id, reset: true}] });
    };

    var setUnread = function(flag) {
        if (undefined !== flag) {
            if (flag) {
                _showItem('unread');
                _hideItem('read');
            }
            else {
                _showItem('read');
                _hideItem('unread');
            }
        }
        else {
            _hideItem('read');
            _hideItem('unread');
        }
    };

    var setImportance = function(importance) {
        if (importance)
            _showItem('important');
        else
            _hideItem('important');
    };

    var setAttachments = function(attachments) {
        if (attachments)
            _showItem('attachments');
        else
            _hideItem('attachments');
    };

    var setTo = function(to) {
        if (undefined === to)
            _hideItem('to');
        else
            filter.advansedFilter({ filters: [{ type: 'combobox', id: 'to', params: { value: to}}] });
        events.trigger('to', [to]); //ToDo: Remove it if it isn't necessary
    };

    var setFrom = function(from) {
        if (undefined === from) {
            _hideItem('from');
        } else
            filter.advansedFilter({ filters: [{ type: fromSenderFilter.type, id: 'from', params: { value: from}}] });
    };

    var setPeriod = function(period) {
        if (period.to > 0)
            filter.advansedFilter({ filters: [{ type: 'daterange', id: 'period', params: { to: period.to, from: period.from}}] });
        else
            _hideItem('period');
    };

    var setSearch = function(text) {
        if(prev_search == text)
            return;
        filter.advansedFilter({ filters: [{ type: 'text', id: 'text', params: { value: text}}] });
    };

    var setSort = function(sort, order) {
        if (undefined !== sort && undefined !== order) {
            filter.advansedFilter({ sorters: [{ id: sort, selected: true, dsc: 'descending' == order}] });
        } else {
            filter.advansedFilter({ sorters: [{ id: 'date', selected: true, dsc: true}] });
        }
    };

    var setTags = function(tags) {
        if (tags.length) {
            filter.advansedFilter({ filters: [{ type: 'combobox', id: 'tag', params: { value: tags}}] });
        } else {
            if (true === skip_tags_hide) {
                skip_tags_hide = false;
                return;
            }
            $.each(filter.advansedFilter(), function (index, value) {
                if ('tag' == value.id) _hideItem('tag');
            });
        }
    };

    var clear = function() {
        filter.advansedFilter(null);
    };

    var update = function() {
        var toOptions = [];
        $.each(accountsManager.getAccountList(), function(index, value) {
            toOptions.push({ value: value.email, classname: 'to', title: value.email });
        });
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'to', options: toOptions}] });

        var tags = [];
        $.each(tagsManager.getAllTags(), function(index, value) {
            if (value.lettersCount > 0)
                tags.push({ value: value.id, classname: 'to', title: value.name });
        });
        filter.advansedFilter({ filters: [{ type: 'combobox', id: 'tag', options: tags}] });

        filter.advansedFilter('resize');
    };

    var show = function() {
        if (TMMail.pageIs('sent') || TMMail.pageIs('drafts')) {
            _setFilterTitle('to', MailScriptResource.FilterFromSender);
            _setFilterTitle('from', MailScriptResource.FilterToMailAddress);
        } else {
            _setFilterTitle('to', MailScriptResource.FilterToMailAddress);
            _setFilterTitle('from', MailScriptResource.FilterFromSender);
        }
        filter.parent().show();
    };

    var _setFilterTitle = function(id, title) {
        var menu_item = filter.find('.advansed-filter-list .filter-item[data-id="' + id + '"]');
        menu_item.attr('title', title);
        menu_item.find('.inner-text').html(title);
        filter.find('.advansed-filter-filters .filter-item[data-id="' + id + '"] .title').html(title);
    };

    var hide = function() {
        filter.parent().hide();
    };

    return {
        init: init,

        clear: clear,
        update: update,
        reset: reset,

        setUnread: setUnread,
        setImportance: setImportance,
        setAttachments: setAttachments,
        setTo: setTo,
        setFrom: setFrom,
        setPeriod: setPeriod,
        setSearch: setSearch,
        setSort: setSort,
        setTags: setTags,

        show: show,
        hide: hide,

        events: events
    };
})(jQuery);