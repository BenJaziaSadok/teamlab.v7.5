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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Studio.UserControls.Users
{
    public class ImportUsersWebControl: WebControl
    {
        private ImportUsers _users;
        private Container _localContainer;
        private Container _importedContainer;

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        public string Text { get; set; }
        public string LinkStyle { get; set; }
        public bool RenderLink { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!ChildControlsCreated)
                CreateChildControls();
        }

        protected override void CreateChildControls()
        {
            if (RenderLink)
            {
                var link = new HtmlAnchor {InnerText = Text, HRef = "#"};
                link.Attributes.Add("onclick", "ImportUsersManager.ShowImportControl();");
                if (!string.IsNullOrEmpty(LinkStyle))
                    link.Attributes.Add("class", LinkStyle);

                Controls.Add(link);
            }
            Controls.Add(Page.LoadControl(ImportUsersTemplate.Location));
            _users = new ImportUsers();
            _users = (ImportUsers)_users.LoadControl(ImportUsers.Location);

            Controls.Add(new LiteralControl("<div id=\"importAreaBlock\" class=\"importAreaBlock\" style=\"display:none\">"));

            _localContainer = new Container { Body = new PlaceHolder(), Header = new PlaceHolder() };
            _localContainer.Body.Controls.Add(_users);
            var html = new HtmlGenericControl("DIV") { InnerHtml = CustomNamingPeople.Substitute<Resources.Resource>("ImportContactsHeader").HtmlEncode() };
            _localContainer.Header.Controls.Add(html);
            Controls.Add(_localContainer);
            Controls.Add(new LiteralControl("</div>"));

            _importedContainer = new Container { Body = new PlaceHolder(), Header = new PlaceHolder() };
            var html1 = new HtmlGenericControl("DIV") { InnerHtml = Resources.Resource.FinishImportUserTitle };
            var html2 = new HtmlGenericControl("DIV") { InnerHtml = CustomNamingPeople.Substitute<Resources.Resource>("ImportContactsHeader").HtmlEncode() };
            _importedContainer.Body.Controls.Add(html1);
            _importedContainer.Header.Controls.Add(html2);

            Controls.Add(new LiteralControl("<div id=\"successImportedArea\">"));
            Controls.Add(_importedContainer);
            Controls.Add(new LiteralControl("</div>"));
            
            Controls.Add(Page.LoadControl(TariffLimitExceed.Location));

            base.CreateChildControls();

            ChildControlsCreated = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            _localContainer.Options.IsPopup = true;
            _importedContainer.Options.IsPopup = true;
            _localContainer.Options.OnCancelButtonClick = "ImportUsersManager.HideImportWindow();";
        }
    }
}
