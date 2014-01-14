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

using System.Threading;

namespace ASC.Xmpp.Server.Statistics
{
	public static class NetStatistics
	{
		private static long readBytes;

		private static long writeBytes;


		public static bool Enabled
		{
			get;
			set;
		}

		public static void ReadBytes(int bytes)
		{
			if (!Enabled) return;
			Interlocked.Add(ref readBytes, bytes);
		}

		public static void WriteBytes(int bytes)
		{
			if (!Enabled) return;
			Interlocked.Add(ref writeBytes, bytes);
		}

		public static void Reset()
		{
			Interlocked.Exchange(ref readBytes, 0);
			Interlocked.Exchange(ref writeBytes, 0);
		}

		public static long GetReadBytes()
		{
			return readBytes;
		}

		public static long GetWriteBytes()
		{
			return writeBytes;
		}
	}
}
