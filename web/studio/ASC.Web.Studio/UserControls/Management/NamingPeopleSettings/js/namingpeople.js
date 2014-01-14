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

NamingPeopleManager = new function() {

    this.MessageTimer = null;

    this.SaveSchema = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_namingPeopleBox').block();
            else
                jq('#studio_namingPeopleBox').unblock();
        };

        if (NamingPeopleManager.MessageTimer != null)
            clearTimeout(NamingPeopleManager.MessageTimer);
            
        var namingContentManager = new NamingPeopleContentManager();
        namingContentManager.SaveSchema(NamingPeopleManager.SaveSchemaCallback);
    }
    this.SaveSchemaCallback = function(result) {
        if (result.Status == 1)
            jq('#studio_namingPeopleInfo').html('<div class="okBox">' + result.Message + '</div>')
        else
            jq('#studio_namingPeopleInfo').html('<div class="errorBox">' + result.Message + '</div>')

        NamingPeopleManager.MessageTimer = setTimeout(function() { jq('#studio_namingPeopleInfo').html('') }, 4000);
    }
};

jq(function() {
    var namingContentManager = new NamingPeopleContentManager();
    jq('#saveNamingPeopleBtn').click(NamingPeopleManager.SaveSchema);

   
});