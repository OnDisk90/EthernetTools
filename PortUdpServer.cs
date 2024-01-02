using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SerialLib
{
    public class PortUdpServer : PortAbstraction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>   
        /// <param name="PortNum">
        /// Port number
        /// /param>
        /// <param name="RxEnabled">
        /// true - Port is open for both tx and rx
        /// false - Port is open for tx only
        /// </param>  
        public PortUdpServer(int Id, int PortNum, bool RxEnabled)
            : base(Id)
        {
            _PortNum = PortNum;
            _RxEnabled = RxEnabled;
            SetLastError(PortErrors.OK, ""); // No errors yet.
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Connect(out string Status)
        {
            // we need to initialize the last error before new connection
            // the pervious error are not relevant already
            SetLastError(PortErrors.OK, "");

            try
            {             
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Assign the any IP of the machine and listen on the correct port number.
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _PortNum);
                Remote = (EndPoint)Sender;

                //Bind and listen on the given address
                _ServerSocket.Bind(ipEndPoint);
                _ServerSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);

                // Start to accept the incoming clients
                if (_RxEnabled)
                {
                    Debug.Print("UDP server begin to listen");
                    _ServerSocket.BeginReceiveFrom(_RxData, 0, _RxData.Length, SocketFlags.None, ref Remote, new AsyncCallback(OnReceive), _ServerSocket);
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

            return GetLastError(out Status);
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Disconnect(out string Status)
        {
            PortErrors res = PortErrors.OK;
            Status = "";
            _connected = false; // Declare port as disconnected.

            try
            {
                _ServerSocket.Close();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.DISCONNECT_ERROR, ex.Message);
            }
            finally
            {
                res = GetLastError(out Status);
                RaiseDisconnect(_Id, res, Status);
            }

            return res;
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Offset"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public override PortErrors Write(byte[] Data, int Offset, int Length, out string status)
        {
            PortErrors res = GetLastError(out status);

            if (res == PortErrors.OK)
            {
                if (Data.Length > MAX_UDP_PACKET_SIZE)
                {
                    SetLastError(PortErrors.SEND_ERROR, "Data size exceed maximum udp packet size.");
                }
                else
                {
                    try
                    {
                        // Debug.Print("going to send " + DateTime.Now.Ticks.ToString());                   
                        _ServerSocket.BeginSendTo(Data, Offset, Length, SocketFlags.None, Remote, new AsyncCallback(OnSend), _ServerSocket);
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
               // Debug.Print("finish sending "  + DateTime.Now.Ticks.ToString());
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
                if (_ServerSocket != null)
                {

                    int RecievedBytes = _ServerSocket.EndReceiveFrom(ar, ref Remote);
                    if (RecievedBytes > 0)
                    {
                        byte[] Buffer = new byte[RecievedBytes];

                        Array.Copy(_RxData, Buffer, RecievedBytes);

                        RaiseNewDataEvent(_Id, Buffer, RecievedBytes);
                    }
                    _ServerSocket.BeginReceiveFrom(_RxData, 0, _RxData.Length, SocketFlags.None, ref Remote, new AsyncCallback(OnReceive), _ServerSocket);
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
                Sender = new IPEndPoint(IPAddress.Any, 0);
                Remote = (EndPoint)Sender;
                _ServerSocket.BeginReceiveFrom(_RxData, 0, _RxData.Length, SocketFlags.None, ref Remote, new AsyncCallback(OnReceive), _ServerSocket);
                SetLastError(PortErrors.RX_ERROR, ex.Message);
            }
        }

        private const int MAX_UDP_PACKET_SIZE = 65507;//in bytes
        private Socket _ServerSocket; //The main socket on which the server listens to the clients
        private readonly int _PortNum; // The port num on which the server listens 
        private readonly byte[] _RxData = new byte[MAX_UDP_PACKET_SIZE]; // The data recievied from the socket on the last recieve request. 
        private IPEndPoint Sender = new IPEndPoint(IPAddress.Any, 0);
        private EndPoint Remote = null;
        private readonly bool _RxEnabled;
    }
}
