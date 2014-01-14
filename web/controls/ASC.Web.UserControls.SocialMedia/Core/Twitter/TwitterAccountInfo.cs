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

namespace ASC.SocialMedia.Twitter
{
    public class TwitterAccountInfo
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public Guid AssociatedID { get; set; }
        public string ScreenName { get; set; }
        public decimal UserID { get; set; }        
        public string UserName { get; set; }
    }
}
