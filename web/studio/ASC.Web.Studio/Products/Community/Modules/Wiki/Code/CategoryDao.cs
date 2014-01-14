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
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Web.UserControls.Wiki.Data
{
    class CategoryDao : BaseDao
    {
        public CategoryDao(string dbid, int tenant)
            : base(dbid, tenant)
        {
        }


        public List<Category> GetCategories()
        {
            var q = Query("wiki_categories").Select("CategoryName").GroupBy(1);

            return db.ExecuteList(q)
                .ConvertAll(r => ToCategory(r));
        }

        public List<Category> GetCategories(string pagename, bool onlyWithThisPage)
        {
            var q = Query("wiki_categories c2").Select("c2.CategoryName").Where("c2.PageName", pagename);
            if (onlyWithThisPage)
            {
                q = Query("wiki_categories c1").Select("c1.CategoryName")
                    .InnerJoin(q, "t", Exp.EqColumns("c1.CategoryName", "t.CategoryName"))
                    .GroupBy(1)
                    .Having(Exp.Eq("count(*)", 1));
            }

            return db.ExecuteList(q)
                .ConvertAll(r => ToCategory(r));
        }

        public Category SaveCategory(Category category)
        {
            if (category == null) throw new ArgumentNullException("category");
            if (string.IsNullOrEmpty(category.CategoryName) || string.IsNullOrEmpty(category.PageName)) return category;

            var i = Insert("wiki_categories")
                .InColumnValue("CategoryName", category.CategoryName)
                .InColumnValue("PageName", category.PageName);
            db.ExecuteNonQuery(i);
            return category;
        }

        public void RemoveCategories(string pagename)
        {
            var d = Delete("wiki_categories").Where("PageName", pagename);
            db.ExecuteNonQuery(d);
        }


        private Category ToCategory(object[] r)
        {
            return new Category
            {
                CategoryName = (string)r[0],
                PageName = 1 < r.Length ? (string)r[1] : string.Empty,
            };
        }
    }
}