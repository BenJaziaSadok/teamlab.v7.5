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
using System.Net.Mime;
using System.Web;
using System.Web.Routing;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Serializers
{
    public class SerializerResponder : IApiResponder
    {
        private readonly ICollection<IApiSerializer> _serializers;

        public SerializerResponder(IUnityContainer container)
        {
            var serializers = container.ResolveAll<IApiSerializer>();
            if (serializers==null)
                throw new ArgumentException("No serializers resolved");

            _serializers = new List<IApiSerializer>(serializers);
            if (!_serializers.Any())
                throw new ArgumentException("No serializers defined");
        }

        public string Name
        {
            get { return "serializer"; }
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return _serializers.SelectMany(x => x.GetSupportedExtensions());
        }

        public bool CanSerializeType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return _serializers.Any(x => x.CanSerializeType(type));
        }

        public bool CanRespondTo(IApiStandartResponce responce, HttpContextBase context)
        {
            if (responce == null) throw new ArgumentNullException("responce");
            if (context == null) throw new ArgumentNullException("context");
            return _serializers.Any(x => x.CanRespondTo(responce, context.Request.Path, context.Request.ContentType));
        }

        public void RespondTo(IApiStandartResponce responce, HttpContextBase httpContext)
        {
            if (responce == null) throw new ArgumentNullException("responce");
            if (httpContext == null) throw new ArgumentNullException("httpContext");

            foreach (var apiSerializer in _serializers)
            {
                var contentType = apiSerializer.RespondTo(responce, httpContext.Response.Output,
                                                                  httpContext.Request.Path, httpContext.Request.ContentType, false, false);
                if (contentType != null)
                {
                    httpContext.Response.ContentType = contentType.ToString();
#if (DEBUG)
                    httpContext.Response.AddHeader("X-Responded", string.Format("{0}", contentType));
#endif
                    return;
                }
            }
#if (DEBUG)
            httpContext.Response.AddHeader("X-Responded", "No");
#endif

        }
    }
}