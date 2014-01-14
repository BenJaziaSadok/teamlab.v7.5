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

window.ASC.Files.ServiceManager = (function () {
    var isInit = false,
        servicePath = "",
        cmdSeparator = "/",
        requestTimeout = 60 * 1000,
        customEvents = {};

    var checkReady = true;
    var ajaxStek = new Array();

    var init = function (path) {
        if (isInit === false) {
            isInit = true;

            if (typeof path !== "string" || path.length === 0) {
                throw "Incorrect service path";
            }

            servicePath = path[path.length - 1] === "/" ? path : path + "/";
        }
    };

    var cacheFiles = new Array();
    var cacheTime = 5000;
    var cacheEventType =
        ASC.Files.TemplateManager
            ? [
                ASC.Files.TemplateManager.events.GetFolderItems,
                ASC.Files.TemplateManager.events.GetFileHistory,
                ASC.Files.TemplateManager.events.GetNews
            ]
            : [];

    var tryGetFromCache = function (eventType, params) {
        for (var i = 0; i < cacheFiles.length; i++) {
            var value = cacheFiles[i];
            if (value.eventType == eventType
                && value.params == JSON.stringify(params)) {
                var dateStamp = new Date() - value.timeStamp;
                if (0 < dateStamp && dateStamp < cacheTime) {
                    completeRequest.apply(this, value.data);
                    return true;
                } else {
                    cacheFiles.splice(i, 1);
                    i--;
                }
            }
        }

        return false;
    };

    var getRandomId = function (prefix) {
        return (typeof prefix !== "undefined" ? prefix + "-" : "") + Math.floor(Math.random() * 1000000);
    };

    var getUniqueId = function (o, prefix) {
        var iterCount = 0,
            maxIterations = 1000,
            uniqueId = getRandomId();

        while (o.hasOwnProperty(uniqueId) && iterCount++ < maxIterations) {
            uniqueId = getRandomId(prefix);
        }
        return uniqueId;
    };

    var execCustomEvent = function (eventType, thisArg, argsArray) {
        eventType = eventType.toLowerCase();
        thisArg = thisArg || window;
        argsArray = argsArray || [];

        if (!customEvents.hasOwnProperty(eventType)) {
            return;
        }
        var customEvent = customEvents[eventType];

        for (var eventId in customEvent) {
            if (customEvent.hasOwnProperty(eventId)) {
                customEvent[eventId].handler.apply(thisArg, argsArray);
                if (customEvent[eventId].type & 1) {
                    delete customEvent[eventId];
                }
            }
        }
    };

    var addCustomEvent = function (eventType, handler, params) {
        if (typeof eventType !== "string" || typeof handler !== "function") {
            return undefined;
        }

        eventType = eventType.toLowerCase();

        if (typeof params !== "object") {
            params = {};
        }
        var isOnceExec = params.hasOwnProperty("once") ? params.once : false;

        // collect the flags mask the new handler
        var handlerType = 0;
        handlerType |= +isOnceExec * 1; // isOnceExec - process once and delete

        if (!customEvents.hasOwnProperty(eventType)) {
            customEvents[eventType] = {};
        }

        var eventId = getUniqueId(customEvents[eventType]);

        customEvents[eventType][eventId] = {
            handler: handler,
            type: handlerType
        };

        return eventId;
    };

    var removeCustomEvent = function (eventType, eventId) {
        if (typeof eventType !== "string" || typeof eventId === "undefined") {
            return false;
        }

        if (customEvents(eventType) && customEvents[eventType].hasOwnProperty(eventId)) {
            delete userEventHandlers[eventType][eventId];
        }
        return true;
    };

    var getUrl = function () {
        var url = servicePath;

        if (arguments.length === 0) {
            return url;
        }
        for (var i = 0, n = arguments.length - 1; i < n; i++) {
            url += arguments[i] + cmdSeparator;
        }

        var res = url + arguments[i];
        res += (res.search(/\?/) > 0 ? "&" : "?") + "_=" + new Date().getTime();

        return res;
    };

    var getNodeContent = function (obj) {
        if (!obj || typeof obj !== "object") {
            return "";
        }

        return obj.text || obj.textContent || (function (o) {
            var result = "",
                childrens = o.childNodes;

            if (!childrens) {
                return result;
            }
            for (var i = 0, n = childrens.length; i < n; i++) {
                var child = childrens.item(i);
                switch (child.nodeType) {
                    case 1:
                    case 5:
                        result += arguments.callee(child);
                        break;
                    case 3:
                    case 2:
                    case 4:
                        result += child.nodeValue;
                        break;
                    default:
                        break;
                }
            }
            return result;
        })(obj);
    };

    var completeRequest = function (eventType, params, dataType, xmlHttpRequest, textStatus) {

        checkReady = true;
        if (typeof LoadingBanner != "undefined" && typeof LoadingBanner.hideLoading != "undefined") {
            LoadingBanner.hideLoading();
        }

        if (textStatus === "error") {
            var errorMessage = "",
                commentMessage = "",
                messageNode = null,
                innerNode;
            var innerMessageNode = null;

            if (xmlHttpRequest.responseXML) {
                messageNode = xmlHttpRequest.responseXML.getElementsByTagName("message")[0];
                innerNode = xmlHttpRequest.responseXML.getElementsByTagName("inner")[0];
                if (innerNode) {
                    innerMessageNode = innerNode.getElementsByTagName("message")[0];
                }
                if (errorMessage === "") {
                    try {
                        errorMessage = eval("[" + xmlHttpRequest.responseText + "]")[0].Detail;
                    } catch (e) {
                        var div = document.createElement("div");
                        errorMessage = jq("#content", jq(div).html(xmlHttpRequest.responseText)).text();
                    }
                }
            } else if (xmlHttpRequest.responseText) {
                div = document.createElement("div");
                errorMessage = jq("#content", jq(div).html(xmlHttpRequest.responseText)).text();
                if (errorMessage === "") {
                    try {
                        errorMessage = eval("[" + xmlHttpRequest.responseText + "]")[0].Detail;
                    } catch (e) {
                    }
                }
            }
            if (messageNode && typeof messageNode === "object") {
                errorMessage = getNodeContent(messageNode);
            }
            if (innerMessageNode && typeof innerMessageNode === "object") {
                commentMessage = getNodeContent(innerMessageNode);
            }

            execCustomEvent(eventType, window, [undefined, params, commentMessage || errorMessage]);
            return;
        }

        var data;

        try {
            switch (dataType) {
                case "xml":
                    data = ASC.Controls.XSLTManager.createXML(xmlHttpRequest.responseText);
                    break;
                case "json":
                    data = jq.parseJSON(xmlHttpRequest.responseText);
                    break;
                default:
                    data = ASC.Controls.XSLTManager.createXML(xmlHttpRequest.responseXML.xml)
                        || jq.parseJSON(xmlHttpRequest.responseText);
            }
        } catch (e) {
            data = xmlHttpRequest.responseText;
        }

        execCustomEvent(eventType, window, [data, params]);

        if (ajaxStek.length != 0 && checkReady == true) {
            var req = ajaxStek.shift();
            checkReady = false;
            execAjax(req);
        }
    };

    var getCompleteCallbackMethod = function (eventType, params, dataType) {
        return function () {
            var argsArray = [eventType, params, dataType];
            for (var i = 0, n = arguments.length; i < n; i++) {
                argsArray.push(arguments[i]);
            }

            if (jq.inArray(eventType, cacheEventType) >= 0) {
                cacheFiles.push({
                    eventType: eventType,
                    params: JSON.stringify(params),
                    timeStamp: new Date(),
                    data: argsArray
                });
            }

            completeRequest.apply(this, argsArray);
        };
    };

    var request = function (type, dataType, eventType, params) {
        if (typeof type === "undefined" || typeof dataType === "undefined" || typeof eventType !== "string") {
            return;
        }

        if (tryGetFromCache(eventType, params)) {
            return;
        }

        if (typeof LoadingBanner == "undefined" || typeof LoadingBanner.displayLoading == "undefined") {
            params.showLoading = false;
        }

        var data = {},
            argsArray = [];
        var contentType = (params.ajaxcontentType || "text/xml");

        if (typeof params !== "object") {
            params = {};
        }

        switch (type.toLowerCase()) {
            case "delete":
            case "get":
                for (var i = 4, n = arguments.length; i < n; i++) {
                    argsArray.push(arguments[i]);
                }
                break;
            case "post":
                data = (contentType == "text/xml" ? ASC.Files.Common.jsonToXml(arguments[4]) : arguments[4]);

                for (i = 5, n = arguments.length; i < n; i++) {
                    argsArray.push(arguments[i]);
                }
                break;
            default:
                return;
        }

        var req = {
            async: (params.ajaxsync != true),
            data: data,
            type: type,
            dataType: dataType,
            contentType: contentType,
            mimeType: "text/xml",
            cache: true,
            url: getUrl.apply(this, argsArray),
            timeout: requestTimeout,
            beforeSend: params.showLoading ? LoadingBanner.displayLoading() : null,
            complete: getCompleteCallbackMethod(eventType, params, dataType)
        };

        if (ajaxStek.length == 0 && checkReady == true) {
            checkReady = false;
            execAjax(req);
        } else {
            ajaxStek.push(req);
        }
    };

    var execAjax = function (req) {
        jq.ajax({
            async: req.async,
            data: req.data,
            type: req.type,
            dataType: req.dataType,
            contentType: req.contentType,
            cache: req.cache,
            url: req.url,
            timeout: req.timeout,
            beforeSend: req.beforeSend,
            complete: req.complete
        });
    };

    var createFolder = function (eventType, params) {
        params.ajaxsync = true;
        request("get", "xml", eventType, params, "folders", "create?parentId=" + encodeURIComponent(params.parentFolderID) + "&title=" + encodeURIComponent(params.title));
    };

    var getFile = function (eventType, params) {
        params.ajaxsync = true;
        request("get", "xml", eventType, params, "folders", "files", "getversion?fileId=" + encodeURIComponent(params.fileId) + "&version=" + (params.version || -1));
    };

    var getFolderItems = function (eventType, params, data) {
        params.showLoading = params.append != true;
        request("post", "xml", eventType, params, data, "folders?parentId=" + encodeURIComponent(params.folderId) + "&from=" + params.from + "&count=" + params.count + "&filter=" + params.filter + "&subjectID=" + params.subject + "&search=" + encodeURIComponent(params.text));
    };

    var renameFolder = function (eventType, params) {
        request("get", "xml", eventType, params, "folders", "rename?folderId=" + encodeURIComponent(params.folderId) + "&title=" + encodeURIComponent(params.newname));
    };

    var renameFile = function (eventType, params) {
        request("get", "xml", eventType, params, "folders", "files", "rename?fileId=" + encodeURIComponent(params.fileId) + "&title=" + encodeURIComponent(params.newname));
    };

    var deleteItem = function (eventType, params, data) {
        request("post", "json", eventType, params, data, "folders", "files?action=delete");
    };

    var emptyTrash = function (eventType, params) {
        request("get", "json", eventType, params, "emptytrash");
    };

    var getFileHistory = function (eventType, params) {
        request("get", "xml", eventType, params, "folders", "files", "history?fileId=" + encodeURIComponent(params.fileId));
    };

    var moveItems = function (eventType, params, data) {
        request("post", "json", eventType, params, data, "moveorcopy?destFolderId=" + encodeURIComponent(params.folderToId) + "&ow=" + (params.overwrite == true) + "&ic=" + (params.isCopyOperation == true));
    };

    var moveFilesCheck = function (eventType, params, data) {
        request("post", "json", eventType, params, data, "folders", "files", "moveOrCopyFilesCheck?destFolderId=" + encodeURIComponent(params.folderToId));
    };

    var download = function (eventType, params, data) {
        params.showLoading = true;
        request("post", "json", eventType, params, data, "bulkdownload");
    };

    var terminateTasks = function (eventType, params) {
        request("get", "json", eventType, params, "tasks?terminate=" + params.isImport);
    };

    var getTasksStatuses = function (eventType, params) {
        request("get", "json", eventType, params, "tasks", "statuses");
    };

    var setCurrentVersion = function (eventType, params) {
        request("get", "xml", eventType, params, "folders", "files", "updateToVersion?fileId=" + encodeURIComponent(params.fileId) + "&version=" + params.version);
    };

    var createNewFile = function (eventType, params) {
        params.ajaxsync = true;
        request("get", "xml", eventType, params, "folders", "files", "createfile?parentId=" + encodeURIComponent(params.folderID) + "&title=" + encodeURIComponent(params.fileTitle));
    };

    var trackEditFile = function (eventType, params) {
        request("get", "json", eventType, params, "trackeditfile?fileId=" + encodeURIComponent(params.fileID) + "&tabId=" + params.tabId + "&docKeyForTrack=" + params.docKeyForTrack + "&isFinish=" + (params.finish == true) + "&lockVersion=" + (params.lockVersion == true) + params.shareLinkParam);
    };

    var checkEditing = function (eventType, params, data) {
        request("post", "json", eventType, params, data, "checkediting");
    };

    var canEditFile = function (eventType, params) {
        request("get", "json", eventType, params, "canedit?fileId=" + encodeURIComponent(params.fileID) + params.shareLinkParam);
    };

    return {
        init: init,
        bind: addCustomEvent,
        unbind: removeCustomEvent,

        request: request,

        createFolder: createFolder,
        createNewFile: createNewFile,

        getFile: getFile,
        getFolderItems: getFolderItems,

        renameFolder: renameFolder,
        renameFile: renameFile,
        deleteItem: deleteItem,
        emptyTrash: emptyTrash,

        download: download,
        terminateTasks: terminateTasks,

        getFileHistory: getFileHistory,
        setCurrentVersion: setCurrentVersion,
        moveFilesCheck: moveFilesCheck,
        moveItems: moveItems,

        getTasksStatuses: getTasksStatuses,

        trackEditFile: trackEditFile,
        checkEditing: checkEditing,

        canEditFile: canEditFile
    };
})();

(function ($) {
    $(function () {
        ASC.Files.ServiceManager.init(ASC.Files.Constants.URL_WCFSERVICE);
    });
})(jQuery);