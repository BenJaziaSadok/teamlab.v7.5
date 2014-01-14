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
using System.Web;
using ASC.Web.Core.Utility;
using System.IO;
using ASC.Forum;
using System.Collections.Generic;

namespace ASC.Web.UserControls.Forum.Common
{
    public class Settings : IDisposable
    {
        public ForumManager ForumManager { get; internal set; }

        public LinkProvider LinkProvider { get; private set; }

        public Guid ID { get; set; }

        public Guid ModuleID { get; set; }

        public Guid ProductID { get; set; }

        public Guid ImageItemID { get; set; }

        public int TopicCountOnPage { get; set; }

        public int PostCountOnPage { get; set; }

        public string ConfigPath { get; set; }

        public string UserControlsVirtualPath { get; set; }

        public string FileStoreModuleID { get; set; }

        public string ThreadParamName{get;set;}

        public string TopicParamName { get; set; }

        public string TagParamName { get; set; }

        public string ActionParamName { get; set; }
        public string PostParamName { get; set; }

        public string NewPostPageVirtualPath { get; set; }
        public string NewPostPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.NewPostPageVirtualPath);
            }
        }

        public string SearchPageVirtualPath { get; set; }
        public string SearchPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.SearchPageVirtualPath);
            }
        }
        
        public string StartPageVirtualPath { get; set; }
        public string StartPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.StartPageVirtualPath);
            }
        }

        public string TopicPageVirtualPath { get; set; }
        public string TopicPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.TopicPageVirtualPath);
            }
        }

        public string EditTopicPageVirtualPath { get; set; }
        public string EditTopicPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.EditTopicPageVirtualPath);
            }
        }

        public string PostPageVirtualPath { get; set; }
        public string PostPageAbsolutePath
        {
            get
            {
                return GetAbsolutePathWithParams(this.PostPageVirtualPath);
            }
        }

        private string GetAbsolutePathWithParams(string virtualPath)
        {
            if (!String.IsNullOrEmpty(virtualPath))
            {
                if(virtualPath.IndexOf("?")!=-1)
                    return VirtualPathUtility.ToAbsolute(virtualPath.Split('?')[0]) + "?" + virtualPath.Split('?')[1];
                else
                    return VirtualPathUtility.ToAbsolute(virtualPath) + "?";
            }

            return String.Empty;
        }

        internal string[] GetPageAdditionalParams(string url)
        {
            if (!String.IsNullOrEmpty(url) && url.IndexOf("?") != -1)
            {
                string query = url.Split('?')[1];
                if (!String.IsNullOrEmpty(query))
                {
                    List<string> result = new List<string>(); 
                    foreach (var param in query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))                    
                        result.Add(param.Split('=')[0]);

                    return result.ToArray();
                }
            }

            return new string[0];
        }

        internal string[] GetAllPageAdditionalParams()
        {
            List<string> result = new List<string>();

            result.AddRange(GetPageAdditionalParams(this.PostPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(this.TopicPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(this.SearchPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(this.SearchPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(this.NewPostPageVirtualPath));
            result.AddRange(GetPageAdditionalParams(this.EditTopicPageVirtualPath));

            return result.ToArray();
        }

        public Settings()
        {
            this.ID = Guid.NewGuid();
            this.TopicCountOnPage = 20;
            this.PostCountOnPage = 20;

            this.ThreadParamName = "f";
            this.TopicParamName = "t";
            this.TagParamName = "tag";
            this.ActionParamName = "a";
            this.PostParamName = "m";            
            this.LinkProvider = new LinkProvider(this);
            //registry
            this.ForumManager = new ForumManager(this);
            ForumManager.RegistrySettings(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            ForumManager.UnRegistrySettings(this.ID);
        }

        #endregion
    }
}
