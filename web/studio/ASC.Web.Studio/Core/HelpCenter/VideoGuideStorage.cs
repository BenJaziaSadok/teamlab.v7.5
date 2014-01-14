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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ASC.Core.Caching;
using ASC.Data.Storage;
using ASC.Web.Core.Client;

namespace ASC.Web.Studio.Core.HelpCenter
{
    [Serializable]
    [DataContract(Name = "VideoGuideItem", Namespace = "")]
    public class VideoGuideItem
    {
        [DataMember(Name = "Title")] public string Title;

        [DataMember(Name = "Id")] public string Id;

        [DataMember(Name = "Link")] public string Link;

        [DataMember(Name = "Status")] public string Status;
    }

    [Serializable]
    [DataContract(Name = "VideoGuideStorageItem", Namespace = "")]
    public class VideoGuideData
    {
        [DataMember(Name = "ListItems")] public List<VideoGuideItem> ListItems;

        [DataMember(Name = "ResetCacheKey")] public String ResetCacheKey;
    }

    public class VideoGuideStorage
    {
        private static readonly string Filepath = ClientSettings.StorePath + "/helpcenter/videoguide.html";

        private static IDataStore GetStore()
        {
            return StorageFactory.GetStorage("-1", "common_static");
        }

        public static Dictionary<string, VideoGuideData> GetVideoGuide()
        {
            var data = FromCache();
            if (data != null) return data;

            if (!GetStore().IsFile(Filepath)) return null;

            using (var stream = GetStore().GetReadStream(Filepath))
            {
                data = (Dictionary<string, VideoGuideData>)FromStream(stream);
            }
            ToCache(data);
            return data;
        }

        public static void UpdateVideoGuide(Dictionary<string, VideoGuideData> data)
        {
            using (var stream = ToStream(data))
            {
                GetStore().Save(Filepath, stream);
            }
            ToCache(data);
        }

        private static MemoryStream ToStream(object objectType)
        {
            var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectType);
            return stream;
        }

        private static object FromStream(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        private static readonly ICache Cache = new AspCache();
        private static readonly TimeSpan ExpirationTimeout = TimeSpan.FromDays(1);
        private const string CacheKey = "videoguide";

        private static void ToCache(Dictionary<string, VideoGuideData> obj)
        {
            Cache.Insert(CacheKey, obj, DateTime.UtcNow.Add(ExpirationTimeout));
        }

        private static Dictionary<string, VideoGuideData> FromCache()
        {
            return Cache.Get(CacheKey) as Dictionary<string, VideoGuideData>;
        }
    }
}