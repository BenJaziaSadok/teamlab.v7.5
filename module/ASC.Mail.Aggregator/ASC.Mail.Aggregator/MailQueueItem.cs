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

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using ActiveUp.Net.Common;
using ActiveUp.Net.Mail;
using System.IO;
using NLog;
using ASC.Mail.Aggregator.Exceptions;
using ASC.Core.Tenants;
using ASC.Mail.Aggregator.Extension;

#endregion

namespace ASC.Mail.Aggregator
{
    public delegate void GetStoredMessagesUidlMd5Delegate(MailQueueItem account, Dictionary<int, string> uidl_list, Dictionary<int, string> md5_list);

    public delegate DateTime GetFolderModifyDateDelegate(MailQueueItem account, int folder_id);

    public delegate void RetrieveNewMessageDelegate(MailQueueItem account, Message message, int folder_id, string uidl, string md5_hash, bool unread, int[] tags_ids);

    public delegate void DoneDelegate(MailQueueItem item, bool quota_error);

    public delegate void AuthSucceedDelegate(MailBox account);

    public delegate void AuthFailedDelegate(MailBox account, string response_line);

    public delegate void UpdateUidlDelegate(MailBox account, int message_id, string new_uidl);

    public delegate int[] GetTagsIdsDelegate(string[] tags, int tenant, string user);

    public delegate void SetTagsForMessages(int tenant, string user, int[] tag_ids, int[] message_ids);

    public delegate void UpdateAccountTimeCheckedDelegate(int id_mailbox, Int64 utc_ticks_time);

    public partial class MailQueueItem : IDisposable
    {
        private readonly NLog.Logger _log;
        private const int CHECKED_TIME_INTERVAL = 3 * 60 * 1000; // min * sec_in_mins * miliseconds_in_sec
        private DateTime _lastTimeItemChecked;

        public const string CONNECTION_TIMEOUT_ERROR = "connection timeout exceeded";

        public MailQueueItem(MailBox account)
        {
            if (account == null) throw new ArgumentNullException("account");

            Account = account;
            Priority = 0;
            _lastTimeItemChecked = DateTime.UtcNow;
            _log = LogManager.GetLogger("MailQueueItem");
        }

        private bool IsUidlSupported { get; set; }

        public MailBox Account { get; private set; }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private DateTime LastRetrieve { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private int Priority { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        public event GetStoredMessagesUidlMd5Delegate GetStoredMessagesUidlMd5;

        private void InvokeGetStoredMessagesUIDL_MD5(Dictionary<int, string> uidl_list, Dictionary<int, string> md5_list)
        {
            var handler = GetStoredMessagesUidlMd5;
            if (handler != null)
            {
                handler(this, uidl_list, md5_list);
            }
        }

        public event RetrieveNewMessageDelegate OnRetrieveNewMessage;

        private void InvokeOnRetrieve(Message message, int folder_id, string uidl, string md5_hash)
        {
            InvokeOnRetrieve(message, folder_id, uidl, md5_hash, true, null);
        }

        private void InvokeOnRetrieve(Message message, int folder_id, string uidl, string md5_hash, bool unread, int[] tags_ids)
        {
            if (OnRetrieveNewMessage != null)
                OnRetrieveNewMessage(this, message, folder_id, uidl, md5_hash, unread, tags_ids);
        }

        public event UpdateAccountTimeCheckedDelegate OnTimeCheckedUpdate;

        private void UpdateTimeCheckedIfNeeded()
        {
            if ((DateTime.UtcNow - _lastTimeItemChecked).Milliseconds > CHECKED_TIME_INTERVAL)
            {
                if (OnTimeCheckedUpdate != null)
                {
                    _lastTimeItemChecked = DateTime.UtcNow;
                    OnTimeCheckedUpdate(Account.MailBoxId, _lastTimeItemChecked.Ticks);
                    _log.Debug(String.Format("Checked time was updated for mailbox: {0}", Account.MailBoxId));
                }
            }
        }

        public event DoneDelegate OnDone;

        private void InvokeOnDone(bool quota_error)
        {
            if (OnDone != null)
                OnDone(this, quota_error);
        }

        public event AuthSucceedDelegate OnAuthSucceed;
        public event AuthFailedDelegate OnAuthFailed;

        public event GetTagsIdsDelegate OnGetOrCreateTags;

        private int[] InvokeOnGetOrCreateTags(string[] tags_names)
        {
            return null != OnGetOrCreateTags && null != tags_names && tags_names.Any() ? OnGetOrCreateTags(tags_names, Account.TenantId, Account.UserId) : null;
        }

        public event SetTagsForMessages OnUpdateMessagesTags;
        
        private void InvokeOnUpdateMessagesTags(int tenant, string user, int[] tag_ids, int[] message_ids)
        {
            if (OnUpdateMessagesTags != null)
                OnUpdateMessagesTags(tenant, user, tag_ids, message_ids);
        }

        public void DownloadMessage(string uidl, string destination_path)
        {
            var pop = MailClientBuilder.Pop();
            try
            {
                _log.Debug("Connecting to {0}", Account.EMail);

                switch (Account.IncomingEncryptionType)
                {
                    case EncryptionType.StartTLS:
                        pop.ConnectTLS(Account.Server, Account.Port);
                        break;
                    case EncryptionType.SSL:
                        pop.ConnectSsl(Account.Server, Account.Port);
                        break;
                    case EncryptionType.None:
                        pop.Connect(Account.Server, Account.Port);
                        break;
                }

                if (Account.AuthenticationTypeIn == SaslMechanism.Login)
                {
                    pop.Login(Account.Account, Account.Password, Account.Server);
                }
                else
                {
                    pop.Authenticate(Account.Account, Account.Password, Account.AuthenticationTypeIn);
                }


                _log.Debug("GetCAPA()");

                GetCAPA(pop);

                _log.Info("Account: MessagesCount={0}, TotalSize={1}, UIDL={2}, LoginDelay={3}",
                    pop.MessageCount, pop.TotalSize, IsUidlSupported, Account.ServerLoginDelay);

                if (pop.UniqueIdExists(uidl))
                {
                    _log.Info("Message with this uidl exists!");

                    var index = pop.GetMessageIndex(uidl);

                    _log.Info("StoreMessage(index: {0})", index);

                    pop.StoreMessage(index, false, destination_path);

                    if (File.Exists(destination_path))
                        _log.Info("Message stored successfully!\r\n");
                    else
                        _log.Error("Message is missing in destination path!\r\n");
                }
                else
                    _log.Info("Message with this uidl not exists!\r\n");

            }
            catch (Pop3Exception e)
            {
                if (e.Command.StartsWith("USER") || e.Command.StartsWith("PASS"))
                {
                    if (OnAuthFailed != null) OnAuthFailed(Account, e.Response);

                    _log.Warn("Retrieve() Pop3: {0} Port: {1} Account: '{2}' ErrorMessage:\r\n{3}\r\n",
                        Account.Server, Account.Port, Account.Account, e.Message);
                }
                else
                {
                    _log.Error("Retrieve() Pop3: {0} Port: {1} Account: '{2}' ErrorMessage:\r\n{3}\r\n",
                        Account.Server, Account.Port, Account.Account, e.ToString());
                }

                throw;
            }
            finally
            {
                try
                {
                    if (pop.IsConnected)
                    {
                        pop.Disconnect();
                    }
                }
                catch { }
            }
        }


        // ReSharper disable UnusedMethodReturnValue.Global
        public bool Retrieve(int max_messages_per_session, WaitHandle stop_event)
        // ReSharper restore UnusedMethodReturnValue.Global
        {
            long log_record_id = AggregatorLogger.Instance.MailBoxProccessingStarts(Account.MailBoxId, Thread.CurrentThread.ManagedThreadId);
            bool result = false;
            int processed_messages_count;
            try
            {
                result = Account.Imap ? RetrieveImap(max_messages_per_session, stop_event, out processed_messages_count) :
                                         RetrievePop(max_messages_per_session, stop_event, out processed_messages_count);
            }
            catch (Exception)
            {
                //Its records proccessing_end_time without proccessing_messages_count. That action needed for correct metrics work.
                AggregatorLogger.Instance.MailBoxProccessingEnds(log_record_id, null);
                throw;
            }

            AggregatorLogger.Instance.MailBoxProccessingEnds(log_record_id, processed_messages_count);

            return result;
        }


        private bool RetrievePop(int max_messages_per_session, WaitHandle stop_event, out int processed_messages_count)
        {
            var pop = MailClientBuilder.Pop();
            try
            {
                pop.Authenticated += OnAuthenticated;

                _log.Debug("Connecting to {0}", Account.EMail);

                switch (Account.IncomingEncryptionType)
                {
                    case EncryptionType.StartTLS:
                        pop.ConnectTLS(Account.Server, Account.Port);
                        break;
                    case EncryptionType.SSL:
                        pop.ConnectSsl(Account.Server, Account.Port);
                        break;
                    case EncryptionType.None:
                        pop.Connect(Account.Server, Account.Port);
                        break;
                }

                if (Account.AuthenticationTypeIn == SaslMechanism.Login)
                {
                    pop.Login(Account.Account, Account.Password, Account.Server);
                }
                else
                {
                    pop.Authenticate(Account.Account, Account.Password, Account.AuthenticationTypeIn);
                }
                UpdateTimeCheckedIfNeeded();

                _log.Debug("UpdateStats()");

                pop.UpdateStats();

                _log.Debug("GetCAPA()");
                
                GetCAPA(pop);

                _log.Info("Account: MessagesCount={0}, TotalSize={1}, UIDL={2}, LoginDelay={3}",
                    pop.MessageCount, pop.TotalSize, IsUidlSupported, Account.ServerLoginDelay);

                if (ProcessMessagesPop(pop, max_messages_per_session, stop_event, out processed_messages_count))
                { // If all messages are proccessed
                    Account.MessagesCount = pop.MessageCount;
                    Account.Size = pop.TotalSize;
                    _log.Info("Account '{0}' has been processed.", Account.EMail);
                }

                LastRetrieve = DateTime.UtcNow;

                return true;
            }
            catch (Pop3Exception e)
            {
                if (e.Command.StartsWith("USER") || e.Command.StartsWith("PASS"))
                {
                    if (OnAuthFailed != null) OnAuthFailed(Account, e.Response);

                    _log.Warn("Retrieve() Pop3: {0} Port: {1} Account: '{2}' ErrorMessage:\r\n{3}\r\n",
                        Account.Server, Account.Port, Account.Account, e.Message);
                }
                else
                {
                    _log.Error("Retrieve() Pop3: {0} Port: {1} Account: '{2}' ErrorMessage:\r\n{3}\r\n",
                        Account.Server, Account.Port, Account.Account, e.ToString());
                }

                throw;
            }
            finally
            {
                try
                {
                    if (pop.IsConnected)
                    {
                        pop.Disconnect();
                    }
                }
                catch { }
            }
        }

        void OnAuthenticated(object sender, AuthenticatedEventArgs e)
        {
            if (OnAuthSucceed != null) OnAuthSucceed(Account);
        }

        private void GetCAPA(Pop3Client client)
        {
            try
            {
                var capa_params = client.GetServerCapabilities();

                if (capa_params.Length <= 0) return;
                var index = Array.IndexOf(capa_params, "LOGIN-DELAY");
                if (index > -1)
                {
                    int delay;
                    if (int.TryParse(capa_params[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out delay))
                        Account.ServerLoginDelay = delay;
                }

                IsUidlSupported = Array.IndexOf(capa_params, "UIDL") > -1;
            }
            catch
            { // CAPA NOT SUPPORTED 
                try
                { // CHECK UIDL SUPPORT
                    // ReSharper disable UnusedVariable
                    var uidls = client.GetUniqueIds();
                    // ReSharper restore UnusedVariable
                    IsUidlSupported = true;
                }
                catch { // UIDL NOT SUPPORTED 
                    IsUidlSupported = false;
                }
            }
        }

        // Returns: True if all messages are proccessed. False if at least one new message is not processed.
        private bool ProcessMessagesPop(Pop3Client client, int max_messages_per_session, WaitHandle stop_event, out int processed_messages_count)
        {
            UpdateTimeCheckedIfNeeded();
            processed_messages_count = max_messages_per_session;
            var bad_messages_exist = false;
            Dictionary<int, string> new_messages;
            var stored_uidl_list = new Dictionary<int, string>();
            var stored_md5_list = new Dictionary<int, string>();

            InvokeGetStoredMessagesUIDL_MD5(stored_uidl_list, stored_md5_list);

            if (!IsUidlSupported)
            {
                _log.Info("UIDL is not supported! Account '{0}' has been skiped.", Account.EMail);
                return true;
            }

            var email_ids = client.GetUniqueIds();

            new_messages =
                email_ids
                .Where(id => !stored_uidl_list.Values.Contains(id.UniqueId))
                .OrderBy(id => id.Index)
                .ToDictionary(id => id.Index, id => id.UniqueId );


            var quota_error_flag = false;

            if (client.IsConnected)
            {
                if (new_messages.Count == 0)
                    _log.Debug("New messages not found.\r\n");
                else
                {
                    _log.Debug("Found {0} new messages.\r\n", new_messages.Count);

                    if (new_messages.Count > 1)
                    {
                        _log.Debug("Calculating order");

                        try
                        {
                            var first_header = client.RetrieveHeaderObject(new_messages.First().Key);
                            var last_header = client.RetrieveHeaderObject(new_messages.Last().Key);

                            if (first_header.Date < last_header.Date)
                            {
                                _log.Debug("Account '{0}' order is DESC", Account.EMail.Address);
                                new_messages = new_messages
                                    .OrderByDescending(item => item.Key) // This is to ensure that the newest message would be handled primarily.
                                    .ToDictionary(id => id.Key, id => id.Value);
                            }
                            else
                                _log.Debug("Account '{0}' order is ASC", Account.EMail.Address);
                        }
                        catch (Exception)
                        {
                            _log.Warn("Calculating order skipped! Account '{0}' order is ASC", Account.EMail.Address);
                        }
                    }

                    var skip_on_date = Account.BeginDate != MailBoxManager.MIN_BEGIN_DATE;

                    var skip_break_on_date = MailQueueItemSettings.PopUnorderedDomains.Contains(Account.Server.ToLowerInvariant());

                    foreach (var new_message in new_messages)
                    {
                        try
                        {
                            if (stop_event.WaitOne(0))
                            {
                                break;
                            }

                            if (max_messages_per_session == 0)
                            {
                                _log.Debug("Limit of max messages per session is exceeded!");
                                break;
                            }

                            _log.Debug("Processing new message\tid={0}\t{1}\t",
                                new_message.Key,
                                (IsUidlSupported ? "UIDL: " : "MD5: ") + new_message.Value);

                            if (!client.IsConnected)
                            {
                                _log.Warn("POP3 server is disconnected. Skip another messages.");
                                bad_messages_exist = true;
                                break;
                            }

                            var message = client.RetrieveMessageObject(new_message.Key);
                            UpdateTimeCheckedIfNeeded();

                            if (message.Date < Account.BeginDate && skip_on_date)
                            {
                                if (!skip_break_on_date)
                                {
                                    _log.Info("Skip other messages older then {0}.", Account.BeginDate);
                                    break;
                                }
                                _log.Debug("Skip message (Date = {0}) on BeginDate = {1}", message.Date, Account.BeginDate);
                                continue;
                            }

                            var header_md5 = string.Empty;

                            if (IsUidlSupported)
                            {
                                var unique_identifier = string.Format("{0}|{1}|{2}|{3}",
                                                                      message.From.Email,
                                                                      message.Subject,
                                                                      message.DateString,
                                                                      message.MessageId);

                                header_md5 = unique_identifier.GetMD5();

                                if (!message.To.Exists(email =>
                                                       email.Email
                                                            .ToLowerInvariant()
                                                            .Equals(message.From.Email
                                                                           .ToLowerInvariant())))
                                {

                                    var found_message_id = stored_md5_list
                                        .Where(el => el.Value == header_md5)
                                        .Select(el => el.Key)
                                        .FirstOrDefault();

                                    if (found_message_id > 0)
                                    {
                                        InvokeOnUpdateUidl(found_message_id, new_message.Value);
                                        continue; // Skip saving founded message
                                    }
                                }
                            }

                            InvokeOnRetrieve(message,
                                MailFolder.Ids.inbox, 
                                IsUidlSupported ? new_message.Value : "", 
                                IsUidlSupported ? header_md5 : new_message.Value);
                        }
                        catch (IOException io_ex)
                        {
                            if (io_ex.Message.StartsWith("Unable to write data to the transport connection") ||
                                io_ex.Message.StartsWith("Unable to read data from the transport connection"))
                            {
                                _log.Error("ProcessMessages() Account='{0}': {1}",
                                     Account.EMail.Address, io_ex.ToString());

                                max_messages_per_session = 0; //It needed for stop messsages proccessing.
                                bad_messages_exist = true;
                                break;
                            }
                        }
                        catch (MailBoxOutException ex)
                        {
                            _log.Info("ProcessMessages() Tenant={0} User='{1}' Account='{2}': {3}",
                                Account.TenantId, Account.UserId, Account.EMail.Address, ex.Message);
                            bad_messages_exist = true;
                            break;
                        }
                        catch (TenantQuotaException qex)
                        {
                            _log.Info("Tenant {0} quota exception: {1}", Account.TenantId, qex.Message);
                            quota_error_flag = true;
                        }
                        catch (Exception e)
                        {
                            bad_messages_exist = true;
                            _log.Error("ProcessMessages() Tenant={0} User='{1}' Account='{2}', MailboxId={3}, MessageIndex={4}, UIDL='{5}' Exception:\r\n{6}\r\n",
                                Account.TenantId, Account.UserId, Account.EMail.Address, Account.MailBoxId, new_message.Key, new_message.Value, e.ToString());
                        }

                        UpdateTimeCheckedIfNeeded();
                        max_messages_per_session--;
                    }
                }
            }
            else
            {
                _log.Debug("POP3 server is disconnected.");
                bad_messages_exist = true;
            }

            InvokeOnDone(quota_error_flag);

            processed_messages_count -= max_messages_per_session;
            return !bad_messages_exist && max_messages_per_session > 0;
        }

        public bool Test()
        {
            BaseProtocolClient ingoing_client = (Account.Imap) ? (BaseProtocolClient)MailClientBuilder.Imap() : MailClientBuilder.Pop();

            MailServerHelper.Test(ingoing_client, new MailServerSettings
            {
                Url = Account.Server,
                Port = Account.Port,
                AccountName = Account.Account,
                AccountPass = Account.Password,
                AuthenticationType = Account.AuthenticationTypeIn,
                EncryptionType = Account.IncomingEncryptionType
            });

            MailServerHelper.TestSmtp(new MailServerSettings
            {
                Url = Account.SmtpServer,
                Port = Account.SmtpPort,
                AccountName = Account.SmtpAccount,
                AccountPass = Account.SmtpPassword,
                AuthenticationType = Account.AuthenticationTypeSmtp,
                EncryptionType = Account.OutcomingEncryptionType
            });

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof(MailQueueItem) && Equals((MailQueueItem)obj);
        }

        private bool Equals(MailQueueItem other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return ReferenceEquals(this, other) || Equals(other.Account, Account);
        }

        public override int GetHashCode()
        {
            return (Account != null ? Account.GetHashCode() : 0);
        }

        public void Dispose()
        {

        }
    }

    public class Pop3ConnectionException : Exception {
        public Pop3ConnectionException(string message) : base(message) {}
    }
    public class ImapConnectionException : Exception {
        public ImapConnectionException(string message) : base(message) {}
    }
    public class SmtpConnectionException : Exception {
        public SmtpConnectionException(string message) : base(message) { }
    }
}