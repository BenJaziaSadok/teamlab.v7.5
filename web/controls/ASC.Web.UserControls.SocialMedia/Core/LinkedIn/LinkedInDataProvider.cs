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
using System.Collections.ObjectModel;
using System.Linq;
using DotNetOpenAuth.OAuth.ChannelElements;
using LinkedIn;
using LinkedIn.ServiceEntities;
using LinkedIn.Utility;
using log4net;

namespace ASC.SocialMedia.LinkedIn
{
    public class LinkedInDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LinkedInDataProvider));
        private IConsumerTokenManager _tokenManager;
        private string _accessToken;

        public LinkedInDataProvider(IConsumerTokenManager tokenManager, string accessToken)
        {
            _tokenManager = tokenManager;
            _accessToken = accessToken;
        }

        public LinkedInUserInfo GetCurrentUserInfo()
        {
            try
            {
                var service = GetLinkedInService();

                Person user = service.GetCurrentUser(ProfileType.Public, GetProfileFields().ToList());
                if (user == null)
                    return null;

                return MapUser(user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public string GetUrlOfUserImage(string userId)
        {
            try
            {
                var service = GetLinkedInService(); 
                Person user = service.GetProfileByMemberId(userId + ":public");
                return user.PictureUrl;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public LinkedInUserInfo GetUserInfo(string userId)
        {
            try
            {
                var service = GetLinkedInService(); 
                Person user = service.GetProfileByMemberId(userId + ":public");
                if (user == null)
                    return null;

                return MapUser(user);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public List<Message> GetCurrentUserNetworkUpdates(int messageCount)
        {
            try
            {
                var service = GetLinkedInService();

#pragma warning disable 612
                Updates updates = service.GetNetworkUpdates(NetworkUpdateTypes.StatusUpdate, messageCount, 0, DateTime.MinValue, DateTime.MinValue);
#pragma warning restore 612

                return Map(updates);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public List<LinkedInUserInfo> FindUsers(string firstName, string lastName)
        {
            List<LinkedInUserInfo> users = new List<LinkedInUserInfo>();

            var service = GetLinkedInService();
            PeopleSearch peopleSearch = service.Search(String.Empty, firstName, lastName, String.Empty, false, SortCriteria.Connections, 0, 20, GetProfileFields());

            if (peopleSearch.People.Items != null && peopleSearch.People.Items.Count > 0)
            {
                foreach (Person person in peopleSearch.People.Items)
                    users.Add(MapUser(person));
            }

            return users;
        }

        private LinkedInService GetLinkedInService()
        {
            var authorization = new WebOAuthAuthorization(_tokenManager, _accessToken);
            return new LinkedInService(authorization);
        }

        private Collection<ProfileField> GetProfileFields()
        {
            Collection<ProfileField> fields = new Collection<ProfileField>();
            fields.Add(ProfileField.PersonId);
            fields.Add(ProfileField.FirstName);
            fields.Add(ProfileField.LastName);
            fields.Add(ProfileField.Headline);
            fields.Add(ProfileField.CurrentStatus);
            fields.Add(ProfileField.PositionId);
            fields.Add(ProfileField.PositionTitle);
            fields.Add(ProfileField.PositionSummary);
            fields.Add(ProfileField.PositionStartDate);
            fields.Add(ProfileField.PositionEndDate);
            fields.Add(ProfileField.PositionIsCurrent);
            fields.Add(ProfileField.PositionCompanyName);
            fields.Add(ProfileField.PictureUrl);
            fields.Add(ProfileField.PublicProfileUrl);
            fields.Add(ProfileField.ThreeCurrentPositions);
            fields.Add(ProfileField.ThreePastPositions);

            return fields;
        }


        public List<Message> Map(Updates updates)
        {
            if (updates == null)
                return null;

            List<Message> messages = new List<Message>(updates.Items.Count);

            foreach (Update update in updates.Items)
            {
                if (update.UpdateType.ToLower() != "stat")
                    continue;

                LinkedInMessage message = new LinkedInMessage();

                DateTime postedOn = DateTime.SpecifyKind(ConvertFromUnixTimestamp(update.Timestamp), DateTimeKind.Utc);
                message.PostedOn = TimeZoneInfo.ConvertTime(postedOn, TimeZoneInfo.Local);
                message.Source = SocialNetworks.LinkedIn;
                message.UserName = update.UpdateContent.Person.Name;
                message.Text = update.UpdateContent.Person.CurrentStatus;
                message.UserImageUrl = update.UpdateContent.Person.PictureUrl;

                messages.Add(message);
            }

            return messages;
        }

        private DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds((double)timestamp);
        }

        private LinkedInUserInfo MapUser(Person person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            LinkedInUserInfo userInfo = new LinkedInUserInfo
            {
                UserID = person.Id,
                UserName = person.Name,
                ImageUrl = person.PictureUrl,
                FirstName = person.FirstName,
                LastName = person.LastName,
            };

            if (person.Positions != null && person.Positions.Count > 0)
            {
                userInfo.Position = person.Positions[0].Title;
                if (person.Positions[0].Company != null)
                    userInfo.CompanyName = person.Positions[0].Company.Name;
            }

            userInfo.PublicProfileUrl = person.PublicProfileUrl;

            return userInfo;
        }
        
        private Exception CreateException(Exception ex)
        {
            if (ex is LinkedInNotAuthorizedException)
            {
                throw new UnauthorizedException();
            }
            throw new SocialMediaException();
        }
    }
}
