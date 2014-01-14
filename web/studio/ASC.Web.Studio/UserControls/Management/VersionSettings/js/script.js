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

var StudioVersionManagement = new function() {

    this.SwitchVersion = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_versionSetting').block();
            else
                jq('#studio_versionSetting').unblock();
        };
        VersionSettingsController.SwitchVersion(jq('#versionSelector  input:radio[name=version]:checked').val(), function(res) {
            if (res.value.Status == '1') {
                jq('#studio_versionSetting').block();
                setTimeout(function() { window.location.reload(true) }, 5000);
            } else {
                jq('#studio_versionSetting_info').html('<div class="errorBox">' + res.value.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_versionSetting_info').html('');", 3000);
            }
        });
    };
};