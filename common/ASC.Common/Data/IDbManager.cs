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
using ASC.Common.Data.Sql;

namespace ASC.Common.Data
{
    public interface IDbManager : IDisposable
    {
        IDbConnection Connection { get; }


        IDbTransaction BeginTransaction();
        
        List<object[]> ExecuteList(string sql, params object[] parameters);

        List<object[]> ExecuteList(ISqlInstruction sql);

        List<T> ExecuteList<T>(ISqlInstruction sql, Converter<IDataRecord, T> converter);

        T ExecuteScalar<T>(string sql, params object[] parameters);

        T ExecuteScalar<T>(ISqlInstruction sql);

        int ExecuteNonQuery(string sql, params object[] parameters);

        int ExecuteNonQuery(ISqlInstruction sql);

        int ExecuteBatch(IEnumerable<ISqlInstruction> batch);
    }
}