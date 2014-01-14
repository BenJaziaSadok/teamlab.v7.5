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

ASC.Projects.MilestoneAction = (function() {
    var isInit, isInitData = false;
    var isAdmin = false;
    var myGuid;
    var loadListProjectsFlag = false;
    var loadTeamFlag = false;
    var currentProjectId;

    // DOM variables
    var milestoneProject;


    var createOptionElement = function (obj) {
        var option = document.createElement('option');
        option.setAttribute('value', obj.value);
        option.appendChild(document.createTextNode(obj.title));
        return option;
    };

    var setProjectCombobox = function () {
        milestoneProject.css('max-width', 300);
        jq("#milestoneResponsible").css('max-width', 300);
        var sortedProjects = ASC.Projects.ProjectsAdvansedFilter.getProjectsForFilter();
        for (var i = 0; i < sortedProjects.length; i++) {
            var project = sortedProjects[i];
            if (project.canCreateMilestone) {
                milestoneProject.append(createOptionElement(project));
            }
        }
        milestoneProject.tlcombobox();
        milestoneProject.prop("disabled", false);
    };

    var setResponsibleCombobox = function(projectParticipants) {
        var milestoneResponsible = jq('#milestoneResponsible');
        var withoutVisitors = ASC.Projects.Common.excludeVisitors(projectParticipants);
        for (var i = 0; i < withoutVisitors.length; i++) {
            var partisipant = projectParticipants[i];
            milestoneResponsible.append(jq('<option value="' + partisipant.id + '"></option>').html(partisipant.displayName));
        }
        milestoneResponsible.tlcombobox();
        milestoneResponsible.prop("disabled", false); // for tablet
        jq("#milestoneResponsibleContainer").show();
    };

    var initData = function() {
        if (isInitData) return;
        isInitData = true;

        jq(document).bind("loadApiData", function () {
            if (loadListProjectsFlag) {
                if (!currentProjectId) {
                    isInitData = true;
                    return;
                }
                if (loadTeamFlag) {
                    isInitData = true;
                }
            }
        });

        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadProjects, function() {
            loadListProjectsFlag = true;
            setProjectCombobox();
            jq(document).trigger("loadApiData");
        });

        if (currentProjectId)
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function() {
                loadListTeamFlag = true;
                setResponsibleCombobox(ASC.Projects.Master.Team);
                jq(document).trigger("loadApiData");
            });
    };

    var init = function() {
        if (isInit === false) {
            isInit = true;
        }

        // init DOM variables
        milestoneProject = jq('#milestoneProject');

        myGuid = Teamlab.profile.id;
        currentProjectId = jq.getURLParam('prjID');

        if (Teamlab.profile.isAdmin) {
            isAdmin = true;
        }

        initData();

        jq("#milestoneDeadlineInputBox").mask(ASC.Resources.Master.DatePatternJQ);

        milestoneProject.on('change', function() {
            if (jq(this).val() > 0) {
                jq('#milestoneProjectContainer').removeClass('requiredFieldError');
                jq('#milestoneResponsibleContainer').show();
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
                getProjectParticipants(jq(this).val(), { responsible: jq(this).attr('responsible') });
            }
            if (milestoneProject.find('option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneProject)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneProject)').removeClass('left-align');
            }
        });

        jq('#milestoneResponsible').on('change', function() {
            if (jq(this).val() != -1) {
                jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
            }
            if (jq(this).val() != -1 && jq(this).val() != myGuid) {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').show();
                jq('#notifyResponsibleCheckbox').attr('checked', true);
            } else {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
            }

            if (jq('#milestoneResponsible option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').removeClass('left-align');
            }
        });

        jq('#milestoneTitleInputBox').keyup(function() {
            if (jq.trim(jq(this).val()) != '') {
                jq('#milestoneTitleContainer').removeClass('requiredFieldError');
            }
        });

        jq('#milestoneActionPanel .deadlineLeft').on('click', function() {
            jq('#milestoneActionPanel .deadlineLeft').css('border-bottom', '1px dotted').css('font-weight', 'normal');
            jq(this).css('border-bottom', 'none').css('font-weight', 'bold');
            var period = parseInt(jq(this).attr('data-value'));
            var date = new Date();
            if (period == 7) {
                date.setDate(date.getDate() + period);
            } else {
                date.setMonth(date.getMonth() + period);
            }
            jq("#milestoneDeadlineInputBox").datepicker('setDate', date);
        });

        jq('#milestoneActionButton').on('click', function() {
            if (jq(this).hasClass("disable")) return;
            jq('#milestoneProjectContainer').removeClass('requiredFieldError');
            jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
            jq('#milestoneDeadlineContainer').removeClass('requiredFieldError');
            jq('#milestoneTitleContainer').removeClass('requiredFieldError');

            var data = {};
            var milestoneId = jq('#milestoneActionPanel').attr('milestoneId');

            if (milestoneProject.val() != "-1") {
                data.projectId = milestoneProject.val();
            } else if (milestoneProject.data("project")) {
                data.projectId = milestoneProject.data("project");
            } else if (currentProjectId) {
                data.projectId = currentProjectId;
            }

            var milestoneResponsible = jq('#milestoneResponsible');
            if (milestoneResponsible.val().length && milestoneResponsible.val() !== '-1') {
                data.responsible = milestoneResponsible.val();
            };

            data.notifyResponsible = jq('#notifyResponsibleCheckbox').is(':checked');

            if (jq('#milestoneDeadlineInputBox').val().length) {
                data.deadline = jq('#milestoneDeadlineInputBox').datepicker('getDate');
                data.deadline.setHours(0);
                data.deadline.setMinutes(0);
                data.deadline = Teamlab.serializeTimestamp(data.deadline);
            }

            data.title = jq.trim(jq('#milestoneTitleInputBox').val());
            data.description = jq('#milestoneDescriptionInputBox').val();

            data.isKey = jq('#milestoneKeyCheckBox').is(':checked');
            data.isNotify = jq('#milestoneNotifyManagerCheckBox').is(':checked');

            var isError = false;
            if (!data.projectId) {
                jq('#milestoneProjectContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.responsible) {
                jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();
                jq('#milestoneResponsibleContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.deadline) {
                jq('#milestoneDeadlineContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.title.length) {
                jq('#milestoneTitleContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (!data.projectId) {
                jq('#milestoneProjectContainer').addClass('requiredFieldError');
                isError = true;
            }
            if (isError) { return false; }

            jq('#milestoneActionButtonsContainer').hide();
            jq('#milestoneActionProcessContainer').show();

            if (jq('#milestoneActionPanel').attr('type') == 'update') {
                updateMilestone(milestoneId, data);
            }
            else {
                addMilestone(data);
            }
            return false;
        });
    };

    var addMilestone = function(milestone) {
        lockMilestoneActionPage();

        var params = { projectId: milestone.projectId };
        Teamlab.addPrjMilestone(params, milestone.projectId, milestone, { success: onAddMilestone, error: onAddMilestoneError });
    };

    var updateMilestone = function(milestoneId, milestone) {
        lockMilestoneActionPage();

        var params = {};
        Teamlab.updatePrjMilestone(params, milestoneId, milestone,
            {
                success: function (params, milestone) {
                    for (var i = 0; i < ASC.Projects.Master.Milestones.length; i++) {
                        if (ASC.Projects.Master.Milestones[i].id == milestone.id) {
                            ASC.Projects.Master.Milestones[i] = milestone;
                            break;
                        }
                    }
                    ASC.Projects.AllMilestones.onUpdateMilestone(params, milestone);
                },
                error: ASC.Projects.AllMilestones.onUpdateMilestoneError
            });
    };

    var lockMilestoneActionPage = function() {
        jq('#milestoneDeadlineInputBox').attr('disabled', true);
        jq('#milestoneTitleInputBox').attr('disabled', true);
        jq('#milestoneDescriptionInputBox').attr('disabled', true);
        jq('#milestoneKeyCheckBox').attr('disabled', true);
        jq('#milestoneNotifyManagerCheckBox').attr('disabled', true);
    };

    var unlockMilestoneActionPage = function() {
        jq('#milestoneDeadlineInputBox').removeAttr('disabled');
        jq('#milestoneTitleInputBox').removeAttr('disabled').val('');
        jq('#milestoneDescriptionInputBox').removeAttr('disabled').val('');
        jq('#milestoneKeyCheckBox').removeAttr('disabled').removeAttr("checked");
        jq('#milestoneNotifyManagerCheckBox').removeAttr('disabled');
        jq('#milestoneActionProcessContainer').hide();
        jq('#milestoneActionButtonsContainer').show();
    };

    var clearPanel = function() {
        jq('#milestoneActionPanel').removeAttr('type');

        milestoneProject.tlcombobox();
        jq('#milestoneResponsible').tlcombobox();

        jq('#notifyResponsibleCheckbox').attr('checked', true);
        jq('#milestoneResponsibleContainer').hide();
        jq('#milestoneResponsibleContainer .notifyResponsibleContainer').hide();

        jq('#milestoneProjectContainer').removeClass('requiredFieldError');
        milestoneProject.val('-1').change();

        jq('#milestoneDeadlineContainer').removeClass('requiredFieldError');

        jq('#milestoneActionPanel .deadlineLeft').css('border-bottom', '1px dotted').css('font-weight', 'normal');

        var milestoneDeadline = jq('#milestoneDeadlineInputBox');
        milestoneDeadline.datepicker({ popupContainer: '#milestoneActionPanel', selectDefaultDate: true });
        jq(milestoneDeadline).on("keydown", function (e) { if (e.keyCode == 13) { milestoneDeadline.blur(); } });
        jq(milestoneDeadline).on("change", function () { milestoneDeadline.blur(); });
        
        var date = new Date();
        date.setDate(date.getDate() + 7);
        milestoneDeadline.datepicker('setDate', date);
        var elemDuration3Days = jq(milestoneDeadline).siblings(".dottedLink[data-value=7]");
        jq(elemDuration3Days).css("border-bottom", "medium none");
        jq(elemDuration3Days).css("font-weight", "bold");


        jq('#milestoneResponsibleContainer').removeClass('requiredFieldError');
        jq('#milestoneResponsible').val('-1').change();

        jq('#milestoneTitleContainer').removeClass('requiredFieldError');
        jq('#milestoneTitleInputBox').val('');

        jq('#milestoneDescriptionInputBox').val('');

        jq('#milestoneKeyCheckBox').removeAttr('checked');

        jq('#milestoneNotifyManagerCheckBox').removeAttr('checked');

        jq('#milestoneActionButtonsContainer').show();
        jq('#milestoneActionProcessContainer').hide();
    };

    var onAddMilestone = function(params, milestone) {
        unlockMilestoneActionPage();
        if (currentProjectId == milestone.projectId) {
            ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.milestones, "add");
            ASC.Projects.Master.Milestones.push(milestone);
        }
        if (location.href.indexOf("milestones.aspx") > 0 && (currentProjectId == null || milestone.projectId == currentProjectId)) {
            ASC.Projects.AllMilestones.onAddMilestone(params, milestone);
        } else {
            jq.unblockUI();
        }
        ASC.Projects.TaskAction.onAddNewMileston(milestone);
    };

    var onAddMilestoneError = function(params, error) {
        var errorBox = jq("#milestoneActionPanel .errorBox");
        var actionContainer = jq("#milestoneActionButtonsContainer");
        errorBox.text(error[0]);
        errorBox.removeClass("display-none");
        jq("#milestoneActionButton").addClass("disable");
        jq("#milestoneActionProcessContainer").hide();
        actionContainer.css('marginTop', '8px');
        actionContainer.show();

        var projectsList = jq("#milestoneProject");
        projectsList.find("option[value='" + params.projectId + "']").remove();
        projectsList.tlcombobox();
        if (projectsList.find("option").length == 1) {
            jq("#createNewMilestone").remove();
            projectsList.tlcombobox(false);
            jq("#milestoneResponsible").children('option[value!=0][value!=-1]').remove();
            jq("#milestoneResponsible").tlcombobox();
            jq("#milestoneResponsible").tlcombobox(false);
        }

        setTimeout(function() {
            errorBox.addClass("display-none");
            actionContainer.css('marginTop', '43px');

            jq('#milestoneDeadlineInputBox').removeAttr('disabled');
            jq('#milestoneTitleInputBox').removeAttr('disabled');
            jq('#milestoneDescriptionInputBox').removeAttr('disabled');
            jq('#milestoneKeyCheckBox').removeAttr('disabled');
            jq('#milestoneNotifyManagerCheckBox').removeAttr('disabled');
        }, 3000);

        if (location.href.indexOf("milestones.aspx") > 0 && (currentProjectId == params.projectId)) {
            ASC.Projects.AllMilestones.removeMilestonesActionsForManager();
        }
        for (var i = 0; i < ASC.Projects.Master.Projects.length; i++) {
            if (params.projectId == ASC.Projects.Master.Projects[i].id) {
                ASC.Projects.Master.Projects[i].canCreateMilestone = false;
                break;
            }
        }
    };

    var onGetProjectParticipants = function(params, participants) {
        var milestoneResponsible = jq('#milestoneResponsible');
        if (!params.serverData) {
            milestoneResponsible.children('option[value!=0][value!=-1]').remove();
            participants = ASC.Projects.Common.excludeVisitors(participants);
            participants = ASC.Projects.Common.removeBlockedUsersFromTeam(participants);
            for (var i = 0; i < participants.length; i++) {
                var p = participants[i];
                if (!p.isVisitor)
                    milestoneResponsible.append(jq('<option value="' + p.id + '"></option>').html(p.displayName));
            }
        }
        milestoneResponsible.tlcombobox();
        if (params.responsible) {
            milestoneResponsible.val(params.responsible).change();
        } else {
            if (!participants.length) {
                jq("#noActiveParticipantsMilNote").removeClass("display-none");
                milestoneResponsible.tlcombobox(false);
                jq("#milestoneActionButton").addClass("disable");
            } else {
                jq("#noActiveParticipantsMilNote").addClass("display-none");
                jq("#milestoneActionButton").removeClass("disable");
            }
            if (jq('#milestoneResponsible option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#milestoneResponsible)').removeClass('left-align');
            }
        }
        milestoneResponsible.prop("disabled", false);
        jq("#milestoneResponsibleContainer").show();
        showMilestoneActionPanel();
        milestoneProject.removeAttr('responsible');
    };

    var onGetMilestoneBeforeUpdate = function(milestone) {
        clearPanel();

        jq('#milestoneActionPanel').attr('type', 'update');
        jq('#milestoneActionPanel').attr('milestoneId', milestone.id);

        var milestoneActionButton = jq('#milestoneActionButton');
        milestoneActionButton.html(milestoneActionButton.attr('update'));

        var milestoneActionHeader = jq('#milestoneActionPanel .containerHeaderBlock table td:first');
        milestoneActionHeader.html(ASC.Projects.Resources.ProjectsJSResource.EditMilestone);

        if (milestone.deadline) {
            jq('#milestoneDeadlineInputBox').datepicker("setDate", milestone.deadline);
            var elemDurationDays = jq("#milestoneDeadlineInputBox").siblings(".dottedLink");
            jq(elemDurationDays).css("border-bottom", "1px dotted");
            jq(elemDurationDays).css("font-weight", "normal");
        }

        jq('#milestoneTitleInputBox').val(milestone.title);
        jq('#milestoneDescriptionInputBox').val(milestone.description);

        jq('#milestoneProjectContainer').hide();

        if (milestone.isKey == 'true') {
            jq('#milestoneKeyCheckBox').prop("checked", true);
        }

        if (milestone.isNotify == 'true') {
            jq('#milestoneNotifyManagerCheckBox').prop("checked", true);
        }

        if (milestone.responsible) {
            milestoneProject.attr('responsible', milestone.responsible);
        }

        milestoneProject.data("project", milestone.project);
        if (milestoneProject.val() == milestone.project && milestoneProject.val() == currentProjectId) {
            onGetProjectParticipants({ serverData: true, responsible: milestone.responsible });
        } else {
            if (projectInSelect(milestone.project)) {
                milestoneProject.val(milestone.project).change();
            } else {
                getProjectParticipants(milestone.project, { responsible: milestone.responsible });
            }
        }
    };

    var getProjectParticipants = function(projectId, params) {
        Teamlab.getPrjProjectTeamPersons(params, projectId, { before: function() { jq("#milestoneResponsible").tlcombobox(false); }, success: onGetProjectParticipants
        });
    };

    var projectInSelect = function (projectId) {
        if(milestoneProject.find('option[value=' + projectId + ']').length) {
            return true;
        }else{
            return false;
        }
    };

    var showNewMilestonePopup = function() {
        initData();
        ASC.Projects.MilestoneAction.clearPanel();
        var milestoneActionButton = jq('#milestoneActionButton');
        milestoneActionButton.html(milestoneActionButton.attr('add'));

        var milestoneActionHeader = jq('#milestoneActionPanel .containerHeaderBlock table td:first');
        milestoneActionHeader.html(ASC.Projects.Resources.ProjectsJSResource.AddMilestone);

        jq('#milestoneProjectContainer').show();

        if (currentProjectId && projectInSelect(currentProjectId)) {
            milestoneProject.val(currentProjectId)
        }
        else {
            var hashProjectId = parseInt(jq.getAnchorParam('project', ASC.Controls.AnchorController.getAnchor()));
            if (hashProjectId && projectInSelect(hashProjectId)) {
                milestoneProject.val(hashProjectId)
            }
        }
        milestoneProject.change();
        showMilestoneActionPanel();
        return false;
    };

    var showMilestoneActionPanel = function () {
        StudioBlockUIManager.blockUI(jq("#milestoneActionPanel"), 550, 550, 0, "absolute");
        jq('#milestoneTitleInputBox').focus();
    };

    return {
        init: init,
        showNewMilestonePopup: showNewMilestonePopup,
        updateMilestone: updateMilestone,
        onGetProjectParticipants: onGetProjectParticipants,
        onGetMilestoneBeforeUpdate: onGetMilestoneBeforeUpdate,
        clearPanel: clearPanel,
        unlockMilestoneActionPage: unlockMilestoneActionPage
    };
})(jQuery);

jq(document).ready(function() {
    jq('textarea').autosize();
});
