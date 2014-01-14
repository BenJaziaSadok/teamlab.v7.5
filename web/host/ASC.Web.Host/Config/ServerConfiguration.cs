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
using System.Configuration;
using System.Net;
using ASC.Web.Host.HttpHandlers;

namespace ASC.Web.Host.Config
{
    static class ServerConfiguration
    {
        public static AuthenticationSchemes AuthenticationSchemes
        {
            get;
            private set;
        }

        public static HttpHandlerElementCollection HttpHandlers
        {
            get;
            private set;
        }

        public static int BufferSize
        {
            get;
            private set;
        }

        public static bool HackWCFBinding
        {
            get;
            private set;
        }

        public static SiteElementCollection Sites
        {
            get;
            private set;
        }

        public static void Configure()
        {
            var section = ConfigurationManager.GetSection("webServer") as ServerSection ?? new ServerSection();

            AuthenticationSchemes = section.AuthenticationSchemes;
            RequestConfiguration.DefaultFileNames = section.DefaultFileNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            RequestConfiguration.RestrictedDirs = section.RestrictedDirs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            BufferSize = section.BufferSize;
            HackWCFBinding = section.HackWCFBinding;
            Sites = section.Sites;

            if (section.HttpHandlers.Count == 0)
            {
                HttpHandlers = new HttpHandlerElementCollection();
                HttpHandlers.Add("*", typeof(StaticFileHttpHandler));
                foreach (var ext in aspHttpExtensions)
                {
                    HttpHandlers.Add(ext, typeof(AspNetHttpHandler));
                }
            }
            else
            {
                HttpHandlers = section.HttpHandlers;
            }
        }


        private static readonly string[] aspHttpExtensions = new[]
        {
            "/",
            "ad",
            "adprototype",
            "asa",
            "asax",
            "ascx",
            "ashx",
            "asmx",
            "asp",
            "aspx",
            "axd",
            "browser",
            "cd",
            "cdx",
            "cer",
            "compiled",
            "dll.config",
            "exe.config",
            "config",
            "cs",
            "csproj",
            "dd",
            "exclude",
            "jdc",
            "java",
            "jsl",
            "json",
            "ldb",
            "ldd",
            "lddprototype",
            "ldf",
            "lic",
            "licx",
            "master",
            "mdb",
            "mdf",
            "msgx",
            "refresh",
            "rem",
            "resources",
            "resx",
            "sd",
            "sdm",
            "sdmDocument",
            "shtm",
            "shtml",
            "sitemap",
            "skin",
            "soap",
            "stm",
            "svc",
            "tml",
            "vb",
            "vbproj",
            "vjsproj",
            "vsdisco",
            "xoml",
            "xsd",
            "webinfo",
            "wsdl",
            "xml",
        };
    }
}