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

using System.Collections.Generic;
using System.Web;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Masters.MasterResources
{
    public class MasterFileUtilityResources : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Files.Utility.Resource"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("ExtsImagePreviewed", FileUtility.ExtsImagePreviewed);
            yield return RegisterObject("ExtsWebPreviewed", FileUtility.ExtsWebPreviewed);
            yield return RegisterObject("ExtsWebEdited", FileUtility.ExtsWebEdited);
            yield return RegisterObject("ExtsCoAuthoring", FileUtility.ExtsCoAuthoring);

            yield return RegisterObject("ExtsMustConvert", FileUtility.ExtsMustConvert);
            yield return RegisterObject("ExtsConvertible", FileUtility.ExtsConvertible);
            yield return RegisterObject("ExtsUploadable", FileUtility.ExtsUploadable);

            yield return RegisterObject("ExtsArchive", FileUtility.ExtsArchive);
            yield return RegisterObject("ExtsVideo", FileUtility.ExtsVideo);
            yield return RegisterObject("ExtsAudio", FileUtility.ExtsAudio);
            yield return RegisterObject("ExtsImage", FileUtility.ExtsImage);
            yield return RegisterObject("ExtsSpreadsheet", FileUtility.ExtsSpreadsheet);
            yield return RegisterObject("ExtsPresentation", FileUtility.ExtsPresentation);
            yield return RegisterObject("ExtsDocument", FileUtility.ExtsDocument);

            yield return RegisterObject("InternalFormats", FileUtility.InternalExtension);

            yield return RegisterObject("ParamVersion", CommonLinkUtility.Version);
            yield return RegisterObject("ParamOutType", CommonLinkUtility.OutType);

            yield return RegisterObject("FileViewUrlString", CommonLinkUtility.FileViewUrlString);
            yield return RegisterObject("FileDownloadUrlString", CommonLinkUtility.FileDownloadUrlString);
            yield return RegisterObject("FileWebViewerUrlString", CommonLinkUtility.FileWebViewerUrlString);
            yield return RegisterObject("FileWebViewerExternalUrlString", CommonLinkUtility.FileWebViewerExternalUrlString);
            yield return RegisterObject("FileWebEditorUrlString", CommonLinkUtility.FileWebEditorUrlString);
            yield return RegisterObject("FileWebEditorExternalUrlString", CommonLinkUtility.FileWebEditorExternalUrlString);
        }

        protected override string GetCacheHash()
        {
            var hash =
                FileUtility.ExtsImagePreviewed.GetHashCode() +
                FileUtility.ExtsWebPreviewed.GetHashCode() +
                FileUtility.ExtsWebEdited.GetHashCode() +
                FileUtility.ExtsCoAuthoring.GetHashCode() +
                FileUtility.ExtsMustConvert.GetHashCode() +
                FileUtility.ExtsConvertible.GetHashCode();
            return hash.ToString();
        }
    }
}