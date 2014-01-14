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

(function($) {
    if (typeof window.ASC === 'undefined') {
        window.ASC = {};
    }

    if (typeof window.ASC.Mail === 'undefined') {
        window.ASC.Mail = {};
    }

    if (typeof LoadingBanner !== 'undefined') {
        LoadingBanner.displayMailLoading = function(withoutDelay, withBackdrop, config) {
            LoadingBanner.animateDelay = config && config.animateDelay || 300;
            LoadingBanner.displayDelay = config && config.displayDelay || 2000;
            LoadingBanner.loaderCss = config && config.loaderCss || "mail-module";
            LoadingBanner.displayOpacity = config && config.displayOpacity || 1;

            LoadingBanner.displayLoading(withoutDelay, withBackdrop);
        };
    }

    window.ASC.Mail.emptyNode = function(o) {
        if (typeof o === 'string') {
            o = document.getElementById(o);
        }
        if (!o || typeof o !== 'object') {
            return null;
        }
        while (o.firstChild) {
            o.removeChild(o.firstChild);
        }
        return o;
    };

    window.ASC.Mail.updateAreaHeight = function(o) {
        o.style.height = defaultHeight + 'px';
        var 
      height = o.offsetHeight,
      scrollHeight = o.scrollHeight;

        if (scrollHeight > height) {
            while (scrollHeight > height) {
                height += lineHeight;
            }
            o.style.height = height + 'px';
        }
    };

    window.ASC.Mail.setVariableHeight = function(areas, params) {
        if (!params || typeof params !== 'object') {
            params = {};
        }
        var 
      defaultHeight = params.hasOwnProperty('height') ? params.height : 16;
        lineHeight = params.hasOwnProperty('lineHeight') ? params.lineHeight : 16;
        if (typeof areas === 'string') {
            areas = document.getElementById(areas);
        }
        if (!(areas instanceof Array)) {
            areas = [areas];
        }
        var o = null;
        var i = areas.length;
        while (i--) {
            if (typeof areas[i] === 'string') {
                o = document.getElementById(areas[i]);
            } else {
                o = areas[i];
            }
            if (!o || typeof o !== 'object') {
                continue;
            }

            var updateAreaHeight = function(o) {
                o.style.height = defaultHeight + 'px';
                var 
          height = o.offsetHeight,
          scrollHeight = o.scrollHeight;

                if (scrollHeight > height) {
                    while (scrollHeight > height) {
                        height += lineHeight;
                    }
                    o.style.height = height + 'px';
                }
            };

            $(o).keydown(function() {
                setTimeout((function(o) { return function() { updateAreaHeight(o); }; })(this), 0);
            });
        }
    };

    window.ASC.Mail.arrayExclude = function(firstArr, secondArr) {
        var 
      isExist = false,
      localArr = firstArr,
      localArrInd = localArr.length,
      secondArrInd = 0;
        while (localArrInd--) {
            secondArrInd = secondArr.length;
            while (secondArrInd--) {
                if (localArr[localArrInd] == secondArr[secondArrInd]) {
                    firstArr.splice(localArrInd, 1);
                    break;
                }
            }
        }
        return firstArr;
    };

    window.ASC.Mail.arrayComplement = function(firstArr, secondArr) {
        var 
      isExist = false,
      localArr = firstArr,
      localArrInd = localArr.length,
      secondArrInd = 0;
        while (localArrInd--) {
            isExist = false;
            secondArrInd = secondArr.length;
            while (secondArrInd--) {
                if (localArr[localArrInd] == secondArr[secondArrInd]) {
                    isExist = true;
                    break;
                }
            }
            if (isExist) {
                localArr.splice(localArrInd, 1);
            }
        }
        return localArr.concat(secondArr);
    };

    window.ASC.Mail.stringFormat = function() {
        if (!arguments.length) {
            return '';
        }
        var 
      pos = -1,
      ind = [],
      reInd = /{(\d)}/g,
      message = arguments[0];
        while (ind = reInd.exec(message)) {
            pos = -1;
            while ((pos = message.indexOf(ind[0], pos + 1)) !== -1) {
                message = message.substring(0, pos) + (arguments[+ind[1] + 1] || '') + message.substring(pos + ind[0].length);
            }
        }
        return message;
    };

    window.ASC.Mail.cookies = (function() {
        var get = function(name) {
            if (typeof name === 'undefined') {
                return '';
            }
            var 
        start = 0,
        end = 0,
        value = '',
        search = ' ' + name + '=',
        cookie = ' ' + document.cookie;

            if ((start = cookie.indexOf(search)) !== -1) {
                start += search.length;
                if ((end = cookie.indexOf(';', start)) === -1) {
                    end = cookie.length;
                }
                value = cookie.substring(start, end);
            }
            return decodeURI(value);
        };

        var set = function(name, value, expires, path, domain, secure) {
            if (typeof name === 'undefined' || typeof value === 'undefined') {
                return undefined;
            }
            expires = expires || 365;
            if (isFinite(+expires)) {
                var currentDate = new Date();
                currentDate.setTime(currentDate.getTime() + expires * 8640000);
                expires = currentDate.toUTCString();
            }
            expires = expires ? ';expires=' + expires : '';
            path = path ? ';path=' + path : ';path=/';
            domain = domain ? ';domain=' + domain : '';
            secure = secure ? ';secure=' + secure : '';
            document.cookie = name + '=' + encodeURI(value) + expires + path + domain + secure;
            return value;
        };

        var remove = function(name, path, domain) {
            if (name && document.cookie.indexOf(name + '=') != -1) {
                path = path ? ';path=' + path : ';path=/';
                domain = domain ? ';domain=' + domain : '';
                document.cookie = name + '=' + path + domain + ';expires=Thu, 01-Jan-1970 00:00:01 GMT';
            }
            return true;
        };

        return {
            get: get,
            set: set,
            remove: remove
        };
    })();

    window.ASC.Mail.hintManager = (function($) {
        var 
      isInit = false,
      needExpression = false,
      defaultClassName = '',
      handlerId = "hintManager",
      newContainer = null,
      timeout_id = null,
      defaultContainer = null;

        var init = function(className) {
            isInit = true;
            needExpression = $.browser.msie && $.browser.version < 7;
            defaultClassName = typeof className === 'string' ? className : 'hint-container';
            defaultContainer = document.createElement('div');
            defaultContainer.style.display = 'none';
            document.body.appendChild(defaultContainer);

            if (needExpression) {
                defaultContainer.style.position = 'absolute';
            }
        };

        var show = function(msg, className, ttl) {
            if (!isInit) {
                init();
            }

            if (newContainer != null)
                hide();

            newContainer = defaultContainer.cloneNode(true);

            document.body.appendChild(newContainer);
            newContainer.setAttribute('id', handlerId);
            newContainer.className = defaultClassName + (className ? ' ' + className : '');
            newContainer.innerHTML = msg;
            newContainer.style.marginLeft = $(newContainer).width() * -0.5 + 'px';

            $(newContainer).fadeIn(500);

            if (typeof ttl === 'number') {
                timeout_id = setTimeout(function() { return hide(); }, ttl);
            }
        };

        var hide = function() {
            if (newContainer == null) {
                return;
            }
            if (timeout_id) {
                clearTimeout(timeout_id);
                timeout_id = null;
            }

            $(newContainer).fadeOut("fast", function() {
                $(newContainer).remove();
                newContainer = null;
            });
        };

        return {
            init: init,
            show: show,
            hide: hide

        };
    })(jQuery);

    // google analitics track
    window.ASC.Mail.ga_track = function(category, action, label) {
        try {
            if (window._gat) {
                window._gaq.push(['_trackEvent', category, action, label]);
            }
        } catch (err) {
        }
    };


    // retrieves highlighted selected text
    window.ASC.Mail.getSelectionText = function() {
        var text = "";
        if (window.getSelection) {
            text = window.getSelection().toString();
        } else if (document.selection && document.selection.type != "Control") {
            text = document.selection.createRange().text;
        }
        return text;
    };

    $.fn.trackEvent = function(category, action, label) {
        $(this).on("click", function() {
            window.ASC.Mail.ga_track(category, action, label);
        });
    };


})(jQuery);

// Google Analytics const
var ga_Categories = {
    folder: "folder",
    teamlabContacts: "teamlab_contacts",
    crmContacts: "crm_contacts",
    accauntsSettings: "accaunts_settings",
    tagsManagement: "tags_management",
    createMail: "create_mail",
    message: "message",
    leftPanel: "left_panel"
};

var ga_Actions = {
    filterClick: "filter_click",
    createNew: "create_new",
    update: "update",
    next: "next",
    actionClick: "action_click",
    quickAction: "quick_action",
    buttonClick: "button_click"
};