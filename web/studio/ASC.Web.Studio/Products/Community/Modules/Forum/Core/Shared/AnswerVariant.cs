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
	public class AnswerVariant
    {
        public virtual int ID { get; set; }

        public virtual string Name { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int AnswerCount { get; set; }

        public virtual int QuestionID { get; set; }
        
    }
}
