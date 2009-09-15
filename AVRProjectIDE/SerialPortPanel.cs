using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace AVRProjectIDE
{
    public partial class SerialPortPanel : UserControl
    {
        public SerialPortPanel()
        {
            InitializeControl();
        }

        public SerialPortPanel(string preferedPortName, int preferedBaudRate)
        {
            InitializeControl();
            SetPreferences(preferedPortName, preferedBaudRate);
        }

        private void InitializeControl()
        {
            InitializeComponent();

            string[] portList = SerialPort.GetPortNames();
            dropPorts.Items.Clear();
            foreach (string portName in portList)
            {
                dropPorts.Items.Add(portName);
            }

            if (dropPorts.Items.Count <= 0)
            {
                this.Enabled = false;
            }
            else
            {
                dropPorts.SelectedIndex = 0;
            }

            dropBaud.SelectedIndex = 5;

            Disconnect();

            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control;
            txtTx.MaxLength = serialPort1.WriteBufferSize;

            textboxBuffer = "";

            StartThread();
        }

        #region Public Methods and Properties

        public string CurrentPort
        {
            get { return (string)dropPorts.Items[dropPorts.SelectedIndex]; }
        }

        public int CurrentBaud
        {
            get { return int.Parse((string)dropBaud.Items[dropBaud.SelectedIndex]); }
        }

        public bool IsConnected
        {
            get
            {
                bool isOpen = false;
                try
                {
                    isOpen = serialPort1.IsOpen;
                }
                catch (Exception ex)
                {
                    RaiseException(ex);
                }
                return isOpen;
            }
        }

        public event SerialPortErrorHandler SerialPortException;
        public delegate void SerialPortErrorHandler(Exception ex);

        public void SetPreferences(string preferedPortName, int preferedBaudRate)
        {
            for (int i = 0; i < dropPorts.Items.Count; i++)
            {
                if (((string)dropPorts.Items[i]).ToLower().Trim() == preferedPortName.ToLower().Trim())
                {
                    dropPorts.SelectedIndex = i;
                    break;
                }
            }

            for (int i = 0; i < dropBaud.Items.Count; i++)
            {
                if (int.Parse(((string)dropBaud.Items[i]).Trim()) == preferedBaudRate)
                {
                    dropBaud.SelectedIndex = i;
                    break;
                }
            }
        }

        public void ClearRxTextBox()
        {
            txtRx.Text = "";
            textboxBuffer = "";
        }

        public void Disconnect()
        {
            while (bgTxWorker.IsBusy) ;

            try
            {
                serialPort1.Close();
            }
            catch (Exception ex)
            {
                RaiseException(ex);
            }
            btnConnect.Text = "Connect";
            btnSend.Enabled = false;
            dropPorts.Enabled = true;
            dropBaud.Enabled = true;
            barSerPortTick.Visible = false;
            pendingDisconnect = false;
            btnConnect.Enabled = true;
        }

        public void Connect()
        {
            try
            {
                serialPort1.PortName = (string)dropPorts.Items[dropPorts.SelectedIndex];
                serialPort1.BaudRate = int.Parse((string)dropBaud.Items[dropBaud.SelectedIndex]);
                serialPort1.Open();
                barRxStatus.Maximum = serialPort1.ReadBufferSize;
                btnConnect.Text = "Disonnect";
                btnSend.Enabled = true;
                dropPorts.Enabled = false;
                dropBaud.Enabled = false;
                attemptedDisconnect = false;
            }
            catch (Exception ex)
            {
                RaiseException(ex);
            }
        }

        public void StartThread()
        {
            if (bgRxWorker.WorkerSupportsCancellation)
            {
                if (bgRxWorker.CancellationPending && bgRxWorker.IsBusy)
                {
                    while (bgRxWorker.IsBusy) ;
                }
            }

            if (bgRxWorker.IsBusy == false)
            {
                bgRxWorker.RunWorkerAsync();
            }

            timerStatusChecker.Enabled = true;
        }

        public void KillThread()
        {
            if (bgRxWorker.WorkerSupportsCancellation)
            {
                bgRxWorker.CancelAsync();
            }
        }

        public void Send(string text)
        {
            if (serialPort1.IsOpen && bgTxWorker.IsBusy == false)
            {
                bgTxWorker.RunWorkerAsync(text);
            }
        }

        #endregion

        #region Async Form Control Modification

        private delegate void SetProgBarCallback(int bytesToRead);

        private void SetRxBufferStatus(int bytesToRead)
        {
            if (InvokeRequired)
            {
                Invoke(new SetProgBarCallback(SetRxBufferStatus), new object[] { bytesToRead, });
            }
            else
            {
                barRxStatus.Value = bytesToRead;

                bool isOpen = false;
                try
                {
                    isOpen = serialPort1.IsOpen;
                }
                catch (Exception ex)
                {
                    RaiseException(ex);
                }

                if (isOpen)
                {
                    if (bytesToRead != 0)
                    {
                        barSerPortTick.Visible = false;
                    }
                    else
                    {
                        barSerPortTick.Visible = true;
                    }
                }

                if (bytesToRead > (serialPort1.ReadBufferSize * 3) / 4)
                {
                    Disconnect();
                    MessageBox.Show("Serial Port was Closed Due to Massive Amount of Traffic");
                }
            }
        }

        private string textboxBuffer;

        private delegate void AppendToRxCallback(string text);

        private void AppendToRxTxt(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new AppendToRxCallback(AppendToRxTxt), new object[] { text, });
            }
            else
            {
                textboxBuffer += text;
            }
        }

        private void timerTextBoxUpdater_Tick(object sender, EventArgs e)
        {
            txtRx.Text = textboxBuffer;
            if (txtRx.SelectionLength < 2)
            {
                txtRx.Select(txtRx.Text.Length, 0);
                txtRx.ScrollToCaret();
            }
        }

        private void RaiseException(Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new SerialPortErrorHandler(RaiseException), new object[] { ex, });
            }
            else
            {
                SerialPortException(ex);
            }
        }

        #endregion

        #region Status Related

        private bool attemptedDisconnect = false;
        private bool pendingDisconnect = false;

        private void timerStatusChecker_Tick(object sender, EventArgs e)
        {
            bool isOpen = false;
            try
            {
                isOpen = serialPort1.IsOpen;
            }
            catch (Exception ex)
            {
                RaiseException(ex);
            }

            if (isOpen)
            {
                btnConnect.Text = "Disconnect";
                btnSend.Enabled = true;
                dropPorts.Enabled = false;
                dropBaud.Enabled = false;
                attemptedDisconnect = false;
            }
            else
            {
                btnConnect.Text = "Connect";
                btnSend.Enabled = false;
                dropPorts.Enabled = true;
                dropBaud.Enabled = true;
                barSerPortTick.Visible = false;

                if (attemptedDisconnect == false)
                {
                    attemptedDisconnect = true;
                    try
                    {
                        serialPort1.Close();
                    }
                    catch (Exception ex)
                    {
                        RaiseException(ex);
                    }
                }

                pendingDisconnect = false;
            }

            string fromTxt = txtRx.Text;
            if (fromTxt.Length > txtRx.MaxLength / 2)
            {
                fromTxt = fromTxt.Substring(txtRx.MaxLength / 2);
                txtRx.Text = fromTxt;
                txtRx.Select(fromTxt.Length, 0);
                txtRx.ScrollToCaret();
            }

            if (pendingDisconnect)
            {
                if (serialPort1.IsOpen)
                {
                    if (bgTxWorker.IsBusy || serialPort1.BytesToRead > 0)
                    {
                        pendingDisconnect = true;
                        btnConnect.Enabled = false;
                        btnSend.Enabled = false;
                        return;
                    }

                    Disconnect();
                }
                else
                {
                    btnConnect.Enabled = true;
                    pendingDisconnect = false;
                }
            }
            else
            {
                btnConnect.Enabled = true;
            }
        }

        #endregion

        #region Button Press Events

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (bgTxWorker.IsBusy || serialPort1.BytesToRead > 0)
                {
                    pendingDisconnect = true;
                    btnConnect.Enabled = false;
                    btnSend.Enabled = false;
                    return;
                }

                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearRxTextBox();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send(txtTx.Text);
        }

        private void txtTx_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && pendingDisconnect == false && serialPort1.IsOpen && btnSend.Enabled)
                Send(txtTx.Text);
        }

        #endregion

        #region Background Workers

        private void backgroundRxWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] bArr = new byte[2048];
            int bCnt;

            while (bgRxWorker.CancellationPending == false)
            {
                string resStr = "";
                bCnt = 0;

                bool isOpen = false;
                try
                {
                    isOpen = serialPort1.IsOpen;
                }
                catch (Exception ex)
                {
                    RaiseException(ex);
                }

                if (isOpen)
                {
                    try
                    {
                        bCnt = serialPort1.BytesToRead;
                        SetRxBufferStatus(bCnt);

                        Thread.Sleep(0);

                        if (bCnt > 0)
                        {
                            bCnt = serialPort1.Read(bArr, 0, 1024);
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseException(ex);
                    }
                }
                else
                {
                    Thread.Sleep(250);
                }

                for (int i = 0; i < bCnt && i < 1024; i++)
                {
                    if (bArr[i] == '\r' || bArr[i] == '\n' || bArr[i] == '\t' || (bArr[i] >= 0x20 && bArr[i] <= 0x7E))
                    {
                        resStr += Convert.ToChar(bArr[i]);
                    }
                    else if (bArr[i] == '\v')
                    {
                        resStr += "\r\n\r\n\r\n";
                    }
                    else
                    {
                        resStr += "{\\x" + Convert.ToString(bArr[i], 16).ToUpper() +"}";
                    }
                }

                if (bCnt > 0)
                {
                    AppendToRxTxt(resStr);
                    Thread.Sleep(0);
                }
                else
                {
                    SetRxBufferStatus(bCnt);
                    Thread.Sleep(100);
                }
            }
        }

        private void bgTxWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bArr = encoding.GetBytes((string)(e.Argument));

                Queue<byte> bQueue = new Queue<byte>();
                for (int i = 0; i < bArr.Length; i++)
                {
                    if (Convert.ToChar(bArr[i]) == '\\')
                    {
                        if (i < bArr.Length - 1)
                        {
                            char nextChar = Convert.ToChar(bArr[i + 1]);
                            if (nextChar == 'x' && i + 3 < bArr.Length)
                            {
                                int hexRes = -1;
                                string hexStr = "0x" + Convert.ToChar(bArr[i + 2]) + Convert.ToChar(bArr[i + 3]);
                                try
                                {
                                    hexRes = Convert.ToInt32(hexStr, 16);
                                }
                                catch
                                {
                                }
                                if (hexRes >= 0)
                                {
                                    bQueue.Enqueue(Convert.ToByte(hexRes));
                                    i += 3;
                                }
                                else
                                {
                                    bQueue.Enqueue(Convert.ToByte('\\'));
                                }
                            }
                            else
                            {
                                if (nextChar == '\\')
                                    bQueue.Enqueue(Convert.ToByte('\\'));
                                else if (nextChar == 'r')
                                    bQueue.Enqueue(Convert.ToByte('\r'));
                                else if (nextChar == 'n')
                                    bQueue.Enqueue(Convert.ToByte('\n'));
                                else if (nextChar == 'a')
                                    bQueue.Enqueue(Convert.ToByte('\a'));
                                else if (nextChar == 'b')
                                    bQueue.Enqueue(Convert.ToByte('\b'));
                                else if (nextChar == 't')
                                    bQueue.Enqueue(Convert.ToByte('\t'));
                                else if (nextChar == 'v')
                                    bQueue.Enqueue(Convert.ToByte('\v'));
                                else
                                {
                                    bQueue.Enqueue(Convert.ToByte('\\'));
                                    i--;
                                }
                                i++;
                            }
                        }
                        else
                        {
                            bQueue.Enqueue(bArr[i]);
                        }
                    }
                    else
                    {
                        bQueue.Enqueue(bArr[i]);
                    }
                }

                int cnt;
                for (cnt = 0; bQueue.Count > 0; cnt++)
                {
                    bArr[cnt] = bQueue.Dequeue();
                }

                e.Result = bArr;

                if (serialPort1.IsOpen)
                {
                    serialPort1.Write(bArr, 0, cnt);
                }
            }
            catch (Exception ex)
            {
                RaiseException(ex);
            }
        }

        #endregion
    }
}
