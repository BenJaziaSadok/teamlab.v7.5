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

namespace ASC.Geolocation
{
    public class  IPGeolocationInfo
    {
        public string Key { get; set; }

        public string City {get; set;}
        
        public double TimezoneOffset { get; set; }
        
        public string TimezoneName { get; set; }

        public string IPStart { get; set; }
        
        public string IPEnd { get; set; }


        public readonly static IPGeolocationInfo Default = new IPGeolocationInfo
        {
            Key = string.Empty,
            IPStart = string.Empty,
            IPEnd = string.Empty,
            City = string.Empty,
            TimezoneName = string.Empty,
        };
    }
}
