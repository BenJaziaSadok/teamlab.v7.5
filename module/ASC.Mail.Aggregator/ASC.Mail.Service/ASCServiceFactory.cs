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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.ServiceModel.Web;
using System.Xml;

namespace ASC.Mail.Service
{
    public class MailServiceHost : WebServiceHost2
    {
        public MailServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, false, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            var readerQuotas = new XmlDictionaryReaderQuotas();

            readerQuotas.MaxStringContentLength = int.MaxValue;

            base.OnOpening();

            // change readerQuotas

            foreach (var ep in this.Description.Endpoints)
            {
                var binding = new CustomBinding(ep.Binding);

                readerQuotas.CopyTo(binding.Elements.Find<WebMessageEncodingBindingElement>().ReaderQuotas);

                ep.Binding = binding;
            }
        }
    }

    class ASCServiceHostFactory : WebServiceHost2Factory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var service = new MailServiceHost(serviceType, baseAddresses);
            //service.Interceptors.Add(new AuthorizationInterceptor());
            service.Interceptors.Add(new FormatterInterceptor());
            service.MaxMessageSize = 25 * 1024 * 1024;
            return service;
        }
    }
}
