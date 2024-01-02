using System;
using System.IO.Ports;
using System.Threading;


namespace SerialLib
{
    public class PortSerial : PortAbstraction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>   
        /// <param name="PortName">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="BaudRate">     
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="DataBits">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="TheStopBits">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="TheParity">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="ReadTimeout">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="WriteTimeout">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="TheHandshake">
        /// See SerialPort class for explanations.
        /// </param>
        /// <param name="RxEnabled">
        /// true - Port is open for both tx and rx
        /// false - Port is open for tx only
        /// </param> 
        public PortSerial(
                int Id,
                String PortName,
                int BaudRate,
                int DataBits,
                StopBits TheStopBits,
                Parity TheParity,
                int ReadTimeout,
                int WriteTimeout,
                Handshake TheHandshake,
                bool RxEnabled)
            : base(Id)
        {
            _Port.PortName = PortName;
            _Port.BaudRate = BaudRate;
            _Port.DataBits = DataBits;
            _Port.StopBits = TheStopBits;
            _Port.Parity = TheParity;
            _Port.ReadTimeout = ReadTimeout;
            _Port.WriteTimeout = WriteTimeout;
            _Port.Handshake = TheHandshake;
            _StopReq = false;
            _StopAck = true;
            _RxEnabled = RxEnabled;
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <returns></returns>
        public override PortErrors Connect(out string Status)
        {
            SetLastError(PortErrors.OK, "");

            try
            {
                

                _StopReq = false;
                _StopAck = true;
                _Port.Open();

                if (_RxEnabled)
                {
                    _StopAck = false;
                    _RcvThread = new Thread(RcvThread);
                    _RcvThread.Start();
                    _connected = true; // Declare the port as connected & ready to recieve data
                }
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
            Status = "";
            _connected = false; // Declare port as disconnected.

            // Ask the recieving thread to exit.
            _StopReq = true;

            // Wait for the thread to actually exit.
            while (_StopAck == false)
            {
                Thread.Sleep(50);
            }

            try
            {
                _Port.Close();
            }
            catch(Exception ex)
            {
                res = PortErrors.CONNECT_ERROR;
                Status = ex.Message;
            }
            finally
            {
                RaiseDisconnect(_Id, res, Status);
            }

            return PortErrors.OK;
        }

        /// <summary>
        /// See interface for function details.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public override PortErrors Write(byte[] Data, int Offset, int Length, out string status)
        {
            try
            {
                _Port.Write(Data, Offset, Length);
            }
            catch(Exception ex)
            {
                SetLastError(PortErrors.SEND_ERROR , ex.Message);
            }

            return GetLastError(out status);
        }
        /// <summary>
        /// Request To Send
        /// </summary>
        /// <param name="flag"></param>
        public void SetRts(bool flag)
        {
            _Port.RtsEnable = flag;
        }
        /// <summary>
        /// Data Terminal Ready
        /// </summary>
        /// <param name="flag"></param>
        public void SetDtr(bool flag)
        {
            _Port.DtrEnable = flag;
        }
        /// <summary>
        /// This is a thread which runs in the background, gets new data 
        /// from the port and send the new data to clients.
        /// </summary>
        private void RcvThread()
        {
            const int Offset = 0;
            const int BufferSize = 1000;
            byte[] Buffer = new byte[BufferSize];
            int Length;

            while (_StopReq == false)
            {
                Length = 0;

                try
                {
                    Length = _Port.Read(Buffer, Offset, Buffer.Length);
                }
                catch
                {
                }

                if (Length > 0)
                {
                    RaiseNewDataEvent(_Id, Buffer, Length);
                }
            }

            _StopAck = true;
        }

        private readonly SerialPort _Port = new SerialPort(); // _Port is the serial port of all the messages in this class.
        private Thread _RcvThread;
        private bool _StopReq;
        private bool _StopAck;
        private readonly bool _RxEnabled;
    }     
}
