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

var PromoCodeManagement = new function () {

    this.ActivateKey = function () {
        AjaxPro.onLoading = function (b) {
            if (b)
                jq('#promoCodeSettings').block();
            else
                jq('#promoCodeSettings').unblock();
        };

        var promocode = jq('#promoCodeSettings_input').val();
        if (promocode && promocode.trim().length !== 0) {
            PromoCodeController.ActivateKey(promocode, function (result) {
                if (result.value.Status == '1') {
                    jq('#promoCodeSettingsInfo').html('<div class="okBox">' + result.value.Message + '</div>');
                    setTimeout(function () { window.location.reload(true) }, 3000);
                }
                else {
                    jq('#promoCodeSettingsInfo').html('<div class="errorBox">' + result.value.Message + '</div>');
                    StudioManagement.TimeoutHandler = setTimeout("jq('#promoCodeSettingsInfo').html('');", 5000);
                }
            });
        }
    };
};