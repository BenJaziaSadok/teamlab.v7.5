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

BackupManager = new function () {

    this.timer;
    this.backupID;
    this.already = false;

    this.url = "";
    this.backupError = "";
    
    this.init = function (url, backupError)
    {
        BackupManager.url = url;
        BackupManager.backupError = backupError;
        BackupManager.already = false;
    };

    this.CreateBackup = function () {
        if (BackupManager.already)
            return;

        BackupManager.already = true;

        var url = BackupManager.url + "/services/backup/service.svc/add?_=" + Math.floor(Math.random() * 1000000);
        jq("#backup_error").hide();
        this.InitProcess();

        jq("#create_btn").attr("class", "button blue disable");
        jq("#create_btn").blur();

        jq("#progressbar_container").show();


        jq.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            success: function (result) {
                BackupManager.backupID = result.id;
                BackupManager.timer = setInterval("BackupManager.CheckBackupProcess()", 10000);
            }
        });

    };

    this.InitProcess = function () {
        jq(".asc-progress-value").animate({"width": "1%"});
        jq("#backup_percent").text("1% ");
    };

    this.SetBackupProcess = function (percent) {
        jq(".asc-progress-value").animate({"width": percent + "%"});
        jq("#backup_percent").text(percent + "% ");
    };

    this.CheckBackupProcess = function () {
        var url = BackupManager.url + "/services/backup/service.svc/list/" + BackupManager.backupID + "?_=" + Math.floor(Math.random() * 1000000);

        jq.ajax({
            type: "GET",
            url: url,
            dataType: "json",
            success: function (result) {
                if (result.completed == true) {
                    clearInterval(BackupManager.timer);
                    jq("#backup_link").html("<a href='" + result.link + "' target='_blank'>" + result.link + "</a>");
                    jq("#progressbar_container").hide();
                    jq("#backup_ready").show();
                } else if (result.status == 5) {
                    clearInterval(BackupManager.timer);
                    jq("#backup_error").text(BackupManager.backupError);
                    jq("#backup_error").show();
                }

                BackupManager.SetBackupProcess(result.percentdone);
            }
        });
    };

    this.Deactivate = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq.blockUI();
            } else {
                jq.unblockUI();
            }
        };
        Backup.Deactivate(true, BackupManager.callBackDeactivate);
    };

    this.Delete = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq.blockUI();
            } else {
                jq.unblockUI();
            }
        };
        Backup.Delete(true, BackupManager.callBackDelete);
    };

    this.callBackDeactivate = function (result) {
        if (result != null) {
            jq("#deativate_sent").html(result.value);
            jq("#deativate_sent").show();

        }
    };

    this.callBackDelete = function (result) {
        if (result != null) {
            jq("#delete_sent").html(result.value);
            jq("#delete_sent").show();
        }
    };

};
