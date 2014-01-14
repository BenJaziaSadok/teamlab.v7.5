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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Documents;
using ASC.Api.Exceptions;
using ASC.Api.Impl;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using OrderBy = ASC.CRM.Core.Entities.OrderBy;

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///   Returns the list of all events matching the parameters specified in the request
        /// </summary>
        /// <short>
        ///   Get event list
        /// </short>
        /// <category>History</category>
        /// <param optional="true" name="entityType" remark="Allowed values: opportunity, contact or case">Related entity type</param>
        /// <param optional="true" name="entityId">Related entity ID</param>
        /// <param optional="true" name="categoryId">Task category ID</param>
        /// <param optional="true" name="createBy">Event author</param>
        /// <param optional="true" name="fromDate">Earliest task due date</param>
        /// <param optional="true" name="toDate">Latest task due date</param>
        /// <returns>
        ///   Event list
        /// </returns>
        [Read("history/filter")]
        public IEnumerable<RelationshipEventWrapper> GetHistory(
            String entityType,
            int entityId,
            int categoryId,
            Guid createBy,
            ApiDateTime fromDate,
            ApiDateTime toDate)
        {

            if (!String.IsNullOrEmpty(entityType) && !(
                                                          String.Compare(entityType, "contact", true) == 0 ||
                                                          String.Compare(entityType, "opportunity", true) == 0 ||
                                                          String.Compare(entityType, "case", true) == 0))
                throw new ArgumentException();

            RelationshipEventByType eventByType;

            IEnumerable<RelationshipEventWrapper> result;

            OrderBy eventOrderBy;

            if (Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out eventByType))
                eventOrderBy = new OrderBy(eventByType, !_context.SortDescending);
            else if (String.IsNullOrEmpty(_context.SortBy))
                eventOrderBy = new OrderBy(RelationshipEventByType.Created, false);
            else
                eventOrderBy = null;

            if (eventOrderBy != null)
            {

                result = ToListRelationshipEventWrapper(DaoFactory.GetRelationshipEventDao().GetItems(
                    _context.FilterValue,
                    ToEntityType(entityType),
                    entityId,
                    createBy,
                    categoryId,
                    fromDate,
                    toDate,
                    (int)_context.StartIndex,
                    (int)_context.Count,
                    eventOrderBy));

                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();
            }
            else
                result = ToListRelationshipEventWrapper(DaoFactory.GetRelationshipEventDao().GetItems(
                    _context.FilterValue,
                    ToEntityType(entityType),
                    entityId,
                    createBy,
                    categoryId,
                    fromDate,
                    toDate,
                    0,
                    0,
                    null));

            return result.ToSmartList();
        }

        /// <summary>
        ///     Deletes the event with the ID specified in the request and all the files associated with this event
        /// </summary>
        /// <short>
        ///     Delete event and related files
        /// </short>
        /// <category>History</category>
        /// <param name="id">Event ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Event
        /// </returns>
        [Delete("history/{id:[0-9]+}")]
        public RelationshipEventWrapper DeleteHistory(int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var item = DaoFactory.GetRelationshipEventDao().GetByID(id);

            if (item == null)
                throw new ItemNotFoundException();

            DaoFactory.GetRelationshipEventDao().DeleteItem(id);

            return ToRelationshipEventWrapper(item);

        }


        /// <summary>
        /// Creates a text (.txt) file in the selected folder with the title and contents sent in the request
        /// </summary>
        /// <short>Create txt</short>
        /// <category>Files</category>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="title">File title</param>
        /// <param name="content">File contents</param>
        /// <returns>
        ///     File info
        /// </returns>
        [Create("{entityType:(contact|opportunity|case)}/{entityid:[0-9]+}/files/text")]
        public FileWrapper CreateTextFile(
             String entityType,
             int entityid,
             String title,
             String content)
        {
            if (title == null) throw new ArgumentNullException("title");
            if (content == null) throw new ArgumentNullException("content");

            var folderid = GetRootFolderID();

            FileWrapper result;

            var extension = ".txt";
            if (!string.IsNullOrEmpty(content))
            {
                if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
                {
                    extension = ".html";
                }
            }

            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                title = title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension);

                result = SaveFile(folderid, memStream, title);
            }

            AttachFiles(entityType, entityid, new List<int> { (int)result.Id });

            return result;
        }

        /// <summary>
        /// Upload file 
        /// </summary>
        /// <short>Upload file</short>
        /// <category>Files</category>
        /// <remarks>
        /// <![CDATA[
        ///  Upload can be done in 2 different ways:
        ///  <ol>
        /// <li>Single file upload. You should set Content-Type &amp; Content-Disposition header to specify filename and content type, and send file in request body</li>
        /// <li>Using standart multipart/form-data method</li>
        /// </ol>]]>
        /// </remarks>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="file" visible="false">Request Input stream</param>
        /// <param name="contentType" visible="false">Content-Type Header</param>
        /// <param name="contentDisposition" visible="false">Content-Disposition Header</param>
        /// <param name="files" visible="false">List of files when posted as multipart/form-data</param>
        /// <param name="storeOriginalFileFlag" visible="false">If True, upload documents in original formats as well</param>
        /// <returns>
        /// File info
        /// </returns>
        [Create("{entityType:(contact|opportunity|case)}/{entityid:[0-9]+}/files/upload")]
        public FileWrapper UploadFileInCRM(String entityType,
                                           int entityid,
                                           Stream file,
                                           ContentType contentType,
                                           ContentDisposition contentDisposition,
                                           IEnumerable<System.Web.HttpPostedFileBase> files,
                                           bool storeOriginalFileFlag
            )
        {
            FilesSettings.StoreOriginalFiles = storeOriginalFileFlag;

            var folderid = GetRootFolderID();

            FileWrapper uploadedFile = null;
            if (files != null && files.Any())
            {
                //For case with multiple files
                foreach (var postedFile in files)
                {
                    uploadedFile = SaveFile(folderid, postedFile.InputStream, postedFile.FileName);
                }
            }
            else if (file != null)
            {
                uploadedFile = SaveFile(folderid, file, contentDisposition.FileName);
            }

            return uploadedFile;
        }

        private static FileWrapper SaveFile(object folderid, Stream file, string fileName)
        {
            var resultFile = FileUploader.Exec(folderid.ToString(), fileName, file.Length, file);

            return new FileWrapper(resultFile);
        }
        

        /// <summary>
        ///   Creates the event with the parameters specified in the request
        /// </summary>
        /// <short>
        ///   Create event
        /// </short>
        /// <category>History</category>
        /// <param optional="true"  name="contactId">Contact ID</param>
        /// <param optional="true"  name="entityType" remark="Allowed values: opportunity or case">Related entity type</param>
        /// <param optional="true"  name="entityId">Related entity ID</param>
        /// <param name="content">Contents</param>
        /// <param name="categoryId">Category ID</param>
        /// <param name="created">Event creation date</param>
        /// <param optional="true" name="fileId">List of IDs of the files associated with the event</param>
        /// <param optional="true" name="notifyUserList">User field list</param>
        /// <returns>
        ///   Created event
        /// </returns>
        [Create("history")]
        public RelationshipEventWrapper AddHistoryTo(
            String entityType,
            int entityId,
            int contactId,
            String content,
            int categoryId,
            ApiDateTime created,
            IEnumerable<int> fileId,
            IEnumerable<Guid> notifyUserList)
        {

            if (!String.IsNullOrEmpty(entityType) &&
                !(String.Compare(entityType, "opportunity", true) == 0 ||
                  String.Compare(entityType, "case", true) == 0))
                throw new ArgumentException();

            var relationshipEvent = new RelationshipEvent
                                        {
                                            CategoryID = categoryId,
                                            EntityType = ToEntityType(entityType),
                                            EntityID = entityId,
                                            Content = content,
                                            ContactID = contactId,
                                            CreateOn = created,
                                            CreateBy = Core.SecurityContext.CurrentAccount.ID
                                        };



            if (DaoFactory.GetListItemDao().GetByID(categoryId) == null)
                throw new ArgumentException();

            var item = DaoFactory.GetRelationshipEventDao().CreateItem(relationshipEvent);

            var fileListInfoHashtable = new Hashtable();

            if (fileId != null)
            {
                var fileIds = fileId.ToList();

                var files = FilesDaoFactory.GetFileDao().GetFiles(fileIds.Cast<object>().ToArray());

                foreach (var file in files)
                {
                    var fileInfo = String.Format("{0} ({1})", file.Title, Path.GetExtension(file.Title).ToUpper());
                    fileListInfoHashtable.Add(fileInfo, file.ViewUrl);
                }

                DaoFactory.GetRelationshipEventDao().AttachFiles(item.ID, fileIds.ToArray());
            }

            if (notifyUserList != null && notifyUserList.Count() > 0)
                NotifyClient.Instance.SendAboutAddRelationshipEventAdd(item, fileListInfoHashtable, notifyUserList.ToArray());

            return ToRelationshipEventWrapper(item);
        }

        /// <summary>
        ///     Associates the selected file(s) with the entity with the ID or type specified in the request
        /// </summary>
        /// <short>
        ///     Associate file with entity
        /// </short>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <param name="fileids">List of IDs of the files</param>
        /// <category>Files</category>
        /// <returns>Entity with the file attached</returns>
        [Create("{entityType:(contact|opportunity|case)}/{entityid:[0-9]+}/files")]
        public RelationshipEventWrapper AttachFiles(String entityType, int entityid, IEnumerable<int> fileids)
        {

            if (entityid <= 0)
                throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                    return ToRelationshipEventWrapper(DaoFactory.GetRelationshipEventDao().AttachFiles(entityid, EntityType.Any, 0, fileids.ToArray()));
                case EntityType.Opportunity:
                case EntityType.Case:
                    return ToRelationshipEventWrapper(DaoFactory.GetRelationshipEventDao().AttachFiles(0, entityTypeObj, entityid, fileids.ToArray()));
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Returns the ID for the root folder used to store the files for the CRM module
        /// </summary>
        /// <short>Get root folder ID</short> 
        /// <category>Files</category>
        /// <returns>
        ///   Root folder ID
        /// </returns>
        [Read("files/root")]
        public object GetRootFolderID()
        {
            return DaoFactory.GetFileDao().GetRoot();
        }

        /// <summary>
        ///    Returns the list of all files for the entity with the ID or type specified in the request
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityid">Entity ID</param>
        /// <short>Get file list</short> 
        /// <category>Files</category>
        /// <returns>
        ///    File list
        /// </returns>
        [Read("{entityType:(contact|opportunity|case)}/{entityid:[0-9]+}/files")]
        public IEnumerable<FileWrapper> GetFiles(String entityType, int entityid)
        {
            if (entityid <= 0)
                throw new ArgumentException();

            var entityTypeObj = ToEntityType(entityType);

            switch (entityTypeObj)
            {
                case EntityType.Contact:
                    return DaoFactory.GetRelationshipEventDao().GetAllFiles(new[] { entityid }, EntityType.Any, 0)
                        .ConvertAll(file => new FileWrapper(file));
                case EntityType.Opportunity:
                case EntityType.Case:
                    return DaoFactory.GetRelationshipEventDao().GetAllFiles(null, entityTypeObj, entityid)
                        .ConvertAll(file => new FileWrapper(file));
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Deletes the file with the ID specified in the request
        /// </summary>
        /// <short>Delete file</short> 
        /// <category>Files</category>
        /// <param name="fileid">File ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    File Info
        /// </returns>
        [Delete("files/{fileid:[0-9]+}")]
        public FileWrapper DeleteCRMFile(int fileid)
        {
            if (fileid < 0)
                throw new ArgumentException();

            var file = FilesDaoFactory.GetFileDao().GetFile(fileid);

            if (file == null)
                throw new ItemNotFoundException();

            var result = new FileWrapper(file);

            DaoFactory.GetRelationshipEventDao().RemoveFile(file);

            return result;
        }

        private IEnumerable<RelationshipEventWrapper> ToListRelationshipEventWrapper(List<RelationshipEvent> itemList)
        {

            if (itemList.Count == 0) return new List<RelationshipEventWrapper>();

            var result = new List<RelationshipEventWrapper>();

            var contactIDs = new List<int>();
            var eventIDs = new List<int>();
            var categoryIDs = new List<int>();
            var entityWrappersIDs = new Dictionary<EntityType, List<int>>();


            foreach (var item in itemList)
            {
                eventIDs.Add(item.ID);

                if (!categoryIDs.Contains(item.CategoryID))
                    categoryIDs.Add(item.CategoryID);

                if (item.ContactID > 0 && !contactIDs.Contains(item.ContactID))
                    contactIDs.Add(item.ContactID);

                if (item.EntityID > 0)
                {
                    if (!entityWrappersIDs.ContainsKey(item.EntityType))
                        entityWrappersIDs.Add(item.EntityType, new List<int>
                                                                   {
                                                                       item.EntityID
                                                                   });
                    else if (!entityWrappersIDs[item.EntityType].Contains(item.EntityID))
                        entityWrappersIDs[item.EntityType].Add(item.EntityID);
                }
            }

            var entityWrappers = new Dictionary<String, EntityWrapper>();

            foreach (EntityType entityType in entityWrappersIDs.Keys)
            {

                switch (entityType)
                {
                    case EntityType.Opportunity:
                        DaoFactory.GetDealDao().GetDeals(entityWrappersIDs[entityType].Distinct().ToArray())
                            .ForEach(item =>
                                         {

                                             if (item == null) return;

                                             entityWrappers.Add(
                                                 String.Format("{0}_{1}", (int)entityType, item.ID),
                                                 new EntityWrapper
                                                     {
                                                         EntityId = item.ID,
                                                         EntityTitle = item.Title,
                                                         EntityType = "opportunity"
                                                     });
                                         });
                        break;
                    case EntityType.Case:
                        DaoFactory.GetCasesDao().GetByID(entityWrappersIDs[entityType].ToArray())
                            .ForEach(item =>
                                         {

                                             if (item == null) return;

                                             entityWrappers.Add(
                                                 String.Format("{0}_{1}", (int)entityType, item.ID),
                                                 new EntityWrapper
                                                     {
                                                         EntityId = item.ID,
                                                         EntityTitle = item.Title,
                                                         EntityType = "case"
                                                     });
                                         });
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            var categories = DaoFactory.GetListItemDao().GetItems(categoryIDs.ToArray()).ToDictionary(x => x.ID, x => new HistoryCategoryBaseWrapper(x));

            var files = DaoFactory.GetRelationshipEventDao().GetFiles(eventIDs.ToArray());

            var contacts = DaoFactory.GetContactDao().GetContacts(contactIDs.ToArray()).ToDictionary(item => item.ID,
                                                                                                     item =>
                                                                                                     ToContactBaseWrapper(item));

            foreach (var item in itemList)
            {
                var eventObjWrap = new RelationshipEventWrapper(item);

                if (contacts.ContainsKey(item.ContactID))
                    eventObjWrap.Contact = contacts[item.ContactID];

                if (item.EntityID > 0)
                {

                    var entityStrKey = String.Format("{0}_{1}", (int)item.EntityType, item.EntityID);

                    if (entityWrappers.ContainsKey(entityStrKey))
                        eventObjWrap.Entity = entityWrappers[entityStrKey];

                }

                if (files.ContainsKey(item.ID))
                    eventObjWrap.Files = files[item.ID].ConvertAll(file => new FileWrapper(file));
                else
                    eventObjWrap.Files = new List<FileWrapper>();

                if (categories.ContainsKey(item.CategoryID))
                    eventObjWrap.Category = categories[item.CategoryID];


                result.Add(eventObjWrap);

            }

            return result;
        }

        private RelationshipEventWrapper ToRelationshipEventWrapper(RelationshipEvent relationshipEvent)
        {
            var result = new RelationshipEventWrapper(relationshipEvent);

            var historyCategory = DaoFactory.GetListItemDao().GetByID(relationshipEvent.CategoryID);

            if (historyCategory != null)
            {
                result.Category = new HistoryCategoryBaseWrapper(historyCategory);
            }

            if (relationshipEvent.EntityID > 0)
                result.Entity = ToEntityWrapper(relationshipEvent.EntityType, relationshipEvent.EntityID);

            result.Files = DaoFactory.GetRelationshipEventDao().GetFiles(relationshipEvent.ID).ConvertAll(file => new FileWrapper(file));

            if (relationshipEvent.ContactID > 0)
            {
                var relativeContact = DaoFactory.GetContactDao().GetByID(relationshipEvent.ContactID);

                if (relativeContact != null)
                    result.Contact = ToContactBaseWrapper(relativeContact);
            }

            result.CanEdit = CRMSecurity.CanAccessTo(relationshipEvent);

            return result;
        }

        private EntityWrapper ToEntityWrapper(EntityType entityType, int entityID)
        {
            if (entityID == 0) return null;

            var result = new EntityWrapper
                             {
                                 EntityId = entityID
                             };

            switch (entityType)
            {
                case EntityType.Case:
                    result.EntityType = "case";

                    var cases = DaoFactory.GetCasesDao().GetByID(entityID);

                    if (cases != null)
                        result.EntityTitle = cases.Title;

                    break;
                case EntityType.Opportunity:
                    result.EntityType = "opportunity";

                    var obj = DaoFactory.GetDealDao().GetByID(entityID);

                    if (obj != null)
                        result.EntityTitle = obj.Title;
                    break;
                default:
                    return null;
            }

            return result;
        }
    }
}