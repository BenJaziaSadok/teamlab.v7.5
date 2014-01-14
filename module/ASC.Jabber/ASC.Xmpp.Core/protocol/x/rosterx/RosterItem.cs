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

namespace ASC.Xmpp.Core.protocol.x.rosterx
{
    /// <summary>
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// </summary>
        NONE = -1,

        /// <summary>
        /// </summary>
        add,

        /// <summary>
        /// </summary>
        remove,

        /// <summary>
        /// </summary>
        modify
    }

    /// <summary>
    ///   Summary description for RosterItem.
    /// </summary>
    public class RosterItem : Base.RosterItem
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public RosterItem()
        {
            Namespace = Uri.X_ROSTERX;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        public RosterItem(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        public RosterItem(Jid jid, string name) : this(jid)
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        /// <param name="jid"> </param>
        /// <param name="name"> </param>
        /// <param name="action"> </param>
        public RosterItem(Jid jid, string name, Action action) : this(jid, name)
        {
            Action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Action Action
        {
            get { return (Action) GetAttributeEnum("action", typeof (Action)); }

            set { SetAttribute("action", value.ToString()); }
        }

        #endregion

        /*
		<item action='delete' jid='rosencrantz@denmark' name='Rosencrantz'>   
			<group>Visitors</group>   
		</item> 
		*/
    }
}