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
using System.IO;
using System.Linq;
using System.Text;
using ASC.Common.Data;
using ASC.Data.Backup.Exceptions;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Data;
using ASC.Data.Backup.Tasks.Modules;

namespace ASC.Data.Backup.Tasks
{
    internal class RestoreDbModuleTask : ProgressTask
    {
        private const int TransactionLength = 10000;

        private static readonly List<string> IgnoredTables = new List<string>(); 

        private readonly IDataReadOperator _reader;
        private readonly IModuleSpecifics _module;
        private readonly ColumnMapper _columnMapper;
        private readonly DbFactory _factory;

        public RestoreDbModuleTask(IModuleSpecifics module, IDataReadOperator reader, ColumnMapper columnMapper, DbFactory factory)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (columnMapper == null)
                throw new ArgumentNullException("columnMapper");

            if (factory == null)
                throw new ArgumentNullException("factory");

            _module = module;
            _reader = reader;
            _columnMapper = columnMapper;
            _factory = factory;
        }

        public void IgnoreTable(string tableName)
        {
            if (!IgnoredTables.Contains(tableName))
                IgnoredTables.Add(tableName);
        }

        public override void Run()
        {
            InvokeInfo("begin restore data for module {0}", _module.ModuleName);
            InitProgress(_module.Tables.Count(t => !IgnoredTables.Contains(t.Name)));
            using (var connection = _factory.OpenConnection(_module.ConnectionStringName))
            {
                foreach (var table in _module.GetTablesOrdered().Where(t => !IgnoredTables.Contains(t.Name) && t.InsertMethod != InsertMethod.None))
                {
                    InvokeInfo("begin restore table {0}", table.Name);

                    var transactionsCommited = 0;
                    var rowsInserted = 0;
                    ActionInvoker.Try(state => RestoreTable(connection.Fix(), (TableInfo)state, ref transactionsCommited, ref rowsInserted),
                                      table, 5, onFailure: error => { throw ThrowHelper.CantRestoreTable(table.Name, error); });

                    SetStepCompleted();
                    InvokeInfo("{0} rows inserted for table {1}", rowsInserted, table.Name);
                }
            }
            InvokeInfo("end restore data for module {0}", _module.ModuleName);
        }

        private void RestoreTable(IDbConnection connection, TableInfo tableInfo, ref int transactionsCommited, ref int rowsInserted)
        {
            using (var stream = _reader.GetEntry(KeyHelper.GetTableZipKey(_module, tableInfo.Name)))
            {
                var lowImportanceRelations = _module
                    .TableRelations
                    .Where(r => string.Equals(r.ParentTable, tableInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    .Where(r => r.Importance == RelationImportance.Low && !r.IsSelfRelation())
                    .Select(r => Tuple.Create(r, _module.Tables.Single(t => t.Name == r.ChildTable)))
                    .ToList();

                foreach (IEnumerable<DataRowInfo> rows in GetRows(tableInfo, stream).Skip(transactionsCommited * TransactionLength).MakeParts(TransactionLength))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        int rowsSuccess = 0;
                        foreach (DataRowInfo row in rows)
                        {
                            using (var insertCommand = _module.CreateInsertCommand(connection, _columnMapper, tableInfo, row))
                            {
                                if (insertCommand == null)
                                {
                                    WarnCantInsertRow(row);
                                    continue;
                                }
                                insertCommand.WithTimeout(120).ExecuteNonQuery();
                                rowsSuccess++;
                            }

                            if (tableInfo.HasIncrementColumn())
                            {
                                var lastIdCommand = _factory.CreateLastInsertIdCommand(_module.ConnectionStringName);
                                lastIdCommand.Connection = connection;

                                var newValue = Convert.ToInt32(lastIdCommand.ExecuteScalar());
                                var oldValue = Convert.ToInt32(row[tableInfo.AutoIncrementColumn]);

                                _columnMapper.SetMapping(tableInfo.Name, tableInfo.AutoIncrementColumn, oldValue, newValue);
                            }

                            foreach (Tuple<RelationInfo, TableInfo> relation in lowImportanceRelations)
                            {
                                if (!relation.Item2.HasTenantColumn())
                                {
                                    InvokeWarning("Table {0} does not contain tenant id column. Can't apply low importance relations on such tables.", relation.Item2.Name);
                                    continue;
                                }

                                object oldValue = row[relation.Item1.ParentColumn];
                                object newValue = _columnMapper.GetMapping(relation.Item1.ParentTable, relation.Item1.ParentColumn, oldValue);

                                connection.CreateCommand(string.Format("update {0} set {1} = {2} where {1} = {3} and {4} = {5}",
                                                                       relation.Item1.ChildTable,
                                                                       relation.Item1.ChildColumn,
                                                                       newValue is string ? "'" + newValue + "'" : newValue,
                                                                       oldValue is string ? "'" + oldValue + "'" : oldValue,
                                                                       relation.Item2.TenantColumn,
                                                                       _columnMapper.GetTenantMapping())).WithTimeout(120).ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        transactionsCommited++;
                        rowsInserted += rowsSuccess;
                    }
                }
            }
        }

        private IEnumerable<DataRowInfo> GetRows(TableInfo table, Stream xmlStream)
        {
            if (xmlStream == null)
                return Enumerable.Empty<DataRowInfo>();

            var rows = DataRowInfoReader.ReadFromStream(xmlStream);

            var selfRelation = _module.TableRelations.SingleOrDefault(x => x.ChildTable == table.Name && x.IsSelfRelation());
            if (selfRelation != null)
            {
                rows = rows
                    .ToTree(x => x[selfRelation.ParentColumn], x => x[selfRelation.ChildColumn])
                    .SelectMany(x => OrderNode(x));
            }

            return rows;
        }

        private static IEnumerable<DataRowInfo> OrderNode(TreeNode<DataRowInfo> node)
        {
            var result = new List<DataRowInfo> {node.Entry};
            result.AddRange(node.Children.SelectMany(x => OrderNode(x)));
            return result;
        }

        private void WarnCantInsertRow(DataRowInfo row)
        {
            var sb = new StringBuilder("Can't create command to insert row with values [\"", 100);

            sb.Append(row.Values[0]);
            for (int i = 1; i < Math.Min(row.Values.Length, 4); i++)
            {
                sb.AppendFormat(", {0}", row.Values[i]);
            }
            if (row.Values.Length > 5)
            {
                sb.Append(", ...");
            }

            sb.AppendFormat("\"] into table {0}", row.TableName);

            InvokeWarning(sb.ToString());
        }
    }
}
