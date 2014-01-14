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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Common.Banner;

namespace ASC.Web.Studio.Masters
{
    public partial class BaseTemplate : MasterPage
    {
        /// <summary>
        /// Block side panel
        /// </summary>
        public bool DisabledSidePanel { get; set; }

        public bool DisabledHelpTour { get; set; }

        public bool DisabledTopStudioPanel { get; set; }

        public TopStudioPanel TopStudioPanel;

        private static Regex _browserNotSupported;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            TopStudioPanel = (TopStudioPanel)LoadControl(TopStudioPanel.Location);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitScripts();

            if (!DisabledTopStudioPanel)
            {
                TopContent.Controls.Add(TopStudioPanel);
            }

            if (_browserNotSupported == null && !string.IsNullOrEmpty(WebConfigurationManager.AppSettings["web.browser-not-supported"]))
            {
                _browserNotSupported = new Regex(WebConfigurationManager.AppSettings["web.browser-not-supported"], RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
            }
            if (_browserNotSupported != null && !string.IsNullOrEmpty(Request.Headers["User-Agent"]) && _browserNotSupported.Match(Request.Headers["User-Agent"]).Success)
            {
                Response.Redirect("~/browser-not-supported.htm");
            }

            if (CoreContext.Configuration.YourDocsDemo && Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context.Request.UserAgent, true))
            {
                Response.Redirect("~/mobile-not-supported.htm");
            }

            if (!EmailActivated && !CoreContext.Configuration.YourDocsDemo && SecurityContext.IsAuthenticated)
            {
                activateEmailPanel.Controls.Add(LoadControl(ActivateEmailPanel.Location));
            }

            if (!(Page is Auth || Page is Tariffs || Page is confirm || Page is Wizard || Page is ServerError || Page is Welcome))
            {
                var curTariff = TenantExtra.GetCurrentTariff();
                if (CoreContext.Configuration.Standalone && curTariff.QuotaId.Equals(Tenant.DEFAULT_TENANT))
                {
                    _contentHolder.Controls.Add(LoadControl(TariffLicenseOver.Location));
                }
            }
            if (AffiliateHelper.BannerAvailable)
            {
                BannerHolder.Controls.Add(LoadControl(Banner.Location));
            }

            if (!DisabledHelpTour)
            {
                HeaderContent.Controls.Add(LoadControl(UserControls.Common.HelpTour.HelpTour.Location));
            }
        }

        protected string RenderStatRequest()
        {
            if (string.IsNullOrEmpty(SetupInfo.StatisticTrackURL)) return string.Empty;

            var page = HttpUtility.UrlEncode(Page.AppRelativeVirtualPath.Replace("~", ""));
            return String.Format("<img style=\"display:none;\" src=\"{0}\"/>", SetupInfo.StatisticTrackURL + "&page=" + page);
        }

        protected string RenderCustomScript()
        {
            var sb = new StringBuilder();
            //custom scripts
            foreach (var script in SetupInfo.CustomScripts.Where(script => !String.IsNullOrEmpty(script)))
            {
                sb.AppendFormat("<script language=\"javascript\" src=\"{0}\" type=\"text/javascript\"></script>", script);
            }

            return sb.ToString();
        }

        protected bool EmailActivated
        {
            get { return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).ActivationStatus == EmployeeActivationStatus.Activated; }
        }

        protected string ColorThemeClass
        {
            get { return ColorThemesSettings.GetColorThemesSettings(); }
        }

        #region Operations

        private void InitScripts()
        {
            AddCommon(LoadControl(VirtualPathUtility.ToAbsolute("~/Masters/CommonStyles.ascx")), HeadStyles);
            AddCommon(LoadControl(VirtualPathUtility.ToAbsolute("~/Masters/CommonBodyScripts.ascx")), BodyScripts);
            AddStyles("~/skins/<theme_folder>/main.less", true);

            AddClientScript(typeof(MasterResources.MasterSettingsResources));
            AddClientScript(typeof(MasterResources.MasterUserResources));
            AddClientScript(typeof(MasterResources.MasterFileUtilityResources));
            AddClientScript(typeof(MasterResources.MasterCustomResources));

            if (ClientLocalizationScript != null)
            {
                AddClientLocalizationScript(typeof(MasterResources.MasterLocalizationResources));
                AddClientLocalizationScript(typeof(MasterResources.MasterTemplateResources));

                ClientLocalizationScript.Scripts.Add(clientScriptReference.GetLink(true));
            }
        }

        #region Style

        private static void AddCommon(Control control, ResourceBundleControl bundle)
        {
            if (bundle == null) return;

            bundle.Controls.AddAt(0, control);
        }

        public void AddStyles(Control control, bool theme)
        {
            if (theme)
            {
                if (ThemeStyles == null) return;

                ThemeStyles.Controls.Add(control);
            }
            else
            {
                if (HeadStyles == null) return;

                HeadStyles.Controls.Add(control);

            }
        }

        public void AddStyles(string src, bool theme)
        {
            if (theme)
            {
                if (ThemeStyles == null) return;

                ThemeStyles.Styles.Add(ResolveUrl(ColorThemesSettings.GetThemeFolderName(src)));
            }
            else
            {

                if (HeadStyles == null) return;

                HeadStyles.Styles.Add(src);
            }
        }

        #endregion

        #region Scripts

        public void AddBodyScripts(string src)
        {
            if (BodyScripts == null) return;
            BodyScripts.Scripts.Add(src);
        }

        public void AddLocalizationScripts(string src)
        {
            if (ClientLocalizationScript == null) return;
            ClientLocalizationScript.Scripts.Add(src);
        }

        public void AddBodyScripts(Control control)
        {
            if (BodyScripts == null) return;
            BodyScripts.Controls.Add(control);
        }


        public void RegisterInlineScript(string script, bool beforeBodyScripts, bool onReady)
        {
            if (!beforeBodyScripts)
                InlineScript.Scripts.Add(new Tuple<string, bool>(script, onReady));
            else
                InlineScriptBefore.Scripts.Add(new Tuple<string, bool>(script, onReady));
        }

        #endregion

        #region Content

        public void AddBodyContent(Control control)
        {
            PageContent.Controls.Add(control);
        }

        public void SetBodyContent(Control control)
        {
            PageContent.Controls.Clear();
            AddBodyContent(control);
        }

        #endregion

        #region ClientScript

        public void AddClientScript(Type type)
        {
            baseTemplateMasterScripts.Includes.Add(type);
        }

        private ClientScriptReference clientScriptReference;

        public void AddClientLocalizationScript(Type type)
        {
            if (clientScriptReference == null)
                clientScriptReference = new ClientScriptReference();

            clientScriptReference.Includes.Add(type);
        }

        #endregion

        #endregion
    }
}