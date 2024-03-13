using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using SerialLib;
using System.Net;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace MultiCastSend
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _configuration = new Configuration();            
            LoadConfiguration("Configuration.xml");
            InitGuiData();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            CreateNewWriter();
            UpdateLocalData();
            CommunicationConnect();
           
        }

        private void CreateNewWriter()
        {
            if (_binaryWriter != null)
                _binaryWriter.Close();
            _binaryWriter = new BinaryWriter(File.OpenWrite(genName()));
        }
        private string genName()
        {
            DateTime now = DateTime.Now;
            string name = string.Format("{0}_{1}_{2}_{3}_{4}_{5}.BIN", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            return name;
        }

        bool checkMulticast(string ip)
        {
            int num = 0;
            string[] ar = ip.Split('.');
            if (ar.Length > 0)
                int.TryParse(ar[0], out num);
            if ((num >= 224) && (num <= 239))
                return true;
            else
            {
                textBoxConnection.Text = " Invalid address should start with 224..239 ";
                return false;
            }

        }

        private void CommunicationConnect()
        {
            if (cmbType.SelectedIndex == 0)
            {
                if (checkMulticast(_configuration.serverIp))
                {
                    MulicastConnect();
                }
            }
            else if (cmbType.SelectedIndex == 1)
            {
                UdpServerConnect();
            }
            else if (cmbType.SelectedIndex == 2)
            {
                UdpClientConnect();
            }
            else if (cmbType.SelectedIndex == 3)
            {
                TcpServerConnect();
            }
            else if (cmbType.SelectedIndex == 4)
            {
                TcpClientConnect();
            }
        }

        private void UdpServerConnect()
        {
            string status;

            _portRx = new PortUdpServer(_Id, _configuration.portNum, true); // rx and tx
            _portRx._OnNewData += OnNewData;
            _portRx.Connect(out status);
            if ((_portRx.Connected) && (status.Length == 0))
            {
                textBoxConnection.Text = "UDP Listener Success, ";
            }
            else
            {
                textBoxConnection.Text = "UDP Listener " + status;
            }
            buttonDisconnect.Enabled = true;
            btnSend.Enabled = true;
            buttonConnect.Enabled = false;

            // Tx Port
            _portTx = _portRx;
        }
        private void UdpClientConnect()
        {
            string status;

            _portRx = new UdpClientPort(_Id, _configuration.portNum, _configuration.serverIp, true); // rx and tx
            _portRx._OnNewData += OnNewData;
            _portRx.Connect(out status);
            if ((_portRx.Connected) && (status.Length == 0))
            {
                textBoxConnection.Text = "UDP Client Success, ";
            }
            else
            {
                textBoxConnection.Text = "UDP Client " + status;
            }
            buttonDisconnect.Enabled = true;
            btnSend.Enabled = true;
            buttonConnect.Enabled = false;

            // Tx Port
            _portTx = _portRx;
        }

        private void TcpServerConnect()
        {
            string status;         

            _portRx = new PortTcpServer(_Id, _configuration.portNum, true); // rx and tx
            _portRx._OnNewData += OnNewData;
            _portRx.Connect(out status);
            if ((_portRx.Connected) && (status.Length == 0))
            {
                textBoxConnection.Text = "TCP Listener Success, ";
            }
            else
            {
                textBoxConnection.Text = "TCP Listener " + status;
            }
            buttonDisconnect.Enabled = true;
            btnSend.Enabled = true;
            buttonConnect.Enabled = false;

            // Tx Port
            _portTx = _portRx;
        }
        private void TcpClientConnect()
        {
            string status;

            _portRx = new PortTcpClient(_Id, _configuration.portNum, _configuration.serverIp, true); // rx and tx
            _portRx._OnNewData += OnNewData;
            _portRx.Connect(out status);
            if ((_portRx.Connected) && (status.Length == 0))
            {
                textBoxConnection.Text = "TCP Client Success, ";
            }
            else
            {
                textBoxConnection.Text = "TCP Client " + status;
            }
            buttonDisconnect.Enabled = true;
            btnSend.Enabled = true;
            buttonConnect.Enabled = false;

            // Tx Port
            _portTx = _portRx;
        }

        private void MulicastConnect()
        {
            string status;
            string ipaddress = null;
            if (_configuration.bindFlag)
            {
                int index = cmbLocalIpAddress.SelectedIndex;
                if ((index >= 0 ) && (index < cmbLocalIpAddress.Items.Count))
                   ipaddress = cmbLocalIpAddress.Items[index].ToString();
            }
            // the Multi cast port will receive

            _portRx = new PortUdpMulticast(_Id, _configuration.portNum, _configuration.serverIp, true, ipaddress, _configuration.ttl); // rx
            _portRx._OnNewData += OnNewData;
            _portRx.Connect(out status);
            if ((_portRx.Connected) && (status.Length == 0))
            {
                textBoxConnection.Text = "UDP Listener Success, ";
            }
            else
            {
                textBoxConnection.Text = "UDP Listener " + status;
            }
            buttonDisconnect.Enabled = true;
            btnSend.Enabled = true;
            buttonConnect.Enabled = false;
           
            // Tx Port
            _portTx = new PortUdpMulticast(_Id, _configuration.portNum, _configuration.serverIp, false, ipaddress, _configuration.ttl); // tx
            
            //_portUdpTx = new UdpClientPort(_Id + 1, _PortNum, _ServerIp, true);
            _portTx.Connect(out status);
            if (status.Length == 0)
            {
                textBoxConnection.Text += " UDP Tx Success";
            }
            else
            {
                textBoxConnection.Text += " UDP Tx " + status;
            }
                      
        }
        private void CommunicationDisconnect()
        {
            if (_binaryWriter != null)
                _binaryWriter.Close();
            _binaryWriter = null;
            if (_portRx != null)
            {
                string status;
                _portRx._OnNewData -= OnNewData;
                _portRx.Disconnect(out status);
                textBoxConnection.Text = status;
                _portRx = null;
                buttonConnect.Enabled = true;
                btnSend.Enabled = false;
                buttonDisconnect.Enabled = false;
            }         
        }
        private void UpdateLocalData()
        {
            int.TryParse(textBoxPortNum.Text, out _configuration.portNum);            
            int.TryParse(textBoxTTL.Text, out _configuration.ttl);
            int.TryParse(txtTimerMsec.Text, out _configuration.timeTimeout);
            _configuration.serverIp = textBoxAddress.Text;
            _configuration.bindFlag = chkBindIp.Checked;
            _configuration.selectedProtocol = cmbType.SelectedIndex;
            _configuration.dataDisplayFlag = chkRxData.Checked;
            _configuration.asciiFlag = chkAscii.Checked;
            _configuration.recordFlag = chkRecord.Checked;
            
            _configuration.timerEnable = checkBoxTimerEnable.Checked;
            _configuration.txBuffer = textBoxData.Text;
        }

        private void OnNewData(int id, byte[] data, int length)
        {
            _totalArived = _totalArived + length;
            if (_configuration.recordFlag)
               _binaryWriter.Write(data, 0, length);
            if (_configuration.dataDisplayFlag)
                DisplayData(id, data, length);
            UpdateRxStatus("Total " + _totalArived + ", Len " + length);
        }
        private void DisplayData (int id, byte[] data, int length)
        { 
            _rxCount++;
            _totalArived = _totalArived + length;
            string status = "Msg " + _rxCount +  " Arived "  + length + " Total " + _totalArived;
            

            if (_configuration.asciiFlag)
            {
                string str = System.Text.Encoding.UTF8.GetString(data);
                UpdateRxData(str);
            }
            else 
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    sb.AppendFormat("{0}", data[i]);
                    sb.Append(" ");
                }
                UpdateRxData(sb.ToString());
            }

        }
        private void UpdateRxData(string status)
        {
            if (InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateRxData);

                this.Invoke(d, new object[] { status });
            }
            else
            {
                txRxData.Text = status;
            }
        }


        private void UpdateRxStatus(string status)
        {
            if (InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateRxStatus);

                this.Invoke(d, new object[] { status });
            }
            else
            {
                textBoxRxStatus.Text = status;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                UpdateLocalData();
                SaveConfiguration("Configuration.xml");
            }
            catch
            {
                
            }
        }
        private void InitGuiData()
        {
            txtTimerMsec.Text = _configuration.timeTimeout.ToString();
            textBufferSize.Text = _byteArray.Length.ToString();
            textBoxAddress.Text = _configuration.serverIp;
            textBoxTTL.Text = _configuration.ttl.ToString();
            textBoxPortNum.Text = _configuration.portNum.ToString();
            checkBoxTimerEnable.Checked = _configuration.timerEnable;
           


            IPAddress[] iPAddressList = Dns.GetHostEntry(String.Empty).AddressList;
            foreach (IPAddress ipadr in iPAddressList)
            {
                if (ipadr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    string ip = ipadr.ToString();
                    cmbLocalIpAddress.Items.Add(ip);
                }
            }
            cmbLocalIpAddress.SelectedIndex = 0;
            chkBindIp.Checked = _configuration.bindFlag;
            
            chkRxData.Checked = _configuration.dataDisplayFlag;
            _totalArived = 0;
            cmbType.SelectedIndex = _configuration.selectedProtocol;
            txtTimerMsec.Text = _configuration.timeTimeout.ToString();
            chkAscii.Checked = _configuration.asciiFlag;
            chkRecord.Checked = _configuration.recordFlag;
            timer1.Enabled = true;
            textBoxData.Text = _configuration.txBuffer;
        }

  

    
        private void btnSend_Click(object sender, EventArgs e)
        {
            string Status;
            List<byte> byteList = new List<byte>();
          
            char[] charSeperator = new char[] { ' ' };
            string s1 = textBoxData.Text;
            string[] stingArray = s1.Split(charSeperator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string letter in stingArray)
            {
                try
                {
                    byte value = Convert.ToByte(letter);
                    byteList.Add(value);
                }
                catch (Exception ex) 
                { 
                    Trace.WriteLine(ex.ToString()); 
                }
            }
            int byteSize = byteList.Count;
            textBufferSize.Text = byteSize.ToString();

            if (Int32.TryParse(textBufferSize.Text, out byteSize))
            {
                if ((byteSize > byteList.Count) && (byteSize < 64000))
                {
                    int addLength = byteSize - byteList.Count;
                    byte[] addBytes = new byte[addLength];
                    byteList.AddRange(addBytes);
                }
            }
            if (chkForceSize.Checked)
            {
                int forceSize = 0;
                int.TryParse(txtForceSize.Text, out forceSize);
                int diff = forceSize - byteList.Count;
                if (diff > 0)
                {
                    byte [] d = new byte[diff];
                    byteList.AddRange(d);
                }
            }
            _byteArray = byteList.ToArray();
            if (_portTx != null)
            {
                _portTx.Write(_byteArray, 0, _byteArray.Length, out Status);
                _totalSend = _totalSend + _byteArray.Length;
                _txCount++;
                textBoxTxStatus.Text = "Msg " + _txCount + ", Total  " + _totalSend;
            }
           
            else
            {
                textBoxTxStatus.Text = "Connect First";
            }
        }

        

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            CommunicationDisconnect();
        }
      

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBoxTimerEnable.Checked)
            {
                if (_portTx != null)
                {
                    string status;
                    _portTx.Write(_byteArray, 0, _byteArray.Length, out status);
                    _txCount++;
                    _totalSend = _totalSend + _byteArray.Length;
                    DateTime currentTime = DateTime.Now;
                    TimeSpan ts = currentTime - _lastUpdate;
                    if (ts.TotalMilliseconds > 1000)
                    {
                        _lastUpdate = currentTime;
                        textBoxTxStatus.Text = "Msg " + _txCount + ", Total  " + _totalSend;
                    }
                    
                }
                else
                {
                    textBoxTxStatus.Text = "Connect First";
                }
            }
            if (_portRx != null)
            {
                if (!_portRx.Connected)
                {
                    textBoxConnection.Text = "Connect First";
                }
            }
        }
        private void chkRxData_Click(object sender, EventArgs e)
        {
            _configuration.dataDisplayFlag = chkRxData.Checked;

        }
        public void SaveConfiguration(string name)
        {
            XmlUtility.Serialize(_configuration, name);
        }
        public void LoadConfiguration(string name)
        {
            var conf = (Configuration)XmlUtility.Deserialize(_configuration.GetType(), name);
            if (conf != null)
                _configuration = conf;
        }

        private void checkBoxTimerEnable_CheckedChanged(object sender, EventArgs e)
        {
            _configuration.timeTimeout = Convert.ToInt32(txtTimerMsec.Text);
            timer1.Stop();
            timer1.Interval = _configuration.timeTimeout;
            timer1.Start();
        }

        private void chkRecord_CheckedChanged(object sender, EventArgs e)
        {
            _configuration.recordFlag = chkRecord.Checked;
        }

        private void chkAscii_CheckedChanged(object sender, EventArgs e)
        {
            _configuration.asciiFlag = chkAscii.Checked;
        }

        delegate void SetTextCallback(string str);

        
        private long _totalArived;
       
        private PortAbstraction _portRx;
        private BinaryWriter _binaryWriter;

        private PortAbstraction _portTx;
        int _Id = 0;

        byte[] _byteArray = new byte[67];
        private int _txCount;
        private int _totalSend;
        private int _rxCount;
        DateTime _lastUpdate = DateTime.Now;
        
       
        private Configuration _configuration;
         

   
    
    }
    public class Configuration
    {
        public string serverIp = "127.0.0.1";
        public int ttl = 32;
        public int portNum = 54000;
        public string localIpAddress = "127.0.0.1";
        public int selectedProtocol = 0;
        public bool asciiFlag = false;
        public bool recordFlag = false;
        public int timeTimeout = 1000;
        public bool dataDisplayFlag = false;
        public bool bindFlag = true;
        public bool timerEnable = false;
        public string txBuffer = "";
    }


}
