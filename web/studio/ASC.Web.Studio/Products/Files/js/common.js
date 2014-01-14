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

if (typeof jq === "undefined") {
    jq = jQuery.noConflict();
}

if (typeof ASC === "undefined") {
    ASC = {};
}

if (typeof ASC.Files === "undefined") {
    ASC.Files = (function () {
        return {};
    })();
}

if (typeof (ASC.Files.Constants) === 'undefined') {
    ASC.Files.Constants = {};
}

ASC.Files.Constants.REQUEST_STATUS_DELAY = 2000;
ASC.Files.Constants.COUNT_ON_PAGE = 30,
ASC.Files.Constants.entryIdRegExpStr = "(\\d+|[a-z]+-\\d+(-.+)*)",
ASC.Files.Constants.storageKeyRecent = "TeamLabRecentDocuments",
ASC.Files.Constants.storageKeyCompactView = "TeamLabDocumentsCompactView",
ASC.Files.Constants.storageKeyOrderBy = "TeamLabDocumentsOrderBy",
ASC.Files.Constants.storageKeyUploaderCompactView = "TeamLabDocumentsUploaderCompactView";

ASC.Files.Common = (function () {

    var storeOriginal = false;

    var isCorrectId = function (id) {
        if (typeof id === "undefined") {
            return false;
        }
        if (id === null) {
            return false;
        }
        if (id === 0) {
            return false;
        }
        var regExp = new RegExp("^" + ASC.Files.Constants.entryIdRegExpStr);
        return regExp.test(id);
    };

    var jsonToXml = function (o, parent) {
        var xml = "";

        if (typeof o === "object") {
            if (o === null) {
                if (typeof parent !== "undefined") {
                    xml += "<" + parent + "></" + parent + ">";
                }
            } else if (o.constructor.toString().indexOf("Array") !== -1) {
                var n;
                for (i = 0, n = o.length; i < n; i++) {
                    if (typeof parent !== "undefined") {
                        xml += "<" + parent + ">" + arguments.callee(o[i]) + "</" + parent + ">";
                    } else {
                        xml += arguments.callee(o[i]);
                    }
                }
            } else {
                for (var i in o) {
                    xml += arguments.callee(o[i], i);
                }
                if (typeof parent !== "undefined") {
                    xml = "<" + parent + ">" + xml + "</" + parent + ">";
                }
            }
        } else if (typeof o === "string") {
            xml = o;
            if (typeof parent !== "undefined") {
                xml = "<" + parent + ">" + xml + "</" + parent + ">";
            }
        } else if (typeof o !== "undefined" && typeof o.toString !== "undefined") {
            xml = o.toString();
            if (typeof parent !== "undefined") {
                xml = "<" + parent + ">" + xml + "</" + parent + ">";
            }
        }
        return xml;
    };

    var getSitePath = function () {
        var sitePath = jq.url.attr("protocol");
        sitePath += "://";
        sitePath += jq.url.attr("host");
        if (jq.url.attr("port") != null) {
            sitePath += ":";
            sitePath += jq.url.attr("port");
        }
        return sitePath;
    };

    var cancelBubble = function (e) {
        if (!e) {
            e = window.event;
        }
        e.cancelBubble = true;
        if (e.stopPropagation) {
            e.stopPropagation();
        }
    };

    var fixEvent = function (e) {
        e = e || window.event;
        if (!e) {
            return {};
        }
        if (e.pageX == null && e.clientX != null) {
            var html = document.documentElement;
            var body = document.body;
            e.pageX = e.clientX + (html && html.scrollLeft || body && body.scrollLeft || 0) - (html.clientLeft || 0);
            e.pageY = e.clientY + (html && html.scrollTop || body && body.scrollTop || 0) - (html.clientTop || 0);
        }

        if (!e.which && e.button) {
            e.which = e.button & 1 ? 1 : (e.button & 2 ? 3 : (e.button & 4 ? 2 : 0));
        }

        return e;
    };

    var stickMovingPanel = function (toggleObjId, movingPopupObj, movingPopupShift, fixBigHeight) {
        var toggleObj = jq("#" + toggleObjId);
        if (!toggleObj.is(":visible")) {
            return;
        }
        var spacerName = "Spacer";

        var toggleObjSpacer = jq("#" + toggleObjId + spacerName);
        var absTop;

        if (jq("#" + toggleObjId + spacerName + ":visible").length == 0) {
            absTop = toggleObj.offset().top;
        } else {
            absTop = toggleObjSpacer.offset().top;
        }

        movingPopupShift = movingPopupShift || 0;
        var jqWindow = jq(window);
        var winScroll = jqWindow.scrollTop();

        if (winScroll >= absTop) {
            var toggleObjHeight = toggleObj.outerHeight();
            var parentObj = toggleObj.parent();
            var parentHeight = parentObj.innerHeight() - parseInt(parentObj.css("padding-top")) - parseInt(parentObj.css("padding-bottom"));

            if (!toggleObj.hasClass("stick-panel") || jq.browser.mobile) {
                if (!fixBigHeight || (winScroll - absTop < parentHeight - toggleObjHeight)) {
                    if (toggleObjSpacer.length == 0) {
                        var createToggleObjSpacer = document.createElement("div");
                        createToggleObjSpacer.id = toggleObjId + spacerName;
                        document.body.appendChild(createToggleObjSpacer);
                        toggleObjSpacer = jq("#" + toggleObjId + spacerName);
                        toggleObjSpacer.insertAfter(toggleObj);
                        toggleObjSpacer.css(
                            {
                                "height": toggleObj.css("height"),
                                "padding-top": toggleObj.css("padding-top"),
                                "padding-bottom": toggleObj.css("padding-bottom")
                            });
                    }
                    toggleObjSpacer.show();

                    toggleObj
                        .addClass("stick-panel")
                        .css("left", (parentObj.offset().left - jqWindow.scrollLeft()));

                    if (movingPopupObj) {
                        movingPopupObj.css({
                            "position": "fixed",
                            "top": movingPopupShift - winScroll
                        });
                    }

                    if (jq.browser.mobile) {
                        toggleObj.css(
                            {
                                "top": jq(document).scrollTop() + "px",
                                "position": "absolute"
                            });
                    }
                }
            }
            if (fixBigHeight && toggleObj.hasClass("stick-panel")) {
                toggleObj.css("top", -Math.max(0, (winScroll - absTop - (parentHeight - toggleObjHeight))));
            }
        } else {
            if (toggleObj.hasClass("stick-panel")) {
                toggleObjSpacer.hide();
                toggleObj.removeClass("stick-panel");
                jq("#mainContentHeader").css("width", "auto");

                if (movingPopupObj) {
                    movingPopupObj.css({
                        "position": "absolute",
                        "top": movingPopupShift
                    });
                }

                if (jq.browser.mobile) {
                    toggleObj.css(
                        {
                            "position": "static"
                        });
                }
            }
        }
    };

    var localStorageManager = function () {
        var isAvailable;
        try {
            isAvailable = "localStorage" in window && window["localStorage"] !== null;
        } catch (e) {
            isAvailable = false;
        }

        var getItem = function (key) {
            if (!key && !ASC.Files.Common.localStorageManager.isAvailable) {
                return null;
            }
            return JSON.parse(localStorage.getItem(key));
        };

        var setItem = function (key, value) {
            if (!key || !ASC.Files.Common.localStorageManager.isAvailable) {
                return;
            }
            try {
                localStorage.setItem(key, JSON.stringify(value));
            } catch (e) {
                if (typeof QUOTA_EXCEEDED_ERR != "undefined" && e == QUOTA_EXCEEDED_ERR) {
                    //throw "Local storage is full";
                }
            }
        };

        return {
            isAvailable: isAvailable,

            getItem: getItem,
            setItem: setItem
        };
    }();

    var characterString = "@#$%&*+:;\"'<>?|\/";
    var characterRegExp = new RegExp("[@#$%&*\+:;\"'<>?|\\\\/]", "gim");

    var replaceSpecCharacter = function (str) {
        return str.trim().replace(ASC.Files.Common.characterRegExp, "_");
    };

    var keyCode = { enter: 13, esc: 27, spaceBar: 32, pageUP: 33, pageDown: 34, end: 35, home: 36, left: 37, up: 38, right: 39, down: 40, deleteKey: 46, a: 65, f: 70, n: 78 };

    return {
        getSitePath: getSitePath,
        cancelBubble: cancelBubble,
        fixEvent: fixEvent,
        stickMovingPanel: stickMovingPanel,
        keyCode: keyCode,

        characterString: characterString,
        characterRegExp: characterRegExp,
        replaceSpecCharacter: replaceSpecCharacter,

        isCorrectId: isCorrectId,
        jsonToXml: jsonToXml,

        storeOriginal: storeOriginal,
        localStorageManager: localStorageManager
    };
})();