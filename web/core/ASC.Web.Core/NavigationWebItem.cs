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
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public class NavigationWebItem : IWebItem
    {
        public virtual Guid ID { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string StartURL { get; set; }

        public virtual string ProductClassName { get; set; }

        public virtual WebItemContext Context { get; set; }

        
        public override bool Equals(object obj)
        {
            var m = obj as IWebItem;
            return m != null && ID == m.ID;
        }
        
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
