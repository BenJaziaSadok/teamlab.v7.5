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
using ASC.Notify.Model;

namespace ASC.Notify.Patterns
{
    public interface IPatternProvider
    {
        Func<INotifyAction, string, NotifyRequest, IPattern> GetPatternMethod { get; set; }

        IPattern GetPattern(INotifyAction action, string senderName);
        
        IPatternFormatter GetFormatter(IPattern pattern);
    }
}