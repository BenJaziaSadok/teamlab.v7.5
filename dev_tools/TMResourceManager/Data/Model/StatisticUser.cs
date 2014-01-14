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

namespace TMResourceData.Model
{
    public class StatisticUser
    {
        public int WordsCount { get; set; }
        public int SignCount { get; set; }
        public string Culture { get; set; }
        public string Module { get; set; }
        public string Login { get; set; }
    }
}
