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

using System.IO;
using System.Reflection;
using System.Resources;

namespace ASC.FederatedLogin.Helpers
{
    public class JsCallbackHelper
    {
        public static string GetCallbackPage()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ASC.FederatedLogin.callback.htm")))
                return reader.ReadToEnd();
        }
    }
}