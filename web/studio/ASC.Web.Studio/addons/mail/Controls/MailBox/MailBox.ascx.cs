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
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Common.DocumentsPopup;
using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Mail.Controls
{
  [AjaxPro.AjaxNamespace("MailBox")]
  public partial class MailBox : System.Web.UI.UserControl
  {
    public static string Location { get { return "~/addons/mail/Controls/MailBox/MailBox.ascx"; } }
    public const int EntryCountOnPage_def = 25;
    public const int VisiblePageCount_def = 10;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/controls/FileUploader/FileUploaderScripts.ascx")));

        FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
        FCKeditor.ToolbarSet = "Mini";
        FCKeditor.EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath;

        AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

        TagsPageHolder.Controls.Add(LoadControl(TagsPage.Location) as TagsPage);
        TagsPageHolder.Controls.Add(LoadControl(AccountsPage.Location) as AccountsPage);
        TagsPageHolder.Controls.Add(LoadControl(ContactsPage.Location) as ContactsPage);
        _phPagerContent.Controls.Add(new PageNavigator
        {
            ID = "mailPageNavigator",
            CurrentPageNumber = 1,
            VisibleOnePage = false,
            EntryCount = 0,
            VisiblePageCount = VisiblePageCount_def,
            EntryCountOnPage = EntryCountOnPage_def
        });

        var documentsPopup = (DocumentsPopup)LoadControl(DocumentsPopup.Location);
        _phDocUploader.Controls.Add(documentsPopup);

        QuestionPopup.Options.IsPopup = true;

        if (!TenantExtra.GetTenantQuota().DocsEdition)
        {
            _phDocUploader.Controls.Add(LoadControl(TariffLimitExceed.Location));
        }
    }

    protected String RenderRedirectUpload()
    {
        return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=mail");
    }

    private static readonly Regex cssBlock = new Regex(@"(\<style(.)*?\>.*?((\r\n)*|.*)*?\<\/style\>)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex cssTag = new Regex(@"(\<style(.)*?\>|<\/style\>)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex cssRow = new Regex(@"((\r\n)(.*){)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private String wrapCSS(string css)
    {
        string res = css;
        res = "\r\n" + cssTag.Replace(css, "").Replace("\r\n", "").Replace("}", "}\r\n");
        MatchCollection mc = cssRow.Matches(res);
        foreach (Match occur in mc)
        {
            string selectors = occur.Value;
            if (!string.IsNullOrEmpty(selectors))
            {
                selectors = selectors.Replace("\r\n", "\r\n#itemContainer .body ").Replace(",", ", #itemContainer .body ");
            }
            res = res.Replace(occur.Value, selectors);

        }
        res = "<style>" + res + "</style>";
        return res;
    }

    private String handleCSS(string html)
    {
        if (cssBlock.IsMatch(html))
        {
            MatchCollection mc = cssBlock.Matches(html);
            foreach (Match occur in mc)
            {
                html = html.Replace(occur.Value, wrapCSS(occur.Value));
            }
        }
        return html;
    }

    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
    public string getHtmlBody(string url)
    {
        try
        {
            HttpWebRequest loHttp = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse loWebResponse = (HttpWebResponse)loHttp.GetResponse();
            StreamReader loResponseStream = new StreamReader(loWebResponse.GetResponseStream());
            string html = loResponseStream.ReadToEnd();
            loWebResponse.Close();
            loResponseStream.Close();

            html = handleCSS(html);

            return html;
        }
        catch
        {
            return String.Empty;
        }
    }
  }
}
