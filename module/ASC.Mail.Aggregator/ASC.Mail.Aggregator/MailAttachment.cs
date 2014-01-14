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

using System.Runtime.Serialization;
using ASC.Data.Storage;
using ASC.Mail.Aggregator.Utils;
using AjaxPro;

namespace ASC.Mail.Aggregator
{
    [DataContract(Name = "attachment", Namespace = "")]
    public class MailAttachment
    {
        public MailAttachment()
        {
            data = new byte[0];
        }

        //DO NOT RENAME field's lower case, for AjaxPro.JavaScriptSerializer (Upload handler) and Api.Serializer (Mail.Api) equal result;
        // ReSharper disable InconsistentNaming
        [DataMember(Name = "fileId", EmitDefaultValue = false)]
        public int fileId { get; set; }

        [DataMember(Name = "fileName", EmitDefaultValue = false)]
        public string fileName { get; set; }
        
        [DataMember(Name = "size", EmitDefaultValue = false)]
        public long size { get; set; }
        
        [DataMember(Name = "contentType", EmitDefaultValue = false)]
        public string contentType { get; set; }
        
        [DataMember(Name = "contentId", EmitDefaultValue = false)]
        public string contentId { get; set; }
        
        [DataMember(Name = "fileNumber", EmitDefaultValue = false)]
        public int fileNumber { get; set; }
        
        [DataMember(Name = "storedName", EmitDefaultValue = false)]
        public string storedName { get; set; }
        
        [DataMember(Name = "streamId", EmitDefaultValue = false)]
        public string streamId { get; set; }

        [IgnoreDataMember]
        [AjaxNonSerializable]
        public string storedFileUrl { get; set; }
        
        [IgnoreDataMember]
        [AjaxNonSerializable]
        public byte[] data { get; set; }

        [IgnoreDataMember]
        [AjaxNonSerializable]
        public string user { get; set; }

        [IgnoreDataMember]
        [AjaxNonSerializable]
        public int tenant { get; set; }

        // ReSharper restore InconsistentNaming

        public string GerStoredFilePath()
        {
            return MailStoragePathCombiner.GetFileKey(user, streamId, fileNumber, storedName);
        }

        public string GerPreSignedUrl(IDataStore data_store = null)
        {
            return MailStoragePathCombiner.GetPreSignedUri(fileId, tenant, user, streamId, fileNumber, storedName, data_store);
        }
    }
}