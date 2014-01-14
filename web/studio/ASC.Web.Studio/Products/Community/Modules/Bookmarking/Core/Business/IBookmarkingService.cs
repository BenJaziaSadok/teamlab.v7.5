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

#region Usings

using System;
using System.Collections.Generic;
using ASC.Bookmarking.Pojo;
using ASC.Core.Users;
using ASC.Notify.Model;

#endregion

namespace ASC.Bookmarking.Business
{
	public interface IBookmarkingService
	{
		IList<Bookmark> GetAllBookmarks(int firstResult, int maxResults);
		IList<Bookmark> GetAllBookmarks();

		Bookmark AddBookmark(Bookmark b, IList<Tag> tags);

		Bookmark UpdateBookmark(UserBookmark userBookmark, IList<Tag> tags);
		Bookmark UpdateBookmark(Bookmark bookmark, IList<Tag> tags);
		
		UserInfo GetCurrentUser();		
		
		IList<Tag> GetAllTags(string startSymbols, int limit);

		IList<Tag> GetAllTags();

		Bookmark GetBookmarkByUrl(string url);

		Bookmark GetBookmarkByID(long id);


		IList<UserBookmark> GetUserBookmarks(Bookmark b);

		UserBookmark GetCurrentUserBookmark(Bookmark b);

		Bookmark GetBookmarkWithUserBookmarks(string url);



		Bookmark RemoveBookmarkFromFavourite(long bookmarkID);

		IList<Bookmark> GetFavouriteBookmarksSortedByRaiting(int firstResult, int maxResults);
		IList<Bookmark> GetFavouriteBookmarksSortedByDate(int firstResult, int maxResults);


		IList<Bookmark> GetMostRecentBookmarks(int firstResult, int maxResults);
		IList<Bookmark> GetMostRecentBookmarksWithRaiting(int firstResult, int maxResults);
		IList<Bookmark> GetTopOfTheDay(int firstResult, int maxResults);
		IList<Bookmark> GetTopOfTheWeek(int firstResult, int maxResults);
		IList<Bookmark> GetTopOfTheMonth(int firstResult, int maxResults);
		IList<Bookmark> GetTopOfTheYear(int firstResult, int maxResults);

		#region Tags		

		IList<Bookmark> GetMostPopularBookmarksByTag(Tag t);
		IList<Bookmark> GetMostPopularBookmarksByTag(IList<Tag> tags);
		IList<Tag> GetBookmarkTags(Bookmark b);
		IList<Tag> GetUserBookmarkTags(UserBookmark b);
		
        #endregion

		#region Comments
		
        Comment GetCommentById(Guid commentID);

		void AddComment(Comment comment);

		void UpdateComment(Guid commentID, string text);

		void RemoveComment(Guid commentID);

		long GetCommentsCount(long bookmarkID);

		IList<Comment> GetBookmarkComments(Bookmark b);

		IList<Comment> GetChildComments(Comment c);
		
        #endregion

		void Subscribe(string objectID, INotifyAction notifyAction);

		bool IsSubscribed(string objectID, INotifyAction notifyAction);

		void UnSubscribe(string objectID, INotifyAction notifyAction);

		#region Search
		
        IList<Bookmark> SearchBookmarks(IList<string> searchStringList, int firstResult, int maxResults);

		IList<Bookmark> SearchAllBookmarks(IList<string> searchStringList);

		IList<Bookmark> SearchBookmarksSortedByRaiting(IList<string> searchStringList, int firstResult, int maxResults);

		IList<Bookmark> SearchBookmarksByTag(string searchString, int firstResult, int maxResults);

		IList<Bookmark> SearchMostPopularBookmarksByTag(string tagName, int firstResult, int maxResults);
		
        #endregion

		IList<Bookmark> GetBookmarksCreatedByUser(Guid userID, int firstResult, int maxResults);

		IList<Bookmark> GetMostPopularBookmarksCreatedByUser(Guid userID, int firstResult, int maxResults);

		long GetBookmarksCountCreatedByUser(Guid userID);


		IList<Bookmark> GetFullBookmarksInfo(IList<long> bookmarkIds);

		IList<Bookmark> GetFullBookmarksInfo(IList<Bookmark> bookmarks);

		void SetBookmarkTags(Bookmark b);
	}
}
