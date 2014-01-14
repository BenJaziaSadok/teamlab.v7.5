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
using System.Linq;
using System.Text;
using ASC.Common.Data.Sql.Expressions;
using ASC.FullTextIndex;
using ASC.Common.Data.Sql;
using ASC.Mail.Aggregator.DbSchema;
using ASC.Mail.Aggregator.Filter;

namespace ASC.Mail.Aggregator.Extension
{
    public static class SqlQueryExtensions
    {
        public static SqlQuery ApplyFilter(this SqlQuery query, MailFilter filter)
        {
            return ApplyFilter(query, filter, false);
        }

        public static SqlUpdate ApplyFilter(this SqlUpdate query, MailFilter filter)
        {
            return ApplyFilter(query, filter, false);
        }

        public static SqlDelete ApplyFilter(this SqlDelete query, MailFilter filter)
        {
            return ApplyFilter(query, filter, false);
        }

        private static SqlQuery ApplyFilter(this SqlQuery query, MailFilter filter, bool skip_folder)
        {
            var conditions = GetMailFilterConditions(filter, skip_folder, string.Empty);

            if (conditions != null)
                query.Where(conditions);

            return query;
        }

        private static SqlUpdate ApplyFilter(this SqlUpdate query, MailFilter filter, bool skip_folder)
        {
            var conditions = GetMailFilterConditions(filter, skip_folder, string.Empty);

            if (conditions != null)
                query.Where(conditions);

            return query;
        }

        private static SqlDelete ApplyFilter(this SqlDelete query, MailFilter filter, bool skip_folder)
        {
            var conditions = GetMailFilterConditions(filter, skip_folder, string.Empty);

            if (conditions != null)
                query.Where(conditions);

            return query;
        }

        public static Exp GetMailFilterConditions(MailFilter filter, bool skip_folder, string alias)
        {
            Exp conditions = null;

            if (!string.IsNullOrEmpty(alias))
                alias += ".";

            if (!skip_folder)
                conditions = Exp.Eq(alias + MailTable.Columns.folder, filter.PrimaryFolder);

            if (filter.CustomLabels != null && filter.CustomLabels.Count > 0)
            {
                var ids_with_any_of_tags = new SqlQuery(MailBoxManager.MAIL_TAG_MAIL)
                    .Select(MailBoxManager.TagMailFields.id_mail)
                    .Where(Exp.In(MailBoxManager.TagMailFields.id_tag, filter.CustomLabels));

                var ids_with_all_tags = new SqlQuery()
                    .Select(MailBoxManager.TagMailFields.id_mail)
                    .From(ids_with_any_of_tags, "a")
                    .GroupBy(MailBoxManager.TagMailFields.id_mail)
                    .Having(
                        Exp.Sql("count(a." + MailBoxManager.TagMailFields.id_mail + ")=" + filter.CustomLabels.Count()));

                conditions &= Exp.In(alias + MailTable.Columns.id, ids_with_all_tags);
            }

            if (filter.Unread.HasValue)
            {
                conditions &= Exp.Eq(alias + MailTable.Columns.unread, filter.Unread);
            }

            if (filter.Attachments)
                conditions &= Exp.Gt(alias + MailTable.Columns.attach_count, 0);

            if (filter.Period_from > 0)
            {
                var from = new DateTime(1970, 1, 1) + new TimeSpan(filter.Period_from * 10000);
                var to = new DateTime(1970, 1, 1) + new TimeSpan(filter.Period_to * 10000) +
                         new TimeSpan(1, 0, 0, 0, 0); // 1 day was added to make the "To" date limit inclusive
                conditions &= Exp.Between(alias + MailTable.Columns.date_sent, from, to);
            }

            if (filter.Important)
            {
                conditions &= Exp.Eq(alias + MailTable.Columns.importance, true);
            }

            if (!string.IsNullOrEmpty(filter.FindAddress))
            {
                if (filter.PrimaryFolder == MailFolder.Ids.sent || filter.PrimaryFolder == MailFolder.Ids.drafts)
                    conditions &= Exp.Like(alias + MailTable.Columns.to, filter.FindAddress, SqlLike.AnyWhere);
                else
                    conditions &= Exp.Like(alias + MailTable.Columns.from, filter.FindAddress, SqlLike.AnyWhere);
            }

            if (filter.MailboxId.HasValue)
            {
                conditions &= Exp.Eq(alias + MailTable.Columns.id_mailbox, filter.MailboxId.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchFilter))
            {
                if (FullTextSearch.SupportModule(FullTextSearch.MailModule))
                {
                    var ids = FullTextSearch.Search(filter.SearchFilter, FullTextSearch.MailModule)
                                            .GetIdentifiers()
                                            .Select(id => int.Parse(id));

                    conditions &= Exp.In(alias + MailTable.Columns.id, ids.Take(MailBoxManager.FULLTEXTSEARCH_IDS_COUNT).ToList());
                }
                else
                {
                    conditions &= Exp.Or(Exp.Like(alias + MailTable.Columns.from, filter.SearchFilter, SqlLike.AnyWhere),
                                       Exp.Or(
                                           Exp.Like(alias + MailTable.Columns.to, filter.SearchFilter, SqlLike.AnyWhere),
                                           Exp.Or(
                                               Exp.Like(alias + MailTable.Columns.cc, filter.SearchFilter,
                                                        SqlLike.AnyWhere),
                                               Exp.Or(
                                                   Exp.Like(alias + MailTable.Columns.bcc, filter.SearchFilter,
                                                            SqlLike.AnyWhere),
                                                   Exp.Like(alias + MailTable.Columns.subject, filter.SearchFilter,
                                                            SqlLike.AnyWhere)))));
                }
            }

            return conditions;
        }


        public static SqlQuery ApplySorting(this SqlQuery query, MailFilter filter)
        {
            var sort_field = MailTable.Columns.date_sent;

            switch (filter.Sort)
            {
                case "subject":
                    sort_field = MailTable.Columns.subject;
                    break;
                case "sender":
                    sort_field = MailTable.Columns.@from;
                    break;
            }

            var sort_order = filter.SortOrder == "ascending";

            query.OrderBy(sort_field, sort_order);

            return query;
        }

        public static string GetSqlWithParameters(this ISqlInstruction instruction)
        {
            var sql = instruction.ToString();
            var parameters = instruction.GetParameters();
            var sb = new StringBuilder();
            var i = 0;
            foreach (var part in sql.Split('?'))
            {
                sb.Append(part);
                if (i < parameters.Length)
                {
                    var p = parameters[i];
                    if (p == null)
                    {
                        sb.Append("null");
                    }
                    else if (p is string || p is char || p is DateTime || p is Guid)
                    {
                        sb.AppendFormat("'{0}'", p);
                    }
                    else
                    {
                        sb.Append(p);
                    }
                }
                i++;
            }
            return sb.ToString();
        }
    }

}
