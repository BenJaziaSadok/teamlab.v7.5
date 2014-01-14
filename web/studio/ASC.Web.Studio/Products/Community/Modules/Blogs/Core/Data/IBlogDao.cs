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

using ASC.Blogs.Core.Domain;
using System;
using System.Collections.Generic;

namespace ASC.Blogs.Core.Data
{
    public interface IPostDao
    {
        List<Post> Select(Guid? id, long? blogId, Guid? userId, string tag, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount);

        List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool asc, int? from, int? count, bool fillTags, bool withCommentsCount);
        
        List<Post> Select(Guid? id, long? blogId, Guid? userId, bool withContent, bool fillTags, bool withCommentsCount);

        List<Post> GetPosts(List<Guid> ids,bool withContent,bool withTags);

        List<Guid> SearchPostsByWord(string word);

        void SavePost(Post post);
        void DeletePost(Guid postId);

        int GetCount(Guid? id, long? blogId, Guid? userId, string tag);

        List<Comment> GetComments(Guid postId);
        Comment GetCommentById(Guid commentId);        
        List<int> GetCommentsCount(List<Guid> postsIds);

        void SaveComment(Comment comment);

        List<TagStat> GetTagStat(int? top);
        List<string> GetTags(string like, int limit);

        void SavePostReview(Guid userId, Guid postId, DateTime datetime);
    }
}
