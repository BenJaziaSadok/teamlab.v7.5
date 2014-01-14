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

window.ASC.Files.Filter = (function () {
    var isInit = false;
    var advansedFilter = null;

    var init = function () {
        if (!isInit) {

            var startOrderBy = ASC.Files.Common.localStorageManager.getItem(ASC.Files.Constants.storageKeyOrderBy);
            startOrderBy = startOrderBy ||
                {
                    property: "DateAndTime",
                    is_asc: false
                };

            ASC.Files.Filter.advansedFilter =
                jq("#files_advansedFilter")
                    .advansedFilter({
                        anykey: true,
                        anykeytimeout: 800,
                        maxfilters: 1,
                        maxlength: ASC.Files.Constants.MAX_NAME_LENGTH,
                        filters: [
                            {
                                type: "combobox",
                                id: "selected-type",
                                title: ASC.Files.FilesJSResources.Types,
                                options: [
                                    {value: ASC.Files.Constants.FilterType.FilesOnly, title: ASC.Files.FilesJSResources.ButtonFilterFile},
                                    {value: ASC.Files.Constants.FilterType.FoldersOnly, title: ASC.Files.FilesJSResources.ButtonFilterFolder, def: true},
                                    {value: ASC.Files.Constants.FilterType.DocumentsOnly, title: ASC.Files.FilesJSResources.ButtonFilterDocument},
                                    {value: ASC.Files.Constants.FilterType.PresentationsOnly, title: ASC.Files.FilesJSResources.ButtonFilterPresentation},
                                    {value: ASC.Files.Constants.FilterType.SpreadsheetsOnly, title: ASC.Files.FilesJSResources.ButtonFilterSpreadsheet},
                                    {value: ASC.Files.Constants.FilterType.ImagesOnly, title: ASC.Files.FilesJSResources.ButtonFilterImage}]
                            },
                            {type: "person", id: "selected-person", title: ASC.Files.FilesJSResources.Users},
                            {type: "group", id: "selected-group", title: ASC.Files.FilesJSResources.Departments}
                        ],
                        sorters: [
                            {id: "DateAndTime", title: ASC.Files.FilesJSResources.ButtonSortModified, def: (startOrderBy.property == "DateAndTime"), dsc: !startOrderBy.is_asc},
                            {id: "AZ", title: ASC.Files.FilesJSResources.ButtonSortName, def: (startOrderBy.property == "AZ"), dsc: !startOrderBy.is_asc},
                            {id: "Type", title: ASC.Files.FilesJSResources.ButtonSortType, def: (startOrderBy.property == "Type"), dsc: !startOrderBy.is_asc},
                            {id: "Size", title: ASC.Files.FilesJSResources.ButtonSortSize, def: (startOrderBy.property == "Size"), dsc: !startOrderBy.is_asc},
                            {id: "Author", title: ASC.Files.FilesJSResources.ButtonSortAuthor, def: (startOrderBy.property == "Author"), dsc: !startOrderBy.is_asc},
                            {id: "New", title: ASC.Files.FilesJSResources.ButtonSortNew, def: (startOrderBy.property == "New"), dsc: !startOrderBy.is_asc}
                        ]
                    })
                    .bind("setfilter", ASC.Files.Filter.setFilter)
                    .bind("resetfilter", ASC.Files.Filter.setFilter);
        }
    };

    var disableFilter = function () {
        var isMy = ASC.Files.Folders.folderContainer == "my";
        var isForMe = ASC.Files.Folders.folderContainer == "forme";
        var isRootProject = ASC.Files.Folders.currentFolder.id == ASC.Files.Constants.FOLDER_ID_PROJECT;
        var isYourDocs = ASC.Resources.Master.YourDocsDemo == true;

        ASC.Files.Filter.advansedFilter.advansedfilter("sort", !isForMe && !isRootProject);

        if (isForMe) {
            ASC.Files.Filter.advansedFilter.advansedFilter({
                sorters: [{id: "New", def: true, dsc: true}]
            });
        }

        if (isRootProject) {
            ASC.Files.Filter.advansedFilter.advansedFilter({
                sorters: [{id: "AZ", def: true, dsc: false}]
            });
        }

        ASC.Files.Filter.advansedFilter.advansedFilter({
            filters: [
                {id: "selected-type", visible: !isRootProject},
                {id: "selected-person", visible: !isMy && !isRootProject && !isYourDocs},
                {id: "selected-group", visible: !isMy && !isRootProject && !isYourDocs}
            ],
            sorters: [
                {id: "Author", visible: !isMy}
            ]
        });

        ASC.Files.Filter.advansedFilter.advansedFilter({
            nonetrigger: true,
            hasButton: !isRootProject,
            sorters: [
                {id: "New", visible: isForMe}
            ]
        });
    };

    var setFilter = function (evt, $container, filter, params, selectedfilters) {
        if (!isInit) {
            isInit = true;
            return;
        }

        ASC.Files.Anchor.navigationSet(ASC.Files.Folders.currentFolder.id, false, true);
    };

    var clearFilter = function (safeMode) {
        safeMode = safeMode === true;
        var mustTrue = false;
        if (safeMode && isInit) {
            mustTrue = true;
            isInit = false;
        }
        ASC.Files.Filter.advansedFilter.advansedFilter(null);
        if (safeMode && mustTrue) {
            isInit = true;
        }
    };

    var resize = function () {
        ASC.Files.Filter.advansedFilter.resize();
    };

    var getFilterSettings = function (currentFolderId) {
        var settings =
            {
                sorter: ASC.Files.Filter.getOrderDefault(),
                text: "",
                filter: 0,
                subject: ""
            };

        if (ASC.Files.Filter.advansedFilter == null) {
            return settings;
        }

        var param = ASC.Files.Filter.advansedFilter.advansedFilter();
        jq(param).each(function (i, item) {
            switch (item.id) {
                case "sorter":
                    var curOrderBy = getOrderBy(item.params.id, !item.params.dsc);
                    settings.sorter = curOrderBy;
                    ASC.Files.Common.localStorageManager.setItem(ASC.Files.Constants.storageKeyOrderBy, curOrderBy);
                    break;
                case "text":
                    settings.text = item.params.value;
                    break;
                case "selected-person":
                    settings.filter = 8;
                    settings.subject = item.params.id;
                    break;
                case "selected-group":
                    settings.filter = 9;
                    settings.subject = item.params.id;
                    break;
                case "selected-type":
                    settings.filter = parseInt(item.params.value || 0);
                    break;
            }
        });

        currentFolderId = currentFolderId || ASC.Files.Folders.currentFolder.id;

        if (currentFolderId == ASC.Files.Constants.FOLDER_ID_SHARE || ASC.Files.Folders.folderContainer == "forme") {
            settings.sorter = ASC.Files.Filter.getOrderByNew(false);
        }

        if (currentFolderId == ASC.Files.Constants.FOLDER_ID_PROJECT) {
            settings.sorter = ASC.Files.Filter.getOrderByAZ(true);
        }

        return settings;
    };

    var getOrderDefault = function () {
        return ASC.Files.Filter.getOrderByDateAndTime(false);
    };

    var getOrderByDateAndTime = function (asc) {
        return getOrderBy("DateAndTime", asc);
    };

    var getOrderByAZ = function (asc) {
        return getOrderBy("AZ", asc);
    };

    var getOrderByNew = function (asc) {
        return getOrderBy("New", asc);
    };

    var getOrderBy = function (name, asc) {
        name = (typeof name != "undefined" && name != "" ? name : "DateAndTime");
        asc = asc === true;
        return {
            is_asc: asc,
            property: name
        };
    };

    return {
        init: init,

        advansedFilter: advansedFilter,
        disableFilter: disableFilter,

        getFilterSettings: getFilterSettings,

        getOrderDefault: getOrderDefault,
        getOrderByDateAndTime: getOrderByDateAndTime,
        getOrderByAZ: getOrderByAZ,
        getOrderByNew: getOrderByNew,

        setFilter: setFilter,
        clearFilter: clearFilter,
        resize: resize
    };
})();

(function ($) {
    $(function () {
        ASC.Files.Filter.init();

        jq("#files_clearFilter").click(function () {
            ASC.Files.Filter.clearFilter();
            return false;
        });
    });
})(jQuery);
