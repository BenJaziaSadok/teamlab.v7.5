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

ASC.Projects.TaskAction = (function() {
    var isInit;
    var updateTaskFlag = false;
    var firstLoadFlag = true;
    var currentTask = null;

    var taskPopup = null;
    /*---task fields----*/

    //text
    var taskTitle = null;
    var taskDescription = null;

    //selectors
    var taskProjectSelector = null;
    var taskMilestoneSelector = null;
    var taskResponsiblesSelector = null;

    //dates
    var taskStartDate = null;
    var taskDeadlineDate = null;

    //other
    var notifyCheckbox = null;
    var priorityCheckbox = null;
    var listTaskResponsibles = null;
    /*---*/

    var currentPage = null;
    var currentUserId;
    var currentProjectId;
    var choosedProjectMilestones = [];
    var choosedMilestone = null;
    var projectsHash = {};

    var createOptionElement = function(obj) {
        var option = document.createElement('option');
        option.setAttribute('value', obj.value);
        option.appendChild(document.createTextNode(obj.title));
        return option;
    };
    var extendSelect = function(select, options) {
        for (var i = 0; i < options.length; i++) {
            select.append(createOptionElement(options[i]));
        }
        return select;
    };
    var onGetProjects = function() {
        var sortedProjects = ASC.Projects.ProjectsAdvansedFilter.getProjectsForFilter();
        for (var i = 0; i < sortedProjects.length; i++) {
            if (sortedProjects[i].canCreateTask) {
                projectsHash[sortedProjects[i].value] = sortedProjects[i];
                taskProjectSelector.append(createOptionElement(sortedProjects[i]));
            }
        }
        if (currentProjectId && projectsHash[currentProjectId]) {
            taskProjectSelector.val(currentProjectId);
        }
        taskProjectSelector.tlcombobox();
    };

    var initData = function() {
        ASC.Projects.Common.bind(ASC.Projects.Common.events.loadProjects, function() {
            onGetProjects();
        });

        if (currentProjectId) {
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadTeam, function() {
                onGetTeam({}, ASC.Projects.Master.Team);
            });
            ASC.Projects.Common.bind(ASC.Projects.Common.events.loadMilestones, function() {
                onGetMilestones({}, ASC.Projects.Master.Milestones);
            });
        }
    };

    var getCurrentPage = function() {
        var url = document.location.href;
        if (url.indexOf("tasks.aspx") > 0) {
            currentPage = "tasks";
            var projId = jq.getURLParam("prjID");
            var id = jq.getURLParam("id");
            if (projId && id)
                currentPage = "taskdescription";
        }

        if (url.indexOf("milestones.aspx") > 0)
            currentPage = "milestones";
    };

    var initTaskFormElementsAndConstants = function() {

        currentUserId = Teamlab.profile.id;
        currentProjectId = jq.getURLParam('prjID');

        taskPopup = jq('#addTaskPanel');

        //text
        taskTitle = jq("#addtask_title");
        taskDescription = jq("#addtask_description");

        //selectors
        taskProjectSelector = jq("#taskProject");
        taskMilestoneSelector = jq("#taskMilestone");
        taskResponsiblesSelector = jq("#taskResponsible");

        //dates
        taskStartDate = jq("#taskStartDate");
        taskDeadlineDate = jq("#taskDeadline");

        //others
        notifyCheckbox = jq("#notify");
        priorityCheckbox = jq("#priority");
        listTaskResponsibles = jq("#fullFormUserList");

        taskDescription.autosize();
        jq(taskProjectSelector, taskMilestoneSelector, taskResponsiblesSelector).css('max-width', 300);

        taskStartDate.mask(ASC.Resources.Master.DatePatternJQ);
        taskDeadlineDate.mask(ASC.Resources.Master.DatePatternJQ);
        taskDeadlineDate.datepicker({ selectDefaultDate: false });
        taskStartDate.datepicker({ selectDefaultDate: false });

        jq(taskDeadlineDate).on("keydown", function (e) { if (e.keyCode == 13) {taskDeadlineDate.blur();}});
        jq(taskDeadlineDate).on("change", function () { taskDeadlineDate.blur(); });
        
        jq(taskStartDate).on("keydown", function (e) { if (e.keyCode == 13) { taskStartDate.blur();}});
        jq(taskStartDate).on("change", function (e) { taskStartDate.blur();});

        getCurrentPage();
        initData();
    };

    var init = function() {
        if (isInit) {
            return;
        }

        isInit = true;

        initTaskFormElementsAndConstants();

        // events handlers
        taskProjectSelector.on('change', function() {
            var selectedPrjId = parseInt(jq(this).val(), 10);
            if (selectedPrjId > 0) {
                jq('.popupActionPanel').hide();
                taskPopup.find('.requiredErrorText.project').hide();
                jq('#pm-milestoneBlock').show();
                jq('.pm-headerLeft.userAddHeader').show();
                jq('.pm-fieldRight.userAdd').show();

                listTaskResponsibles.html('').hide();

                jq(taskMilestoneSelector, taskResponsiblesSelector).tlcombobox(false);

                if (selectedPrjId == currentProjectId) {
                    onGetMilestones({}, ASC.Projects.Master.Milestones);
                    onGetTeam({}, ASC.Projects.Master.Team);
                } else {
                    getMilestones({}, selectedPrjId);
                    getTeam({}, selectedPrjId);
                }
            }
            if (taskProjectSelector.find("option:selected").text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskProject)').removeClass('left-align');
            }
        });

        taskMilestoneSelector.on('change', function() {
            if (jq('#taskMilestone option:selected').text().length < 15) {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskMilestone)').addClass('left-align');
            } else {
                jq('span.tl-combobox.tl-combobox-container:has(select#taskMilestone)').removeClass('left-align');
            }
            var milestoneId = jq(this).val();
            if (milestoneId == "0") {
                choosedMilestone = null;
            } else {
                var milestone = getMilestoneById(parseInt(milestoneId), 10);
                choosedMilestone = milestone;
            }
        });

        taskResponsiblesSelector.on('change', function(evt) {
            var value = jq(this).val();
            if (value == -1) {
                jq(this).val('-1');
                return;
            }
            var userName = jq(this).find('option[value="' + value + '"]').html();
            if (!listTaskResponsibles.find("div[data-value='" + value + "']").length) {
                listTaskResponsibles.show().append('<div data-value="' + value + '" class="user">' + userName + '</div>');
                jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').hide();
                var addedUserCount = listTaskResponsibles.find(".user").length;
                var usersCount = taskResponsiblesSelector.find("option").length - 1;
                if (addedUserCount == usersCount) {
                    taskResponsiblesSelector.tlcombobox(false);
                }
            }
            jq(this).val('-1').change();
            showOrHideNotifyCheckbox();
            evt.stopPropagation();
        });

        taskPopup.on('click', '#fullFormUserList .user', function() {
            value = jq(this).attr('data-value');
            jq(this).remove();
            taskResponsiblesSelector.tlcombobox(true);
            jq('.userAdd .combobox-container li.option-item[data-value="' + value + '"]').show();
            if (!listTaskResponsibles.find(".user").length) {
                listTaskResponsibles.hide();
            }
            showOrHideNotifyCheckbox();
        });

        taskPopup.on('click', '.deadline_left', function() {
            taskPopup.find('.deadline_left').css('border-bottom', '1px dotted').css('font-weight', 'normal');
            jq(this).css('border-bottom', 'none').css('font-weight', 'bold');
            var daysCount = parseInt(jq(this).attr('data-value'), 10);
            var date = new Date();
            date.setDate(date.getDate() + daysCount);
            taskDeadlineDate.datepicker('setDate', date);
        });

        jq('#saveTaskAction, #createTaskAndCreateNew').on('click', function() {
            clearErrorMessages();
            var data = {};
            data.title = jq.trim(taskTitle.val());
            data.description = taskDescription.val();
            var responsibles = listTaskResponsibles.find(".user");
            if (responsibles.length) {
                data.responsibles = [];
                responsibles.each(function() {
                    data.responsibles.push(jq(this).attr('data-value'));
                });
            }

            if (notifyCheckbox.is(':checked')) {
                data.notify = true;
            }

            data.milestoneid = taskMilestoneSelector.val();
            var oldMilestone = taskMilestoneSelector.data("milestone");
            if (!data.milestoneid || data.milestoneid == "-1") {
                data.milestoneid = 0;
                if (updateTaskFlag && oldMilestone != "0") {
                    data.milestoneid = oldMilestone;
                }
            }
            data.priority = priorityCheckbox.is(':checked') ? 1 : 0;

            var isError = false;
            if (!data.title.length) {
                taskPopup.find('.titlePanel').addClass('requiredFieldError');
                taskPopup.find('.requiredErrorText.title').html(taskPopup.find('.requiredErrorText').attr('error'));
                isError = true;
            }
            data.projectId = taskProjectSelector.val();
            if (!data.projectId || data.projectId == "-1") {
                if (!updateTaskFlag) {
                    taskPopup.find('.requiredErrorText.project').show().html(taskPopup.find('.requiredErrorText.project').attr('error'));
                    isError = true;
                } else {
                    data.projectId = taskProjectSelector.data("project");
                }
            } 

            if (compareTaskDatesAndShowError()) {
                isError = true;
            } else {
                if (taskDeadlineDate.val().length) {
                    data.deadline = taskDeadlineDate.datepicker('getDate');
                    data.deadline.setHours(0);
                    data.deadline.setMinutes(0);
                    data.deadline = Teamlab.serializeTimestamp(data.deadline);
                }

                if (taskStartDate.val().length) {
                    data.startDate = taskStartDate.datepicker('getDate');
                    data.startDate.setHours(0);
                    data.startDate.setMinutes(0);
                    data.startDate = Teamlab.serializeTimestamp(data.startDate);
                }
            }

            if (isError) {
                return;
            }

            lockTaskForm();

            var params = { saveAndView: false };
            if (jq(this).attr("id") == "saveTaskAction") {
                params.saveAndView = true;
            }
            if (updateTaskFlag) {
                if (currentTask.links && currentTask.milestoneId != parseInt(data.milestoneid, 10)) {
                    MoveTaskQuestionPopup.setParametrs("#addTaskPanel", currentTask.links,
                                                   function () {
                                                       updateTask({}, currentTask.id, data);
                                                   },
                                                   showTaskFormAfterQuestionPopup);
                    MoveTaskQuestionPopup.showDialog();
                } else {
                    updateTask({}, currentTask.id, data);
                }
            } else {
                var project = jq('#taskProject').val();
                addTask(params, project, data);
            }
        });

        jq('#addTaskPanel #closeTaskAction').on('click', function() {
            closeTaskForm();
        });
    };

    var compareTaskDatesAndShowError = function() {
        if (taskStartDate.val().trim() == "" && taskDeadlineDate.val().trim() == "") return false;

        var startDate = taskStartDate.datepicker('getDate');
        var deadlineDate = taskDeadlineDate.datepicker('getDate');
        var milestoneDeadline = choosedMilestone ? choosedMilestone.deadline : null;

        var errorFlag = false;

        if ((startDate && deadlineDate) && (startDate > deadlineDate)) {
            var errorStartDate = taskPopup.find(".startDate-error");
            errorStartDate.text(errorStartDate.attr("error"));
            errorStartDate.show();
            taskStartDate.addClass("red-border");
            errorFlag = true;
        }

        return errorFlag;
    };

    var clearErrorMessages = function() {
        taskPopup.find('.titlePanel').removeClass('requiredFieldError');
        taskPopup.find('.requiredErrorText').html('');
        taskPopup.find('.requiredErrorText.project').hide();
        taskPopup.find(".startDate-error").hide();
        taskStartDate.removeClass("red-border");
        taskDeadlineDate.removeClass("red-border");
    };

    var showOrHideNotifyCheckbox = function() {
        var usersCount = listTaskResponsibles.find(".user").length;
        if (!usersCount || usersCount == listTaskResponsibles.find(".user[data-value=" + currentUserId + "]").length) {
            taskPopup.find('.notify').hide();
            return;
        }
        taskPopup.find('.notify').show();
    };

    var lockTaskForm = function() {
        taskTitle.attr("disabled", "disabled");
        taskDescription.attr("disabled", "disabled");
        taskStartDate.attr("disabled", "disabled");
        taskDeadlineDate.attr("disabled", "disabled");
        notifyCheckbox.attr("disabled", "disabled");
        priorityCheckbox.attr("disabled", "disabled");

        taskProjectSelector.tlcombobox(false);
        taskMilestoneSelector.tlcombobox(false);
        taskResponsiblesSelector.tlcombobox(false);

        taskPopup.find('.pm-action-block').addClass('display-none');
        taskPopup.find('.pm-ajax-info-block').removeClass('display-none');
    };

    var unlockTaskForm = function() {
        taskTitle.removeAttr("disabled");
        taskDescription.removeAttr("disabled");
        taskStartDate.removeAttr("disabled");
        taskDeadlineDate.removeAttr("disabled");
        notifyCheckbox.removeAttr("disabled");
        priorityCheckbox.removeAttr("disabled");

        taskProjectSelector.tlcombobox();
        taskMilestoneSelector.tlcombobox();
        taskResponsiblesSelector.tlcombobox();

        taskPopup.find('.pm-action-block').removeClass('display-none');
        taskPopup.find('.pm-ajax-info-block').addClass('display-none');
    };

    var addTask = function(params, projectId, data) {
        Teamlab.addPrjTask(params, projectId, data, { success: onAddTask, error: onTaskError });
    };

    var updateTask = function(params, taskId, data) {
        var success;
        switch (currentPage) {
            case "tasks":
                success = ASC.Projects.TasksManager.onUpdateTask;
                break;
            case "taskdescription":
                success = ASC.Projects.TaskDescroptionPage.onUpdateTask;
                break;
            default:
                success = null;
        }
        Teamlab.updatePrjTask(params, taskId, data, { success: success, error: onTaskError });
    };

    var onAddTask = function(params, task) {
        if (currentPage == "tasks" && (currentProjectId == null || task.projectOwner.id == currentProjectId)) {
            ASC.Projects.TasksManager.onAddTask(params, task);
        }

        if (currentPage == "milestones" && (currentProjectId == null || task.projectOwner.id == currentProjectId)) {
            ASC.Projects.AllMilestones.onAddTask(params, task);
        }

        if (currentProjectId == task.projectId) {
            ASC.Projects.projectNavPanel.changeModuleItemsCount(ASC.Projects.projectNavPanel.projectModulesNames.tasks, "add");
        }

        if (params.saveAndView) {
            closeTaskForm();
        }
        else {
            taskPopup.find('#taskLink').attr("href", "tasks.aspx?prjID=" + task.projectId + "&id=" + task.id);
            unlockTaskForm();
            renderTaskForm(getEmptyTask());
            jq(".pm-action-block .okBox").removeClass('display-none');
            jq(".pm-action-block").css('marginTop', '0');
            taskTitle.focus();
        }
    };

    var onTaskError = function(params, error) {
        var actionContainer = jq("#addTaskPanel .pm-action-block");
        var taskErrorBox = jq("#addTaskPanel .errorBox");

        taskPopup.find(".pm-ajax-info-block").addClass('display-none');
        taskErrorBox.text(error[0]);
        taskErrorBox.removeClass("display-none");
        actionContainer.css('marginTop', '8px');
        actionContainer.show();
        setTimeout(function() {
            taskErrorBox.addClass("display-none");
            actionContainer.css('marginTop', '43px');
        }, 3000);
        unlockTaskForm();
    };

    var getMilestones = function(params, projectId) {
        Teamlab.getPrjMilestones(params, null, { filter: { status: 'open', projectId: projectId }, success: onGetMilestones });
    };

    var onGetMilestones = function(params, milestones) {
        var options = [];
        choosedProjectMilestones = milestones;
        var milestonesInd = milestones ? milestones.length : 0;
        taskMilestoneSelector.find('option[value!=0][value!=-1]').remove();

        if (milestonesInd > 0) {
            while (milestonesInd--) {
                options.unshift({ value: milestones[milestonesInd].id, title: '[' + milestones[milestonesInd].displayDateDeadline + '] ' + milestones[milestonesInd].title });
            }
            extendSelect(taskMilestoneSelector, options);
        }
        taskMilestoneSelector.tlcombobox();
        taskMilestoneSelector.prop("disabled", false);

        if (updateTaskFlag && currentTask) {
            if (currentTask.milestoneId && taskMilestoneSelector.find("option[value='" + currentTask.milestoneId + "']").length) {
                taskMilestoneSelector.val(currentTask.milestoneId).change();
            } else {
                taskMilestoneSelector.val(0).change();
            }
        }
    };

    var getTeam = function(params, projectId) {
        Teamlab.getPrjTeam(params, projectId, { success: onGetTeam });
    };

    var onGetTeam = function(params, team) {
        var teamWithoutVisitors = ASC.Projects.Common.excludeVisitors(team);
        teamWithoutVisitors = ASC.Projects.Common.removeBlockedUsersFromTeam(teamWithoutVisitors);
        var teamInd = teamWithoutVisitors ? teamWithoutVisitors.length : 0;

        jq('#taskResponsible option[value!=0][value!=-1]').remove();

        if (teamInd != 0) {
            jq("#noActiveParticipantsTaskNote").addClass("display-none");

            for (var i = 0; i < teamInd; i++) {
                var user = teamWithoutVisitors[i];
                taskResponsiblesSelector.append(jq('<option value="' + user.id + '"></option>').html(user.displayName));
            }
            taskResponsiblesSelector.tlcombobox();
            listTaskResponsibles.empty();
            setResponsibles();
        } else {
            jq("#noActiveParticipantsTaskNote").removeClass("display-none");
            taskResponsiblesSelector.tlcombobox();
            taskResponsiblesSelector.tlcombobox(false);
        }
    };

    var setResponsibles = function() {
        if (currentTask && currentTask.responsibles.length) {
            listTaskResponsibles.empty();
            jQuery.each(currentTask.responsibles, function() {
                var elem = jq('.userAdd .combobox-container li.option-item[data-value="' + this.id + '"]');
                if (elem.length) {
                    var name = elem.text();
                    listTaskResponsibles.append('<div data-value="' + this.id + '" class="user">' + Encoder.htmlEncode(name) + '</div>');
                    elem.hide();
                }
            });
            var users = listTaskResponsibles.find(".user");
            if (jq('.userAdd .combobox-container li.option-item').length - 1 == users.length) {
                taskResponsiblesSelector.tlcombobox(false);
            }
            if (users.length) {
                listTaskResponsibles.show();
            }
        }
        if (currentTask && !currentTask.responsibles.length && updateTaskFlag){
            listTaskResponsibles.empty();
        }
        showOrHideNotifyCheckbox();
    };

    var getMilestoneById = function(id) {
        var milestonesInd = choosedProjectMilestones ? choosedProjectMilestones.length : 0;
        while (milestonesInd--) {
            if (id == choosedProjectMilestones[milestonesInd].id) return choosedProjectMilestones[milestonesInd];
        }
    };

    var getEmptyTask = function() {
        var task = {};
        task.title = "";
        task.description = "";
        if (currentProjectId) {
            task.projectId = currentProjectId;
        }
        task.responsibles = [];
        task.priority = null;
        task.startDate = null;
        task.deadline = null;
        task.milestoneid = null;

        return task;
    };

    var isNeedChangeProject = function(projectId) {
        if (projectId && (firstLoadFlag || taskProjectSelector.val() != projectId.toString())) return true;
        return false;
    };

    var renderTaskForm = function(task) {
        clearErrorMessages();
        taskPopup.find('.okBox, .errorBox').addClass('display-none');
        taskPopup.find('.pm-action-block').css('marginTop', '43px');
        // task body
        taskTitle.val(task.title);
        taskDescription.val(task.description);

        if (isNeedChangeProject(task.projectId)) {
            if (projectsHash[task.projectId]) {
                taskProjectSelector.val(task.projectId).change();
            } else {
                if (!taskProjectSelector.find("option[value='" + task.projectId + "']").length) {
                    jq("#pm-milestoneBlock").hide();
                    getMilestones({}, task.projectId);
                    getTeam({}, task.projectId);
                }
            }
        } else {
            if(currentProjectId)  onGetMilestones({}, ASC.Projects.Master.Milestones);
            if (task.milestoneId) {
                taskMilestoneSelector.val(task.milestoneId).change();
            }else{
                if(updateTaskFlag) taskMilestoneSelector.val(0).change();
            }
            setResponsibles();
        }
        if (taskProjectSelector.val() == "-1" && !updateTaskFlag) {
            jq("#pm-milestoneBlock").hide();
            jq(".userAddHeader, .userAdd").hide();
        }

        if (task.deadline) {
            taskDeadlineDate.datepicker('setDate', task.deadline);
            var elemDurationDays = taskDeadlineDate.siblings('.dottedLink');
            elemDurationDays.css('border-bottom', '1px dotted');
            elemDurationDays.css('font-weight', 'normal');
        }else{
            if(updateTaskFlag) taskDeadlineDate.datepicker('setDate', null);
        }

        if (task.startDate) {
            taskStartDate.datepicker('setDate', task.startDate);
        }else{
            if(updateTaskFlag) taskStartDate.datepicker('setDate', null);
        }

        if (task.priority) {
            priorityCheckbox.prop("checked", true);
        } else {
            priorityCheckbox.prop("checked", false);
        }

        if (updateTaskFlag) {
            setDescriptionHeight();
            jq("#pm-projectBlock").hide();
        } else {
            taskDescription.height(60);
            jq('#pm-projectBlock').show();
        }

        //buttons and title
        var saveButton = taskPopup.find('#saveTaskAction');
        var createNewButton = jq("#createTaskAndCreateNew");

        if (updateTaskFlag) {
            createNewButton.hide();
            createNewButton.siblings(".splitter-buttons").first().hide();
            saveButton.html(saveButton.attr('update'));
            taskPopup.find('.containerHeaderBlock table td:first').html(ASC.Projects.Resources.ProjectsJSResource.EditThisTask);
        } else {
            createNewButton.show();
            createNewButton.siblings(".splitter-buttons").show();

            saveButton.html(saveButton.attr('add'));
            taskPopup.find('.containerHeaderBlock table td:first').html(ASC.Projects.Resources.ProjectsJSResource.CreateNewTask);
        }

        if (firstLoadFlag) {
            firstLoadFlag = false;
        }
        LoadingBanner.hideLoading();
    };

    var showTaskForm = function(task) {
        currentTask = task;
        unlockTaskForm();
        renderTaskForm(task);
        StudioBlockUIManager.blockUI(jq("#addTaskPanel"), 550, 550, 0, "absolute");
    };

    var showTaskFormAfterQuestionPopup = function () {
        unlockTaskForm();
        StudioBlockUIManager.blockUI(jq("#addTaskPanel"), 550, 550, 0, "absolute");
    };

    var closeTaskForm = function() {
        jq.unblockUI();
    };

    var setDescriptionHeight = function() {
        var description = jq("#addtask_description");

        //default
        description.height(60);
        description.css("overflowY", "auto");

        var colsCount = parseInt(description.attr("cols"), 10);
        var text = description.val();
        var countStr = text.split("\n").length;
        if (countStr > 4) {
            if (countStr > 26) {
                description.height(400);
                description.css("overflowY", "scroll");
                return;
            }
            description.height(countStr * 15);
        }
        var signsCount = text.length;
        if (signsCount > colsCount * 6) {
            var rows = Math.floor(signsCount / (colsCount * 2.5));
            if (rows > 27) {
                description.height(400);
                description.css("overflowY", "scroll");
            } else {
                if (rows > countStr) {
                    description.height(rows * 15);
                }
            }
        }
    };

    var showCreateNewTaskForm = function(taskParams) {
        updateTaskFlag = false;

        var task = getEmptyTask();
        if (taskParams) {
            if (taskParams.projectId) {
                task.projectId = taskParams.projectId;
            }
            if (taskParams.milestoneId) {
                task.milestoneId = taskParams.milestoneId;
            }
            if (taskParams.responsibles) {
                task.responsibles = taskParams.responsibles;
            }
        }

        showTaskForm(task);
    };

    var showUpdateTaskForm = function(taskId, task) {
        LoadingBanner.displayLoading();
        updateTaskFlag = true;
        if (task) {
            showTaskForm(task);
            setUpdatedTaskAttr(task);
        } else {
            Teamlab.getPrjTask({}, taskId, function(params, targetTask) {
                showTaskForm(targetTask);
                setUpdatedTaskAttr(targetTask);
            });
        }
    };

    var setUpdatedTaskAttr = function (task) {
         taskProjectSelector.data("project", task.projectId);
         taskMilestoneSelector.data("milestone", task.milestoneId);
    };

    var onAddNewMileston = function(milestone) {
        if (taskProjectSelector.val() == milestone.projectId) {
            taskMilestoneSelector.append(createOptionElement({ value: milestone.id, title: '[' + milestone.displayDateDeadline + '] ' + milestone.title }));
            taskMilestoneSelector.tlcombobox();
        }
    };

    var onUpdateTeam = function () {
        if (currentProjectId == taskProjectSelector.val()) {
            onGetTeam({}, ASC.Projects.Master.Team);
        }
    };

    return {
        init: init,
        showUpdateTaskForm: showUpdateTaskForm,
        showCreateNewTaskForm: showCreateNewTaskForm,
        onUpdateProjectTeam: onUpdateTeam,
        onAddNewMileston: onAddNewMileston
    };
})(jQuery);
