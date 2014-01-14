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

var DefaultPage = new function() {
    this.SaveSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);


        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_defaultPegeSettings').block();
            else
                jq('#studio_defaultPegeSettings').unblock();
        };

        var selectedProductID = jq("input[name='defaultPage']:checked").val();
        
        DefaultPageController.SaveSettings(selectedProductID, function(result) {

            var res = result.value;
            if (res.Status == 1)
                jq('#studio_defaultPegeSettings').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_defaultPegeSettings').html('<div class="errorBox">' + res.Message + '</div>');
            DefaultPage.TimeoutHandler = setTimeout(function() { jq('#studio_defaultPegeSettings').html(''); }, 4000);
        });
    }
}