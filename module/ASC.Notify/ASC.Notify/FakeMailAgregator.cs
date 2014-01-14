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
using System.Linq;
using System.Text;
using ASC.Notify.Messages;
using System.Threading;

namespace ASC.Notify
{
    
    static class FakeMailAgregator
    {
        private static readonly Random rnd = new Random();

        internal static bool SendMail(NotifyMessage m)
        {
            Thread.Sleep(rnd.Next(1000, 3000));
            return (rnd.Next(100)>10);
        }
    }
}
