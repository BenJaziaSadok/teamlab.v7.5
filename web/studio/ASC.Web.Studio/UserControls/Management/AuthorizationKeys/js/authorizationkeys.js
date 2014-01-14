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

AuthorizationKeysManager = new function () {
    this.Initialize = function () {
        jq('#authKeysButtonSave').click(function () {
            AuthorizationKeysManager.Save();
        });
    };

    this.Save = function () {
        var authKeys = [];

        var ids = jq('.auth-service-id');
        for (var i = 0; i < ids.length; i++) {
            authKeys.push({ Name: ids[i].id, Value: ids[i].value });
        }

        var keys = jq('.auth-service-key');
        for (i = 0; i < keys.length; i++) {
            authKeys.push({ Name: keys[i].id, Value: keys[i].value });
        }

        jq('.auth-service-block .errorBox').addClass('display-none');
        jq('#authKeysContainer').block();
        AuthorizationKeys.SaveAuthKeys(authKeys, function(result) {
            if (result.error != null) {
                jq('.auth-service-block .errorBox').text(result.error.Message);
                jq('.auth-service-block .errorBox').removeClass('display-none');
            }
            jq('#authKeysContainer').unblock();
        });
    };
};

jq(function() {
    AuthorizationKeysManager.Initialize();
});