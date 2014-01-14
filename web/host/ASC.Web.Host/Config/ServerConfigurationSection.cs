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

namespace ASC.Web.Host.Config
{
    class ServerSection : ConfigurationSection
    {
        [ConfigurationProperty("authSchemes", DefaultValue = AuthenticationSchemes.Anonymous)]
        public AuthenticationSchemes AuthenticationSchemes
        {
            get { return (AuthenticationSchemes)base["authSchemes"]; }
        }

        [ConfigurationProperty("defaultFileNames", DefaultValue = "default.aspx,default.htm,default.html,index.htm,index.html")]
        public string DefaultFileNames
        {
            get { return (string)base["defaultFileNames"]; }
        }

        [ConfigurationProperty("restrictedDirs", DefaultValue = "/bin,/_private_folder,/app_browsers,/app_code,/app_data,/app_localresources,/app_globalresources,/app_webreferences")]
        public string RestrictedDirs
        {
            get { return (string)base["restrictedDirs"]; }
        }

        [ConfigurationProperty("httpHandlers")]
        public HttpHandlerElementCollection HttpHandlers
        {
            get { return (HttpHandlerElementCollection)base["httpHandlers"]; }
        }

        [ConfigurationProperty("bufferSize", DefaultValue = 65536)]
        public int BufferSize
        {
            get { return (int)base["bufferSize"]; }
        }

        [ConfigurationProperty("hackWCFBinding", DefaultValue = true)]
        public bool HackWCFBinding
        {
            get { return (bool)base["hackWCFBinding"]; }
        }

        [ConfigurationProperty("sites")]
        [ConfigurationCollection(typeof(SiteElementCollection), AddItemName = "site")]
        public SiteElementCollection Sites
        {
            get { return (SiteElementCollection)base["sites"]; }
        }
    }

    class HttpHandlerElement : ConfigurationElement
    {

        [ConfigurationProperty("extension", IsRequired = true, IsKey = true)]
        public string Extension
        {
            get { return (string)base["extension"]; }
            set { base["extension"] = value; }
        }

        [ConfigurationProperty("handler", IsRequired = true)]
        public string HandlerType
        {
            get { return (string)base["handler"]; }
            set { base["handler"] = value; }
        }
    }

    class HttpHandlerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new HttpHandlerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HttpHandlerElement)element).Extension;
        }

        public void Add(string extension, Type handlerType)
        {
            base.BaseAdd(new HttpHandlerElement { Extension = extension, HandlerType = handlerType.FullName });
        }
    }

    class SiteElement : ConfigurationElement
    {

        [ConfigurationProperty("binding", IsRequired = true, IsKey = true)]
        public string Binding
        {
            get { return (string)base["binding"]; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)base["path"]; }
        }
    }

    class SiteElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteElement)element).Binding;
        }
    }
}