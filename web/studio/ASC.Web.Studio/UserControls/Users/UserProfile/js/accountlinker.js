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

jq(function() {
    jq('#accountLinks').delegate('.popup', 'click', function() {
        var obj = jq(this);
        if (obj.hasClass('linked')) {
            //unlink
            var res = AccountLinkControl.UnlinkAccount(obj.attr('id'));
            jq('#accountLinks').html(res.value.rs1);
        }
        else {
            var link = obj.attr('href');
            window.open(link, 'login', 'width=800,height=500,status=no,toolbar=no,menubar=no,resizable=yes,scrollbars=no');
        }
        return false;
    });
});

function loginCallback(profile) {
    var res = AccountLinkControl.LinkAccount(profile.Serialized);
    jq('#accountLinks').html(res.value.rs1);
}

function authCallback(profile) {
    __doPostBack('signInLogin', profile.HashId);
}

function loginJoinCallback(profile) {
    __doPostBack('thirdPartyLogin', profile.Serialized);
}