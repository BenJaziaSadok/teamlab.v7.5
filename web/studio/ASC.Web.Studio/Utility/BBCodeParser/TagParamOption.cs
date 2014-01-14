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
    public class TagParamOption
    {
        public int ParamNumber { get; set; }
        public string DefaultValue { get; set; }
        public string PreValue { get; set; }
        public bool IsUseAnotherParamValue { get; set; }
        public int AnotherParamNumber { get; set; }

        public TagParamOption()
        {
            DefaultValue = "";
            ParamNumber = 0;
            PreValue = "";
            IsUseAnotherParamValue = false;
            AnotherParamNumber = 0;
        }
    }
}