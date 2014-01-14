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
using System.Linq;
using System.Text;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Api.Calendar
{
    public class CalendarAccessRights
    {
        public static readonly Action FullAccessAction = new Action(
                                                        new Guid("{0d68b142-e20a-446e-a832-0d6b0b65a164}"),
                                                        "Full Access", false, false);
       
    }
}
