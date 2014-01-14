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

namespace ASC.Data.Backup.Tasks
{
    public interface IProgressTask
    {
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        event EventHandler<MessageEventArgs> Message; 

        void Run();
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public int Progress { get; private set; }

        public ProgressChangedEventArgs(int progress)
        {
            Progress = progress;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public MessageReason Reason { get; private set; }

        public MessageEventArgs(string message, MessageReason reason)
        {
            Message = message;
            Reason = reason;
        }
    }

    public enum MessageReason
    {
        Info,
        Warning
    }
}