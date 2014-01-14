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

using System.Collections.Generic;
using ASC.Collections;
using ASC.Xmpp.Core.protocol.iq.disco;

namespace ASC.Xmpp.Server.Session
{
	public class ClientInfo
	{
		private const string DEFAULT_NODE = "DEFAULT_NODE";

        private IDictionary<string, DiscoInfo> discoCache = new SynchronizedDictionary<string, DiscoInfo>();

		public DiscoInfo GetDiscoInfo(string node)
		{
			if (string.IsNullOrEmpty(node)) node = DEFAULT_NODE;
			return discoCache.ContainsKey(node) ? discoCache[node] : null;
		}

		public void SetDiscoInfo(DiscoInfo discoInfo)
		{
			if (discoInfo == null) return;
			var node = !string.IsNullOrEmpty(discoInfo.Node) ? discoInfo.Node : DEFAULT_NODE;
			discoCache[node] = discoInfo;
		}
	}
}
