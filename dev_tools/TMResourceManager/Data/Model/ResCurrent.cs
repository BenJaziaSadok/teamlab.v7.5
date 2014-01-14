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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMResourceData.Model
{
    public class ResCurrent
    {
        public ResProject Project { get; set; }
        public ResModule Module { get; set; }
        public ResWord Word { get; set; }
        public ResCulture Language { get; set; }
        public Author Author { get; set; }
    }
}
