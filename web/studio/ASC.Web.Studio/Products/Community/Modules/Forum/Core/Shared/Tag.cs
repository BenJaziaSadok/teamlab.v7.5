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

using System.Reflection;

namespace ASC.Forum
{
    public class Tag
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual int TenantID { get; set; }
    }


	public class RankTag : Tag
    {
        

        public virtual int Rank { get; internal set; }

        public RankTag()
        {
            this.Rank = 0;
        }

        public RankTag(int id, string name)
        {
            this.ID = id;
            this.Name = name;
            this.IsApproved = false;
            this.Rank = 0;
        }
    }
}
