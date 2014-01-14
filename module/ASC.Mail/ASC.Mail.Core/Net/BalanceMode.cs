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

namespace ASC.Mail.Net
{
    /// <summary>
    /// This enum specified balance mode.
    /// </summary>
    public enum BalanceMode
    {
        /// <summary>
        /// Operation is load balanched by all workers.
        /// </summary>
        LoadBalance,

        /// <summary>
        /// Operation will be handed over to next worker, if last one fails.
        /// </summary>
        FailOver,
    }
}