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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class TemplateEngine
    {
        private readonly EngineFactory _factory;
        private readonly ITemplateDao _dao;

		public TemplateEngine(IDaoFactory daoFactory, EngineFactory factory)
		{
		    _factory = factory;
		    _dao = daoFactory.GetTemplateDao();			
		}

        public List<Template> GetAll()
        {
            return _dao.GetAll();
        }

        public Template GetByID(int id)
        {
            return _dao.GetByID(id);
        }

        public Template SaveOrUpdate(Template template)
        {
            if (template.Id == default(int))
            {
                if (template.CreateBy == default(Guid)) template.CreateBy = SecurityContext.CurrentAccount.ID;
                if (template.CreateOn == default(DateTime)) template.CreateOn = TenantUtil.DateTimeNow();
            }

            template.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            template.LastModifiedOn = TenantUtil.DateTimeNow();

            return _dao.Save(template);
        }

        public void Delete(int id)
        {
            _dao.Delete(id);
        }
    }
}
