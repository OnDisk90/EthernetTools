namespace MultiCastSend
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.textBoxRxStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textBoxPortNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxData = new System.Windows.Forms.TextBox();
            this.textBoxTxStatus = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxConnection = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.checkBoxTimerEnable = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBufferSize = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.cmbLocalIpAddress = new System.Windows.Forms.ComboBox();
            this.chkBindIp = new System.Windows.Forms.CheckBox();
            this.txRxData = new System.Windows.Forms.TextBox();
            this.chkRxData = new System.Windows.Forms.CheckBox();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtTimerMsec = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxTTL = new System.Windows.Forms.TextBox();
            this.txtForceSize = new System.Windows.Forms.TextBox();
            this.chkForceSize = new System.Windows.Forms.CheckBox();
            this.chkAscii = new System.Windows.Forms.CheckBox();
            this.chkRecord = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Location = new System.Drawing.Point(125, 10);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.Size = new System.Drawing.Size(164, 20);
            this.textBoxAddress.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address";
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(196, 505);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // textBoxRxStatus
            // 
            this.textBoxRxStatus.Location = new System.Drawing.Point(114, 390);
            this.textBoxRxStatus.Name = "textBoxRxStatus";
            this.textBoxRxStatus.ReadOnly = true;
            this.textBoxRxStatus.Size = new System.Drawing.Size(271, 20);
            this.textBoxRxStatus.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 393);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "RX Status";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(24, 505);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 6;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textBoxPortNum
            // 
            this.textBoxPortNum.Location = new System.Drawing.Point(125, 35);
            this.textBoxPortNum.Name = "textBoxPortNum";
            this.textBoxPortNum.Size = new System.Drawing.Size(164, 20);
            this.textBoxPortNum.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Tx Data Buffer";
            // 
            // textBoxData
            // 
            this.textBoxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxData.Location = new System.Drawing.Point(117, 92);
            this.textBoxData.Multiline = true;
            this.textBoxData.Name = "textBoxData";
            this.textBoxData.Size = new System.Drawing.Size(381, 69);
            this.textBoxData.TabIndex = 10;
            // 
            // textBoxTxStatus
            // 
            this.textBoxTxStatus.Location = new System.Drawing.Point(114, 416);
            this.textBoxTxStatus.Name = "textBoxTxStatus";
            this.textBoxTxStatus.ReadOnly = true;
            this.textBoxTxStatus.Size = new System.Drawing.Size(271, 20);
            this.textBoxTxStatus.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 419);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "TX Status";
            // 
            // textBoxConnection
            // 
            this.textBoxConnection.Location = new System.Drawing.Point(114, 442);
            this.textBoxConnection.Name = "textBoxConnection";
            this.textBoxConnection.ReadOnly = true;
            this.textBoxConnection.Size = new System.Drawing.Size(271, 20);
            this.textBoxConnection.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 445);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Connection";
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(105, 505);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 15;
            this.buttonDisconnect.Text = "DisConnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // checkBoxTimerEnable
            // 
            this.checkBoxTimerEnable.AutoSize = true;
            this.checkBoxTimerEnable.Location = new System.Drawing.Point(7, 66);
            this.checkBoxTimerEnable.Name = "checkBoxTimerEnable";
            this.checkBoxTimerEnable.Size = new System.Drawing.Size(106, 17);
            this.checkBoxTimerEnable.TabIndex = 3;
            this.checkBoxTimerEnable.Text = "Auto Send mSec";
            this.checkBoxTimerEnable.UseVisualStyleBackColor = true;
            this.checkBoxTimerEnable.CheckedChanged += new System.EventHandler(this.checkBoxTimerEnable_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBufferSize
            // 
            this.textBufferSize.Location = new System.Drawing.Point(116, 360);
            this.textBufferSize.Name = "textBufferSize";
            this.textBufferSize.ReadOnly = true;
            this.textBufferSize.Size = new System.Drawing.Size(53, 20);
            this.textBufferSize.TabIndex = 73;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(9, 363);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(58, 13);
            this.label35.TabIndex = 74;
            this.label35.Text = "Buffer Size";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(9, 473);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(41, 13);
            this.label36.TabIndex = 75;
            this.label36.Text = "local Ip";
            // 
            // cmbLocalIpAddress
            // 
            this.cmbLocalIpAddress.FormattingEnabled = true;
            this.cmbLocalIpAddress.Location = new System.Drawing.Point(114, 469);
            this.cmbLocalIpAddress.Name = "cmbLocalIpAddress";
            this.cmbLocalIpAddress.Size = new System.Drawing.Size(121, 21);
            this.cmbLocalIpAddress.TabIndex = 76;
            // 
            // chkBindIp
            // 
            this.chkBindIp.AutoSize = true;
            this.chkBindIp.Location = new System.Drawing.Point(251, 473);
            this.chkBindIp.Name = "chkBindIp";
            this.chkBindIp.Size = new System.Drawing.Size(60, 17);
            this.chkBindIp.TabIndex = 77;
            this.chkBindIp.Text = "Bind IP";
            this.chkBindIp.UseVisualStyleBackColor = true;
            // 
            // txRxData
            // 
            this.txRxData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txRxData.Location = new System.Drawing.Point(10, 192);
            this.txRxData.Multiline = true;
            this.txRxData.Name = "txRxData";
            this.txRxData.ReadOnly = true;
            this.txRxData.Size = new System.Drawing.Size(488, 146);
            this.txRxData.TabIndex = 78;
            // 
            // chkRxData
            // 
            this.chkRxData.AutoSize = true;
            this.chkRxData.Location = new System.Drawing.Point(7, 169);
            this.chkRxData.Name = "chkRxData";
            this.chkRxData.Size = new System.Drawing.Size(65, 17);
            this.chkRxData.TabIndex = 79;
            this.chkRxData.Text = "Rx Data";
            this.chkRxData.UseVisualStyleBackColor = true;
            this.chkRxData.Click += new System.EventHandler(this.chkRxData_Click);
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "UDP Multicast",
            "UDP Server",
            "UDP Client",
            "TCP Server",
            "TCP Client"});
            this.cmbType.Location = new System.Drawing.Point(325, 10);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(121, 21);
            this.cmbType.TabIndex = 80;
            // 
            // txtTimerMsec
            // 
            this.txtTimerMsec.Location = new System.Drawing.Point(125, 64);
            this.txtTimerMsec.Name = "txtTimerMsec";
            this.txtTimerMsec.Size = new System.Drawing.Size(77, 20);
            this.txtTimerMsec.TabIndex = 81;
            this.txtTimerMsec.Text = "1000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(295, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(27, 13);
            this.label7.TabIndex = 82;
            this.label7.Text = "TTL";
            // 
            // textBoxTTL
            // 
            this.textBoxTTL.Location = new System.Drawing.Point(325, 41);
            this.textBoxTTL.Name = "textBoxTTL";
            this.textBoxTTL.Size = new System.Drawing.Size(121, 20);
            this.textBoxTTL.TabIndex = 83;
            // 
            // txtForceSize
            // 
            this.txtForceSize.Location = new System.Drawing.Point(325, 67);
            this.txtForceSize.Name = "txtForceSize";
            this.txtForceSize.Size = new System.Drawing.Size(77, 20);
            this.txtForceSize.TabIndex = 85;
            // 
            // chkForceSize
            // 
            this.chkForceSize.AutoSize = true;
            this.chkForceSize.Location = new System.Drawing.Point(216, 66);
            this.chkForceSize.Name = "chkForceSize";
            this.chkForceSize.Size = new System.Drawing.Size(74, 17);
            this.chkForceSize.TabIndex = 86;
            this.chkForceSize.Text = "Force size";
            this.chkForceSize.UseVisualStyleBackColor = true;
            // 
            // chkAscii
            // 
            this.chkAscii.AutoSize = true;
            this.chkAscii.Location = new System.Drawing.Point(7, 144);
            this.chkAscii.Name = "chkAscii";
            this.chkAscii.Size = new System.Drawing.Size(48, 17);
            this.chkAscii.TabIndex = 87;
            this.chkAscii.Text = "Ascii";
            this.chkAscii.UseVisualStyleBackColor = true;
            this.chkAscii.CheckedChanged += new System.EventHandler(this.chkAscii_CheckedChanged);
            // 
            // chkRecord
            // 
            this.chkRecord.AutoSize = true;
            this.chkRecord.Location = new System.Drawing.Point(7, 121);
            this.chkRecord.Name = "chkRecord";
            this.chkRecord.Size = new System.Drawing.Size(61, 17);
            this.chkRecord.TabIndex = 88;
            this.chkRecord.Text = "Record";
            this.chkRecord.UseVisualStyleBackColor = true;
            this.chkRecord.CheckedChanged += new System.EventHandler(this.chkRecord_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 542);
            this.Controls.Add(this.chkRecord);
            this.Controls.Add(this.chkAscii);
            this.Controls.Add(this.chkForceSize);
            this.Controls.Add(this.txtForceSize);
            this.Controls.Add(this.textBoxTTL);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtTimerMsec);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.chkRxData);
            this.Controls.Add(this.txRxData);
            this.Controls.Add(this.chkBindIp);
            this.Controls.Add(this.cmbLocalIpAddress);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.textBufferSize);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxConnection);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxTxStatus);
            this.Controls.Add(this.textBoxData);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPortNum);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxRxStatus);
            this.Controls.Add(this.checkBoxTimerEnable);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxAddress);
            this.Name = "Form1";
            this.Text = "Ethernet Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox textBoxRxStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPortNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxData;
        private System.Windows.Forms.TextBox textBoxTxStatus;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxConnection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.CheckBox checkBoxTimerEnable;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBufferSize;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.ComboBox cmbLocalIpAddress;
        private System.Windows.Forms.CheckBox chkBindIp;
        private System.Windows.Forms.TextBox txRxData;
        private System.Windows.Forms.CheckBox chkRxData;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TextBox txtTimerMsec;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxTTL;
        private System.Windows.Forms.TextBox txtForceSize;
        private System.Windows.Forms.CheckBox chkForceSize;
        private System.Windows.Forms.CheckBox chkAscii;
        private System.Windows.Forms.CheckBox chkRecord;
    }
}

