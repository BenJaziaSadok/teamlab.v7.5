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
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web;

namespace ASC.Web.Files.Services.WCFService
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAuthenticationBehavior : Attribute, IServiceBehavior
    {
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var policies = new List<IAuthorizationPolicy>();
            policies.Add(new AuthorizationPolicy());

            serviceHostBase.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
            var bh = serviceDescription.Behaviors.Find<ServiceAuthorizationBehavior>();
            if (bh != null)
            {
                bh.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }


        class AuthorizationPolicy : IAuthorizationPolicy
        {
            private readonly string id = Guid.NewGuid().ToString();

            public string Id
            {
                get { return id; }
            }

            public ClaimSet Issuer
            {
                get { return ClaimSet.System; }
            }

            public bool Evaluate(EvaluationContext evaluationContext, ref object state)
            {
                evaluationContext.Properties["Principal"] = HttpContext.Current.User;
                return true;
            }
        }
    }
}
