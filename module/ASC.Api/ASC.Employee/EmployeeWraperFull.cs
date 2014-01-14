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
using System.Runtime.Serialization;
using ASC.Api.Impl;
using ASC.Core.Users;
using ASC.Specific;
using ASC.Web.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "person", Namespace = "")]
    public class EmployeeWraperFull : EmployeeWraper
    {
        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string FirstName { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string LastName { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 2)]
        public string UserName { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public string Email { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        protected List<Contact> Contacts { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public ApiDateTime Birthday { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Sex { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public EmployeeStatus Status { get; set; }

        [DataMember(Order = 10)]
        public EmployeeActivationStatus ActivationStatus { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10)]
        public ApiDateTime Terminated { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Department { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public ApiDateTime WorkFrom { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<GroupWrapperSummary> Groups { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Location { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Notes { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 20)]
        public string AvatarMedium { get; set; }

        [DataMember(Order = 20)]
        public string Avatar { get; set; }

        [DataMember(Order = 20)]
        public bool IsOnline { get; set; }

        [DataMember(Order = 20)]
        public bool IsAdmin { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<string> ListAdminModules { get; set; }

        [DataMember(Order = 20)]
        public bool IsOwner { get; set; }

        [DataMember(Order = 2)]
        public bool IsVisitor { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CultureName { get; set; }


        [DataMember(Order = 11, EmitDefaultValue = false)]
        protected String MobilePhone { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        protected MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }

        ///<summary>
        ///</summary>
        public EmployeeWraperFull()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="userInfo"></param>
        ///<exception cref="ArgumentException"></exception>
        public EmployeeWraperFull(UserInfo userInfo) : this(userInfo, null)
        {
           
        }

        public EmployeeWraperFull(UserInfo userInfo, ApiContext context)
            : base(userInfo)
        {
            UserName = userInfo.UserName;
            IsVisitor = userInfo.IsVisitor();
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Birthday = (ApiDateTime)userInfo.BirthDate;

            if (userInfo.Sex.HasValue)
                Sex = userInfo.Sex.Value ? "male" : "female";

            Status = userInfo.Status;
            ActivationStatus = userInfo.ActivationStatus;
            Terminated = (ApiDateTime)userInfo.TerminatedDate;

            if (!string.IsNullOrEmpty(userInfo.Department))
                Department = userInfo.Department;

            WorkFrom = (ApiDateTime)userInfo.WorkFromDate;
            Email = userInfo.Email;

            if (!string.IsNullOrEmpty(userInfo.Location))
                Location = userInfo.Location;

            if (!string.IsNullOrEmpty(userInfo.Notes))
                Notes = userInfo.Notes;

            if (!string.IsNullOrEmpty(userInfo.MobilePhone))
                MobilePhone = userInfo.MobilePhone;

            MobilePhoneActivationStatus = userInfo.MobilePhoneActivationStatus;

            if (!string.IsNullOrEmpty(userInfo.CultureName))
                CultureName = userInfo.CultureName;

            FillConacts(userInfo);

            var groups = Core.CoreContext.UserManager.GetUserGroups(userInfo.ID).Select(x => new GroupWrapperSummary(x)).ToList();

            if (groups.Any())
                Groups = groups;

            try
            {
                if (CheckContext(context, "avatarSmall"))
                    AvatarSmall = UserPhotoManager.GetSmallPhotoURL(userInfo.ID);

                if (CheckContext(context, "avatarMedium"))
                    AvatarMedium = UserPhotoManager.GetMediumPhotoURL(userInfo.ID);

                if (CheckContext(context, "avatar"))
                    Avatar = UserPhotoManager.GetBigPhotoURL(userInfo.ID);
            }
            catch (Exception)
            {
            }

            try
            {
                IsOnline = false;
                IsAdmin = userInfo.IsAdmin();

                if (CheckContext(context, "listAdminModules"))
                {
                    var listAdminModules = userInfo.GetListAdminModules();

                    if (listAdminModules.Any())
                        ListAdminModules = listAdminModules;
                }

                IsOwner = userInfo.IsOwner();
            }
            catch (Exception)
            {
            }
        }

        private void FillConacts(UserInfo userInfo)
        {
            var contacts = new List<Contact>();
            for (var i = 0; i < userInfo.Contacts.Count; i += 2)
            {
                if (i + 1 < userInfo.Contacts.Count)
                {
                    contacts.Add(new Contact(userInfo.Contacts[i], userInfo.Contacts[i + 1]));
                }
            }

            if (contacts.Any())
            {
                Contacts = contacts;
            }
        }

        private static bool CheckContext(ApiContext context, string field)
        {
            return context == null || context.Fields == null ||
                   (context.Fields != null && context.Fields.Contains(field));
        }

        public new static EmployeeWraperFull GetSample()
        {
            return new EmployeeWraperFull
                {
                    Avatar = "url to big avatar",
                    AvatarSmall = "url to small avatar",
                    Contacts = new List<Contact> {new Contact("GTalk", "mike@gmail.com")},
                    Email = "mike@gmail.com",
                    FirstName = "Mike",
                    Id = Guid.NewGuid(),
                    IsAdmin = false,
                    ListAdminModules = new List<string> {"projects", "crm"},
                    UserName = "Mike.Zanyatski",
                    LastName = "Zanyatski",
                    Title = "Manager",
                    Groups = new List<GroupWrapperSummary> {GroupWrapperSummary.GetSample()},
                    AvatarMedium = "url to medium avatar",
                    Birthday = new ApiDateTime(new DateTime(1917, 11, 7)),
                    Department = "Marketing",
                    Location = "Palo Alto",
                    Notes = "Notes to worker",
                    Sex = "male",
                    Status = EmployeeStatus.Active,
                    WorkFrom = new ApiDateTime(new DateTime(1945, 5, 9)),
                    Terminated = new ApiDateTime(new DateTime(2029, 12, 12)),
                    CultureName = "en-EN"
                };
        }
    }
}