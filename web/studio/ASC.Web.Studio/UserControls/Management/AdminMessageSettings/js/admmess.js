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

var AdmMess = new function() {
    this.SaveSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);


        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_admMessSettingsInfo').block();
            else
                jq('#studio_admMessSettingsInfo').unblock();
        };

        AdmMessController.SaveSettings(jq("#chk_studio_admMess").is(":checked"), function(result) {

            var res = result.value;
            if (res.Status == 1)
                jq('#studio_admMessSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_admMessSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
            AdmMess.TimeoutHandler = setTimeout(function() { jq('#studio_admMessSettingsInfo').html(''); }, 4000);
        });
    }
}