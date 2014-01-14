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
using System.Text;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Import;

namespace ASC.Web.Files.Controls
{
    public partial class ThirdParty : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("ThirdParty/ThirdParty.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/products/files/Controls/ThirdParty/thirdparty.js"));

            ThirdPartyEditorTemp.Options.IsPopup = true;
            ThirdPartyDeleteTmp.Options.IsPopup = true;
            ThirdPartyNewAccountTmp.Options.IsPopup = true;

            EmptyScreenThirdParty.Controls.Add(ThirdPartyEmptyScreen("emptyThirdPartyContainer"));
        }

        public static EmptyScreenControl ThirdPartyEmptyScreen(string controlId)
        {

            var buttonAddBoxNet = string.Format("<span id=\"emptyContainerAddBoxNet\" class=\"empty-container-add-account BoxNet\"><a class=\"baseLinkAction\">{0}</a></span>", FilesUCResource.ButtonAddBoxNet);
            var buttonAddDropBox = string.Format("<span id=\"emptyContainerAddDropBox\" class=\"empty-container-add-account DropBox\"><a class=\"baseLinkAction\">{0}</a></span>", FilesUCResource.ButtonAddDropBox);
            var buttonAddGoogle = string.Format("<span id=\"emptyContainerAddGoogle\" class=\"empty-container-add-account Google\"><a class=\"baseLinkAction\">{0}</a></span>", FilesUCResource.ButtonAddGoogle);
            var buttonAddSkyDrive = string.Format("<span id=\"emptyContainerAddSkyDrive\" class=\"empty-container-add-account SkyDrive\"><a class=\"baseLinkAction\">{0}</a></span>", FilesUCResource.ButtonAddSkyDrive);

            var buttons = new StringBuilder();

            if (ImportConfiguration.SupportDropboxInclusion)
                buttons.Append(buttonAddDropBox);

            if (ImportConfiguration.SupportGoogleInclusion)
                buttons.Append(buttonAddGoogle);

            if (ImportConfiguration.SupportBoxNetInclusion)
                buttons.Append(buttonAddBoxNet);

            if (ImportConfiguration.SupportSkyDriveInclusion)
                buttons.Append(buttonAddSkyDrive);

            var descrMy = string.Format("");
            return new EmptyScreenControl
                {
                    ID = controlId,
                    ImgSrc = PathProvider.GetImagePath("empty_screen_my.png"),
                    Header = FilesUCResource.ThirdPartyConnectAccounts,
                    HeaderDescribe = FilesUCResource.EmptyScreenThirdPartyDscr,
                    Describe = descrMy,
                    ButtonHTML = buttons.ToString()
                };
        }
    }
}