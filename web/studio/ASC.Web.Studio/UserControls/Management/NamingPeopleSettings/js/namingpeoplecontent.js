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

NamingPeopleContentManager = function() {

    this.SaveSchema = function(parentCallback) {
        var schemaId = jq('#namingPeopleSchema').val();

        if (schemaId == 'custom') {
            NamingPeopleContentController.SaveCustomNamingSettings(jq('#usrcaption').val().substring(0, 30), jq('#usrscaption').val().substring(0, 30),
                                                       jq('#grpcaption').val().substring(0, 30), jq('#grpscaption').val().substring(0, 30),
                                                       jq('#usrstatuscaption').val().substring(0, 30), jq('#regdatecaption').val().substring(0, 30),
                                                       jq('#grpheadcaption').val().substring(0, 30),
                                                       jq('#guestcaption').val().substring(0, 30), jq('#guestscaption').val().substring(0, 30),
                                                       function(result) { if (parentCallback != null) parentCallback(result.value); });
        }
        else
            NamingPeopleContentController.SaveNamingSettings(schemaId, function(result) { if (parentCallback != null) parentCallback(result.value); });
    }

    this.SaveSchemaCallback = function(res) {
    }

    this.LoadSchemaNames = function(parentCallback) {

        var schemaId = jq('#namingPeopleSchema').val();
        NamingPeopleContentController.GetPeopleNames(schemaId, function(res) {
            var names = res.value;

            jq('#usrcaption').val(names.UserCaption);
            jq('#usrscaption').val(names.UsersCaption);
            jq('#grpcaption').val(names.GroupCaption);
            jq('#grpscaption').val(names.GroupsCaption);
            jq('#usrstatuscaption').val(names.UserPostCaption);
            jq('#regdatecaption').val(names.RegDateCaption);
            jq('#grpheadcaption').val(names.GroupHeadCaption);
            jq('#guestcaption').val(names.GuestCaption);
            jq('#guestscaption').val(names.GuestsCaption);

            if (parentCallback != null)
                parentCallback(res.value);
        });
    }
}

NamingPeopleContentViewer = new function() {
    this.ChangeValue = function(event) {
    jq('#namingPeopleSchema').val('custom');
    }
};

jq(document).ready(function() {
    jq('.namingPeopleBox input[type="text"]').each(function(i, el) {
        jq(el).keypress(function(event) { NamingPeopleContentViewer.ChangeValue(); });
    });
    var manager = new NamingPeopleContentManager();
	jq('#namingPeopleSchema').change(function () {
		manager.LoadSchemaNames(null);
	});
    manager.LoadSchemaNames(null);
});