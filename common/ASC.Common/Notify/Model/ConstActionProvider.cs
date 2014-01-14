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

using System.Collections.Generic;
using System.Linq;

namespace ASC.Notify.Model
{
    public sealed class ConstActionProvider : IActionProvider
    {
        private readonly Dictionary<string, INotifyAction> actions;

        
        public ConstActionProvider(params INotifyAction[] actions)
        {
            this.actions = actions.ToDictionary(a => a.ID);
        }

        public INotifyAction[] GetActions()
        {
            return actions.Values.ToArray();
        }

        public INotifyAction GetAction(string id)
        {
            INotifyAction action;
            actions.TryGetValue(id, out action);
            return action;
        }
    }
}
