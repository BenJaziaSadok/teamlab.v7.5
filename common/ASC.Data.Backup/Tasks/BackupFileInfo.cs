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

using System.Xml.Linq;
using ASC.Data.Backup.Extensions;

namespace ASC.Data.Backup.Tasks
{
    public class BackupFileInfo
    {
        public string Domain { get; set; }
        public string Module { get; set; }
        public string Path { get; set; }

        public BackupFileInfo()
        {
        }

        public BackupFileInfo(string domain, string module, string path)
        {
            Domain = domain;
            Module = module;
            Path = path;
        }

        public XElement ToXElement()
        {
            return new XElement("file",
                                new XElement("domain", Domain),
                                new XElement("module", Module),
                                new XElement("path", Path));
        }

        public static BackupFileInfo FromXElement(XElement el)
        {
            return new BackupFileInfo
                {
                    Domain = el.Element("domain").ValueOrDefault(),
                    Module = el.Element("module").ValueOrDefault(),
                    Path = el.Element("path").ValueOrDefault()
                };
        }
    }
}
