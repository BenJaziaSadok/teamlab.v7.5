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

window.ASC.Controls.LoadPhotoImage = function () {

    var showPhotoDialog = function () {
        StudioBlockUIManager.blockUI("#divLoadPhotoWindow", 520, 550, 0);
    }
    var setDefaultPhoto = function (userId) {       

        PopupKeyUpActionProvider.CloseDialog();
        jq(".profile-action-usericon").unblock();
        jq("#userProfilePhotoError").html("");
        
        var data = { userid: userId };
        Teamlab.removeUserPhoto({}, userId, data, {
            before: LoadingBanner.displayLoading,
            error: LoadingBanner.hideLoading,
            success: function (params, data) {
                jq('#userProfilePhotoError').html('');
                var src = jq("#divLoadPhotoWindow .default-image").attr("data-src");
                jq("#userProfilePhoto").find("img").attr("src", src);
            },
            after: LoadingBanner.hideLoading
        });
    }

    return {
        showPhotoDialog: showPhotoDialog,
        setDefaultPhoto: setDefaultPhoto
    };
}();