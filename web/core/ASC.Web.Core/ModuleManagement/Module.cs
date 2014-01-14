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
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core.ModuleManagement
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public class Module : IModule
    {
        public Module()
        {
            Context = new ModuleContext();
        }


        public virtual Guid ID
        {
            get;
            set;
        }

        public virtual Guid ProjectId
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string ModuleSysName
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string StartURL
        {
            get;
            set;
        }
        public virtual string ProductClassName
        {
            get;
            set;
        }

        public virtual ModuleContext Context
        {
            get;
            set;
        }

        WebItemContext IWebItem.Context
        {
            get { return Context; }
        }      

        public bool DisplayedAlways
        {
            get;
            set;
        }
    }
}
