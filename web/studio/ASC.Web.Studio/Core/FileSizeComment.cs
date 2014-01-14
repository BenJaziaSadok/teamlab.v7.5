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
using ASC.Core.Tenants;
using Resources;

namespace ASC.Web.Studio.Core
{
    public static class FileSizeComment
    {
        public static string FileSizeExceptionString
        {
            get { return GetFileSizeExceptionString(SetupInfo.MaxUploadSize); }
        }

        public static string FileImageSizeExceptionString
        {
            get { return GetFileSizeExceptionString(SetupInfo.MaxImageUploadSize); }
        }

        public static string GetFileSizeExceptionString(long size)
        {
            return string.Format("{0} ({1}).", Resource.FileSizeMaxExceed, FilesSizeToString(size));
        }

        /// <summary>
        /// The maximum file size is exceeded (25 MB).
        /// </summary>
        public static Exception FileSizeException
        {
            get { return new TenantQuotaException(FileSizeExceptionString); }
        }

        /// <summary>
        /// The maximum file size is exceeded (1 MB).
        /// </summary>
        public static Exception FileImageSizeException
        {
            get { return new TenantQuotaException(FileImageSizeExceptionString); }
        }

        public static Exception GetFileSizeException(long size)
        {
            return new TenantQuotaException(GetFileSizeExceptionString(size));
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public static string GetFileSizeNote()
        {
            return GetFileSizeNote(true);
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public static string GetFileSizeNote(bool withHtmlStrong)
        {
            return GetFileSizeNote(Resource.FileSizeNote, withHtmlStrong);
        }

        /// <summary>
        /// Get note about maximum file size
        /// </summary>
        /// <param name="note">Resource fromat of note</param>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 25 MB</returns>
        public static string GetFileSizeNote(string note, bool withHtmlStrong)
        {
            return
                String.Format(note,
                              FilesSizeToString(SetupInfo.MaxUploadSize),
                              withHtmlStrong ? "<strong>" : string.Empty,
                              withHtmlStrong ? "</strong>" : string.Empty);
        }

        /// <summary>
        /// Get note about maximum file size of image
        /// </summary>
        /// <param name="note">Resource fromat of note</param>
        /// <param name="withHtmlStrong">Highlight a word about size</param>
        /// <returns>Note: the file size cannot exceed 1 MB</returns>
        public static string GetFileImageSizeNote(string note, bool withHtmlStrong)
        {
            return
                String.Format(note,
                              FilesSizeToString(SetupInfo.MaxImageUploadSize),
                              withHtmlStrong ? "<strong>" : string.Empty,
                              withHtmlStrong ? "</strong>" : string.Empty);
        }

        /// <summary>
        /// Generates a string the file size
        /// </summary>
        /// <param name="size">Size in bytes</param>
        /// <returns>10 b, 100 Kb, 25 Mb, 1 Gb</returns>
        public static string FilesSizeToString(long size)
        {
            var sizeNames = !string.IsNullOrEmpty(Resource.FileSizePostfix) ? Resource.FileSizePostfix.Split(',') : new[] {"bytes", "KB", "MB", "GB", "TB"};
            var power = 0;

            double resultSize = size;
            if (1024 <= resultSize)
            {
                power = (int) Math.Log(resultSize, 1024);
                power = power < sizeNames.Length ? power : sizeNames.Length - 1;
                resultSize = resultSize/Math.Pow(1024d, power);
            }
            return string.Format("{0:#,0.##} {1}", resultSize, sizeNames[power]);
        }
    }
}