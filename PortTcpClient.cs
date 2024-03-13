using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SerialLib
{
    public class PortTcpClient : PortAbstraction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>   
        /// <param name="PortNum">
        /// Port number.
        /// </param>
        /// <param name="ServerIp">
        /// Ip address of the server.
        /// </param>
        /// <param name="RxEnabled">
        /// true - Port is open for both tx and rx
        /// false - Port is open for tx only
        /// </param> 
        public PortTcpClient(int Id, int PortNum, String ServerIp, bool RxEnabled)
            : base(Id)
        {
            _PortNum = PortNum;
            _ServerIp = ServerIp;
            _RxEnabled = RxEnabled;
            _DataQueue.Clear();

            SetLastError(PortErrors.OK, ""); // No errors yet.
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Connect(out string Status)
        {
             SetLastError(PortErrors.OK , "");

            try
            {
                SetLastError(PortErrors.OK, ""); // No errors yet.
                _ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress = IPAddress.Parse(_ServerIp);

                //Server is listening on the selected port.
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, _PortNum);

                //Reset event that waiting for connection to finished.
                ConnectedEvent.Reset();

                //Connect to the server          
                IAsyncResult result = _ClientSocket.BeginConnect(ipEndPoint, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(1500, true);
                if (!success)
                {
                    _ClientSocket.Close();
                    SetLastError(PortErrors.CONNECT_ERROR, "Failed to connect port"); 
                }
                else
                {                    
                    OnConnect(result);
                    _connected = true; // Declare the port as connected & ready to recieve data
                }
            }
            catch (System.Security.SecurityException ex)
            {
                SetLastError(PortErrors.PERMISSION_ERROR, ex.Message);
                _connected = false;
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.CONNECT_ERROR, ex.Message);
                _connected = false;
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
                _ClientSocket.Shutdown(SocketShutdown.Both);
                _ClientSocket.Close();

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
                    _ClientSocket.BeginSend(Data, Offset, Length, SocketFlags.None, new AsyncCallback(OnSend), _ClientSocket);
                }
                catch (Exception ex)
                {
                    _connected = false;
                    SetLastError(PortErrors.SEND_ERROR , ex.Message);
                }

            }

            return GetLastError(out status);
        }

       #region PRIVATE

        /// <summary>
        /// This function is called after the server accepted this client.
        /// The client starts async waiting from the server.
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                _ClientSocket.EndConnect(ar);

                if (_RxEnabled)
                {
                    _ClientSocket.BeginReceive(_RxData, 0, _RxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
                }
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.CONNECT_ERROR_2 , ex.Message);
                _connected = false;
            }
            // Raise the end of connection event.
            // The Connect function above can now return safely.
            ConnectedEvent.Set();
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
                _ClientSocket.EndSend(ar);
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
            try
            {
                int RecievedBytes = _ClientSocket.EndReceive(ar);

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

                    _ClientSocket.BeginReceive(_RxData, 0, _RxData.Length, SocketFlags.None, new AsyncCallback(OnReceive), _ClientSocket);
                }
                else
                {
                    SetLastError(PortErrors.RX_ERROR , "Recieved corrupted data.");
                    _connected = false;
                }
               

            }
            catch (ObjectDisposedException ex)
            {
                SetLastError(PortErrors.RX_ERROR , ex.Message);
                _connected = false;
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.RX_ERROR, ex.Message);
                _connected = false;
            }
        }

        private Socket _ClientSocket;
        private readonly ManualResetEvent ConnectedEvent = new ManualResetEvent(false);

        private readonly int _PortNum; // The port num of the client.
        private readonly String _ServerIp; // A string with the ip or the server
        private readonly byte[] _RxData = new byte[1024* 128]; // The data recievied from the socket on the last recieve request.
        private readonly Queue<byte> _DataQueue = new Queue<byte>(); // Fifo of incoming data.
        private object _Mutex = new object();
        private readonly bool _RxEnabled;

        #endregion PRIVATE

    }

}
