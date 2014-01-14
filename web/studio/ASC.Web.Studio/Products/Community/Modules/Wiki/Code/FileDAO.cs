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
using System.Linq;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Wiki.Data
{
    class FileDAO : BaseDao
    {
        public FileDAO(string dbid, int tenant)
            : base(dbid, tenant)
        {
        }


        public List<File> GetFiles(IEnumerable<string> names)
        {
            return GetFiles(names == null ? Exp.Empty : Exp.In("filename", names.ToArray()));
        }

        public List<File> FindFiles(string name)
        {
            if (string.IsNullOrEmpty(name)) return new List<File>();

            return GetFiles(Exp.Like("filename", name, SqlLike.StartWith));
        }

        public File SaveFile(File file)
        {
            if (file == null) throw new ArgumentNullException("file");

            var i = Insert("wiki_files")
                .InColumnValue("filename", file.FileName)
                .InColumnValue("uploadfilename", file.UploadFileName)
                .InColumnValue("version", file.Version)
                .InColumnValue("userid", file.UserID.ToString())
                .InColumnValue("date", TenantUtil.DateTimeToUtc(file.Date))
                .InColumnValue("filelocation", file.FileLocation)
                .InColumnValue("filesize", file.FileSize);

            db.ExecuteNonQuery(i);

            return file;
        }

        public void RemoveFile(string fileName)
        {
            var d1 = Delete("wiki_files").Where("filename", fileName);
            var d2 = Delete("wiki_comments").Where("pagename", fileName);

            db.ExecuteBatch(new[] { d1, d2 });
        }


        private List<File> GetFiles(Exp where)
        {
            var q = Query("wiki_files")
                .Select("filename", "uploadfilename", "version", "userid", "date", "filelocation", "filesize")
                .Where(where)
                .OrderBy("filename", true);

            return db
                .ExecuteList(q)
                .ConvertAll(r => ToFile(r))
                .GroupBy(f => f.FileName)
                .Select(g => g.OrderByDescending(f => f.Version).First())
                .ToList();
        }

        private File ToFile(object[] r)
        {
            return new File
            {
                FileName = (string)r[0],
                UploadFileName = (string)r[1],
                Version = Convert.ToInt32(r[2]),
                UserID = new Guid((string)r[3]),
                Date = TenantUtil.DateTimeFromUtc((DateTime)r[4]),
                FileLocation = (string)r[5],
                FileSize = Convert.ToInt32(r[6]),
                Tenant = this.tenant,
            };
        }
    }
}