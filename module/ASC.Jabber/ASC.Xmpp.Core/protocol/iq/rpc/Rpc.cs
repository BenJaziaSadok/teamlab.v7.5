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

namespace ASC.Xmpp.Core.protocol.iq.rpc
{
    /*         

        Example 1. A typical request

        <iq type='set' to='responder@company-a.com/jrpc-server' id='1'>
          <query xmlns='jabber:iq:rpc'>
            <methodCall>
              <methodName>examples.getStateName</methodName>
              <params>
                <param>
                  <value><i4>6</i4></value>
                </param>
              </params>
            </methodCall>
          </query>
        </iq>

        Example 2. A typical response

        <iq type='result' to='requester@company-b.com/jrpc-client' 
                    from='responder@company-a.com/jrpc-server' id='1'>
          <query xmlns='jabber:iq:rpc'>
            <methodResponse>
              <params>
                <param>
                  <value><string>Colorado</string></value>
                </param>
              </params>
            </methodResponse>
          </query>
        </iq>

    */

    /// <summary>
    ///   JEP-0009: Jabber-RPC, transport RPC over Jabber/XMPP
    /// </summary>
    public class Rpc : Element
    {
        public Rpc()
        {
            TagName = "query";
            Namespace = Uri.IQ_RPC;
        }


        /// <summary>
        /// </summary>
        public MethodCall MethodCall
        {
            get { return (MethodCall) SelectSingleElement(typeof (MethodCall)); }
            set
            {
                RemoveTag(typeof (MethodCall));
                if (value != null)
                    AddChild(value);
            }
        }

        /// <summary>
        /// </summary>
        public MethodResponse MethodResponse
        {
            get { return (MethodResponse) SelectSingleElement(typeof (MethodResponse)); }
            set
            {
                RemoveTag(typeof (MethodResponse));
                if (value != null)
                    AddChild(value);
            }
        }
    }
}