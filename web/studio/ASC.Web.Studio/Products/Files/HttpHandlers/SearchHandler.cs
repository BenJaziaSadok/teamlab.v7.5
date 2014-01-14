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
using ASC.Files.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Utility.HtmlUtility;

namespace ASC.Web.Files.Configuration
{
    public class SearchHandler : BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return ProductEntryPoint.ID; }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "common_search_icon.png" }; }
        }

        public override Guid ModuleID
        {
            get { return ProductID; }
        }

        public override string SearchName
        {
            get { return FilesCommonResource.Search; }
        }

        public override SearchResultItem[] Search(string text)
        {
            var security = Global.GetFilesSecurity();
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                var files = fileDao.Search(text, FolderType.USER | FolderType.COMMON)
                                   .Where(security.CanRead)
                                   .Select(r => new SearchResultItem
                                       {
                                           Name = r.Title ?? string.Empty,
                                           Description = string.Empty,
                                           URL = CommonLinkUtility.GetFileWebPreviewUrl(r.Title, r.ID),
                                           Date = r.ModifiedOn,
                                           Additional = new Dictionary<string, object>
                                               {
                                                   { "Author", r.CreateByString.HtmlEncode() },
                                                   { "Path", FolderPathBuilder(EntryManager.GetBreadCrumbs(r.FolderID, folderDao)) },
                                                   { "Size", FileSizeComment.FilesSizeToString(r.ContentLength) }
                                               }
                                       });

                var folders = folderDao.Search(text, FolderType.USER | FolderType.COMMON)
                                       .Where(security.CanRead)
                                       .Select(f => new SearchResultItem
                                           {
                                               Name = f.Title ?? string.Empty,
                                               Description = String.Empty,
                                               URL = PathProvider.GetFolderUrl(f),
                                               Date = f.ModifiedOn,
                                               Additional = new Dictionary<string, object>
                                                   {
                                                       { "Author", f.CreateByString.HtmlEncode() },
                                                       { "Path", FolderPathBuilder(EntryManager.GetBreadCrumbs(f.ID, folderDao)) },
                                                       { "IsFolder", true }
                                                   }
                                           });

                return folders.Concat(files).ToArray();
            }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }

        private static String FolderPathBuilder(IEnumerable<Folder> folders)
        {
            var titles = folders.Select(f => f.Title).ToList();
            const string separator = " \\ ";
            return 4 < titles.Count
                       ? string.Join(separator, new[] { titles.First(), "...", titles.ElementAt(titles.Count - 2), titles.Last() })
                       : string.Join(separator, titles.ToArray());
        }
    }

    public sealed class ResultsView : ItemSearchControl
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "tableBase");
            writer.AddAttribute("cellspacing", "0");
            writer.AddAttribute("cellpadding", "8");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

            foreach (var srGroup in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-result-item");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase left-column gray-text");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                var typeTitle = srGroup.Additional.ContainsKey("IsFolder") && srGroup.Additional["IsFolder"].Equals(true) ? FilesCommonResource.Folder : FilesCommonResource.File;

                writer.AddAttribute(HtmlTextWriterAttribute.Title, typeTitle);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(typeTitle);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase center-column");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, srGroup.URL);
                writer.AddAttribute(HtmlTextWriterAttribute.Title, srGroup.Name);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "link bold");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, srGroup.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                writer.WriteBreak();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "text-medium-describe");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (srGroup.Additional.ContainsKey("Author"))
                {
                    writer.Write("{0} {1}", FilesCommonResource.Author, srGroup.Additional["Author"]);
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "splitter");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write("|");
                writer.RenderEndTag();

                writer.Write("{0} {1}", FilesCommonResource.TitleUploaded, srGroup.Date);

                if (srGroup.Additional.ContainsKey("Size"))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "separator");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write("|");
                    writer.RenderEndTag();
                    writer.Write("{0} {1}", FilesCommonResource.Size, srGroup.Additional["Size"]);
                }
                writer.RenderEndTag();

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase right-column gray-text");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(srGroup.Additional["Path"]);
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}