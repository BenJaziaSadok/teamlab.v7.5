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
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;


#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {
        /// <summary>
        ///    Returns the list of descriptions for all existing user fields
        /// </summary>
        /// <short>Get user field list</short> 
        /// <category>User fields</category>
        ///<returns>
        ///    User field list
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Read("{entityType:(contact|person|company|opportunity|case)}/customfield/definitions")]
        public IEnumerable<CustomFieldWrapper> GetCustomFieldDefinitions(String entityType)
        {

            return DaoFactory.GetCustomFieldDao().GetFieldsDescription(ToEntityType(entityType)).ConvertAll(item => ToCustomFieldWrapper(item)).ToSmartList();

        }

        /// <summary>
        ///   Returns the list of all user field values using the entity type and entity ID specified in the request
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <param name="entityid">ID</param>
        /// <short>Get user field values</short> 
        /// <category>User fields</category>
        /// <returns></returns>
        [Read("{entityType:(contact|person|company|opportunity|case)}/{entityid:[0-9]+}/customfield")]
        public IEnumerable<CustomFieldBaseWrapper> GetCustomFieldForSubject(String entityType, int entityid)
        {
            return DaoFactory.GetCustomFieldDao().GetEnityFields(ToEntityType(entityType), entityid, false).ConvertAll(item => ToCustomFieldBaseWrapper(item)).ToItemList();

        }

        /// <summary>
        ///    Sets the new user field value using the entity type, ID, field ID and value specified in the request
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <param name="entityid">ID</param>
        /// <param name="fieldid">Field ID</param>
        /// <param name="fieldValue">Field Value</param>
        /// <short>Set user field value</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        [Create("{entityType:(contact|person|company|opportunity|case)}/{entityid:[0-9]+}/customfield/{fieldid:[0-9]+}")]
        public CustomFieldBaseWrapper SetEntityCustomFieldValue(String entityType, int entityid, int fieldid, String fieldValue)
        {
            var customField = DaoFactory.GetCustomFieldDao().GetFieldDescription(fieldid);

            var entityTypeStr = ToEntityType(entityType);

            customField.EntityID = entityid;
            customField.Value = fieldValue;

            DaoFactory.GetCustomFieldDao().SetFieldValue(entityTypeStr, entityid, fieldid, fieldValue);

            return ToCustomFieldBaseWrapper(customField);
        }

        /// <summary>
        ///    Creates a new user field with the parameters (entity type, field title, type, etc.) specified in the request
        /// </summary>
        /// <param optional="true" name="entityType">Entity type</param>
        /// <param optional="true" name="label">Field title</param>
        /// <param name="fieldType" 
        /// remark="Allowed values: TextField, TextArea, SelectBox, CheckBox, Heading or Date">
        ///   User field value
        /// </param>
        /// <param optional="true" name="position">Field position</param>
        /// <param optional="true" name="mask" remark="Sent in json format only" >Mask</param>
        /// <short>Create user field</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        ///<example>
        /// <![CDATA[
        /// Data transfer in application/json format:
        /// 
        /// 1) Creation of a user field of  TextField type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample TextField",
        ///    fieldType: 0,          
        ///    position: 0,
        ///    mask: {"size":"40"}        - this is the text field size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 2) Creation of a user field of TextArea type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample TextArea",
        ///    fieldType: 1,
        ///    position: 1,
        ///    mask: '{"rows":"2","cols":"30"}'        - this is the TextArea size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 3) Creation of a user field of   SelectBox type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample SelectBox",
        ///    fieldType: 2,
        ///    position: 0,
        ///    mask: ["1","2","3"]   - SelectBox values.
        /// }
        /// 
        /// 
        /// 
        /// 4) Creation of a user field of  CheckBox type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample CheckBox",
        ///    fieldType: 3,
        ///    position: 0,
        ///    mask: ""     
        /// }
        /// 
        /// 
        /// 
        /// 5) Creation of a user field of   Heading type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample Heading",
        ///    fieldType: 4,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// 
        /// 6) Creation of a user field of   Date type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample Date",
        ///    fieldType: 5,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// ]]>
        /// </example>
        [Create("{entityType:(contact|person|company|opportunity|case)}/customfield")]
        public CustomFieldWrapper CreateCustomFieldValue(
            String entityType,
            String label,
            int fieldType,
            int position,
            String mask)
        {
            var fieldID = DaoFactory.GetCustomFieldDao().CreateField(
                          ToEntityType(entityType),
                          label,
                          (CustomFieldType)fieldType,
                          mask);

            return ToCustomFieldWrapper(DaoFactory.GetCustomFieldDao().GetFieldDescription(fieldID));

        }

        /// <summary>
        ///    Updates the selected user field with the parameters (entity type, field title, type, etc.) specified in the request
        /// </summary>
        /// <param name="id">User field id</param>
        /// <param name="entityType">Entity type</param>
        /// <param optional="true" name="label">Field title</param>
        /// <param name="fieldType" 
        /// remark="Allowed values: 0 (TextField),1 (TextArea),2 (SelectBox),3 (CheckBox),4 (Heading) or 5 (Date)">
        ///   User field value
        /// </param>
        /// <param optional="true" name="position">Field position</param>
        /// <param optional="true" name="mask" remark="Sent in json format only" >Mask</param>
        /// <short> Updates the selected user field</short> 
        /// <category>User fields</category>
        /// <returns>
        ///    User field
        /// </returns>
        ///<example>
        /// <![CDATA[
        /// Data transfer in application/json format:
        /// 
        /// 1) Creation of a user field of  TextField type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample TextField",
        ///    fieldType: 0,          
        ///    position: 0,
        ///    mask: {"size":"40"}        - this is the text field size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 2) Creation of a user field of TextArea type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample TextArea",
        ///    fieldType: 1,
        ///    position: 1,
        ///    mask: '{"rows":"2","cols":"30"}'        - this is the TextArea size. All other values are ignored.
        /// }
        /// 
        /// 
        /// 3) Creation of a user field of   SelectBox type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample SelectBox",
        ///    fieldType: 2,
        ///    position: 0,
        ///    mask: ["1","2","3"]   - SelectBox values.
        /// }
        /// 
        /// 
        /// 
        /// 4) Creation of a user field of  CheckBox type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample CheckBox",
        ///    fieldType: 3,
        ///    position: 0,
        ///    mask: ""     
        /// }
        /// 
        /// 
        /// 
        /// 5) Creation of a user field of   Heading type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample Heading",
        ///    fieldType: 4,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// 
        /// 6) Creation of a user field of   Date type
        /// 
        /// {
        ///    entityType: "contact",
        ///    label: "Sample Date",
        ///    fieldType: 5,
        ///    position: 0,
        ///    mask: "" 
        /// }
        /// 
        /// 
        /// ]]>
        /// </example>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("{entityType:(contact|person|company|opportunity|case)}/customfield/{id:[0-9]+}")]
        public CustomFieldWrapper UpdateCustomFieldValue(
            int id,
            String entityType,
            String label,
            int fieldType,
            int position,
            String mask)
        {

            if (id <= 0)
                throw new ArgumentException();

             if (!DaoFactory.GetCustomFieldDao().IsExist(id))
                 throw new ItemNotFoundException();


            var customField = new CustomField
                                  {
                                      EntityType = ToEntityType(entityType),
                                      FieldType = (CustomFieldType)fieldType,
                                      ID = id,
                                      Mask = mask,
                                      Label = label,
                                      Position = position
                                  };


            DaoFactory.GetCustomFieldDao().EditItem(customField);


            return ToCustomFieldWrapper(DaoFactory.GetCustomFieldDao().GetFieldDescription(id));

        }

        /// <summary>
        ///    Deletes the user field with the ID specified in the request
        /// </summary>
        /// <param name="entityType">Type</param>
        /// <param name="fieldid">Field ID</param>
        /// <short>Delete user field</short> 
        /// <category>User fields</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    User field
        /// </returns>
        [Delete("{entityType:(contact|person|company|opportunity|case)}/customfield/{fieldid:[0-9]+}")]
        public CustomFieldWrapper DeleteCustomField(String entityType, int fieldid)
        {

            if (fieldid <= 0)
                throw new ArgumentException();

            var customField = DaoFactory.GetCustomFieldDao().GetFieldDescription(fieldid);

            if (customField == null)
                throw new ItemNotFoundException();

            var result = ToCustomFieldWrapper(customField);

            DaoFactory.GetCustomFieldDao().DeleteField(fieldid);

            return result;
        }

        /// <summary>
        ///    Updates user fields order
        /// </summary>
        /// <param name="fieldids">User field ID list</param>
        /// <param name="entityType">Entity type</param>
        /// <category>User fields</category>
        /// <returns>
        ///    User fields
        /// </returns>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("{entityType:(contact|person|company|opportunity|case)}/customfield/reorder")]
        public IEnumerable<CustomFieldBaseWrapper> UpdateCustomFieldsOrder(IEnumerable<int> fieldids, String entityType)
        {
            if (fieldids == null)
                throw new ArgumentException();

            if (!(CRMSecurity.IsAdmin))
                throw new SecurityException("");

            var result = new List<CustomFieldBaseWrapper>();
            foreach (var id in fieldids)
            {
                try
                {
                    if (!DaoFactory.GetCustomFieldDao().IsExist(id))
                        throw new ItemNotFoundException();

                    var userFieldWrapper = ToCustomFieldBaseWrapper(DaoFactory.GetCustomFieldDao().GetFieldDescription(id));

                    result.Add(userFieldWrapper);

                }
                catch (ArgumentException)
                {}
            }

            DaoFactory.GetCustomFieldDao().ReorderFields(fieldids.ToArray<int>());
 
            return result;
        }

        private CustomFieldBaseWrapper ToCustomFieldBaseWrapper(CustomField customField)
        {
            return new CustomFieldBaseWrapper(customField);
        }

        private CustomFieldWrapper ToCustomFieldWrapper(CustomField customField)
        {
            var result = new CustomFieldWrapper(customField);
            result.RelativeItemsCount = DaoFactory.GetCustomFieldDao().GetContactLinkCount(customField.EntityType, customField.ID);
            return result;
        }

        private CustomField ToCustomField(CustomFieldBaseWrapper customFieldWrapper)
        {

            return new CustomField
                       {

                           EntityID = customFieldWrapper.EntityId,
                           FieldType = customFieldWrapper.FieldType,
                           ID = customFieldWrapper.ID,
                           Label = customFieldWrapper.Label,
                           Mask = customFieldWrapper.Mask,
                           Position = customFieldWrapper.Position,
                           Value = customFieldWrapper.FieldValue
                       };

        }

        private EntityType ToEntityType(String entityTypeStr)
        {
            EntityType entityType;

            if (String.IsNullOrEmpty(entityTypeStr))
                return EntityType.Any;

            switch (entityTypeStr.ToLower())
            {
                case "person":
                    entityType = EntityType.Person;
                    break;
                case "company":
                    entityType = EntityType.Company;
                    break;
                case "contact":
                    entityType = EntityType.Contact;
                    break;
                case "opportunity":
                    entityType = EntityType.Opportunity;
                    break;
                case "case":
                    entityType = EntityType.Case;
                    break;
                default:
                    entityType = EntityType.Any;
                    break;
            }

            return entityType;
        }
    }
}
