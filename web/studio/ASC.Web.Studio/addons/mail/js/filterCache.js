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

/*
 Caches information about filtered messageses list such as first and last messages ids and etc.
 Usefull for detecting message position and prev next displaing.
*/

window.filterCache = (function() {
    var cache;

    function init() {
        cache = {};
        serviceManager.bind(window.Teamlab.events.getMailFilteredConversations, onGetMailConversations);
    };

    function filterHash(filter) {
        return TMMail.strHash(filter.toAnchor(true, {}, false));
    };

    function onGetMailConversations(params, conversations) {
        if (undefined == conversations.length || 0 == conversations.length)
            return;

        var folder = MailFilter.getFolder();
        var folder_cache = (cache[folder] = cache[folder] || {});
        var hash = filterHash(MailFilter);
        var filter_cache = (folder_cache[hash] = folder_cache[hash] || {});

        var has_next = (true === MailFilter.getPrevFlag()) || params.__total > MailFilter.getPageSize();
        var has_prev = (false === MailFilter.getPrevFlag() && null != MailFilter.getFromDate() && undefined != MailFilter.getFromDate()) || (true === MailFilter.getPrevFlag() && params.__total > MailFilter.getPageSize());

        if (!has_prev)
            filter_cache.first = conversations[0].id;

        if (!has_next)
            filter_cache.last = conversations[conversations.length - 1].id;

        var order_cache = (filter_cache['conversations'] = filter_cache['conversations'] || []);
        for (var i = 0, len = conversations.length - 1; i < len; i++) {
            order_cache[conversations[i].id] = order_cache[conversations[i].id] || {};
            order_cache[conversations[i].id].next = conversations[i + 1].id;

            order_cache[conversations[i + 1].id] = order_cache[conversations[i + 1].id] || {};
            order_cache[conversations[i + 1].id].prev = conversations[i].id;
        };
    };

    // get next or previous conversation id, or 0
    function getNextPrevConversation(filter, id, next) {
        try {
            return cache[filter.getFolder()][filterHash(filter)]['conversations'][id][true === next ? 'next' : 'prev'] || 0;
        }
        catch (e) { }

        return 0;
    }

    // try to get next conversation id from cache or return 0
    function getNextConversation(filter, id) {
        return getNextPrevConversation(filter, id, true);
    }

    // try to get prev conversation id from cache or return 0
    function getPrevConversation(filter, id) {
        return getNextPrevConversation(filter, id, false);
    }

    // get cached filter info
    function getCache(filter) {
        if (cache[filter.getFolder()])
            return cache[filter.getFolder()][filterHash(filter)] || {};
        return {};
    };

    // drop folder cached values for filter
    function drop(folder) {
        cache[folder] = {};
    };

    return {
        init: init,
        drop: drop,
        getCache: getCache,
        getNextConversation: getNextConversation,
        getPrevConversation: getPrevConversation
    };
})();