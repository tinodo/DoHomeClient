namespace DoHome
{
    using System.Net;
    using System.Net.Sockets;

    public class DoHomeUdpClient : UdpClient
    {
        public DoHomeUdpClient() : base()
        {
            Socket socket = this.Client;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

        public DoHomeUdpClient(IPEndPoint ipLocalEndPoint) : base(ipLocalEndPoint)
        {
            Socket socket = this.Client;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
        }

    }
}
