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

if (!String.prototype.trim) {
    String.prototype.trim = function() {
        return jq.trim(arguments.length ? arguments[0] : "");
    };
}

/*******************************************************************************
JQuery Extension
*******************************************************************************/

jQuery.extend({
    getAnchorParam: function(paramName, url) {
        var regex = new RegExp("[#&]" + paramName + "=([^&]*)");
        var results = regex.exec('#' + url);
        if (results == null)
            return "";
        else
            return results[1];
    },
    hasParam: function(paramName, url) {
        var regex = new RegExp('(\\#|&|^)' + paramName + '=', 'g'); //matches `#param=` or `&param=` or `param=`
        return regex.test(url);
    },
    removeParam: function(paramName, url) {
        var regex = new RegExp("[#&]" + paramName + "=([^&]*)");
        return url.replace(regex, '');
    },
    addParam: function(paramsList, name, value) {
        if (paramsList.length) paramsList += '&';
        paramsList = paramsList + name + '=' + value;
        return paramsList;
    },
    changeParamValue: function(paramsList, name, value) {
        if (jq.hasParam(name, paramsList)) {
            var regex = new RegExp(name + "[=][0-9a-z\-]*");
            return paramsList.replace(regex, name + '=' + value);
        } else {
            return jq.addParam(paramsList, name, value);
        }
    },
    format: function jQuery_dotnet_string_format(text) {
        //check if there are two arguments in the arguments list
        if (arguments.length <= 1) {
            //if there are not 2 or more arguments there's nothing to replace
            //just return the text
            return text;
        }
        //decrement to move to the second argument in the array
        var tokenCount = arguments.length - 2;
        for (var token = 0; token <= tokenCount; ++token) {
            //iterate through the tokens and replace their placeholders from the text in order
            text = text.replace(new RegExp("\\{" + token + "\\}", "gi"), arguments[token + 1]);
        }
        return text;
    },
    isValidDate: function(date) { // for dates of deytpiker with a mask
        var dateFormat = Teamlab.constants.dateFormats.date;
        var separator = "/";
        var dateComponent;
        var dateFormatComponent = dateFormat.split('/');
        if (dateFormatComponent.length == 1) {
            dateFormatComponent = dateFormat.split('.');
            separator = ".";
            if (dateFormatComponent.length == 1) {
                dateFormatComponent = dateFormat.split('-');
                separator = "-";
                if (dateFormatComponent.length == 1) {
                    return "Unknown format date";
                }
            }
        }
        dateComponent = date.split(separator);

        for (var i = 0; i < dateFormatComponent.length; i++) {
            if (dateFormatComponent[i][0].toLowerCase() == "d") {
                if (parseInt(dateComponent[i]) > 31) {
                    return false;
                }
            }
            if (dateFormatComponent[i][0].toLowerCase() == "m") {
                if (parseInt(dateComponent[i]) > 12) {
                    return false;
                }
            }

        }
        return true;
    },
    timeFormat: function(hours) { // convert time to format h:mm
        var h = Math.floor(parseFloat(hours));
        var m = Math.round((parseFloat(hours) - h) * 60);
        if (m < 10) {
            m = '0' + m;
        } else {
            if (m == 60) {
                m = "00";
                h = h + 1;
            }
        }
        return h + ':' + m;
    }
});


jQuery.fn.swap = function(b) {
    b = jQuery(b)[0];
    var a = this[0],
        a2 = a.cloneNode(true),
        b2 = b.cloneNode(true),
        stack = this;

    a.parentNode.replaceChild(b2, a);
    b.parentNode.replaceChild(a2, b);

    stack[0] = a2;
    return this.pushStack(stack);
};
