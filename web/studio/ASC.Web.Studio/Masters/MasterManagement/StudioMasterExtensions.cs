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

using System.Web.UI;
using ASC.Web.Studio.Masters;

namespace System.Web
{
    public static class StudioMasterExtensions
    {
        #region Style

        public static void RegisterStyleControl(this Page page, string lessPath, bool theme = false)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddStyles(lessPath, theme);
            }
        }

        public static void RegisterStyleControl(this Page page, Control control, bool theme = false)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddStyles(control, theme);
            }
        }

        #endregion

        #region Script

        public static void RegisterBodyScripts(this Page page, string scriptPath)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddBodyScripts(scriptPath);
            }
        }

        public static void RegisterBodyScripts(this Page page, Control control)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddBodyScripts(control);
            }
        }

        public static void RegisterInlineScript(this Page page, string script, bool beforeBodyScript = false, bool onReady = true)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.RegisterInlineScript(script, beforeBodyScript, onReady);
            }
        }

        #endregion

        #region ClientScript

        public static void RegisterClientScript(this Page page, Type type)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddClientScript(type);
            }
        }

        public static void RegisterClientLocalizationScript(this Page page, Type type)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = GetStudioMaster(page);
            if (master != null)
            {
                master.AddClientLocalizationScript(type);
            }
        }

        #endregion

        private static BaseTemplate GetStudioMaster(this Page page)
        {
            if (page == null) throw new ArgumentNullException("page");
            var master = page.Master;
            while (master != null && !(master is BaseTemplate))
            {
                master = master.Master;
            }
            return master as BaseTemplate;
        }
    }
}