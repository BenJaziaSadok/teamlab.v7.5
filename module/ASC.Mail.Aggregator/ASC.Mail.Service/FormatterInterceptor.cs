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
using System.Net;
using System.ServiceModel.Channels;
using Microsoft.ServiceModel.Web;

namespace ASC.Mail.Service
{
	public class FormatterInterceptor : RequestInterceptor
	{
		public FormatterInterceptor() : base(true) { }

		public override void ProcessRequest(ref RequestContext requestContext)
		{
			if (requestContext == null) return;

			var request = requestContext.RequestMessage;
			if (request == null) return;

			var prop = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
			string format = null;
			string accepts = prop.Headers[HttpRequestHeader.Accept];
			if (accepts != null)
			{
				if (accepts.Contains("text/xml") || accepts.Contains("application/xml"))
				{
					format = "xml";
				}
				else if (accepts.Contains("application/json"))
				{
					format = "json";
				}
			}
			else
			{
				string contentType = prop.Headers[HttpRequestHeader.ContentType];
				if (contentType != null)
				{
					if (contentType.Contains("text/xml") || contentType.Contains("application/xml"))
					{
						format = "xml";
					}
					else if (contentType.Contains("application/json"))
					{
						format = "json";
					}
				}
			}
			if (format != null)
			{
				var toBuilder = new UriBuilder(request.Headers.To);
				if (string.IsNullOrEmpty(toBuilder.Query))
				{
					toBuilder.Query = "format=" + format;
				}
				else if (!toBuilder.Query.Contains("format="))
				{
					toBuilder.Query += "&format=" + format;
				}
				request.Headers.To = toBuilder.Uri;
			}
		}
	}
}