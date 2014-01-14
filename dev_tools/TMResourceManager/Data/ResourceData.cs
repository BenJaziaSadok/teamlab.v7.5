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
using System.Configuration;
using System.IO;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

using TMResourceData.Model;

namespace TMResourceData
{
    public static class ResourceData
    {
        static ResourceData()
        {
            if (!DbRegistry.IsDatabaseRegistered("tmresource"))
            {
                DbRegistry.RegisterDatabase("tmresource", ConfigurationManager.ConnectionStrings["tmresource"]);
            }
        }

        public static void AddCulture(string cultureTitle, string name)
        {
            using (var dbManager = new DbManager("tmresource"))
            {
                var sqlInsert = new SqlInsert("res_cultures");
                sqlInsert.InColumnValue("title", cultureTitle).InColumnValue("value", name);
                dbManager.ExecuteNonQuery(sqlInsert);
            }
        }

        public static void AddResource(string cultureTitle, string resType, DateTime date, ResWord word, bool isConsole, string authorLogin)
        {
            using (var db = new DbManager("tmresource"))
            {
                var resData = db.ExecuteScalar<string>(GetQuery("res_data", cultureTitle, word));
                var resReserve = db.ExecuteScalar<string>(GetQuery("res_reserve", cultureTitle, word)); 

                
                if (string.IsNullOrEmpty(resData))
                {
                    
                    db.ExecuteNonQuery(Insert("res_data", cultureTitle, word)
                                              .InColumnValue("resourceType", resType)
                                              .InColumnValue("timechanges", date)
                                              .InColumnValue("flag", 2)
                                              .InColumnValue("authorLogin", authorLogin));

                    
                    if (isConsole)  db.ExecuteNonQuery(Insert("res_reserve", cultureTitle, word));
                }
                else
                {
                    var isChangeResData = resData != word.ValueFrom;
                    var isChangeResReserve = resReserve != word.ValueFrom;

                    
                    if ((isConsole && isChangeResData && isChangeResReserve) || !isConsole)
                    {
                        
                        if (cultureTitle == "Neutral")
                        {
                            var update = new SqlUpdate("res_data")
                                            .Set("flag", 3)
                                            .Where("fileID", word.ResFile.FileID)
                                            .Where("title", word.Title);

                            db.ExecuteNonQuery(update);
                        }
                        
                        db.ExecuteNonQuery(Insert("res_data", cultureTitle, word)
                                              .InColumnValue("resourceType", resType)
                                              .InColumnValue("timechanges", date)
                                              .InColumnValue("flag", 2)
                                              .InColumnValue("authorLogin", authorLogin));

                        if (isConsole) db.ExecuteNonQuery(Update("res_reserve", cultureTitle, word));
                    }
                    else if (isConsole && isChangeResData && !isChangeResReserve)
                    {
                        db.ExecuteNonQuery(Update("res_reserve", cultureTitle, word));
                    }
                }
            }
        }

        private static SqlQuery GetQuery(string table,string cultureTitle, ResWord word)
        {
            return new SqlQuery(table)
                      .Select("textvalue")
                      .Where("fileID", word.ResFile.FileID)
                      .Where("cultureTitle", cultureTitle)
                      .Where("title", word.Title);

        }

        private static SqlUpdate Update(string table, string cultureTitle, ResWord word)
        {
            return new SqlUpdate(table)
                       .Set("flag", 2)
                       .Set("textvalue", word.ValueFrom)
                       .Where("fileID", word.ResFile.FileID)
                       .Where("title", word.Title)
                       .Where("cultureTitle", cultureTitle);

        }

        private static SqlInsert Insert(string table, string cultureTitle, ResWord word)
        {
            return new SqlInsert(table, true)
                    .InColumns(new[] { "title", "textvalue", "cultureTitle", "fileID" })
                    .Values(new object[] { word.Title, word.ValueFrom, cultureTitle, word.ResFile.FileID });

        }

        public static void EditEnglish(ResWord word)
        {
            using (var dbManager = new DbManager("tmresource"))
            {
                var update = new SqlUpdate("res_data");

                update.Set("textvalue", word.ValueFrom).Where("fileID", word.ResFile.FileID).Where("title", word.Title).Where("cultureTitle", "Neutral");

                dbManager.ExecuteNonQuery(update);

            }
        }

        public static void AddComment(ResWord word)
        {
            using (var dbManager = new DbManager("tmresource"))
            {
                var sqlUpdate = new SqlUpdate("res_data");
                sqlUpdate.Set("description", word.TextComment).Where("title", word.Title).Where("fileID", word.ResFile.FileID).Where("cultureTitle", "Neutral");
                dbManager.ExecuteNonQuery(sqlUpdate);
            }
        }

        public static int AddFile(string fileName, string projectName, string moduleName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            if (fileNameWithoutExtension != null && fileNameWithoutExtension.Split('.').Length > 1)
            {
                fileName = fileNameWithoutExtension.Split('.')[0] + Path.GetExtension(fileName);
            }

            using (var dbManager = new DbManager("tmresource"))
            {
                var sql = new SqlQuery("res_files")
                    .SelectCount()
                    .Where("resName", fileName)
                    .Where("projectName", projectName)
                    .Where("moduleName", moduleName);

                if (dbManager.ExecuteScalar<int>(sql) == 0)
                {
                    var insert = new SqlInsert("res_files")
                        .InColumns(new[] { "resName", "projectName", "moduleName" })
                        .Values(new[] { fileName, projectName, moduleName });

                    dbManager.ExecuteNonQuery(insert);
                }

                var update = new SqlUpdate("res_files")
                    .Set("lastUpdate", DateTime.UtcNow.AddHours(4));
                dbManager.ExecuteNonQuery(update);

                sql = new SqlQuery("res_files")
                    .Select("id")
                    .Where("resName", fileName)
                    .Where("projectName", projectName)
                    .Where("moduleName", moduleName);
                return dbManager.ExecuteScalar<int>(sql);
            }
        }

        public static void UpdateDeletedData(int fileId, DateTime date)
        {
            using (var dbManager = new DbManager("tmresource"))
            {
                var sqlUpdate = new SqlUpdate("res_data");
                sqlUpdate.Set("flag", 4).Where("flag", 1).Where(Exp.Lt("TimeChanges", date)).Where("fileID", fileId);
                dbManager.ExecuteNonQuery(sqlUpdate);
            }
        }
    }
}