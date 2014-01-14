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
var StudioManagement = new function () {

    //--------------departments management---------------------------------------

    this._groupId = null;
    this._addGroup = '';
    this._editGroup = '';
    this._btnAddGroup = '';
    this._btnEditGroup = '';
    this._deleteGroup = '';

    this.AddDepartmentOpenDialog = function () {
        HideRequiredError();
        StudioBlockUIManager.blockUI("#studio_departmentAddDialog", 400, 300, 0);
        //jq('#addDepartment_info').html('');
        jq("#studio_departmentAddDialog .infoPanel").hide();
        jq("#studio_newDepName").val('');
        headSelector.ClearFilter();
        jq("#grouptitle").html(StudioManagement._addGroup);
        jq("#studio_departmentAddDialog .middle-button-container a.blue").html(StudioManagement._btnAddGroup);
        jq('#' + jq('#addDepartment_infoID').val()).html("<div></div>");
        jq('#' + jq('#addDepartment_infoID').val()).hide();

        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = 'StudioManagement.AddDepartmentCallback()';
    };

    this.EditDepartmentOpenDialog = function (id, gName, gOwnerId) {
        this.AddDepartmentOpenDialog();
        this._groupId = id;
        jq("#grouptitle").html(StudioManagement._editGroup);
        jq("#studio_departmentAddDialog .middle-button-container .blue").html(StudioManagement._btnEditGroup);
        jq("#studio_newDepName").val(gName);

        var obj = document.getElementById("User_" + gOwnerId);
        if (obj != null) {
            headSelector.SelectUser(obj);
        }
    };

    this.AddDepartmentCallback = function () {
        var departmentName = jq("#studio_newDepName");
        if (jq.trim(jq(departmentName).val()) == "") {
            ShowRequiredError(departmentName);
            return;
        }
        var data = {
            groupManager: (headSelector.SelectedUserId != null && headSelector.SelectedUserId != -1) ? headSelector.SelectedUserId : "00000000-0000-0000-0000-000000000000",
            groupName: jq("#studio_newDepName").val(),
            groupId: StudioManagement._groupId,
            members: []
        };

        StudioManagement.LockDepartmentDialog();

        if (StudioManagement._groupId == null) {
            Teamlab.addGroup(null, data, {
                success: function (params) {
                    StudioManagement.CloseAddDepartmentDialog();
                    window.location.reload(true);
                },
                error: function (params, errors) {
                    StudioManagement.UnlockDepartmentDialog();
                    jq("#studio_departmentAddDialog .infoPanel").html("<div>" + errors + "</div>");
                    jq("#studio_departmentAddDialog .infoPanel").show();
                    //console.log('management.js AddDepartmentCallback error', errors);
                }
            });
        } else {
            Teamlab.updateGroup(null, StudioManagement._groupId, data, {
                success: function (params) {
                    StudioManagement.CloseAddDepartmentDialog();
                    window.location.reload(true);
                },
                error: function (params, errors) {
                    StudioManagement.UnlockDepartmentDialog();
                    jq("#studio_departmentAddDialog .infoPanel").html("<div>" + errors + "</div>");
                    jq("#studio_departmentAddDialog .infoPanel").show();
                    //console.log('management.js AddDepartmentCallback update error', errors);
                }
            });
        }
    };

    this.CloseAddDepartmentDialog = function () {
        PopupKeyUpActionProvider.ClearActions();
        jq.unblockUI();
    };

    this.ShowEmployeeSelectorDialog = function () {
        empSelector.OnOkButtonClick = this.SaveEmployee2Department;
        empSelector.ShowDialog();
    };

    this.SaveEmployee2Department = function () {
        var userIDs = new Array();
        jq(empSelector.GetSelectedUsers()).each(function (i, el) {

            userIDs.push(el.ID);
        });

        AjaxPro.onLoading = function (b) {
            if (b) {
                jq.blockUI();
            } else {
                jq.unblockUI();
            }
        };

        EmployeeService.TransferUser2Department(jq('#studio_depEditDepID').val(),
            userIDs, function (result) {
                var res = result.value;
                if (res.rs1 == 1) {
                    window.location.reload(true);
                } else {
                    jq('#studio_depEditAddUserInfo').html("<div class='errorBox'>" + res.rs2 + '</div>');
                    setTimeout("jq('#studio_depEditAddUserInfo').html('');", 5000);
                }
            });
    };

    this.LockDepartmentDialog = function () {
        jq("#depActionBtn").addClass("display-none");
        jq("#dep_action_loader").removeClass("display-none");
    };

    this.UnlockDepartmentDialog = function () {
        jq("#depActionBtn").removeClass("display-none");
        jq("#dep_action_loader").addClass("display-none");
    };

    //----studio settings-----------------------------------
    this.TimeoutHandler = null;

    this.SaveDnsSettings = function () {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_enterDnsBox').block();
            } else {
                jq('#studio_enterDnsBox').unblock();
            }
        };

        StudioSettings.SaveDnsSettings(jq('#studio_dnsName').val(), jq('#studio_TenantAlias').val(), jq('#studio_enableDnsName').is(':checked'),
            function (result) {
                jq('#dnsChange_sent').html(result.value.rs2);
                jq('#dnsChange_sent').show();
            }
        );
    };

    this.SaveLanguageTimeSettings = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_lngTimeSettingsBox').block();
            } else {
                jq('#studio_lngTimeSettingsBox').unblock();
            }
        };
        var timeManager = new TimeAndLanguageContentManager();
        timeManager.SaveTimeLangSettings(function (res) {
            if (res.Status == '2') {
                jq('#studio_lngTimeSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);

            } else if (res.Status == '1') {
                window.location.reload(true);
            } else {
                jq('#studio_lngTimeSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
                StudioManagement.TimeoutHandler = setTimeout("jq('#studio_lngTimeSettingsInfo').html('');", 3000);
            }
        });
    };
};

jq(function () {
    var domainWidth = jq('#studio_TenantBaseDomain').width();
    if (domainWidth > 0) {
        domainWidth += 2;
    }
    if (domainWidth >= 200) {
        domainWidth = 0;
    }
    jq('#studio_TenantAlias').width(jq('#studio_dnsName').width() - domainWidth);
});