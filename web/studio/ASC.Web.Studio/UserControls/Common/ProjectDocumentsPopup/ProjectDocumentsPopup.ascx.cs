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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using Resources;
using System.Web.UI.HtmlControls;

namespace ASC.Web.Studio.UserControls.Common.ProjectDocumentsPopup
{
    public partial class ProjectDocumentsPopup : System.Web.UI.UserControl
    {
        public string PopupName { get; set; }
        public int ProjectId { get; set; }

        public static string Location { get { return "~/UserControls/Common/ProjectDocumentsPopup/ProjectDocumentsPopup.ascx"; } }

        public ProjectDocumentsPopup()
        {
            PopupName = UserControlsCommonResource.AttachOfProjectDocuments;
            ProjectId = 0;
        }

        private void InitScripts()
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/projectdocumentspopup/css/projectDocumentsPopup.less"));

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/projectdocumentspopup/js/projectDocumentsPopup.js"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _documentUploader.Options.IsPopup = true;
            InitScripts();
            var emptyParticipantScreenControl = new EmptyScreenControl
            {
                ImgSrc = VirtualPathUtility.ToAbsolute("~/UserControls/Common/ProjectDocumentsPopup/Images/project-documents.png"),
                Header = UserControlsCommonResource.ProjectDocuments,
                HeaderDescribe = UserControlsCommonResource.EmptyDocsHeaderDescription,
                Describe = Resources.UserControlsCommonResource.EmptyDocsDescription
            };
            _phEmptyDocView.Controls.Add(emptyParticipantScreenControl);
        }
    }
}