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

window.ASC.Files.Tree = (function() {
    var isInit = false;

    var pathParts = new Array();

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }
    };

    var getTreeNode = function(folderId) {
        return jq("#treeViewContainer .tree-node[data-id=\"" + folderId + "\"]");
    };

    var getStreeNode = function(folderId) {
        return jq("#treeViewSelector .stree-node[data-id=\"" + folderId + "\"]");
    };

    var getFolderId = function(treeNode) {
        return jq(treeNode).attr("data-id");
    };

    var getFolderTitle = function(folderId) {
        return ASC.Files.Tree.getStreeNode(folderId).children("a").text().trim();
    };

    var getParentId = function(folderId) {
        var treeNode = ASC.Files.Tree.getTreeNode(folderId);
        var parentsNode = treeNode.parents("#treeViewContainer .tree-node");
        if (!parentsNode.length) {
            return 0;
        }

        return ASC.Files.Tree.getFolderId(parentsNode[0]);
    };

    var registerHideTree = function(event) {
        if (!jq((event.target) ? event.target : event.srcElement).parents().addBack()
            .is("#treeViewPanelSelector, #foldersMoveto, #filesMoveto, #foldersCopyto, #filesCopyto,\
                 #foldersRestore, #filesRestore, #buttomMoveto, #buttomCopyto, #buttomRestore,\
                 #mainMove, #mainCopy, #mainRestore, #importToFolderBtn")) {
            ASC.Files.Actions.hideAllActionPanels();
            jq("body").unbind("click", ASC.Files.Tree.registerHideTree);
        }
    };

    var renderTreeView = function(folderId, response) {
        var htmlData = jq('#getfolderstree').tmpl(response[1]).html().trim();

        var treeNode = jq("#treeViewContainer");
        if (folderId !== ASC.Files.Constants.FOLDER_ID_PROJECT || !ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT) {
            treeNode = ASC.Files.Tree.getTreeNode(folderId);
        }
        var streeNode = ASC.Files.Tree.getStreeNode(folderId);

        treeNode.addClass("jstree-open").removeClass("jstree-closed");
        streeNode.addClass("jstree-open").removeClass("jstree-closed");

        if (htmlData != "") {
            treeNode.removeClass("jstree-empty").append("<ul>" + htmlData + "</ul>");
            var shtmlData = htmlData.replace(/tree-node/g, "stree-node");
            streeNode.removeClass("jstree-empty").append("<ul>" + shtmlData + "</ul>");
        } else {
            treeNode.addClass("jstree-empty");
            streeNode.addClass("jstree-empty");
        }
    };

    var select = function(treeNode) {
        jq("#treeViewContainer a").removeClass("selected");
        jq("#treeViewSelector a").removeClass("selected");

        treeNode.children("a").addClass("selected");

        treeNode.parents("#treeViewContainer .jstree-closed")
            .addClass("jstree-open").removeClass("jstree-closed");

        if (treeNode && treeNode.offset()) {
            var nodeY = treeNode.offset().top;
            nodeY -= jq("#treeViewContainer").offset().top;
            nodeY += jq("#treeViewContainer").scrollTop();
            nodeY -= jq("#treeViewContainer").height() / 2;
            jq("#treeViewContainer").scrollTop(nodeY);
        }
    };

    var showSelect = function(folderId) {
        ASC.Files.Tree.getStreeNode(folderId).children("a").addClass("selected");

        ASC.Files.Tree.getTreeNode(folderId)
            .parents("#treeViewSelector .jstree-closed")
            .addClass("jstree-open").removeClass("jstree-closed");
    };

    var expand = function(treeNode) {
        var folderId = ASC.Files.Tree.getFolderId(treeNode);

        if (treeNode.children().is("ul")) {
            treeNode.toggleClass("jstree-closed").toggleClass("jstree-open");
        } else {
            ASC.Files.Tree.getTreeSubFolders(folderId);
        }
    };

    var resetNode = function(folderId) {
        if (!ASC.Files.Common.isCorrectId(folderId)) {
            return;
        }

        ASC.Files.Tree.getTreeNode(folderId)
            .addClass("jstree-closed").removeClass("jstree-open jstree-empty")
            .children("ul").remove();

        ASC.Files.Tree.getStreeNode(folderId)
            .addClass("jstree-closed").removeClass("jstree-open jstree-empty")
            .children("ul").remove();
    };

    //request

    var getTreeSubFolders = function(folderId) {
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.jstree-expander").addClass("jstreeLoadNode");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.jstree-expander").addClass("jstreeLoadNode");

        Teamlab.getDocFolder(null, folderId, function() {
            onGetTreeSubFolders(arguments, { folderId: folderId });
        });
    };

    //event handler

    var onGetTreeSubFolders = function(xmlData, params, errorMessage) {
        var folderId = params.folderId;
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.jstree-expander").removeClass("jstreeLoadNode");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.jstree-expander").removeClass("jstreeLoadNode");
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        var treeNodeUl = jq("#treeViewContainer");
        if (folderId !== ASC.Files.Constants.FOLDER_ID_PROJECT || !ASC.Files.Constants.FOLDER_ID_CURRENT_ROOT) {
            treeNodeUl = ASC.Files.Tree.getTreeNode(folderId).find("ul");
        }

        if (treeNodeUl.html() == null || treeNodeUl.html().trim() == "") {
            renderTreeView(folderId, xmlData);
            ASC.Files.Tree.getTreeNode(folderId)
                .addClass("jstree-open").removeClass("jstree-closed");
            ASC.Files.Tree.getStreeNode(folderId)
                .addClass("jstree-open").removeClass("jstree-closed");
        }
    };

    return {
        init: init,
        expand: expand,
        resetNode: resetNode,
        select: select,
        getTreeNode: getTreeNode,
        getStreeNode: getStreeNode,
        getFolderId: getFolderId,
        getFolderTitle: getFolderTitle,
        getParentId: getParentId,
        pathParts: pathParts,
        showSelect: showSelect,
        registerHideTree: registerHideTree,
        getTreeSubFolders: getTreeSubFolders
    };
})();

(function ($) {
    ASC.Files.Tree.init();
    $(function () {

        jq("#treeViewContainer").on("click", ".jstree-icon.jstree-expander", function () {
            ASC.Files.Tree.expand(jq(this).parent());
            return false;
        });

        jq("#treeViewContainer").on("click", "a", function () {
            var treeNode = jq(this).parent();
            ASC.Files.Tree.select(treeNode);
            return false;
        });

        jq("#treeViewSelector").on("click", ".jstree-icon.jstree-expander", function () {
            ASC.Files.Tree.expand(jq(this).parent());
            return false;
        });
    });
})(jQuery);