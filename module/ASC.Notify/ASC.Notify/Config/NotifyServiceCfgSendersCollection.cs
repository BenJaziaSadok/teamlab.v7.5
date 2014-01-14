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

using System.Configuration;

namespace ASC.Notify.Config
{
    public class NotifyServiceCfgSendersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new NotifyServiceCfgSenderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NotifyServiceCfgSenderElement)element).Name;
        }
    }
}
