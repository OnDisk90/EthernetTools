using System;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SerialLib
{
    public class PortUdpMulticast : PortAbstraction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>   
        ///         /// <param name="portNum">
        ///     port number 
        /// </param>
        /// <param name="mcastGroup">
        ///     Multicast ip address
        /// </param>
        /// <param name="rxEnabled">
        ///     Port is open for tx or for tx only (true)
        /// </param>
        /// <param name="localIpAddress">
        ///     local computer IP address
        /// </param>
        /// <param name="ttl"></param>
        public PortUdpMulticast(int id, int portNum, string mcastGroup, bool rxEnabled, string localIpAddress, int ttl)
            : base(id)
        {
            _mcastGroupIp = mcastGroup;
            _portNum = portNum;
            _rxEnabled = rxEnabled;
            _localIpAddress = localIpAddress;
            _Ttl = ttl;
            SetLastError(PortErrors.OK, ""); // No errors yet.
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Connect(out string status)
        {
            // we need to initialize the last error before new connection
            // the pervious error are not relevant already
            SetLastError(PortErrors.OK, "");

            try
            {             
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                var iPAddress = _localIpAddress != null ? IPAddress.Parse(_localIpAddress) : IPAddress.Any;
                IPEndPoint ipEndPoint = new IPEndPoint(iPAddress, _portNum);
                if (_rxEnabled) // socet for RX
                {
                   
                    
                    _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);                    
                    _serverSocket.Bind(ipEndPoint);
                    IPAddress ip = IPAddress.Parse(_mcastGroupIp);
                    _serverSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
                    Debug.Print("UDP server begin to listen");
                    _serverSocket.BeginReceive(_rxData, 0, _rxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), _serverSocket);                                        
                }
                else // socet for tx
                {
                    // Join to multicast
                    IPAddress ip = IPAddress.Parse(_mcastGroupIp);
                    _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _serverSocket.Bind(ipEndPoint);
                    _serverSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));                    
                    // Set time to live                    
                    _serverSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, _Ttl);
                    IPEndPoint ipep = new IPEndPoint(ip, _portNum);
                    _serverSocket.Connect(ipep);

                }
                _connected = true; // Declare the port as connected & ready to recieve data
            }
            catch (System.Security.SecurityException ex)
            {
                SetLastError(PortErrors.PERMISSION_ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.CONNECT_ERROR, ex.Message);
            }

            return GetLastError(out status);
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Disconnect(out string status)
        {
            PortErrors res;
            status = "";
            _connected = false; // Declare port as disconnected.

            try
            {
                _serverSocket.Close();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.DISCONNECT_ERROR, ex.Message);
            }
            finally
            {
                res = GetLastError(out status);
                RaiseDisconnect(_Id, res, status);
            }

            return res;
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override PortErrors Write(byte[] data, int offset, int length, out string status)
        {
            PortErrors res = GetLastError(out status);

            if (res == PortErrors.OK)
            {
                if (data.Length > MaxUdpPacketSize)
                {
                    SetLastError(PortErrors.SEND_ERROR, "Data size exceed maximum udp packet size.");
                }
                else
                {
                    try
                    {
                        _serverSocket.BeginSend(data, offset, length, SocketFlags.None, new AsyncCallback(OnSend), _serverSocket);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);

                        SetLastError(PortErrors.SEND_ERROR, ex.Message);
                    }
                }
            }
            return GetLastError(out status);
        }

        /// <summary>
        /// This functions is called after a send operation is finished.
        /// It cleans up.
        /// </summary>
        /// <param name="ar"></param>
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
                RaiseEndSendEvent(_Id, PortErrors.OK, "");
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.SEND_ERROR , ex.Message);
            }
        }
        /// <summary>
        /// This function is called after a recieve operation was finished.
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            SetLastError(PortErrors.OK, ""); // No errors yet.
            try
            {
                if (_serverSocket != null)
                {

                    int recievedBytes = _serverSocket.EndReceive(ar);
                    if (recievedBytes > 0)
                    {
                        byte[] buffer = new byte[recievedBytes];

                        Array.Copy(_rxData, buffer, recievedBytes);

                        RaiseNewDataEvent(_Id, buffer, recievedBytes);
                    }
                    //_ServerSocket.BeginReceiveFrom(_RxData, 0, _RxData.Length, SocketFlags.None, ref Remote, new AsyncCallback(OnReceive), _ServerSocket);
                    _serverSocket.BeginReceive(_rxData, 0, _rxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), _serverSocket);
                }
            }
            catch (ObjectDisposedException ex)
            {
                SetLastError(PortErrors.RX_ERROR , ex.Message);
            }
            catch (NullReferenceException ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.RX_ERROR, ex.Message);
            }
            catch (Exception ex)
            {
                //in case the client was forcedly closed
                //start another "listen" session  
                Debug.Print(ex.Message);
                //_sender = new IPEndPoint(IPAddress.Any, 0);
                //Remote = (EndPoint)_sender;
                //_ServerSocket.BeginReceiveFrom(_RxData, 0, _RxData.Length, SocketFlags.None, ref Remote, new AsyncCallback(OnReceive), _ServerSocket);
                if (_serverSocket != null)
                   _serverSocket.BeginReceive(_rxData, 0, _rxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), _serverSocket);
                SetLastError(PortErrors.RX_ERROR, ex.Message);
            }
        }

        private const int MaxUdpPacketSize = 65507;//in bytes
        private Socket _serverSocket; //The main socket on which the server listens to the clients
        private readonly int _portNum; // The port num on which the server listens 
        private readonly byte[] _rxData = new byte[MaxUdpPacketSize]; // The data recievied from the socket on the last recieve request. 
        //private IPEndPoint _sender = new IPEndPoint(IPAddress.Any, 0);
        private readonly string _localIpAddress;
        //private EndPoint Remote = null;
        private readonly bool _rxEnabled;
        private readonly string _mcastGroupIp;
        private int _Ttl = 4;
    }
}
