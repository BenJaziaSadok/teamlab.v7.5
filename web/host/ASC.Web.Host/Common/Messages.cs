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

using System.IO;
using System.Text;
using System.Web;
using ASC.Web.Host.HttpRequestProcessor;

namespace ASC.Web.Host.Common
{
    static class Messages
    {
        private const string _dirListingDirFormat =
            "{0,38:dddd, MMMM dd, yyyy hh:mm tt}        &lt;dir&gt; <A href=\"{1}/\">{2}</A>\r\n";

        private const string _dirListingFileFormat =
            "{0,38:dddd, MMMM dd, yyyy hh:mm tt} {1,12:n0} <A href=\"{2}\">{3}</A>\r\n";

        private const string _dirListingFormat1 =
            "<html>\r\n    <head>\r\n    <title>Directory Listing -- {0}</title>\r\n";

        private const string _dirListingFormat2 =
            "    </head>\r\n    <body bgcolor=\"white\">\r\n\r\n    <h2> <i>Directory Listing -- {0}</i> </h2></span>\r\n\r\n            <hr width=100% size=1 color=silver>\r\n\r\n<PRE>\r\n";

        private const string _dirListingParentFormat = "<A href=\"{0}\">[To Parent Directory]</A>\r\n\r\n";

        private static readonly string _dirListingTail =
            (string.Format(
                "</PRE>\r\n            <hr width=100% size=1 color=silver>\r\n\r\n            <b>Version Information:</b>&nbsp;ASC Web Server {0}\r\n\r\n            </font>\r\n\r\n    </body>\r\n</html>\r\n",
                VersionString));

        private const string _httpErrorFormat1 = "<html>\r\n    <head>\r\n        <title>{0}</title>\r\n";

        private static readonly string _httpErrorFormat2 =
            (string.Format(
                "    </head>\r\n    <body bgcolor=\"white\">\r\n\r\n            <span><h1>Server Error in '{{0}}' Application.<hr width=100% size=1 color=silver></h1>\r\n\r\n            <h2> <i>HTTP Error {{1}} - {{2}}.</i> </h2></span>\r\n\r\n            <hr width=100% size=1 color=silver>\r\n\r\n            <b>Version Information:</b>&nbsp;ASC Web Server {0}\r\n\r\n            </font>\r\n\r\n    </body>\r\n</html>\r\n",
                VersionString));

        private const string _httpStyle =
            "        <style>\r\n        \tbody {font-family:\"Verdana\";font-weight:normal;font-size: 8pt;color:black;} \r\n        \tp {font-family:\"Verdana\";font-weight:normal;color:black;margin-top: -5px}\r\n        \tb {font-family:\"Verdana\";font-weight:bold;color:black;margin-top: -5px}\r\n        \th1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red }\r\n        \th2 { font-family:\"Verdana\";font-weight:normal;font-size:14pt;color:maroon }\r\n        \tpre {font-family:\"Lucida Console\";font-size: 8pt}\r\n        \t.marker {font-weight: bold; color: black;text-decoration: none;}\r\n        \t.version {color: gray;}\r\n        \t.error {margin-bottom: 10px;}\r\n        \t.expandable { text-decoration:underline; font-weight:bold; color:navy; cursor:hand; }\r\n        </style>\r\n";

        public static string VersionString = typeof(Server).Assembly.GetName().Version.ToString();

        public static string FormatDirectoryListing(string dirPath,
                                                    string parentPath,
                                                    FileSystemInfo[] elements)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(
                string.Format("<html>\r\n    <head>\r\n    <title>Directory Listing -- {0}</title>\r\n",
                              dirPath));
            builder.Append(
                "        <style>\r\n        \tbody {font-family:\"Verdana\";font-weight:normal;font-size: 8pt;color:black;} \r\n        \tp {font-family:\"Verdana\";font-weight:normal;color:black;margin-top: -5px}\r\n        \tb {font-family:\"Verdana\";font-weight:bold;color:black;margin-top: -5px}\r\n        \th1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red }\r\n        \th2 { font-family:\"Verdana\";font-weight:normal;font-size:14pt;color:maroon }\r\n        \tpre {font-family:\"Lucida Console\";font-size: 8pt}\r\n        \t.marker {font-weight: bold; color: black;text-decoration: none;}\r\n        \t.version {color: gray;}\r\n        \t.error {margin-bottom: 10px;}\r\n        \t.expandable { text-decoration:underline; font-weight:bold; color:navy; cursor:hand; }\r\n        </style>\r\n");
            builder.Append(
                string.Format(
                    "    </head>\r\n    <body bgcolor=\"white\">\r\n\r\n    <h2> <i>Directory Listing -- {0}</i> </h2></span>\r\n\r\n            <hr width=100% size=1 color=silver>\r\n\r\n<PRE>\r\n",
                    dirPath));
            if (parentPath != null)
            {
                if (!parentPath.EndsWith("/"))
                {
                    parentPath = parentPath + "/";
                }
                builder.Append(string.Format("<A href=\"{0}\">[To Parent Directory]</A>\r\n\r\n", parentPath));
            }
            if (elements != null)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] is FileInfo)
                    {
                        FileInfo info = (FileInfo)elements[i];
                        builder.Append(
                            string.Format(
                                "{0,38:dddd, MMMM dd, yyyy hh:mm tt} {1,12:n0} <A href=\"{2}\">{3}</A>\r\n",
                                new object[] { info.LastWriteTime, info.Length, info.Name, info.Name }));
                    }
                    else if (elements[i] is DirectoryInfo)
                    {
                        DirectoryInfo info2 = (DirectoryInfo)elements[i];
                        builder.Append(
                            string.Format(
                                "{0,38:dddd, MMMM dd, yyyy hh:mm tt}        &lt;dir&gt; <A href=\"{1}/\">{2}</A>\r\n",
                                info2.LastWriteTime,
                                info2.Name,
                                info2.Name));
                    }
                }
            }
            builder.Append(_dirListingTail);
            return builder.ToString();
        }

        public static string FormatErrorMessageBody(int statusCode, string appName)
        {
            string statusDescription = HttpWorkerRequest.GetStatusDescription(statusCode);
            return
                (string.Format("<html><head><title>{0}</title>", statusDescription) +
                 "<style>body {font-family:\"Verdana\";font-weight:normal;font-size: 8pt;color:black;}p {font-family:\"Verdana\";font-weight:normal;color:black;margin-top: -5px}b {font-family:\"Verdana\";font-weight:bold;color:black;margin-top: -5px}h1 { font-family:\"Verdana\";font-weight:normal;font-size:18pt;color:red }h2 { font-family:\"Verdana\";font-weight:normal;font-size:14pt;color:maroon }pre {font-family:\"Lucida Console\";font-size: 8pt}.marker {font-weight: bold; color: black;text-decoration: none;}.version {color: gray;}.error {margin-bottom: 10px;}.expandable { text-decoration:underline; font-weight:bold; color:navy; cursor:hand; }</style>" +
                 string.Format(_httpErrorFormat2, appName, statusCode, statusDescription));
        }
    }
}