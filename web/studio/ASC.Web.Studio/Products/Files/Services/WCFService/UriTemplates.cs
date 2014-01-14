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

namespace ASC.Web.Files.Services.WCFService
{
    internal class UriTemplates
    {
        #region Folder Template

        //  [Create("{folderid}")]
        public const String XMLCreateFolder = "folders/create?parentId={parentId}&title={title}";

        // [Read("{folderid}")]?&fields=folders
        public const String XMLPostSubFolders = "folders/subfolders?parentId={parentId}";

        //  [Update("{folderid}")]?&fields=folders
        public const String XMLRenameFolder = "folders/rename?folderId={folderId}&title={title}";

        // [Read("{folderid}")]
        public const String XMLPostFolderItems = "folders?parentId={parentId}&from={from}&count={count}&filter={filter}&subjectID={subjectID}&search={searchText}";

        #endregion

        #region File Template

        //  [Create("{folderid}/file")]
        public const String XMLCreateNewFile = "folders/files/createfile?parentId={parentId}&title={fileTitle}";

        //  [Update("file/{fileid}")]
        public const String XMLRenameFile = "folders/files/rename?fileId={fileId}&title={title}";

        // [Read("file/{fileid}/history")]
        public const String XMLGetFileHistory = "folders/files/history?fileId={fileId}";

        public const String JSONPostCheckMoveFiles = "folders/files/moveOrCopyFilesCheck?destFolderId={destFolderId}";

        // [Read("{folderid}")]?filterType=ImagesOnly&fields=files
        public const String JSONPostGetSiblingsFile = "folders/files/siblings?fileId={fileId}&filter={filter}&subjectID={subjectID}&search={searchText}";

        // [Update("file/{fileid}")]?lastVersion={version}
        public const String XMLUpdateToVersion = "folders/files/updateToVersion?fileId={fileId}&version={version}";

        // [Read("file/{fileid}/history")]?count=1
        public const String XMLLastFileVersion = "folders/files/getversion?fileId={fileId}&version={version}";

        #endregion

        #region Utils Template

        //[Read("settings/import/{source:(boxnet|google|zoho)}/data")]
        public const String XMLPostGetImportDocs = "import?source={source}";

        //  [Update("settings/import/{source:(boxnet|google|zoho)}/data")]
        public const String JSONPostExecImportDocs = "import/exec?login={login}&password={password}&token={token}&source={source}&tofolder={parentId}&ignoreCoincidenceFiles={ignoreCoincidenceFiles}";

        //  [Read("fileops")]
        public const String JSONGetTasksStatuses = "tasks/statuses";

        // [Update("fileops/terminate")] or [Update("settings/import/terminate")]
        public const String JSONTerminateTasks = "tasks?terminate={import}";

        // [Update("fileops/bulkdownload")]
        public const String JSONPostBulkDownload = "bulkdownload";

        // [Update("fileops/delete")]
        public const String JSONPostDeleteItems = "folders/files?action=delete";

        // [Update("fileops/move")] or [Update("fileops/copy")]
        public const String JSONPostMoveItems = "moveorcopy?destFolderId={destFolderId}&ow={overwriteFiles}&ic={isCopyOperation}";

        // [Update("fileops/emptytrash")]
        public const String JSONEmptyTrash = "emptytrash";

        public const String JSONGetShortenLink = "shorten?fileId={fileId}";

        public const String JSONGetTrackEditFile = "trackeditfile?fileID={fileId}&tabId={tabId}&docKeyForTrack={docKeyForTrack}&doc={shareLinkKey}&isFinish={isFinish}&lockVersion={lockVersion}";

        public const String JSONPostCheckConversion = "checkconversion";

        public const String JSONPostCheckEditing = "checkediting";

        public const String JSONGetCanEdit = "canedit?fileId={fileId}&doc={shareLinkKey}";

        public const String XMLPostLinkToEmail = "sendlinktoemail?fileId={fileId}";

        public const String JSONGetStoreOriginal = "storeoriginal?set={store}";

        public const String JSONGetUpdateIfExist = "updateifexist?set={update}";

        public const String JSONGetHelpCenter = "gethelpcenter";

        #endregion

        #region Ace Tempate

        //  [Read("file/{fileid}/share")]   [Read("folder/{folderid}/share")]
        public const String JSONGetSharedInfo = "sharedinfo?objectId={objectId}";

        public const String JSONGetSharedInfoShort = "sharedinfoshort?objectId={objectId}";

        // [Update("file/{fileid}/share")]  [Update("folder/{folderid}/share")]
        public const String JSONPostSetAceObject = "setaceobject?objectId={objectId}&notify={notify}";

        //  [Delete("folder/{folderid}/share")]  [Delete("file/{fileid}/share")] 
        public const String JSONPostRemoveAce = "removeace";

        // [Update("fileops/markasread")]
        public const String JSONPostMarkAsRead = "markasread";

        public const String JSONGetNewItems = "getnews?folderId={folderId}";

        #endregion

        #region ThirdParty

        // [Read("settings/thirdparty")]
        public const String JSONGetThirdParty = "thirdparty";

        // [Create("settings/thirdparty")]
        public const String JSONPostSaveThirdParty = "thirdparty/save";

        // [Delete("settings/thirdparty/{folderid}")]
        public const String JSONDeleteThirdParty = "thirdparty/delete?providerId={providerId}";

        public const String JSONGetChangeAccessToThirdparty = "thirdparty?enable={enableThirdpartySettings}";

        #endregion
    }
}