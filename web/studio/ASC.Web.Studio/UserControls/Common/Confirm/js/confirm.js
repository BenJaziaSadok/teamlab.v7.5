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

StudioConfirm = new function() {
    this.OpenDialog = function(additionalID) {
        jq("#studio_confirmMessage").html('');
        StudioBlockUIManager.blockUI("#studio_confirmDialog" + additionalID, 400, 300, 0);
        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = jq('[id$="_confirmEnterCode"]').val();
    };
    this.Cancel = function() {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };
    this.SelectCallback = function() {
        alert('empty callback');
    };
    this.Select = function(additionalID, callback) {
        callback(jq("#studio_confirmInput" + additionalID).val());
    };
}