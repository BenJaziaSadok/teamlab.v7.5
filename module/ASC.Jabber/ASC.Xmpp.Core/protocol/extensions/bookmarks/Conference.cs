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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.bookmarks
{
    /// <summary>
    ///   One of the most common uses of bookmarks will likely be to bookmark conference rooms on various Jabber servers
    /// </summary>
    public class Conference : Element
    {
        /*
             <iq type='result' id='2'>
               <query xmlns='jabber:iq:private'>
                 <storage xmlns='storage:bookmarks'>
                   <conference name='Council of Oberon' 
                               autojoin='true'
                               jid='council@conference.underhill.org'>
                     <nick>Puck</nick>
                     <password>titania</password>
                   </conference>
                 </storage>
               </query>
             </iq>   
         */

        public Conference()
        {
            TagName = "conference";
            Namespace = Uri.STORAGE_BOOKMARKS;
        }

        public Conference(Jid jid, string name) : this()
        {
            Jid = jid;
            Name = name;
        }

        public Conference(Jid jid, string name, string nickname) : this(jid, name)
        {
            Nickname = nickname;
        }

        public Conference(Jid jid, string name, string nickname, string password) : this(jid, name, nickname)
        {
            Password = password;
        }

        public Conference(Jid jid, string name, string nickname, string password, bool autojoin)
            : this(jid, name, nickname, password)
        {
            AutoJoin = autojoin;
        }

        /// <summary>
        ///   A name/description for this bookmarked room
        /// </summary>
        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        /// <summary>
        ///   Should the client join this room automatically after successfuil login?
        /// </summary>
        public bool AutoJoin
        {
            get { return GetAttributeBool("autojoin"); }
            set { SetAttribute("autojoin", value); }
        }

        /// <summary>
        ///   The Jid of the bookmarked room
        /// </summary>
        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }
            set { SetAttribute("jid", value); }
        }

        /// <summary>
        ///   The Nickname for this room
        /// </summary>
        public string Nickname
        {
            get { return GetTag("nickname"); }
            set { SetTag("nickname", value); }
        }

        /// <summary>
        ///   The password for password protected rooms
        /// </summary>
        public string Password
        {
            get { return GetTag("password"); }
            set { SetTag("password", value); }
        }
    }
}