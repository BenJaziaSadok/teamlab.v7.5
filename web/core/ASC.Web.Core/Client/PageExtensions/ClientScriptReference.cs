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
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.Core.Client.PageExtensions
{
    [ToolboxData("<{0}:ClientScriptReference runat=server></{0}:ClientScriptReference>")]
    public class ClientScriptReference : WebControl
    {
        private readonly ICollection<Type> _includes = new HashSet<Type>();

        public virtual ICollection<Type> Includes
        {
            get { return _includes; }
            set
            {
                if (value == null) return;

                foreach (var type in value)
                {
                    _includes.Add(type);
                }
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (Includes == null)
            {
                throw new ArgumentNullException("Types is empty", "Includes");
            }

            var rendered = Context.Items["clinetScript" + string.Join(",", Includes.Select(r => r.ToString()))] as bool?;
            if (rendered.HasValue) return;

            output.Write("<script type=\"text/javascript\" src=\"{0}\"></script>", ClientScriptBundle.ResolveHandlerPath(Includes));
            Context.Items["clinetScript" + Includes.Select(r => r.ToString())] = (bool?)true;
        }

        public string GetLinks()
        {
            return ClientScriptBundle.ResolveHandlerPath(Includes);
        }
    }
}