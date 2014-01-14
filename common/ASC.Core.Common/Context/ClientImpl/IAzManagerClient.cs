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
using System.Collections.Generic;
using ASC.Common.Security;

namespace ASC.Core
{
    public interface IAzManagerClient
    {
        IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId);

        IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId, ISecurityObjectId objectId);

        IEnumerable<AzRecord> GetAcesWithInherits(Guid subjectId, Guid actionId, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider);

        void AddAce(AzRecord azRecord);

        void RemoveAce(AzRecord azRecord);

        void RemoveAllAces(ISecurityObjectId id);
    }
}