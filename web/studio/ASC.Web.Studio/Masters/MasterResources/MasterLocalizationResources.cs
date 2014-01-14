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
using System.Globalization;
using System.Threading;
using System.Web;
using ASC.Core;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.PublicResources;
using Resources;

namespace ASC.Web.Studio.Masters.MasterResources
{
    public class MasterLocalizationResources : ClientScriptLocalization
    {
        private static string GetDatepikerDateFormat(string s)
        {
            return s
                .Replace("yyyy", "yy")
                .Replace("yy", "yy")
                .Replace("MMMM", "MM")
                .Replace("MMM", "M")
                .Replace("MM", "mm")
                .Replace("M", "mm")
                .Replace("dddd", "DD")
                .Replace("ddd", "D")
                .Replace("dd", "11")
                .Replace("d", "dd")
                .Replace("11", "dd")
                ;
        }

        protected override string BaseNamespace
        {
            get { return "ASC.Resources.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterResourceSet("Resource", ResourceJS.ResourceManager);
            yield return RegisterResourceSet("FeedResource", FeedResource.ResourceManager);

            yield return RegisterObject("TimezoneDisplayName", CoreContext.TenantManager.GetCurrentTenant().TimeZone.DisplayName);
            yield return RegisterObject("TimezoneOffsetMinutes", CoreContext.TenantManager.GetCurrentTenant().TimeZone.GetUtcOffset(DateTime.UtcNow).TotalMinutes);

            var dateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            yield return RegisterObject("DatePattern", dateTimeFormat.ShortDatePattern);
            yield return RegisterObject("TimePattern", dateTimeFormat.ShortTimePattern);
            yield return RegisterObject("DateTimePattern", dateTimeFormat.FullDateTimePattern);
            yield return RegisterObject("DatePatternJQ", DateTimeExtension.DateMaskForJQuery); //.Replace(" ", string.Empty) -  remove because, crash date in datepicker on czech language (bug 21954)

            yield return RegisterObject("DatepickerDatePattern", GetDatepikerDateFormat(dateTimeFormat.ShortDatePattern));
            yield return RegisterObject("DatepickerTimePattern", GetDatepikerDateFormat(dateTimeFormat.ShortTimePattern));
            yield return RegisterObject("DatepickerDateTimePattern", GetDatepikerDateFormat(dateTimeFormat.FullDateTimePattern));

            yield return RegisterObject("FirstDay", (int)dateTimeFormat.FirstDayOfWeek);
            yield return RegisterObject("DayNames", dateTimeFormat.AbbreviatedDayNames);
            yield return RegisterObject("DayNamesFull", dateTimeFormat.DayNames);
            yield return RegisterObject("MonthNames", Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AbbreviatedMonthGenitiveNames);
            yield return RegisterObject("MonthNamesFull", Thread.CurrentThread.CurrentUICulture.DateTimeFormat.MonthNames);

            yield return RegisterObject("TwoLetterISOLanguageName", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            yield return RegisterObject("CurrentCultureName", Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant());
            yield return RegisterObject("CurrentCulture", CultureInfo.CurrentCulture.Name);

            yield return RegisterObject("ErrorFileSizeLimit", FileSizeComment.FileSizeExceptionString);
            yield return RegisterObject("FileSizePostfix", Resource.FileSizePostfix);
        }
    }

    public class MasterCustomResources : ClientScriptCustom
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Resources.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("User", CustomNamingPeople.Substitute<Resource>("User"));
            yield return RegisterObject("Guest", CustomNamingPeople.Substitute<Resource>("Guest"));
            yield return RegisterObject("Department", CustomNamingPeople.Substitute<Resource>("Department"));
            yield return RegisterObject("ConfirmRemoveUser", CustomNamingPeople.Substitute<Resource>("ConfirmRemoveUser").HtmlEncode());
            yield return RegisterObject("ConfirmRemoveDepartment", CustomNamingPeople.Substitute<Resource>("DeleteDepartmentConfirmation").HtmlEncode());
            yield return RegisterObject("AddDepartmentHeader", CustomNamingPeople.Substitute<Resource>("AddDepartmentDlgTitle").HtmlEncode());
            yield return RegisterObject("EditDepartmentHeader", CustomNamingPeople.Substitute<Resource>("DepEditHeader").HtmlEncode());
            yield return RegisterObject("EmployeeAllDepartments", CustomNamingPeople.Substitute<Resource>("EmployeeAllDepartments").HtmlEncode());
            yield return RegisterObject("AddEmployees", Studio.Core.Users.CustomNamingPeople.Substitute<UserControlsCommonResource>("AddEmployees").HtmlEncode());
 
        }
    }
}