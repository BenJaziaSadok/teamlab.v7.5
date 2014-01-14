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
using System.Collections.Specialized;
using System.IO;
using ASC.Data.Storage.Configuration;

namespace ASC.Data.Storage
{
    public abstract class BaseStorage : IDataStore
    {
        #region IDataStore Members

        public IQuotaController QuotaController { get; set; }

        public Uri GetUri(string path)
        {
            return GetUri(string.Empty, path);
        }

        public Uri GetUri(string domain, string path)
        {
            return GetPreSignedUri(domain, path, TimeSpan.MaxValue, null);
        }

        public abstract Uri GetPreSignedUri(string domain, string path, TimeSpan expire, IEnumerable<string> headers);

        public abstract Stream GetReadStream(string domain, string path);
        public abstract Stream GetReadStream(string domain, string path, int offset);

        public abstract Uri Save(string domain, string path, Stream stream);
        public abstract Uri Save(string domain, string path, Stream stream, ACL acl);

        public Uri Save(string domain, string path, Stream stream, string attachmentFileName)
        {
            if (!string.IsNullOrEmpty(attachmentFileName))
            {
                return SaveWithAutoAttachment(domain, path, stream, attachmentFileName);
            }
            return Save(domain, path, stream);
        }

        protected abstract Uri SaveWithAutoAttachment(string domain, string path, Stream stream, string attachmentFileName);

        public abstract Uri Save(string domain, string path, Stream stream, string contentType,
                                 string contentDisposition);
        public abstract Uri Save(string domain, string path, Stream stream, string contentEncoding, int cacheDays);

        public abstract Uri UploadWithoutQuota(string domain, string path, Stream stream, string contentType,
                                 string contentDisposition);

        #region chunking

        public virtual string InitiateChunkedUpload(string domain, string path)
        {
            throw new NotImplementedException();
        }

        public virtual string UploadChunk(string domain, string path, string uploadId, Stream stream, int chunkNumber, long chunkLength)
        {
            throw new NotImplementedException();
        }

        public virtual Uri FinalizeChunkedUpload(string domain, string path, string uploadId, Dictionary<int, string> eTags)
        {
            throw new NotImplementedException();
        }

        public virtual void AbortChunkedUpload(string domain, string path, string uploadId)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsSupportChunking { get { return false; } }

        #endregion

        public abstract void Delete(string domain, string path);
        public abstract void DeleteFiles(string domain, string folderPath, string pattern, bool recursive);
        public abstract void MoveDirectory(string srcdomain, string srcdir, string newdomain, string newdir);
        public abstract Uri Move(string srcdomain, string srcpath, string newdomain, string newpath);
        public abstract Uri SaveTemp(string domain, out string assignedPath, Stream stream);
        public abstract Uri[] List(string domain, string path, bool recursive);
        public abstract Uri[] ListFiles(string domain, string path, string pattern, bool recursive);
        public abstract string[] ListFilesRelative(string domain, string path, string pattern, bool recursive);
        public abstract bool IsFile(string domain, string path);
        public abstract bool IsDirectory(string domain, string path);
        public abstract void DeleteDirectory(string domain, string path);
        public abstract long GetFileSize(string domain, string path);
        public abstract long ResetQuota(string domain);
        public abstract long GetUsedQuota(string domain);
        public abstract Uri Copy(string srcdomain, string path, string newdomain, string newpath);
        public abstract void CopyDirectory(string srcdomain, string dir, string newdomain, string newdir);
        public abstract Uri GetUriInternal(string path);

        public Stream GetReadStream(string path)
        {
            return GetReadStream(string.Empty, path);
        }

        public Uri Save(string path, Stream stream, string attachmentFileName)
        {
            return Save(string.Empty, path, stream, attachmentFileName);
        }

        public Uri Save(string path, Stream stream)
        {
            return Save(string.Empty, path, stream);
        }

        public void Delete(string path)
        {
            Delete(string.Empty, path);
        }

        public void DeleteFiles(string folderPath, string pattern, bool recursive)
        {
            DeleteFiles(string.Empty, folderPath, pattern, recursive);
        }

        public Uri Move(string srcpath, string newdomain, string newpath)
        {
            return Move(string.Empty, srcpath, newdomain, newpath);
        }

        public Uri SaveTemp(out string assignedPath, Stream stream)
        {
            return SaveTemp(string.Empty, out assignedPath, stream);
        }

        public Uri[] List(string path, bool recursive)
        {
            return List(string.Empty, path, recursive);
        }

        public Uri[] ListFiles(string path, string pattern, bool recursive)
        {
            return ListFiles(string.Empty, path, pattern, recursive);
        }

        public bool IsFile(string path)
        {
            return IsFile(string.Empty, path);
        }

        public bool IsDirectory(string path)
        {
            return IsDirectory(string.Empty, path);
        }

        public void DeleteDirectory(string path)
        {
            DeleteDirectory(string.Empty, path);
        }

        public long GetFileSize(string path)
        {
            return GetFileSize(string.Empty, path);
        }

        public Uri Copy(string path, string newdomain, string newpath)
        {
            return Copy(string.Empty, path, newdomain, newpath);
        }

        public void CopyDirectory(string dir, string newdomain, string newdir)
        {
            CopyDirectory(string.Empty, dir, newdomain, newdir);
        }

        public abstract IDataStore Configure(IDictionary<string, string> props);

        public IDataStore SetQuotaController(IQuotaController controller)
        {
            QuotaController = controller;
            return this;
        }

        public abstract string SavePrivate(string domain, string path, Stream stream, DateTime expires);
        public abstract void DeleteExpired(string domain, string path, TimeSpan oldThreshold);

        public abstract string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize,
                                             string contentType, string contentDisposition, string submitLabel);

        public abstract string GetUploadedUrl(string domain, string directoryPath);
        public abstract string GetUploadUrl();

        public abstract string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType,
                                             string contentDisposition);

        #endregion
    }
}