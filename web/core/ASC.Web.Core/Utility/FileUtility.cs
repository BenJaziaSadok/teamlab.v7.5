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
using System.IO;
using System.Linq;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Web.Studio.Utility
{
    public enum FileType
    {
        Unknown = 0,
        Archive = 1,
        Video = 2,
        Audio = 3,
        Image = 4,
        Spreadsheet = 5,
        Presentation = 6,
        Document = 7
    }

    public static class FileUtility
    {
        static FileUtility()
        {
            if (!string.IsNullOrEmpty(CommonLinkUtility.DocServiceApiUrl))
            {
                ExtsWebEdited = (WebConfigurationManager.AppSettings["files.docservice.edited-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                ExtsWebPreviewed = (WebConfigurationManager.AppSettings["files.docservice.viewed-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            ExtsImagePreviewed = (WebConfigurationManager.AppSettings["files.viewed-images"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            ExtsMustConvert = (WebConfigurationManager.AppSettings["files.docservice.convert-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            ExtsCoAuthoring = (WebConfigurationManager.AppSettings["files.docservice.coauthor-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            ExtsUploadable.AddRange(ExtsWebPreviewed);
            ExtsUploadable.AddRange(ExtsWebEdited);
            ExtsUploadable.AddRange(ExtsImagePreviewed);
            ExtsUploadable = ExtsUploadable.Distinct().ToList();
        }

        #region method

        public static string GetFileExtension(string fileName)
        {
            string extension = null;
            try
            {
                extension = Path.GetExtension(fileName);
            }
            catch (Exception)
            {
                var position = fileName.LastIndexOf('.');
                if (0 <= position)
                    extension = fileName.Substring(position).Trim().ToLower();
            }
            return extension == null ? string.Empty : extension.Trim().ToLower();
        }

        public static string GetInternalExtension(string fileName)
        {
            var extension = GetFileExtension(fileName);
            string internalExtension;
            return InternalExtension.TryGetValue(GetFileTypeByExtention(extension), out internalExtension)
                       ? internalExtension
                       : extension;
        }

        public static string ReplaceFileExtension(string fileName, string newExtension)
        {
            newExtension = string.IsNullOrEmpty(newExtension) ? string.Empty : newExtension;
            return Path.GetFileNameWithoutExtension(fileName) + newExtension;
        }

        public static FileType GetFileTypeByFileName(string fileName)
        {
            return GetFileTypeByExtention(GetFileExtension(fileName));
        }

        public static FileType GetFileTypeByExtention(string extension)
        {
            extension = extension.ToLower();

            if (ExtsArchive.Contains(extension)) return FileType.Archive;
            if (ExtsVideo.Contains(extension)) return FileType.Video;
            if (ExtsAudio.Contains(extension)) return FileType.Audio;
            if (ExtsImage.Contains(extension)) return FileType.Image;
            if (ExtsSpreadsheet.Contains(extension)) return FileType.Spreadsheet;
            if (ExtsPresentation.Contains(extension)) return FileType.Presentation;
            if (ExtsDocument.Contains(extension)) return FileType.Document;

            return FileType.Unknown;
        }

        public static bool CanImageView(string fileName)
        {
            return ExtsImagePreviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool CanWebView(string fileName)
        {
            return ExtsWebPreviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool CanWebEdit(string fileName)
        {
            return ExtsWebEdited.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool CanCoAuhtoring(string fileName)
        {
            return ExtsCoAuthoring.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion

        #region member

        private static Dictionary<string, List<string>> _extsConvertible;

        public static Dictionary<string, List<string>> ExtsConvertible
        {
            get
            {
                if (_extsConvertible != null) return _extsConvertible;

                _extsConvertible = new Dictionary<string, List<string>>();
                const string databaseId = "files";
                const string tableTitle = "files_converts";

                using (var dbManager = new DbManager(databaseId))
                {
                    var sqlQuery = new SqlQuery(tableTitle).Select("input", "output");

                    var list = dbManager.ExecuteList(sqlQuery);

                    list.ForEach(item =>
                                     {
                                         var input = item[0] as string;
                                         var output = item[1] as string;
                                         if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
                                             return;
                                         input = input.ToLower().Trim();
                                         output = output.ToLower().Trim();
                                         if (!_extsConvertible.ContainsKey(input))
                                             _extsConvertible[input] = new List<string>();
                                         _extsConvertible[input].Add(output);
                                     });
                }
                return _extsConvertible;
            }
        }

        public static readonly List<string> ExtsUploadable = new List<string>();

        public static readonly List<string> ExtsImagePreviewed = new List<string>();

        public static readonly List<string> ExtsWebPreviewed = new List<string>();

        public static readonly List<string> ExtsWebEdited = new List<string>();

        public static readonly List<string> ExtsMustConvert = new List<string>();

        public static readonly List<string> ExtsCoAuthoring = new List<string>();

        public static readonly List<string> ExtsArchive = new List<string>
            {
                ".zip", ".rar", ".ace", ".arc", ".arj",
                ".bh", ".cab", ".enc", ".gz", ".ha",
                ".jar", ".lha", ".lzh", ".pak", ".pk3",
                ".tar", ".tgz", ".uu", ".uue", ".xxe",
                ".z", ".zoo"
            };

        public static readonly List<string> ExtsVideo = new List<string>
            {
                ".avi", ".mpg", ".mkv", ".mp4",
                ".mov", ".3gp", ".vob", ".m2ts"
            };

        public static readonly List<string> ExtsAudio = new List<string>
            {
                ".wav", ".mp3", ".wma", ".ogg"
            };

        public static readonly List<string> ExtsImage = new List<string>
            {
                ".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg",
                ".jfif", ".tiff", ".tif", ".cmx", ".ico", ".pnm", ".pbm",
                ".png", ".ppm", ".rgb", ".svg", ".xbm", ".xpm", ".xwd",
                ".svgt", ".svgy"
            };

        public static readonly List<string> ExtsSpreadsheet = new List<string>
            {
                ".xls", ".xlsx",
                ".ods", ".csv",
                ".xlst", ".xlsy"
            };

        public static readonly List<string> ExtsPresentation = new List<string>
            {
                ".pps", ".ppsx",
                ".ppt", ".pptx",
                ".odp",
                ".pptt", ".ppty"
            };

        public static readonly List<string> ExtsDocument = new List<string>
            {
                ".docx", ".doc", ".odt", ".rtf", ".txt",
                ".html", ".htm", ".mht", ".pdf", ".djvu",
                ".fb2", ".epub", ".xps",
                ".doct", ".docy"
            };

        public static readonly Dictionary<FileType, string> InternalExtension = new Dictionary<FileType, string>
            {
                { FileType.Document, WebConfigurationManager.AppSettings["files.docservice.internal-doc"] ?? ".docx" },
                { FileType.Spreadsheet, WebConfigurationManager.AppSettings["files.docservice.internal-xls"] ?? ".xlsx" },
                { FileType.Presentation, WebConfigurationManager.AppSettings["files.docservice.internal-ppt"] ?? ".pptx" }
            };

        public enum CsvDelimiter
        {
            None = 0,
            Tab = 1,
            Semicolon = 2,
            Сolon = 3,
            Comma = 4,
            Space = 5
        }

        #endregion
    }
}