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

(function() {
    function onRenderProfiles(evt, params, data) {
        jq('#peopleFilter').advansedFilter({
            nonetrigger: true,
            sorters: [
                        { id: 'by-name', dsc: params.sortorder }
                     ],
            filters: [
                    {
                        id: 'text',
                        params: { value: params.query }
                    },
                    {
                        id: 'selected-group',
                        params: params.groupId ? { id: params.groupId} : null
                    },
                    {
                        id: 'selected-status-active',
                        params: params.status ? { value: params.status} : null
                    },
                    {
                        id: 'selected-status-pending',
                        params: params.status ? { value: params.status} : null
                    },
                    {
                        id: 'selected-status-disabled',
                        params: params.status ? { value: params.status} : null
                    },
                    {
                        id: 'selected-type-user',
                        params: params.type ? { value: params.type} : null
                    },
                    {
                        id: 'selected-type-visitor',
                        params: params.type ? { value: params.type} : null
                    }
                  ]
        });
        if (params.groupId || params.status || params.type) {
            jq('#peopleFilter').addClass("has-filters");
        }
    }

    jq(window).bind('people-render-profiles', onRenderProfiles);
})();
