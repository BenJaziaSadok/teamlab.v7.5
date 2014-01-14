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
    public class PaymentInfo
    {
        public int ID { get; set; }

        public int Status { get; set; }

        public string PaymentType { get; set; }

        public double ExchangeRate { get; set; }

        public double GrossSum { get; set; }

        public string CartId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime Date { get; set; }

        public Decimal Price { get; set; }

        public string Currency { get; set; }

        public string Method { get; set; }

        public int QuotaId { get; set; }

        public string ProductId { get; set; }
    }
}
