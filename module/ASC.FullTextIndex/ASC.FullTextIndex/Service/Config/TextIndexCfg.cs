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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using ASC.Notify.Cron;

namespace ASC.FullTextIndex.Service.Config
{
    class TextIndexCfg
    {
        private readonly string indexPath;


        public readonly static int MaxQueryLength = 30;


        public CronExpression ChangedCron
        {
            get;
            private set;
        }

        public CronExpression RemovedCron
        {
            get;
            private set;
        }

        public IList<ModuleInfo> Modules
        {
            get;
            private set;
        }

        public string ConnectionStringName
        {
            get;
            private set;
        }

        public int UserActivityDays
        {
            get;
            private set;
        }


        public TextIndexCfg()
        {
            var cfg = (TextIndexCfgSectionHandler)ConfigurationManager.GetSection("fullTextIndex");

            indexPath = cfg.IndexPath.Trim('\\').Trim();
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(indexPath))
            {
                indexPath = currentDirectory;
            }
            if (!Path.IsPathRooted(indexPath))
            {
                indexPath = Path.Combine(currentDirectory, indexPath);
            }

            ChangedCron = new CronExpression(cfg.ChangedCron);
            RemovedCron = new CronExpression(cfg.RemovedCron);
            ConnectionStringName = cfg.ConnectionStringName;
            UserActivityDays = cfg.UserActivityDays;
            Modules = cfg.Modules
                .Cast<TextIndexCfgModuleElement>()
                .Select(e => new ModuleInfo(e.Name, e.Select, e.ConnectionStringName ?? ConnectionStringName))
                .ToList();
        }


        public string GetIndexPath(int tenantId, string module)
        {
            var path = Path.Combine(indexPath, tenantId.ToString("00/00/00", CultureInfo.InvariantCulture));
            path = Path.Combine(path, module);
            return path;
        }
    }
}
