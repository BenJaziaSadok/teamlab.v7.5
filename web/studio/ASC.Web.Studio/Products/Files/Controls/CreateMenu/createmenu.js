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

window.ASC.Files.CreateMenu = (function () {
    var isInit = false;

    var init = function () {
        if (isInit === false) {
            isInit = true;
        }

        jq.dropdownToggle({
            switcherSelector: "#menuCreateNewButton:not(.disable)",
            dropdownID: "newDocumentPanel"
        });

        jq.dropdownToggle({
            switcherSelector: "#buttonThirdparty",
            dropdownID: "thirdPartyListPanel"
        });
    };

    var updateCreateDocList = function () {
        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Utility.Resource.InternalFormats.Document)) {
            jq("#createDocument").remove();
            jq("#emptyContainer .empty-folder-create-document").remove();
        }

        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Utility.Resource.InternalFormats.Spreadsheet)) {
            jq("#createSpreadsheet").remove();
            jq("#emptyContainer .empty-folder-create-spreadsheet").remove();
        }

        if (!ASC.Files.Utility.CanWebEdit(ASC.Files.Utility.Resource.InternalFormats.Presentation)) {
            jq("#createPresentation").remove();
            jq("#emptyContainer .empty-folder-create-presentation").remove();
        }

        if (!jq(".empty-folder-create-editor a").length) {
            jq(".empty-folder-create-editor").remove();
        }
    };

    var disableMenu = function (enable) {
        var listButtons = jq("#buttonUpload, #createDocument, #createSpreadsheet, #createPresentation, #createNewFolder" +
            (!ASC.Files.Tree.folderIdCurrentRoot
                ? ", .page-menu .menu-actions .menu-main-button"
                : ""));

        listButtons.toggleClass("disable", !enable);
        ASC.Files.ChunkUploads.disableBrowseButton(!enable);
    };

    return {
        init: init,
        updateCreateDocList: updateCreateDocList,

        disableMenu: disableMenu
    };
})();

(function ($) {
    ASC.Files.CreateMenu.init();

    $(function () {
        ASC.Files.CreateMenu.updateCreateDocList();

        jq(document).on("click", "#createDocument:not(.disable), #createSpreadsheet:not(.disable), #createPresentation:not(.disable)", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.typeNewDoc = this.id.replace("create", "").toLowerCase();
            ASC.Files.Folders.createNewDoc();
        });

        jq("#emptyContainer .empty-folder-create a").click(function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.typeNewDoc = (
                jq(this).hasClass("empty-folder-create-document")
                    ? "document"
                    : (jq(this).hasClass("empty-folder-create-spreadsheet")
                        ? "spreadsheet"
                        : (jq(this).hasClass("empty-folder-create-presentation")
                            ? "presentation"
                            : ""
                        )));
            ASC.Files.Folders.createNewDoc();
        });

        jq(document).on("click", "#createNewFolder:not(.disable), #emptyContainer .empty-folder-create-folder", function () {
            ASC.Files.Actions.hideAllActionPanels();
            ASC.Files.Folders.createFolder();
        });
    });
})(jQuery);