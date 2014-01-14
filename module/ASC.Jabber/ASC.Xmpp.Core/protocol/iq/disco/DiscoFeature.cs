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

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    /// <summary>
    ///   Disco feature
    /// </summary>
    /// <remarks>
    ///   see: http://www.jabber.org/registrar/disco-features.html
    /// </remarks>
    public class DiscoFeature : Element
    {
        #region << Constructors >>

        public DiscoFeature()
        {
            TagName = "feature";
            Namespace = Uri.DISCO_INFO;
        }

        public DiscoFeature(string var) : this()
        {
            Var = var;
        }

        #endregion

        /*
        <iq type='result'
            from='plays.shakespeare.lit'
            to='romeo@montague.net/orchard'
            id='info1'>
        <query xmlns='http://jabber.org/protocol/disco#info'>
            <identity
                category='conference'
                type='text'
                name='Play-Specific Chatrooms'/>
            <identity
                category='directory'
                type='chatroom'
                name='Play-Specific Chatrooms'/>
            <feature var='http://jabber.org/protocol/disco#info'/>
            <feature var='http://jabber.org/protocol/disco#items'/>
            <feature var='http://jabber.org/protocol/muc'/>
            <feature var='jabber:iq:register'/>
            <feature var='jabber:iq:search'/>
            <feature var='jabber:iq:time'/>
            <feature var='jabber:iq:version'/>
        </query>
        </iq>
        */

        /// <summary>
        ///   feature name or namespace
        /// </summary>
        public string Var
        {
            get { return GetAttribute("var"); }
            set { SetAttribute("var", value); }
        }
    }
}