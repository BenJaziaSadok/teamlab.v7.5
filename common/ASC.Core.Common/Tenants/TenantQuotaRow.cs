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

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantQuotaRow
    {
        public int Tenant { get; set; }

        public string Path { get; set; }

        public long Counter { get; set; }

        public string Tag { get; set; }
    }
}