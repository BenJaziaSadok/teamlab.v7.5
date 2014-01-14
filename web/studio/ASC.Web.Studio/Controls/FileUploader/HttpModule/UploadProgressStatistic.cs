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

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    public class UploadProgressStatistic
    {
        private static readonly Dictionary<string, UploadProgressStatistic> Statistics = new Dictionary<string, UploadProgressStatistic>();
        public const string UploadIdField = "__UixdId";

        public string UploadId { get; set; }
        public long TotalBytes { get; set; }
        public long UploadedBytes { get; set; }
        public bool IsFinished { get; set; }
        public string CurrentFile { get; set; }
        public int CurrentFileIndex { get; set; }
        public float Progress { get; set; }
        public int ReturnCode { get; set; }

        internal UploadProgressStatistic()
        {
            CurrentFile = string.Empty;
            CurrentFileIndex = -1;
        }

        public string ToJson()
        {
            return string.Format("{{\"TotalBytes\":{0},\"Progress\":{1},\"CurrentFileIndex\":{2},\"CurrentFile\":\"{3}\",\"UploadId\":{4},\"IsFinished\":{5},\"Swf\":false,\"ReturnCode\":{6}}}",
                                 TotalBytes, Progress.ToString().Replace(',', '.'), CurrentFileIndex, CurrentFile, UploadId ?? "null", (IsFinished ? "true" : "false"), ReturnCode);
        }

        internal void AddUploadedBytes(int bytesCount)
        {
            UploadedBytes += bytesCount;
            Progress = (float)UploadedBytes/TotalBytes;
            Progress = Progress > 1 ? 1 : Progress;
        }

        public static UploadProgressStatistic GetStatistic(string id)
        {
            UploadProgressStatistic us;
            if (!Statistics.TryGetValue(id, out us))
                us = new UploadProgressStatistic();
            return us;
        }

        internal void EndUpload()
        {
            IsFinished = true;
        }

        internal void BeginFileUpload(string fileName)
        {
            CurrentFile = fileName;
            CurrentFileIndex++;
        }

        internal void AddFormField(string name, string value)
        {
            if (name == UploadIdField)
            {
                UploadId = value;

                if (!Statistics.ContainsKey(value))
                    Statistics.Add(value, this);
            }
        }
    }
}