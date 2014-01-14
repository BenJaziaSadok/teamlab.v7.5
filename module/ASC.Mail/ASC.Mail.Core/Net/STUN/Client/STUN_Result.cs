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

namespace ASC.Mail.Net.STUN.Client
{
    #region usings

    using System.Net;

    #endregion

    /// <summary>
    /// This class holds STUN_Client.Query method return data.
    /// </summary>
    public class STUN_Result
    {
        #region Members

        private readonly STUN_NetType m_NetType = STUN_NetType.OpenInternet;
        private readonly IPEndPoint m_pPublicEndPoint;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="netType">Specifies UDP network type.</param>
        /// <param name="publicEndPoint">Public IP end point.</param>
        public STUN_Result(STUN_NetType netType, IPEndPoint publicEndPoint)
        {
            m_NetType = netType;
            m_pPublicEndPoint = publicEndPoint;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets UDP network type.
        /// </summary>
        public STUN_NetType NetType
        {
            get { return m_NetType; }
        }

        /// <summary>
        /// Gets public IP end point. This value is null if failed to get network type.
        /// </summary>
        public IPEndPoint PublicEndPoint
        {
            get { return m_pPublicEndPoint; }
        }

        #endregion
    }
}