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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    [DataContract(Namespace = "taskTemplateContainer")]
    public class TaskTemplateContainerWrapper : ObjectWrapperBase
    {

        public TaskTemplateContainerWrapper() : 
            base(0)
        {
            
        }
       
        public TaskTemplateContainerWrapper(int id)
            : base(id)
        {
            
        }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String Title { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public String EntityType { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<TaskTemplateWrapper> Items { get; set; }

        public static TaskTemplateContainerWrapper GetSample()
        {
            return new TaskTemplateContainerWrapper
            {
                EntityType = "contact",
                Title = "Birthday greetings",
                Items = new List<TaskTemplateWrapper>()
                            {
                                TaskTemplateWrapper.GetSample()
                            }
            };
        }

    }

    [DataContract(Namespace = "taskTemplate")]
    public class TaskTemplateWrapper : ObjectWrapperBase
    {

        public TaskTemplateWrapper():base(0)
        {
            
        }

        public TaskTemplateWrapper(int id) : 
            base(id)
        {


        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int ContainerID { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TaskCategoryWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool isNotify { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public long OffsetTicks { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool DeadLineIsFixed { get; set; }

        public static TaskTemplateWrapper GetSample()
        {
            return new TaskTemplateWrapper
                       {
                           Title = "Send an Email",
                           Category = TaskCategoryWrapper.GetSample(),
                           isNotify= true,
                           Responsible = EmployeeWraper.GetSample(),
                           ContainerID = 12,
                           DeadLineIsFixed = false,
                           OffsetTicks = TimeSpan.FromDays(10).Ticks,
                           Description = ""
                       };
        }

    }
}
