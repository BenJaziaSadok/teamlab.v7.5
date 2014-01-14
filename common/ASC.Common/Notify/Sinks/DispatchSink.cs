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
using ASC.Notify.Engine;
using ASC.Notify.Messages;

namespace ASC.Notify.Sinks
{
    class DispatchSink : Sink
    {
        private readonly string senderName;
        private readonly DispatchEngine dispatcher;

        public DispatchSink(string senderName, DispatchEngine dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            
            this.dispatcher = dispatcher;
            this.senderName = senderName;
        }

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            return dispatcher.Dispatch(message, senderName);
        }

        public override void ProcessMessageAsync(INoticeMessage message)
        {
            dispatcher.Dispatch(message, senderName);
        }
    }
}