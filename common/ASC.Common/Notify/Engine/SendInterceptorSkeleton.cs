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

namespace ASC.Notify.Engine
{
    public class SendInterceptorSkeleton : ISendInterceptor
    {
        private readonly Func<NotifyRequest, InterceptorPlace, bool> method;


        public string Name { get; internal set; }

        public InterceptorPlace PreventPlace { get; internal set; }

        public InterceptorLifetime Lifetime { get; internal set; }


        public SendInterceptorSkeleton(string name, InterceptorPlace preventPlace, InterceptorLifetime lifetime, Func<NotifyRequest, InterceptorPlace, bool> sendInterceptor)
        {
            if (String.IsNullOrEmpty("name")) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty("sendInterceptor")) throw new ArgumentNullException("sendInterceptor");

            method = sendInterceptor;
            Name = name;
            PreventPlace = preventPlace;
            Lifetime = lifetime;
        }

        public bool PreventSend(NotifyRequest request, InterceptorPlace place)
        {
            return method(request, place);
        }
    }
}