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
using System.Web;
using log4net;

namespace ASC.Api
{
    public class ApiSetupHttpModule : IHttpModule
    {
        private static volatile bool initialized = false;
        private static object locker = new object();


        public void Init(HttpApplication context)
        {
            if (!initialized)
            {
                lock (locker)
                {
                    if (!initialized)
                    {
                        try
                        {
                            ApiSetup.ConfigureEntryPoints();
                            ApiSetup.RegisterRoutes();
                            initialized = true;
                        }
                        catch (Exception err)
                        {
                            if (err is TypeInitializationException && err.InnerException != null)
                            {
                                err = err.InnerException;
                            }
                            LogManager.GetLogger(GetType()).Error(err);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
