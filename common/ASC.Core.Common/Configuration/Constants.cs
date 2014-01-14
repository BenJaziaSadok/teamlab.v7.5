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
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Configuration
{
    public sealed class Constants
    {
        public static readonly string NotifyEMailSenderSysName = "email.sender";

        public static readonly string NotifyMessengerSenderSysName = "messanger.sender";

        public static readonly string NotifyPushSenderSysName = "push.sender";

        public static readonly ISystemAccount CoreSystem = new SystemAccount(new Guid("A37EE56E-3302-4a7b-B67E-DDBEA64CD032"), "asc system", true);

        public static readonly ISystemAccount Guest = new SystemAccount(new Guid("712D9EC3-5D2B-4b13-824F-71F00191DCCA"), "guest", false);

        public static readonly IPrincipal Anonymous = new GenericPrincipal(Guest, new[] {Role.Everyone});

        public static readonly ISystemAccount[] SystemAccounts = new[] {CoreSystem, Guest};

        public static readonly int DefaultTrialPeriod = 45;
    }
}