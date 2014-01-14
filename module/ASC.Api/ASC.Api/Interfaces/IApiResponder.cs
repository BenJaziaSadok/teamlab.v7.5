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
using System.Web;
using System.Web.Routing;
using ASC.Api.Impl;

namespace ASC.Api.Interfaces
{
    public interface IApiResponder
    {
        string Name { get; }
        IEnumerable<string> GetSupportedExtensions();
        bool CanSerializeType(Type type);
        bool CanRespondTo(IApiStandartResponce responce, HttpContextBase context);
        void RespondTo(IApiStandartResponce responce, HttpContextBase context);
    }
}