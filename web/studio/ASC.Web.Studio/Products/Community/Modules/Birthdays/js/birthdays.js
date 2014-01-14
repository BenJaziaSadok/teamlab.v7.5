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

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Community === "undefined")
    ASC.Community = (function () { return {} })();

ASC.Community.Birthdays = (function() {
    return {
        openContact: function(obj) {
            var name = jq(obj).attr("username");
            var tcExist = false;
            try {
                tcExist = !!ASC.Controls.JabberClient;
            } catch (err) {
                tcExist = false;
            }
            if (tcExist === true) {
                try {
                    ASC.Controls.JabberClient.open(name);
                } catch (err) {
                }
            }
        },

        remind: function(obj) {
            var userCard = jq(obj).parents(".small-user-card");
            var userId = jq(userCard).find("input[type=hidden]").val();

            window.AjaxPro.onLoading = function(b) { };

            window.AjaxPro.Birthdays.RemindAboutBirthday(userId, true, function(result) {
                if (result.error != null) {
                    alert(result.error.Message);
                    return false;
                }
                jq(userCard).addClass("active");
            });
        },

        clearRemind: function(obj) {
            var userCard = jq(obj).parents(".small-user-card");
            var userId = jq(userCard).find("input[type=hidden]").val();

            window.AjaxPro.onLoading = function(b) { };

            window.AjaxPro.Birthdays.RemindAboutBirthday(userId, false, function(result) {
                if (result.error != null) {
                    alert(result.error.Message);
                    return false;
                }
                jq(userCard).removeClass("active");
            });
        }

    };
})(jQuery);