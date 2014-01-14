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

if (typeof ASC === "undefined")
    ASC = {};

if (typeof ASC.Settings === "undefined")
    ASC.Settings = {};

ASC.Settings.AccessRights = new function() {

    var timeoutHandler = null;
    var pNameList = [];
    var gsLinkText = "";
    
    var getUserSelector = function (pName) {
        return window["userSelector_" + pName];
    };
    var getGroupSelector = function (pName) {
        return window["groupSelector_" + pName];
    };
    var getSelectedUsers = function (pName) {
        return window["SelectedUsers_" + pName];
    };
    var getSelectedGroups = function (pName) {
        return window["SelectedGroups_" + pName];
    };

    return {
        init: function (products, linkText) {
            pNameList = products;
            gsLinkText = linkText;

            jq("#changeOwnerBtn").click(ASC.Settings.AccessRights.changeOwner);
            jq("#adminTable tbody tr").remove();
            jq("#adminTmpl").tmpl(window.adminList).prependTo("#adminTable tbody");

            for (var i = 0; i < window.adminList.length; i++)
                window.adminSelector.HideUser(window.adminList[i].id);

            var items = jq("[id^=switcherAccessRights]");
            var pName;
            for (var i = 0; i < items.length; i++) {
                pName = jq(items[i]).attr("data-id");
                jq.switcherAction("#switcherAccessRights_" + pName, "#accessRightsContainer_" + pName);
            }

        },

        selectOwner: function() {
            jq("#changeOwnerBtn").removeClass("disable");
        },
            

        changeOwner: function() {
            var ownerId = window.ownerSelector.SelectedUserId;

            if (ownerId == null) return false;

            if (timeoutHandler)
                clearTimeout(timeoutHandler);

            window.AjaxPro.onLoading = function(b) {
                if (b)
                    jq("#ownerSelectorContent").block();
                else
                    jq("#ownerSelectorContent").unblock();
            };

            window.AccessRightsController.ChangeOwner(ownerId, function(result) {
                var res = result.value;
                if (res.Status == 1)
                    jq("#accessRightsInfo").html("<div class='okBox'>" + res.Message + "</div>");
                else
                    jq("#accessRightsInfo").html("<div class='errorBox'>" + res.Message + "</div>");

                timeoutHandler = setTimeout(function() { jq("#accessRightsInfo").html(""); }, 4000);
            });
            return false;
        },

        addAdmin: function(uId) {
            window.AjaxPro.onLoading = function(b) {
                if (b)
                    jq("#adminContent").block();
                else
                    jq("#adminContent").unblock();
            };

            window.AccessRightsController.AddAdmin(uId, function(res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return false;
                }

                jq("#adminTmpl").tmpl(res.value).appendTo("#adminTable tbody");
                ASC.Settings.AccessRights.hideUserFromAll(uId, true);
                window.adminSelector.HideUser(uId);
            });
        },

        changeAccessType: function(obj, pName) {

            var subjects = ASC.Settings.AccessRights.getSubjects(pName);

            var type = jq(obj).attr("id").split("_")[0];
            var id = jq(obj).attr("id").split("_")[1];
            var params = {};
            var data = {
                id: id,
                enabled: true
            };

            if (type == "all") {
                Teamlab.setWebItemSecurity(params, data, {
                    success: function() {
                        jq("#selectorContent_" + pName).hide();
                    }
                });
            } else {
                if (subjects.length == 0)
                    data.enabled = false;
                if (subjects.length > 0)
                    data.subjects = subjects;
                Teamlab.setWebItemSecurity(params, data, {
                    success: function() {
                        jq("#selectorContent_" + pName).show();
                    }
                });
            }
        },

        selectedItem_mouseOver: function(obj) {
            jq(obj).find("img:first").hide();
            jq(obj).find("img:last").show();
        },

        selectedItem_mouseOut: function(obj) {
            jq(obj).find("img:first").show();
            jq(obj).find("img:last").hide();
        },
        
        initProduct: function (pId, pName, pIsPuplic) {
            jq.tmpl("groupSelectorTemplate",
                {
                    selectorID: "groupSelector_" + pName,
                    linkText: gsLinkText
                })
                .appendTo("#selectorContent_" + pName + " .accessRights-selectorContent");

            window["groupSelector_" + pName] = new ASC.Controls.GroupSelector(
                "groupSelector_" + pName,
                jq.browser.mobile,
                true,
                false);

            var us = getUserSelector(pName);
            var gs = getGroupSelector(pName);

            var su = getSelectedUsers(pName);
            var sg = getSelectedGroups(pName);

            us.AdditionalFunction = ASC.Settings.AccessRights.pushUserIntoList;
            gs.AdditionalFunction = ASC.Settings.AccessRights.pushGroupIntoList;

            for (var i = 0; i < su.IDs.length; i++)
                us.HideUser(su.IDs[i]);

            for (var j = 0; j < sg.IDs.length; j++)
                gs.HideGroup(sg.IDs[j]);

            if (pIsPuplic) {
                jq("#all_" + pId).prop("checked", true);
                jq("#emptyUserListLabel_" + pName).show();
                jq("#selectorContent_" + pName).hide();
            } else {
                jq("#fromList_" + pId).prop("checked", true);
                jq("#emptyUserListLabel_" + pName).hide();
                jq("#selectorContent_" + pName).show();
            }
            
            jq("#selectorContent_" + pName).on("mouseover", ".accessRights-selectedItem", function () {
                ASC.Settings.AccessRights.selectedItem_mouseOver(jq(this));
                return false;
            });
            jq("#selectorContent_" + pName).on("mouseout", ".accessRights-selectedItem", function () {
                ASC.Settings.AccessRights.selectedItem_mouseOut(jq(this));
                return false;
            });
            jq("#selectedUsers_" + pName).on("click", "img[id^=deleteSelectedUserImg_]", function () {
                ASC.Settings.AccessRights.deleteUserFromList(jq(this));
                return false;
            });
            jq("#selectedGroups_" + pName).on("click", "img[id^=deleteSelectedGroupImg_]", function () {
                ASC.Settings.AccessRights.deleteGroupFromList(jq(this));
                return false;
            });
        },
        
        pushUserIntoList: function (uId, uName) {
            var pName = this.ObjName.split('_')[1];
            var pId = jq("#accessRightsContainer_" + pName).attr("data-id");
            var alreadyExist = false;

            var su = getSelectedUsers(pName);
            var us = getUserSelector(pName);

            for (var i = 0; i < su.IDs.length; i++)
                if (su.IDs[i] == uId) {
                    alreadyExist = true;
                    break;
                }

            if (alreadyExist) return false;

            su.IDs.push(uId);
            su.Names.push(uName);

            var item = jq("<div></div>")
                .attr("id", "selectedUser_" + pName + "_" + uId)
                .addClass("accessRights-selectedItem");
            
            var peopleImg = jq("<img>")
                .attr("src", su.PeopleImgSrc);
            
            var deleteImg = jq("<img>")
                    .attr("src", su.TrashImgSrc)
                    .css("display", "none")
                    .attr("id", "deleteSelectedUserImg_" + pName + "_" + uId)
                    .attr("title", su.TrashImgTitle);

            item.append(peopleImg).append(deleteImg).append(Encoder.htmlEncode(uName));

            jq("#selectedUsers_" + pName).append(item);
            jq("#emptyUserListLabel_" + pName).hide();

            us.ClearFilter();
            
            jq("#selectedUsers_" + pName).parent().find("div.adv-userselector-DepsAndUsersContainer").hide();

            var data = {
                id: pId,
                enabled: true
            };

            var subjects = ASC.Settings.AccessRights.getSubjects(pName);

            if (subjects.length > 0)
                data.subjects = subjects;

            Teamlab.setWebItemSecurity({}, data, {
                before: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").block();
                },
                success: function () {
                    us.HideUser(uId);
                },
                after: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").unblock();
                }
            });
        },

        pushGroupIntoList: function (group) {
            var pName = this.ID.split('_')[1];
            var pId = jq("#accessRightsContainer_" + pName).attr("data-id");
            var alreadyExist = false;

            var sg = getSelectedGroups(pName);
            var gs = getGroupSelector(pName);

            for (var i = 0; i < sg.IDs.length; i++)
                if (sg.IDs[i] == group.Id) {
                    alreadyExist = true;
                    break;
                }

            if (alreadyExist) return false;

            sg.IDs.push(group.Id);
            sg.Names.push(group.Name);

            var item = jq("<div></div>")
                .attr("id", "selectedGroup_" + pName + "_" + group.Id)
                .addClass("accessRights-selectedItem");
            
            var groupImg = jq("<img>")
                .attr("src", sg.GroupImgSrc);
            
            var deleteImg = jq("<img>")
                .attr("src", sg.TrashImgSrc)
                .css("display", "none")
                .attr("id", "deleteSelectedGroupImg_" + pName + "_" + group.Id)
                .attr("title", sg.TrashImgTitle);

            item.append(groupImg).append(deleteImg).append(Encoder.htmlEncode(group.Name));

            jq("#selectedGroups_" + pName).append(item);
            jq("#emptyUserListLabel_" + pName).hide();

            gs.ClearFilter();
            
            jq("#selectedUsers_" + pName).parent().find("div[id^=groupSelectorContainer_]").hide();

            var data = {
                id: pId,
                enabled: true
            };

            var subjects = ASC.Settings.AccessRights.getSubjects(pName);

            if (subjects.length > 0)
                data.subjects = subjects;

            Teamlab.setWebItemSecurity({}, data, {
                before: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").block();
                },
                success: function () {
                    gs.HideGroup(group.Id);
                },
                after: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").unblock();
                }
            });
        },
        
        deleteUserFromList: function (obj) {
            var idComponent = jq(obj).attr("id").split("_");
            var pName = idComponent[1];
            var uId = idComponent[2];
            var pId = jq("#accessRightsContainer_" + pName).attr("data-id");
            
            jq(obj).parent().remove();

            var sg = getSelectedGroups(pName);
            var su = getSelectedUsers(pName);

            var us = getUserSelector(pName);

            for (var i = 0; i < su.IDs.length; i++) {
                if (su.IDs[i] == uId) {
                    su.IDs.splice(i, 1);
                    su.Names.splice(i, 1);
                    break;
                }
            }

            if (su.IDs.length == 0 && sg.IDs.length == 0)
                jq("#emptyUserListLabel_" + pName).show();

            var data = {
                id: pId,
                enabled: true
            };

            var subjects = ASC.Settings.AccessRights.getSubjects(pName);

            if (subjects.length > 0) {
                data.subjects = subjects;
            } else {
                data.enabled = false;
            }

            Teamlab.setWebItemSecurity({}, data, {
                before: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").block();
                },
                success: function () {
                    us.HideUser(uId, false);
                },
                after: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").unblock();
                }
            });

        },
        
        deleteGroupFromList: function (obj) {
            var idComponent = jq(obj).attr("id").split("_");
            var pName = idComponent[1];
            var gId = idComponent[2];
            var pId = jq("#accessRightsContainer_" + pName).attr("data-id");

            jq(obj).parent().remove();

            var sg = getSelectedGroups(pName);
            var su = getSelectedUsers(pName);

            var gs = getGroupSelector(pName);

            for (var i = 0; i < sg.IDs.length; i++) {
                if (sg.IDs[i] == gId) {
                    sg.IDs.splice(i, 1);
                    sg.Names.splice(i, 1);
                    break;
                }
            }

            if (su.IDs.length == 0 && sg.IDs.length == 0)
                jq("#emptyUserListLabel_" + pName).show();

            var data = {
                id: pId,
                enabled: true
            };

            var subjects = ASC.Settings.AccessRights.getSubjects(pName);

            if (subjects.length > 0) {
                data.subjects = subjects;
            } else {
                data.enabled = false;
            }

            Teamlab.setWebItemSecurity({}, data, {
                before: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").block();
                },
                success: function () {
                    gs.HideGroup(gId, false);
                },
                after: function () {
                    jq("#selectorContent_" + pName).closest(".accessRights-content").unblock();
                }
            });
        },
        
        getSubjects: function(pName) {
            var su = getSelectedUsers(pName);
            var sg = getSelectedGroups(pName);
            return su.IDs.concat(sg.IDs);
        },
        
        setAdmin: function (obj, pId) {
            var idComponent = jq(obj).attr("id").split("_");
            var pName = idComponent[1];
            var uId = idComponent[2];
            var isChecked = jq(obj).is(":checked");
            var data = {
                productid: pId,
                userid: uId,
                administrator: isChecked
            };
            
            if (pName == "full") {
                ASC.Settings.AccessRights.setFullAdmin(obj, data);
            } else {
                ASC.Settings.AccessRights.setProductAdmin(pName, data);
            }
        },
        
        setProductAdmin: function (pName, data) {
            Teamlab.setProductAdministrator({}, data, {
                success: function () {
                    var us = getUserSelector(pName);
                    us.HideUser(data.userid, data.administrator);
                }
            });
        },

        setFullAdmin: function (obj, data) {
            Teamlab.setProductAdministrator({}, data, {
                success: function() {
                    if (data.administrator) {
                        jq("#adminItem_" + data.userid + " input[type=checkbox]").prop("checked", true).attr("disabled", true);
                        jq(obj).removeAttr("disabled");
                    } else {
                        jq("#adminItem_" + data.userid + " input[type=checkbox]").removeAttr("checked").removeAttr("disabled");
                    }
                    ASC.Settings.AccessRights.hideUserFromAll(data.userid, data.administrator);
                }
            });
        },
        
        hideUserFromAll: function (uId, hide) {
            for (var i = 0; i < pNameList.length; i++) {
                var us = getUserSelector(pNameList[i]);
                us.HideUser(uId, hide);
            }
        }
    };
    
};
