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

using ASC.Core.Configuration;

namespace ASC.Core
{
    public interface IConfigurationClient
    {
        SmtpSettings SmtpSettings { get; set; }

        bool Standalone { get; }

        bool YourDocs { get; }

        bool YourDocsDemo { get; }

        string SKey { get; set; }


        string GetSetting(string key);

        void SaveSetting(string key, string value);

        string GetKey(int tenant);
    }
}