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
using System.ServiceModel;
using System.ServiceModel.Web;
using ASC.Files.Core;
using ASC.Web.Files.Services.WCFService.FileOperations;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Services.WCFService
{
    [ServiceContract]
    public interface IFileStorageService
    {
        #region Folder Manager

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostSubFolders, Method = "POST")]
        ItemList<Folder> GetFolders(String parentId, OrderBy orderBy);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLCreateFolder)]
        Folder CreateNewFolder(String title, String parentId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLRenameFolder)]
        Folder FolderRename(String folderId, String title);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostFolderItems, Method = "POST")]
        DataWrapper GetFolderItems(String parentId, String from, String count, String filter, OrderBy orderBy, String subjectID, String searchText);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostCheckMoveFiles, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemDictionary<String, String> MoveOrCopyFilesCheck(ItemList<String> items, String destFolderId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostMoveItems, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> MoveOrCopyItems(ItemList<String> items, String destFolderId, String overwriteFiles, String isCopyOperation);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostDeleteItems, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> DeleteItems(ItemList<String> items);

        ItemList<FileOperationResult> DeleteItems(ItemList<String> items, bool ignoreException);

        IEnumerable<Folder> GetFolderUpdates(DateTime from, DateTime to);

        #endregion

        #region File Manager

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLLastFileVersion)]
        File GetFile(String fileId, int version);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLCreateNewFile)]
        File CreateNewFile(String parentId, String fileTitle);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLRenameFile)]
        File FileRename(String fileId, String title);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLUpdateToVersion)]
        File UpdateToVersion(String fileId, int version);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.XMLGetFileHistory)]
        ItemList<File> GetFileHistory(String fileId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostGetSiblingsFile, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        KeyValuePair<string, ItemDictionary<String, String>> GetSiblingsFile(String fileId, String filter, OrderBy orderBy, String subjectID, String searchText);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetTrackEditFile, ResponseFormat = WebMessageFormat.Json)]
        KeyValuePair<bool, String> TrackEditFile(String fileId, Guid tabId, String docKeyForTrack, String shareLinkKey, bool isFinish, bool lockVersion);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostCheckEditing, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemDictionary<String, String> CheckEditing(ItemList<String> filesId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetCanEdit, ResponseFormat = WebMessageFormat.Json)]
        Guid CanEdit(String fileId, String shareLinkKey);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostCheckConversion, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> CheckConversion(ItemList<ItemList<String>> filesIdVersion);

        IEnumerable<File> GetFileUpdates(DateTime from, DateTime to);

        #endregion

        #region Utils

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostBulkDownload, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> BulkDownload(ItemDictionary<String, String> items);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetTasksStatuses, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> GetTasksStatuses();

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONEmptyTrash, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> EmptyTrash();

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONTerminateTasks, ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> TerminateTasks(bool import);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetShortenLink, ResponseFormat = WebMessageFormat.Json)]
        String GetShortenLink(String fileId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostLinkToEmail, Method = "POST")]
        void SendLinkToEmail(String fileId, ItemDictionary<String, ItemList<String>> messageAddresses);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetStoreOriginal, ResponseFormat = WebMessageFormat.Json)]
        bool StoreOriginal(bool store);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetUpdateIfExist, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateIfExist(bool update);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetHelpCenter, ResponseFormat = WebMessageFormat.Json)]
        String GetHelpCenter();

        #endregion

        #region Import

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.XMLPostGetImportDocs, Method = "POST")]
        ItemList<DataToImport> GetImportDocs(String source, AuthData authData);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostExecImportDocs, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> ExecImportDocs(String login, String password, String token, String source, String parentId, String ignoreCoincidenceFiles, List<DataToImport> dataToImport);

        #endregion

        #region Ace Manager

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetSharedInfo, ResponseFormat = WebMessageFormat.Json)]
        ItemList<AceWrapper> GetSharedInfo(String objectId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetSharedInfoShort, ResponseFormat = WebMessageFormat.Json)]
        ItemList<AceShortWrapper> GetSharedInfoShort(String objectId);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostSetAceObject, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        bool SetAceObject(ItemList<AceWrapper> aceWrappers, String objectId, bool notify);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostRemoveAce, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        void RemoveAce(ItemList<String> items);

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostMarkAsRead, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ItemList<FileOperationResult> MarkAsRead(ItemList<String> items);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetNewItems)]
        ItemList<FileEntry> GetNewItems(String folderId);

        #endregion

        #region ThirdParty

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetThirdParty, ResponseFormat = WebMessageFormat.Json)]
        ItemList<ThirdPartyParams> GetThirdParty();

        [OperationContract]
        [WebInvoke(UriTemplate = UriTemplates.JSONPostSaveThirdParty, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        Folder SaveThirdParty(ThirdPartyParams thirdPartyParams);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONDeleteThirdParty, ResponseFormat = WebMessageFormat.Json)]
        void DeleteThirdParty(String providerId);

        [OperationContract]
        [WebGet(UriTemplate = UriTemplates.JSONGetChangeAccessToThirdparty, ResponseFormat = WebMessageFormat.Json)]
        bool ChangeAccessToThirdparty(bool enableThirdpartySettings);

        #endregion
    }
}