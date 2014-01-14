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

using System.Collections.Generic;

namespace TMResourceData.Model
{
    public class ResWord
    {
        public ResFile ResFile { get; set; }
        public WordStatusEnum Status { get; set; }

        public string Title { get; set; }
        public string ValueFrom { get; set; }
        public string ValueTo { get; set; }
        public string TextComment { get; set; }
        public string Link { get; set; }
        public List<string> Alternative { get; set; }

        public int Flag { get; set; }
    }
}
