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

#region Import

using System.Linq;
using System.Text;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using ASC.Core;
using ASC.Projects.Engine;
using System.Web;

#endregion

namespace ASC.Web.Projects
{
    public partial class ProjectTeam : BasePage
    {

        #region Properties

        public UserInfo Manager { get; set; }

        public bool CanEditTeam { get; set; }

        public string UserProfileLink
        {
            get { return CommonLinkUtility.GetUserProfile(); }
        }

        public string ManagerName { get; set; }

        public string ManagerAvatar { get; set; }

        public string ManagerProfileUrl { get; set; }

        public string ManagerDepartmentUrl { get; set; }

        #endregion

        #region Methods

        protected void InitView()
        {
            CanEditTeam = ProjectSecurity.CanEditTeam(Project);
        }

        public bool CanCreateTask()
        {
            return ProjectSecurity.CanCreateTask(Project);
        }

        private void InitScripts()
        {
            var scripts = new StringBuilder();
            scripts.Append("var url = location.href;");
            scripts.Append("if (url.indexOf('projectTeam.aspx') > 0) {");
            scripts.Append("window.ASC.Projects.ProjectTeam.init();");
            scripts.Append("jq('#PrivateProjectHelp').click(function() {");
            scripts.Append("jq(this).helper({ BlockHelperID: 'AnswerForPrivateProjectTeam' });");
            scripts.Append("});");
            scripts.Append("jq('#RestrictAccessHelp').click(function() {");
            scripts.Append("jq(this).helper({ BlockHelperID: 'AnswerForRestrictAccessTeam' });");
            scripts.Append("});");
            scripts.Append("}");

            Page.RegisterInlineScript(scripts.ToString(), true);
        }

        #endregion

        #region Events

        protected override void PageLoad()
        {
            InitView();
            InitScripts();

            Manager = Global.EngineFactory.GetParticipantEngine().GetByID(Project.Responsible).UserInfo;
            ManagerName = Manager.DisplayUserName();
            ManagerAvatar = Manager.GetBigPhotoURL();
            ManagerProfileUrl = Manager.GetUserProfilePageURL();
            foreach (var g in CoreContext.UserManager.GetUserGroups(Manager.ID))
            {
                ManagerDepartmentUrl += string.Format("<a href=\"{0}\" class=\"linkMedium\">{1}</a>, ", CommonLinkUtility.GetDepartment(g.ID), HttpUtility.HtmlEncode(g.Name));
            }
            if (!string.IsNullOrEmpty(ManagerDepartmentUrl))
            {
                ManagerDepartmentUrl = ManagerDepartmentUrl.Substring(0, ManagerDepartmentUrl.Length - 2);
            }

            var userSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            userSelector.BehaviorID = "UserSelector";
            userSelector.DisabledUsers.Add(Project.Responsible);
            userSelector.Title = ProjectResource.ManagmentTeam;
            userSelector.SelectedUserListTitle = ProjectResource.Team;
            userSelector.CustomBottomHtml = string.Format("<div style='padding-top:10px'><input id='userSelectorNotify' type='checkbox'/><label for='userSelectorNotify' style='padding-left:10px' >{0}</label></div>", ProjectResource.NotifyProjectTeam);

            var selectedUsers = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID).Select(participant => participant.ID).ToList();

            userSelector.SelectedUsers = selectedUsers;

            _phTeamSelector.Controls.Add(userSelector);

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.ProjectTeam);

        }

        #endregion
    }
}