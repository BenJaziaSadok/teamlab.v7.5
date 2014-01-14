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

window.ASC.Files.Tree = (function () {
    var isInit = false;

    var pathParts = new Array();
    var folderIdCurrentRoot = null;

    var init = function () {
        if (isInit === false) {
            isInit = true;

            ASC.Files.ServiceManager.bind(ASC.Files.TemplateManager.events.GetTreeSubFolders, onGetTreeSubFolders);
        }
    };

    var getTreeNode = function (folderId) {
        return jq("#treeViewContainer .tree-node[data-id=\"" + folderId + "\"]");
    };

    var getStreeNode = function (folderId) {
        return jq("#treeViewSelector .stree-node[data-id=\"" + folderId + "\"]");
    };

    var getFolderId = function (treeNode) {
        return jq(treeNode).attr("data-id");
    };

    var getFolderTitle = function (folderId) {
        return ASC.Files.Tree.getStreeNode(folderId).children("a").text().trim();
    };

    var getParentsTreeNode = function (folderId) {
        var treeNode = ASC.Files.Tree.getTreeNode(folderId);
        return treeNode.parents("#treeViewContainer .tree-node");
    };

    var getParentId = function (folderId) {
        var parentsNode = ASC.Files.Tree.getParentsTreeNode(folderId);
        if (!parentsNode.length) {
            return 0;
        }

        return ASC.Files.Tree.getFolderId(parentsNode[0]);
    };

    var registerHideTree = function (event) {
        if (!jq((event.target) ? event.target : event.srcElement).parents().addBack()
            .is("#treeViewPanelSelector, #foldersMoveto, #filesMoveto, #foldersCopyto, #filesCopyto,\
                 #foldersRestore, #filesRestore, #buttomMoveto, #buttomCopyto, #buttomRestore,\
                 #mainMove, #mainCopy, #mainRestore, #importToFolderBtn")) {
            ASC.Files.Actions.hideAllActionPanels();
            jq("body").unbind("click", ASC.Files.Tree.registerHideTree);
        }
    };

    var showMoveToSelector = function (isCopy) {
        if (ASC.Files.Import) {
            ASC.Files.Import.isImport = false;
        }
        ASC.Files.Folders.isCopyTo = (isCopy === true);

        if (ASC.Files.Folders.folderContainer != "trash"
            && !ASC.Files.Folders.isCopyTo) {
            if (!ASC.Files.UI.accessibleItem()) {
                var listTrackAccess = jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):has(.checkbox input:checked)");

                for (var i = 0; i < listTrackAccess.length && !ASC.Files.UI.accessAdmin(jq(listTrackAccess[i])) ; i++) {
                    ;
                }

                if (i >= listTrackAccess.length) {
                    return;
                }
            }
        }

        ASC.Files.Tree.updateTreePath();
        ASC.Files.Tree.showSelect(ASC.Files.Folders.currentFolder.id);
        ASC.Files.Actions.hideAllActionPanels();

        if (!isCopy
            && jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):not(.on-edit):has(.checkbox input:checked)").length == 0) {
            return;
        }

        if (ASC.Files.ThirdParty
            && !ASC.Files.Folders.isCopyTo
            && !ASC.Files.ThirdParty.isThirdParty()
            && jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):not(.third-party-entry):has(.checkbox input:checked)").length == 0) {
            return;
        }

        if (jq("#filesMainContent .file-row:not(.checkloading):not(.new-folder):not(.new-file):not(.error-entry):has(.checkbox input:checked)").length == 0) {
            return;
        }

        jq("#treeViewPanelSelector").removeClass("without-third-party");

        jq.dropdownToggle().toggle(".menuActionSelectAll", "treeViewPanelSelector");

        jq("body").bind("click", ASC.Files.Tree.registerHideTree);
    };

    var updateTreePath = function () {
        var parentId = ASC.Files.Tree.pathParts[0] ? ASC.Files.Tree.pathParts[0].Key : ASC.Files.Constants.FOLDER_ID_MY_FILES;
        var parentNode = ASC.Files.Tree.getTreeNode(parentId);

        if (parentNode.length != 0
            || (ASC.Files.Tree.folderIdCurrentRoot && parentId == ASC.Files.Constants.FOLDER_ID_PROJECT)) {
            for (var i = 1; i < ASC.Files.Tree.pathParts.length; i++) {
                var curNodeId = ASC.Files.Tree.pathParts[i].Key;
                var treeNode = ASC.Files.Tree.getTreeNode(curNodeId);

                if (treeNode.length == 0) {
                    ASC.Files.Tree.getTreeSubFolders(parentId, true);
                } else {
                    parentNode.addClass("jstree-open").removeClass("jstree-closed");

                    var streeNode = ASC.Files.Tree.getStreeNode(parentId);
                    streeNode.children(".jstree-icon").parent().addClass("jstree-open").removeClass("jstree-closed");
                }

                parentId = curNodeId;
                parentNode = treeNode;
            }
        }
        ASC.Files.Tree.select(ASC.Files.Tree.getTreeNode(ASC.Files.Folders.currentFolder.id));
        ASC.Files.Tree.showSelect(ASC.Files.Folders.currentFolder.id);
    };

    var renderTreeView = function (folderId, xmlData) {
        var htmlData = ASC.Files.TemplateManager.translate(xmlData);

        var treeNode = jq("#treeViewContainer");
        if (folderId !== ASC.Files.Constants.FOLDER_ID_PROJECT || !ASC.Files.Tree.folderIdCurrentRoot) {
            treeNode = ASC.Files.Tree.getTreeNode(folderId);
        }
        var streeNode = ASC.Files.Tree.getStreeNode(folderId);

        treeNode.addClass("jstree-open").removeClass("jstree-closed");
        streeNode.addClass("jstree-open").removeClass("jstree-closed");

        if (htmlData != "") {
            treeNode.removeClass("jstree-empty").append("<ul>" + htmlData + "</ul>");
            treeNode.find("ul a").each(function () {
                var hash = ASC.Files.Tree.getFolderId(this);
                hash = "#" + ASC.Files.Anchor.fixHash(hash);
                jq(this).attr("href", hash);
            });

            var shtmlData = htmlData.replace(/tree-node/g, "stree-node");
            streeNode.removeClass("jstree-empty").append("<ul>" + shtmlData + "</ul>");
            streeNode.find("ul a").each(function () {
                var hash = ASC.Files.Tree.getFolderId(this);
                hash = "#" + ASC.Files.Anchor.fixHash(hash);
                jq(this).attr("href", hash);
            });
        } else {
            treeNode.addClass("jstree-empty");
            streeNode.addClass("jstree-empty");
        }
    };

    var select = function (treeNode) {
        jq("#treeViewContainer a.selected").removeClass("selected");
        jq("#treeViewSelector a.selected").removeClass("selected");
        jq("#treeViewContainer .parent-selected").removeClass("parent-selected");
        jq("#treeViewSelector .parent-selected").removeClass("parent-selected");

        treeNode.children("a").addClass("selected");

        treeNode
            .parents("#treeViewContainer .jstree-closed")
            .addClass("jstree-open")
            .removeClass("jstree-closed");

        treeNode
            .parents("#treeViewContainer li.tree-node")
            .addClass("parent-selected");

        if (treeNode && treeNode.offset()) {
            var nodeY = treeNode.offset().top;
            nodeY -= jq("#treeViewContainer").offset().top;
            nodeY += jq("#treeViewContainer").scrollTop();
            nodeY -= 30;

            jq("#treeViewContainer").scrollTop(nodeY);
        }
    };

    var showSelect = function (folderId) {
        ASC.Files.Tree.getStreeNode(folderId)
            .children("a")
            .addClass("selected")
            .parents("#treeViewSelector li.stree-node")
            .addClass("parent-selected");

        ASC.Files.Tree.getTreeNode(folderId)
            .parents("#treeViewSelector .jstree-closed")
            .addClass("jstree-open").removeClass("jstree-closed");
    };

    var selectS = function (treeNode) {
        var folderId = ASC.Files.Tree.getFolderId(treeNode);

        if (ASC.Files.Import && ASC.Files.Import.isImport == true) {
            var importToFolder = "";
            var liArray = treeNode.parents(".stree-node").addBack();
            for (var j = 0; j < liArray.length; j++) {
                if (j > 0) {
                    importToFolder += " > ";
                }
                importToFolder += jq(liArray[j]).children("a").text().trim();
            }

            ASC.Files.Import.setFolderImportTo(folderId, importToFolder);
        } else {
            var titleDf = ASC.Files.Tree.getStreeNode(folderId).children("a").text().trim();

            var pathDest = ASC.Files.Tree.getStreeNode(folderId).parents("li").map(function () {
                return ASC.Files.Tree.getFolderId(jq(this));
            });
            pathDest = jq.makeArray(pathDest);
            pathDest.unshift(folderId);

            ASC.Files.Folders.curItemFolderMoveTo(folderId, titleDf, pathDest);
        }
    };

    var expand = function (treeNode) {
        if (treeNode.children().is("ul")) {
            treeNode.toggleClass("jstree-closed").toggleClass("jstree-open");
        } else {
            var folderId = ASC.Files.Tree.getFolderId(treeNode);
            ASC.Files.Tree.getTreeSubFolders(folderId);
        }
    };

    var resetNode = function (folderId) {
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

    var clickOnNode = function (treeNode) {
        var folderId = ASC.Files.Tree.getFolderId(treeNode);

        if (ASC.Files.Tree.folderIdCurrentRoot
            && ASC.Files.Tree.folderIdCurrentRoot != ASC.Files.Constants.FOLDER_ID_PROJECT) {

            var toParent = jq(ASC.Files.Tree.pathParts).is(function () {
                return this.Key == folderId;
            });
            if (!toParent) {
                var treeNodes = ASC.Files.Tree.getParentsTreeNode(folderId);
                var toChild = treeNodes.is(function () {
                    var nodeId = ASC.Files.Tree.getFolderId(jq(this));
                    return nodeId == ASC.Files.Folders.currentFolder.id;
                });

                if (!toChild) {
                    var rootNode = treeNode;
                    if (treeNodes.length) {
                        rootNode = treeNodes[treeNodes.length - 1];
                    }
                    var href = jq(rootNode).attr("data-href");
                    href = href.substring(0, href.lastIndexOf("#"));
                    href = href + "#" + folderId;
                    location.href = href;
                    return false;
                }
            }
        }

        ASC.Files.Tree.select(treeNode);

        ASC.Files.Anchor.navigationSet(folderId);
        return false;
    };

    //request

    var getTreeSubFolders = function (folderId, ajaxsync) {
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.jstree-expander").addClass("jstree-load-node");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.jstree-expander").addClass("jstree-load-node");

        ASC.Files.ServiceManager.request("post",
            "xml",
            ASC.Files.TemplateManager.events.GetTreeSubFolders,
            { folderId: folderId, ajaxsync: (ajaxsync === true) },
            { orderBy: ASC.Files.Filter.getOrderByAZ(true) },
            "folders", "subfolders?parentId=" + encodeURIComponent(folderId));
    };

    //event handler

    var onGetTreeSubFolders = function (xmlData, params, errorMessage) {
        var folderId = params.folderId;
        ASC.Files.Tree.getTreeNode(folderId).find(".jstree-icon.jstree-expander").removeClass("jstree-load-node");
        ASC.Files.Tree.getStreeNode(folderId).find(".jstree-icon.jstree-expander").removeClass("jstree-load-node");
        if (typeof errorMessage != "undefined") {
            ASC.Files.UI.displayInfoPanel(errorMessage, true);
            return;
        }

        var treeNodeUl = jq("#treeViewContainer");
        if (folderId !== ASC.Files.Constants.FOLDER_ID_PROJECT || !ASC.Files.Tree.folderIdCurrentRoot) {
            treeNodeUl = ASC.Files.Tree.getTreeNode(folderId).find("ul");
        }

        if (treeNodeUl.html() != null && treeNodeUl.html().trim() != "") {
            ASC.Files.Tree.resetNode(folderId);
        }

        renderTreeView(folderId, xmlData);
        ASC.Files.Tree.getTreeNode(folderId)
            .addClass("jstree-open").removeClass("jstree-closed");
        ASC.Files.Tree.getStreeNode(folderId)
            .addClass("jstree-open").removeClass("jstree-closed");

        if (folderId == ASC.Files.Folders.currentFolder.id) {
            ASC.Files.Tree.select(ASC.Files.Tree.getTreeNode(folderId));
            ASC.Files.Tree.showSelect(folderId);
        }
    };

    return {
        init: init,
        expand: expand,
        resetNode: resetNode,

        clickOnNode: clickOnNode,

        select: select,
        selectS: selectS,

        getTreeNode: getTreeNode,
        getStreeNode: getStreeNode,
        getFolderId: getFolderId,
        getFolderTitle: getFolderTitle,

        getParentsTreeNode: getParentsTreeNode,
        getParentId: getParentId,

        pathParts: pathParts,
        folderIdCurrentRoot:folderIdCurrentRoot,

        showSelect: showSelect,
        showMoveToSelector: showMoveToSelector,

        registerHideTree: registerHideTree,

        updateTreePath: updateTreePath,
        getTreeSubFolders: getTreeSubFolders
    };
})();

(function ($) {
    ASC.Files.Tree.init();
    $(function () {
        jq("#buttomMoveto, #buttomCopyto, #buttomRestore,\
            #mainMove, #mainCopy, #mainRestore").click(function () {
                if (!jq(this).is(".menuAction:not(.unlockAction)")) {
                    ASC.Files.Tree.showMoveToSelector(this.id == "buttomCopyto" || this.id == "mainCopy");
                }
            });

        jq("#filesMoveto, #filesRestore, #filesCopyto,\
            #foldersMoveto, #foldersRestore, #foldersCopyto").click(function () {
                ASC.Files.Actions.hideAllActionPanels();
                ASC.Files.Tree.showMoveToSelector(this.id == "filesCopyto" || this.id == "foldersCopyto");
            });

        jq("#treeViewContainer, #treeViewSelector").on("click", ".jstree-icon.jstree-expander", function () {
            ASC.Files.Tree.expand(jq(this).parent());
            return false;
        });

        jq("#treeViewContainer").on("click", "a", function () {
            var treeNode = jq(this).parent();
            ASC.Files.Tree.clickOnNode(treeNode);
            return false;
        });

        jq("#studio_sidePanel .page-menu").on("click", ".is-new", function () {
            var target = jq(this);
            var folderId = target.attr("data-id");
            ASC.Files.Marker.getNews(target, folderId);
            return false;
        });

        jq("#treeViewSelector").on("click", "a", function () {
            var treeNode = jq(this).parent();
            if (!treeNode.hasClass("access-read")) {
                ASC.Files.Tree.selectS(treeNode);
                return false;
            } else {
                var errorString = ASC.Files.FilesJSResources.ErrorMassage_SecurityException;
                var folderId = ASC.Files.Tree.getFolderId(treeNode);
                if (folderId == ASC.Files.Constants.FOLDER_ID_PROJECT
                    || folderId == ASC.Files.Constants.FOLDER_ID_SHARE) {
                    errorString = ASC.Files.FilesJSResources.ErrorMassage_SecurityException_PrivateRoot;
                }
                ASC.Files.UI.displayInfoPanel(errorString, true);
                ASC.Files.Tree.expand(treeNode);
                return false;
            }
        });

        if (jq("#contentPanel").attr("data-rootid")) {
            ASC.Files.Tree.folderIdCurrentRoot = jq("#contentPanel").attr("data-rootid");
            ASC.Files.Tree.getTreeSubFolders(ASC.Files.Constants.FOLDER_ID_PROJECT);
        }
        jq("#studio_sidePanel .page-menu .is-new").trackEvent("files_tree", "action-click", "mark_as_read");

    });
})(jQuery);