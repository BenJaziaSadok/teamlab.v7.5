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
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Specific.SerializationFilters
{
    public sealed class CustomSerializer : ApiCallFilter
    {
        public Type SerializerType { get; set; } 

        public CustomSerializer(Type serializerType)
        {
            SerializerType = serializerType;
            Responder = ServiceLocator.Current.GetInstance(SerializerType) as IApiResponder;
        }

        private IApiResponder Responder { get; set; }

        public override void PostMethodCall(IApiMethodCall method, Api.Impl.ApiContext context, object methodResponce)
        {
            method.Responders.Add(Responder); //Resolve type
            //Add serializers to
            base.PostMethodCall(method, context, methodResponce);
        }
    }
}