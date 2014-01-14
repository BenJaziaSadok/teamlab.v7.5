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
using ASC.Core.Tenants;

namespace ASC.Core.Billing
{
    public class Tariff
    {
        public int QuotaId { get; set; }

        public TariffState State { get; set; }

        public DateTime DueDate { get; set; }

        public bool Autorenewal { get; set; }

        public bool Prolongable { get; set; }


        public static Tariff CreateDefault()
        {
            return new Tariff
                {
                    QuotaId = Tenant.DEFAULT_TENANT,
                    State = TariffState.Paid,
                    DueDate = DateTime.MaxValue,
                };
        }


        public override int GetHashCode()
        {
            return QuotaId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t = obj as Tariff;
            return t != null && t.QuotaId == QuotaId;
        }
    }
}