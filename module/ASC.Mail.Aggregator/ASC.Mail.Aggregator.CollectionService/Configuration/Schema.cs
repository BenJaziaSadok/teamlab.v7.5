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

namespace ASC.Mail.Aggregator.Client.Configuration
{
    internal static class Schema
    {
        public const string SECTION_NAME = "mail";
        public const string CONFIGURATION_ELEMENT_NAME = "queue";
        public const string CONFIGURATION_PROPERTY_CHECK_INTERVAL = "check_interval";
        public const string CONFIGURATION_PROPERTY_ACTIVITY_TIMEOUT = "activity_timeout";
        public const string CONFIGURATION_PROPERTY_NEWMESSAGESPERSESSION = "maxnew";
        public const string CONFIGURATION_PROPERTY_CONCURENTTHREADS = "threads";
        public const string CONFIGURATION_PROPERTY_OVERDUEACCOUNTDELAY = "overdue_delay";
        public const string CONFIGURATION_PROPERTY_LONGDEADACCOUNTDELAY = "longdead_delay";
    }
}