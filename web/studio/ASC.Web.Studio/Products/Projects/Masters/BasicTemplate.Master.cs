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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Projects.Masters
{
    public partial class BasicTemplate : MasterPage
    {
        #region Properties

        private string currentPage;

        public string CurrentPage
        {
            get
            {
                if (string.IsNullOrEmpty(currentPage))
                {
                    var absolutePathWithoutQuery = Request.Url.AbsolutePath.Substring(0, Request.Url.AbsolutePath.IndexOf(".aspx", StringComparison.Ordinal));
                    currentPage = absolutePathWithoutQuery.Substring(absolutePathWithoutQuery.LastIndexOf('/') + 1);
                }
                return currentPage;
            }
        }

        public string EssenceTitle { get; set; }

        public string EssenceStatus { get; set; }

        public bool IsSubcribed { get; set; }

        public EntityType EssenceType { get; set; }

        public bool DisabledSidePanel
        {
            get { return Master.DisabledSidePanel; }
            set { Master.DisabledSidePanel = value; }
        }

        public bool DisabledPrjNavPanel { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControls();

            WriteClientScripts();

            Page.EnableViewState = false;
        }

        #endregion

        #region Methods

        protected void InitControls()
        {
            if (!Master.DisabledSidePanel)
            {
                projectsNavigationPanel.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("NavigationSidePanel.ascx")));
            }

            if (!DisabledPrjNavPanel && RequestContext.IsInConcreteProject)
            {
                _projectNavigatePanel.Controls.Add(LoadControl(PathProvider.GetControlVirtualPath("ProjectNavigatePanel.ascx")));
            }
        }

        protected void WriteClientScripts()
        {
            WriteProjectResources();

            if (Page is GanttChart)
            {
                Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/projects/masters/GanttBodyScripts.ascx")));
                return;
            }

            Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/products/projects/masters/Styles.ascx")));
            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/projects/masters/CommonBodyScripts.ascx")));
        }

        public void RegisterCRMResources()
        {
            Page.RegisterStyleControl(ResolveUrl(VirtualPathUtility.ToAbsolute("~/products/crm/app_themes/default/css/common.css")));
            Page.RegisterStyleControl(ResolveUrl(VirtualPathUtility.ToAbsolute("~/products/crm/app_themes/default/css/contacts.css")));

            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/third-party/jquery/jquery.watermarkinput.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/crm/js/contacts.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/crm/js/common.js"));
        }

        public void WriteProjectResources()
        {
            Page.RegisterClientLocalizationScript(typeof(ClientScripts.ClientLocalizationResources));
            Page.RegisterClientLocalizationScript(typeof(ClientScripts.ClientTemplateResources));

            Page.RegisterClientScript(typeof(ClientScripts.ClientUserResources));
            Page.RegisterClientScript(typeof(ClientScripts.ClientCurrentUserResources));

            if (RequestContext.IsInConcreteProject)
                Page.RegisterClientScript(typeof(ClientScripts.ClientProjectResources));
        }

        public void JsonPublisher<T>(T data, String jsonClassName) where T : class
        {
            String json;

            using (var stream = new MemoryStream())
            {

                var serializer = new DataContractJsonSerializer(typeof(T));

                serializer.WriteObject(stream, data);

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(),
                                                        Guid.NewGuid().ToString(),
                                                        String.Format(" var {1} = {0};", json, jsonClassName),
                                                        true);
        }

        public static EmptyScreenControl RenderEmptyScreenForFilter(string headerText, string description)
        {
            var emptyScreenControlFilter = new EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png"),
                    Header = headerText,
                    Describe = description,
                    ID = "emptyScreenForFilter",
                    ButtonHTML = String.Format("<a class='baseLinkAction clearFilterButton'>{0}</a>",
                                               ProjectsFilterResource.ClearFilter)
                };
            return emptyScreenControlFilter;
        }

        #endregion
    }
}