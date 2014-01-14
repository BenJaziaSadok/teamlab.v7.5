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
    public abstract class ProgressTask : IProgressTask
    {
        private int _progress;
        private int _stepsCount;
        private int _stepsCompleted;

        public abstract void Run();

        protected void InitProgress(int stepsCount)
        {
            _stepsCount = stepsCount;
            _stepsCompleted = 0;
            _progress = 0;
        }

        protected void RunSubtask(IProgressTask task)
        {
            task.Message += (sender, args) => InvokeMessage(args.Reason, args.Message);
            task.ProgressChanged += (sender, args) => SetStepProgress(args.Progress);
            task.Run();
        }

        protected void SetProgress(int progress)
        {
            if (_progress != progress)
            {
                _progress = progress;
                InvokeProgressChanged(_progress);
            }
        }

        protected void SetStepProgress(int progress)
        {
            SetProgress((int)((_stepsCompleted*100 + progress)/(double)_stepsCount));
            
            if (progress == 100)
            {
                _stepsCompleted++;
            }
        }

        protected void SetStepCompleted()
        {
            SetProgress((int)((++_stepsCompleted * 100) / (double)_stepsCount));
        }

        #region events

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<MessageEventArgs> Message; 

        protected void InvokeProgressChanged(int progress)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(progress));
            }
        }

        protected void InvokeInfo(string messageFormat, params object[] args)
        {
            InvokeMessage(MessageReason.Info, messageFormat, args);
        }

        protected void InvokeWarning(string messageFormat, params object[] args)
        {
            InvokeMessage(MessageReason.Warning, messageFormat, args);
        }

        protected void InvokeMessage(MessageReason reason, string messageFormat, params object[] args)
        {
            if (args.Length > 0)
            {
                messageFormat = string.Format(messageFormat, args);
            }
            if (Message != null)
            {
                Message(this, new MessageEventArgs(messageFormat, reason));
            }
        }

        #endregion
    }
}
