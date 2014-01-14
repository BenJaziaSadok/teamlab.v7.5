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

namespace ASC.Web.UserControls.Wiki.Handlers
{
    public class WikiSection : ConfigurationSection
    {
        private static readonly WikiSection defaultSection = new WikiSection();


        public static WikiSection Section
        {
            get
            {
                return (ConfigurationManager.GetSection("wikiSettings") as WikiSection ?? defaultSection);
            }
        }

        [ConfigurationProperty("dbase")]
        public DBElement DB
        {
            get
            {
                return (DBElement)this["dbase"];
            }
            set
            {
                this["dbase"] = value;
            }
        }

        [ConfigurationProperty("fckeditorInfo")]
        public FckeditorInfoElement FckeditorInfo
        {
            get
            {
                return (FckeditorInfoElement)this["fckeditorInfo"];
            }
            set
            {
                this["fckeditorInfo"] = value;
            }
        }

        [ConfigurationProperty("dataStorage")]
        public DataStorageElement DataStorage
        {
            get
            {
                return (DataStorageElement)this["dataStorage"];
            }
            set
            {
                this["dataStorage"] = value;
            }
        }

        [ConfigurationProperty("imageHangler")]
        public ImageHanglerElement ImageHangler
        {
            get
            {
                return (ImageHanglerElement)this["imageHangler"];
            }
            set
            {
                this["imageHangler"] = value;
            }
        }

    }

    public class MainPageElement : ConfigurationElement
    {
        [ConfigurationProperty("url", DefaultValue = "", IsRequired = true)]
        public string Url
        {
            get
            {
                return (String)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

        [ConfigurationProperty("wikiView", DefaultValue = "", IsRequired = true)]
        public string WikiView
        {
            get
            {
                return (string)this["wikiView"];
            }
            set
            {
                this["wikiView"] = value;
            }
        }

        [ConfigurationProperty("wikiEdit", DefaultValue = "", IsRequired = true)]
        public string WikiEdit
        {
            get
            {
                return (string)this["wikiEdit"];
            }
            set
            {
                this["wikiEdit"] = value;
            }
        }
    }

    public class DBElement : ConfigurationElement
    {
        [ConfigurationProperty("connectionStringName", DefaultValue = "community", IsRequired = true)]
        public string ConnectionStringName
        {
            get
            {
                return (string)this["connectionStringName"];
            }
            set
            {
                this["connectionStringName"] = value;
            }
        }
    }

    public class FileStorageElement : ConfigurationElement
    {
        [ConfigurationProperty("location", DefaultValue = "", IsRequired = true)]
        public string Location
        {
            get
            {
                return (string)this["location"];
            }
            set
            {
                this["location"] = value;
            }
        }
    }

    public class DataStorageElement : ConfigurationElement
    {
        [ConfigurationProperty("moduleName", DefaultValue = "wiki", IsRequired = true)]
        public string ModuleName
        {
            get
            {
                return (string)this["moduleName"];
            }
            set
            {
                this["moduleName"] = value;
            }
        }

        [ConfigurationProperty("defaultDomain", DefaultValue = "", IsRequired = true)]
        public string DefaultDomain
        {
            get
            {
                return (string)this["defaultDomain"];
            }
            set
            {
                this["defaultDomain"] = value;
            }
        }

        [ConfigurationProperty("tempDomain", DefaultValue = "temp", IsRequired = true)]
        public string TempDomain
        {
            get
            {
                return (string)this["tempDomain"];
            }
            set
            {
                this["tempDomain"] = value;
            }
        }
    }

    public class ImageHanglerElement : ConfigurationElement
    {
        [ConfigurationProperty("urlFormat", DefaultValue = "~/Products/Community/Modules/Wiki/WikiFile.ashx?file={0}", IsRequired = true)]
        public string UrlFormat
        {
            get
            {
                return (string)this["urlFormat"];
            }
            set
            {
                this["urlFormat"] = value;
            }
        }
    }

    public class FckeditorInfoElement : ConfigurationElement
    {
        [ConfigurationProperty("pathFrom", DefaultValue = "../../../../../Products/Community/Modules/Wiki/WikiUC/", IsRequired = true)]
        public string PathFrom
        {
            get
            {
                return (string)this["pathFrom"];
            }
            set
            {
                this["pathFrom"] = value;
            }
        }

        [ConfigurationProperty("baseRelPath", DefaultValue = "~/UserControls/Common/fckeditor/", IsRequired = true)]
        public string BaseRelPath
        {
            get
            {
                return (string)this["baseRelPath"];
            }
            set
            {
                this["baseRelPath"] = value;
            }
        }

    }



}
