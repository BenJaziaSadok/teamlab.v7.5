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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;

namespace TMResourceData
{
    public class DBResourceManager : ResourceManager
    {
        static object lockObject = new object();
        static DateTime _updateDate = DateTime.UtcNow;
        static Hashtable _resData;
        static Hashtable _resDataForTrans;

        // settings
        readonly static int updateSeconds;
        readonly static string getPagePortal;
        readonly static List<string> updatePortals;


        readonly string _fileName;
        readonly ResourceManager _resManager;

        protected Hashtable ResourceSetsTable;

        // not beforfieldInit
        static DBResourceManager()
        {
            updateSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["resources.cache-timeout"] ?? "10");
            updatePortals = (ConfigurationManager.AppSettings["resources.trans-portals"] ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            getPagePortal = ConfigurationManager.AppSettings["resources.pageinfo-portal"];
        }

        public DBResourceManager(string fileName, ResourceManager resManager)
        {
            ResourceSetsTable = new Hashtable();
            _fileName = fileName;
            _resManager = resManager;
        }

        public override ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            var baseCulture = culture;

            DBResourceSet databaseResourceSet;

            while (true)
            {
                if (ResourceSetsTable.Contains(culture.Name) && ResourceSetsTable[culture.Name] != null)
                {
                    databaseResourceSet = (DBResourceSet)ResourceSetsTable[culture.Name];
                }
                else
                {
                    databaseResourceSet = new DBResourceSet(_fileName, culture);
                    ResourceSetsTable.Add(culture.Name, databaseResourceSet);
                }

                if (databaseResourceSet.TableCount != 0)
                    break;

                if (culture.Equals(CultureInfo.InvariantCulture))
                    return _resManager.GetResourceSet(baseCulture, createIfNotExists, tryParents);

                culture = culture.Parent;
            }

            if (0 < updateSeconds && DateTime.UtcNow > _updateDate.AddSeconds(2))
            {
                GetResource.UpdateDBRS(databaseResourceSet, _fileName, culture.Name, _updateDate);
                _updateDate = DateTime.UtcNow;
            }

            return databaseResourceSet;

        }

        public override string GetString(string name, CultureInfo culture)
        {
            try
            {
                var pageLink = string.Empty;
                var resDataTable = LoadData();

                try
                {
                    if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    {
                        var uri = HttpContext.Current.Request.Url;

                        if (uri.Host.Contains("-translator") || uri.Host.Contains("we-translate") || updatePortals.Contains(uri.Host, StringComparer.InvariantCultureIgnoreCase))
                        {
                            resDataTable = LoadDataTrans();
                            if (0 < updateSeconds && DateTime.UtcNow > _updateDate.AddSeconds(updateSeconds))
                            {
                                GetResource.UpdateHashTable(ref resDataTable, _updateDate);
                                _updateDate = DateTime.UtcNow;
                            }
                        }

                        if (uri.Host == getPagePortal)
                        {
                            pageLink = uri.AbsolutePath;
                        }
                    }
                }
                catch (Exception err)
                {
                    log4net.LogManager.GetLogger("ASC.DbRes").Error(err);
                }

                var ci = culture ?? CultureInfo.CurrentUICulture;
                while (true)
                {
                    var language = !string.IsNullOrEmpty(ci.Name) ? ci.Name : "Neutral";

                    var resdata = resDataTable[name + _fileName + language];
                    if (resdata != null)
                    {
                        if (!string.IsNullOrEmpty(pageLink))
                        {
                            GetResource.AddLink(name, _fileName, pageLink);
                        }
                        return resdata.ToString();
                    }

                    if (ci.Equals(CultureInfo.InvariantCulture))
                    {
                        break;
                    }
                    ci = ci.Parent;
                }
            }
            catch (Exception err)
            {
                log4net.LogManager.GetLogger("ASC.DbRes").Error(err);
            }

            return _resManager.GetString(name, culture);
        }


        private Hashtable LoadData()
        {
            if (_resData == null)
            {
                lock (lockObject)
                {
                    if (_resData == null)
                    {
                        _resData = GetResource.GetAllData("tmresource");
                    }
                }
            }
            return _resData;
        }

        public ResourceSet GetBaseNeutralResourceSet()
        {
            return _resManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
        }

        private Hashtable LoadDataTrans()
        {
            if (_resDataForTrans == null)
            {
                lock (lockObject)
                {
                    if (_resDataForTrans == null)
                    {
                        _resDataForTrans = GetResource.GetAllData("tmresourceTrans");
                    }
                }
            }
            return _resDataForTrans;
        }
    }
}
