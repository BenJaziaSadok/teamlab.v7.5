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
using System.Threading;

namespace ASC.Data.Backup
{
    public static class ActionInvoker
    {
        public static void Try(
            Action action, 
            int maxAttempts,
            Action<Exception> onFailure = null,
            Action<Exception> onAttemptFailure = null,
            int sleepMs = 1000,
            bool isSleepExponential = true)
        {
            Try(state => action(), null, maxAttempts, onAttemptFailure, onFailure, sleepMs, isSleepExponential);
        }

        public static void Try(
            Action<object> action,
            object state,
            int maxAttempts,
            Action<Exception> onFailure = null,
            Action<Exception> onAttemptFailure = null,
            int sleepMs = 1000,
            bool isSleepExponential = true)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            int countAttempts = 0;
            while (countAttempts++ < maxAttempts)
            {
                try
                {
                    action(state);
                    return;
                }
                catch (Exception error)
                {
                    if (countAttempts < maxAttempts)
                    {
                        if (onAttemptFailure != null)
                            onAttemptFailure(error);

                        if (sleepMs > 0) 
                            Thread.Sleep(isSleepExponential ? sleepMs*countAttempts : sleepMs);
                    }
                    else if (onFailure != null)
                    {
                        onFailure(error);
                    }
                }
            }
        }
    }
}
