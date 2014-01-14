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
using System.IO;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Utils;
using Newtonsoft.Json;

namespace ASC.Api.Impl.Serializers
{
    public class JsonNetSerializer : IApiSerializer
    {

        public JsonNetSerializer()
        {
            
        }

        #region IApiSerializer Members

        public IEnumerable<string> GetSupportedExtensions()
        {
            return new[] {".json", ".xml",".tml"};
        }

        public bool CanSerializeType(Type type)
        {
            return true;
        }

        public bool CanRespondTo(IApiStandartResponce responce, string path, string contentType)
        {
            return IsJsonRequest(path, contentType) || IsXmlRequest(path, contentType);
        }

        public ContentType RespondTo(IApiStandartResponce responce, TextWriter output, string path, string contentType,
                                     bool prettify, bool async)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new SerializerContractResolver(responce.Response,responce.ApiContext),
                Converters = new[] { new JsonStringConverter() }
            };

            string responseJson = JsonConvert.SerializeObject(responce, prettify ? Formatting.Indented : Formatting.None,settings);
            if (string.IsNullOrEmpty(responseJson))
                throw new InvalidOperationException("Failed to serialize object");

            ContentType type;
            if (IsJsonRequest(path, contentType))
            {
                //Just write
                type = new ContentType(Constants.JsonContentType) {CharSet = "UTF-8"};
            }
            else
            {
                type = new ContentType(Constants.XmlContentType) {CharSet = "UTF-8"};
                responseJson =
                    JsonConvert.DeserializeXNode(responseJson, "result").ToString(prettify
                                                                                      ? SaveOptions.None
                                                                                      : SaveOptions.DisableFormatting);
            }

            try
            {
                output.Write(responseJson);
            }
            catch (Exception e)
            {
                throw new SerializationException("Failed to write:"+responseJson,e);
            }
            return type;
        }

        #endregion

        private static bool IsJsonRequest(string request, string contentType)
        {
            return StringUtils.GetExtension(request) == ".json" ||
                   StringUtils.IsContentType(Constants.JsonContentType, contentType);
        }

        private static bool IsXmlRequest(string request, string contentType)
        {
            return !IsJsonRequest(request, contentType);
        }
    }
}