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

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IParticipantDao
    {
        int[] GetFollowingProjects(Guid participant);

        int[] GetMyProjects(Guid participant);

        List<int> GetInterestedProjects(Guid participant);

        void AddToFollowingProjects(int project, Guid participant);

        void RemoveFromFollowingProjects(int project, Guid participant);
    }
}
