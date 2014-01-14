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

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ASC.Web.Host;
using SmartAssembly.Attributes;
using System.Reflection;

#endregion

#if (PERFPROFILE)
	[Obfuscation(Exclude = true, ApplyToMembers = true)]
	[DoNotObfuscateType]
public class PerfCounter : MarshalByRefObject
{
    private readonly Dictionary<string, Stopwatch> watches = new Dictionary<string, Stopwatch>();

    private readonly Dictionary<string, long> counters = new Dictionary<string, long>();

    private const int watchResultCount = 10000;
    private const int flushwatchResultCount = 1000;

    private readonly Dictionary<string, Queue<TimeSpan>> flushwatchesResults =
        new Dictionary<string, Queue<TimeSpan>>();

    private readonly Dictionary<string, Queue<TimeSpan>> watchesResults =
        new Dictionary<string, Queue<TimeSpan>>();

    private readonly object SynchWatch = new object();
    private readonly object SynchCounter = new object();

    public event EventHandler UpdateCallback = null;

    private static PerfCounter _instance;

    public static PerfCounter Instnace
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PerfCounter();
            }
            return _instance;
        }
        set { _instance = value; }
    }

    public static void Inc(string name, long count)
    {
        Instnace.IncCount(name, count);
    }

    public static void Start(string name)
    {
        Instnace.StartWatch(name);
    }

    public static void Stop(string name)
    {
        Instnace.StopWatch(name);
    }

    private PerfCounter()
    {
        new Timer(OnTimer, null, 0, 300);
    }

    private void OnTimer(object state)
    {
        if (UpdateCallback != null)
        {
            UpdateCallback(null, null);
        }
    }

    internal void StartWatch(string counterName)
    {
        lock (SynchWatch)
        {
            Stopwatch sw = EnsureWatchExists(counterName);
            sw.Reset();
            sw.Start();
        }
    }

    internal TimeSpan CheckpointWatch(string counterName)
    {
        lock (SynchWatch)
        {
            return EnsureWatchExists(counterName).Elapsed;
        }
    }

    internal TimeSpan StopWatch(string counterName)
    {
        lock (SynchWatch)
        {
            Stopwatch swOperation = new Stopwatch();
            swOperation.Start();
            Stopwatch sw = EnsureWatchExists(counterName);
            swOperation.Stop();
            sw.Stop();
            TimeSpan actualElapsed = sw.Elapsed - swOperation.Elapsed;
            if (watchesResults[counterName].Count == watchResultCount)
            {
                //Deque first
                watchesResults[counterName].Dequeue();
            }
            if (flushwatchesResults[counterName].Count == flushwatchResultCount)
            {
                //Deque first
                flushwatchesResults[counterName].Dequeue();
            }
            watchesResults[counterName].Enqueue(actualElapsed);
            flushwatchesResults[counterName].Enqueue(actualElapsed);
            return actualElapsed;
        }
    }

    public string[] GetWatches()
    {
        lock (SynchWatch)
        {
            string[] keys = new string[watches.Keys.Count];
            watches.Keys.CopyTo(keys, 0);
            return keys;
        }
    }

    public string[] GetCounters()
    {
        lock (SynchWatch)
        {
            string[] keys = new string[counters.Keys.Count];
            counters.Keys.CopyTo(keys, 0);
            return keys;
        }
    }

    public TimeSpan[] GetSeriesAndFlush(string name)
    {
        lock (SynchWatch)
        {
            EnsureWatchExists(name);
            TimeSpan[] times = flushwatchesResults[name].ToArray();
            flushwatchesResults[name].Clear();
            return times;
        }
    }

    public TimeSpan[] GetSeries(string name)
    {
        lock (SynchWatch)
        {
            EnsureWatchExists(name);
            return watchesResults[name].ToArray();
        }
    }

    public TimeSpan GetMax(string name)
    {
        lock (SynchWatch)
        {
            EnsureWatchExists(name);
            long maxTicks = 0;
            foreach (var time in watchesResults[name])
            {
                if (time.Ticks > maxTicks)
                {
                    maxTicks = time.Ticks;
                }
            }
            return TimeSpan.FromTicks(maxTicks);
        }
    }

    public TimeSpan GetMin(string name)
    {
        lock (SynchWatch)
        {
            EnsureWatchExists(name);
            long minTicks = TimeSpan.MaxValue.Ticks;
            foreach (var time in watchesResults[name])
            {
                if (time.Ticks < minTicks)
                {
                    minTicks = time.Ticks;
                }
            }
            return TimeSpan.FromTicks(minTicks);
        }
    }

    public TimeSpan GetAvg(string name)
    {
        lock (SynchWatch)
        {
            EnsureWatchExists(name);
            if (watchesResults[name].Count > 0)
            {
                long avgTicks = 0;
                foreach (var time in watchesResults[name])
                {
                    avgTicks += time.Ticks;
                }
                return TimeSpan.FromTicks(avgTicks/watchesResults[name].Count);
            }
            return TimeSpan.FromMilliseconds(0);
        }
    }

    public TimeSpan GetWatch(string name)
    {
        lock (SynchWatch)
        {
            return EnsureWatchExists(name).Elapsed;
        }
    }

    private Stopwatch EnsureWatchExists(string counterName)
    {
        Stopwatch sw;
        object dataSlotValue =
            Thread.GetData(Thread.GetNamedDataSlot(string.Format(CountersResource.Watches, counterName)));
        if (dataSlotValue == null)
        {
            sw = new Stopwatch();
            Thread.SetData(Thread.GetNamedDataSlot(string.Format(CountersResource.Watches, counterName)), sw);
            if (!watches.ContainsKey(counterName))
            {
                watches.Add(counterName, sw);
            }
            if (!watchesResults.ContainsKey(counterName))
            {
                watchesResults.Add(counterName, new Queue<TimeSpan>(watchResultCount));
            }
            if (!flushwatchesResults.ContainsKey(counterName))
            {
                flushwatchesResults.Add(counterName, new Queue<TimeSpan>(flushwatchResultCount));
            }
        }
        else
        {
            sw = (Stopwatch) dataSlotValue;
        }
        return sw;
    }

    internal void IncCount(string name, long count)
    {
        lock (SynchCounter)
        {
            long value = EnsureCounterExists(name);
            counters[name] = value + count;
        }
    }

    internal static void Dec(string name, long count)
    {
        Instnace.DecCount(name, count);
    }

    internal void DecCount(string name, long count)
    {
        lock (SynchCounter)
        {
            long value = EnsureCounterExists(name);
            counters[name] = value - count;
        }
    }

    public long GetCounter(string name)
    {
        lock (SynchCounter)
        {
            return EnsureCounterExists(name);
        }
    }

    private long EnsureCounterExists(string counterName)
    {
        long counter;
        if (!counters.TryGetValue(counterName, out counter))
        {
            counters.Add(counterName, 0);
        }
        return counter;
    }

    public override object InitializeLifetimeService()
    {
        return null;
    }
}
#else
[Obfuscation(Exclude = true, ApplyToMembers = true)]
[DoNotObfuscateType]
public class PerfCounter : MarshalByRefObject
{
    public static PerfCounter Instnace
    {
        get
        {
            return null;
        }
        set
        {
            
        }
    }

    public static void Inc(string name, long count)
    {
        
    }

    public TimeSpan[] GetSeriesAndFlush(string name)
    {
        return null;
    }

    public static void Start(string name)
    {
        
    }
    public override object InitializeLifetimeService()
    {
        return null;

    }

    public static void Stop(string name)
    {
        
    }
    internal static void Dec(string name, long count)
    {
   
    }

    public string[] GetWatches()
    {
        return null;
    }

    public string[] GetCounters()
    {
        return null;
    }

    public TimeSpan[] GetSeries(string name)
    {
        return null;
    }

    public TimeSpan GetMax(string name)
    {
        return TimeSpan.MinValue;
    }

    public TimeSpan GetMin(string name)
    {
        return TimeSpan.MinValue;
    }

    public TimeSpan GetAvg(string name)
    {
        return TimeSpan.MinValue;
    }

    public TimeSpan GetWatch(string name)
    {
        return TimeSpan.MinValue;
    }
    public long GetCounter(string name)
    {
        return 0;
    }

}
#endif