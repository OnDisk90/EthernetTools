

namespace SerialLib
{
    public abstract class PortAbstraction
    {
        /// <summary>
        /// List of errors & statuses which a port can return.
        /// </summary>
        public enum PortErrors
        {
            OK, // All OK
            TIMEOUT, // Exit on timeout
            CONNECT_ERROR, // Failed to connect port.
            PERMISSION_ERROR, // Application doesn't have enough permissions.
            CONNECT_ERROR_2, // Failed to connect port.
            DISCONNECT_ERROR, // Failed to disconnect.
            SEND_ERROR, // A send data operation failed.
            RX_ERROR // A recieve error operation failed.
        }

        /// <summary>
        /// Subscribe to this event in order to recieve events
        /// when new data is recieved by the port.
        /// </summary>
        public event OnNewData _OnNewData;

        /// <summary>
        /// Subscribe to this event in order to recieve events
        /// when the current send operation was finished.
        /// </summary>
        public event OnEndSend _OnEndSend;

        /// <summary>
        // Subscribe to this event in order to recieve
        /// errors during communication. 
        /// </summary>
        public event OnError _OnError;

        /// <summary>
        /// Called when the port is disconnected from the other port.
        /// </summary>
        public event OnDisconnect _OnDisconnect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>  
        public PortAbstraction(int Id)
        {
            _Id = Id;
        }

        /// <summary>
        /// Returns if the port is connected or not.
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
        }


        /// <summary>
        /// Get the last error of the port.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public PortErrors GetLastError(out string status)
        {
             status = _LastStatus;
            return _LastError;
        }

        /// <summary>
        /// Sets the last error 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="status"></param>
        public void SetLastError(PortErrors error, string status)
        {
            _LastError = error;
            _LastStatus = status;

            if (_OnError != null && _LastError != PortErrors.OK)
            {
                _OnError(_Id, _LastError, _LastStatus);
            }
        }


        /// <summary>
        /// Subscribe this type of function to the _OnNewData event.
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>
        /// <param name="Data"></param>
        /// <param name="Length"></param>
        public delegate void OnNewData(int Id, byte[] Data, int Length);

        /// <summary>
        /// Subscribe this type of function to the _OnEndSend event.
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param> 
        /// <param name="Error"> 
        /// An error 
        /// </param> 
        /// <param name="Status"> 
        /// Status message
        /// </param> 
        public delegate void OnEndSend(int Id, PortErrors Error, string Status);

        /// <summary>
        /// Subscribe this type of function to the _OnError event.
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>  
        /// <param name="Error">
        /// The error.
        /// </param>
        /// <param name="Status"> 
        /// Status message
        /// </param>  
        public delegate void OnError(int Id, PortErrors Error, string Status);

        /// <summary>
        /// Subscribe this type of function to the _OnDisconnect event.
        /// </summary>
        /// <param name="Id">
        /// An id of the port.
        /// It is recommended that this id would be unique although the library doesn't enforce this restriction.
        /// </param>  
        /// <param name="Error">
        /// The error.
        /// </param>
        /// <param name="Status"> 
        /// Status message
        /// </param>   
        public delegate void OnDisconnect(int Id, PortErrors Error, string Status);

        /// <summary>
        /// Connect to the port
        /// This function resets the previouse error value of the port
        /// All functions in this abstract class must set the LastError value.
        /// </summary>
        /// <param name="Status"> 
        /// Status message
        /// </param>   
        /// <returns>
        /// The LastError value
        /// </returns>
        public abstract PortErrors Connect(out string Status);

        /// <summary>
        /// Disconnect to the port
        /// All functions in this abstract class must set the LastError value.
        /// </summary>
        /// <param name="Status"> 
        /// Status message
        /// </param>   
        /// <returns>
        /// The LastError value
        /// </returns>
        public abstract PortErrors Disconnect(out string Status);
       
        /// <summary>
        /// Sends data to the port
        /// </summary>
        /// <param name="Data">
        /// The data to be sent
        /// </param>
        /// <param name="Offset">
        /// Offset in the Data from which to write
        /// </param> 
        /// <param name="Length">
        /// Length in bytes of data
        /// </param>
        /// <param name="Status"> 
        /// Status message
        /// </param> 
        /// <returns>
        /// The LastError value
        /// </returns>
        public abstract PortErrors Write(byte[] Data, int Offset, int Length, out string status);               

        /// <summary>
        /// Raise new data event.
        /// </summary>
        /// <param name="Id">
        /// The id of the port.
        /// </param>  
        /// <param name="Data">
        /// The data
        /// </param>
        /// <param name="Length">
        /// Length of data
        /// </param>
        /// <param name="Offset">
        /// Offset into the Data buffer.
        /// </param>
        protected void RaiseNewDataEvent(int Id, byte[] Data, int Length)
        {
            if (_OnNewData != null)
            {
                _OnNewData(Id, Data, Length);
            }
        }

        /// <summary>
        /// Raise the EndSend Event
        /// </summary>
        /// <param name="Id">
        /// The id of the port.
        /// </param>   
        /// <param name="Error">
        /// The error.
        /// </param>
        /// <param name="Status"> 
        /// Status message
        /// </param>  
        protected void RaiseEndSendEvent(int Id, PortErrors Error, string Status)
        {
            if (_OnEndSend != null)
            {
                _OnEndSend(Id, Error, Status);
            }
        }

        /// <summary>
        /// Raise disconnect event.
        /// </summary>
        /// <param name="Id">
        /// The id of the port.
        /// </param>   
        /// <param name="Error">
        /// The error.
        /// </param>
        /// <param name="Status"> 
        /// Status message
        /// </param>   
        protected void RaiseDisconnect(int Id, PortErrors Error, string Status)
        {
            _connected = false;

            if (_OnDisconnect != null)
            {
                _OnDisconnect(Id, Error, Status);
            }
        }


        protected int _Id;
        protected bool _connected = false;     
        private PortErrors _LastError = PortErrors.OK; // The last error 
        private string _LastStatus = ""; // The last status       
    }
}
