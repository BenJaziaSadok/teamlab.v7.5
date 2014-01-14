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
using ASC.SocialMedia;
using ASC.Web.Core.Utility.Skins;
using System.Web.UI;

namespace ASC.Web.UserControls.SocialMedia
{
    public static class ActivityMessageViewExtensions
    {
        public static string GetSocialNetworkUrl(this Message message)
        {
            switch (message.Source)
            {
                case SocialNetworks.Twitter:
                    return "http://www.twitter.com";
                case SocialNetworks.Facebook:
                    return "http://www.facebook.com";
                case SocialNetworks.LinkedIn:
                    return "http://www.linkedin.com";
                case SocialNetworks.Digg:
                    return "http://www.digg.com";
                case SocialNetworks.Quora:
                    return "http://www.quora.com";
                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetSocialNetworkTitle(this Message message)
        {
            switch (message.Source)
            {
                case SocialNetworks.Twitter:
                    return "twitter.com";
                case SocialNetworks.Facebook:
                    return "facebook.com";
                case SocialNetworks.LinkedIn:
                    return "linkedin.com";
                case SocialNetworks.Digg:
                    return "digg.com";
                case SocialNetworks.Quora:
                    return "quora.com";
                default:
                    throw new NotImplementedException();
            }
        }
        
    }
}
