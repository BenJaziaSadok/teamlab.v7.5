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


jq(document).ready(function() {
bindEvent('studio_confirm_Email');
bindEvent('studio_confirm_FirstName');
bindEvent('studio_confirm_LastName');
bindEvent('studio_confirm_pwd');
bindEvent('studio_confirm_repwd');
});

function bindEvent(controlId) {
    jq('#' + controlId).keyup(function(event) {
        var code;
        if (!e) var e = event;
        if (e.keyCode) code = e.keyCode;
        else if (e.which) code = e.which;

        if (code == 13) {
            if (controlId == 'studio_confirm_repwd') {
                //do postback
                AuthManager.ConfirmInvite();
            } else {
                //set focus to next field
                var inputs = jq(".rightPart").find(".property .value input");
                var index = 0;
                inputs.each(function (indx, item) {
                    if (jq(item).attr("id") == controlId) {
                        index = indx;
                    }
                });
                var input = jq(inputs[index + 1]);
                input.focus();
                //set focus to end of text
                var tmpStr = input.val();
                input.val("");
                input.val(tmpStr);
            }
        }
    });
}