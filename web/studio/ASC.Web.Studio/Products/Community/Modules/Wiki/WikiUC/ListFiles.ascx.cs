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
using System.Web.UI.WebControls;

namespace ASC.Web.UserControls.Wiki.UC
{
    public partial class ListFiles : BaseUserControl
    {
        public string mainPath
        {
            get
            {
                if (ViewState["mainPath"] == null)
                    return string.Empty;
                else
                    return ViewState["mainPath"].ToString();
            }
            set { ViewState["mainPath"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindListFiles();
            }
        }

        protected void cmdDeleteFile_Click(object sender, EventArgs e)
        {
            string fileName = (sender as LinkButton).CommandName;
            Wiki.RemoveFile(fileName);
            BindListFiles();
        }

        protected string GetFileLink(string FileName)
        {
            return ActionHelper.GetViewFilePath(mainPath, FileName);
        }


        private void BindListFiles()
        {
            rptListFiles.DataSource = Wiki.GetFiles();
            rptListFiles.DataBind();
        }     
    }
}