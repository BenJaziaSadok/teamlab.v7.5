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
using System.Configuration;

namespace ASC.Feed.Aggregator.Config
{
    internal class FeedConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("serverRoot", DefaultValue = "http://*/")]
        public string ServerRoot
        {
            get { return (string)this["serverRoot"]; }
        }

        [ConfigurationProperty("aggregatePeriod", DefaultValue = "0:5:0")]
        public TimeSpan AggregatePeriod
        {
            get { return (TimeSpan)this["aggregatePeriod"]; }
        }

        [ConfigurationProperty("aggregateInterval", DefaultValue = "14.0:0:0")]
        public TimeSpan AggregateInterval
        {
            get { return (TimeSpan)this["aggregateInterval"]; }
        }

        [ConfigurationProperty("removePeriod", DefaultValue = "1.0:0:0")]
        public TimeSpan RemovePeriod
        {
            get { return (TimeSpan)this["removePeriod"]; }
        }


        public static FeedConfigurationSection GetFeedSection()
        {
            return (FeedConfigurationSection)ConfigurationManager.GetSection("feed");
        }
    }
}