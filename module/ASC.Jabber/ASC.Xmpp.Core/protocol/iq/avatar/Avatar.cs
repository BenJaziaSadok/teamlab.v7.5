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

namespace ASC.Xmpp.Core.protocol.iq.avatar
{
    //	<iq id='2' type='result' to='user@server/resource'>
    //		<query xmlns='jabber:iq:avatar'>
    //			<data mimetype='image/jpeg'>
    //			Base64-Encoded Data
    //			</data>
    //		</query>
    //	</iq>

    /// <summary>
    ///   Summary description for Avatar.
    /// </summary>
    public class Avatar : Base.Avatar
    {
        public Avatar()
        {
            Namespace = Uri.IQ_AVATAR;
        }
    }
}