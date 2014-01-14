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
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Files.Controls
{
    public partial class EmptyFolder : UserControl
    {
        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("EmptyFolder/EmptyFolder.ascx"); }
        }

        public bool AllContainers { get; set; }

        #endregion

        #region Members

        protected string ExtsWebPreviewed = string.Join(", ", Studio.Utility.FileUtility.ExtsWebPreviewed.ToArray());
        protected string ExtsWebEdited = string.Join(", ", Studio.Utility.FileUtility.ExtsWebEdited.ToArray());

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var isMobile = Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);
            var isVisitor = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor();

            var strCreateFile =
                !isMobile && !isVisitor
                    ? string.Format(@"<span class=""empty-folder-create empty-folder-create-editor"">
<a class=""baseLinkAction empty-folder-create-document"">{0}</a>,
<a class=""baseLinkAction empty-folder-create-spreadsheet"">{1}</a>,
<a class=""baseLinkAction empty-folder-create-presentation"">{2}</a>
</span>",
                                    FilesUCResource.ButtonCreateText,
                                    FilesUCResource.ButtonCreateSpreadsheet,
                                    FilesUCResource.ButtonCreatePresentation)
                    : string.Empty;

            var strCreateFolder =
                !isMobile
                    ? string.Format(@"<a class=""empty-folder-create empty-folder-create-folder baseLinkAction"">{0}</a>", FilesUCResource.ButtonCreateFolder)
                    : string.Empty;

            var strDragDrop = string.Format("<span class=\"emptyContainer_dragDrop\" > {0}</span>", FilesUCResource.EmptyScreenDescrDragDrop);

            var strToParent = string.Format("<a class=\"empty-folder-toparent baseLinkAction\" >{0}</a>", FilesUCResource.ButtonToParentFolder);

            if (AllContainers)
            {
                //my
                if (!isVisitor)
                {
                    var myButton = new StringBuilder();
                    myButton.Append(strCreateFile);
                    myButton.Append(strCreateFolder);
                    myButton.Append(strToParent);

                    var descrMy = string.Format(FilesUCResource.EmptyScreenDescrMy,
                                                //create
                                                "<span class=\"hintCreate baseLinkAction\" >", "</span>",
                                                //upload
                                                "<span class=\"hintUpload baseLinkAction\" >", "</span>",
                                                //open
                                                "<span class=\"hintOpen baseLinkAction\" >", "</span>",
                                                //edit
                                                "<span class=\"hintEdit baseLinkAction\" >", "</span>"
                        );
                    descrMy += strDragDrop;

                    EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                        {
                            ID = "emptyContainer_my",
                            ImgSrc = PathProvider.GetImagePath("empty_screen_my.png"),
                            Header = FilesUCResource.MyFiles,
                            HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                            Describe = descrMy,
                            ButtonHTML = myButton.ToString()
                        });
                }

                if (!CoreContext.Configuration.YourDocs)
                {
                    //forme
                    var formeButton = new StringBuilder();
                    formeButton.Append(strCreateFile);
                    formeButton.Append(strCreateFolder);
                    formeButton.Append(strToParent);

                    EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                        {
                            ID = "emptyContainer_forme",
                            ImgSrc = PathProvider.GetImagePath("empty_screen_forme.png"),
                            Header = FilesUCResource.SharedForMe,
                            HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                            Describe = FilesUCResource.EmptyScreenDescrForme,
                            ButtonHTML = formeButton.ToString()
                        });

                    //corporate
                    var corporateButton = new StringBuilder();
                    corporateButton.Append(strCreateFile);
                    corporateButton.Append(strCreateFolder);
                    corporateButton.Append(strToParent);

                    EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                        {
                            ID = "emptyContainer_corporate",
                            ImgSrc = PathProvider.GetImagePath("empty_screen_corporate.png"),
                            Header = FilesUCResource.CorporateFiles,
                            HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                            Describe = FilesUCResource.EmptyScreenDescrCorporate + strDragDrop,
                            ButtonHTML = corporateButton.ToString()
                        });
                }

                if (!CoreContext.Configuration.YourDocsDemo)
                {
                    var strGotoMy = !isVisitor ? string.Format("<a href=\"#{1}\" class=\"empty-folder-goto baseLinkAction\">{0}</a>", FilesUCResource.ButtonGotoMy, Global.FolderMy) : string.Empty;
                    //trash
                    EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                        {
                            ID = "emptyContainer_trash",
                            ImgSrc = PathProvider.GetImagePath("empty_screen_trash.png"),
                            Header = FilesUCResource.Trash,
                            HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                            Describe = FilesUCResource.EmptyScreenDescrTrash,
                            ButtonHTML = strGotoMy
                        });
                }
                else
                {
                    EmptyScreenFolder.Controls.Add(ThirdParty.ThirdPartyEmptyScreen("emptyContainer_thirdParty"));
                }
            }

            if (!CoreContext.Configuration.YourDocs)
            {
                //project
                var projectButton = new StringBuilder();
                projectButton.Append(strCreateFile);
                projectButton.Append(strCreateFolder);
                projectButton.Append(strToParent);

                EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                    {
                        ID = "emptyContainer_project",
                        ImgSrc = PathProvider.GetImagePath("empty_screen_project.png"),
                        Header = FilesUCResource.ProjectFiles,
                        HeaderDescribe = FilesUCResource.EmptyScreenHeader,
                        Describe = FilesUCResource.EmptyScreenDescrProject + strDragDrop,
                        ButtonHTML = projectButton.ToString()
                    });
            }

            //Filter
            EmptyScreenFolder.Controls.Add(new EmptyScreenControl
                {
                    ID = "emptyContainer_filter",
                    ImgSrc = PathProvider.GetImagePath("empty_screen_filter.png"),
                    Header = FilesUCResource.Filter,
                    HeaderDescribe = FilesUCResource.EmptyScreenFilter,
                    Describe = FilesUCResource.EmptyScreenFilterDescr,
                    ButtonHTML = string.Format("<a id=\"files_clearFilter\" class=\"baseLinkAction\" >{0}</a>",
                                               FilesUCResource.ButtonClearFilter)
                });
        }
    }
}