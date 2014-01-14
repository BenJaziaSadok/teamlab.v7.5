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

using ASC.Web.Files.Classes;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace ASC.Web.Files.Services.WCFService
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    internal class ServiceErrorLoggingBehaviorAttribute : Attribute, IServiceBehavior, IErrorHandler
    {
        private static readonly ILog logger = Global.Logger;


        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        public bool HandleError(Exception error)
        {
            if (error is WebFaultException<Service.FileError>)
            {
                var fault = (WebFaultException<Service.FileError>)error;
                if (logger.IsDebugEnabled && fault.Detail != null)
                {
                    logger.DebugFormat("File service error: {0}: {1}\r\n{2}", fault.Detail.Inner.Type, fault.Detail.Inner.Message, fault.Detail.Inner.Stack);
                }
            }
            else
            {
                logger.Error(error);
            }
            return true;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher d in serviceHostBase.ChannelDispatchers)
            {
                d.ErrorHandlers.Add(this);
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}