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

//-----------------------------------------------------------------------
// <copyright file="GoogleConsumer.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.ApplicationBlock {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Security.Cryptography.X509Certificates;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Xml.Linq;
	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OAuth;
	using DotNetOpenAuth.OAuth.ChannelElements;

	/// <summary>
	/// A consumer capable of communicating with Google Data APIs.
	/// </summary>
	public static class DiggConsumer {
		/// <summary>
		/// The Consumer to use for accessing Google data APIs.
		/// </summary>
		public static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription {
            RequestTokenEndpoint = new MessageReceivingEndpoint("http://services.digg.com/oauth/request_token", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://digg.com/oauth/authorize", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("http://services.digg.com/oauth/access_token", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
			TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
		};

		
        //
		/// <summary>
		/// The URI to get contacts once authorization is granted.
		/// </summary>
        private static readonly MessageReceivingEndpoint DiggEndpoint = new MessageReceivingEndpoint("http://services.digg.com/2.0/story.digg", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest);
        private static readonly MessageReceivingEndpoint GetDiggsEndpoint = new MessageReceivingEndpoint("http://services.digg.com/2.0/story.getDiggs", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest);
        private static readonly MessageReceivingEndpoint FollowUserEndpoint = new MessageReceivingEndpoint("http://services.digg.com/2.0/user.follow", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest);

		/// <summary>
		/// Requests authorization from Google to access data from a set of Google applications.
		/// </summary>
		/// <param name="consumer">The Google consumer previously constructed using <see cref="CreateWebConsumer"/> or <see cref="CreateDesktopConsumer"/>.</param>
		/// <param name="requestedAccessScope">The requested access scope.</param>
		public static void RequestAuthorization(WebConsumer consumer) {
			if (consumer == null) {
				throw new ArgumentNullException("consumer");
			}

			
			Uri callback = Util.GetCallbackUrlFromContext();
			var request = consumer.PrepareRequestUserAuthorization(callback, null, null);
			consumer.Channel.Send(request);
		}

        public static string Digg(ConsumerBase consumer, string accessToken, string id)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var extraData = new Dictionary<string, string>() {
				{ "story_id", id }
			};
            var request = consumer.PrepareAuthorizedRequest(DiggEndpoint, accessToken, extraData);
            var response = consumer.Channel.WebRequestHandler.GetResponse(request);
            string body = response.GetResponseReader().ReadToEnd();
            return body;
        }

        public static string GetDigs(ConsumerBase consumer, string accessToken, string id)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var extraData = new Dictionary<string, string>() {
				{ "story_id", id }
			};
            var request = consumer.PrepareAuthorizedRequest(GetDiggsEndpoint, accessToken, extraData);
            var response = consumer.Channel.WebRequestHandler.GetResponse(request);
            string body = response.GetResponseReader().ReadToEnd();
            return body;
        }

        public static string Follow(ConsumerBase consumer, string accessToken, string username)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            var extraData = new Dictionary<string, string>() {
				{ "username", username }
			};
            var request = consumer.PrepareAuthorizedRequest(FollowUserEndpoint, accessToken, extraData);
            var response = consumer.Channel.WebRequestHandler.GetResponse(request);
            string body = response.GetResponseReader().ReadToEnd();
            return body;
        }

	}
}
