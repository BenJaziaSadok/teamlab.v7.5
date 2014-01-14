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
    public class FeedApiFilter
    {
        public string Product { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Offset { get; set; }
        public int Max { get; set; }
        public Guid Author { get; set; }
        public string[] SearchKeys { get; set; }
        public bool OnlyNew { get; set; }
    }
}