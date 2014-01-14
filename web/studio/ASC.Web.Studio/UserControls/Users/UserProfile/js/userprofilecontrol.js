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

if (!window.userProfileControl) {
  window.userProfileControl = {
    openContact : function (name) {
      var tcExist = false;
      try {
        tcExist = !!ASC.Controls.JabberClient;
      } catch (err) {
        tcExist = false;
      }
      if (tcExist === true) {
        try {ASC.Controls.JabberClient.open(name)} catch (err) {}
      }
    }
  };
}

jq(function () {
    var tcExist = false;
    try {
        tcExist = !!ASC.Controls.JabberClient;
    } catch (err) {
        tcExist = false;
    }
    if (tcExist === true) {
        jq('div.userProfileCard:first ul.info:first li.contact span')
          .addClass('link')
          .click(function () {
              var username = jq(this).parents('li.contact:first').attr('data-username');
              if (!username) {
                  var
                    search = location.search,
                    arr = null,
                    ind = 0,
                    b = null;
                  if (search.charAt(0) === '?') {
                      search = search.substring(1);
                  }
                  arr = search.split('&');
                  ind = arr.length;
                  while (ind--) {
                      b = arr[ind].split('=');
                      if (b[0].toLowerCase() !== 'user') {
                          continue;
                      }
                      username = b[1];

                      break;
                  }
              }
              if (typeof username === 'string') {
                  userProfileControl.openContact(username);
              }
          });
    }

    initActionMenu();
    initBorderPhoto();
    initTenantQuota();

    if (!jq.browser.mobile) {
        initPhotoUploader();
    }
    else {
        jq("#loadPhotoImage").hide();
        jq(".profile-role").css("bottom", "6px");
    }
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
        var userId = jq("#studio_userProfileCardInfo").attr("data-id");
        ASC.Controls.LoadPhotoImage.setDefaultPhoto(userId);
    });


    jq("#joinToAffilliate:not(.disable)").on("click", function () {
        JoinToAffilliate();
    });

    if (jq("#studio_emailChangeDialog").length == 0) {
        jq(".profile-status.pending div").css("cursor", "default");
    }

    jq.switcherAction("#switcherAccountLinks", "#accountLinks");
    jq.switcherAction("#switcherCommentButton", "#commentContainer");
    jq.switcherAction("#switcherContactsPhoneButton", "#contactsPhoneContainer");
    jq.switcherAction("#switcherContactsSocialButton", "#contactsSocialContainer");
    jq.switcherAction("#switcherSubscriptionButton", "#subscriptionContainer");

    //track event

    jq("#joinToAffilliate").trackEvent("affilliate-button", "affilliate-button-click", "");
});

function initActionMenu() {
    var _top = jq(".profile-header").offset() == null ? 0 : -jq(".profile-header").offset().top,
        _left = jq(".profile-header").offset() == null ? 0 : -jq(".profile-header").offset().left,
        menuID = "actionMenu";

    jq.dropdownToggle({
        dropdownID: menuID,
        switcherSelector: ".header-with-menu .menu-small",
        addTop: _top - 4,
        addLeft: _left - 11,
        showFunction: function(switcherObj, dropdownItem) {
            if (dropdownItem.is(":hidden")) {
                switcherObj.addClass("active");
            } else {
                switcherObj.removeClass("active");
            }
        },
        hideFunction: function() {
            jq(".header-with-menu .menu-small.active").removeClass("active");
        }
    });

    jq(jq("#" + menuID).find(".dropdown-item")).on("click", function () {
        var userId = jq("#" + menuID).attr("data-id"),
            userEmail = jq("#" + menuID).attr("data-email"),
            userAdmin = jq("#" + menuID).attr("data-admin") == "true",
            userName = jq("#" + menuID).attr("data-name"),
            parent = jq(this).parent();

            jq("#actionMenu").hide();
            jq("#userMenu").removeClass("active");

        if (jq(parent).hasClass("enable-user")) {
            onChangeUserStatus(userId, 1);
        }
        if (jq(parent).hasClass("disable-user")) {
            onChangeUserStatus(userId, 2);
        }
        if (jq(parent).hasClass("psw-change")) {            
            AuthManager.ShowPwdReminderDialog('1', userEmail);
        }
        if (jq(parent).hasClass("email-change")) {
            EmailOperationManager.ShowEmailChangeWindow(userEmail, userId, userAdmin);
        }
        if (jq(parent).hasClass("email-activate")) {          
            EmailOperationManager.ShowEmailActivationWindow(userEmail, userId, userAdmin);
        }
        if (jq(parent).hasClass("edit-photo")) {           
            UserPhotoThumbnail.ShowDialog();
        }
        if (jq(parent).hasClass("delete-user")) {            
            ProfileManager.ShowDeleteUserWindow(userId);
        }
        if (jq(parent).hasClass("delete-self")) {            
            AuthManager.RemoveUser(userId, userName);
        }
    })

    jq.dropdownToggle({
        dropdownID: "languageMenu",
        switcherSelector: ".usrLang",
        addTop:  4,
        addLeft: -10,
        showFunction: function(switcherObj, dropdownItem) {
            if (dropdownItem.is(":hidden")) {
                switcherObj.addClass('active');
            } else {
                switcherObj.removeClass("active");
            }
        },
        hideFunction: function() {
            jq(".languageMenu.active").removeClass("active");
        }
    });

    jq(".languageMenu ul.options li.option").on("click", function(event) {
        var lng = jq(this).attr('data');
        UserLangManager.SaveLanguage(lng);
    });
}

function onChangeUserStatus(userID, status) { 

    if (status == 1 && tenantQuota.availableUsersCount == 0) {
        if (jq("#tariffLimitExceedUsersPanel").length) {
            TariffLimitExceed.showLimitExceedUsers();
        }
        return;
    }

    var user = new Array();
    user.push(userID);

    var data = { userIds: user };

    Teamlab.updateUserStatus({}, status, data, {
        success: function (params, data) {
            switch (status) {
                case 1:
                    jq(".profile-status").show();
                    jq(".profile-status.blocked").hide();
                    jq(".enable-user, .delete-self").addClass("display-none");
                    jq(".edit-user, .psw-change, .email-change, .disable-user, .email-activate").removeClass("display-none");
                    break;
                case 2:
                    jq(".profile-status").hide();
                    jq(".profile-status.blocked").show();
                    jq(".enable-user, .delete-self").removeClass("display-none");
                    jq(".edit-user, .psw-change, .email-change, .disable-user, .email-activate").addClass("display-none");
                    break;
            }
            toastr.success(ASC.People.Resources.PeopleJSResource.SuccessChangeUserStatus);
            initTenantQuota();
        },
        before: LoadingBanner.displayLoading,
        after: LoadingBanner.hideLoading,
        error: function (params, errors) {
            toastr.error(errors);
        }
    });
}

var tenantQuota = {};

var initTenantQuota = function () {
    Teamlab.getQuotas({}, {
        success: function (params, data) {
            tenantQuota = data;
        },
        error: function (params, errors) { }
    });
};

function initPhotoUploader() {
    new AjaxUpload('changeLogo', {
        action: "ajaxupload.ashx?type=ASC.Web.People.UserPhotoUploader,ASC.Web.People",
        autoSubmit: true,
        onChange: function(file, extension) {
            PopupKeyUpActionProvider.CloseDialog();
            return true;
        },
        onComplete: ChangeUserPhoto,
        parentDialog: jq("#divLoadPhotoWindow"),
        isInPopup: true,
        name: "changeLogo"
    });
};

function initBorderPhoto() {
    var $block = jq("#userProfilePhoto"),
        $img = $block.children("img"),
        status = $block.siblings(".profile-status"),
        indent = $img.width() < $block.width() ? ($block.width() - $img.width()) / 2 : 0;
    
    $img.css("padding", indent + "px 0");
    for (var i = 0, n = status.length; i < n; i++) {
        jq(status[i]).css("top", ($img.outerHeight() - jq(status[i]).outerHeight()) / 2);
        jq(status[i]).css("left", ($block.width() - jq(status[i]).outerWidth()) / 2);
        if (jq(status[i]).attr("data-visible") !== "hidden") {
            jq(status[i]).show();
        }
    }
};

function ChangeUserPhoto(file, response) {
    jq('.profile-action-usericon').unblock();
    var result = eval("(" + response + ")");
    if (result.Success) {

        var userId = jq("#studio_userProfileCardInfo").attr("data-id");
        var data = { userid:userId, files: result.Data };
        Teamlab.updateUserPhoto({}, userId, data, {
            before: LoadingBanner.displayLoading,
            error: LoadingBanner.hideLoading,
            success: function(params, data) {
                jq('#userProfilePhotoError').html('');
                jq('#userProfilePhoto').find("img").attr("src", result.Data);
            }
        });       
        
    } else {
        jq('#userProfilePhotoError').html('<div class="errorBox">' + result.Message + '</div>');
    }
}

function JoinToAffilliate() {
    var $button = jq("#joinToAffilliate");
    AjaxPro.onLoading = function(b) {
        if (b) {
            $button.addClass("disable");
            LoadingBanner.displayLoading();
        } else {
            $button.removeClass("disable");
            LoadingBanner.hideLoading();
        }
    };
    AjaxPro.timeoutPeriod = 1800000;
    AjaxPro.UserProfileControl.JoinToAffiliateProgram(function(result) {
        var res = result.value;
        if (res.rs1 == "1" && res.rs2) {
            location.href = res.rs2;
        } else if (res.rs1 == "0") {
            jq("#errorAffilliate").text(res.rs2);
        }
    });
}

var UserLangManager = new function() {
    this.SaveLanguage = function(lng) {
        AjaxPro.UserLangController.SaveUserLanguageSettings(lng, function(result) {
            UserLangManager.SaveLanguageCallback(result, lng);
        });
    };
    this.SaveLanguageCallback = function(res, lng) {
        jq("#languageMenu").hide();
        var langOption = jq("#languageMenu").find("li." + lng);
        if (langOption.length == 1) {
            jq(".usrLang").attr("class", "field-value usrLang active " + lng);
            jq(".usrLang>.val").text(langOption.children("a").html());
        }
        var result = res.value;
        if (result.Status == 1) {
            LoadingBanner.displayLoading();
            window.location.reload(true);
        } else if (result.Status == 2 && jq("#studio_lngTimeSettingsInfo").length == 1) {
            jq("#studio_lngTimeSettingsInfo").html(result.Message + "<span class='selector'></span>");
            setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);
        } else if (jq("#studio_lngTimeSettingsInfo").length == 1) {
            jq("#studio_lngTimeSettingsInfo").html(result.Message + "<span class='selector'></span>");
            setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);
        }
    };
}