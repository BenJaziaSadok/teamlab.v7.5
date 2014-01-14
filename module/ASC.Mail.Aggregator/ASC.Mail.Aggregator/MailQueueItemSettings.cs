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
using System.Text;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Mail.Aggregator
{
    public static class MailQueueItemSettings
    {
        static MailQueueItemSettings()
        {
            using (var db = new DbManager(MailBoxManager.ConnectionStringName))
            {
                var imap_flags = db.ExecuteList(new SqlQuery(DbSchema.ImapFlags.table)
                                                    .Select(DbSchema.ImapFlags.Columns.folder_id,
                                                            DbSchema.ImapFlags.Columns.name,
                                                            DbSchema.ImapFlags.Columns.skip))
                                   .ConvertAll(r => new
                                       {
                                           folder_id = Convert.ToInt32(r[0]),
                                           name = (string) r[1],
                                           skip = Convert.ToBoolean(r[2])
                                       });

                SkipImapFlags = imap_flags.FindAll(i => i.skip).ConvertAll(i => i.name).ToArray();

                ImapFlags = new Dictionary<string, int>();
                imap_flags.FindAll(i => !i.skip).ForEach(i => { ImapFlags[i.name] = i.folder_id; });

                SpecialDomainFolders = new Dictionary<string, Dictionary<string, MailboxInfo>>();
                db.ExecuteList(new SqlQuery(DbSchema.ImapSpecialMailbox.table)
                                   .Select(DbSchema.ImapSpecialMailbox.Columns.server,
                                           DbSchema.ImapSpecialMailbox.Columns.name,
                                           DbSchema.ImapSpecialMailbox.Columns.folder_id,
                                           DbSchema.ImapSpecialMailbox.Columns.skip))
                  .ForEach(r =>
                      {
                          var server = ((string) r[0]).ToLower();
                          var name = ((string)r[1]).ToLower();
                          var mb = new MailboxInfo
                              {
                                  folder_id = Convert.ToInt32(r[2]),
                                  skip = Convert.ToBoolean(r[3])
                              };
                          if (SpecialDomainFolders.Keys.Contains(server))
                              SpecialDomainFolders[server][name] = mb;
                          else
                              SpecialDomainFolders[server] = new Dictionary<string, MailboxInfo> { { name, mb } };
                      });

                PopUnorderedDomains = db.ExecuteList(new SqlQuery(DbSchema.PopUnorderedDomain.table)
                                                         .Select(DbSchema.PopUnorderedDomain.Columns.server))
                                        .ConvertAll(r => (string) r[0])
                                        .ToArray();
            }
        }

        public static Dictionary<string, int> ImapFlags { get; private set; }
        public static string[] SkipImapFlags { get; private set; }
        public static string[] PopUnorderedDomains { get; private set; }

        public struct MailboxInfo
        {
            public int folder_id;
            public bool skip;
        }
        public static Dictionary<string, Dictionary<string, MailboxInfo>> SpecialDomainFolders { get; private set; }
    }
}
