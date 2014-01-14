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

namespace ASC.Mail.Net.UDP
{
    #region usings

    using System;
    using System.Net;
    using System.Net.Sockets;

    #endregion

    /// <summary>
    /// This class provides data for <b>UdpServer.PacketReceived</b> event.
    /// </summary>
    public class UDP_PacketEventArgs : EventArgs
    {
        #region Members

        private readonly byte[] m_pData;
        private readonly IPEndPoint m_pRemoteEP;
        private readonly Socket m_pSocket;
        private readonly UDP_Server m_pUdpServer;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">UDP server which received packet.</param>
        /// <param name="socket">Socket which received packet.</param>
        /// <param name="remoteEP">Remote end point which sent data.</param>
        /// <param name="data">UDP data.</param>
        internal UDP_PacketEventArgs(UDP_Server server, Socket socket, IPEndPoint remoteEP, byte[] data)
        {
            m_pUdpServer = server;
            m_pSocket = socket;
            m_pRemoteEP = remoteEP;
            m_pData = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets UDP packet data.
        /// </summary>
        public byte[] Data
        {
            get { return m_pData; }
        }

        /// <summary>
        /// Gets local end point what recieved packet.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return (IPEndPoint) m_pSocket.LocalEndPoint; }
        }

        /// <summary>
        /// Gets remote end point what sent data.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return m_pRemoteEP; }
        }

        /// <summary>
        /// Gets UDP server which received packet.
        /// </summary>
        public UDP_Server UdpServer
        {
            get { return m_pUdpServer; }
        }

        /// <summary>
        /// Gets socket which received packet.
        /// </summary>
        internal Socket Socket
        {
            get { return m_pSocket; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends reply to received packet. This method uses same local end point to send packet which
        /// received packet, this ensures right NAT traversal.
        /// </summary>
        /// <param name="data">Data buffer.</param>
        /// <param name="offset">Offset in the buffer.</param>
        /// <param name="count">Number of bytes to send.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>data</b> is null.</exception>
        public void SendReply(byte[] data, int offset, int count)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            IPEndPoint localEP = null;
            m_pUdpServer.SendPacket(m_pSocket, data, offset, count, m_pRemoteEP, out localEP);
        }

        #endregion
    }
}