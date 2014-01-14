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
using System.Web;
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewData["splash"] = "HomeSplash";
            return View();
        }

        public ActionResult Authentication()
        {
            return View();
        }

        public ActionResult Basic()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Filters()
        {
            return View();
        }

        public ActionResult Batch()
        {
            return View();
        }
    }
}
