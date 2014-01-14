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
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using System.Web.Hosting;
using ASC.Core.Common.Notify.Push;
using ASC.Core.Configuration;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;
using log4net;

namespace ASC.Core.Common.Notify
{
    internal class PushSenderSink : Sink
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(PushSenderSink));
        private bool? _isServiceConfigured;

        private bool IsServiceConfigured
        {
            get
            {
                if (!_isServiceConfigured.HasValue)
                {
                    try
                    {
                        var webConfig = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
                        var serviceModelConfig = ServiceModelSectionGroup.GetSectionGroup(webConfig);
                        if (serviceModelConfig != null)
                        {
                            var clientConfig = serviceModelConfig.Sections["client"] as ClientSection;
                            if (clientConfig != null)
                            {
                                var pushServiceContract = typeof(IPushService).FullName;
                                var endpointConfig = clientConfig.Endpoints.Cast<ChannelEndpointElement>()
                                                                 .FirstOrDefault(enpoint => enpoint.Contract == pushServiceContract);

                                _isServiceConfigured = endpointConfig != null;
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _log.Error("couldn't check if push service endpoint is configured", error);
                    }
                    if (!_isServiceConfigured.HasValue)
                    {
                        _isServiceConfigured = false;
                    }
                }
                return _isServiceConfigured.Value;
            }
        }

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            try
            {
                var notification = new PushNotification
                    {
                        Module = GetTagValue<PushModule>(message, PushConstants.PushModuleTagName),
                        Action = GetTagValue<PushAction>(message, PushConstants.PushActionTagName),
                        Item = GetTagValue<PushItem>(message, PushConstants.PushItemTagName),
                        ParentItem = GetTagValue<PushItem>(message, PushConstants.PushParentItemTagName),
                        Message = message.Body,
                        ShortMessage = message.Subject
                    };

                if (IsServiceConfigured)
                {
                    using (var pushClient = new PushServiceClient())
                    {
                        pushClient.EnqueueNotification(
                            CoreContext.TenantManager.GetCurrentTenant().TenantId,
                            message.Recipient.ID,
                            notification,
                            new List<string>());
                    }
                }
                else
                {
                    _log.Debug("push sender endpoint is not configured!");
                }

                return new SendResponse(message, Constants.NotifyPushSenderSysName, SendResult.OK);
            }
            catch (Exception error)
            {
                return new SendResponse(message, Constants.NotifyPushSenderSysName, error);
            }
        }

        private T GetTagValue<T>(INoticeMessage message, string tagName)
        {
            var tag = message.Arguments.FirstOrDefault(arg => arg.Tag == tagName);
            return tag != null ? (T)tag.Value : default(T);
        }
    }
}
