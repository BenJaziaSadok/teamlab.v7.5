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

using System.Collections.Generic;
using System.Collections;

#endregion

namespace ASC.Projects.Core.Domain
{
    public class EntityList<TEntity> : IEnumerable<TEntity>
     where TEntity : class
    {

        #region Members

        private readonly IList _entities;

        private readonly IList<TEntity> _copyEntities = new List<TEntity>();

        #endregion

        #region Constructor

        public EntityList(IList entities)
        {

            _entities = entities;

            foreach (TEntity entity in entities)
                _copyEntities.Add(entity);

        }

        #endregion

        #region Property

        public TEntity this[int i]
        {
            get
            {
                return _copyEntities[i];
            }
        }

        public int Count
        {
            get { return _entities.Count; }
        }

        #endregion

        #region Methods

        public void Add(TEntity entity)
        {

            if (entity != null && !_entities.Contains(entity))
            {
                _entities.Add(entity);

                _copyEntities.Add(entity);
            }

        }

        public void Remove(TEntity entity)
        {
            if (entity != null && _entities.Contains(entity))
            {
                _entities.Remove(entity);

                _copyEntities.Remove(entity);
            }
        }

        #endregion


        #region Implementation of IEnumerable

        public IEnumerator<TEntity> GetEnumerator()
        {

            return _copyEntities.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
