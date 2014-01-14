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
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.UserControls.Common;

namespace ASC.Web.Calendar.UserControls
{
    public partial class CalendarControl : UserControl
    {
        public static string Location
        {
            get { return "~/addons/calendar/usercontrols/calendarcontrol.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitScripts();

            _sharingContainer.Controls.Add(LoadControl(SharingSettings.Location));
        }

        private void InitScripts()
        {
            Page.RegisterStyleControl("~/addons/calendar/app_themes/<theme_folder>/calendar.less", true);
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/popup/css/popup.css"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/fullcalendar/css/asc-dialog/jquery-ui-1.8.14.custom.css"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/fullcalendar/css/asc-datepicker/jquery-ui-1.8.14.custom.css"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/css/jquery.jscrollpane.css"));

            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/uploader/ajaxupload.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/popup/popup.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/calendar_controller.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/recurrence_rule.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/jquery.jscrollpane.min.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/jquery.mousewheel.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/jquery.cookie.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/js/jquery.jscrollpane.min.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/fullcalendar/fullcalendar.js"));


            Page.ClientScript.RegisterClientScriptBlock(GetType(), "calendar_full_screen",
                                                        @"<style type=""text/css"">
                    #studioPageContent{padding-bottom:0px;}
                    #studio_sidePanelUpHeight20{display:none;}
                    </style>", false);

            var script = new StringBuilder();
            script.AppendFormat("ASC.CalendarController.init([{0}], '{1}', '{2}');", RenderTimeZones(), VirtualPathUtility.ToAbsolute("~/addons/calendar/usercontrols/fullcalendar/tmpl/notifications.editor.tmpl"), Studio.Core.SetupInfo.IsPersonal.ToString().ToLower());

            Page.RegisterInlineScript(script.ToString());
        }

        protected string RenderTimeZones()
        {
            var sb = new StringBuilder();
            var i = 0;
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (i > 0)
                    sb.Append(",");

                sb.AppendFormat("{{name:\"{0}\", id:\"{1}\", offset:{2}}}", tz.DisplayName, tz.Id, (int)tz.BaseUtcOffset.TotalMinutes);
                i++;
            }
            return sb.ToString();
        }
    }
}