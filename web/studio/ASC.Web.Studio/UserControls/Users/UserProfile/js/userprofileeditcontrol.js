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

/*
Copyright (c) Ascensio System SIA 2013. All rights reserved.
http://www.teamlab.com
*/
window.EditProfileManager = (function () {

    var init = function () {

        if (!jq.browser.mobile) {
            initPhotoUploader();
        }
        else {
            jq("#loadPhotoImage").hide();
            jq(".profile-role").css("bottom", "6px");
        }

        initBorderPhoto();

        jq("#userProfilePhoto img").on("load", function () {
            initBorderPhoto();
            LoadingBanner.hideLoading();
        });

        jq("#loadPhotoImage").on("click", function () {
            if (!jq.browser.mobile) {
                ASC.Controls.LoadPhotoImage.showPhotoDialog();
            }
        });

        jq("#divLoadPhotoWindow .default-image").on("click", function () {
            if (typeof (window.userId) != "undefined") {
                var userId = window.userId;
                ASC.Controls.LoadPhotoImage.setDefaultPhoto(userId);
            }
        });
        jq("#userType").change(function () {
            changeDescriptionUserType(this);
        });

        jq.switcherAction("#switcherCommentButton", "#commentContainer");
        jq.switcherAction("#switcherContactInfoButton", "#contactInfoContainer");
        jq.switcherAction("#switcherSocialButton", "#socialContainer");

        jq(".containerBodyBlock").on("click", ".delete-field", function () {
            jq(this).parent(".field-with-actions").remove();
        });

        jq(".add-new-field").on("click", function () { addNewField(this) });

        jq(".tabs-content").on("change", "select", function () {
            var className = jq(jq(this).find("option:selected")).val();
            jq(this).siblings(".combobox-title").removeClass().addClass("combobox-title " + className);
        });

        renderDepsContacts();
        var sex = jq("#userdataSex").attr("data-value");
        jq('#userdataSex option[value=' + sex + ']').attr('selected', true);

        jq("#userType").tlcombobox();
        jq("#userdataSex").tlcombobox();
        jq(".group-field").tlcombobox();
        jq("#profileFirstName").focus();

        jq(".tabs-content select").each(function () {
            var className = jq(this).find("option:selected").val();
            jq(this).siblings(".combobox-title").addClass(className);
        });

        InitDatePicker();

        jq("#cancelProfileAction").on("click", function () {
            document.location.href = document.referrer;
        })

        jq("#profileActionButton").on("click", function () {

            HideRequiredError();
            jq("#serverError").hide();

            var isVisitor = false;

            if (jq("#userType").length) {
                isVisitor = (jq("#userType").val() == "user") ? false : true;
            } else {
                isVisitor = (jq("#userTypeField").attr("data-type") == "user") ? false : true;
            }
            var firstName = jq("#profileFirstName").val(),
                lastName = jq("#profileSecondName").val(),
                position = jq("#profilePosition").val(),
                location = jq("#profilePlace").val(),
                email = jq("#profileEmail").val(),
                workFromDate = jq("#profileRegistrationDate").val(),
                birthDate = jq("#profileBirthDate").val(),
                pathname = jq("#userProfilePhoto").find("img").attr("src"),
                sex,
                departments = [],
                contacts = [];

            var comment = jq("#profileComment").val();
            if (comment == null || comment == "null") {
                comment = "";
            }

            var isError = false;
            if (firstName == "") {
                ShowRequiredError(jq("#profileFirstName"));
                isError = true;
            }
            if (lastName == "") {
                ShowRequiredError(jq("#profileSecondName"));
                isError = true;
            }
            if (!jq.isValidEmail(email)) {
                ShowRequiredError(jq("#profileEmail"));
                isError = true;
            }
            if (firstName.length > 64) {
                jq("#profileFirstName").siblings(".requiredErrorText").text(ASC.Resources.Master.Resource.ErrorMessageLongField64);
                ShowRequiredError(jq("#profileFirstName"));
                isError = true;
            }
            if (lastName.length > 64) {
                jq("#profileSecondName").siblings(".requiredErrorText").text(ASC.Resources.Master.Resource.ErrorMessageLongField64);
                ShowRequiredError(jq("#profileSecondName"));
                isError = true;
            }
            if (position.length > 64) {
                ShowRequiredError(jq("#profilePosition"));
                isError = true;
            }
            if (location.length > 255) {
                ShowRequiredError(jq("#profilePlace"));
                isError = true;
            }
            if (workFromDate != "" && !jq.isDateFormat(workFromDate)) {
                ShowRequiredError(jq("#profileRegistrationDate"));
                isError = true;
            }
            if (birthDate != "" && !jq.isDateFormat(birthDate)) {
                ShowRequiredError(jq("#profileBirthDate"));
                isError = true;
            }
            if (birthDate != "" && workFromDate != "" && jq("#profileRegistrationDate").datepicker('getDate').getTime() < jq("#profileBirthDate").datepicker('getDate').getTime()) {
                jq("#profileRegistrationDate").siblings(".requiredErrorText").text(ASC.Resources.Master.Resource.ErrorMessage_InvalidDate);
                ShowRequiredError(jq("#profileRegistrationDate"));
                isError = true;
            }

            if (isError) {
                return;
            }

            var
            type = "",
            value = "",
            $contact = null,
            $contacts = jq(".contacts-group div.field-with-actions:not(.default)");

            for (var i = 0, n = $contacts.length; i < n; i++) {
                $contact = $contacts.slice(i, i + 1);
                type = $contact.find("select").val();
                value = $contact.find("input.textEdit").val();
                if (type && value) {
                    for (var j = 0, k = contacts.length; j < k; j++) {
                        if (type == contacts[j].Type && value == contacts[j].Value) {
                            jq("#serverError").text(ASC.Resources.Master.Resource.ErrorMessageContactsDuplicated).show();
                            return;
                        }
                    }
                    contacts.push({ Type: type, Value: value });
                }
            }

            switch (jq("#userdataSex").val()) {
                case "1": sex = "male"; break;
                case "0": sex = "female"; break;
                case "-1": sex = ""; break;
            }

            if (jq("#profileBirthDate").val().length) {
                var birthDate = jq("#profileBirthDate").datepicker('getDate');
                birthDate.setHours(0);
                birthDate.setMinutes(0);
                birthDate = Teamlab.serializeTimestamp(birthDate);
            }
            if (jq("#profileRegistrationDate").val().length) {
                var workFromDate = jq("#profileRegistrationDate").datepicker('getDate');
                workFromDate.setHours(0);
                workFromDate.setMinutes(0);
                workFromDate = Teamlab.serializeTimestamp(workFromDate);
            }

            var
            $depSelector = null,
            $depSelectors = jq("#departmentsField div.field-with-actions:not(.default) select");

            for (var i = 0, n = $depSelectors.length; i < n; i++) {
                $depSelector = jq($depSelectors[i]);
                departments.push($depSelector.find("option:selected").val());
            }



            var profile =
            {
                isVisitor: isVisitor,
                firstname: firstName,
                lastname: lastName,
                comment: comment,
                sex: sex,
                title: position,
                location: location,
                email: email,
                birthday: birthDate,
                worksfrom: workFromDate,
                contacts: contacts,
                files: pathname,
                department: departments
            };

            var edit = jq.getURLParam("action") == "edit" ? true : false;

            lockProfileActionPageElements();
            if (edit && typeof (window.userId) != "undefined") {
                updateProfile(window.userId, profile);
            } else {
                addProfile(profile);
            }

        });
    };

    function addProfile(profile) {
        var params = {};
        Teamlab.addProfile(params, profile, { success: onAddProfile, error: onProfileError });
    };

    function onAddProfile(params, profile) {
        profile = this.__responses[0];
        document.location.replace('profile.aspx?user=' + encodeURIComponent(profile.userName));
    };


    function updateProfile(profileId, profile) {
        var params = {};
        Teamlab.updateProfile(params, profileId, profile, { success: onUpdateProfile, error: onProfileError });
    }

    var onUpdateProfile = function (params, profile) {
        if (document.location.href.indexOf("my.aspx") > 0) {
            document.location.replace("/my.aspx");
        } else {
            document.location.replace("profile.aspx?user=" + encodeURIComponent(profile.userName));
        }
    };


    var IsInitDatePicker = false;

    var InitDatePicker = function () {

        var fromDateInp = jq("#profileRegistrationDate"),
            birthDateInp = jq("#profileBirthDate");
        if (!IsInitDatePicker) {
            jq(fromDateInp).mask(ASC.Resources.Master.DatePatternJQ);
            jq(birthDateInp).mask(ASC.Resources.Master.DatePatternJQ);

            jq(fromDateInp)
                .datepicker({
                    onSelect: function () {
                        var date = jq(this).datepicker("getDate");
                        jq(birthDateInp).datepicker("option", "maxDate", date || null);
                    }
                }).val(jq(fromDateInp).attr("data-value"));

            jq(birthDateInp)
                .datepicker({
                    onSelect: function () {
                        var date = jq(this).datepicker("getDate");
                        jq(fromDateInp).datepicker("option", "minDate", date || null);
                    }
                }).val(jq(birthDateInp).attr("data-value"));
            IsInitDatePicker = true;
        };
        jq(fromDateInp).datepicker("option", "minDate", jq(birthDateInp).datepicker("getDate") || null);
        jq(birthDateInp).datepicker("option", "maxDate", jq(fromDateInp).datepicker("getDate") || null);
    };

    var isValidBithday = function () {
        var fromDateInp = jq("#profileRegistrationDate");
        var birthDateInp = jq("#profileBirthDate");
        return (fromDateInp.datepicker("getDate").getTime() > birthDateInp.datepicker("getDate").getTime()) ? true : false;
    };

    var ChangeUserPhoto = function (file, response) {
        jq('.profile-action-usericon').unblock();
        var result = eval("(" + response + ")");
        if (result.Success) {
            jq('#userProfilePhotoError').html('');
            jq('#userProfilePhoto').find("img").attr("src", result.Data);
        } else {
            jq('#userProfilePhotoError').html('<div class="errorBox">' + result.Message + '</div>');
        }
    };

    var initPhotoUploader = function () {
        new AjaxUpload('changeLogo', {
            action: "ajaxupload.ashx?type=ASC.Web.People.UserPhotoUploader,ASC.Web.People",
            autoSubmit: true,
            onChange: function (file, extension) {
                PopupKeyUpActionProvider.CloseDialog();
                return true;
            },
            onComplete: EditProfileManager.ChangeUserPhoto,
            parentDialog: jq("#divLoadPhotoWindow"),
            isInPopup: true,
            name: "changeLogo"
        });
    };
    var initBorderPhoto = function () {
        var $block = jq("#userProfilePhoto"),
            $img = $block.children("img"),
            indent;

        indent = $img.width() < $block.width() ? ($block.width() - $img.width()) / 2 : 0;
        $img.css("padding", indent + "px 0");
    };
    var renderDepsContacts = function () {
        if (typeof (window.departmentsList) != "undefined" && window.departmentsList.length != 0) {
            for (var i = 0, n = window.departmentsList.length; i < n; i++) {
                var dep = window.departmentsList[i];
                if (dep.hasOwnProperty("id")) {
                    addNewBlock(dep.id, null, "#departmentsField");
                }
            }
        }
        if (typeof (window.socContacts) != "undefined" && window.socContacts.length != 0) {
            for (var i = 0, n = window.socContacts.length; i < n; i++) {
                var soc = window.socContacts[i];
                if (soc.hasOwnProperty("classname")) {
                    addNewBlock(soc.classname, soc.text, "#socialContainer");
                }
            }
        }
        if (typeof (window.otherContacts) != "undefined" && window.otherContacts.length != 0) {
            for (var i = 0, n = window.otherContacts.length; i < n; i++) {
                var contact = window.otherContacts[i];
                if (contact.hasOwnProperty("classname")) {
                    addNewBlock(contact.classname, contact.text, "#contactInfoContainer");
                }
            }
        }
    };

    var addNewField = function (item) {
        var clone = jq(item).siblings(".field-with-actions.default").clone().removeClass("default");
        jq(clone).children("input").val("");
        jq(item).before(jq(clone));
        var selects = jq(item).siblings().find("select");
        jq(selects).tlcombobox();
        if (jq(item).parent().attr("id") === "departmentsField") {
            /*Hide the name of groups which user choose yet*/
            jq(".group-field").on("mousedown", function () {
                var chooseElems = jq(item).siblings().find("span.group-field").not(this).find("li.selected-item"),
                    options = jq(this).find("li.option-item");
                jq(options).each(function () {
                    jq(this).show();
                    for (var i = 0; i < chooseElems.length; i++) {
                        if (jq(this).attr("data-value") === jq(chooseElems[i]).attr("data-value") && jq(chooseElems[i]).attr("data-value") !== "00000000-0000-0000-0000-000000000000") {
                            jq(this).hide();
                        }
                    }
                });
            });
        }
    };

    var addNewBlock = function (selectedValue, value, fieldID) {
        var $linkAdd = jq(fieldID).children(".add-new-field"),
            $newSelect = $linkAdd.siblings(".field-with-actions.default").clone().removeClass("default");
        if (selectedValue != "") {
            $newSelect.find("select").val(selectedValue);
            if (value != null) {
                $newSelect.find("select").siblings("input").val(value);
            }
        } else {
            $newSelect.find("select").val($newSelect.find("select").children("option:first").val());
        }
        $linkAdd.before($newSelect);
    };

    function onProfileError() {
        unlockProfileActionPageElements();
        jq("#serverError").text(this.__errors[0]).show();
    };
    function lockProfileActionPageElements() {
        jq(".profile-action-userdata .userdata-value input, #profileComment, .contacts-group input").attr("disabled", "disabled");
        jq("#userType, #userdataSex , .group-field").tlcombobox(false);
        jq(".add-new-field, #loadPhotoImage").off();
        jq('#profileActionsContainer').hide();
        jq('#profileActionsInfoContainer').show();
    }

    function unlockProfileActionPageElements() {
        jq(".profile-action-userdata .userdata-value input, #profileComment, .contacts-group input").removeAttr("disabled");
        jq("#userType, #userdataSex , .group-field").tlcombobox(true);

        jq(".add-new-field").on("click", function () { addNewField(this) });

        jq("#loadPhotoImage").on("click", function () {
            ASC.Controls.LoadPhotoImage.showPhotoDialog();
        });
        jq('#profileActionsContainer').show();
        jq('#profileActionsInfoContainer').hide();
    }

    function changeDescriptionUserType(typeSelector) {
        var isEdit = jq.getURLParam("action") == "edit" ? true : false;
        if (jq(typeSelector).val() == "user") {
            jq("#collaboratorCanBlock").hide();
            jq("#userCanBlock").show();
        } else {
            jq("#userCanBlock").hide();
            jq("#collaboratorCanBlock").show();
        }
    };

    return {
        init: init,
        ChangeUserPhoto: ChangeUserPhoto
    };
})();

jq(function () {
    EditProfileManager.init();
})
