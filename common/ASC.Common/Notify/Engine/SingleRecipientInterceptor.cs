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
using ASC.Notify.Recipients;

namespace ASC.Notify.Engine
{
    class SingleRecipientInterceptor : ISendInterceptor
    {
        private const string prefix = "__singlerecipientinterceptor";
        private readonly List<IRecipient> sendedTo = new List<IRecipient>(10);


        public string Name { get; private set; }

        public InterceptorPlace PreventPlace { get { return InterceptorPlace.GroupSend | InterceptorPlace.DirectSend; } }

        public InterceptorLifetime Lifetime { get { return InterceptorLifetime.Call; } }


        internal SingleRecipientInterceptor(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");
            Name = name;
        }

        public bool PreventSend(NotifyRequest request, InterceptorPlace place)
        {
            var sendTo = request.Recipient;
            if (!sendedTo.Exists(rec => Equals(rec, sendTo)))
            {
                sendedTo.Add(sendTo);
                return false;
            }
            return true;
        }
    }
}