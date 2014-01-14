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
using System.Web;
using System.Web.Configuration;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Web.Files.Services.DocumentService
{
    public static class DocumentServiceConnector
    {
        static DocumentServiceConnector()
        {
            DocumentConverterUrl = WebConfigurationManager.AppSettings["files.docservice.url.converter"] ?? "";
            DocumentStorageUrl = WebConfigurationManager.AppSettings["files.docservice.url.storage"] ?? "";

            Int32.TryParse(WebConfigurationManager.AppSettings["files.docservice.timeout"], out ConvertTimeout);
            ConvertTimeout = ConvertTimeout > 0 ? ConvertTimeout : 120000;
        }

        private static readonly int ConvertTimeout;
        private static readonly string DocumentConverterUrl;
        private static readonly string DocumentStorageUrl;

        public static string GenerateRevisionId(string expectedKey)
        {
            return DocumentService.GenerateRevisionId(expectedKey);
        }

        public static string GenerateValidateKey(string documentRevisionId)
        {
            var docServiceConnector = new DocumentService(
                StudioKeySettings.GetKey(),
                StudioKeySettings.GetSKey(),
                TenantStatisticsProvider.GetUsersCount());

            string userIp = null;
            try
            {
                if (HttpContext.Current != null) userIp = HttpContext.Current.Request.UserHostAddress;
            }
            catch
            {
                userIp = string.Empty;
            }

            return docServiceConnector.GenerateValidateKey(documentRevisionId, userIp);
        }

        public static int GetConvertedUri(string documentUri,
                                          string fromExtension,
                                          string toExtension,
                                          string documentRevisionId,
                                          bool isAsync,
                                          out string convertedDocumentUri)
        {
            var docServiceConnector = new DocumentService(
                StudioKeySettings.GetKey(),
                StudioKeySettings.GetSKey(),
                TenantStatisticsProvider.GetUsersCount())
                {
                    ConvertTimeout = ConvertTimeout
                };
            try
            {
                return docServiceConnector.GetConvertedUri(
                    DocumentConverterUrl,
                    documentUri,
                    fromExtension,
                    toExtension,
                    documentRevisionId,
                    isAsync,
                    out convertedDocumentUri);
            }
            catch (Exception e)
            {
                throw CustomizeError(e.Message);
            }
        }

        public static string GetExternalUri(Stream fileStream, string contentType, string documentRevisionId)
        {
            var docServiceConnector = new DocumentService(
                StudioKeySettings.GetKey(),
                StudioKeySettings.GetSKey(),
                TenantStatisticsProvider.GetUsersCount())
                {
                    ConvertTimeout = ConvertTimeout
                };
            try
            {
                return docServiceConnector.GetExternalUri(
                    DocumentStorageUrl,
                    fileStream,
                    contentType,
                    documentRevisionId);
            }
            catch (Exception e)
            {
                throw CustomizeError(e.Message);
            }
        }

        private static Exception CustomizeError(string errorMessage)
        {
            var error = FilesCommonResource.ErrorMassage_DocServiceException;
            if (!string.IsNullOrEmpty(errorMessage))
                error += string.Format(" ({0})", errorMessage);

            return new Exception(error);
        }
    }
}