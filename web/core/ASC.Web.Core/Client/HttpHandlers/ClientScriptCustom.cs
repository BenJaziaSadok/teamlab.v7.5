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

using ASC.Core;
using System.Threading;

namespace ASC.Web.Core.Client.HttpHandlers
{
    public abstract class ClientScriptCustom : ClientScript
    {
        protected internal sealed override string GetCacheHash()
        {
            return ClientSettings.ResetCacheKey +
                   Thread.CurrentThread.CurrentCulture.Name + 
                   CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}
