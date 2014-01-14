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
using ASC.FullTextIndex.Service.Config;
using log4net;

namespace ASC.FullTextIndex.Service
{
    class TextIndexerService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TextIndexerService));

        private readonly TextIndexCfg configuration;
        private readonly TenantsProvider tenantsProvider;

        private readonly Thread worker;
        private readonly ManualResetEvent stop;


        public TextIndexerService()
        {
            configuration = new TextIndexCfg();
            tenantsProvider = new TenantsProvider(configuration.ConnectionStringName, configuration.UserActivityDays);

            worker = new Thread(DoWork) { Priority = ThreadPriority.Lowest, Name = "Full Text Indexer", };
            stop = new ManualResetEvent(false);
        }


        public void Start()
        {
            worker.Start();
        }

        public void Stop()
        {
            stop.Set();
            worker.Join();
            stop.Close();
        }


        private void DoWork()
        {
            var period = TimeSpan.FromSeconds(1);
            var action = TextIndexAction.None;
            do
            {
                try
                {
                    if (stop.WaitOne(period))
                    {
                        return;
                    }

                    DoIndex(action);

                    var now = DateTime.UtcNow;
                    var indexDateTime = configuration.ChangedCron.GetTimeAfter(now) ?? DateTime.MaxValue;
                    var removeDateTime = configuration.RemovedCron.GetTimeAfter(now) ?? DateTime.MaxValue;

                    action = TextIndexAction.Index | TextIndexAction.Remove;
                    if (indexDateTime < removeDateTime) action = TextIndexAction.Index;
                    if (indexDateTime > removeDateTime) action = TextIndexAction.Remove;

                    period = ((indexDateTime < removeDateTime ? indexDateTime : removeDateTime) - now).Add(TimeSpan.FromSeconds(1));

                    log.DebugFormat("Next action '{0}' over {1}", action, period);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Error in DoIndex: {0}", ex);
                    period = TimeSpan.FromMinutes(10);
                }
            }
            while (true);
        }

        private void DoIndex(TextIndexAction action)
        {
            if (action == TextIndexAction.None) return;
            foreach (var t in tenantsProvider.GetTenants())
            {
                foreach (var m in configuration.Modules)
                {
                    if (stop.WaitOne(TimeSpan.Zero))
                    {
                        return;
                    }

                    var indexPath = configuration.GetIndexPath(t.TenantId, m.Name);
                    var indexer = new TextIndexer(indexPath, t, m);
                    try
                    {
                        if (TextIndexAction.Index == (action & TextIndexAction.Index))
                        {
                            var affected = indexer.FindChangedAndIndex();
                            log.DebugFormat("Indexed {0} objects at tenant {1} in module {2}", affected, t, m);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error FindChangedAndIndex in tenant {0}: {1}", t, ex);
                    }

                    try
                    {
                        if (TextIndexAction.Remove == (action & TextIndexAction.Remove))
                        {
                            var affected = indexer.FindRemovedAndIndex();
                            log.DebugFormat("Removed {0} objects at tenant {1} in module {2}", affected, t, m);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Error FindRemovedAndIndex in tenant {0}: {1}", t, ex);
                    }
                }
            }
        }


        [Flags]
        enum TextIndexAction
        {
            None = 0,
            Index = 1,
            Remove = 2,
        }
    }
}
