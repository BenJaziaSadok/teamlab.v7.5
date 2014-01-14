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
using ASC.Api.Interfaces;

namespace ASC.Specific
{
    public class AntiXssParamInspector:IApiParamInspector
    {
        public IEnumerable<object> InspectParams(IEnumerable<object> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is string)
                {
                    var safeString = Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(parameter as string);
                    yield return safeString;
                }
                else
                {
                    yield return parameter;    
                }
            }
        }
    }
}