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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using AjaxPro;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility;
using System.Collections;
using dotless.Core.Parser.Tree;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("ColorThemeController")] 
    public partial class ColorThemes : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/ColorThemes/ColorThemes.ascx"; }
        }
        protected List<PortalColorTheme> ColorThemesList { get; set; }

        protected string ChosenTheme { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/ColorThemes/js/colorthemes.js"));
            Page.RegisterStyleControl(ResolveUrl("~/usercontrols/management/ColorThemes/css/colorthemes.css"));

            ChosenTheme = ColorThemesSettings.GetColorThemesSettings();

            ColorThemesList = new List<PortalColorTheme>(){
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemeDefault,
                    Value = "default"
                },
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemePureOrange,
                    Value = "pure-orange"
                },
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemeDarkGreen,
                    Value = "dark-green"
                },
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemeDeepBlue,
                    Value = "deep-blue"
                },
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemeWildPink,
                    Value = "wild-pink"
                },
                new PortalColorTheme
                {
                    Title = Resources.Resource.ColorThemeBrightBlue,
                    Value = "bright-blue"
                }
            };
        }

        [AjaxMethod]
        public void SaveColorTheme(string theme)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            ColorThemesSettings.SaveColorTheme(theme);
        }

        public class PortalColorTheme
        {
            public string Value { get; set; }
            public string Title { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}