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
    [DataContract(Name = "HelpCenterItem", Namespace = "")]
    public class HelpCenterItem
    {
        [DataMember(Name = "Title")] public string Title;

        [DataMember(Name = "Content")] public string Content;
    }

    [Serializable]
    [DataContract(Name = "HelpCenterData", Namespace = "")]
    public class HelpCenterData
    {
        [DataMember(Name = "ListItems")] public List<HelpCenterItem> ListItems;

        [DataMember(Name = "ResetCacheKey")] public String ResetCacheKey;
    }

    public class HelpCenterStorage
    {
        private static readonly string Filepath = ClientSettings.StorePath + "/helpcenter/helpcenter.html";

        private static IDataStore GetStore()
        {
            return StorageFactory.GetStorage("-1", "common_static");
        }

        public static Dictionary<string, HelpCenterData> GetHelpCenter()
        {
            var data = FromCache();
            if (data != null) return data;

            if (!GetStore().IsFile(Filepath)) return null;

            using (var stream = GetStore().GetReadStream(Filepath))
            {
                data = (Dictionary<string, HelpCenterData>)FromStream(stream);
            }
            ToCache(data);
            return data;
        }

        public static void UpdateHelpCenter(Dictionary<string, HelpCenterData> data)
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
        private const string CacheKey = "helpcenter";

        private static void ToCache(Dictionary<string, HelpCenterData> obj)
        {
            Cache.Insert(CacheKey, obj, DateTime.UtcNow.Add(ExpirationTimeout));
        }

        private static Dictionary<string, HelpCenterData> FromCache()
        {
            return Cache.Get(CacheKey) as Dictionary<string, HelpCenterData>;
        }
    }
}