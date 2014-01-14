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
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class ServerError : MainPage
    {
        protected string ErrorCaption = Resources.Resource.ServerErrorTitle;
        protected string ErrorText = Resources.Resource.ServerErrorText;

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.TopStudioPanel.DisableProductNavigation = true;
            Master.TopStudioPanel.DisableSearch = true;
            Master.TopStudioPanel.DisableVideo = true;


            var errorCode = Request["code"];
            if (!string.IsNullOrEmpty(errorCode))
            {
                switch (Convert.ToInt32(errorCode))
                {
                    case 403:
                        ErrorCaption = Resources.Resource.Error403Title;
                        ErrorText = Resources.Resource.Error403Text;
                        break;
                    case 404:
                        Response.Redirect("~/error404.aspx");
                        return;
                }
            }

            Title = HeaderStringHelper.GetPageTitle(ErrorCaption);
        }
    }
}