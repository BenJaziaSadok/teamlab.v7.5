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
using ASC.Common.Security;
using ASC.Forum.Module;
using ASC.Web.Community.Product;

namespace ASC.Forum
{   

    internal class SecurityActionPresenter : PresenterTemplate<ISecurityActionView>
    {
        protected override void RegisterView()
        {
            _view.ValidateAccess+=new EventHandler<SecurityAccessEventArgs>(ValidateAccessHandler);
        }

        private void ValidateAccessHandler(object sender, SecurityAccessEventArgs e)
        {
            ISecurityObject securityObject = null;
            if (e.TargetObject is ISecurityObject)
                securityObject = (ISecurityObject)e.TargetObject;

            switch (e.Action)
            {
                case ForumAction.ReadPosts:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.ReadPostsAction);
                    break;

                case ForumAction.PostCreate:
                    
                    Topic topic = (Topic)e.TargetObject;    
                    if (CommunitySecurity.CheckPermissions(topic, Constants.PostCreateAction))
                    {   
                        if(!topic.Closed)
                            _view.IsAccessible = true;

                        else if (topic.Closed && CommunitySecurity.CheckPermissions(topic, Constants.TopicCloseAction))
                            _view.IsAccessible = true;

                        else
                            _view.IsAccessible = false;
                    }
                    else
                        _view.IsAccessible = false;

                    break;
                    
                case ForumAction.ApprovePost:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.PostApproveAction);
                    break;

                case ForumAction.PostEdit:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.PostEditAction);
                    break;

                case ForumAction.PostDelete:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.PostDeleteAction);
                    break;

                case ForumAction.TopicCreate:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TopicCreateAction);
                    break;

                case ForumAction.PollCreate:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.PollCreateAction);
                    break;

                case ForumAction.TopicClose:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TopicCloseAction);
                    break;

                case ForumAction.TopicSticky:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TopicStickyAction);
                    break;

                case ForumAction.TopicEdit:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TopicEditAction);
                    break;

                case ForumAction.TopicDelete:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TopicDeleteAction);
                    break;

                case ForumAction.PollVote:

                    Question question = (Question)e.TargetObject;
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(new Topic() { ID = question.TopicID}, Constants.PollVoteAction);
                    break;


                case ForumAction.TagCreate:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TagCreateAction);
                    break;
                
                case ForumAction.AttachmentCreate:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.AttachmentCreateAction);
                    break;

                case ForumAction.AttachmentDelete:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.AttachmentDeleteAction);
                    break;
               
                case ForumAction.GetAccessForumEditor:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.ForumManagementAction);
                    break;

                case ForumAction.GetAccessTagEditor:
                    _view.IsAccessible = CommunitySecurity.CheckPermissions(securityObject, Constants.TagManagementAction);
                    break;
            }
        }
    }
}
