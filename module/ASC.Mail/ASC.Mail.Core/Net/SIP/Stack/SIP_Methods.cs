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

namespace ASC.Mail.Net.SIP.Stack
{
    /// <summary>
    /// This class represents known SIP request methods.
    /// </summary>
    public class SIP_Methods
    {
        #region Constants

        /// <summary>
        /// ACK method. Defined in RFC 3261.
        /// </summary>
        public const string ACK = "ACK";

        /// <summary>
        /// BYE method. Defined in RFC 3261.
        /// </summary>
        public const string BYE = "BYE";

        /// <summary>
        /// CANCEL method. Defined in RFC 3261.
        /// </summary>
        public const string CANCEL = "CANCEL";

        /// <summary>
        /// INFO method. Defined in RFC 2976.
        /// </summary>
        public const string INFO = "INFO";

        /// <summary>
        /// INVITE method. Defined in RFC 3261.
        /// </summary>
        public const string INVITE = "INVITE";

        /// <summary>
        /// MESSAGE method. Defined in RFC 3428.
        /// </summary>
        public const string MESSAGE = "MESSAGE";

        /// <summary>
        /// NOTIFY method. Defined in RFC 3265.
        /// </summary>
        public const string NOTIFY = "NOTIFY";

        /// <summary>
        /// OPTIONS method. Defined in RFC 3261.
        /// </summary>
        public const string OPTIONS = "OPTIONS";

        /// <summary>
        /// PRACK method. Defined in RFC 3262.
        /// </summary>
        public const string PRACK = "PRACK";

        /// <summary>
        /// PUBLISH method. Defined in RFC 3903.
        /// </summary>
        public const string PUBLISH = "PUBLISH";

        /// <summary>
        /// REFER method. Defined in RFC 3515.
        /// </summary>
        public const string REFER = "REFER";

        /// <summary>
        /// REGISTER method. Defined in RFC 3261.
        /// </summary>
        public const string REGISTER = "REGISTER";

        /// <summary>
        /// SUBSCRIBE method. Defined in RFC 3265.
        /// </summary>
        public const string SUBSCRIBE = "SUBSCRIBE";

        /// <summary>
        /// UPDATE method. Defined in RFC 3311.
        /// </summary>
        public const string UPDATE = "UPDATE";

        #endregion
    }
}