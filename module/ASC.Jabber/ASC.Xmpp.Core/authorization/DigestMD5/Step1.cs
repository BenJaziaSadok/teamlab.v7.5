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

using System;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.authorization.DigestMD5
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Step1.
    /// </summary>
    public class Step1 : DigestMD5Mechanism
    {
        #region Members

        /// <summary>
        /// </summary>
        private string m_Algorithm;

        /// <summary>
        /// </summary>
        private string m_Charset = "utf-8";

        /// <summary>
        /// </summary>
        private string m_Message;

        /// <summary>
        /// </summary>
        private string m_Nonce;

        /// <summary>
        /// </summary>
        private string m_Qop; // 			= "auth";		

        /// <summary>
        /// </summary>
        private string m_Realm;

        /// <summary>
        /// </summary>
        private string m_Rspauth;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public Step1()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"> </param>
        public Step1(string message)
        {
            m_Message = message;
            Parse(message);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Algorithm
        {
            get { return m_Algorithm; }

            set { m_Algorithm = value; }
        }

        /// <summary>
        /// </summary>
        public string Charset
        {
            get { return m_Charset; }

            set { m_Charset = value; }
        }

        public Encoding Encoding
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(Charset);
                }
                catch (Exception)
                {
                    return Encoding.UTF8;
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Nonce
        {
            get { return m_Nonce; }

            set { m_Nonce = value; }
        }

        /// <summary>
        /// </summary>
        public string Qop
        {
            get { return m_Qop; }

            set { m_Qop = value; }
        }

        /// <summary>
        /// </summary>
        public string Realm
        {
            get { return m_Realm; }

            set { m_Realm = value; }
        }

        /// <summary>
        /// </summary>
        public string Rspauth
        {
            get { return m_Rspauth; }

            set { m_Rspauth = value; }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <param name="message"> </param>
        /// <exception cref="ChallengeParseException"></exception>
        private void Parse(string message)
        {
            try
            {
                int start = 0;
                int end = 0;
                while (start < message.Length)
                {
                    int equalPos = message.IndexOf('=', start);
                    if (equalPos > 0)
                    {
                        // look if the next char is a quote
                        if (message.Substring(equalPos + 1, 1) == "\"")
                        {
                            // quoted value, find the end now
                            end = message.IndexOf('"', equalPos + 2);
                            ParsePair(message.Substring(start, end - start + 1));
                            start = end + 2;
                        }
                        else
                        {
                            // value is not quoted, ends at the next comma or end of string   
                            end = message.IndexOf(',', equalPos + 1);
                            if (end == -1)
                            {
                                end = message.Length;
                            }

                            ParsePair(message.Substring(start, end - start));

                            start = end + 1;
                        }
                    }
                }
            }
            catch
            {
                throw new ChallengeParseException("Unable to parse challenge");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="pair"> </param>
        private void ParsePair(string pair)
        {
            int equalPos = pair.IndexOf("=");
            if (equalPos > 0)
            {
                string key = pair.Substring(0, equalPos);
                string data;

                // is the value quoted?
                if (pair.Substring(equalPos + 1, 1) == "\"")
                {
                    data = pair.Substring(equalPos + 2, pair.Length - equalPos - 3);
                }
                else
                {
                    data = pair.Substring(equalPos + 1);
                }

                switch (key)
                {
                    case "realm":
                        m_Realm = data;
                        break;
                    case "nonce":
                        m_Nonce = data;
                        break;
                    case "qop":
                        m_Qop = data;
                        break;
                    case "charset":
                        m_Charset = data;
                        break;
                    case "algorithm":
                        m_Algorithm = data;
                        break;
                    case "rspauth":
                        m_Rspauth = data;
                        break;
                }
            }
        }

        #endregion

        // Mechanism 

        /*
            nonce="deqOGux/N6hDPtf9vkGMU5Vzae+zfrqpBIvh6LovbBM=",
            realm="amessage.de",
            qop="auth,auth-int,auth-conf",
            cipher="rc4-40,rc4-56,rc4,des,3des",
            maxbuf=1024,
            charset=utf-8,
            algorithm=md5-sess
        */

        #region Nested type: ChallengeParseException

        /// <summary>
        ///   Exception occurs when we were unable to parse the challenge
        /// </summary>
        public class ChallengeParseException : Exception
        {
            #region Constructor

            /// <summary>
            /// </summary>
            /// <param name="message"> </param>
            public ChallengeParseException(string message)
                : base(message)
            {
            }

            #endregion
        }

        #endregion
    }
}