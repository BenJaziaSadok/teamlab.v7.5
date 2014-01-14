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

if (typeof jq == "undefined")
    var jq = jQuery.noConflict();

jQuery.extend(
    jQuery.expr[":"],
    {
        reallyvisible: function (a) {
            return !(jQuery(a).is(':hidden') || jQuery(a).parents(':hidden').length);
        }
    }
);

jQuery.fn.yellowFade = function () {
    return (this.css({backgroundColor: "#ffffcc"}).animate(
        {backgroundColor: "#ffffff"},
        1500,
        function () {
            jq(this).css({backgroundColor: ""});
        }));
    };

// google analitics track
jQuery.fn.trackEvent = function (category, action, label, typeAction) { // only for static objects (don't use for elements added dynamically)
    var trackingFun = function (ctg, act, lbl) {
        try {
            if (window._gat) {
                window._gaq.push(['_trackEvent', ctg, act, lbl]);
            }
        } catch (err) {
        }
    };
    switch (typeAction) {
        case "click":
            jq(this).on("click", function () {
                trackingFun(category, action, label);
                return true;
            });
            break;
        case "enter":
            jq(this).keypress(function (e) {
                if (e.which == 13) {
                    trackingFun(category, action, label);
                }
                return true;
            });
            break;
        default:
            jq(this).on("click", function () {
                trackingFun(category, action, label);
                return true;
            });
            break;
    }
};

jQuery.extend({
    loadTemplates: function(templateUrl) {
        jQuery.get(templateUrl, function(templates) {
            jQuery(templates).filter("script")
                .each(function() {
                    var templateName = jQuery(this).attr("id");
                    jQuery(this).template(templateName);
                });
        });
    },

    /*
    var result = $.format("Hello, {0}.", "world");
    //result -> "Hello, world."
    */
    format: function jQuery_dotnet_string_format(text) {
        //check if there are two arguments in the arguments list
        if (arguments.length <= 1) {
            //if there are not 2 or more arguments there's nothing to replace
            //just return the text
            return text;
        }
        //decrement to move to the second argument in the array
        var tokenCount = arguments.length - 2;
        for (var token = 0; token <= tokenCount; ++token) {
            //iterate through the tokens and replace their placeholders from the text in order
            text = text.replace(new RegExp("\\{" + token + "\\}", "gi"), arguments[token + 1]);
        }
        return text;
    },

    htmlEncodeLight: function(string) {
        var newStr = string.replace(/</ig, '&lt;').replace(/>/ig, '&gt;').replace(/\n/ig, '<br/>').replace('&amp;', '&');
        return newStr;
    },

    linksParser: function(val) {
        var replaceUrl = function(str) {
            return '<a target="_new" href="' + (/^http/.test(str) ? str : 'http://' + str) + '">' + str + '</a>';
        };
        var regUrl = /(\b(((https?|ftp|file):\/\/)|(www.))[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
        return val.replace(regUrl, replaceUrl);
    },

    registerHeaderToggleClick: function(parentSelector, toggleLineSelector) {
        if (typeof (parentSelector) != "string")
            return;
        jq(parentSelector).find(".headerToggle, .openBlockLink, .closeBlockLink").bind("click", function() {
            var $lineElt = jq(this).parents(toggleLineSelector + ":first");
            $lineElt.nextUntil(toggleLineSelector).toggle();
            $lineElt.toggleClass("open");
        });
    },

    getURLParam: function(strParamName) {
        strParamName = strParamName.toLowerCase();

        var strReturn = "";
        var strHref = window.location.href.toLowerCase();
        var bFound = false;

        var cmpstring = strParamName + "=";
        var cmplen = cmpstring.length;

        if (strHref.indexOf("?") > -1) {
            var strQueryString = strHref.substr(strHref.indexOf("?") + 1);
            var aQueryString = strQueryString.split("&");
            for (var iParam = 0; iParam < aQueryString.length; iParam++) {
                if (aQueryString[iParam].substr(0, cmplen) == cmpstring) {
                    var aParam = aQueryString[iParam].split("=");
                    strReturn = aParam[1];
                    bFound = true;
                    break;
                }
            }
        }
        if (bFound == false) {
            return null;
        }

        if (strReturn.indexOf("#") > -1) {
            return strReturn.split("#")[0];
        }

        return strReturn;
    },

    isDateFormat: function(txtDate) {
        var aoDate, // needed for creating array and object
            ms, // date in milliseconds
            month, day, year; // (integer) month, day and year

        var dateFormat = Teamlab.constants.dateFormats.date;
        var separator = "/";
        var dateFormatComponent = dateFormat.split(separator);
        if (dateFormatComponent.length == 1) {
            separator = ".";
            dateFormatComponent = dateFormat.split(separator);
            if (dateFormatComponent.length == 1) {
                separator = "-";
                dateFormatComponent = dateFormat.split(separator);
                if (dateFormatComponent.length == 1) {
                    separator = ". ";       // for czech language
                    dateFormatComponent = dateFormat.split(separator);
                    if (dateFormatComponent.length == 1) {
                        return "Unknown format date";
                    }
                }
            }
        }

        // split input date to month, day and year
        aoDate = txtDate.split(separator);
        // array length should be exactly 3 (no more no less)
        // the second condition for format dd.mm.yyyy. (for example in Latvia)
        if (aoDate.length !== 3 && txtDate.charAt(txtDate.length - 1) !== separator) {
            return false;
        }
        // define month, day and year from array (expected format is m/d/yyyy)
        // subtraction will cast variables to integer implicitly
        for (var i = 0; i < dateFormatComponent.length; i++) {
            if (dateFormatComponent[i][0]) {
                if (dateFormatComponent[i].trim()[0].toLowerCase() == "d") {
                    day = aoDate[i] - 0;
                }
                if (dateFormatComponent[i].trim()[0].toLowerCase() == "m") {
                    month = aoDate[i] - 1; // because months in JS start from 0
                }
                if (dateFormatComponent[i].trim()[0].toLowerCase() == "y") {
                    year = aoDate[i] - 0;
                }
            }
        }

        // test year range
        if (year < 1000 || year > 3000) {
            return false;
        }
        // convert input date to milliseconds
        ms = (new Date(year, month, day)).getTime();
        // initialize Date() object from milliseconds (reuse aoDate variable)
        aoDate = new Date();
        aoDate.setTime(ms);
        // compare input date and parts from Date() object
        // if difference exists then input date is not valid
        if (aoDate.getFullYear() !== year ||
            aoDate.getMonth() !== month ||
            aoDate.getDate() !== day) {
            return false;
        }
        // date is OK, return true
        return true;
    },

    isValidEmail: function(email) {
        var reg = new RegExp(ASC.Resources.Master.EmailRegExpr, "i");
        if (reg.test(email) == true) {
            return true;
        }
        return false;
    },

    switcherAction: function(el, block) {
        var elem = jq(el);
        elem.on("click", function() {
            var bl = jq(block);
            var sw = elem.attr("data-switcher");

            if (sw == "0") {
                elem.attr("data-switcher", "1");
                elem.text(elem.attr("data-showtext"));
                bl.hide();
            } else {
                elem.attr("data-switcher", "0");
                elem.text(elem.attr("data-hidetext"));
                bl.show();
            }
        });
    }
});

String.prototype.format = function() {
    var txt = this,
        i = arguments.length;

    while (i--) {
        txt = txt.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return txt;
};

StudioBlockUIManager = {
    blockUI: function (obj, width, height, top, position) {
        try {
            width = width | 0;
            height = height | 0;
            left = width > 0 ? (-width / 2 | 0) : -200;
            top = (top || -height / 2) | 0 + (position ? jq(window).scrollTop() : 0);
            position = position ? position : "fixed";

            jq.blockUI({
                message: jq(obj),
                css: {
                    left: '50%',
                    top: '50%',
                    opacity: '1',
                    border: 'none',
                    padding: '0px',
                    width: width > 0 ? width + 'px' : 'auto',
                    height: height > 0 ? height + 'px' : 'auto',
                    cursor: 'default',
                    textAlign: 'left',
                    position: position,
                    'margin-left': left + 'px',
                    'margin-top': top + 'px',
                    'background-color': 'Transparent'
                },

                overlayCSS: {
                    backgroundColor: '#aaaaaa',
                    cursor: 'default',
                    opacity: '0.3'
                },

                focusInput: true,
                baseZ: 666,

                fadeIn: 0,
                fadeOut: 0
            });
        } catch (e) {
        }
    }
};

/**
 * init FCKeditor
 */
function GetMaxImageWidth() {
    return ASC.Resources.Master.MaxImageFCKWidth;
}

var fckPrevValue = new Array();

var fckEditorOncompleteArr = new Array();

function addFckEditor_OnComplete(func, arg) {
    fckEditorOncompleteArr.push({ f: func, a: arg });
}

function clearFckEditorOncompleteArr() {
    fckEditorOncompleteArr = new Array();
}

function FCKeditor_OnComplete(editorInstance) {
    try {
        var fck_editing_area = editorInstance.EditingArea.TargetElement;
        fck_editing_area.style.height = "100.1%";
        setTimeout(function () {
            fck_editing_area.style.height = "100%";
        }, 100);
        if (typeof (Page_FCKeditor_OnComplete) == "function") {
            Page_FCKeditor_OnComplete(editorInstance);
        }
        for (var i = 0; i < fckEditorOncompleteArr.length; i++) {
            if (typeof (fckEditorOncompleteArr[i].f) == "function") {
                fckEditorOncompleteArr[i].f(editorInstance, fckEditorOncompleteArr[i].a);
            }
        }
    } catch (e) {
    }
}



jq(document).ready(function () {
    window.FCKConfig_OnLoad_Main = function(config) {
        config.DefaultLanguage = ASC.Resources.Master.TwoLetterISOLanguageName;
    };
    //cancel fckeditor changes button prepare
    jq('.cancelFckEditorChangesButtonMarker').each(function () {
        var fckEditorId = jq(this).attr('name');
        if (fckEditorId == null || fckEditorId == '') {
            return;
        }

        var cancelButtonClick = jq(this).attr('onclick');
        jq(this).attr('onclick', '');
        jq(this).click(
            function (e) {
                if (!CancelButtonController.ConfirmCancellationWithPrevValue(fckEditorId, fckPrevValue[fckEditorId])) {
                    return false;
                }
                if (cancelButtonClick) {
                    eval(cancelButtonClick);
                }
            }
        );

        addFckEditor_OnComplete(function (editorInstance, obj) {
            if (editorInstance.Name == jq(obj).attr('name')) {
                fckPrevValue[jq(obj).attr('name')] = editorInstance.GetXHTML();
            }
        }, this);

        window.onbeforeunload =
            function (e) {
                if (fckEditorId) {
                    var curLink = jq('[name="' + fckEditorId + '"]');
                    if (!curLink.is(':reallyvisible')) {
                        return;
                    }

                    var prev = '';
                    if (fckPrevValue[fckEditorId]) {
                        prev = fckPrevValue[fckEditorId];
                    }

                    if (!CancelButtonController.IsEmptyFckEditorTextFieldWithPrevValue(fckEditorId, prev)) {
                        return ASC.Resources.Master.Resource.CancelConfirmMessage;
                    } else {
                        return;
                    }
                }
            };
    });

    keepSessionAliveForFckEditor();
});

var keepSessionAliveForFckEditor = function () {
    setTimeout(function () {
        var hasFckEditor = false;
        try {
            for (var fckInstance in FCKeditorAPI.Instances) {
                hasFckEditor = true;
                if (!CancelButtonController.IsEmptyFckEditorTextField(fckInstance)) {
                    jq.post("KeepSessionAlive.ashx", null, function () {
                    });
                }
            }
        } catch (err) {
        }

        if (hasFckEditor) {
            keepSessionAliveForFckEditor();
        }
    }, 3 * 60 * 1000);
};

//------------fck uploads for comments-------------------
var FCKCommentsController = new function () {
    this.Callback = null;
    this.EditCommentHandler = function (commentID, text, domain, isEdit, callback) {
        this.Callback = callback;
        if (text == null || text == undefined)
            text = "";

        CommonControlsConfigurer.EditCommentComplete(commentID, domain, text, isEdit, this.CallbackHandler);
    };

    this.CancelCommentHandler = function (commentID, domain, isEdit, callback) {
        this.Callback = callback;
        CommonControlsConfigurer.CancelCommentComplete(commentID, domain, isEdit, this.CallbackHandler);
    };

    this.RemoveCommentHandler = function (commentID, domain, callback) {
        this.Callback = callback;
        CommonControlsConfigurer.RemoveCommentComplete(commentID, domain, this.CallbackHandler);
    };

    this.CallbackHandler = function (result) {
        if (FCKCommentsController.Callback != null && FCKCommentsController.Callback != '')
            ;
        eval(FCKCommentsController.Callback + '()');
    };
};

/**
 * FCK Cancel controller
 */
var CancelButtonController = new function () {
    this.ConfirmCancellationWithPrevValue = function (fckEditorID, prevValue) {
        try {
            if (!this.IsEmptyFckEditorTextFieldWithPrevValue(fckEditorID, prevValue)) {
                return window.confirm(ASC.Resources.Master.Resource.CancelConfirmMessage);
            }
        } catch (err) {
        }
        return true;
    };

    this.IsEmptyFckEditorTextFieldWithPrevValue = function (fckEditorID, prevValue) {
        try {
            var curValue = FCKeditorAPI.GetInstance(fckEditorID).GetXHTML();
            return this.IsEmptyTextField(curValue) || (prevValue && curValue == prevValue);
        } catch (err) {
        }
        return true;
    };

    this.IsEmptyTextField = function (text) {
        if (text.replace(/<\/?\s*(br|p|div)[\s\S]*?>|&nbsp;?|\s+/gi, '') == '') {
            return true;
        }
        return false;
    };

    this.IsEmptyFckEditorTextField = function (fckEditorID) {
        try {
            return this.IsEmptyTextField(FCKeditorAPI.GetInstance(fckEditorID).GetXHTML());
        } catch (err) {
        }
        return true;
    };
};

/**
 * PopupKeyUpActionProvider
 */
var PopupKeyUpActionProvider = new function () {
    //close dialog by esc
    jq(document).keyup(function (event) {

        if (!jq('.popupContainerClass').is(':visible'))
            return;

        var code;
        if (!e)
            var e = event;
        if (e.keyCode)
            code = e.keyCode;
        else if (e.which)
            code = e.which;

        if (code == 27 && PopupKeyUpActionProvider.EnableEsc) {
            PopupKeyUpActionProvider.CloseDialog();
        } else if ((code == 13) && e.ctrlKey) {
            if (PopupKeyUpActionProvider.CtrlEnterAction != null && PopupKeyUpActionProvider.CtrlEnterAction != '') {
                eval(PopupKeyUpActionProvider.CtrlEnterAction);
            }
        } else if (code == 13) {
            if (e.target.nodeName.toLowerCase() !== 'textarea' && PopupKeyUpActionProvider.EnterAction != null && PopupKeyUpActionProvider.EnterAction != '')
                eval(PopupKeyUpActionProvider.EnterAction);
            if (e.target.nodeName.toLowerCase() !== 'textarea' && PopupKeyUpActionProvider.EnterActionCallback != null && PopupKeyUpActionProvider.EnterActionCallback != '')
                eval(PopupKeyUpActionProvider.EnterActionCallback);
        }

    });

    this.CloseDialog = function () {
        jq.unblockUI();

        if (PopupKeyUpActionProvider.CloseDialogAction != null && PopupKeyUpActionProvider.CloseDialogAction != '') {
            eval(PopupKeyUpActionProvider.CloseDialogAction);
        }

        PopupKeyUpActionProvider.ClearActions();
    };

    this.CloseDialogAction = '';
    this.EnterAction = '';
    this.EnterActionCallback = '';
    this.CtrlEnterAction = '';
    this.EnableEsc = true;

    this.ClearActions = function () {
        this.CloseDialogAction = '';
        this.EnterAction = '';
        this.EnterActionCallback = '';
        this.CtrlEnterAction = '';
        this.EnableEsc = true;
    };
};

/**
 * AuthManager
 */
var AuthManager = new function () {
    this.ConfirmMessage = 'Are you sure?';

    jq(document).ready(function () {
        var currentURL = new String(document.location);
        if (currentURL.indexOf('auth.aspx') != -1) {
            jq('#pwd').keydown(function (event) {
                var code;
                if (!e)
                    var e = event;
                if (e.keyCode)
                    code = e.keyCode;
                else if (e.which)
                    code = e.which;

                if (code == 13)
                    AuthManager.Login();
                if (code == 27)
                    jq('#pwd').val('');

            });

        }
        ;
    });

    this.ShowPwdReminderDialog = function (pswdRecoveryType, userEmail) {

        //pswdRecoveryType can be
        //1 if the password need for change
        //0 if the password need for recovery
        //if pswdRecoveryType == 1 => userEmail must be setted

        if (pswdRecoveryType == 1) {
            jq("#pswdRecoveryDialogPopupHeader").hide();
            jq("#pswdRecoveryDialogText").hide();
            jq("#pswdChangeDialogPopupHeader").show();
            jq("#pswdChangeDialogText").show();

            jq('#studio_emailPwdReminder').val(userEmail);
            jq("#studio_pwdReminderDialog [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        } else {
            jq("#pswdRecoveryDialogPopupHeader").show();
            jq("#pswdRecoveryDialogText").show();
            jq("#pswdChangeDialogPopupHeader").hide();
            jq("#pswdChangeDialogText").hide();
        }

        //jq('#studio_pwdReminderInfo').html('');
        jq('#' + jq('#studio_pwdReminderInfoID').val()).html('<div></div>');
        jq('#' + jq('#studio_pwdReminderInfoID').val()).hide();

        StudioBlockUIManager.blockUI("#studio_pwdReminderDialog", 350, 400, 0);

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = 'AuthManager.RemindPwd();';

        jq('#studio_pwdReminderContent').show();
        jq('#studio_pwdReminderMessage').hide();

        var loginEmail = jq('#login').val();
        if (loginEmail != null && loginEmail != undefined && loginEmail.indexOf('@') >= 0) {
            jq('#studio_emailPwdReminder').val(loginEmail);
        }
    };

    this.RemindPwd = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#pwd_rem_panel_buttons').hide();
                jq('#pwd_rem_action_loader').show();
            } else {
                jq('#pwd_rem_action_loader').hide();
                jq('#pwd_rem_panel_buttons').show();
            }
        };

        MySettings.RemindPwd(jq('#studio_emailPwdReminder').val().trim(), function (result) {
            var res = result.value;
            var infoPanel = jq('#' + jq('#studio_pwdReminderInfoID').val());
            if (res.rs1 == "1") {
                jq('#studio_emailPwdReminder').val('');
                jq('#studio_pwdReminderContent').hide();

                jq(infoPanel).removeClass('alert')
                    .addClass('info')
                    .html("<div>" + res.rs2 + "</div>")
                    .show();

                setTimeout("jq.unblockUI();", 2000);
            } else {
                jq(infoPanel).removeClass('info')
                    .addClass('alert')
                    .html(res.rs2)
                    .show();
            }
        });        
        PopupKeyUpActionProvider.ClearActions();
    };

    this.ClosePwdReminder = function () {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };

    this.ConfirmInvite = function () {
        __doPostBack('confirmInvite', '');
    };

    this.Login = function () {
        document.forms[0].submit();
    };

    this.CloseInviteJoinDialog = function () {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };


    this.ShowAdminMessageDialog = function () {

        if (jq('#studio_admMessDialog:visible').length > 0 && jq('#studio_admMessage:visible').length == 0) {
            this.SendAdmMail1stState();
            return;
        }

        jq('#GreetingBlock .help-block-signin .signUpBlock .join').removeClass('opened');
        jq('#GreetingBlock .help-block-signin .signUpBlock .mess').addClass('opened');
        jq('#studio_invJoinDialog').hide();
        jq('#studio_admMessDialog').show();
        jq('#studio_admMessInfo').html('');
        jq('#studio_yourEmail').val('');
        jq('#studio_yourSituation').val('');

        jq('#studio_admMessContent').show();
        jq('#studio_admMessage').hide();

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.CtrlEnterAction = 'AuthManager.SendAdminMessage();';
    };

    this.SendAdminMessage = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#adm_panel_buttons').hide();
                jq('#adm_action_loader').show();
            } else {
                jq('#adm_action_loader').hide();
                jq('#adm_panel_buttons').show();

            }
        };

        AuthCommunicationsController.SendAdmMail(jq('#studio_yourEmail').val(), jq('#studio_yourSituation').val(), function (result) {
            var res = result.value;
            if (res.Status == 1) {
                jq('#studio_admMessage').html(res.Message);
                jq('#studio_admMessContent').hide();
                jq('#studio_admMessage').show();

                setTimeout('AuthManager.SendAdmMail1stState();', 3000);
            } else {
                jq('#studio_admMessage').html('<div class="errorBox">' + res.Message + '</div>');
            }
        });
    };

    this.SendAdmMail1stState = function () {
        jq('#GreetingBlock .help-block-signin .signUpBlock .mess').removeClass('opened');
        jq('#studio_admMessDialog').hide();
    };

    this.ShowInviteJoinDialog = function (userID) {
        if (jq('#studio_invJoinDialog:visible').length > 0 && jq('#studio_invJoinMessage:visible').length == 0) {
            this.ShowInviteJoin1stState();
            return;
        }
        jq('#GreetingBlock .help-block-signin .signUpBlock .mess').removeClass('opened');
        jq('#GreetingBlock .help-block-signin .signUpBlock .join').addClass('opened');
        jq('#studio_invJoinInfo').html('');
        jq('#studio_joinEmail').val('');

        jq("#studio_admMessDialog").hide();
        jq("#studio_invJoinDialog").show();

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.CtrlEnterAction = "AuthManager.SendInviteJoinMail();";

        jq("#studio_invJoinContent").show();
        jq("#studio_invJoinMessage").hide();
    };

    this.SendInviteJoinMail = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq("#join_inv_panel_buttons").hide();
                jq("#join_inv_action_loader").show();
            } else {
                jq("#join_inv_action_loader").hide();
                jq("#join_inv_panel_buttons").show();
            }
        };

        AuthCommunicationsController.SendJoinInviteMail(jq("#studio_joinEmail").val(), function (result) {
            var res = result.value;
            if (res.rs1 == 1) {
                jq("#studio_invJoinMessage").html(res.rs2);
                jq("#studio_invJoinContent").hide();
                jq("#studio_invJoinMessage").show();
                setTimeout("AuthManager.ShowInviteJoin1stState();", 3000);
            } else {
                jq("#studio_invJoinInfo").html('<div class="errorBox">' + res.rs2 + '</div>');
            }
        });
    };

    this.ShowInviteJoin1stState = function () {
        jq("#GreetingBlock .help-block-signin .signUpBlock .join").removeClass("opened");
        jq("#studio_invJoinDialog").hide();
    };

    this.EditProfileCallback = function (makeResult) {
        if (makeResult.Action == 'edit_user') {
            if (makeResult.SelfProfile)
                window.location.reload(true);
        }
    };

    this.RemoveUser = function (userID, userName, successCallback) {
        jq("#actionMenu").hide();

        jq("#confirmationDeleteUserPanel .confirmationAction").html(jq.format(this.ConfirmRemoveUser, Encoder.htmlEncode(userName)));
        jq("#confirmationDeleteUserPanel .middle-button-container>.button.blue.middle").unbind("click").bind("click", function () {
            AjaxPro.onLoading = function (b) {
                if (b) {
                    jq.blockUI();
                } else {
                    jq.unblockUI();
                }
            };
            AjaxPro.UserProfileControl.RemoveUser(userID, function (result) {
                var res = result.value;
                if (typeof(successCallback) === "function") {
                    successCallback(res);
                } else {
                    var $infoPanel = jq("#studio_userProfileCardInfo");
                    if ($infoPanel.length == 1) {
                        if (res.rs1 == '1') {
                            $infoPanel.html('<div class="okBox">' + res.rs2 + '</div>');
                            jq(".userProfileCard").hide();
                            jq(".user-block").hide();
                            jq("#userMenu").remove();
                        } else {
                            $infoPanel.html('<div class="errorBox">' + res.rs2 + '</div>');
                        }
                    }
                }
            });
        });
        StudioBlockUIManager.blockUI("#confirmationDeleteUserPanel", 500, 500, 0);
    };

    this.ResendActivation = function (userID) {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq.blockUI();
            } else {
                jq.unblockUI();
            }
        };

        AjaxPro.UserProfileControl.ResendActivation(userID, function (result) {
            var res = result.value;
            var $infoPanel = jq("#studio_userProfileCardInfo");
            if ($infoPanel.length == 1) {
                if (res.rs1 == '1') {
                    window.location.reload(true);
                } else {
                    $infoPanel.html('<div class="errorBox">' + res.rs2 + '</div>');
                }
            }
        });
    };

    this.CreateAdmin = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq.blockUI();
            } else {
                jq.unblockUI();
            }
        };

        Wizard.CreateAdmin(jq('#studio_adminFirstName').val(),
            jq('#studio_adminLastName').val(),
            jq('#studio_adminEmail').val(),
            jq('#studio_adminPwd').val(),
            function (result) {
                var res = result.value;
                if (res.rs1 == '1') {
                    window.location.reload(true);
                } else {
                    jq('#studio_wizardMessage').html('<div class="errorBox">' + res.rs2 + '</div>');
                }
            });
    };
};

/**
 * StudioManager
 */
var StudioManager = new function () {
    this.GetImage = function (imageName) {
        return ASC.Resources.Master.ImageWebPath + "/" + imageName;
    };

    this.getLocationPathToModule = function (moduleName) { // example path = jq.getLocationPathToModule("projects") - http://localhost/asc/products/projects/
        var products = "products";
        var mass = location.href.toLowerCase().split(products);
        var path = mass[0] + products + "/" + moduleName + "/";
        return path;
    };

    this.getBasePathToModule = function () {
        var products = "products",
            addons = "addons",
            mass,
            parts,
            moduleName = "",
            path = "/";

        mass = location.pathname.toLowerCase().split(products);
        if (mass.length > 1) {
            parts = mass[1].split('/');
            if (parts.length > 1) {
                moduleName = parts[1] + "/";
            }
            path = mass[0] + products + "/" + moduleName;
            return path;
        }

        mass = location.pathname.toLowerCase().split(addons);
        if (mass.length > 1) {
            parts = mass[1].split('/');
            if (parts.length > 1) {
                moduleName = parts[1] + "/";
            }
            path = mass[0] + addons + "/" + moduleName;
            return path;
        }
        return path;
    };

    this.createCustomSelect = function (selects, hiddenBorder, AdditionalBottomOption) {
        if (typeof selects === 'string') {
            selects = document.getElementById(selects);
        }
        if (!selects || typeof selects !== 'object') {
            return undefined;
        }
        if (typeof selects.join !== 'function' && !(selects instanceof String)) {
            selects = [selects];
        }

        for (var i = 0, n = selects.length; i < n; i++) {
            var select = selects[i];
            var selectValue = select.value;


            if (select.className.indexOf('originalSelect') !== -1) {
                continue;
            }

            var container = document.createElement('div');
            container.setAttribute('value', selectValue);
            container.className = select.className + (select.className.length ? ' ' : '') + 'customSelect';
            var position = (document.defaultView && document.defaultView.getComputedStyle) ? document.defaultView.getComputedStyle(select, '').getPropertyValue('position') : (select.currentStyle ? select.currentStyle['position'] : 'static');
            container.style.position = position === 'static' ? 'relative' : position;

            var title = document.createElement('div');
            title.className = 'title' + ' ' + selectValue;
            title.style.height = '100%';
            title.style.position = 'relative';
            title.style.zIndex = '1';
            container.appendChild(title);

            var selector = document.createElement('div');
            selector.className = 'selector';
            selector.style.position = 'absolute';
            selector.style.zIndex = '1';
            selector.style.right = '0';
            selector.style.top = '0';
            selector.style.height = '100%';
            container.appendChild(selector);

            var optionsList = document.createElement('ul');
            optionsList.className = 'options';
            optionsList.style.display = 'none';
            optionsList.style.position = 'absolute';
            optionsList.style.zIndex = '777';
            optionsList.style.width = '115%';
            optionsList.style.maxHeight = '200px';
            optionsList.style.overflow = 'auto';
            container.appendChild(optionsList);

            var optionHtml = '',
                optionValue = '',
                optionTitle = '',
                optionClassName = '',
                fullClassName = '',
                option = null,
                options = select.getElementsByTagName('option');
            for (var j = 0, m = options.length; j < m; j++) {
                option = document.createElement('li');
                optionValue = options[j].value;
                optionHtml = options[j].innerHTML;
                optionTitle = optionHtml.replace(/&amp;/gi, "&");
                optionsList.appendChild(option);
                fullClassName = 'option' + ' ' + optionValue + ((optionClassName = options[j].className) ? ' ' + optionClassName : '');
                option.setAttribute('title', optionTitle);
                option.setAttribute('value', optionValue);
                if (selectValue === optionValue) {
                    title.innerHTML = optionHtml;
                    fullClassName += ' selected';
                }
                option.selected = selectValue === optionValue;
                option.innerHTML = optionHtml;
                option.className = fullClassName;
            }
            if (AdditionalBottomOption !== undefined && AdditionalBottomOption !== "") {
                optionsList.appendChild(AdditionalBottomOption);
            }

            select.parentNode.insertBefore(container, select);
            container.appendChild(select);

            select.className += (select.className.length ? ' ' : '') + 'originalSelect';
            select.style.display = 'none';

            if (hiddenBorder) {
                jq(container).addClass('comboBoxHiddenBorder');
            }


            jq(optionsList).bind('selectstart', function () {
                return false;
            }).mousedown(function () {
                return false;
            }).click(function (evt) {
                var $target = jq(evt.target);
                if ($target.hasClass('option')) {
                    var containerNewValue = evt.target.getAttribute('value'),
                        $container = $target.parents('div.customSelect:first'),
                        container = $container.get(0);
                    if (!container || container.getAttribute('value') === containerNewValue) {
                        return undefined;
                    }
                    container.setAttribute('value', containerNewValue);
                    $container.find('li.option').removeClass('selected').filter('li.' + containerNewValue + ':first').addClass('selected');
                    $container.find('div.title:first').html($target.html() || '&nbsp;')
                        .attr('className', 'title ' + containerNewValue)
                        .attr('class', 'title ' + containerNewValue);
                    $container.find('select.originalSelect:first').val(containerNewValue).change();
                }
            });
            if (jq.browser.msie && jq.browser.version < 7) {
                jq(optionsList).find('li.option').hover(
                    function () {
                        jq(this).addClass('hover');
                    }, function () {
                        jq(this).removeClass('hover');
                    });
            }

            jq(selector).add(title).bind('selectstart', function () {
                return false;
            }).mousedown(function () {
                return false;
            }).click(function (evt) {
                var $options = jq(this.parentNode).find('ul.options:first');
                if ($options.is(':hidden')) {
                    $options.css({
                        top: jq(this.parentNode).height() + 1 + 'px'
                    }).slideDown(1, function () {
                        jq(document).one('click', function () {
                            jq('div.customSelect ul.options').hide();
                            jq(container).removeClass('comboBoxHiddenBorderFocused');
                        });
                    });

                    if (hiddenBorder) {
                        if (!jq(container).hasClass('comboBoxHiddenBorderFocused')) {
                            container.className = 'comboBoxHiddenBorderFocused ' + container.className;
                        }
                        var leftOffset = jq(container).width() - jq(select).width() - 1;
                        //                        if (jQuery.browser.mozilla) { leftOffset -= 1;    }
                        $options.css({
                            'width': jq(select).width(),
                            'border-top': '1px solid #c7c7c7',
                            'left': leftOffset,
                            'top': jq(container).height()
                        });
                    }
                }
            });
        }
    };

    this.CloseAddContentDialog = function () {
        jq.unblockUI();
        return false;
    };

    this.ShowAddContentDialog = function () {
        StudioBlockUIManager.blockUI("#studio_AddContentDialog", 400, 350, 0);
    };

    this.Disable = function (obj_id) {
        jq('#' + obj_id).addClass("disable");
    };

    this.Enable = function (obj_id) {
        jq('#' + obj_id).removeClass("disable");
    };

    this.initImageZoom = function (options) {
        var setting = jq.extend({
            rel: true,
            scrolling: false,
            photo: true,
            retinaImage: true,
            opacity: 0.5,
            current: "{current} / {total}",
            previous: "<",
            next: ">",
        }, options);

        jq(".screenzoom").colorbox(setting);
    };
};

/**
 * EventTracker
 */
var EventTracker = new function () {
    this.Track = function (event) {

        try {
            var pageTracker = _gat._getTracker("UA-12442749-4");

            if (pageTracker != null) {
                pageTracker._trackPageview(event);
            }
        } catch (err) {
        }
    };
};

/*--------Error messages for required field-------*/
function ShowRequiredError (item, withouthScroll) {
    jq("div[class='infoPanel alert']").hide();
    jq("div[class='infoPanel alert']").empty();
    var parentBlock = jq(item).parents(".requiredField");
    jq(parentBlock).addClass("requiredFieldError");

    if (typeof(withouthScroll) == "undefined" || withouthScroll == false) {
        jq.scrollTo(jq(parentBlock).position().top - 50, {speed: 500});
    }
    jq(item).focus();
}

function HideRequiredError () {
    jq(".requiredField").removeClass("requiredFieldError");
}

function RemoveRequiredErrorClass (item) {
    var parentBlock = jq(item).parents(".requiredField");
    jq(parentBlock).removeClass("requiredFieldError");
}

function AddRequiredErrorText (item, text) {
    var parent = jq(item).parents(".requiredField");
    var errorSpan = jq(parent).children(".requiredErrorText");
    jq(errorSpan).text(text);
}

/**
 * EmailOperationManager
 */
var EmailOperationManager = new function () {

    this.SendEmailActivationInstructionsOnChange = function (oldEmail, newEmail, newEmailConfirm, queryString) {
        jq("[id$='studio_confirmMessage']").hide();
        jq("[id$='studio_confirmMessage'] div").hide();
        jq("#emailInputContainer").addClass("display-none"); //hide email on send
        EmailOperationService.SendEmailActivationInstructionsOnChange(oldEmail, newEmail, newEmailConfirm, queryString, this._SendEmailActivationInstructionsOnChangeResponse);
    };

    this._SendEmailActivationInstructionsOnChangeResponse = function (response) {
        jq("[id$='studio_confirmMessage']").show();

        if (response.error != null) {
            jq("#studio_confirmMessage_errortext").text(response.error.Message).show();
            return;
        }
        if (response.value.status == "success") {
            jq("#studio_confirmMessage_successtext").html(response.value.message).show();
            jq("#emailInputContainer").addClass("display-none");
            jq("#currentEmailText").hide();
        }
        if (response.value.status == "error") {
            jq("#studio_confirmMessage_errortext").text(response.value.message).show();
            jq("#emailInputContainer").removeClass("display-none");
        }
        if (response.value.status == "fatalerror") {
            jq("#studio_confirmMessage_errortext").text(response.value.message).show();
            jq("#emailInputContainer").addClass("display-none");
        }
    };

    this.SendEmailActivationInstructions = function (userEmail, userID) {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_emailChangeDialog').block();
            } else {
                jq('#studio_emailChangeDialog').unblock();
            }
        };
        EmailOperationService.SendEmailActivationInstructions(userID, userEmail, function (response) {
            EmailOperationManager._EmailOperationInstructionsServerResponse(response);
        });
    };

    this.SendEmailChangeInstructions = function (userEmail, userID) {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_emailChangeDialog').block();
            } else {
                jq('#studio_emailChangeDialog').unblock();
            }
        };
        EmailOperationService.SendEmailChangeInstructions(userID, userEmail, function (response) {
            EmailOperationManager._EmailOperationInstructionsServerResponse(response);
        });
    };

    this._EmailOperationInstructionsServerResponse = function (response) {
        if (response.error != null) {
            jq("#divEmailOperationError").html(response.error.Message).show();
        } else {
            jq("#divEmailOperationError").html("").hide();
            jq("#studio_emailOperationContent").addClass("display-none");
            jq("#studio_emailOperationResult").show();
            jq("#studio_emailOperationResultText").html(response.value);
            PopupKeyUpActionProvider.EnterAction = "EmailOperationManager.CloseEmailOperationWindow();";
        }
    };

    this.ShowEmailChangeWindow = function (userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").removeClass("display-none");
            jq("#emailMessageContainer").addClass("display-none");
        } else {
            jq("#emailInputContainer").addClass("display-none");
            jq("#emailMessageContainer").removeClass("display-none");

            jq("#resendInviteText").addClass("display-none");
            jq("#emailActivationText").addClass("display-none");
            jq("#emailChangeText").removeClass("display-none");

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#resendInviteDialogPopupHeader").addClass("display-none");
        jq("#emailActivationDialogPopupHeader").addClass("display-none");
        jq("#emailChangeDialogPopupHeader").removeClass("display-none");

        jq("#studio_emailOperationContent").removeClass("display-none");

        jq("#emailChangeDialogText").removeClass("display-none");
        jq("#resendInviteDialogText").addClass("display-none");
        jq("#emailActivationDialogText").addClass("display-none");

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function () {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailChangeInstructions(newEmail, userID);
            return false;
        });
    };

    this.ShowEmailActivationWindow = function (userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").removeClass("display-none");
            jq("#emailMessageContainer").hide();
        } else {
            jq("#emailInputContainer").addClass("display-none");
            jq("#emailMessageContainer").show();

            jq("#resendInviteText").hide();
            jq("#emailActivationText").show();
            jq("#emailChangeText").hide();

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#emailActivationDialogPopupHeader").show();
        jq("#emailChangeDialogPopupHeader").hide();
        jq("#resendInviteDialogPopupHeader").hide();

        jq("#studio_emailOperationContent").removeClass("display-none");

        jq("#emailChangeDialogText").hide();
        jq("#resendInviteDialogText").hide();
        jq("#emailActivationDialogText").show();

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function () {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailActivationInstructions(newEmail, userID);
            return false;
        });

    };

    this.ShowResendInviteWindow = function (userEmail, userID, adminMode) {
        jq("#divEmailOperationError").html("").hide();
        jq("#studio_emailOperationResult").hide();

        if (adminMode == true) {
            jq("#emailInputContainer").removeClass("display-none");
            jq("#emailMessageContainer").hide();
        } else {
            jq("#emailInputContainer").addClass("display-none");
            jq("#emailMessageContainer").show();

            jq("#emailActivationText").hide();
            jq("#emailChangeText").hide();
            jq("#resendInviteText").show();

            jq("#emailMessageContainer [name='userEmail']").attr("href", "mailto:" + userEmail).html(userEmail);
        }

        jq("#emailActivationDialogPopupHeader").hide();
        jq("#emailChangeDialogPopupHeader").hide();
        jq("#resendInviteDialogPopupHeader").show();

        jq("#studio_emailOperationContent").removeClass("display-none");

        jq("#emailChangeDialogText").hide();
        jq("#emailActivationDialogText").hide();
        jq("#resendInviteDialogText").show();

        jq("#emailOperation_email").val(userEmail);

        this.OpenPopupDialog();

        jq("#btEmailOperationSend").unbind("click");

        jq("#btEmailOperationSend").click(function () {
            var newEmail = jq("#emailOperation_email").val();
            EmailOperationManager.SendEmailActivationInstructions(newEmail, userID);
            return false;
        });

    };

    this.OpenPopupDialog = function () {
        StudioBlockUIManager.blockUI("#studio_emailChangeDialog", 400, 300, 0);

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = "jq(\"#btEmailOperationSend\").click();";
    };

    this.CloseEmailOperationWindow = function () {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
        document.location.reload(true);
    };

    this.SendInstructions = function (userID, userEmail) {
        if (EmailOperationService) {
            EmailOperationService.SendEmailActivationInstructions(userID, userEmail, function (res) {
                if (res.error != null) {
                    return false;
                }
                jq(".info-box.excl .first-step").addClass("display-none");
                jq(".info-box.excl .second-step").removeClass("display-none");
            });
        }
    };
};

/**
 * ProfileManager
 */
var ProfileManager = new function () {
    this.SendInstrunctionsToRemoveProfile = function () {
        AjaxPro.UserProfileControl.SendInstructionsToDelete(function (result) {
            ProfileManager.ShowResultStatus(result.value);
        });
    };

    this.ShowResultStatus = function (result) {
        var res = '<div class="operationInfoBox">' + result.Message + '</div>';
        jq('#remove_content').html(res);
        jq('#studio_deleteProfileDialog .middle-button-container').html('');

        setTimeout("jq.unblockUI();", 2000);
    };

    this.ShowDeleteUserWindow = function () {
        jq('#actionMenu').hide();
        StudioBlockUIManager.blockUI("#studio_deleteProfileDialog", 400, 300, 0);
    };
};

/**
 * LoadingBanner
 */
LoadingBanner = function () {
    var animateDelay = 2000,
        displayDelay = 500,
        displayOpacity = 1,
        loaderCss = "",
        loaderId = "loadingBanner",
        strLoading = "Loading...",
        strDescription = "Please wait...",
        successId = "successBanner",
        strSuccess = "Successfully completed";
    return {
        animateDelay: animateDelay,
        displayDelay: displayDelay,
        displayOpacity: displayOpacity,
        loaderCss: loaderCss,
        loaderId: loaderId,
        strLoading: strLoading,
        strDescription: strDescription,
        successId : successId,
        strSuccess: strSuccess,

        displayLoading: function (withoutDelay, withBackdrop) {
            var id = "#" + LoadingBanner.loaderId;

            if (jq(id).length != 0)
                return;

            var innerHtml = '<div id="{0}" class="loadingBanner {1}"><div class="loader-block">{2}<div>{3}</div></div></div>'
                .format(LoadingBanner.loaderId, LoadingBanner.loaderCss, LoadingBanner.strLoading, LoadingBanner.strDescription);

            if (withBackdrop)
                jq("body").append('<div class="loadingBannerBackDrop"></div>');

            jq("body").append(innerHtml).addClass("loading");

            if (jq.browser.mobile)
                jq(id).css("top", jq(window).scrollTop() + "px");

            jq(id).animate({opacity: 0}, withoutDelay === true ? 0 : LoadingBanner.displayDelay, function () {
                jq(id).animate({opacity: LoadingBanner.displayOpacity}, LoadingBanner.animateDelay);
            });
        },

        hideLoading: function () {
            jq("#" + LoadingBanner.loaderId).remove();
            jq(".loadingBannerBackDrop").remove();
            jq("body").removeClass("loading");
        },

        displaySuccess: function (){
            var id = "#" + LoadingBanner.successId;

            if (jq(id).length != 0)
                return;

            var innerHtml = '<div id="{0}" class="loadingBanner"><div class="loader-block success">{1}</div></div>'
               .format(LoadingBanner.successId, LoadingBanner.strSuccess);

            jq("body").append(innerHtml);

            if (jq.browser.mobile)
                jq(id).css("top", jq(window).scrollTop() + "px");

            setTimeout(function () { jq(id).remove(); }, 3000);
    }
    };
}();

/************************************************/
/**
 * LeftMenuManager
 */
var LeftMenuManager = new function () {
    var menuObjs = [],
        cookiePath = "/",
        cookieKey = "/";
    var init = function (cookiePath, menuObjs) {
        LeftMenuManager.cookiePath = cookiePath;
        LeftMenuManager.cookieKey = encodeURIComponent(cookiePath);
        LeftMenuManager.menuObjs = menuObjs;
    };

    var updateCookies = function () {
        var newMenuObjStates = [];
        for (var i = 0, n = LeftMenuManager.menuObjs.length; i < n; i++) {
            newMenuObjStates.push(jq(LeftMenuManager.menuObjs[i]).hasClass("open") ? "open" : "");
        }
        jq.cookies.set(LeftMenuManager.cookieKey, jq.toJSON(newMenuObjStates), {path: LeftMenuManager.cookiePath});
    };

    var restoreLeftMenu = function () {
        var menuObjStates = jq.cookies.get(LeftMenuManager.cookieKey);
        if (menuObjStates != null) {
            if (menuObjStates.length == LeftMenuManager.menuObjs.length) {
                for (var i = 0, n = menuObjStates.length; i < n; i++) {
                    var $menu = jq(LeftMenuManager.menuObjs[i]);
                    if ($menu.hasClass("currentCategory") && !$menu.hasClass("active") && !$menu.hasClass("open")) {
                        $menu.addClass("open");
                    }
                    if (menuObjStates[i] == "")
                        continue;
                    if (!$menu.hasClass(menuObjStates[i])) {
                        $menu.addClass(menuObjStates[i]);
                    }
                }
            }
        } else {
            for (var i = 0, n = LeftMenuManager.menuObjs.length; i < n; i++) {
                var $menu = jq(LeftMenuManager.menuObjs[i]);
                if ($menu.hasClass("currentCategory") && !$menu.hasClass("active") && !$menu.hasClass("open")) {
                    $menu.addClass("open");
                }
            }
        }
        updateCookies();
    };

    var bindEvents = function () {
        jq(".page-menu").on("click", ".filter a", function () {
            jq(".page-menu .active").removeClass("active");
            jq(this).parent().addClass("active");
        });

        jq(".page-menu").on("click", ".expander", function () {
            var menuItem = jq(this).closest(".menu-item");
            if (jq(menuItem).hasClass("open")) {
                jq(menuItem).removeClass("open");
            } else {
                jq(menuItem).addClass("open");
            }
            updateCookies();
        });

        var $dropDownObj = jq("#createNewButton");
        if ($dropDownObj.length == 1) {
            jq.dropdownToggle({
                switcherSelector: ".without-separator",
                dropdownID: "createNewButton",
                noActiveSwitcherSelector: ".with-separator .white-combobox",
                addTop: 1,
                addLeft: 0
            });

            $dropDownObj.show();
            var cornerLeft = $dropDownObj.children(".corner-top").position().left;
            $dropDownObj.hide();
            var addLeft = -cornerLeft + 7;

            jq.dropdownToggle({
                switcherSelector: ".with-separator .white-combobox",
                dropdownID: "createNewButton",
                noActiveSwitcherSelector: ".without-separator",
                position: "absolute",
                addTop: 2,
                addLeft: addLeft
            });

            jq(".menu-main-button .main-button-text").click(function (event) {
                if (!jq(this).hasClass("override")) {
                    jq(".menu-main-button .white-combobox").click();
                    event.stopPropagation();
                }
            });
        }

        jq.dropdownToggle({
            switcherSelector: "#menuOtherActionsButton",
            dropdownID: "otherActions",
            position: "absolute",
            addTop: 1,
            addLeft: 0
        });
    };

    return {
        init: init,
        updateCookies: updateCookies,
        restoreLeftMenu: restoreLeftMenu,
        bindEvents: bindEvents
    };
};

/**
* ScrolledGroupMenu
*/

/* Example from CRM contact list: */
//    options = {
//      menuSelector: "#contactsHeaderMenu",
//      menuAnchorSelector: "#mainSelectAll",
//      menuSpacerSelector: "#companyListBox .header-menu-spacer",
//      userFuncInTop: function() { jq("#contactsHeaderMenu .menu-action-on-top").hide(); },
//      userFuncNotInTop: function() { jq("#contactsHeaderMenu .menu-action-on-top").show(); }
//    }
//    ScrolledGroupMenu.init(options);

var ScrolledGroupMenu = new function () {

    var init = function (options) {
        jq(window).scroll(function () {
            stickMenuToTheTop(options);
        });
        jq(window).resize(function () {
            resizeContentHeaderWidth(options.menuSelector);
        });
    };
    var stickMenuToTheTop = function (options) {
        if (typeof(options) != "object" ||
            typeof(options.menuSelector) == "undefined" ||
            typeof(options.menuAnchorSelector) == "undefined" ||
            typeof(options.menuSpacerSelector) == "undefined") {
            return;
        }

        var $menuObj = jq(options.menuSelector);
        var $boxTop = jq(options.menuAnchorSelector);
        var $menuSpacerObj = jq(options.menuSpacerSelector);

        if ($menuObj.length == 0 || $boxTop.length == 0 || $menuSpacerObj.length == 0) {
            return;
        }

        var newTop = $boxTop.offset().top + $boxTop.outerHeight();
        var winScrollTop = jq(window).scrollTop();
        var tempTop = 0;

        if ($menuSpacerObj.css("display") == "none") {
            tempTop += $menuObj.offset().top;
        } else {
            tempTop += $menuSpacerObj.offset().top;
        }

        if (winScrollTop >= tempTop) {
            $menuSpacerObj.show();

            fixContentHeaderWidth(jq(options.menuSelector));

            if (typeof(options.userFuncNotInTop) == "function") {
                options.userFuncNotInTop();
            }

            $menuObj.css(
                {
                    "top": "0px",
                    "position": "fixed"
                });
            jq(".studio-action-panel.group-actions").css(
                {
                    "top": jq(document).scrollTop() + $menuObj.outerHeight() - 4 + "px",
                    "position": "absolute"
                });
        } else {
            $menuSpacerObj.hide();
            $menuObj.css("width", "auto");

            if (typeof(options.userFuncInTop) == "function") {
                options.userFuncInTop();
            }

            $menuObj.css(
                {
                    "position": "static"
                });
            jq(".studio-action-panel.group-actions").css(
                {
                    "top": $menuObj.offset().top + $menuObj.outerHeight() - 4 + "px"
                });
        }
    };

    var fixContentHeaderWidth = function (header) {
        jq(header).css("width",
            jq(header).parent().innerWidth()
                - parseInt(jq(header).css("margin-left"))
                - parseInt(jq(header).css("margin-right"))
                - parseInt(jq(header).css("padding-left"))
                - parseInt(jq(header).css("padding-right")));
    };

    var resizeContentHeaderWidth = function (header) {
        jq(header).css("width", "");
        fixContentHeaderWidth(header);
    };

    return {
        init: init,
        stickMenuToTheTop: stickMenuToTheTop,
        fixContentHeaderWidth: fixContentHeaderWidth,
        resizeContentHeaderWidth: resizeContentHeaderWidth
    };
};

/**
 * File Size Manager
 */
var FileSizeManager = new function () {
    var filesSizeToString = function (size) {
        var sizeNames = ASC.Resources.Master.FileSizePostfix ? ASC.Resources.Master.FileSizePostfix.split(',') : ["bytes", "KB", "MB", "GB", "TB"];
        var power = 0;

        var resultSize = size;
        if (1024 <= resultSize) {
            power = parseInt(Math.log(resultSize) / Math.log(1024));
            power = power < sizeNames.length ? power : sizeNames.length - 1;
            resultSize = resultSize / Math.pow(1024, power);
        }

        var intRegex = /^\d+$/;
        if (intRegex.test(resultSize)) {
            resultSize = parseInt(resultSize);
        } else {
            resultSize = parseFloat(resultSize.toFixed(2));
        }

        return jq.format("{0} {1}", resultSize, sizeNames[power]);
    };

    return {
        filesSizeToString: filesSizeToString
    };
};

/**
 * Encoder
 */
Encoder = { EncodeType: "entity", isEmpty: function(val) { if (val) { return ((val === null) || val.length == 0 || /^\s+$/.test(val)); } else { return true; } }, HTML2Numerical: function(s) { var arr1 = new Array('&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'); var arr2 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'); return this.swapArrayVals(s, arr1, arr2); }, NumericalToHTML: function(s) { var arr1 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;'); var arr2 = new Array('&nbsp;', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;'); return this.swapArrayVals(s, arr1, arr2); }, numEncode: function(s) { if (this.isEmpty(s)) return ""; var e = ""; for (var i = 0; i < s.length; i++) { var c = s.charAt(i); if (c < " " || c > "~") { c = "&#" + c.charCodeAt() + ";"; } e += c; } return e; }, htmlDecode: function(s) { var c, m, d = s; if (this.isEmpty(d)) return ""; d = this.HTML2Numerical(d); arr = d.match(/&#[0-9]{1,5};/g); if (arr != null) { for (var x = 0; x < arr.length; x++) { m = arr[x]; c = m.substring(2, m.length - 1); if (c >= -32768 && c <= 65535) { d = d.replace(m, String.fromCharCode(c)); } else { d = d.replace(m, ""); } } } return d; }, htmlEncode: function(s, dbl) { if (this.isEmpty(s)) return ""; dbl = dbl | false; if (dbl) { if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;"); } else { s = s.replace(/&/g, "&amp;"); } } s = this.XSSEncode(s, false); if (this.EncodeType == "numerical" || !dbl) { s = this.HTML2Numerical(s); } s = this.numEncode(s); if (!dbl) { s = s.replace(/&#/g, "##AMPHASH##"); if (this.EncodeType == "numerical") { s = s.replace(/&/g, "&#38;"); } else { s = s.replace(/&/g, "&amp;"); } s = s.replace(/##AMPHASH##/g, "&#"); } s = s.replace(/&#\d*([^\d;]|$)/g, "$1"); if (!dbl) { s = this.correctEncoding(s); } if (this.EncodeType == "entity") { s = this.NumericalToHTML(s); } return s; }, XSSEncode: function(s, en) { if (!this.isEmpty(s)) { en = en || true; if (en) { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&quot;"); s = s.replace(/</g, "&lt;"); s = s.replace(/>/g, "&gt;"); } else { s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&#34;"); s = s.replace(/</g, "&#60;"); s = s.replace(/>/g, "&#62;"); } return s; } else { return ""; } }, hasEncoded: function(s) { if (/&#[0-9]{1,5};/g.test(s)) { return true; } else if (/&[A-Z]{2,6};/gi.test(s)) { return true; } else { return false; } }, stripUnicode: function(s) { return s.replace(/[^\x20-\x7E]/g, ""); }, correctEncoding: function(s) { return s.replace(/(&amp;)(amp;)+/, "$1"); }, swapArrayVals: function(s, arr1, arr2) { if (this.isEmpty(s)) return ""; var re; if (arr1 && arr2) { if (arr1.length == arr2.length) { for (var x = 0, i = arr1.length; x < i; x++) { re = new RegExp(arr1[x], 'g'); s = s.replace(re, arr2[x]); } } } return s; }, inArray: function(item, arr) { for (var i = 0, x = arr.length; i < x; i++) { if (arr[i] === item) { return i; } } return -1; } }
less = {}; less.env = 'development';
