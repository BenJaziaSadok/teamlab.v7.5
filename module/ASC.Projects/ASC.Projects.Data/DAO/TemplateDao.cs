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
using System.Linq;
using ASC.Common.Data;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Data.DAO
{
    class TemplateDao : BaseDao, ITemplateDao
    {
        private readonly string[] templateColumns = new[] { "id", "title", "description", "create_by", "create_on" };

        public TemplateDao(string dbId, int tenant)
            : base(dbId, tenant)
        {

        }

        public List<Template> GetAll()
        {
            var q = Query(TemplatesTable + " p").Select(templateColumns);

            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(q).ConvertAll(ToTemplate);
            }
        }

        public Template GetByID(int id)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var query = Query(TemplatesTable + " p").Select(templateColumns).Where("p.id", id);
                return db.ExecuteList(query).ConvertAll(ToTemplate).SingleOrDefault();
            }
        }

        public Template Save(Template template)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var insert = Insert(TemplatesTable)
                    .InColumnValue("id", template.Id)
                    .InColumnValue("title", template.Title)
                    .InColumnValue("description", template.Description)
                    .InColumnValue("create_by", template.CreateBy.ToString())
                    .InColumnValue("create_on", TenantUtil.DateTimeToUtc(template.CreateOn))
                    .InColumnValue("last_modified_by", template.LastModifiedBy.ToString())
                    .InColumnValue("last_modified_on", TenantUtil.DateTimeToUtc(template.LastModifiedOn))
                    .Identity(1, 0, true);

                template.Id = db.ExecuteScalar<int>(insert);

                return template;
            }
        }

        public void Delete(int id)
        {
            using (var db = new DbManager(DatabaseId))
            {
                db.ExecuteNonQuery(Delete(TemplatesTable).Where("id", id));
            }
        }

        private static Template ToTemplate(IList<object> r)
        {
            return new Template
            {
                Id = Convert.ToInt32(r[0]),
                Title = (string)r[1],
                Description = (string)r[2],
                CreateBy = new Guid((string)r[3]),
                CreateOn = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[4]))
            };
        }
    }
}
