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

using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.Core.Users
{
    public sealed class MyUserProfile
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GroupId { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public bool? Sex { get; set; }
        public DateTime? RegDate { get; set; }
        public string RegDateValue { get; set; }
        public DateTime? BirthDate { get; set; }
        public string BirthDateValue { get; set; }
        public string Place { get; set; }
        public string Comment { get; set; }
        public EmployeeStatus Status { get; set; }
        public List<MyContact> Phones { get; set; }
        public List<MyContact> Emails { get; set; }
        public List<MyContact> Messengers { get; set; }
        public List<MyContact> Contacts { get; set; }

        private readonly UserInfo userInfo;
        private string avatar;
        public string Avatar { 
            get
            {
                if (string.IsNullOrEmpty(avatar))
                    avatar = userInfo.GetPhotoURL();
                return avatar;
            }
        }

        private static MyContact GetSocialContact(String type, String value)
        {
            var node = SocialContactsManager.xmlSocialContacts.GetElementById(type);
            var title = node != null ? node.Attributes["title"].Value : type;
            var template = node != null ? node.GetElementsByTagName("template")[0].InnerXml : "{0}";

            return new MyContact
                {
                type = type,
                classname = type,
                label = title,
                text = HttpUtility.HtmlEncode(value),
                link = String.Format(template, HttpUtility.HtmlEncode(value))
            };
        }

        private static List<MyContact> GetContacts(List<string> userInfoContacts)
        {
            var contacts = new List<MyContact>();
            for (int i = 0, n = userInfoContacts.Count; i < n; i += 2)
            {
                if (i + 1 < userInfoContacts.Count)
                {
                    contacts.Add(GetSocialContact(userInfoContacts[i], userInfoContacts[i + 1]));
                }
            }
            return contacts;
        }

        private static List<MyContact> GetPhones(List<string> userInfoContacts)
        {
            var contacts = GetContacts(userInfoContacts);
            var phones = new List<MyContact>();

            for (int i = 0, n = contacts.Count; i < n; i++)
            {
                switch (contacts[i].type)
                {
                    case "phone":
                    case "mobphone":
                        phones.Add(contacts[i]);
                        break;
                }
            }

            return phones;
        }

        private static List<MyContact> GetEmails(List<string> userInfoContacts)
        {
            var contacts = GetContacts(userInfoContacts);
            var emails = new List<MyContact>();

            for (int i = 0, n = contacts.Count; i < n; i++)
            {
                switch (contacts[i].type)
                {
                    case "mail":
                    case "gmail":
                        emails.Add(contacts[i]);
                        break;
                }
            }

            return emails;
        }

        private static List<MyContact> GetMessengers(List<string> userInfoContacts)
        {
            var contacts = GetContacts(userInfoContacts);
            var messengers = new List<MyContact>();

            for (int i = 0, n = contacts.Count; i < n; i++)
            {
                switch (contacts[i].type)
                {
                    case "jabber":
                    case "skype":
                    case "msn":
                    case "aim":
                    case "icq":
                    case "gtalk":
                        messengers.Add(contacts[i]);
                        break;
                }
            }

            return messengers;
        }

        private static List<MyContact> GetSocialContacts(List<string> userInfoContacts)
        {
            var contacts = GetContacts(userInfoContacts);
            var soccontacts = new List<MyContact>();

            for (int i = 0, n = contacts.Count; i < n; i++)
            {
                switch (contacts[i].type)
                {
                    case "mail":
                    case "gmail":

                    case "phone":
                    case "mobphone":

                    case "jabber":
                    case "skype":
                    case "msn":
                    case "aim":
                    case "icq":
                    case "gtalk":
                        continue;
                }
                soccontacts.Add(contacts[i]);
            }

            return soccontacts;
        }

        public MyUserProfile(Guid id)
        {
            Id = id;

            var userGroups = CoreContext.UserManager.GetUserGroups(Id);
            userInfo = CoreContext.UserManager.GetUsers(Id);

            UserName = userInfo.UserName;
            DisplayName = userInfo.DisplayUserName(true);
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            GroupId = userGroups.Any() ? userGroups.First().ID.ToString() : String.Empty;
            Group = HttpUtility.HtmlEncode(userInfo.Department);
            Title = HttpUtility.HtmlEncode(userInfo.Title);
            Email = HttpUtility.HtmlEncode(userInfo.Email);
            Phone = HttpUtility.HtmlEncode(userInfo.MobilePhone);
            Gender = userInfo.Sex != null ? userInfo.Sex == true ? Resources.Resource.MaleSexStatus : Resources.Resource.FemaleSexStatus : String.Empty;
            Sex = userInfo.Sex;
            RegDate = userInfo.WorkFromDate;
            RegDateValue = userInfo.WorkFromDate.HasValue ? userInfo.WorkFromDate.Value.ToShortDateString() : String.Empty;
            BirthDate = userInfo.BirthDate;
            BirthDateValue = userInfo.BirthDate.HasValue ? userInfo.BirthDate.Value.ToShortDateString() : String.Empty;
            Place = HttpUtility.HtmlEncode(userInfo.Location);
            Comment = HttpUtility.HtmlEncode(userInfo.Notes);
            Status = userInfo.Status;

            Phones = GetPhones(userInfo.Contacts);
            Emails = GetEmails(userInfo.Contacts);
            Messengers = GetMessengers(userInfo.Contacts);
            Contacts = GetSocialContacts(userInfo.Contacts);            
        }

    }
}
