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

using System.Collections.Generic;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Thrdparty.TokenManagers
{
    public interface IAssociatedTokenManager : IConsumerTokenManager
    {
        IEnumerable<string> GetAccessTokens();
        IEnumerable<string> GetAssociations();
        string GetAssociation(string token);
        IEnumerable<string> GetTokensByAssociation(string associateData);
        void AssociateToken(string token, string associateData);

        void RemoveAssociationFromToken(string token);
        void ExpireToken(string token);
    }
}