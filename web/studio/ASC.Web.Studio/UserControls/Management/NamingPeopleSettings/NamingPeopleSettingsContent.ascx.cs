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
using AjaxPro;
using ASC.Core;
using ASC.Core.Common.Logging;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("NamingPeopleContentController")]
    public partial class NamingPeopleSettingsContent : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/NamingPeopleSettings/NamingPeopleSettingsContent.ascx"; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/namingpeoplesettings/js/namingpeoplecontent.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/namingpeoplesettings/css/namingpeople.less"));

            var schemas = new List<object>();
            var currentSchemaId = CustomNamingPeople.Current.Id;

            foreach (var schema in CustomNamingPeople.GetSchemas())
            {
                schemas.Add(new
                {
                    Id = schema.Key,
                    Name = schema.Value,
                    Current = string.Equals(schema.Key, currentSchemaId, StringComparison.InvariantCultureIgnoreCase)
                });
            }

            namingSchemaRepeater.DataSource = schemas;
            namingSchemaRepeater.DataBind();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object GetPeopleNames(string schemaId)
        {
            var names = CustomNamingPeople.GetPeopleNames(schemaId);

            return new
            {
                names.Id,
                names.UserCaption,
                names.UsersCaption,
                names.GroupCaption,
                names.GroupsCaption,
                names.UserPostCaption,
                names.RegDateCaption,
                names.GroupHeadCaption,
                names.GuestCaption,
                names.GuestsCaption,
            };
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveNamingSettings(string schemaId)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                CustomNamingPeople.SetPeopleNames(schemaId);

                AdminLog.PostAction("Settings: saved team template settings to schema={0}", schemaId);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };

            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveCustomNamingSettings(string usrCaption, string usrsCaption, string grpCaption, string grpsCaption,
                                               string usrStatusCaption, string regDateCaption,
                                               string grpHeadCaption,
                                               string guestCaption, string guestsCaption)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                usrCaption = (usrCaption ?? "").Trim();
                usrsCaption = (usrsCaption ?? "").Trim();
                grpCaption = (grpCaption ?? "").Trim();
                grpsCaption = (grpsCaption ?? "").Trim();
                usrStatusCaption = (usrStatusCaption ?? "").Trim();
                regDateCaption = (regDateCaption ?? "").Trim();
                grpHeadCaption = (grpHeadCaption ?? "").Trim();
                guestCaption = (guestCaption ?? "").Trim();
                guestsCaption = (guestsCaption ?? "").Trim();

                if (String.IsNullOrEmpty(usrCaption)
                    || String.IsNullOrEmpty(usrsCaption)
                    || String.IsNullOrEmpty(grpCaption)
                    || String.IsNullOrEmpty(grpsCaption)
                    || String.IsNullOrEmpty(usrStatusCaption)
                    || String.IsNullOrEmpty(regDateCaption)
                    || String.IsNullOrEmpty(grpHeadCaption)
                    || String.IsNullOrEmpty(guestCaption)
                    || String.IsNullOrEmpty(guestsCaption))
                {
                    throw new Exception(Resources.Resource.ErrorEmptyFields);
                }

                var names = new PeopleNamesItem
                {
                    Id = PeopleNamesItem.CustomID,
                    UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
                    UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
                    GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
                    GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
                    UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
                    RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
                    GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
                    GuestCaption = guestCaption.Substring(0, Math.Min(30, guestCaption.Length)),
                    GuestsCaption = guestsCaption.Substring(0, Math.Min(30, guestsCaption.Length)),
                };

                CustomNamingPeople.SetPeopleNames(names);

                AdminLog.PostAction("Settings: saved team template settings to {0:Json}", names);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };

            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }
    }
}