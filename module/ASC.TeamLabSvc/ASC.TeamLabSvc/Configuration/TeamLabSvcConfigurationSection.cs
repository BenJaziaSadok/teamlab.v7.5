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

using System.Configuration;

namespace ASC.TeamLabSvc.Configuration
{
    class TeamLabSvcConfigurationSection : ConfigurationSection
    {
        public const string SECTION_NAME = "teamlab";

        public const string SERVICES = "services";

        public const string TYPE = "type";

        public const string DISABLE = "disable";


        [ConfigurationProperty(SERVICES)]
        public TeamLabSvcConfigurationCollection TeamlabServices
        {
            get { return (TeamLabSvcConfigurationCollection)this[SERVICES]; }
        }


        public static TeamLabSvcConfigurationSection GetSection()
        {
            return (TeamLabSvcConfigurationSection)ConfigurationManager.GetSection(TeamLabSvcConfigurationSection.SECTION_NAME);
        }
    }
}
