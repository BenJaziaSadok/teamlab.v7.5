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

ASC.Projects.Contacts = (function() {
    var projectTeam, currentProjectId;
    var init = function(prjId) {
        var dataForContactSelectorInit = {
            SelectorType: -1,
            EntityType: 0,
            EntityID: 0,

            ShowOnlySelectorContent: true,
            DescriptionText: ASC.Projects.Resources.ProjectsJSResource.CRMDescrForSelector,
            DeleteContactText: "",
            AddContactText: "",
            IsInPopup: false,
            NewCompanyTitleWatermark: ASC.Projects.Resources.ProjectsJSResource.CRMCompanyName,
            NewContactFirstNameWatermark: ASC.Projects.Resources.ProjectsJSResource.CRMFirstName,
            NewContactLastNameWatermark: ASC.Projects.Resources.ProjectsJSResource.CRMLastName,

            ShowChangeButton: false,
            ShowAddButton: false,
            ShowDeleteButton: false,
            ShowContactImg: false,
            ShowNewCompanyContent: true,
            ShowNewContactContent: true,

            ExcludedContacts: [],
            HTMLParent: "#projContactSelectorParent"
        };

        if (prjId) {
            currentProjectId = prjId;
        } else {
            currentProjectId = jq.getURLParam("prjID");
        }

        if (typeof (ASC.Projects.Master.Team) != 'undefined') {
            projectTeam = ASC.Projects.Master.Team;
        } else {
            Teamlab.getPrjTeam({}, currentProjectId, {
                success: function(param, team) {
                    ASC.Projects.Master.Team = team;
                    projectTeam = team;
                }
            });
        }

        var params = {};
        params.showCompanyLink = true;
        if (jq("#contactsForProjectPanel").length) {
            params.showUnlinkBtn = true;
        } else {
            params.showUnlinkBtn = false;
        }
        Teamlab.getCrmContactsForProject(params, currentProjectId, {
            success: function(param, data) {
                if (data.length != 0) {
                    jq.extend(dataForContactSelectorInit, { presetSelectedContactsJson: jq.toJSON(data) });
                }
                window["projContactSelector"] = new ASC.CRM.ContactSelector.ContactSelector("projContactSelector", dataForContactSelectorInit);

                if (data.length == 0) {
                    jq("#escNoContacts.display-none").removeClass("display-none");
                } else {
                    ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.contacts, data.length);
                    jq("#contactsForProjectPanel").show();
                }

                window.projContactSelector.SelectItemEvent = addContactToProject;
                ASC.CRM.ListContactView.removeMember = removeContactFromProject;

                ASC.CRM.ListContactView.CallbackMethods.render_simple_content(param, data);
            }
        });

        jq("#escNoContacts .emptyScrBttnPnl>a").bind("click", function() {
            jq("#escNoContacts").addClass("display-none");
            jq("#contactsForProjectPanel").show();
        });
    };

    var addContactToProject = function(obj, params) {
        if (jq("#contactItem_" + obj.id).length > 0) {
            return false;
        }
        var data = {
            contactid: obj.id,
            projectid: currentProjectId
        };

        if (params.newContact === true) {
            var dataRights = {
                    contactid: [obj.id],
                    isShared: false,
                    managerList: new Array(Teamlab.profile.id)
                };

            Teamlab.updateCrmContactRights({}, dataRights, {});
        }
        var param = {};
        param.showCompanyLink = true;
        if (jq("#contactsForProjectPanel").length) {
            param.showUnlinkBtn = true;
        } else {
            param.showUnlinkBtn = false;
        }
        Teamlab.addCrmEntityMember(param, "project", data.projectid, data.contactid, data, {
            success: function(par, contact) {
                ASC.CRM.ListContactView.CallbackMethods.addMember(par, contact);
                window.projContactSelector.SelectedContacts.push(contact.id);
                ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.contacts, "add");
            }
        });

    };

    var removeContactFromProject = function(id) {
        Teamlab.removeCrmEntityMember({ contactID: id }, "project", currentProjectId, id, {
            before: function(params) {
                jq("#trashImg_" + params.contactID).hide();
                jq("#loaderImg_" + params.contactID).show();
            },
            after: function(params) {

                var index = jq.inArray(params.contactID, window.projContactSelector.SelectedContacts);
                window.projContactSelector.SelectedContacts.splice(index, 1);

                jq("#contactItem_" + params.contactID).animate({ opacity: "hide" }, 500);

                setTimeout(function() {
                    jq("#contactItem_" + params.contactID).remove();
                    ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.contacts, "delete");
                    if (window.projContactSelector.SelectedContacts.length == 0) {
                        jq("#contactsForProjectPanel").hide();
                        jq("#escNoContacts.display-none").removeClass("display-none");
                    }
                }, 500);

            }
        });
    };

    return {
        init: init
    };
})(jQuery);

jq(document).ready(function() {
    var prjId = jq.getURLParam("prjID");
    if (prjId) {
        ASC.Projects.Contacts.init(prjId);
    }
});