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

#region Import

using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.FederatedLogin.Profile;
using System.Linq;
using System.Linq.Expressions;
using System.Configuration;

#endregion


namespace ASC.FederatedLogin
{

    public class MultiRegionAccountLinker
    {

        #region Members

        private readonly Dictionary<String, AccountLinker> _accountLinkers = new Dictionary<String, AccountLinker>();
        private readonly String _baseDatabaseId = null;

        #endregion

        private String GetDatabaseId(String hostedRegion)
        {
            var databaseId = _baseDatabaseId;

            if (!String.IsNullOrEmpty(hostedRegion))
                databaseId = String.Join(".", new[] { _baseDatabaseId, hostedRegion.Trim() });

            if (!_accountLinkers.ContainsKey(databaseId))
                throw new ArgumentException(String.Format("Region {0} is not defined", databaseId), "hostedRegion");

            return databaseId;
        }


        public MultiRegionAccountLinker(String databaseId)
        {
            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
            {
                if (connection.Name.StartsWith(databaseId))
                    _accountLinkers.Add(connection.Name, new AccountLinker(connection.Name));
            }
        }

        public IEnumerable<String> GetLinkedObjects(string id, string provider)
        {
            return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(id, provider));
        }

        public IEnumerable<String> GetLinkedObjects(LoginProfile profile)
        {
            return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(profile));
        }

        public IEnumerable<String> GetLinkedObjectsByHashId(string hashid)
        {
            return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjectsByHashId(hashid));
        }

        public void AddLink(String hostedRegion, string obj, LoginProfile profile)
        {
            _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, profile);
        }

        public void AddLink(String hostedRegion, string obj, string id, string provider)
        {
            _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, id, provider);
        }

        public void RemoveLink(String hostedRegion, string obj, string id, string provider)
        {
            _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, id, provider);
        }

        public void RemoveLink(String hostedRegion, string obj, LoginProfile profile)
        {
            _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, profile);
        }

        public void Unlink(String region, string obj)
        {
            _accountLinkers[GetDatabaseId(region)].Unlink(obj);
        }

        public void RemoveProvider(String hostedRegion, string obj, string provider)
        {
            _accountLinkers[GetDatabaseId(hostedRegion)].RemoveProvider(obj, provider);
        }

        public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
        {
            return _accountLinkers.Values.SelectMany(x => x.GetLinkedProfiles(obj));
        }

    }
}