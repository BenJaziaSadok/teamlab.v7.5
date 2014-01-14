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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ASC.Web.Studio.Services.Backup
{
    [ServiceContract]
    public interface IBackupService
    {
        [OperationContract]
        [WebGet(UriTemplate = "add", ResponseFormat = WebMessageFormat.Json)]
        BackupRequest RequestBackup();

        [OperationContract]
        [WebGet(UriTemplate = "list/{id}", ResponseFormat = WebMessageFormat.Json)]
        BackupRequest GetBackupStatus(string id);
    }


    [CollectionDataContract(Name = "{0}List", Namespace = "", ItemName = "entry")]
    public class ItemList<T> : List<T>
    {
        public ItemList()
            : base()
        {
        }

        public ItemList(IEnumerable<T> items)
            : base(items)
        {
        }
    }

    [CollectionDataContract(Name = "{1}Hash", Namespace = "", ItemName = "entry", KeyName = "key", ValueName = "value")]
    public class ItemDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public ItemDictionary()
            : base()
        {
        }

        public ItemDictionary(IDictionary<TKey,TValue> items):base(items)
        {
        }
    }
}
