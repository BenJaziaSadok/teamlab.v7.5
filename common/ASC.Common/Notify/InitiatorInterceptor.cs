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

using System.Linq;
using ASC.Notify.Engine;
using ASC.Notify.Recipients;

namespace ASC.Notify
{
    public class InitiatorInterceptor : SendInterceptorSkeleton
    {
        public InitiatorInterceptor(params IRecipient[] initiators)
            : base("Sys.InitiatorInterceptor", InterceptorPlace.GroupSend | InterceptorPlace.DirectSend, InterceptorLifetime.Call,
                (r, p) => (initiators ?? Enumerable.Empty<IRecipient>()).Any(recipient => r.Recipient.Equals(recipient)))
        {
        }
    }
}