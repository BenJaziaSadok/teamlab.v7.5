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

using ASC.Core.Users;
using System;

namespace ASC.Web.Studio.Core.SMS
{
    public static class SmsManager
    {
        public static string BuildPhoneNoise(string mobilePhone)
        {
            throw new NotImplementedException();
        }

        public static string SaveMobilePhone(UserInfo user, string mobilePhone)
        {
            throw new NotImplementedException();
        }

        public static void PutAuthCode(UserInfo user, bool again)
        {
            throw new NotImplementedException();
        }

        public static void ValidateSmsCode(UserInfo user, string code)
        {
            throw new NotImplementedException();
        }
    }
}