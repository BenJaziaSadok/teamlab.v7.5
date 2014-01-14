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
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Controls.Reports
{
    public partial class ReportView : BaseUserControl
    {
        public Report Report { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            reportTemplateContainer.Options.IsPopup = true;
            InitReport();
            Page.Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, Report.ReportInfo.Title)); 
        }

        private void InitReport()
        {
            var filter = TaskFilter.FromUri(HttpContext.Current.Request.GetUrlRewriter());
            var reportType = Request["reportType"];
            if (string.IsNullOrEmpty(reportType)) return;

            Report = Report.CreateNewReport((ReportType) int.Parse(reportType), filter);

            var filters = (ReportFilters) LoadControl(PathProvider.GetControlVirtualPath("ReportFilters.ascx"));
            filters.Report = Report;
            _filter.Controls.Add(filters);
        }
    }
}