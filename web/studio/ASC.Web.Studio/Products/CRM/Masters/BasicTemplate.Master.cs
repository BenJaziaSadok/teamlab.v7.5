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

using System;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Web.CRM.SocialMedia;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.CRM.Controls.Common;
using ASC.CRM.Core;

using AjaxPro;

namespace ASC.Web.CRM
{
    public partial class BasicTemplate : MasterPage
    {
        public Boolean ShowChangeButton = true;

        protected void Page_Init(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(SocialMediaUI));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();

            Page.EnableViewState = false;

            Page.RegisterClientScript(typeof(Masters.ClientScripts.ClientCustomResources));
            Page.RegisterClientLocalizationScript(typeof(Masters.ClientScripts.ClientLocalizationResources));
            Page.RegisterClientLocalizationScript(typeof(Masters.ClientScripts.ClientTemplateResources));

            Page.RegisterClientScript(typeof(Masters.ClientScripts.CommonData));
        }

        protected void RegisterScriptForTaskAction()
        {
            Page.RegisterClientScript(typeof(Masters.ClientScripts.TaskActionViewData));

            var sb = new StringBuilder();
            sb.AppendFormat(@"ASC.CRM.TaskActionView.init(""{0}"",{1},{2});",
                            DateTimeExtension.DateMaskForJQuery,
                            (int)ContactSelectorTypeEnum.All,
                            ShowChangeButton.ToString().ToLower()
                );
            Page.RegisterInlineScript(sb.ToString());
        }

        protected void InitControls()
        {
            SideNavigation.Controls.Add(LoadControl(NavigationSidePanel.Location));

            Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/products/crm/masters/Styles.ascx")));
            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/crm/masters/CommonBodyScripts.ascx")));

            RegisterScriptForTaskAction();
        }

        #region Methods

        public string CurrentPageCaption
        {
            get { return _commonContainer.CurrentPageCaption; }
            set { _commonContainer.CurrentPageCaption = value; }
        }

        public String CommonContainerHeader
        {
            set { _commonContainer.Options.HeaderBreadCrumbCaption = value; }
        }

        #endregion
    }
}