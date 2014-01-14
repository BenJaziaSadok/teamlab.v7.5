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
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class ParticipantEngine
    {
        private readonly IParticipantDao _participantDao;


        public ParticipantEngine(IDaoFactory daoFactory)
        {
            _participantDao = daoFactory.GetParticipantDao();
        }


        public Participant GetByID(Guid userID)
        {
            return new Participant(userID);
        }

        public void AddToFollowingProjects(int project, Guid participant)
        {
            _participantDao.AddToFollowingProjects(project, participant);
        }

        public void RemoveFromFollowingProjects(int project, Guid participant)
        {
            _participantDao.RemoveFromFollowingProjects(project, participant);
        }

        public List<int> GetInterestedProjects(Guid participant)
        {
            return _participantDao.GetInterestedProjects(participant);
        }
        public List<int> GetFollowingProjects(Guid participant)
        {
            return new List<int>(_participantDao.GetFollowingProjects(participant));
        }

        public List<int> GetMyProjects(Guid participant)
        {
            return new List<int>(_participantDao.GetMyProjects(participant));
        }
    }
}