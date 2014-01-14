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
using ASC.Api.Attributes;
using ASC.Api.Exceptions;
using ASC.Api.Projects.Wrappers;
using ASC.Api.Utils;
using ASC.Projects.Core.Domain;

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
        #region comments
		 ///<summary>
		 ///Returns the information about the comment with the ID specified in the request
		 ///</summary>
		 ///<short>
		 ///Get comment
		 ///</short>
		 ///<category>Comments</category>
		 ///<param name="commentid">Comment ID</param>
        ///<returns>Comment</returns>        
        /// <exception cref="ItemNotFoundException"></exception>
        [Read(@"comment/{commentid}")]
        public CommentWrapper GetComment(Guid commentid)
        {
            return new CommentWrapper(EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull());
        }

		  ///<summary>
		  ///Updates the seleted comment using the comment text specified in the request
		  ///</summary>
		  ///<short>
		  ///Update comment
		  ///</short>
		  /// <category>Comments</category>
		  ///<param name="commentid">comment ID</param>
        ///<param name="content">comment text</param>
        ///<returns>Comment</returns>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <example>
        /// <![CDATA[
        /// Sending data in application/json:
        /// 
        /// {
        ///     text:"My comment text",
        ///     
        /// }
        /// 
        /// Sending data in application/x-www-form-urlencoded
        /// content=My%20comment%20text
        /// ]]>
        /// </example>
        [Update(@"comment/{commentid}")]
        public CommentWrapper UpdateComments(Guid commentid, string content)
		 {
            var comment = EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull();
            
            comment.Content = Update.IfNotEquals(comment.Content, content);

            return new CommentWrapper(SaveComment(comment));
        }

        ///<summary>
        ///Delete the comment with the ID specified in the request from the portal
        ///</summary>
        ///<short>
        ///Delete comment
        ///</short>
        /// <category>Comments</category>
        ///<param name="commentid">comment ID</param>
        /// <exception cref="ItemNotFoundException"></exception>
        [Delete(@"comment/{commentid}")]
        public CommentWrapper DeleteComments(Guid commentid)
        {
            var comment = EngineFactory.GetCommentEngine().GetByID(commentid).NotFoundIfNull();

            comment.Inactive = true;

            return new CommentWrapper(SaveComment(comment));
        }

        private Comment SaveComment(Comment comment)
        {
            var targetUniqID = comment.TargetUniqID.Split('_');
            var entityType = targetUniqID[0];
            var entityId = targetUniqID[1];

            switch (entityType)
            {
                case "Task":
                    var taskEngine = EngineFactory.GetTaskEngine();
                    var task = taskEngine.GetByID(Convert.ToInt32(entityId)).NotFoundIfNull();
                    comment = taskEngine.SaveOrUpdateComment(task, comment);
                    break;

                case "Message":
                    var messageEngine = EngineFactory.GetMessageEngine();
                    var message = messageEngine.GetByID(Convert.ToInt32(entityId)).NotFoundIfNull();
                    comment = messageEngine.SaveOrUpdateComment(message, comment);
                    break;
            }

            return comment;
        }

        #endregion
    }
}