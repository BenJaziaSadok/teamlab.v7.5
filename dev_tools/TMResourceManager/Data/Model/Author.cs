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

namespace TMResourceData.Model
{
    public class Author
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public List<ResCulture> Langs { get; set; }
        public List<ResProject> Projects { get; set; }

        public Author()
        {}

        public Author(string login)
        {
            Login = login;
            IsAdmin = false;
            Langs = new List<ResCulture>();
            Projects = new List<ResProject>();
        }
    }
}
