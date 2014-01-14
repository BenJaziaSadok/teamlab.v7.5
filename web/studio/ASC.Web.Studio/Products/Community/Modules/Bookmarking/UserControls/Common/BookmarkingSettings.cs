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
using System.Drawing.Imaging;
using System.Text;
using ASC.Web.UserControls.Bookmarking.Common.Util;
using System.Drawing;

namespace ASC.Web.UserControls.Bookmarking.Common
{
    public class BookmarkingSettings
    {
        public static Guid ModuleId = new Guid("28B10049-DD20-4f54-B986-873BC14CCFC7");

        public const string BookmarkingSctiptKey = "__boomarking_script_key";

        public const string BookmarkingTagsAutocompleteSctiptKey = "__boomarking_Tags_Autocomplete_script_key";

        public static int BookmarksCountOnPage = 10;
        public static int VisiblePageCount = 3;

        public static ImageFormat CaptureImageFormat = ImageFormat.Jpeg;

        public static string ThumbnailAbsoluteFilePath { get; set; }

        public static string ThumbnailAbsolutePath { get; set; }

        public static string ThumbnailRelativePath = "Products/Community/Modules/Bookmarking/Data/images/";

        public const string ThumbnailVirtualPath = "~/Products/Community/Modules/Bookmarking/Data/images/";

        public static int Timeout = 180;

        public static readonly BookmarkingThumbnailSize ThumbSmallSize = new BookmarkingThumbnailSize(96, 72);

        public static readonly BookmarkingThumbnailSize ThumbMediumSize = new BookmarkingThumbnailSize(192, 152);

        public static readonly Size BrowserSize = new Size(1280, 1020);

        public static Encoding PageTitleEncoding = Encoding.GetEncoding("windows-1251");

        public static int PingTimeout = 3000;

        public const int NameMaxLength = 255;

        public const int DescriptionMaxLength = 65535;

    }
}