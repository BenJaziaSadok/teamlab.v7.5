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
using System.Web.UI;
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;

namespace ASC.Web.UserControls.Wiki.UC
{
    public partial class ViewFile : BaseUserControl
    {
        private string _fileName = string.Empty;
        public string FileName
        {
            get {
                return /*PageNameUtil.Encode*/(_fileName); }
            set { _fileName = /*PageNameUtil.Decode*/(value); }
        }


        private File _fileInfo;
        protected File CurrentFile
        {
            get
            {
                if (_fileInfo == null)
                {
                    if (string.IsNullOrEmpty(FileName))
                        return null;

                    _fileInfo = Wiki.GetFile(FileName);
                }
                return _fileInfo;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if(CurrentFile != null && !string.IsNullOrEmpty(CurrentFile.FileName))
                {
                    RiseWikiPageLoaded(CurrentFile);
                    RisePublishVersionInfo(CurrentFile);
                }
            }
        }


        protected string GetFileRender()
        {
            var file = Wiki.GetFile(_fileName);
            if(file == null)
            {               
                RisePageEmptyEvent();
                return string.Empty;// "nonefile.png";
            }

            string ext = file.FileLocation.Split('.')[file.FileLocation.Split('.').Length - 1];
            if (!string.IsNullOrEmpty(ext) && !WikiFileHandler.ImageExtentions.Contains(ext.ToLower()))
            {
                return string.Format(@"<a class=""linkHeaderMedium"" href=""{0}"" title=""{1}"">{2}</a>",
                    ResolveUrl(string.Format(ImageHandlerUrlFormat, FileName)),
                    file.FileName,
                    Resources.WikiUCResource.wikiFileDownloadCaption);
            }

            return string.Format(@"<img src=""{0}"" style=""max-width:300px; max-height:200px"" />",
                ResolveUrl(string.Format(ImageHandlerUrlFormat, FileName)));                      
        }
    }
}