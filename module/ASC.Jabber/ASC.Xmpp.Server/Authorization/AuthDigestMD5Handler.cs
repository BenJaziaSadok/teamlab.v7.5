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
using ASC.Collections;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.authorization.DigestMD5;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;
using log4net;

namespace ASC.Xmpp.Server.Authorization
{
    [XmppHandler(typeof(Auth))]
    [XmppHandler(typeof(Response))]
    [XmppHandler(typeof(Abort))]
    class AuthDigestMD5Handler : XmppStreamHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthDigestMD5Handler));
        private IDictionary<string, AuthData> authData = new Dictionary<string, AuthData>();


        public override void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context)
        {
            lock (authData)
            {
                authData.Remove(stream.Id);
            }
        }

        public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
        {
            if (stream.Authenticated) return;

            if (element is Auth) ProcessAuth(stream, (Auth)element, context);
            if (element is Response) ProcessResponse(stream, (Response)element, context);
            if (element is Abort) ProcessAbort(stream, (Abort)element, context);
        }

        private void ProcessAuth(XmppStream stream, Auth auth, XmppHandlerContext context)
        {
            AuthData authStep;
            lock (authData)
            {
                authData.TryGetValue(stream.Id, out authStep);
            }

            if (auth.MechanismType != MechanismType.DIGEST_MD5)
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.InvalidMechanism);
            }
            else if (authStep != null)
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
            }
            else
            {
                lock (authData)
                {
                    authData[stream.Id] = new AuthData();
                }
                var challenge = GetChallenge(stream.Domain);
                context.Sender.SendTo(stream, challenge);
            }
        }

        private void ProcessResponse(XmppStream stream, Response response, XmppHandlerContext context)
        {
            AuthData authStep;
            lock (authData)
            {
                authData.TryGetValue(stream.Id, out authStep);
            }

            if (authStep == null)
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
                return;
            }

            if (authStep.Step == AuthStep.Step1)
            {
                var challenge = ProcessStep1(stream, response, context);
                if (challenge != null)
                {
                    context.Sender.SendTo(stream, challenge);
                    authStep.DoStep();
                }
                else
                {
                    context.Sender.SendToAndClose(stream, XmppFailureError.NotAuthorized);
                }
            }
            else if (authStep.Step == AuthStep.Step2)
            {
                var success = ProcessStep2(stream, response, context);
                context.Sender.SendTo(stream, success);
            }
            else
            {
                context.Sender.SendToAndClose(stream, XmppFailureError.TemporaryAuthFailure);
            }
        }

        private void ProcessAbort(XmppStream stream, Abort abort, XmppHandlerContext context)
        {
            context.Sender.SendToAndClose(stream, XmppFailureError.Aborted);
        }

        private Challenge GetChallenge(string domain)
        {
            var challenge = new Challenge();
            challenge.TextBase64 = string.Format("realm=\"{0}\",nonce=\"{1}\",qop=\"auth\",charset=utf-8,algorithm=md5-sess", domain, UniqueId.CreateNewId());
            return challenge;
        }

        private Challenge ProcessStep1(XmppStream stream, Response response, XmppHandlerContext ctx)
        {
            var step = new Step2(response.TextBase64);
            var userName = step.Username;
            var user = ctx.UserManager.GetUser(new Jid(userName, stream.Domain, null));

            log.DebugFormat("User {0} {1}. Realm={2}", userName, user == null ? "not found" : user.ToString(), step.Realm);

            if (user != null && string.Compare(stream.Domain, step.Realm, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (step.Authorize(userName, user.Password))
                {
                    log.DebugFormat("User authorized");
                    lock (authData)
                    {
                        authData[stream.Id].UserName = userName;
                    }
                    var challenge = new Challenge();
                    challenge.TextBase64 = string.Format("rspauth={0}", step.CalculateResponse(userName, user.Password, string.Empty));
                    return challenge;
                }
                else
                {
                    log.DebugFormat("User not authorized");
                }
            }
            return null;
        }

        private Success ProcessStep2(XmppStream stream, Response response, XmppHandlerContext ctx)
        {
            lock (authData)
            {
                stream.Authenticate(authData[stream.Id].UserName);
                authData.Remove(stream.Id);
            }
            ctx.Sender.ResetStream(stream);
            return new Success();
        }

        private enum AuthStep
        {
            Step1,
            Step2,
        }

        private class AuthData
        {
            public string UserName
            {
                get;
                set;
            }

            public AuthStep Step
            {
                get;
                private set;
            }

            public void DoStep()
            {
                Step++;
            }
        }
    }
}