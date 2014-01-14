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
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Notify.Channels
{
    public class SenderChannel : ISenderChannel
    {
        private ISink firstSink;
        private ISink senderSink;
        private Context context;


        public string SenderName
        {
            get;
            private set;
        }


        public SenderChannel(Context context, string senderName, ISink decorateSink, ISink senderSink)
        {
            if (senderName == null) throw new ArgumentNullException("senderName");
            if (context == null) throw new ArgumentNullException("context");
            if (senderSink == null) throw new ApplicationException(string.Format("channel with tag {0} not created sender sink", senderName));

            this.context = context;
            this.SenderName = senderName;
            this.firstSink = decorateSink;
            this.senderSink = senderSink;
            
            var dispatcherSink = new DispatchSink(SenderName, this.context.DispatchEngine);
            this.firstSink = AddSink(firstSink, dispatcherSink);
        }

        public void SendAsync(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            firstSink.ProcessMessageAsync(message);
        }

        public SendResponse DirectSend(INoticeMessage message)
        {
            return senderSink.ProcessMessage(message);
        }

        
        private ISink AddSink(ISink firstSink, ISink addedSink)
        {
            if (firstSink == null) return addedSink;
            if (addedSink == null) return firstSink;

            var current = firstSink;
            while (current.NextSink != null)
            {
                current = current.NextSink;
            }
            current.NextSink = addedSink;
            return firstSink;
        }
    }
}
