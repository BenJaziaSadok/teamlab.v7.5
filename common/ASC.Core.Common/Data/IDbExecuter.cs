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
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public interface IDbExecuter
    {
        T ExecScalar<T>(ISqlInstruction sql);

        int ExecNonQuery(ISqlInstruction sql);

        List<object[]> ExecList(ISqlInstruction sql);

        void ExecBatch(IEnumerable<ISqlInstruction> batch);

        void ExecAction(Action<IDbExecuter> action);
    }
}
