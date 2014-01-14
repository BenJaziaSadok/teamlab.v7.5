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

namespace ASC.Feed
{
    public struct FeedFilter
    {
        public TimeInterval Time { get; private set; }

        public int Tenant { get; set; }

        public FeedFilter(TimeInterval time) : this()
        {
            Time = time;
        }

        public FeedFilter(DateTime from, DateTime to)
            : this()
        {
            Time = new TimeInterval(from, to);
        }
    }
}