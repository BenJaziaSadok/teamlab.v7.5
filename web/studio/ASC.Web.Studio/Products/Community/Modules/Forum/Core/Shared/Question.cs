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
using System.Text;
using System.Reflection;

namespace ASC.Forum
{
    public enum QuestionType
    {
        SeveralAnswer =0,

        OneAnswer=1
    }

	public class Question
    {
        public virtual int ID { get; set; } 

        public virtual string Name { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual QuestionType Type { get; set; }

        public virtual List<AnswerVariant> AnswerVariants { get; set; }

        public virtual int TopicID { get; set; }

        public virtual int TenantID { get; set; }

        public Question()
        {   
            AnswerVariants = new List<AnswerVariant>(0);
        }
    }   
}
