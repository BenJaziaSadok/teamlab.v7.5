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
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Mail.Resources;

namespace ASC.Web.Mail
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class MailAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID
        {
            get { return WebItemManager.MailProductID; }
        }

        public static string BaseVirtualPath
        {
            get { return "~/addons/mail"; }
        }

        private AddonContext _context;

        public AddonContext Context
        {
            get { return _context; }
        }

        WebItemContext IWebItem.Context
        {
            get { return _context; }
        }

        public string Description
        {
            get { return MailResource.MailDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public void Init()
        {
            _context = new AddonContext
                {
                    DisabledIconFileName = "mail_disabled.png",
                    IconFileName = "mail.png",
                    SpaceUsageStatManager = new Configuration.MailSpaceUsageStatManager()
                };
        }

        public string Name
        {
            get { return MailResource.ProductName; }
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return BaseVirtualPath; }
        }

        public string ProductClassName
        {
            get { return "mail"; }
        }

        #region IRenderCustomNavigation Members

        public string RenderCustomNavigation(Page page)
        {
            if (CoreContext.Configuration.YourDocs) return string.Empty;

            var func = "";

            if (!page.AppRelativeTemplateSourceDirectory.Contains(BaseVirtualPath))
                func = @"setTimeout(function () {
                            Teamlab.getMailFolders({}, new Date(0), {});
                        }, 3000);";

            var script = string.Format(@"
                                        var _inbox_folder_id = 1;
                                        var _setUnreadMailMessagesCount = function(params, folders){{
                                          jq.each(folders, function(index, value) {{
                                            if(_inbox_folder_id==value.id) {{
                                              jq(""#TPUnreadMessagesCount"").text(value.unread);
                                              jq(""#TPUnreadMessagesCount"").parent().toggleClass(""has-led"", value.unread != 0);
                                              return false;
                                            }};
                                          }});
                                        }};
                                        Teamlab.bind(Teamlab.events.getMailFolders, _setUnreadMailMessagesCount);
                                        {0}", func);

            page.RegisterInlineScript(script);

            return string.Format(@"<li class=""top-item-box mail"">
                                     <a class=""inner-text"" href=""{0}"" title=""{1}"">
                                       <span id=""TPUnreadMessagesCount"" class=""inner-label""></span>
                                     </a>
                                   </li>",
                                 VirtualPathUtility.ToAbsolute(BaseVirtualPath + "/"),
                                 MailResource.MailTitle);
        }

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        #endregion
    }
}