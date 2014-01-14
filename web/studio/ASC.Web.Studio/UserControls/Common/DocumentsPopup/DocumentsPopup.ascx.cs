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

namespace ASC.Web.Studio.UserControls.Common.DocumentsPopup
{
    public partial class DocumentsPopup : System.Web.UI.UserControl
    {
        public string PopupName { get; set; }
        public int ProjectId { get; set; }

        public static string Location { get { return "~/UserControls/Common/DocumentsPopup/DocumentsPopup.ascx"; } }

        public DocumentsPopup()
        {
            PopupName = UserControlsCommonResource.AttachFromDocuments;
            ProjectId = 0;
        }

        private void InitScripts()
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/documentspopup/css/documentsPopup.less"));

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/documentspopup/js/documentsPopup.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/products/files/js/common.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/documentspopup/js/foldermanager.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/products/files/js/templatemanager.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/documentspopup/js/tree.js"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _documentUploader.Options.IsPopup = true;
            InitScripts();
            var emptyParticipantScreenControl = new EmptyScreenControl
            {
                ImgSrc = VirtualPathUtility.ToAbsolute("~/UserControls/Common/DocumentsPopup/css/images/project-documents.png"),
                Header = "", //UserControlsCommonResource.DocumentsProduct,
                HeaderDescribe = UserControlsCommonResource.EmptyDocsHeaderDescription,
                Describe = ""//"<a id='empty_screen_back_link' href='javascript:;' onclick='DocumentsPopup.openPreviosFolder(); return false;'>Back</a>"//Resources.UserControlsCommonResource.EmptyDocsDescription
            };
            _phEmptyDocView.Controls.Add(emptyParticipantScreenControl);
        }
    }
}