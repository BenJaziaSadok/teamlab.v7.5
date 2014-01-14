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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using ASC.Data.Storage.S3;
using ASC.Mail.Aggregator.DbSchema;
using ASC.Mail.Aggregator.Exceptions;
using ASC.Mail.Aggregator.Extension;
using ASC.Mail.Aggregator.Utils;
using ASC.Web.Files.Api;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using System.IO;
using System.Web;
using ASC.Common.Data;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;
using ASC.Data.Storage;
using HtmlAgilityPack;
using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        #region private defines

// ReSharper disable InconsistentNaming
        private const long ATTACHMENTS_TOTAL_SIZE_LIMIT = 25 * 1024 * 1024; // 25 megabytes
// ReSharper restore InconsistentNaming

        private delegate SqlInsert CreateInsertDelegate();

        #endregion

        #region public methods

        public MailAttachment AttachFileFromDocuments(int id_tenant, string id_user, int id_message, string id_file,
                                                          string version, string share_link, string id_stream)
        {
            MailAttachment result;

            using (var file_dao = FilesIntegration.GetFileDao())
            {
                Files.Core.File file;
                var check_link = FileShareLink.Check(share_link, true, file_dao, out file);
                if (!check_link && file == null)
                    file = String.IsNullOrEmpty(version)
                               ? file_dao.GetFile(id_file)
                               : file_dao.GetFile(id_file, Convert.ToInt32(version));

                if (file == null)
                    throw new AttachmentsException(AttachmentsException.Types.DOCUMENT_NOT_FOUND, "File not found.");

                if (!check_link && !FilesIntegration.GetFileSecurity().CanRead(file))
                    throw new AttachmentsException(AttachmentsException.Types.DOCUMENT_ACCESS_DENIED,
                                                   "Access denied.");

                if (!file_dao.IsExistOnStorage(file))
                {
                    throw new AttachmentsException(AttachmentsException.Types.DOCUMENT_NOT_FOUND,
                                                   "File not exists on storage.");
                }

                var file_ext = FileUtility.GetFileExtension(file.Title);
                var cur_file_type = FileUtility.GetFileTypeByFileName(file.Title);

                if (file.ConvertedType != null)
                {
                    if (cur_file_type == FileType.Image)
                        file_ext = file.ConvertedType == ".zip" ? ".pptt" : file.ConvertedType;
                    else if (cur_file_type == FileType.Spreadsheet)
                        file_ext = file.ConvertedType != ".xlsx" ? ".xlst" : file.ConvertedType;
                    else if (file.ConvertedType == ".doct" || file.ConvertedType == ".xlst" || file.ConvertedType == ".pptt")
                        file_ext = file.ConvertedType;
                }

                var convert_to_ext = string.Empty;
                switch (cur_file_type)
                {
                    case FileType.Document:
                        if (file_ext == ".doct")
                            convert_to_ext = ".docx";
                        break;
                    case FileType.Spreadsheet:
                        if (file_ext == ".xlst")
                            convert_to_ext = ".xlsx";
                        break;
                    case FileType.Presentation:
                        if (file_ext == ".pptt")
                            convert_to_ext = ".pptx";
                        break;
                }

                if (!string.IsNullOrEmpty(convert_to_ext) && file_ext != convert_to_ext)
                {
                    var file_name = Path.ChangeExtension(file.Title, convert_to_ext);

                    using (var read_stream = FileConverter.Exec(file, convert_to_ext))
                    {
                        if (read_stream != null)
                        {
                            using (var mem_stream = new MemoryStream())
                            {
                                read_stream.StreamCopyTo(mem_stream);
                                result = AttachFile(id_tenant, id_user, id_message, file_name, mem_stream,
                                                    id_stream);
                            }
                        }
                        else
                            throw new AttachmentsException(AttachmentsException.Types.DOCUMENT_ACCESS_DENIED,
                                                           "Access denied.");
                    }
                }
                else
                {
                    using (var read_stream = file_dao.GetFileStream(file))
                    {
                        if (read_stream != null)
                            result = AttachFile(id_tenant, id_user, id_message, file.Title, read_stream, id_stream);
                        else
                            throw new AttachmentsException(AttachmentsException.Types.DOCUMENT_ACCESS_DENIED,
                                                           "Access denied.");
                    }
                }
            }

            return result;
        }

        public MailAttachment AttachFile(int id_tenant, string id_user, int id_message,
            string name, Stream input_stream, string id_stream)
        {
            if (id_message < 0)
                throw new AttachmentsException(AttachmentsException.Types.BAD_PARAMS, "Field 'id_message' must have non-negative value.");

            if (id_tenant < 0)
                throw new AttachmentsException(AttachmentsException.Types.BAD_PARAMS, "Field 'id_tenant' must have non-negative value.");

            if (String.IsNullOrEmpty(id_user))
                throw new AttachmentsException(AttachmentsException.Types.BAD_PARAMS, "Field 'id_user' is empty.");

            if (input_stream.Length == 0)
                throw new AttachmentsException(AttachmentsException.Types.EMPTY_FILE, "Empty files not supported.");


            if (string.IsNullOrEmpty(id_stream))
                throw new AttachmentsException(AttachmentsException.Types.MESSAGE_NOT_FOUND, "Message not found.");

            var total_size = GetAttachmentsTotalSize(id_message) + input_stream.Length;

            if (total_size > ATTACHMENTS_TOTAL_SIZE_LIMIT)
                throw new AttachmentsException(AttachmentsException.Types.TOTAL_SIZE_EXCEEDED, "Total size of all files exceeds limit!");

            var file_number = GetMaxAttachmentNumber(id_message);

            var attachment = new MailAttachment
            {
                fileName = name,
                contentType = MimeMapping.GetMimeMapping(name),
                fileNumber = file_number,
                size = input_stream.Length,
                data = input_stream.GetCorrectBuffer(),
                streamId = id_stream,
                tenant = id_tenant,
                user = id_user
            };

            QuotaUsedAdd(id_tenant, input_stream.Length);
            try
            {
                StoreAttachmentWithoutQuota(id_tenant, id_user, attachment);
            }
            catch
            {
                QuotaUsedDelete(id_tenant, input_stream.Length);
                throw;
            }

            try
            {
                using (var db = GetDb())
                {
                    using (var tx = db.BeginTransaction())
                    {
                        attachment.fileId = SaveAttachment(db, id_tenant, id_message, attachment);
                        UpdateMessageChainAttachmentsFlag(db, id_tenant, id_user, id_message);

                        tx.Commit();
                    }
                }
            }
            catch
            {
                //TODO: If exception has happened, need to remove stored files from s3 and remove quota
                throw;
            }

            return attachment;
        }

        public int GetAttachmentId(int message_id, string content_id)
        {
            using (var db = GetDb())
            {
                var query = new SqlQuery(AttachmentTable.name)
                                .Select(AttachmentTable.Columns.id)
                                .Where(AttachmentTable.Columns.id_mail, message_id)
                                .Where(AttachmentTable.Columns.need_remove, false)
                    .Where(AttachmentTable.Columns.content_id, content_id);

                var attach_id = db.ExecuteScalar<int>(query);
                return attach_id;
            }
        }

        public void StoreAttachmentCopy(int id_tenant, string id_user, MailAttachment attachment, string stream_id)
        {
            try
            {
                if (!attachment.streamId.Equals(stream_id))
                {
                    var s3_key = attachment.GerStoredFilePath();

                        var data_client = GetDataStore(id_tenant);

                        if (data_client.IsFile(s3_key))
                        {
                        attachment.fileNumber =
                            !string.IsNullOrEmpty(attachment.contentId) //Upload hack: embedded attachment have to be saved in 0 folder
                                ? 0
                                : attachment.fileNumber;
                        
                        var new_s3_key = MailStoragePathCombiner.GetFileKey(id_user, stream_id, attachment.fileNumber,
                                                                            attachment.storedName);

                            var copy_s3_url = data_client.Copy(s3_key, string.Empty, new_s3_key);

                        attachment.storedFileUrl = MailStoragePathCombiner.GetStoredUrl(copy_s3_url);

                        attachment.streamId = stream_id;
                        }
                    }
                }
            catch (Exception ex)
            {
                _log.Error("CopyAttachment(). filename: {0}, ctype: {1} Exception:\r\n{2}\r\n",
                           attachment.fileName,
                           attachment.contentType,
                    ex.ToString());

                throw;
            }
        }

        public void StoreAttachmentWithoutQuota(int id_tenant, string id_user, MailAttachment attachment)
        {
            StoreAttachmentWithoutQuota(id_tenant, id_user, attachment, GetDataStore(id_tenant));
        }

        public void StoreAttachmentWithoutQuota(int id_tenant, string id_user, MailAttachment attachment, IDataStore storage)
        {
            try
            {
                if (attachment.data == null || attachment.data.Length == 0)
                    return;

                if (string.IsNullOrEmpty(attachment.fileName))
                    attachment.fileName = "attachment.ext";

                var content_disposition = MailStoragePathCombiner.PrepareAttachmentName(attachment.fileName);

                var ext = Path.GetExtension(attachment.fileName);

                attachment.storedName = CreateNewStreamId();

                if (!string.IsNullOrEmpty(ext))
                    attachment.storedName = Path.ChangeExtension(attachment.storedName, ext);

                attachment.fileNumber =
                    !string.IsNullOrEmpty(attachment.contentId) //Upload hack: embedded attachment have to be saved in 0 folder
                        ? 0
                        : attachment.fileNumber;

                var attachment_path = attachment.GerStoredFilePath();

                using (var reader = new MemoryStream(attachment.data))
                {
                    var upload_url = storage.UploadWithoutQuota(string.Empty, attachment_path, reader, attachment.contentType, content_disposition);
                    attachment.storedFileUrl = MailStoragePathCombiner.GetStoredUrl(upload_url);
                }
            }
            catch (Exception e)
            {
                _log.Error("StoreAttachmentWithoutQuota(). filename: {0}, ctype: {1} Exception:\r\n{2}\r\n",
                    attachment.fileName,
                    attachment.contentType,
                    e.ToString());

                throw;
            }
        }

        public void DeleteMessageAttachments(int tenant, string id_user, int message_id, IEnumerable<int> attachment_ids)
        {
            var ids = attachment_ids.ToArray();
            long used_quota;
            using (var db = GetDb())
            {
                using (var tx = db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    used_quota = MarkAttachmetsNeedRemove(db, tenant,
                                                          Exp.And(Exp.Eq(AttachmentTable.Columns.id_mail, message_id),
                                                                  Exp.In(AttachmentTable.Columns.id, ids)));
                    ReCountAttachments(db, message_id);
                    UpdateMessageChainAttachmentsFlag(db, tenant, id_user, message_id);

                    tx.Commit();
                }
            }

            if (used_quota > 0)
                QuotaUsedDelete(tenant, used_quota);
        }

        public MailAttachment GetMessageAttachment(int attach_id, int id_tenant, string id_user = null)
        {
            using (var db = GetDb())
            {
                var query = GetAttachmentsSelectQuery()
                        .Where(AttachmentTable.Columns.id.Prefix(AttachmentTable.name), attach_id)
                    .Where(AttachmentTable.Columns.need_remove.Prefix(AttachmentTable.name), false);

                var where_exp = string.IsNullOrEmpty(id_user)
                                    ? Exp.Eq(MailTable.Columns.id_tenant.Prefix(MailTable.name), id_tenant)
                                    : GetUserWhere(id_user, id_tenant, MailTable.name);

                query.Where(where_exp);

                var attachment = db.ExecuteList(query)
                                   .ConvertAll(ToMailItemAttachment)
                        .FirstOrDefault();

                return attachment;
            }
        }

        public long GetAttachmentsTotalSize(int id_mail)
        {
            using (var db = GetDb())
            {
                long total_size = db.ExecuteList(
                    new SqlQuery(AttachmentTable.name)
                        .SelectSum(AttachmentTable.Columns.size)
                        .Where(AttachmentTable.Columns.id_mail, id_mail)
                        .Where(AttachmentTable.Columns.need_remove, false)
                        .Where(Exp.Eq(AttachmentTable.Columns.content_id, Exp.Empty)))
                        .ConvertAll(r => Convert.ToInt64(r[0]))
                        .FirstOrDefault();

                return total_size;
            }
        }

        public int GetMaxAttachmentNumber(int id_mail)
        {
            using (var db = GetDb())
            {
                var number = db.ExecuteList(
                    new SqlQuery(AttachmentTable.name)
                        .SelectMax(AttachmentTable.Columns.file_number)
                        .Where(AttachmentTable.Columns.id_mail, id_mail))
                        .ConvertAll(r => Convert.ToInt32(r[0]))
                        .FirstOrDefault();

                number++;

                return number;
            }
        }

        public int SaveAttachmentInTransaction(int id_tenant, int id_mail, MailAttachment attachment)
        {
            int id;
            using (var db = GetDb())
            {
                using (var tx = db.BeginTransaction())
                {
                    id = SaveAttachment(db, id_tenant, id_mail, attachment);

                    tx.Commit();
                }
            }
            return id;
        }

        public string StoreMailBody(int id_tenant, string id_user, MailMessageItem mail)
        {
            try
            {
                if (!string.IsNullOrEmpty(mail.HtmlBody))
                {
                    var safe_body = ReplaceEmbeddedImages(mail);

                    using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(safe_body)))
                    {
                        // Using id_user as domain in S3 Storage - allows not to add quota to tenant.
                        var save_body_path = MailStoragePathCombiner.GetBodyKey(id_user, mail.StreamId);

                        return GetDataStore(id_tenant)
                            .UploadWithoutQuota(string.Empty, save_body_path, reader, "text/html", string.Empty)
                            .ToString();
                    }
                }

                return string.Empty;
            }
            catch (Exception)
            {
                GetDataStore(id_tenant).Delete(string.Empty, MailStoragePathCombiner.GetBodyKey(id_user, mail.StreamId));
                _log.Debug(String.Format("Problems with message saving in messageId={0}. Body was deleted.", mail.MessageId));
                throw;
            }
        }

        public void SaveAttachments(int id_tenant, int id_mail, List<MailAttachment> attachments)
        {
            using (var db = GetDb())
            {
                using (var tx = db.BeginTransaction())
                {
                    SaveAttachments(db, id_tenant, id_mail, attachments);
                    tx.Commit();
                }
            }
        }

        public void SaveAttachments(DbManager db, int id_tenant, int id_mail, List<MailAttachment> attachments)
        {
            if (!attachments.Any()) return;

            CreateInsertDelegate create_insert_query = () => new SqlInsert(AttachmentTable.name)
                                                         .InColumns(AttachmentTable.Columns.id_mail,
                                                                    AttachmentTable.Columns.name,
                                                                    AttachmentTable.Columns.stored_name,
                                                                    AttachmentTable.Columns.type,
                                                                    AttachmentTable.Columns.size,
                                                                    AttachmentTable.Columns.file_number,
                                                                    AttachmentTable.Columns.need_remove,
                                                                    AttachmentTable.Columns.content_id,
                                                                    AttachmentTable.Columns.id_tenant);

            var insert_query = create_insert_query();

            int i, len;

            for (i = 0, len = attachments.Count; i < len; i++)
            {
                var attachment = attachments[i];

                insert_query
                    .Values(id_mail, attachment.fileName, attachment.storedName, attachment.contentType, attachment.size,
                            attachment.fileNumber, 0, attachment.contentId, id_tenant);

                if (i % 100 == 0 && i != 0 || i + 1 == len)
                {
                    db.ExecuteNonQuery(insert_query);
                    insert_query = create_insert_query();
                }
            }

            ReCountAttachments(db, id_mail);
        }

        public int SaveAttachment(int id_tenant, int id_mail, MailAttachment attachment)
        {
            using (var db = GetDb())
            {
                return SaveAttachment(db, id_tenant, id_mail, attachment);
            }
        }

        public int SaveAttachment(DbManager db, int id_tenant, int id_mail, MailAttachment attachment)
        {
            return SaveAttachment(db, id_tenant, id_mail, attachment, true);
        }

        public int SaveAttachment(DbManager db, int id_tenant, int id_mail, MailAttachment attachment, bool need_recount)
        {
            var id = db.ExecuteScalar<int>(
                        new SqlInsert(AttachmentTable.name)
                            .InColumnValue(AttachmentTable.Columns.id, 0)
                            .InColumnValue(AttachmentTable.Columns.id_mail, id_mail)
                            .InColumnValue(AttachmentTable.Columns.name, attachment.fileName)
                            .InColumnValue(AttachmentTable.Columns.stored_name, attachment.storedName)
                            .InColumnValue(AttachmentTable.Columns.type, attachment.contentType)
                            .InColumnValue(AttachmentTable.Columns.size, attachment.size)
                            .InColumnValue(AttachmentTable.Columns.file_number, attachment.fileNumber)
                            .InColumnValue(AttachmentTable.Columns.need_remove, false)
                            .InColumnValue(AttachmentTable.Columns.content_id, attachment.contentId)
                            .InColumnValue(AttachmentTable.Columns.id_tenant, id_tenant)
                            .Identity(0, 0, true));

            if (need_recount)
                ReCountAttachments(db, id_mail);

            return id;
        }

        #endregion

        #region private methods
        public void StoreAttachments(int tenant, string user, List<MailAttachment> attachments, string stream_id)
        {
            StoreAttachments(tenant, user, attachments, stream_id, -1);
        }

        public void StoreAttachments(int tenant, string user, List<MailAttachment> attachments, string stream_id, int id_mailbox)
        {
            if (!attachments.Any() || string.IsNullOrEmpty(stream_id)) return;

            var storage = GetDataStore(tenant);

            if(storage == null)
                throw new NullReferenceException("GetDataStore has returned null reference.");

            var quota_add_size = attachments.Sum(a => a.data.LongLength);

            QuotaUsedAdd(tenant, quota_add_size);

            try
            {
                foreach (var attachment in attachments)
                {
                    bool is_attachment_name_has_bad_name = string.IsNullOrEmpty(attachment.fileName)
                                                         || attachment.fileName.IndexOfAny(Path.GetInvalidPathChars()) != -1
                                                         || attachment.fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1;
                    if (is_attachment_name_has_bad_name)
                    {
                        attachment.fileName = string.Format("attacment{0}{1}", attachment.fileNumber,
                                                                               MimeMapping.GetExtention(attachment.contentType));
                    }

                    attachment.streamId = stream_id;
                    attachment.tenant = tenant;
                    attachment.user = user;
                    StoreAttachmentWithoutQuota(tenant, user, attachment, storage);

                    //This is dirty hack needed for mailbox updating on attachment processing. If we doesn't update mailbox checktime, it will be reset.
                    if (id_mailbox > 0)
                    {
                        LockMailbox(id_mailbox, DateTime.UtcNow.Ticks);
                    }
                }
            }
            catch
            {
                var stored_attachments_keys = attachments
                                            .Where(a => !string.IsNullOrEmpty(a.storedFileUrl))
                                            .Select(a => a.GerStoredFilePath())
                                            .ToList();

                //Todo: Rewrite this ugly code
                bool is_quota_cleaned = false;
                if (stored_attachments_keys.Any())
                {
                    var s3_storage = storage as S3Storage;
                    if (s3_storage != null)
                    {
                        // ToDo: Delete with quota argument needs to be moved to IDataStore!
                        stored_attachments_keys
                            .ForEach(key => s3_storage.Delete(string.Empty, key, false));
                    }
                    else
                    {
                        is_quota_cleaned = true;
                        stored_attachments_keys.ForEach(key => storage.Delete(string.Empty, key));
                    }
                }

                if(!is_quota_cleaned)
                    QuotaUsedDelete(tenant, quota_add_size);

                _log.Debug(String.Format("Problems with attachment saving in mailboxId={0}. All message attachments was deleted.", id_mailbox));
                throw;
            }
        }

        public class AttachmentStream : IDisposable
        {
            public Stream FileStream { get; set; }
            public string FileName { get; set; }

            public void Dispose()
            {
                FileStream.Dispose();
            }
        }

        public AttachmentStream GetAttachmentStream(int id_attachment, int tenant)
        {
            var attachment = GetMessageAttachment(id_attachment, tenant);
            return GetAttachmentStream(attachment);
        }

        public AttachmentStream GetAttachmentStream(int id_attachment, int tenant, string id_user)
        {
            var attachment = GetMessageAttachment(id_attachment, tenant, id_user);
            return GetAttachmentStream(attachment);
        }

        public AttachmentStream GetAttachmentStream(MailAttachment attachment)
        {
            if (attachment != null)
            {
                var storage = GetDataStore(attachment.tenant);
                var attachment_path = attachment.GerStoredFilePath();
                var result = new AttachmentStream
                {
                    FileStream = storage.GetReadStream("", attachment_path),
                    FileName = attachment.fileName
                };
                return result;
            }
            throw new InvalidOperationException("Attachment not found");
        }
        #endregion

        #region private methods

        
        private static string ReplaceEmbeddedImages(MailMessageItem m)
        {
            try
            {
                var doc = new HtmlDocument();

                doc.LoadHtml(m.HtmlBody);

                if (m.Attachments != null && m.Attachments.Count > 0)
                {
                    foreach (var attach in m.Attachments)
                    {
                        if (string.IsNullOrEmpty(attach.contentId)) continue;
                        var old_nodes =
                            doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'cid:" +
                                                         attach.contentId.Trim('<').Trim('>') + "'))]");

                        if (old_nodes == null || attach.storedFileUrl == null) continue;
                        foreach (var node in old_nodes)
                        {
                            node.SetAttributeValue("cid", attach.contentId);
                            node.SetAttributeValue("src", attach.storedFileUrl);
                        }
                    }
                }

                return doc.DocumentNode.OuterHtml;
            }
            catch (RecursionDepthException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception)
            {
                return m.HtmlBody;
            }
        }

        private static int GetCountAttachments(IDbManager db, int id_mail)
        {
            var count_attachments = db.ExecuteScalar<int>(
                        new SqlQuery(AttachmentTable.name)
                            .SelectCount(AttachmentTable.Columns.id)
                            .Where(AttachmentTable.Columns.id_mail, id_mail)
                            .Where(AttachmentTable.Columns.need_remove, false)
                            .Where(Exp.Eq(AttachmentTable.Columns.content_id, Exp.Empty)));
            return count_attachments;
        }

        private void ReCountAttachments(DbManager db, int id_mail)
        {
            var count_attachments = GetCountAttachments(db, id_mail);

            db.ExecuteNonQuery(
                  new SqlUpdate(MailTable.name)
                  .Set(MailTable.Columns.attach_count, count_attachments)
                  .Where(MailTable.Columns.id, id_mail));
        }

        // Returns size of all tenant attachments. This size used in the quota calculation.
        private static long MarkAttachmetsNeedRemove(IDbManager db, int tenant, Exp condition)
        {
            var query_for_attachment_size_calculation = new SqlQuery(AttachmentTable.name)
                                                                .SelectSum(AttachmentTable.Columns.size)
                                                                .Where(AttachmentTable.Columns.id_tenant, tenant)
                                                                .Where(AttachmentTable.Columns.need_remove, false)
                                                                .Where(condition);

            var size = db.ExecuteScalar<long>(query_for_attachment_size_calculation);

            db.ExecuteNonQuery(
                new SqlUpdate(AttachmentTable.name)
                    .Set(AttachmentTable.Columns.need_remove, true)
                    .Where(AttachmentTable.Columns.id_tenant, tenant)
                    .Where(condition));

            return size;
        }

        private static void QuotaUsedAdd(int tenant, long used_quota)
        {
            var quota_controller = new TennantQuotaController(tenant);
            quota_controller.QuotaUsedAdd(MODULE_NAME, string.Empty, DATA_TAG, used_quota);
        }

        private static void QuotaUsedDelete(int tenant, long used_quota)
        {
            var quota_controller = new TennantQuotaController(tenant);
            quota_controller.QuotaUsedDelete(MODULE_NAME, string.Empty, DATA_TAG, used_quota);
        }
        #endregion
                }
            }
