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
using System.Runtime.InteropServices;
using System.ServiceProcess;
using NLog;


namespace ASC.Mail.Aggregator.CollectionService
{
    internal static class ConsoleHandler
    {

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public static readonly HandlerRoutine GoodHandler = ConsoleCtrlCheck;

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            AggregatorLogger.Instance.Stop();
            return true;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            ConsoleHandler.SetConsoleCtrlHandler(ConsoleHandler.GoodHandler, true);

            var service = new CollectorService();

            if ((args.Length == 5 && args[0] == "-d"))
            {
                service.SaveMessage(args[1], Convert.ToInt32(args[2]), args[3], args[4]);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            else if (Environment.UserInteractive)
            {
                service.StartDaemon();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }


        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Fatal("Unhandled exception: {0}", e.ExceptionObject.ToString());
            LogManager.Flush();
        }
    }
}
