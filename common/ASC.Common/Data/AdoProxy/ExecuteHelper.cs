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
using System.Diagnostics;

namespace ASC.Common.Data.AdoProxy
{
    class ExecuteHelper : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly Action<TimeSpan> onStop;


        private ExecuteHelper(Action<TimeSpan> onStop)
        {
            if (onStop == null) throw new ArgumentNullException("onStop");

            this.onStop = onStop;
            stopwatch = Stopwatch.StartNew();
        }


        public void Dispose()
        {
            stopwatch.Stop();
            try
            {
                onStop(stopwatch.Elapsed);
            }
            catch { }
        }


        public static IDisposable Begin(Action<TimeSpan> onStop)
        {
            return new ExecuteHelper(onStop);
        }
    }
}
