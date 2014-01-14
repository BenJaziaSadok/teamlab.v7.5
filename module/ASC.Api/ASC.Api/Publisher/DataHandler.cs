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

namespace ASC.Api.Publisher
{
    public delegate void DataAvailibleDelegate(object data, object userData);

    public class DataHandler : IDisposable
    {
        private readonly DataAvailibleDelegate _dataAvailible;
        public event DataAvailibleDelegate DataAvailible = null;
        private bool _isDisposed;
        public object UserData { get; set; }

        public DataHandler(object userData, DataAvailibleDelegate dataAvailible)
        {
            _dataAvailible = dataAvailible;
            UserData = userData;
            DataAvailible += dataAvailible;
        }

        ~DataHandler()
        {
            Dispose(false);
        }

        public void OnDataAvailible(object data)
        {
            var handler = DataAvailible;
            if (handler != null) handler(data, UserData);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                DataAvailible -= _dataAvailible;
                _isDisposed = true;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DataHandler)) return false;
            return Equals((DataHandler) obj);
        }

        public bool Equals(DataHandler other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._dataAvailible, _dataAvailible) && Equals(other.UserData, UserData);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dataAvailible != null ? _dataAvailible.GetHashCode() : 0)*397) ^ (UserData != null ? UserData.GetHashCode() : 0);
            }
        }
    }
}