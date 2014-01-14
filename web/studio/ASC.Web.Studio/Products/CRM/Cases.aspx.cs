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

using System;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Cases;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;

#endregion

namespace ASC.Web.CRM
{
    public partial class Cases : BasePage
    {
        #region Properies

        #endregion

        #region Events

        protected override void PageLoad()
        {
            int caseID;

            if (int.TryParse(UrlParameters.ID, out caseID))
            {

                ASC.CRM.Core.Entities.Cases targetCase = Global.DaoFactory.GetCasesDao().GetByID(caseID);

                if (targetCase == null || !CRMSecurity.CanAccessTo(targetCase))
                    Response.Redirect(PathProvider.StartURL() + "cases.aspx");

                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                {
                    ExecCasesActionView(targetCase);
                }
                else
                {
                    ExecCasesDetailsView(targetCase);
                }
            }
            else
            {
                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecCasesActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListCasesView();
            }
        }

        #endregion

        #region Methods

        protected void ExecImportView()
        {
            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);
            importViewControl.EntityType = EntityType.Case;
            CommonContainerHolder.Controls.Add(importViewControl);

            Master.CurrentPageCaption = CRMCasesResource.ImportCases;
            Title = HeaderStringHelper.GetPageTitle(CRMCasesResource.ImportCases);
        }

        protected void ExecListCasesView()
        {
            CommonContainerHolder.Controls.Add(LoadControl(ListCasesView.Location));
            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? CRMCasesResource.AllCases);
        }

        protected void ExecCasesDetailsView(ASC.CRM.Core.Entities.Cases targetCase)
        {
            var casesDetailsViewControl = (CasesDetailsView)LoadControl(CasesDetailsView.Location);
            casesDetailsViewControl.TargetCase = targetCase;
            CommonContainerHolder.Controls.Add(casesDetailsViewControl);

            var title = targetCase.Title.HtmlEncode();

            Master.CurrentPageCaption = title ;
            Master.CommonContainerHeader = Global.RenderItemHeaderWithMenu(title, EntityType.Case, CRMSecurity.IsPrivate(targetCase));

            Title = HeaderStringHelper.GetPageTitle(title);
        }

        protected void ExecCasesActionView(ASC.CRM.Core.Entities.Cases targetCase)
        {
            var casesActionViewControl = (CasesActionView)LoadControl(CasesActionView.Location);

            casesActionViewControl.TargetCase = targetCase;
            CommonContainerHolder.Controls.Add(casesActionViewControl);

            var headerTitle = targetCase == null ? CRMCasesResource.CreateNewCase : String.Format(CRMCasesResource.EditCaseHeader, targetCase.Title);

            Master.CurrentPageCaption = headerTitle;
            Title = HeaderStringHelper.GetPageTitle(headerTitle);
        }

        #endregion

    }
}