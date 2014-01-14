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
using System.IO;
using ASC.Xmpp.Core.protocol.extensions.compression;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Compress))]
	class CompressionHandler : XmppStreamHandler
	{
		public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
		{
			context.Sender.SendTo(stream, new Compressed());
			var connection = context.Sender.GetXmppConnection(stream.ConnectionId);
			connection.SetStreamTransformer(new GZipTransformer());
			context.Sender.ResetStream(stream);
		}

		public override void OnRegister(IServiceProvider serviceProvider)
		{
			//ManagedZLib.ManagedZLib.Initialize();
		}

		public override void OnUnregister(IServiceProvider serviceProvider)
		{
			//ManagedZLib.ManagedZLib.Terminate();
		}
	}

	class GZipTransformer : IStreamTransformer
	{
		public Stream TransformInputStream(Stream inputStream)
		{
			//return new DeflateStream(inputStream, CompressionMode.Decompress, true);
			//return new GZipStream(inputStream, CompressionMode.Decompress, true);
			//return new ZipInputStream(inputStream);
			//return new ManagedZLib.CompressionStream(inputStream, ManagedZLib.CompressionOptions.Decompress) {  };
			throw new NotImplementedException();
		}

		public Stream TransformOutputStream(Stream outputStream)
		{
			//return new DeflateStream(outputStream, CompressionMode.Compress, true);
			//return new GZipStream(outputStream, CompressionMode.Compress, true);
			//return new ZipOutputStream(outputStream);
			//return new ManagedZLib.CompressionStream(outputStream, ManagedZLib.CompressionOptions.Compress);
			throw new NotImplementedException();
		}
	}
}
