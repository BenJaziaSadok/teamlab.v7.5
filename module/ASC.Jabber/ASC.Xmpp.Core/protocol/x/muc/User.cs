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

#region using

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for MucUser.
    /// </summary>
    public class User : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public User()
        {
            TagName = "x";
            Namespace = Uri.MUC_USER;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The Decline Element
        /// </summary>
        public Decline Decline
        {
            get { return SelectSingleElement(typeof (Decline)) as Decline; }

            set
            {
                if (HasTag(typeof (Decline)))
                {
                    RemoveTag(typeof (Decline));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   The Invite Element
        /// </summary>
        public Invite Invite
        {
            get { return SelectSingleElement(typeof (Invite)) as Invite; }

            set
            {
                if (HasTag(typeof (Invite)))
                {
                    RemoveTag(typeof (Invite));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        public Item Item
        {
            get { return SelectSingleElement(typeof (Item)) as Item; }

            set
            {
                RemoveTag(typeof (Item));
                AddChild(value);
            }
        }

        /// <summary>
        /// </summary>
        public string Password
        {
            get { return GetTag("password"); }

            set { SetTag("password", value); }
        }

        /// <summary>
        ///   The Status Element
        /// </summary>
        public Status Status
        {
            get { return SelectSingleElement(typeof (Status)) as Status; }

            set
            {
                if (HasTag(typeof (Status)))
                {
                    RemoveTag(typeof (Status));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        #endregion

        /*
        <x xmlns='http://jabber.org/protocol/muc#user'>
             <item affiliation='admin' role='moderator'/>
        </x>
         
        <message from='darkcave@macbeth.shakespeare.lit'
                 to='hag66@shakespeare.lit/pda'
                 type='groupchat'>
            <body>This room is not anonymous.</body>
            <x xmlns='http://jabber.org/protocol/muc#user'>
                <status code='100'/>
            </x>
        </message>
         
        <message
            from='crone1@shakespeare.lit/desktop'
            to='darkcave@macbeth.shakespeare.lit'>
          <x xmlns='http://jabber.org/protocol/muc#user'>
            <invite to='hecate@shakespeare.lit'>
              <reason>
                Hey Hecate, this is the place for all good witches!
              </reason>
            </invite>
          </x>
        </message>
         
        <message
            from='darkcave@macbeth.shakespeare.lit'
            to='hecate@shakespeare.lit'>
          <body>You have been invited to darkcave@macbeth by crone1@shakespeare.lit.</body>
          <x xmlns='http://jabber.org/protocol/muc#user'>
            <invite from='crone1@shakespeare.lit'>
              <reason>
                Hey Hecate, this is the place for all good witches!
              </reason>
            </invite>
            <password>cauldron</password>
          </x>
          <x jid='darkcave@macbeth.shakespeare.lit' xmlns='jabber:x:conference'>
            Hey Hecate, this is the place for all good witches!
          </x>
        </message>
        
        */
    }
}