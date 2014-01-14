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
using System.Web;
using System.Web.UI;
using ASC.Web.Core.Mobile;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.UserControls.Common.Attachments
{
    public partial class Attachments : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/Attachments/Attachments.ascx"; }
        }

        public bool PortalDocUploaderVisible { get; set; }

        public string MenuNewDocument { get; set; }

        public string MenuUploadFile { get; set; }

        public string MenuProjectDocuments { get; set; }

        public string ModuleName { get; set; }

        public string EntityType { get; set; }

        public bool CanAddFile { get; set; }

        public int ProjectId { get; set; }

        public bool EmptyScreenVisible { get; set; }

        protected string ExtsWebPreviewed = string.Join(", ", FileUtility.ExtsWebPreviewed.ToArray());
        protected string ExtsWebEdited = string.Join(", ", FileUtility.ExtsWebEdited.ToArray());

        public Attachments()
        {
            PortalDocUploaderVisible = true;
            EmptyScreenVisible = true;
            MenuNewDocument = UserControlsCommonResource.NewFile;
            MenuUploadFile = UserControlsCommonResource.UploadFile;
            MenuProjectDocuments = UserControlsCommonResource.AttachOfProjectDocuments;

            EntityType = "";
            ModuleName = "";
            CanAddFile = true;
        }

        private void InitScripts()
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/attachments/css/attachments.less"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/attachments/js/attachments.js"));
        }

        private void CreateEmptyPanel()
        {
            var buttons = "<a id='uploadFirstFile' class='baseLinkAction'>" + MenuUploadFile + "</a><br/>" +
                          "<a id='createFirstDocument' class='baseLinkAction'>" + MenuNewDocument + "</a>" +
                          "<span class='sort-down-black newDocComb'></span>";
            if (ModuleName != "crm")
            {
                buttons += "<br/><a id='attachProjDocuments' class='baseLinkAction'>" + MenuProjectDocuments + "</a>";
            }

            var emptyParticipantScreenControl = new EmptyScreenControl
                {
                    ImgSrc = VirtualPathUtility.ToAbsolute("~/UserControls/Common/Attachments/Images/documents-logo.png"),
                    Header = UserControlsCommonResource.EmptyListDocumentsHead,
                    Describe =
                        MobileDetector.IsRequestMatchesMobile(Context)
                            ? UserControlsCommonResource.EmptyListDocumentsDescrMobile
                            : String.Format(UserControlsCommonResource.EmptyListDocumentsDescr,
                                            //create
                                            "<span class='hintCreate baseLinkAction' >", "</span>",
                                            //upload
                                            "<span class='hintUpload baseLinkAction' >", "</span>",
                                            //open
                                            "<span class='hintOpen baseLinkAction' >", "</span>",
                                            //edit
                                            "<span class='hintEdit baseLinkAction' >", "</span>"),
                    ButtonHTML = buttons
                };
            _phEmptyDocView.Controls.Add(emptyParticipantScreenControl);
        }

        private void InitProjectDocumentsPopup()
        {
            var projectDocumentsPopup = (ProjectDocumentsPopup.ProjectDocumentsPopup)LoadControl(ProjectDocumentsPopup.ProjectDocumentsPopup.Location);
            projectDocumentsPopup.ProjectId = ProjectId;
            _phDocUploader.Controls.Add(projectDocumentsPopup);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _hintPopup.Options.IsPopup = true;
            InitScripts();

            if (EmptyScreenVisible)
                CreateEmptyPanel();

            if (!TenantExtra.GetTenantQuota().DocsEdition)
            {
                TariffDocsEditionPlaceHolder.Controls.Add(LoadControl(TariffLimitExceed.Location));
            }

            if (ModuleName != "crm")
            {
                var projId = Request["prjID"];
                if (!String.IsNullOrEmpty(projId))
                {
                    ProjectId = Convert.ToInt32(projId);
                    InitProjectDocumentsPopup();
                }
                else
                {
                    ProjectId = 0;
                }
            }
            else
            {
                PortalDocUploaderVisible = false;
            }
        }
    }
}