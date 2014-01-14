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
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Controls.Reports
{
    public partial class ReportTemplateView : BaseUserControl
    {
        public int TemplateId { get; set; }
        public ReportTemplate Template { get; set; }

        public string TmplParamHour { get; set; }
        public int TmplParamMonth { get; set; }
        public int TmplParamWeek { get; set; }
        public string TmplParamPeriod { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Template = Global.EngineFactory.GetReportEngine().GetTemplate(TemplateId);
            if (Template == null)
            {
                Response.Redirect("reports.aspx?reportType=0");
            }
            else
            {
                var filters = (ReportFilters) LoadControl(PathProvider.GetControlVirtualPath("ReportFilters.ascx"));
                filters.Report = Report.CreateNewReport(Template.ReportType, Template.Filter);
                _filter.Controls.Add(filters);
                InitTmplParam();

                Page.Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, HttpUtility.HtmlDecode(Template.Name)));
            }
            _hintPopup.Options.IsPopup = true;
        }

        private void InitTmplParam()
        {
            var cron = Template.Cron.Split(' ');

            try
            {
                TmplParamWeek = Int32.Parse(cron[5]);
                TmplParamPeriod = "week";
            }
            catch (FormatException)
            {
                try
                {
                    TmplParamMonth = Int32.Parse(cron[3]);
                    TmplParamPeriod = "month";
                }
                catch (FormatException)
                {
                    TmplParamPeriod = "day";
                }
            }

            TmplParamHour = cron[2];

        }
    }
}