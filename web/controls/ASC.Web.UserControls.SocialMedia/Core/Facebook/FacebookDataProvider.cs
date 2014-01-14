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
using System.Net;
using System.Web;
using Facebook;
using log4net;

namespace ASC.SocialMedia.Facebook
{
    public class FacebookDataProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FacebookDataProvider));
        private readonly FacebookApiInfo _apiInfo;
        private const string Status = "status";
        private const string Link = "link";
        private const string Photo = "photo";
        private const string Video = "video";


        public FacebookDataProvider(FacebookApiInfo apiInfo)
        {
            if (apiInfo == null) throw new ArgumentNullException("apiInfo");

            _apiInfo = apiInfo;
        }

        public FacebookUserInfo LoadCurrentUserInfo()
        {
            try
            {
                FacebookClient client = new FacebookClient();
                client.AccessToken = _apiInfo.AccessToken;

                JsonObject jsonData = (JsonObject)client.Get("me");

                return Map(jsonData);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public FacebookUserInfo LoadUserInfo(string userID)
        {
            try
            {
                FacebookClient client = new FacebookClient();
                client.AccessToken = _apiInfo.AccessToken;

                JsonObject jsonData = (JsonObject)client.Get(userID);

                return Map(jsonData);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public List<Message> LoadUserWall(string userID, int messageCount)
        {
            try
            {
                FacebookClient client = new FacebookClient();
                client.AccessToken = _apiInfo.AccessToken;

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("limit", messageCount);

                string url = String.Format("https://graph.facebook.com/{0}/feed", userID);
                JsonObject jsonData = (JsonObject)client.Get(url, parameters);
                return MapMessage(jsonData);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public List<FacebookUserInfo> FindUsers(string search)
        {
            try
            {
                const int pageRowCount = 20;

                FacebookClient client = new FacebookClient();
                client.AccessToken = _apiInfo.AccessToken;

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("limit", pageRowCount);

                string url = String.Format("https://graph.facebook.com/search?q={0}&type=user", HttpUtility.UrlEncode(search));
                JsonObject jsonData = (JsonObject)client.Get(url, parameters);

                return MapUserList(jsonData);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        public enum ImageSize
        {
            Small,
            Original
        }

        /// <summary>
        /// Gets url of User image
        /// </summary>
        /// <param name="userScreenName"></param>        
        /// <exception cref="ASC.SocialMedia.FacebookDataProvider.Exceptions.InternalProviderException">InternalProviderException</exception>
        /// <returns>Url of image or null if does not exist</returns>
        public string GetUrlOfUserImage(string userID, ImageSize imageSize)
        {
            String url = String.Format("http://graph.facebook.com/{0}/picture?type={1}", userID, GetFacebookImageSizeText(imageSize));

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    if (response.ContentType.ToLower().Contains("text/javascript"))
                        return null;
                    return response.ResponseUri.ToString();
                }
            }
            catch (WebException ex)
            {
                log.Error(ex);
                throw CreateException(ex);
            }
        }

        private string GetFacebookImageSizeText(ImageSize imageSize)
        {
            string result = "large";
            if (imageSize == ImageSize.Small)
                result = "small";
            return result;
        }


        private FacebookUserInfo Map(JsonObject jsonUser)
        {
            return new FacebookUserInfo
            {
                UserID = Convert.ToString(jsonUser["id"]),
                UserName = Convert.ToString(jsonUser["name"]),
            };
        }

        private List<FacebookUserInfo> MapUserList(JsonObject jsonUsers)
        {
            JsonArray jsonUserArray = (JsonArray)jsonUsers["data"];

            List<FacebookUserInfo> userList = new List<FacebookUserInfo>();

            foreach (JsonObject jsonUser in jsonUserArray)
            {
                FacebookUserInfo userInfo = new FacebookUserInfo
                {
                    UserID = Convert.ToString(jsonUser["id"]),
                    UserName = Convert.ToString(jsonUser["name"])
                };
                userList.Add(userInfo);
            }

            return userList;
        }

        private List<Message> MapMessage(JsonObject jsonData)
        {
            List<Message> messages = new List<Message>();

            JsonArray data = (JsonArray)jsonData["data"];

            foreach (JsonObject jsonMsg in data)
            {
                string messageType = (string)jsonMsg["type"];

                switch (messageType)
                {
                    case Status:
                        messages.Add(CreateStatusMessage(jsonMsg));
                        break;
                    case Link:
                        messages.Add(CreateLinkMessage(jsonMsg));
                        break;
                    case Photo:
                        messages.Add(CreatePhotoMessage(jsonMsg));
                        break;
                    case Video:
                        messages.Add(CreateVideoMessage(jsonMsg));
                        break;
                    default:
                        break;
                }
            }

            return messages;
        }

        private Message CreateLinkMessage(JsonObject jsonData)
        {
            FacebookMessage msg = new FacebookMessage();

            string text = "<br />";

            if (jsonData.ContainsKey("message"))
            {
                text += HttpUtility.HtmlEncode((string)jsonData["message"]);
                text += "<br /><br />";
            }
            text += String.Format("<a href='{0}' target='_blank'>{1}</a><br />", jsonData["link"].ToString(), jsonData["name"].ToString());

            if (jsonData.ContainsKey("caption"))
            {
                text += "<span class='facebookLinkDescription'>" + jsonData["caption"].ToString() + "</span>";
                text += "<br />";
            }
            text += "<br />";
            if (jsonData.ContainsKey("description"))
            {
                text += "<span class='facebookLinkDescription'>" + jsonData["description"].ToString() + "</span>";
                text += "<br />";
            }

            msg.Text = text;
            msg.PostedOn = Convert.ToDateTime(jsonData["updated_time"].ToString());
            msg.Source = SocialNetworks.Facebook;

            JsonObject jsonUser = (JsonObject)(jsonData["from"]);
            msg.UserName = jsonUser["name"].ToString();
            msg.UserImageUrl = String.Format("http://graph.facebook.com/{0}/picture?type=small", jsonUser["id"]);

            return msg;
        }

        private Message CreatePhotoMessage(JsonObject jsonData)
        {
            FacebookMessage msg = new FacebookMessage();

            string text = "<br />";

            if (jsonData.ContainsKey("message"))
            {
                text += HttpUtility.HtmlEncode((string)jsonData["message"]);
                text += "<br /><br />";
            }
            text += String.Format("<a href='{0}' target='_blank'><img src='{1}' /></a><br /><br />", jsonData["link"].ToString(), jsonData["picture"].ToString());

            msg.Text = text;
            msg.PostedOn = Convert.ToDateTime(jsonData["updated_time"].ToString());
            msg.Source = SocialNetworks.Facebook;

            JsonObject jsonUser = (JsonObject)(jsonData["from"]);
            msg.UserName = jsonUser["name"].ToString();
            msg.UserImageUrl = String.Format("http://graph.facebook.com/{0}/picture?type=small", jsonUser["id"]);

            return msg;
        }

        private Message CreateVideoMessage(JsonObject jsonData)
        {
            FacebookMessage msg = new FacebookMessage();

            string text = "<br />";

            if (jsonData.ContainsKey("message"))
            {
                text += HttpUtility.HtmlEncode((string)jsonData["message"]);
                text += "<br /><br />";
            }
            text += String.Format("<a href='{0}' target='_blank'><img src='{1}' /></a><br /><br />", jsonData["link"].ToString(), jsonData["picture"].ToString());

            msg.Text = text;
            msg.PostedOn = Convert.ToDateTime(jsonData["updated_time"].ToString());
            msg.Source = SocialNetworks.Facebook;

            JsonObject jsonUser = (JsonObject)(jsonData["from"]);
            msg.UserName = jsonUser["name"].ToString();
            msg.UserImageUrl = String.Format("http://graph.facebook.com/{0}/picture?type=small", jsonUser["id"]);

            return msg;
        }

        private Message CreateStatusMessage(JsonObject jsonData)
        {
            FacebookMessage msg = new FacebookMessage();
            msg.Text = HttpUtility.HtmlEncode((string)jsonData["message"]);
            msg.PostedOn = Convert.ToDateTime(jsonData["updated_time"].ToString());
            msg.Source = SocialNetworks.Facebook;

            JsonObject jsonUser = (JsonObject)(jsonData["from"]);
            msg.UserName = jsonUser["name"].ToString();
            msg.UserImageUrl = String.Format("http://graph.facebook.com/{0}/picture?type=small", jsonUser["id"]);

            return msg;
        }
        
        private Exception CreateException(Exception ex)
        {
            if (ex is FacebookApiLimitException)
            {
                throw new APILimitException();
            }
            if (ex is FacebookOAuthException)
            {
                throw new OAuthException();
            }
            throw new SocialMediaException();
        }
    }
}
