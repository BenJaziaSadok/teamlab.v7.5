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

namespace ASC.CRM.Core
{

    public enum TaskSortedByType
    {
        Title,
        Category,
        DeadLine,
        Contact,
        ContactManager
    }

    public enum DealSortedByType
    {
        Title,
        Responsible,
        Stage,
        BidValue,
        DateAndTime
    }

    public enum RelationshipEventByType
    {
        Created,
        CreateBy,
        Category,
        Content
    }

    public enum ContactSortedByType
    {
        DisplayName,
        ContactType,
        Created,
        FirstName, 
        LastName
    }

    public enum SortedByType
    {
        DateAndTime,
        Title,
        CreateBy
    }
}
