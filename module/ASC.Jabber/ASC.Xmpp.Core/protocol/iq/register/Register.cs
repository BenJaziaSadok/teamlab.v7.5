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

using ASC.Xmpp.Core.protocol.x.data;
using ASC.Xmpp.Core.utils.Xml.Dom;

// Sample 1
// <SENT> <iq id="2" type="set"><query xmlns="jabber:iq:register"><username>gnauck2</username><password>secret</password></query></iq>
// <RECV> <iq id='2' type='result'/>

// Sample 2
// <SEND> <iq xmlns="jabber:client" id="agsXMPP_1" type="get" to="127.0.0.1"><query xmlns="jabber:iq:register"><username>test1</username><password>secret</password></query></iq>
// <RECV> <iq xmlns="jabber:client" id="agsXMPP_1" type="result"><query xmlns="jabber:iq:register"><username>test1</username><password>mysecret</password><password /><instructions>Choose a username and password to register with this server.</instructions><name /><email /><username /></query></iq>

namespace ASC.Xmpp.Core.protocol.iq.register
{
    /// <summary>
    ///   Used for registering new usernames on Jabber/XMPP Servers
    /// </summary>
    public class Register : Element
    {
        #region << Constructors >>

        public Register()
        {
            TagName = "query";
            Namespace = Uri.IQ_REGISTER;
        }

        public Register(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        #endregion

        #region << Properties >>

        public string Username
        {
            get { return GetTag("username"); }
            set { SetTag("username", value); }
        }

        public string Password
        {
            get { return GetTag("password"); }
            set { SetTag("password", value); }
        }

        public string Instructions
        {
            get { return GetTag("instructions"); }
            set { SetTag("instructions", value); }
        }

        public string Name
        {
            get { return GetTag("name"); }
            set { SetTag("name", value); }
        }

        public string Email
        {
            get { return GetTag("email"); }
            set { SetTag("email", value); }
        }

        /// <summary>
        ///   Remove registration from the server
        /// </summary>
        public bool RemoveAccount
        {
            get { return HasTag("remove"); }
            set
            {
                if (value)
                    SetTag("remove");
                else
                    RemoveTag("remove");
            }
        }

        /// <summary>
        ///   The X-Data Element
        /// </summary>
        public Data Data
        {
            get { return SelectSingleElement(typeof (Data)) as Data; }
            set
            {
                if (HasTag(typeof (Data)))
                    RemoveTag(typeof (Data));

                if (value != null)
                    AddChild(value);
            }
        }

        #endregion
    }
}