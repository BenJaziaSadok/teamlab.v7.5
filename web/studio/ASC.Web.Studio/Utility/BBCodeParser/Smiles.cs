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

namespace ASC.Web.Studio.Utility.BBCodeParser
{
    public struct Smile
    {
        public string Value;
        public string Img;
        public string Title;
        public string JavascriptValue;

        public Smile(string value, string img)
        {
            Value = value;
            Img = img;
            Title = "";
            JavascriptValue = "";
        }

        public Smile(string value, string img, string title, string javascriptValue)
        {
            Value = value;
            Img = img;
            Title = title;
            JavascriptValue = javascriptValue;
        }
    }
}