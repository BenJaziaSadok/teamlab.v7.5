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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;

namespace ASC.Files.Core.Data
{
    public class AbstractDao : IDisposable
    {
        private readonly string _dbId;


        protected int TenantID { get; private set; }

        protected readonly Cache _cache = HttpRuntime.Cache;


        protected AbstractDao(int tenantID, String storageKey)
        {
            TenantID = tenantID;
            _dbId = storageKey;
        }

        protected DbManager GetDbManager()
        {
            return new DbManager(_dbId);
        }

        public void Dispose()
        {
        }


        protected List<object[]> ExecList(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteList(sql);
            }
        }

        protected List<object[]> ExecList(string sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteList(sql);
            }
        }

        protected T ExecScalar<T>(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteScalar<T>(sql);
            }
        }

        protected int ExecNonQuery(ISqlInstruction sql)
        {
            using (var manager = GetDbManager())
            {
                return manager.ExecuteNonQuery(sql);
            }
        }


        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(GetTenantColumnName(table)).Values(TenantID);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }

        protected SqlQuery GetFileQuery(Exp where)
        {
            return Query("files_file f")
                .Select("f.id")
                .Select("f.title")
                .Select("f.folder_id")
                .Select("f.create_on")
                .Select("f.create_by")
                .Select("f.version")
                .Select("f.content_length")
                .Select("f.modified_on")
                .Select("f.modified_by")
                .Select(GetRootFolderType("folder_id"))
                .Select(GetSharedQuery(FileEntryType.File))
                .Select("converted_type")
                .Where(where);
        }

        protected File ToFile(object[] r)
        {
            var result = new File
                {
                    ID = Convert.ToInt32(r[0]),
                    Title = (String)r[1],
                    FolderID = Convert.ToInt32(r[2]),
                    CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[3])),
                    CreateBy = new Guid((string)r[4]),
                    Version = Convert.ToInt32(r[5]),
                    ContentLength = Convert.ToInt64(r[6]),
                    ModifiedOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[7])),
                    ModifiedBy = new Guid((string)r[8]),
                    RootFolderType = ParseRootFolderType(r[9]),
                    RootFolderCreator = ParseRootFolderCreator(r[9]),
                    RootFolderId = ParseRootFolderId(r[9]),
                    SharedByMe = Convert.ToBoolean(r[10]),
                    ConvertedType = (string)r[11],
                };

            return result;
        }

        protected SqlQuery GetRootFolderType(string parentFolderColumnName)
        {
            return Query("files_folder d")
                .From("files_folder_tree t")
                .Select("concat(cast(d.folder_type as char),d.create_by,cast(d.id as char))")
                .Where(Exp.EqColumns("d.id", "t.parent_id") &
                       Exp.EqColumns("t.folder_id", "f." + parentFolderColumnName))
                .OrderBy("level", false)
                .SetMaxResults(1);
        }

        protected FolderType ParseRootFolderType(object v)
        {
            return v != null
                       ? (FolderType)Enum.Parse(typeof(FolderType), v.ToString().Substring(0, 1))
                       : default(FolderType);
        }

        protected Guid ParseRootFolderCreator(object v)
        {
            return v != null ? new Guid(v.ToString().Substring(1, 36)) : default(Guid);
        }

        protected int ParseRootFolderId(object v)
        {
            return v != null ? int.Parse(v.ToString().Substring(1 + 36)) : default(int);
        }

        protected SqlQuery GetSharedQuery(FileEntryType type)
        {
            return Query("files_security s")
                .SelectCount()
                .Where(Exp.EqColumns("s.entry_id", "f.id"))
                .Where("s.entry_type", (int)type)
                .Where("owner", SecurityContext.CurrentAccount.ID.ToString());
        }

        protected SqlUpdate GetRecalculateFilesCountUpdate(object folderId)
        {
            if (DbRegistry.GetSqlDialect(_dbId).SupportMultiTableUpdate)
            {
                return new SqlUpdate("files_folder d, files_folder_tree t")
                    .Set(
                        "d.filesCount = (select count(distinct f.id) from files_file f, files_folder_tree t2 where f.tenant_id = d.tenant_id and f.folder_id = t2.folder_id and t2.parent_id = d.id)")
                    .Where(Exp.EqColumns("d.id", "t.parent_id") & Exp.Eq("t.folder_id", folderId) &
                           Exp.Eq("d.tenant_id", TenantID));
            }
            else
            {
                return new SqlUpdate("files_folder")
                    .Set(
                        "filesCount = (select count(distinct f.id) from files_file f, files_folder_tree t2 where f.tenant_id = files_folder.tenant_id and f.folder_id = t2.folder_id and t2.parent_id = files_folder.id)")
                    .Where(Exp.Eq("files_folder.tenant_id", TenantID) &
                           Exp.In("files_folder.id",
                                  new SqlQuery("files_folder_tree t").Select("t.parent_id").Where("t.folder_id",
                                                                                                  folderId)));
            }
        }

        protected object MappingID(object id, bool saveIfNotExist)
        {
            if (id == null) return null;

            int n;

            var isNumeric = int.TryParse(id.ToString(), out n);

            if (isNumeric) return n;

            object result;

            using (var DbManager = GetDbManager())
            {
                if (id.ToString().StartsWith("sbox"))
                    result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
                else
                    result = DbManager.ExecuteScalar<String>(Query("files_thirdparty_id_mapping")
                                                                 .Select("id")
                                                                 .Where(Exp.Eq("hash_id", id)));

                if (saveIfNotExist)
                    DbManager.ExecuteNonQuery(Insert("files_thirdparty_id_mapping")
                                                  .InColumnValue("id", id)
                                                  .InColumnValue("hash_id", result)
                        );
            }

            return result;
        }

        protected object MappingID(object id)
        {
            return MappingID(id, false);
        }
    }
}