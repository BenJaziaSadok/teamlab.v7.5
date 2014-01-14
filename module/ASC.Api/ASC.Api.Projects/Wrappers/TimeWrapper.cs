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

#region Usings

using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "time", Namespace = "")]
    public class TimeWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 5)]
        public ApiDateTime Date { get; set; }

        [DataMember(Order = 6)]
        public float Hours { get; set; }

        [DataMember(Order = 6)]
        public string Note { get; set; }

        [DataMember(Order = 7)]
        public int RelatedProject { get; set; }

        [DataMember(Order = 7)]
        public int RelatedTask { get; set; }

        [DataMember(Order = 7)]
        public string RelatedTaskTitle { get; set; }

        [DataMember(Order = 51)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 52)]
        public EmployeeWraper Person { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember(Order = 53)]
        public PaymentStatus PaymentStatus { get; set; }

        [DataMember(Order = 54)]
        public ApiDateTime StatusChanged { get; set; }

        [DataMember(Order = 55)]
        public bool CanEditPaymentStatus { get; set; }

        private TimeWrapper()
        {
        }

        public TimeWrapper(TimeSpend timeSpend)
        {
            Date = (ApiDateTime)timeSpend.Date;
            Hours = timeSpend.Hours;
            Id = timeSpend.ID;
            Note = timeSpend.Note;
            CreatedBy = EmployeeWraper.Get(timeSpend.CreateBy);
            RelatedProject = timeSpend.Task.Project.ID;
            RelatedTask = timeSpend.Task.ID;
            RelatedTaskTitle = timeSpend.Task.Title;
            CanEdit = ProjectSecurity.CanEdit(timeSpend);
            PaymentStatus = timeSpend.PaymentStatus;
            StatusChanged = (ApiDateTime)timeSpend.StatusChangedOn;
            CanEditPaymentStatus = ProjectSecurity.CanEditPaymentStatus(timeSpend);
            

            if (timeSpend.CreateBy != timeSpend.Person)
            {
                Person = EmployeeWraper.Get(timeSpend.Person);
            }
        }


        public static TimeWrapper GetSample()
        {
            return new TimeWrapper
                       {
                           Id = 10,
                           Date = (ApiDateTime) DateTime.Now,
                           Hours = 3.5F,
                           Note = "Sample note",
                           RelatedProject = 123,
                           RelatedTask = 13456,
                           RelatedTaskTitle = "Sample task",
                           CreatedBy = EmployeeWraper.GetSample(),
                           Person = EmployeeWraper.GetSample(),
                           CanEdit = true,
                           PaymentStatus = PaymentStatus.Billed,
                           StatusChanged = (ApiDateTime)DateTime.Now,
                           CanEditPaymentStatus = true
                       };
        }
    }
}
