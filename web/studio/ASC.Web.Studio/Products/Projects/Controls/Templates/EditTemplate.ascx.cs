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
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Helpers;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Controls.Templates
{
    public partial class EditTemplate : BaseUserControl
    {
        private int projectTmplId;
        public Template Templ { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.Participant.IsAdmin || Page.Participant.IsVisitor)
                HttpContext.Current.Response.Redirect(PathProvider.BaseVirtualPath, true);

            if (Int32.TryParse(UrlParameters.EntityID, out projectTmplId))
            {
                Templ = Global.EngineFactory.GetTemplateEngine().GetByID(projectTmplId);
                Page.Master.JsonPublisher(Templ, "template");
            }

            var title = projectTmplId == 0 ? ProjectTemplatesResource.CreateProjTmpl : ProjectTemplatesResource.EditProjTmpl;

            Page.Title = HeaderStringHelper.GetPageTitle(title);

            _attantion.Options.IsPopup = true;
        }

        protected string GetPageTitle()
        {
            return projectTmplId == 0 ? ProjectTemplatesResource.CreateProjTmpl : ProjectTemplatesResource.EditProjTmpl;
        }

        protected string ChooseMonthNumeralCase(double count)
        {
            return count == 0 ? string.Empty : count + " " + ChooseNumeralCase(count, GrammaticalResource.MonthNominative,
                GrammaticalResource.MonthGenitiveSingular, GrammaticalResource.MonthGenitivePlural);
        }
        protected static string ChooseNumeralCase(double number, string nominative, string genitiveSingular, string genitivePlural)
        {
            if (number == 0.5)
            {
                if (
                    String.Compare(
                        System.Threading.Thread.CurrentThread.CurrentUICulture.ThreeLetterISOLanguageName,
                        "rus", true) == 0)
                {
                    return genitiveSingular;
                }
            }
            if (number == 1)
            {
                return nominative;
            }
            var count = (int)number;
            return GrammaticalHelper.ChooseNumeralCase(count, nominative, genitiveSingular, genitivePlural);
        }
    }
}