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
using System.Configuration;
using ASC.Data.Storage.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace ASC.Data.Storage.S3
{
    public class S3UploadGuard
    {
        private string _accessKey;
        private string _secretAccessKey;
        private string _bucket;
        private bool _configErrors;
        private bool _configured;

        public void DeleteExpiredUploads(TimeSpan trustInterval)
        {
            Configure();

            if (_configErrors)
            {
                return;
            }

            using (var s3 = GetClient())
            {
                var nextKeyMarker = string.Empty;
                var nextUploadIdMarker = string.Empty;
                bool isTruncated;

                do
                {
                    var request = new ListMultipartUploadsRequest { BucketName = _bucket };

                    if (!string.IsNullOrEmpty(nextKeyMarker))
                    {
                        request.KeyMarker = nextKeyMarker;
                    }

                    if (!string.IsNullOrEmpty(nextUploadIdMarker))
                    {
                        request.UploadIdMarker = nextUploadIdMarker;
                    }

                    var response = s3.ListMultipartUploads(request);

                    foreach (var u in response.MultipartUploads.Where(x => x.Initiated + trustInterval <= DateTime.UtcNow))
                    {
                        AbortMultipartUpload(u, s3);
                    }

                    isTruncated = response.IsTruncated;
                    nextKeyMarker = response.NextKeyMarker;
                    nextUploadIdMarker = response.NextUploadIdMarker;
                }
                while (isTruncated);
                
            }
        }

        private void AbortMultipartUpload(MultipartUpload u, AmazonS3Client client)
        {
            var request = new AbortMultipartUploadRequest
            {
                BucketName = _bucket,
                Key = u.Key,
                UploadId = u.UploadId,
            };

            client.AbortMultipartUpload(request);
        }

        private AmazonS3Client GetClient()
        {
            var s3Config = new AmazonS3Config {CommunicationProtocol = Protocol.HTTP, MaxErrorRetry = 3};
            return new AmazonS3Client(_accessKey, _secretAccessKey, s3Config);
        }

        private void Configure()
        {
            if (!_configured)
            {
                var config = (StorageConfigurationSection)ConfigurationManager.GetSection(Schema.SECTION_NAME);
                var handler = config.Handlers.GetHandler("s3");
                if (handler != null)
                {
                    var props = handler.GetProperties();
                    _bucket = props["bucket"];
                    _accessKey = props["acesskey"];
                    _secretAccessKey = props["secretaccesskey"];
                }
                _configErrors = string.IsNullOrEmpty(ConfigurationManager.AppSettings["core.base-domain"]) //localhost
                                || string.IsNullOrEmpty(_accessKey)
                                || string.IsNullOrEmpty(_secretAccessKey)
                                || string.IsNullOrEmpty(_bucket);

                _configured = true;
            }
        }
    }
}
