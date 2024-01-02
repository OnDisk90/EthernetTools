using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SerialLib
{
    public class PortTcpServer : PortAbstraction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>   
        /// <param name="portNum"/>
        /// Port number
        /// /param>
        /// <param name="rxEnabled">
        /// true - Port is open for both tx and rx
        /// false - Port is open for tx only
        /// </param> 
        public PortTcpServer(int id, int portNum, bool rxEnabled)
            : base(id)
        {
            _PortNum = portNum;
            _RxEnabled = rxEnabled;

            _ClientSockets.Clear();

            _DataQueue.Clear();

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
                //We are using TCP sockets
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Assign the any IP of the machine and listen on the correct port number.
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _PortNum);

                //Bind and listen on the given address
                _ServerSocket.Bind(ipEndPoint);
                _ServerSocket.Listen(_Backlog);               

                // Start to accept the incoming clients
                _ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                // The socket is now connected & ready to recieve data
                _connected = true;
            }
            catch (System.Security.SecurityException ex)
            {
                SetLastError(PortErrors.PERMISSION_ERROR , ex.Message);
            }
            catch (Exception ex)
            {
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
            _connected = false; // Declare port as disconnected.
            Status = "";

            try
            {
                foreach (Socket ASocket in _ClientSockets)
                {
                    ASocket.Shutdown(SocketShutdown.Both);
                    ASocket.Close();
                }

                _ServerSocket.Close();
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.CONNECT_ERROR, ex.Message);
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
                try
                {
                    _ClientSockets[0].BeginSend(Data, Offset, Length, SocketFlags.None, new AsyncCallback(OnSend), _ClientSockets[0]);
                }
                catch (Exception ex)
                {
                     SetLastError(PortErrors.SEND_ERROR , ex.Message);
                }

            }

            return GetLastError(out status);
        }

        #region PRIVATE

        /// <summary>
        /// This function is called after the server accepts new client
        /// It adds the client to its client list and starts the waiting for 
        /// data from the client async mode.
        /// </summary>
        /// <param name="ar"></param>
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                // we need to initialize the last error before accepting new client, 
                // the pervious error are not relevant already
                SetLastError(PortErrors.OK, ""); // No errors yet.

                Socket ClientSocket = _ServerSocket.EndAccept(ar);
                _ClientSockets.Clear();
                _ClientSockets.Add(ClientSocket);

                //Start listening for more clients
                _ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //Once the client connects then start receiving the commands from it
                if (_RxEnabled)
                {
                    ClientSocket.BeginReceive(_RxData, 0, _RxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), ClientSocket);
                }
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.CONNECT_ERROR_2 , ex.Message);
            }
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
                SetLastError(PortErrors.SEND_ERROR , ex.Message);
            }
        }

        /// <summary>
        /// This function is called after a recieve operation was finished.
        /// It adds the incoming data to a fifo and restart the read operation.
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            Socket ClientSocket = null;

            try
            {
                ClientSocket = (Socket)ar.AsyncState;

                int RecievedBytes = ClientSocket.EndReceive(ar);

                // This means that the server is diconnected from client
                if (RecievedBytes == 0)
                {
                    RaiseDisconnect(_Id, PortErrors.OK, "");
                }
                else if (RecievedBytes > 0)
                {
                    byte[] Buffer = new byte[RecievedBytes];

                    Array.Copy(_RxData, Buffer, RecievedBytes);

                    RaiseNewDataEvent(_Id, Buffer, RecievedBytes);

                    ClientSocket.BeginReceive(_RxData, 0, _RxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), ClientSocket);
                }
                else
                {
                    SetLastError(PortErrors.RX_ERROR, "Recieved corrupted data.");
                }
            }
            catch (ObjectDisposedException ex)
            {
                SetLastError(PortErrors.RX_ERROR , ex.Message);
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.RX_ERROR, ex.Message);
            }
        }

        private Socket _ServerSocket; //The main socket on which the server listens to the clients
        private readonly List<Socket> _ClientSockets = new List<Socket>(); 
        private readonly int _PortNum; // The port num on which the server listens
        private const int _Backlog = 4; // The maximum length of the pending connections queue. 
        private readonly byte[] _RxData = new byte[256 * 1024]; // The data recievied from the socket on the last recieve request.
        private readonly Queue<byte> _DataQueue = new Queue<byte>(); // Fifo of incoming data.
  
        private object _Mutex = new object();
        private readonly bool _RxEnabled;

        #endregion PRIVATE
    }
}
