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
using System.Xml.Linq;

namespace ASC.Data.Backup
{
    public interface IBackupProvider
    {
        string Name
        {
            get;
        }

        IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer);

        void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader);

        event EventHandler<ProgressChangedEventArgs> ProgressChanged;

    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public string Status
        {
            get;
            private set;
        }

        public double Progress
        {
            get;
            private set;
        }

        public bool Completed
        {
            get;
            private set;
        }

        public ProgressChangedEventArgs(string status, double progress)
            : this(status, progress, false)
        {
        }

        public ProgressChangedEventArgs(string status, double progress, bool completed)
        {
            Status = status;
            Progress = progress;
            Completed = completed;
        }
    }
}