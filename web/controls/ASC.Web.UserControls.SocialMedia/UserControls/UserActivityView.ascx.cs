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
using ASC.SocialMedia;
using ASC.Web.UserControls.SocialMedia.Resources;
using ASC.SocialMedia.Twitter;

namespace ASC.Web.UserControls.SocialMedia.UserControls
{
    public partial class UserActivityView : BaseUserControl
    {
        #region - Classes -

        public class TwitterInfo
        {
            public TwitterApiInfo ApiInfo { get; set; }
            public List<UserAccountInfo> UserAccounts { get; set; }

            public TwitterInfo()
            {
                ApiInfo = new TwitterApiInfo();
                UserAccounts = new List<UserAccountInfo>();
            }
        }

        public class UserAccountInfo
        {
            public string ScreenName { get; set; }
            public Decimal? UserID { get; set; }
        }

        #endregion

        #region - Members -

        private List<SocialNetworks> _socialNetworks = new List<SocialNetworks>();

        #endregion

        #region - Properties -

        public TwitterInfo TwitterInformation { get; set; }

        public List<SocialNetworks> SelectedSocialNetworks
        {
            get { return _socialNetworks; }
        }

        public int MessageCount { get; set; }

        public int LoadedMessageCount { get; set; }

        public const string MessageCountCookieName = "sm_msg_count";

        public Exception LastException { get; set; }

        public bool InvalidTwitterAccountExist { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckInputParameters();

            SetMessageCount();

            var ctrlMessageList = (ListActivityMessageView)LoadControl("ListActivityMessageView.ascx");

            try
            {
                ctrlMessageList.MessageList = GetUserActivity();
                LoadedMessageCount = ctrlMessageList.MessageList.Count;
            }
            catch (Exception ex)
            {
                LastException = ex;
                throw ex;
            }

            CommonContainerHolder.Controls.Add(ctrlMessageList);
        }

        #region - Methods -

        private void CheckInputParameters()
        {
            if (SelectedSocialNetworks.Contains(SocialNetworks.Twitter))
            {
                if (TwitterInformation == null)
                    throw new ArgumentException("You must set TwitterInformation object");
            }
        }

        private void SetMessageCount()
        {
            int messageCountFromCookie;
            if (HttpContext.Current.Request.Cookies[MessageCountCookieName] != null && Int32.TryParse(HttpContext.Current.Request.Cookies[MessageCountCookieName].Value, out messageCountFromCookie))
                MessageCount = messageCountFromCookie;

            _ctrlMessageCount.SelectedValue = MessageCount.ToString();
        }

        private List<Message> GetUserActivity()
        {
            if (SelectedSocialNetworks == null)
                return null;

            List<Message> messages = new List<Message>();
            if (SelectedSocialNetworks.Contains(SocialNetworks.Twitter))
            {
                TwitterDataProvider twitterProvider = new TwitterDataProvider(TwitterInformation.ApiInfo);
                foreach (UserAccountInfo accountInfo in TwitterInformation.UserAccounts)
                {
                    try
                    {
                        messages.AddRange(twitterProvider.GetUserTweets(accountInfo.UserID, accountInfo.ScreenName, MessageCount));
                    }
                    catch (ResourceNotFoundException)
                    {
                        _ctrlErrorDescription.InnerText += String.Format("{0}: {1}", SocialMediaResource.ErrorUnknownTwitterAccount, accountInfo.ScreenName);
                        _ctrlErrorDescriptionContainer.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "block");
                        this.InvalidTwitterAccountExist = true;
                    }
                }
            }

            return messages.OrderByDescending(m => m.PostedOn).Take(MessageCount).ToList();
        }


        #endregion
    }
}