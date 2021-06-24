//-----------------------------------------------------------------------
// <copyright file="DoHomeUdpClient.cs" company="Company">
//    Copyright (c) Tino Donderwinkel. All rights reserved.
// </copyright>
// <author>Tino Donderwinkel</author>
//-----------------------------------------------------------------------

namespace DoHome
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// This class defines a UDP listener, based on the standard <see cref="UdpClient"/>.
    /// </summary>
    public class DoHomeUdpClient : UdpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoHomeUdpClient"/> class.
        /// </summary>
        /// <param name="ipLocalEndPoint">The local <see cref="IPEndPoint"/> for the <see cref="DoHomeUdpClient"/>.</param>
        public DoHomeUdpClient(IPEndPoint ipLocalEndPoint) : base(ipLocalEndPoint)
        {
            Socket socket = this.Client;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }
    }
}
