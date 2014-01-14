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

window.ASC.Files.EmptyScreen = (function () {
    var isInit = false;

    var init = function () {
        if (isInit === false) {
            isInit = true;
        }
        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintCreate",
            dropdownID: "hintCreatePanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintUpload",
            dropdownID: "hintUploadPanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintOpen",
            dropdownID: "hintOpenPanel",
            fixWinSize: false
        });

        jq.dropdownToggle({
            switcherSelector: "#emptyContainer .hintEdit",
            dropdownID: "hintEditPanel",
            fixWinSize: false
        });
    };

    var displayEmptyScreen = function () {
        ASC.Files.UI.hideAllContent(true);
        ASC.Files.Mouse.finishMoveTo();
        ASC.Files.Mouse.finishSelecting();

        jq("#filesMainContent, #switchViewFolder, #mainContentHeader, #pageNavigatorHolder, #toParentFolder").hide();
        jq("#emptyContainer > div").hide();

        var filter = ASC.Files.Filter.getFilterSettings();
        if (filter.filter == 0 && filter.text == "") {
            jq("#filterContainer").hide();
            if (ASC.Files.UI.accessibleItem()) {
                jq("#emptyContainer .empty-folder-create").css("display", "");
            } else {
                jq("#emptyContainer .empty-folder-create").hide();
            }

            if (ASC.Files.Constants.YOUR_DOCS_DEMO && ASC.Files.Folders.currentFolder.id == ASC.Files.Constants.FOLDER_ID_MY_FILES) {
                jq("#emptyContainer_thirdParty").show();
            } else {
                jq("#emptyContainer_" + ASC.Files.Folders.folderContainer).show();
            }

            ASC.Files.UI.checkButtonBack(".empty-folder-toparent");
        } else {
            jq("#emptyContainer_filter").show();
        }

        jq("#emptyContainer").show();
        ASC.Files.UI.stickContentHeader();
    };

    var hideEmptyScreen = function () {
        if (jq("#filesMainContent").is(":visible")) {
            return;
        }
        ASC.Files.UI.hideAllContent(true);
        ASC.Files.Mouse.finishMoveTo();
        ASC.Files.Mouse.finishSelecting();

        jq("#emptyContainer").hide();

        ASC.Files.UI.checkButtonBack("#toParentFolderLink", "#toParentFolder");

        jq("#filterContainer").show();
        ASC.Files.Filter.resize();

        jq("#filesMainContent, #switchViewFolder, #mainContentHeader").show();
        ASC.Files.UI.stickContentHeader();
    };

    return {
        init: init,

        hideEmptyScreen: hideEmptyScreen,
        displayEmptyScreen: displayEmptyScreen
    };
})();

(function ($) {
    ASC.Files.EmptyScreen.init();
    $(function () {
    });
})(jQuery);