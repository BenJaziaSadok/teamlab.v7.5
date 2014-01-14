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

namespace ASC.Core.Caching
{
    public class TrustInterval
    {
        private TimeSpan interval;


        public DateTime StartTime
        {
            get;
            private set;
        }

        public bool Expired
        {
            get { return interval == default(TimeSpan) || interval < (DateTime.UtcNow - StartTime).Duration(); }
        }


        public void Start(TimeSpan interval)
        {
            this.interval = interval;
            StartTime = DateTime.UtcNow;
        }

        public void Expire()
        {
            interval = default(TimeSpan);
        }
    }
}
