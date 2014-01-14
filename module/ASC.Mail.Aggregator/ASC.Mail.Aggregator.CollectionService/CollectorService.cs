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
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Net;
using ASC.Mail.Aggregator.Client;
using NLog;

namespace ASC.Mail.Aggregator.CollectionService
{
    public sealed class CollectorService: ServiceBase
    {
        public const string AscMailCollectionServiceName = "ASC Mail Collection Service";
        private readonly MailBoxManager _manger;
        private readonly Collector _collector;
        private readonly Logger _log;

        public CollectorService()
        {
            this.ServiceName = AscMailCollectionServiceName;
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            // type of event. Set to true if you need it, false otherwise.
            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = true;
            this.CanStop = true;
            try
            {
                _log = LogManager.GetLogger("CollectorService");
                _log.Info("Connecting to db...");
                _manger = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 25);
                _log.Info("Creating collector service...");
                _collector = new Collector(_manger, MailQueueSettings.FromConfig);
                _log.Info("Service is ready.");

                AggregatorLogger.Instance.Initialize(_manger, GetServiceIp());
                _log.Info("Aggregator logger initialized.");
            }
            catch (Exception ex)
            {
                _log.Fatal("CollectorService error under constuct: {0}", ex.ToString());
            }
        }

        private string GetServiceIp()
        {
            var dns_host_name = Dns.GetHostName();
            IPHostEntry ip_entry = Dns.GetHostEntry(dns_host_name);
            IPAddress first_or_default = ip_entry.AddressList.FirstOrDefault(x => x.AddressFamily != AddressFamily.InterNetworkV6);
            if (first_or_default != null)
                return first_or_default.ToString();

            return "Unspecified address for hostname: " + dns_host_name;
        }

        public void SaveMessage(string user_id, int tenant_id, string email, string uidl)
        { 
            _collector.SaveMessage(user_id, tenant_id, email, uidl);
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            _log.Info("Service Dispose(disposing = {0}).", disposing);
            base.Dispose(disposing);
        }

        /// <summary>
        /// OnStart(): Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _log.Info("Service start.");
            _collector.Start();
            base.OnStart(args);
        }

        /// <summary>
        /// OnStop(): Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            _log.Info("Service stop.");
            _collector.Stop();
            LogManager.Flush();
            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            _log.Info("Service pause.");
            LogManager.Flush();
            base.OnPause();
        }

        /// <summary>
        /// OnContinue(): Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            _log.Info("Service continue.");
            LogManager.Flush();
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            _log.Info("Service shutdown.");
            _collector.Stop();
            LogManager.Flush();
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            _log.Info("Service call custom command({0}).", command);
            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcast Status
        /// (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            _log.Info("Service power event detected = {0}.", powerStatus.ToString());
            LogManager.Flush();
            if (powerStatus == PowerBroadcastStatus.Suspend)
            {
                _collector.Stop();
            }
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event
        ///   from a Terminal Server session.
        ///   Useful if you need to determine
        ///   when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription">The Session Change
        /// Event that occured.</param>
        protected override void OnSessionChange(
                  SessionChangeDescription changeDescription)
        {
            _log.Info("Service session change event detected = {0}.", changeDescription.ToString());
            LogManager.Flush();
            base.OnSessionChange(changeDescription);
        }

        /// <summary>
        /// Start service in console mode.
        /// </summary>
        public void StartDaemon()
        {
            _log.Info("Service Start console daemon().");
            _collector.Start();
        }
    }
}