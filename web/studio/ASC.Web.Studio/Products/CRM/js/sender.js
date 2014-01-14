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

if (typeof ASC === "undefined") {
    ASC = {};
}

if (typeof ASC.CRM === "undefined") {
    ASC.CRM = (function () { return {}; })();
}

ASC.CRM.SmtpSender = (function () {
    var activateFileUploader = function () {
        ASC.CRM.FileUploader.activateUploader();
        ASC.CRM.FileUploader.fileIDs.clear();
    };

    var initFileUploaderCallback = function () {
        ASC.CRM.FileUploader.OnAllUploadCompleteCallback_function = function () {
            jq('#sendEmailPanel').hide();
            jq("#sendProcessPanel").show();

            var contacts = ASC.CRM.SmtpSender.selectedItems.map(function (item) { return item.id; }),
                watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                                        jq.format(ASC.CRM.Resources.CRMJSResource.TeamlabWatermark,
                                        jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com"))
            );

            var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
            if (subj == "") {
                 subj = ASC.CRM.Resources.CRMJSResource.NoSubject;
            }

            var storeInHistory = jq("#storeInHistory").is(":checked");

            AjaxPro.SmtpSender.SendEmail(ASC.CRM.FileUploader.fileIDs,
                    contacts,
                    subj,
                    ASC.CRM.SmtpSender.fckEditor.GetHTML() + watermark,
                    storeInHistory,
                    function (res) {
                        if (res.error != null) {
                            alert(res.error.Message);
                            return;
                        }
                        ASC.CRM.SmtpSender.checkSendStatus(true);
                    });
        };
    };
    
    var initFckEeditor = function () {
        var editorObj = window.FCKeditorAPI.GetInstance(ASC.CRM.SmtpSender.editorClientId);
        ASC.CRM.SmtpSender.fckEditor = editorObj;
        ASC.CRM.SmtpSender.fckEditor.Config.ToolbarSets.Email = [
            ["Source"],
            ["Undo", "Redo"],
            ["Bold", "Italic", "Underline", "StrikeThrough"],
            ["Link", "Unlink"],
            ["TextColor", "BGColor"],
            ["FontSize", "FontName"],
            ["Smiley"],
            ["UnorderedList", "OrderedList"],
            ["JustifyLeft", "JustifyCenter", "JustifyRight"]
        ];
        ASC.CRM.SmtpSender.fckEditor.ToolbarSet.Load("Email");
        activateFileUploader();
    };

    var showUploaderAndEditor = function () {
        if (!jq.browser.mobile && !(jq.browser.msie && (jq.browser.version == 10 || jq.browser.version == 11))) {
            return true;
        } else {
            return false;
        }
    };

    var checkValidation = function () {
        if (ASC.CRM.SmtpSender.selectedItems.length > ASC.CRM.SmtpSender.emailQuotas) {
            console.log(jq.format(ASC.CRM.Resources.CRMJSResource.ErrorEmailRecipientsCount, ASC.CRM.SmtpSender.emailQuotas));
            return false;
        }

        if (!ASC.CRM.SmtpSender.selectedItems.length) {
            console.log("Empty recipient list.");
            return false;
        }

        if (!ASC.CRM.ListContactView.checkSMTPSettings()) {
            console.log("Eempty field in smtp settings.");
            return false;
        }

        return true;
    };

    return {
        selectedItems: [],
        emailQuotas: 0,
        editorClientId: "",
        fckEditor: null,

        init: function (emailQuotas, editorClientId) {
            ASC.CRM.SmtpSender.selectedItems = ASC.CRM.SmtpSender.getItems();
            ASC.CRM.SmtpSender.emailQuotas = emailQuotas;
            ASC.CRM.SmtpSender.editorClientId = editorClientId;
            if (!checkValidation()) {
                window.location.href = "default.aspx";
            } else {
                if (showUploaderAndEditor()) {
                    initFileUploaderCallback();
                    ASC.CRM.SmtpSender.waitFckEditorAPI();
                } else {
                    ASC.CRM.SmtpSender.showSendEmailPanel();
                }
            }
        },

        waitFckEditorAPI: function () {
            if (typeof(window.FCKeditorAPI) != "undefined" && window.FCKeditorAPI != null) {
                initFckEeditor();
                ASC.CRM.SmtpSender.showSendEmailPanel();
            } else {
                setTimeout(ASC.CRM.SmtpSender.waitFckEditorAPI, 300);
            }
        },

        showSendEmailPanel: function () {
            AjaxPro.SmtpSender.GetStatus(function (res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }
                if (res.value == null || res.value.IsCompleted) {
                    jq("#sendEmailPanel #emailFromLabel").text(jq.format("{0} ({1})", ASC.CRM.Data.smtpSettings.SenderDisplayName, ASC.CRM.Data.smtpSettings.SenderEmailAddress));
                    jq("#sendEmailPanel #previewEmailFromLabel").text(jq.format("{0} ({1})", ASC.CRM.Data.smtpSettings.SenderDisplayName, ASC.CRM.Data.smtpSettings.SenderEmailAddress));
                    jq("#tbxEmailSubject").val("");

                    if (showUploaderAndEditor()) {
                        ASC.CRM.SmtpSender.fckEditor.SetHTML("<br />");
                    } else {
                        jq("#mobileMessageBody").val("");
                    }

                    jq("#storeInHistory").prop("checked", false);

                    jq("#sendButton").text(ASC.CRM.Resources.CRMJSResource.NextPreview).unbind("click").bind("click", function () {
                        ASC.CRM.SmtpSender.showSendEmailPanelPreview();
                    });

                    jq("#backButton a.button.blue.middle").unbind("click").bind("click", function () {
                        ASC.CRM.SmtpSender.showSendEmailPanelCreate();
                    });

                    var count = ASC.CRM.SmtpSender.selectedItems.length,
                        countString =
                                count == 1 ?
                                ASC.CRM.Resources.CRMJSResource.AddressGenitiveSingular :
                                ASC.CRM.Resources.CRMJSResource.AddressGenitivePlural;

                    jq("#emailAddresses").html([count, countString].join(" "));
                    jq("#previewEmailAddresses").html([count, countString].join(" "));

                    jq("#sendEmailPanel").show();
                    jq("#sendEmailPanel #createContent").show();
                    jq("#sendEmailPanel #previewContent").hide();
                    
                    jq("#sendProcessPanel").hide();
                } else {
                    ASC.CRM.SmtpSender.checkSendStatus(true);
                }
            });
        },

        showSendEmailPanelCreate: function () {
            jq("#createContent").show();
            jq("#previewContent").hide();
            jq("#sendProcessPanel").hide();
            jq("#backButton").hide();
            jq("#sendButton").text(ASC.CRM.Resources.CRMJSResource.NextPreview).unbind("click").bind("click", function () {
                ASC.CRM.SmtpSender.showSendEmailPanelPreview();
            });
        },

        showSendEmailPanelPreview: function () {
            AjaxPro.onLoading = function (b) {
                if (b) {
                    jq("#sendEmailPanel .crm-actionButtonsBlock").hide();
                    jq("#sendEmailPanel .crm-actionProcessInfoBlock").show();
                } else {
                    jq("#sendEmailPanel .crm-actionProcessInfoBlock").hide();
                    jq("#sendEmailPanel .crm-actionButtonsBlock").show();
                }
            };

            var mess = "";

            if (showUploaderAndEditor()) {
                mess = ASC.CRM.SmtpSender.fckEditor.GetHTML();
                if (mess == "<br />") {
                    AddRequiredErrorText(jq("#requiredMessageBody"), ASC.CRM.Resources.CRMJSResource.EmptyLetterBodyContent);
                    ShowRequiredError(jq("#requiredMessageBody"), true);
                    return false;
                } else RemoveRequiredErrorClass(jq("#requiredMessageBody"));
            } else {
                mess = jq("#mobileMessageBody").val().trim();
                if (mess == "") {
                    AddRequiredErrorText(jq("#requiredMessageBody"), ASC.CRM.Resources.CRMJSResource.EmptyLetterBodyContent);
                    ShowRequiredError(jq("#requiredMessageBody"), true);
                    return false;
                } else {
                    RemoveRequiredErrorClass(jq("#requiredMessageBody"));
                }
                mess = mess.replace(/\n/g, "<br />");
            }

            var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
            if (subj == "") {
                subj = ASC.CRM.Resources.CRMJSResource.NoSubject;
            }

            AjaxPro.SmtpSender.GetMessagePreview(mess, ASC.CRM.SmtpSender.selectedItems[0].id, function (res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    jq("#sendEmailPanel .crm-actionButtonsBlock").show();
                    jq("#sendEmailPanel .crm-actionProcessInfoBlock").hide();
                    return false;
                }

                jq("#previewSubject").text(subj);

                var watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                                            jq.format(ASC.CRM.Resources.CRMJSResource.TeamlabWatermark,
                                            jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com"))
                );

                jq("#previewMessage").html(res.value + watermark);

                if (showUploaderAndEditor()) {
                    var attachments = jq("#history_uploadContainer div.studioFileUploaderFileName");
                    jq("#previewAttachments span").html("");
                    if (attachments.length > 0) {
                        attachments.each(function (index) {
                            jq("#previewAttachments span").append(jq(this).text());
                            if (index != attachments.length - 1)
                                jq("#previewAttachments span").append(", ");
                        });
                        jq("#previewAttachments").show();
                    } else {
                        jq("#previewAttachments").hide();
                    }
                }

                jq("#sendProcessPanel").hide();
                jq("#createContent").hide();
                jq("#previewContent").show();
                jq("#backButton").show();
                jq("#sendButton").text(ASC.CRM.Resources.CRMJSResource.Send).unbind("click").bind("click", function () {
                    ASC.CRM.SmtpSender.sendEmail();
                });
                jq("#sendButton").trackEvent(ga_Categories.contacts, ga_Actions.actionClick, 'mass_email');
            });
        },

        sendEmail: function () {
            AjaxPro.onLoading = function (b) { };

            if (showUploaderAndEditor() && fileUploader.GetUploadFileCount() > 0) {
                jq("#" + fileUploader.ButtonID).css("visibility", "hidden");
                jq("#pm_upload_btn_html5").hide();
                fileUploader.Submit();
            } else {
                var contacts = ASC.CRM.SmtpSender.selectedItems.map(function (item) { return item.id; }),
                    watermark = jq.format("<div style='color:#787878;font-size:12px;margin-top:10px'>{0}</div>",
                                            jq.format(ASC.CRM.Resources.CRMJSResource.TeamlabWatermark,
                                            jq.format("<a style='color:#787878;font-size:12px;' href='http://www.teamlab.com'>{0}</a>", "Teamlab.com"))
                );

                var subj = jq("#sendEmailPanel #tbxEmailSubject").val().trim();
                if (subj == "") {
                    subj = ASC.CRM.Resources.CRMJSResource.NoSubject;
                }

                var storeInHistory = jq("#storeInHistory").is(":checked"),
                    letterBody = "";

                if (showUploaderAndEditor()) {
                    letterBody = ASC.CRM.SmtpSender.fckEditor.GetHTML();
                } else {
                    letterBody = jq("#mobileMessageBody").val().trim();
                }

                AjaxPro.SmtpSender.SendEmail(new Array(),
                        contacts,
                        subj,
                        letterBody + watermark,
                        storeInHistory,
                        function (res) {
                            if (res.error != null) {
                                alert(res.error.Message);
                                return;
                            }
                            ASC.CRM.SmtpSender.checkSendStatus(true);
                        });
            }
        },

        checkSendStatus: function (isFirstVisit) {
            jq("#sendEmailPanel").hide();
            jq("#sendProcessPanel").show();
            jq("#sendProcessPanel #abortButton").show();
            jq("#sendProcessPanel #okButton").hide();

            if (isFirstVisit) {
                jq("#sendProcessProgress .progress").css("width", "0%");
                jq("#sendProcessProgress .percent").text("0%");
                jq("#emailsTotalCount").html("");
                jq("#emailsAlreadySentCount").html("");
                jq("#emailsEstimatedTime").html("");
                jq("#emailsErrorsCount").html("");
            }

            AjaxPro.SmtpSender.GetStatus(function (res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }

                if (res.value == null) {
                    jq("#sendProcessProgress .progress").css("width", "100%");
                    jq("#sendProcessProgress .percent").text("100%");
                    jq("#sendProcessPanel #abortButton").hide();
                    jq("#sendProcessPanel #okButton").show();
                    return;
                } else {
                    ASC.CRM.SmtpSender.displayProgress(res.value);
                }

                if (res.value.Error != null && res.value.Error != "") {
                    ASC.CRM.SmtpSender.buildErrorList(res);
                    jq("#sendProcessPanel #abortButton").hide();
                    jq("#sendProcessPanel #okButton").show();
                } else {
                    if (res.value.IsCompleted) {
                        jq("#sendProcessPanel #abortButton").hide();
                        jq("#sendProcessPanel #okButton").show();
                    } else {
                        setTimeout("ASC.CRM.SmtpSender.checkSendStatus(false)", 3000);
                    }
                }
            });
        },

        buildErrorList: function (res) {
            var mess = "error";
            switch (typeof res.value.Error) {
                case "object":
                    mess = res.value.Error.Message + "<br/>";
                    break;
                case "string":
                    mess = res.value.Error;
                    break;
            }

            jq("#emailsErrorsCount")
            .html(jq("<div></div>").addClass("red-text").html(mess))
            .append(jq("<a></a>").attr("href", "settings.aspx?type=common").text(ASC.CRM.Resources.CRMJSResource.GoToSettings));
        },

        abortMassSend: function () {
            AjaxPro.onLoading = function (b) { };
            AjaxPro.SmtpSender.Cancel(function (res) {
                if (res.error != null) {
                    alert(res.error.Message);
                    return;
                }
                if (res.value != null) {
                    ASC.CRM.SmtpSender.displayProgress(res.value);
                }
                jq("#sendProcessPanel #abortButton").hide();
            });
        },

        displayProgress: function (progressItem) {
            jq("#sendProcessProgress .progress").css("width", progressItem.Percentage + "%");
            jq("#sendProcessProgress .percent").text(progressItem.Percentage + "%");
            jq("#emailsAlreadySentCount").html(progressItem.Status.DeliveryCount);
            jq("#emailsEstimatedTime").html(progressItem.Status.EstimatedTime);
            jq("#emailsTotalCount").html(progressItem.Status.RecipientCount);
            jq("#emailsErrorsCount").html("0");
        },

        emailInsertTag: function () {
            var isCompany = jq("#emailTagTypeSelector option:selected").val() == "company",
                tagName = isCompany ? jq('#emailCompanyTagSelector option:selected').val() : jq('#emailPersonTagSelector option:selected').val();

            if (showUploaderAndEditor()) {
                ASC.CRM.SmtpSender.fckEditor.InsertHtml(tagName);
            } else {
                var caretPos = jq("#mobileMessageBody").caret().begin,
                    oldText = jq("#mobileMessageBody").val(),
                    newText = oldText.slice(0, caretPos) + tagName + oldText.slice(caretPos);
                jq("#mobileMessageBody").val(newText);
            }

        },

        renderTagSelector: function () {
            var isCompany = jq("#emailTagTypeSelector option:selected").val() == "company";
            if (isCompany) {
                jq("#emailPersonTagSelector").hide();
                jq("#emailCompanyTagSelector").show();
            } else {
                jq("#emailCompanyTagSelector").hide();
                jq("#emailPersonTagSelector").show();
            }
        },

        setItems: function (targets) {
            var s = JSON.stringify(targets);
            if (!localStorage.hasOwnProperty("senderTargets")) {
                localStorage.setItem("senderTargets", s);
            } else {
                localStorage.senderTargets = s;
            }
        },
        
        getItems: function () {
            var s = "[]";
            if (localStorage.hasOwnProperty("senderTargets")) {
                s = localStorage.senderTargets;
                localStorage.removeItem("senderTargets");
            }
            return JSON.parse(s);
        },
    };
})();