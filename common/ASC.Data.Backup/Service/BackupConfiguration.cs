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
using System.Linq;

namespace ASC.Data.Backup.Service
{
    public class BackupConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("tmpFolder", DefaultValue = "..\\Data\\Backup\\")]
        public string TmpFolder
        {
            get { return (string)this["tmpFolder"]; }
            set { this["tmpFolder"] = value; }
        }

        [ConfigurationProperty("expire", DefaultValue = "1.0:0:0")]
        public TimeSpan ExpirePeriod
        {
            get { return (TimeSpan)this["expire"]; }
            set { this["expire"] = value; }
        }

        [ConfigurationProperty("threads", DefaultValue = 4)]
        public int ThreadCount
        {
            get { return (int)this["threads"]; }
            set { this["threads"] = value; }
        }

        [ConfigurationProperty("webConfigs")]
        public WebConfigCollection RegionConfigs
        {
            get { return (WebConfigCollection)this["webConfigs"]; }
            set { this["webConfigs"] = value; }
        }

        [ConfigurationProperty("demo")]
        public DemoPortalCollection DemoPortals
        {
            get { return (DemoPortalCollection)this["demo"]; }
            set { this["demo"] = value; }
        }

        public static BackupConfigurationSection GetSection()
        {
            return (BackupConfigurationSection)ConfigurationManager.GetSection("backup");
        }
    }

    public class WebConfigCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("current")]
        public string CurrentRegion
        {
            get { return (string)this["current"]; }
            set { this["current"] = value; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new WebConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebConfigElement)element).Region;
        }

        public WebConfigElement GetConfig(string region)
        {
            return BaseGet(region) as WebConfigElement;
        }

        public string GetCurrentConfig()
        {
            if (Count == 0)
            {
                return "Backup";
            }
            if (Count == 1)
            {
                return this.Cast<WebConfigElement>().First().Path;
            }
            return GetConfig(CurrentRegion).Path;
        }
    }

    public class WebConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("region", IsRequired = true, IsKey = true)]
        public string Region
        {
            get { return (string)this["region"]; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
        }
    }

    public class DemoPortalCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("default", DefaultValue = "en-US")]
        public string DefaultID
        {
            get { return (string)this["default"]; }
            set { this["default"] = value; }
        }

        public DemoPortalElement Default
        {
            get { return !string.IsNullOrEmpty(DefaultID) ? GetDemoPortal(DefaultID) : null; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DemoPortalElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DemoPortalElement)element).ID;
        }

        public DemoPortalElement GetDemoPortal(string name)
        {
            return BaseGet(name) as DemoPortalElement;
        }
    }

    public class DemoPortalElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string ID
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string DataPath
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}
