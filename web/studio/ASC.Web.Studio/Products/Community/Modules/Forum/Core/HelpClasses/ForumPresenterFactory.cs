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
using System.Reflection;
using System.Text;

namespace ASC.Forum
{
    public class ForumPresenterFactory : IPresenterFactory
    {
        public IPresenter GetPresenter<T>() where T : class
        {
            IPresenter presenter = null;            

            if (typeof(T).Equals(typeof(ISecurityActionView)))
                presenter = new SecurityActionPresenter();
         
            else if (typeof(T).Equals(typeof(INotifierView)))
                presenter = new NotifierPresenter();

            else if (typeof(T).Equals(typeof(ISubscriberView)))
                presenter = new SubscriberPresenter();

            else if (typeof(T).Equals(typeof(ISubscriptionGetcherView)))
                presenter = new SubscriptionGetcherPresenter();

            return presenter;
        }    
    }
}
