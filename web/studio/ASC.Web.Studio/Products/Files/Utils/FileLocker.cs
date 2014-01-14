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
using System.Web.Configuration;
using ASC.Core;

namespace ASC.Web.Files.Utils
{
    public class FileLocker
    {
        internal class LockInfo
        {
            public bool CheckRight;
            public DateTime DateTimeLock;
            public Guid UserId;

            public LockInfo()
            {
                DateTimeLock = DateTime.UtcNow;
                UserId = SecurityContext.CurrentAccount.ID;
            }
        }

        private static readonly Dictionary<string, FileLocker> NowEditing = new Dictionary<string, FileLocker>();
        public static readonly TimeSpan EditTimeout;

        public static bool SingleEditing
        {
            get { return !string.IsNullOrEmpty(WebConfigurationManager.AppSettings["files.docservice.singleediting"]); }
        }

        private readonly Dictionary<Guid, LockInfo> _editingBy;
        private bool _lockVersion;

        static FileLocker()
        {
            int timer;
            Int32.TryParse(WebConfigurationManager.AppSettings["files.docservice.edit-timeout"], out timer);
            EditTimeout = TimeSpan.FromMilliseconds(timer > 0 ? timer : 12000);
        }

        private FileLocker(Guid tabId)
        {
            _lockVersion = false;
            _editingBy = new Dictionary<Guid, LockInfo> { { tabId, new LockInfo() } };
        }

        public static Guid Add(object fileId, bool lockVersion)
        {
            bool checkRight;
            var tabId = Guid.NewGuid();
            ProlongLock(fileId, tabId, lockVersion, out checkRight);
            return tabId;
        }

        public static void ProlongLock(object fileId, Guid tabId, bool lockVersion, out bool checkRight)
        {
            checkRight = false;
            lock (NowEditing)
            {
                if (IsLocked(fileId))
                {
                    if (NowEditing[fileId.ToString()]._editingBy.Keys.ToList().Contains(tabId))
                    {
                        NowEditing[fileId.ToString()]._editingBy[tabId].DateTimeLock = DateTime.UtcNow;
                        checkRight = NowEditing[fileId.ToString()]._editingBy[tabId].CheckRight;
                    }
                    else
                    {
                        NowEditing[fileId.ToString()]._editingBy.Add(tabId, new LockInfo());
                    }
                }
                else
                {
                    NowEditing[fileId.ToString()] = new FileLocker(tabId);
                }

                if (lockVersion)
                    NowEditing[fileId.ToString()]._lockVersion = true;
            }
        }

        public static void Remove(object fileId, Guid tabId)
        {
            lock (NowEditing)
            {
                if (NowEditing.ContainsKey(fileId.ToString()) && NowEditing[fileId.ToString()]._editingBy.Count > 1)
                {
                    NowEditing[fileId.ToString()]._editingBy.Remove(tabId);
                }
                else
                {
                    NowEditing.Remove(fileId.ToString());
                }
            }
        }

        public static bool IsLocked(object fileId)
        {
            lock (NowEditing)
            {
                if (fileId != null && NowEditing.ContainsKey(fileId.ToString()))
                {
                    var listForRemove = NowEditing[fileId.ToString()]
                        ._editingBy
                        .ToList()
                        .Where(lockBy => (DateTime.UtcNow - lockBy.Value.DateTimeLock).Duration() > EditTimeout);
                    foreach (var lockBy in listForRemove)
                    {
                        NowEditing[fileId.ToString()]._editingBy.Remove(lockBy.Key);
                    }

                    if (NowEditing[fileId.ToString()]._editingBy.Count == 0)
                    {
                        NowEditing.Remove(fileId.ToString());
                        return false;
                    }

                    return true;
                }
            }
            return false;
        }

        public static bool LockVersion(object fileId)
        {
            lock (NowEditing)
            {
                if (fileId != null && NowEditing.ContainsKey(fileId.ToString()))
                {
                    return NowEditing[fileId.ToString()]._lockVersion;
                }
            }
            return false;
        }

        public static void ChangeRight(object fileId, Guid userId, bool check)
        {
            lock (NowEditing)
            {
                if (fileId == null || !NowEditing.ContainsKey(fileId.ToString())) return;

                NowEditing[fileId.ToString()]._editingBy.Values.ToList()
                                             .ForEach(lockInfo =>
                                                          {
                                                              if (lockInfo.UserId == userId)
                                                              {
                                                                  lockInfo.CheckRight = check;
                                                              }
                                                          });
            }
        }

        public static List<Guid> GetLockedBy(object fileId)
        {
            lock (NowEditing)
            {
                return IsLocked(fileId)
                           ? NowEditing[fileId.ToString()]._editingBy.Values.ToList()
                                                          .Select(lockInfo => lockInfo.UserId).Distinct().ToList()
                           : new List<Guid>();
            }
        }
    }
}