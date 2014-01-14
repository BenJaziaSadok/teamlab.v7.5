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
using System.IO;

namespace ASC.Api.Calendar.iCalParser
{
    internal class iCalendarCacheParams
    {
        public string FolderCachePath { get; private set; }
        public int ExpiredPeriod { get; private set; }

        public iCalendarCacheParams(string folderCachPath, int expiredPeriod)
        {
            FolderCachePath = folderCachPath.TrimEnd('\\') + "\\";
            ExpiredPeriod = expiredPeriod;
        }

        public static iCalendarCacheParams Default
        {
            get {
                var path = AppDomain.CurrentDomain.BaseDirectory + "addons\\calendar\\ical_cache";
                return new iCalendarCacheParams(path, 2);
            }
        }
    }

    internal class iCalendarCache
    {
        private iCalendarCacheParams _cacheParams;

        public iCalendarCache() : this(iCalendarCacheParams.Default){}
        public iCalendarCache(iCalendarCacheParams cacheParams)
        {
            _cacheParams = cacheParams;
        }

        public bool UpdateCalendarCache(string calendarId, TextReader textReader)
        {
            var curDate = DateTime.UtcNow;

            string fileName = calendarId+".ics";
            ClearCache(calendarId);

            var buffer = new char[1024*1024];
            try
            {
                if (!Directory.Exists(_cacheParams.FolderCachePath))
                    Directory.CreateDirectory(_cacheParams.FolderCachePath);

                using (var sw = File.CreateText(_cacheParams.FolderCachePath + fileName))
                {
                    while (true)
                    {
                        var count = textReader.Read(buffer, 0, buffer.Length);
                        if (count <= 0)
                            break;

                        sw.Write(buffer, 0, count);
                    }
                }
            }
            catch
            {
                return false;
            }            

            return true;
        }

        public iCalendar GetCalendarFromCache(string calendarId)
        {
            var filePath = _cacheParams.FolderCachePath + calendarId+".ics";
            if (File.Exists(filePath))            
            {                
                var fi = new FileInfo(filePath);
                if ((DateTime.UtcNow - fi.LastWriteTimeUtc).TotalMinutes > _cacheParams.ExpiredPeriod)
                    return null;

                using (var tr = new StreamReader(File.OpenRead(filePath)))
                {
                    return iCalendar.GetFromStream(tr);
                }
            }

            return null;
        }

        public void ClearCache(string calendarId)
        {
            var filePath = _cacheParams.FolderCachePath + calendarId + ".ics";
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch { }
        }

        public void ClearCache()
        {
            foreach (var file in Directory.GetFiles(_cacheParams.FolderCachePath))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }
    }
}
