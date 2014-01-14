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

;
window.peopleActions = (function() {
    function getProfile(id) {
        var 
            profiles = ASC.People.model.profiles || [],
            profilesInd;

        var profilesInd = profiles.length;
        while (profilesInd--) {
            if (profiles[profilesInd].id == id) {
                return profiles[profilesInd];
            }
        }
        return null;
    };
    
    var callMethodByClassname = function(classname, thisArg, argsArray) {
        if ((' ' + classname + ' ').indexOf(' disabled ') !== -1) {
            return undefined;
        }

        var cls = '',
            classesInd = 0,
            classes = typeof classname === 'string' ? classname.split(/\s+/) : [],
            classesInd = classes ? classes.length : 0;
        while (classesInd--) {
            cls = classes[classesInd].replace(/-/g, '_');
            if (typeof peopleActions[cls] === 'function') {
                peopleActions[cls].apply(thisArg, argsArray);
            }
        }
    };

    var callbackAddUser = function(result) {
        var $o = jq.tmpl("userListTemplate", { users: [result.Data], isAdmin: jq.profile.isAdmin });

        if (jq("#peopleData tr.profile").length == 0) {
            jq("#emptyContentForPeopleFilter").addClass("display-none");
            jq("#peopleContent").removeClass("display-none");
        }
        jq("#peopleData").prepend($o);
        ASC.Controls.AnchorController.trigger();
    };

    return {
        callMethodByClassname: callMethodByClassname,

        add_group: function(evt, $btn) {
            StudioManagement.AddDepartmentOpenDialog();
        },

        update_group: function(evt, $btn) {
            ASC.People.PeopleController.getEditGroup();
        },

        delete_group: function(evt, $btn) {
            jq("#confirmationDeleteDepartmentPanel .confirmationAction").html(jq.format(StudioManagement._confirmDeleteGroup, jq(".profile-title:first>.header").html()));
            jq("#confirmationDeleteDepartmentPanel .middle-button-container>.button.blue.middle").unbind("click").bind("click", function() {
                ASC.People.PeopleController.deleteGroup();
            });
            StudioBlockUIManager.blockUI("#confirmationDeleteDepartmentPanel", 500, 500, 0);
        },

        add_profiles: function(evt, $btn) {
            ImportUsersManager.ShowImportControl('ASC.Controls.AnchorController.trigger()');
        },

        send_invites: function(evt, $btn) {
            InvitesResender.Show();
        },

        send_email: function(evt, $btn) {
            var userId = $btn.parents('tr.item.profile:first').attr('data-id');
            if (userId) {
                var email = $btn.parents('tr.item.profile:first').attr('data-email');
                if (email) {
                    window.open('../../addons/mail/#composeto/email=' + email, "_blank");
                }
                //var profile = getProfile(userId);
                //if (profile) {
                //  location.href = 'mailto:' + profile.email;
                //}
            }
        },

        open_dialog: function(evt, $btn) {
            var userId = $btn.parents('tr.item.profile:first').attr('data-id');
            if (userId) {
                var userName = $btn.parents('tr.item.profile:first').attr('data-username');
                if (userName) {
                    try {
                        ASC.Controls.JabberClient.open(userName);
                    } catch (err) { }
                }
                //var profile = getProfile(userId);
                //if (profile) {
                //  try { ASC.Controls.JabberClient.open(profile.userName) } catch (err) {console.log(err)}
                //}
            }
        }
    };
})();

jq(function () {
    

    jq("#defaultLinkPeople").on("click", function () {
        var pathname = "/products/people/";
        if (location.pathname == pathname) {
            var oldAnchor = jq.anchorToObject(ASC.Controls.AnchorController.getAnchor());
            ASC.People.PeopleController.moveToPage("1");
            if (oldAnchor.hasOwnProperty("group")) {
                delete oldAnchor.group;
                ASC.Controls.AnchorController.move(jq.objectToAnchor(oldAnchor));
            }
        }
        else {
            location.href = pathname;
        }
    });

    jq("#groupList .menu-item-label").on("click", function () {
        var pathname = "/products/people/",
            id = jq(this).parents(".menu-sub-item").attr("data-id"),
            oldAnchor = jq.anchorToObject(ASC.Controls.AnchorController.getAnchor()),
            newAnchor = jq.mergeAnchors(oldAnchor, jq.anchorToObject("group=" + id));
        if (location.pathname == pathname) {
            if (!jq.isEqualAnchors(newAnchor, oldAnchor)) {
                ASC.Controls.AnchorController.move(jq.objectToAnchor(newAnchor));
            }
        }
        else {
            location.href = pathname + "#group=" + id;
        }
    })
})