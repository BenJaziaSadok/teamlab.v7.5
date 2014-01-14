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

using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.auth
{
    //	Send:<iq type='get' to='myjabber.net' id='MX_7'>
    //			<query xmlns='jabber:iq:auth'><username>gnauck</username></query>
    //		 </iq>
    //	Recv:<iq type="result" id="MX_7"><query xmlns="jabber:iq:auth"><username>gnauck</username><password/><digest/><resource/></query></iq> 
    //
    //	Send:<iq type='set' id='mx_login'><query xmlns='jabber:iq:auth'><username>gnauck</username><digest>27c05d464e3f908db3b2ca1729674bfddb28daf2</digest><resource>Office</resource></query></iq>
    //	Recv:<iq id="mx_login" type="result"/> 


    ///<summary>
    ///</summary>
    public class Auth : Element
    {
        #region << Constructors >>

        public Auth()
        {
            TagName = "query";
            Namespace = Uri.IQ_AUTH;
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

        public string Resource
        {
            get { return GetTag("resource"); }
            set { SetTag("resource", value); }
        }

        public string Digest
        {
            get { return GetTag("digest"); }
            set { SetTag("digest", value); }
        }

        #endregion

        #region << Public Methods >>

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="password"> </param>
        /// <param name="StreamID"> </param>
        public void SetAuthDigest(string username, string password, string StreamID)
        {
            // Jive Messenger has a problem when we dont remove the password Tag
            RemoveTag("password");
            Username = username;
            Digest = Hash.Sha1Hash(StreamID + password);
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="password"> </param>
        public void SetAuthPlain(string username, string password)
        {
            // remove digest Tag when existing
            RemoveTag("digest");
            Username = username;
            Password = password;
        }

        /// <summary>
        /// </summary>
        public void SetAuth(string username, string password, string streamId)
        {
            if (HasTag("digest"))
                SetAuthDigest(username, password, streamId);
            else
                SetAuthPlain(username, password);
        }

        #endregion
    }
}