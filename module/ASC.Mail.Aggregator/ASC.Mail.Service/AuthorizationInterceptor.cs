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
using System.Reflection;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Web;
using ASC.Core;
using Microsoft.ServiceModel.Web;

namespace ASC.Mail.Service
{
	public class Error
	{
		public string Message { get; set; }
	}

	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AuthorizationInterceptor : RequestInterceptor
	{
		public AuthorizationInterceptor()
			: base(false)
		{

		}

		public override void ProcessRequest(ref RequestContext requestContext)
		{
			if (!IsValidUserKey(requestContext))
			{
				GenerateErrorResponse(ref requestContext, HttpStatusCode.Unauthorized, "Unauthorized");
			}
		}

		public bool IsValidUserKey(RequestContext requestContext)
		{
			if (requestContext != null && requestContext.RequestMessage != null)
			{
				var prop = (HttpRequestMessageProperty)requestContext.RequestMessage.Properties[HttpRequestMessageProperty.Name];
				var cookie = prop.Headers[HttpRequestHeader.Cookie];
				if (string.IsNullOrEmpty(cookie)) return false;

				var coockieName = CookiesManager.GetCookiesName();
				foreach (string s in cookie.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (s.IndexOf('=') == -1) continue;

					var key = s.Substring(0, s.IndexOf('=')).Trim();
					if (key.Equals(coockieName, StringComparison.Ordinal))
					{
						try
						{
							var value = s.Substring(s.IndexOf('=') + 1).Trim();
							if (SecurityContext.IsAuthenticated || SecurityContext.AuthenticateMe(value))
							{
								return true;
							}
						}
						catch { }
						break;
					}
				}
			}
			return false;
		}

		private HttpContext TryGetContext(object hosting)
		{
			HttpContext context = HttpContext.Current;
			if (context == null && hosting != null)
			{
				try
				{
					//Get context through reflections
					var currentThreadDataField = hosting.GetType().GetField("currentThreadData", BindingFlags.Instance | BindingFlags.NonPublic);
					if (currentThreadDataField != null)
					{
						var currentThreadData = currentThreadDataField.GetValue(hosting);
						if (currentThreadData != null)
						{
							var currentHttpContextDataField = currentThreadData.GetType().GetField("httpContext", BindingFlags.Instance | BindingFlags.NonPublic);
							if (currentHttpContextDataField != null)
							{
								var currentContext = currentHttpContextDataField.GetValue(currentThreadData);
								if (currentContext is HttpContext)
								{
									context = (HttpContext)currentContext;
								}
							}
						}
					}
				}
				catch
				{

				}
			}
			return context;
		}

		public void GenerateErrorResponse(ref RequestContext requestContext, HttpStatusCode statusCode, string errorMessage)
		{
			// The error message is padded so that IE shows the response by default
			var reply = Message.CreateMessage(MessageVersion.None, null, new Error() { Message = errorMessage });
			var responseProp = new HttpResponseMessageProperty()
			{
				StatusCode = statusCode
			};
			reply.Properties[HttpResponseMessageProperty.Name] = responseProp;
			requestContext.Reply(reply);
			requestContext = null;
		}
	}
}