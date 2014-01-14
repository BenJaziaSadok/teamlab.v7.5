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

using System.Collections.Generic;
using ASC.Api.Attributes;
using ASC.Mail.Aggregator;
using ASC.Specific;

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        ///    Returns the list of all folders
        /// </summary>
        /// <param name="last_check_time" optional="true"> Filter folders for last_check_time. Get folders with greater date time.</param>
        /// <returns>Folders list</returns>
        /// <short>Get folders</short> 
        /// <category>Folders</category>
        [Read(@"folders")]
        public IEnumerable<MailBoxManager.MailFolderInfo> GetFolders(ApiDateTime last_check_time)
        {
            if (null != last_check_time)
            {
                var api_date = new ApiDateTime(mailBoxManager.GetMessagesModifyDate(TenantId, Username));
                var compare_rez = api_date.CompareTo(last_check_time);

                if (compare_rez < 1 && System.DateTime.MinValue != api_date) // if api_date == DateTime.MinValue then there are no folders in mail_folder
                {
                    return null;
                }
            }
            return FoldersList;
        }

        /// <summary>
        ///    Returns change date of folderid.
        /// </summary>
        /// <param name="folderid">Selected folder id.</param>
        /// <returns>Last modify folder DateTime</returns>
        /// <short>Get folder change date</short> 
        /// <category>Folders</category>
        [Read(@"folders/{folderid:[0-9]+}/modify_date")]
        public ApiDateTime GetFolderModifyDate(int folderid)
        {
            return new ApiDateTime(mailBoxManager.GetFolderModifyDate(TenantId, Username, folderid));
        }

        /// <summary>
        ///    Removes all the messages from the folder. Trash or Spam.
        /// </summary>
        /// <param name="folderid">Selected folder id. Trash - 4, Spam 5.</param>
        /// <short>Remove all messages from folder</short> 
        /// <category>Folders</category>
        [Delete(@"folders/{folderid:[0-9]+}/messages")]
        public int RemoveFolderMessages(int folderid)
        {
            if (folderid == MailFolder.Ids.trash || folderid == MailFolder.Ids.spam)
            {
                mailBoxManager.DeleteFoldersMessages(TenantId, Username, folderid);
            }

            return folderid;
        }

        private IEnumerable<MailBoxManager.MailFolderInfo> FoldersList
        {
            get { return mailBoxManager.GetFoldersList(TenantId, Username, true); }
        }
    }
}
