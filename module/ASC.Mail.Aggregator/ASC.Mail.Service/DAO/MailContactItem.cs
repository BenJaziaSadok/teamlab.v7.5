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
using System.Runtime.Serialization;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.Collection;
using ASC.Mail.Service;

namespace ASC.Mail.Service.DAO
{
    [DataContract(Name = "Contact")]
    public class MailContactItem
    {
        internal static MailContactItem FromContactItemInfo(MailContactItem itemInfo)
        {
            if (itemInfo != null)
            {
                var contact = new MailContactItem
                {
                    Name = itemInfo.Name,
                    Description = itemInfo.Description,   
                    Labels = new ItemList<int>(itemInfo.Labels),
                    Emails = new ItemList<string>(itemInfo.Emails)
                };
                return contact;
            }
            return null;
        }

        [DataMember(IsRequired = true, Name = "Name")]
        public string Name { get; set; }

        [DataMember(IsRequired = false, Name = "Description")]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ItemList<int> Labels { get; set; }

        public string LabelsString
        {
            get { return MailUtil.GetStringFromLabels(Labels); }
            set
            {
                Labels = new ItemList<int>(MailUtil.GetLabelsFromString(value));
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public ItemList<string> Emails { get; set; }

        public string EmailsString
        {
            get { return MailUtil.GetStringFromEmails(Emails); }
            set
            {
                Emails = new ItemList<string>(MailUtil.GetEmailsFromString(value));
            }
        }
    }
}
