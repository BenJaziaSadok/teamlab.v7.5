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

#region Import

using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using ASC.Thrdparty;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth;
using log4net;

#endregion

namespace ASC.SocialMedia.Twitter
{
    /// <summary>
    /// Contains methods for getting data from Twitter
    /// </summary>
    public class TwitterDataProvider
    {

        private static WebConsumer signInConsumer;
        private static readonly object SignInConsumerInitLock = new object();

        private static WebConsumer TwitterSignIn
        {
            get
            {
                if (signInConsumer == null)
                {
                    lock (SignInConsumerInitLock)
                    {
                        if (signInConsumer == null)
                        {

                            var tokentManagerHolder = TokenManagerHolder.Get(ProviderConstants.Twitter);

                            signInConsumer = new WebConsumer(TwitterConsumer.SignInWithTwitterServiceDescription, tokentManagerHolder);
                        }
                    }
                }

                return signInConsumer;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(TwitterDataProvider));
        private readonly TwitterApiInfo _apiInfo;

        public enum ImageSize
        {
            Small,
            Original
        }

        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="apiInfo">TwitterApiInfo object</param>
        public TwitterDataProvider(TwitterApiInfo apiInfo)
        {
            if (apiInfo == null)
                throw new ArgumentNullException("apiInfo");

            TwitterSignIn.TokenManager.ExpireRequestTokenAndStoreNewAccessToken(
                TwitterSignIn.TokenManager.ConsumerKey,
                String.Empty,
                apiInfo.AccessToken,
                apiInfo.AccessTokenSecret);

            _apiInfo = apiInfo;
        }

        /// <summary>
        /// Gets current user (defined by access token) home timeline
        /// </summary>
        /// <param name="messageCount">Message count</param>        
        /// <returns>Message list</returns>
        public List<Message> GetUserHomeTimeLine(int messageCount)
        {
            var homeTimeLine = TwitterConsumer.GetHomeTimeLine(TwitterSignIn, _apiInfo.AccessToken, true, messageCount);

            if (homeTimeLine == null) return new List<Message>();
            
            return homeTimeLine.Select(x => (Message)(new TwitterMessage
            {
                UserName = x["user"].Value<String>("name"),
                PostedOn = ParseTweetDateTime(x.Value<String>("created_at")),
                Source = SocialNetworks.Twitter,
                Text = ParseTweetTextIntoHtml(x.Value<String>("text")),
                UserImageUrl = x["user"].Value<String>("profile_image_url")
            })).Take(20).ToList();
        }

        private String ParseTweetTextIntoHtml(String text)
        {
            text = Regex.Replace(text, "(https?://([-\\w\\.]+)+(/([\\w/_\\.]*(\\?\\S+)?(#\\S+)?)?)?)", "<a href='$1'>$1</a>");
            text = Regex.Replace(text, "@(\\w+)", "<a href='https://twitter.com/$1'>@$1</a>");
            text = Regex.Replace(text, "\\s#(\\w+)", "<a href='https://twitter.com/search?q=%23$1&src=hash'>#$1</a>");
           
            return text;
        }

         private DateTime ParseTweetDateTime(String dateTimeAsText)
         {
             var dayOfWeek = dateTimeAsText.Substring(0, 3).Trim();

             var month = dateTimeAsText.Substring(4, 3).Trim();

             var dayInMonth = dateTimeAsText.Substring(8, 2).Trim();

             var time = dateTimeAsText.Substring(11, 9).Trim();

             var offset = dateTimeAsText.Substring(20, 5).Trim();

             var year = dateTimeAsText.Substring(25, 5).Trim();

             var dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);

             var ret = DateTime.Parse(dateTime);

             return ret;
         }


        /// <summary>
        /// Gets tweets posted by specified user
        /// </summary>
        /// <param name="messageCount">Message count</param>        
        /// <returns>Message list</returns>
        public List<Message> GetUserTweets(decimal? userID, string screenName, int messageCount)
        {

            int localUserId = 0;

            if (userID.HasValue)
                localUserId = (int)userID.Value;

            var userTimeLine = TwitterConsumer.GetUserTimeLine(TwitterSignIn, _apiInfo.AccessToken, localUserId, screenName, true, messageCount);

            if (userTimeLine == null) return new List<Message>();

            return userTimeLine.Select(x => (Message)(new TwitterMessage
            {
               UserName =x["user"].Value<String>("name") ,
               PostedOn = ParseTweetDateTime(x.Value<String>("created_at")),
               Source = SocialNetworks.Twitter,
               Text = ParseTweetTextIntoHtml(x.Value<String>("text")),
               UserImageUrl = x["user"].Value<String>("profile_image_url")
            })).Take(20).ToList();
        }

        /// <summary>
        /// Loads specified user information
        /// </summary>
        /// <param name="userID">Twitter user ID</param>
        /// <returns>TwitterUserInfo obect</returns>
        public TwitterUserInfo LoadUserInfo(decimal userID)
        {
            var userInfo = TwitterConsumer.GetUserInfo(TwitterSignIn, (int)userID,String.Empty, _apiInfo.AccessToken);
            
            if (userInfo == null) return new TwitterUserInfo();

            return new TwitterUserInfo
            {
                UserID = userInfo.Value<Decimal>("id"),
                Description = userInfo.Value<String>("description"),
                ScreenName = userInfo.Value<String>("screen_name"),
                SmallImageUrl = userInfo.Value<String>("profile_image_url"),
                UserName = userInfo.Value<String>("name")
            };
        }

        /// <summary>
        /// Gets last 20 users
        /// </summary>
        /// <param name="search">Search string</param>
        /// <returns>TwitterUserInfo list</returns>
        public List<TwitterUserInfo> FindUsers(string search)
        {
            var findedUsers = TwitterConsumer.SearchUsers(TwitterSignIn, search, _apiInfo.AccessToken);

            if (findedUsers == null)
                return new List<TwitterUserInfo>();

            return findedUsers.Select(x => new TwitterUserInfo
            {
                UserID = x.Value<Decimal>("id"),
                Description = x.Value<String>("description"),
                ScreenName = x.Value<String>("screen_name"), 
                SmallImageUrl = x.Value<String>("profile_image_url"), 
                UserName = x.Value<String>("name")  
            }).Take(20).ToList();
           
        }
        
        /// <summary>
        /// Gets url of User image
        /// </summary>
        /// <param name="userScreenName"></param>
        /// <exception cref="TwitterDataProvider.Exceptions.ResourceNotFoundException">ResourceNotFoundException</exception>
        /// <exception cref="TwitterDataProvider.Exceptions.InternalProviderException">InternalProviderException</exception>
        /// <returns>Url of image or null if resource does not exist</returns>
        public string GetUrlOfUserImage(string userScreenName, ImageSize imageSize)
        {
            var userInfo = TwitterConsumer.GetUserInfo(TwitterSignIn,0,userScreenName, _apiInfo.AccessToken);

            if (userInfo == null) return null;

            var profileImageUrl = userInfo.Value<String>("profile_image_url");

            var size = GetTwitterImageSizeText(imageSize);

            if (size == "original")
                profileImageUrl = profileImageUrl.Replace("_normal", String.Empty);

            return profileImageUrl;

        }

        private string GetTwitterImageSizeText(ImageSize imageSize)
        {
            string result = "original";
            if (imageSize == ImageSize.Small)
                result = "normal";
            return result;
        }
    }
}
