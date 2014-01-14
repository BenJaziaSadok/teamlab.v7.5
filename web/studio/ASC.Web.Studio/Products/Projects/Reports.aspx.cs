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
using System.Text;
using System.Threading;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Classes;
using Global = ASC.Web.Projects.Classes.Global;
using PathProvider = ASC.Web.Projects.Classes.PathProvider;

namespace ASC.Web.Projects
{
    public partial class Reports : BasePage
    {
        public List<Report> ListReports { get; private set; }

        public List<ReportTemplate> ListTemplates { get; set; }

        protected override void PageLoad()
        {
            if (Participant.IsVisitor)
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("reports.js"));

            var tmplId = Request["tmplId"];
            if (!string.IsNullOrEmpty(tmplId))
            {
                var reportTemplateControl = (Controls.Reports.ReportTemplateView)LoadControl(PathProvider.GetControlVirtualPath("ReportTemplateView.ascx"));
                reportTemplateControl.TemplateId = int.Parse(tmplId);
                _content.Controls.Add(reportTemplateControl);
            }
            else
            {
                if (!string.IsNullOrEmpty(Request["reportType"]))
                {
                    var report = (Controls.Reports.ReportView)LoadControl(PathProvider.GetControlVirtualPath("ReportView.ascx"));
                    _content.Controls.Add(report);
                }
                else
                {
                    Response.Redirect("reports.aspx?reportType=0");
                }
            }

            ListTemplates = Global.EngineFactory.GetReportEngine().GetTemplates(Participant.ID);
            SetReportList();
        }


        #region private Methods

        private void SetReportList()
        {
            var typeCount = Enum.GetValues(typeof(ReportType)).Length - 1;
            ListReports = new List<Report>();
            for (var i = 0; i < typeCount; i++)
            {
                var report = Report.CreateNewReport((ReportType)i, new TaskFilter());
                ListReports.Add(report);
            }
        }


        #endregion
    }

    public static class TemplateParamInitialiser
    {
        public static string InitHoursCombobox()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < 24; i++)
            {
                var value = string.Format(i < 10 ? "0{0}:00" : "{0}:00", i);

                sb.AppendFormat(
                    i == 12
                        ? "<option value='{0}' selected='selected'>{1}</option>"
                        : "<option value='{0}' >{1}</option>", i, value);
            }

            return sb.ToString();
        }
        public static string InitDaysOfWeek()
        {
            var sb = new StringBuilder();
            var format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            //in cron expression week day 1-7 (not 0-6)
            var firstday = (int)format.FirstDayOfWeek;
            for (var i = firstday; i < firstday + 7; i++)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", i % 7 + 1, format.GetDayName((DayOfWeek)(i % 7)));
            }
            return sb.ToString();
        }
        public static string InitDaysOfMonth()
        {
            var sb = new StringBuilder();
            for (var i = 1; i < 32; i++)
            {
                sb.AppendFormat("<option value='{0}'>{0}</option>", i);
            }
            return sb.ToString();
        }
    }
}
