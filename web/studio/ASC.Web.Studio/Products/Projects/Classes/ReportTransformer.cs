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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using ASC.Projects.Core.Domain.Reports;
using ASC.Web.Projects.Resources;
using System.Reflection;
using ASC.Web.Studio.Core;

namespace ASC.Web.Projects.Classes
{
    static class ReportTransformer
    {
        public static string Transform(IList<object[]> reportData, Report report, int subType, ReportViewType view, int templateID)
        {
            var xml = new StringBuilder();

            if (reportData.Count != 0)
            {
                xml = xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
                   .Append("<reportResult>");
                foreach (var row in reportData)
                {
                    xml.Append("<r ");
                    for (var i = 0; i < row.Length; i++)
                    {
                        xml.AppendFormat("c{0}=\"{1}\" ", i, ToString(row[i]));
                    }
                    xml.Append("/>");
                }
                xml.Append("</reportResult>");
            }
            else
            {
                xml = xml.Append(string.Format("<div class='noContentBlock'>{0}</div>", ProjectsCommonResource.NoData));
            }

            return Transform(xml.ToString(), report, subType, view, templateID);
        }

        public static string Transform(string xml, Report report, int subType, ReportViewType view, int templateID)
        {
            if (view == ReportViewType.Xml)
            {
                return Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(xml)));
            }
            if (view == ReportViewType.EMail)
            {
                xml = Transform(xml, report, subType, ReportViewType.Html, templateID);
            }

            var xslt = GetXslTransform(report.ReportType, subType, view);
            if (xslt == null) throw new InvalidOperationException("Xslt not found for type " + report.ReportType + " and view " + view);

            using (var reader = XmlReader.Create(new StringReader(xml)))
            using (var writer = new StringWriter())
            using (XmlWriter.Create(writer, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                xslt.Transform(reader, GetXslParameters(report, view, templateID), writer);
                return writer.ToString();
            }
        }


        private static string ToString(object value)
        {
            if (value == null) return null;
            if (value is Enum) return ((Enum)value).ToString("d");
            if (value is DateTime) return ((DateTime)value).ToString("o");
            if (value is float) return ((float) value).ToString(CultureInfo.InvariantCulture);
            return value.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static XslCompiledTransform GetXslTransform(ReportType reportType, int subType, ReportViewType viewType)
        {
            var viewTypeStr = viewType.ToString().ToLower();
            return GetXslTransform(string.Format("{0}_{1}.{2}.xsl", reportType, subType, viewTypeStr)) ??
                GetXslTransform(string.Format("{0}.{1}.xsl", reportType, viewTypeStr)) ??
                GetXslTransform(string.Format("{0}.xsl", viewTypeStr));
        }

        public static XslCompiledTransform GetXslTransform(string fileName)
        {
            var transform = new XslCompiledTransform();
            var assembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(assembly.Location);
            if ("bin".Equals(Path.GetFileName(path), StringComparison.InvariantCultureIgnoreCase) && File.Exists(fileName))
            {
                    transform.Load(fileName);
                    return transform;
            }

            using (var stream = assembly.GetManifestResourceStream("ASC.Web.Projects.templates." + fileName))
            {
                if (stream != null)
                {
                    using (var xmlReader = XmlReader.Create(stream))
                    {
                        transform.Load(xmlReader);
                        return transform;
                    }
                }
            }
            return null;
        }

        private static XsltArgumentList GetXslParameters(Report report, ReportViewType view, int templateID)
        {
            var parameters = new XsltArgumentList();
            var columns = report.GetColumns(view, templateID);
            string logo = string.IsNullOrEmpty(SetupInfo.MainLogoMailTmplURL) ? "http://cdn.teamlab.com/media/newsletters/images/00.jpg" : SetupInfo.MainLogoMailTmplURL;

            for (var i = 0; i < columns.Count; i++)
            {
                parameters.AddParam("p" + i, string.Empty, columns[i]);
            }

            parameters.AddParam("p" + columns.Count, string.Empty, Global.ReportCsvDelimiter.Value);
            parameters.AddParam("logo", string.Empty, logo);

            return parameters;
        }
    }
}