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

namespace ASC.Projects.Core.Domain
{
    public class TimeSpend : DomainObject<int>
    {
        public Task Task { get; set; }

        public DateTime Date { get; set; }

        public float Hours { get; set; }

        public Guid Person { get; set; }

        public string Note { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime StatusChangedOn { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid CreateBy { get; set; }
    }
}
