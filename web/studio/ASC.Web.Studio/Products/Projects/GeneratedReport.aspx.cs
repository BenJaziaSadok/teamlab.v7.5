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
using System.IO;
using System.Text;
using System.Web;
using ASC.Core.Tenants;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Files.Utils;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Reports;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using Global = ASC.Web.Projects.Classes.Global;
using PathProvider = ASC.Web.Projects.Classes.PathProvider;

namespace ASC.Web.Projects
{
    public partial class GeneratedReport : BasePage
    {
        protected int GenerateReportType;

        protected bool HasData { get; set; }

        protected bool TemplateNotFound { get; set; }

        protected override void PageLoad()
        {
            if (Participant.IsVisitor)
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/sorttable.js"));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("reports.js"));

            exportReportPopup.Options.IsPopup = true;
            HasData = true;
            TemplateNotFound = false;
            Master.DisabledSidePanel = true;
            Master.Master.DisabledTopStudioPanel = true;

            int.TryParse(UrlParameters.ReportType, out GenerateReportType);
            var repType = (ReportType) GenerateReportType;

            var filter = new TaskFilter();
            int templateID;
            var reportName = "";
            Report report;

            if (int.TryParse(UrlParameters.EntityID, out templateID))
            {
                var template = Global.EngineFactory.GetReportEngine().GetTemplate(templateID);
                if (template != null)
                {
                    filter = template.Filter;
                    repType = template.ReportType;
                    GenerateReportType = (int) template.ReportType;
                    Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, HttpUtility.HtmlDecode(template.Name)));
                    reportName = string.Format(ReportResource.ReportPageTitle, template.Name);
                }
                else
                {
                    TemplateNotFound = true;
                    HasData = false;
                    emptyScreenControlPh.Controls.Add(new ElementNotFoundControl
                        {
                            Header = ReportResource.TemplateNotFound_Header,
                            Body = ReportResource.TemplateNotFound_Body,
                            RedirectURL = "reports.aspx?reportType=0",
                            RedirectTitle = ReportResource.TemplateNotFound_RedirectTitle
                        });
                    Master.DisabledSidePanel = false;
                }
            }
            else
            {
                filter = TaskFilter.FromUri(HttpContext.Current.Request.GetUrlRewriter());

                report = Report.CreateNewReport(repType, filter);

                Title = HeaderStringHelper.GetPageTitle(string.Format(ReportResource.ReportPageTitle, report.ReportInfo.Title));
                reportName = string.Format(ReportResource.ReportPageTitle, report.ReportInfo.Title);
            }

            report = Report.CreateNewReport(repType, filter);

            var filters = (ReportFilters) LoadControl(PathProvider.GetControlVirtualPath("ReportFilters.ascx"));
            filters.Report = report;
            _filter.Controls.Add(filters);

            var outputFormat = GetOutputFormat();
            var result = ReportHelper.BuildReport(repType, filter, outputFormat, templateID);

            OutputData(result, reportName, report, outputFormat);

        }

        private static ReportViewType GetOutputFormat()
        {
            var outputFormat = ReportViewType.Html;
            switch (HttpContext.Current.Request["format"])
            {
                case "csv":
                    outputFormat = ReportViewType.Csv;
                    break;
                case "xml":
                    outputFormat = ReportViewType.Xml;
                    break;
                case "email":
                    outputFormat = ReportViewType.EMail;
                    break;
                case "html":
                    outputFormat = ReportViewType.Html;
                    break;
            }

            return outputFormat;
        }

        private void OutputData(string result, string reportName, Report report, ReportViewType outputFormat)
        {
            switch (outputFormat)
            {
                case ReportViewType.Html:
                    reportResult.Text = result;

                    var sb = new StringBuilder();
                    sb.Append("<div class='report-name'>");
                    sb.Append(reportName);
                    sb.Append("<span class='generation-date'> (");
                    sb.Append(TenantUtil.DateTimeNow().ToString(DateTimeExtension.ShortDatePattern));
                    sb.Append(")</span>");
                    sb.Append("</div>");
                    reportFilter.Text = sb.ToString();
                    break;

                case ReportViewType.Xml:
                case ReportViewType.EMail:
                    if (result != null)
                    {
                        var ext = outputFormat.ToString().ToLower();
                        Response.Clear();
                        Response.ContentType = "text/" + ext + "; charset=utf-8";
                        Response.ContentEncoding = Encoding.UTF8;
                        Response.Charset = Encoding.UTF8.WebName;
                        Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.{1}", report.FileName, ext));
                        Response.Write(result);
                        Response.End();
                    }
                    break;

                case ReportViewType.Csv:
                    string fileURL;

                    using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                    {
                        var file = FileUploader.Exec(Files.Classes.Global.FolderMy.ToString(), report.FileName + ".csv", result.Length, memStream, true);

                        fileURL = CommonLinkUtility.GetFileWebEditorUrl((int) file.ID);
                        fileURL += string.Format("&options={{\"delimiter\":{0},\"codePage\":{1}}}",
                                                 (int)Global.ReportCsvDelimiter.Key,
                                                 Encoding.UTF8.CodePage);
                    }

                    Response.Redirect(fileURL);

                    break;
            }
        }
    }
}