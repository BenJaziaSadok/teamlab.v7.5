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

namespace ASC.Core.Billing
{
    [Serializable]
    public class PaymentOffice
    {
        public string Key1
        {
            get;
            set;
        }

        public string Key2
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public int UsersCount
        {
            get;
            set;
        }

        public bool Editing
        {
            get;
            set;
        }

        public bool CoEditing
        {
            get;
            set;
        }

        public bool Ad
        {
            get;
            set;
        }
    }
}
