using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace SerialLib
{
    public class UdpClientPort : PortAbstraction
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
        public UdpClientPort(int Id, int PortNum, String ServerIp, bool RxEnabled)
            : base(Id)
        {
            _PortNum = PortNum;
            _ServerIp = ServerIp;
            _RxEnabled = RxEnabled;
        }
        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Connect(out string Status)
        {
            Status = "";

            try
            {
                _UDPClient = new UdpClient();
                IPAddress ip = IPAddress.Parse(_ServerIp);
                IPEndPoint Server = new IPEndPoint(ip, _PortNum);
                _EndPointServer = (EndPoint)Server;
                _UDPClient.Connect(ip, _PortNum);
                _EndPointClient = _UDPClient.Client.LocalEndPoint;

                if (_RxEnabled)
                {
                    _UDPClient.BeginReceive(new AsyncCallback(OnRecieve), _UDPClient);
                }

                _connected = true; // Declare the port as connected & ready to recieve data
            }
            catch (System.Security.SecurityException ex)
            {
                Debug.Print(ex.Message);
                Status = ex.Message;
                SetLastError(PortErrors.PERMISSION_ERROR, Status);
            }
            catch (ArgumentNullException ex  )
            {
                Debug.Print(ex.Message);
                Status = ex.Message;
                SetLastError(PortErrors.CONNECT_ERROR, Status);
            }
            catch (FormatException ex)
            {
                Debug.Print(ex.Message);
                Status = ex.Message;
                SetLastError(PortErrors.CONNECT_ERROR, Status);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Status = ex.Message;
                SetLastError(PortErrors.CONNECT_ERROR, Status);
            }

            return GetLastError(out Status);
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors  Disconnect(out string Status)
        {
            PortErrors res = PortErrors.OK;
            _connected = false; // Declare port as disconnected.
            Status = "";

            try
            {
                _UDPClient.Close();
                _UDPClient = null;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
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
        /// <param name="status"></param>
        /// <returns></returns>
        public override PortErrors Write(byte[] Data, int Offset, int Length, out string status)
        {
            if (Data.Length > MAX_UDP_PACKET_SIZE)
            {
                SetLastError(PortErrors.SEND_ERROR, "Data length exceeds maximum udp packet size.");
            }
            else
            {
                try
                {
                    _UDPClient.BeginSend(Data, Length, new AsyncCallback(OnSend), _UDPClient);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    SetLastError(PortErrors.SEND_ERROR, ex.Message);
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
                _UDPClient.EndSend(ar);
                RaiseEndSendEvent(_Id, PortErrors.OK, "");
            }
            catch (Exception ex)
            {
                SetLastError(PortErrors.SEND_ERROR , ex.Message);
            }
        }

        /// <summary>
        /// This function is called after a recieve operation was finished.
        /// </summary>
        /// <param name="ar"></param>
        private void OnRecieve(IAsyncResult ar)
        {
            try
            {
                if (((UdpClient)(ar.AsyncState)).Client != null)
                {
                    IPEndPoint e = (IPEndPoint)_EndPointClient;

                    Byte[] receiveBytes = _UDPClient.EndReceive(ar, ref e);

                    RaiseNewDataEvent(_Id, receiveBytes, receiveBytes.Length);
                    _UDPClient.BeginReceive(new AsyncCallback(OnRecieve), _UDPClient);

                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                SetLastError(PortErrors.RX_ERROR , ex.Message);
            }

        }

        private const int MAX_UDP_PACKET_SIZE = 65507;//in bytes
        private readonly int _PortNum; // The port num of the client.
        private readonly String _ServerIp; // A string with the ip or the server
        private EndPoint _EndPointServer = null;
        private EndPoint _EndPointClient = null;
        private UdpClient _UDPClient;
        private readonly bool _RxEnabled;

    }
}
