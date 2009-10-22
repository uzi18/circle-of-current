using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LogUtilities;

namespace NetUtilities
{
    public class NetTunnel
    {
        #region Fields

        public const int TIMEOUT = 10000;

        public int timeout = TIMEOUT;

        Socket socket;
        NetworkStream stream;

        Thread listenerThread;

        private volatile bool quitThread = false;
        private volatile bool hasQuit = false;
        private volatile bool disconnected = false;

        Queue<string> messages = new Queue<string>();

        ManualResetEvent clearToSend = new ManualResetEvent(true);

        private string identifier = "";
        private System.Windows.Forms.Control winFormObj;

        #endregion

        #region Properties

        /// <summary>
        /// Access the unhandled message queue
        /// </summary>
        public Queue<string> MessageQueue
        {
            get { return messages; }
        }

        /// <summary>
        /// Whether or not there are any unhandled messages
        /// </summary>
        public bool HasNewMessage
        {
            get { return messages.Count > 0; }
        }

        /// <summary>
        /// Returns the number of unhandled messages
        /// </summary>
        public int MessageCount
        {
            get { return messages.Count; }
        }

        /// <summary>
        /// Whether or not the socket is connected
        /// </summary>
        public bool Connected
        {
            get { return !hasQuit || !disconnected; }
        }

        /// <summary>
        /// A string used to identify this tunnel
        /// </summary>
        public string Identifier
        {
            get
            {
                if (string.IsNullOrEmpty(identifier))
                {
                    if (socket == null)
                    {
                        return "null tunnel";
                    }

                    try
                    {
                        return String.Format(
                            "Local: {0}, Remote: {1}",
                            socket.LocalEndPoint.ToString(),
                            socket.RemoteEndPoint.ToString()
                            );
                    }
                    catch
                    {
                        return "NetTunnel is Closed";
                    }
                }
                else
                {
                    if (socket == null)
                    {
                        return identifier + " (null tunnel)";
                    }
                    else
                        return identifier;
                }
            }

            set
            {
                identifier = value;
            }
        }

        /// <summary>
        /// Allows you to attach a form control which will reinvoke methods on its creator thread
        /// </summary>
        public System.Windows.Forms.Control Invoker
        {
            set { winFormObj = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// A NetTunnel is a wrapper to make sending and receiving messages easy
        /// this constructor uses the default server timeout
        /// this constructor establishes a connection to a server
        /// </summary>
        /// <param name="ip">ip of the server</param>
        /// <param name="port">port of the server</param>
        public NetTunnel(IPAddress ip, int port)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.socket.Connect(ip, port);
                Construct(socket, TIMEOUT);
            }
            catch (SocketException ex)
            {
                Logger.Log("Unable to establish connection to " + ip.ToString() + ":" + port.ToString("0"), ex);
                quitThread = true;
                hasQuit = true;
            }
        }

        /// <summary>
        /// A NetTunnel is a wrapper to make sending and receiving messages easy
        /// this constructor establishes a connection to a server
        /// </summary>
        /// <param name="ip">ip of the server</param>
        /// <param name="port">port of the server</param>
        /// <param name="timeout">custom timeout</param>
        public NetTunnel(IPAddress ip, int port, int timeout)
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                this.socket.Connect(ip, port);
                Construct(socket, timeout);
            }
            catch (SocketException ex)
            {
                Logger.Log("Unable to establish connection to " + ip.ToString() + ":" + port.ToString("0"), ex);
                quitThread = true;
                hasQuit = true;
            }
        }

        /// <summary>
        /// A NetTunnel is a wrapper to make sending and receiving messages easy
        /// this constructor uses the default server timeout
        /// </summary>
        /// <param name="socket">the remote socket</param>
        public NetTunnel(Socket socket)
        {
            Construct(socket, TIMEOUT);
        }

        /// <summary>
        /// A NetTunnel is a wrapper to make sending and receiving messages easy
        /// </summary>
        /// <param name="socket">the remote socket</param>
        /// <param name="timeout">timeout for sending/receiving</param>
        public NetTunnel(Socket socket, int timeout)
        {
            Construct(socket, timeout);
        }

        /// <summary>
        /// Constructs a null nettunnel
        /// </summary>
        public NetTunnel()
        {
            socket = null;
            stream = null;
        }

        /// <summary>
        /// Constructs the NetTunnel
        /// </summary>
        /// <param name="socket">socket to be used</param>
        /// <param name="timeout">timeout in milliseconds</param>
        private void Construct(Socket socket, int timeout)
        {
            this.socket = socket;
            this.stream = new NetworkStream(this.socket);
            this.timeout = timeout;
            this.stream.ReadTimeout = timeout;
            this.stream.WriteTimeout = timeout;

            this.OnMessageReceivedEvent += new OnMessageReceivedCall(DefaultMessageReceivedEvent);
            this.OnUnexpectedDisconnectEvent += new OnUnexpectedDisconnectCall(DefaultUnexpectedDisconnectEvent);

            listenerThread = new Thread(new ThreadStart(BackgroundListener));
            listenerThread.Priority = ThreadPriority.Lowest;
            listenerThread.IsBackground = true;
            listenerThread.Start();

            Logger.Log(LogType.Event, "NetTunnel created, " + this.ToString());
        }

        #endregion

        #region Delegates and Events

        /// <summary>
        /// Used to invoke the OnMessageReceived event so that windows form controls
        /// can be modified from within the event handler
        /// </summary>
        /// <param name="msg">the message received</param>
        private delegate void MessageReceivedInvoker(string msg);

        /// <summary>
        /// Delegate for message receieved event
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>return true if handled, if returned false then the message enters the message queue</returns>
        public delegate bool OnMessageReceivedCall(string msg);

        /// <summary>
        /// Raised when a message is received
        /// </summary>
        public event OnMessageReceivedCall OnMessageReceivedEvent;

        /// <summary>
        /// Delegate for when an unexpected disconnection occurs
        /// </summary>
        /// <param name="tunnel">the nettunnel that disconnected</param>
        public delegate void OnUnexpectedDisconnectCall(NetTunnel tunnel);

        /// <summary>
        /// Raised when a nettunnel disconnects undexpectedly
        /// </summary>
        public event OnUnexpectedDisconnectCall OnUnexpectedDisconnectEvent;

        #endregion

        #region Private Methods

        /// <summary>
        /// The background thread that checks for new messages
        /// </summary>
        private void BackgroundListener()
        {
            try
            {
                Logger.Log(LogType.Event, this.ToString() + ", listener thread started");
                hasQuit = false;
                while (quitThread == false)
                {
                    try
                    {
                        int msgLength = GetMessageLength();

                        if (msgLength > 0)
                        {
                            byte[] bArr = new byte[msgLength];
                            int readLen = stream.Read(bArr, 0, msgLength);
                            string msg = new UTF8Encoding().GetString(bArr);

                            if (readLen != msgLength)
                            {
                                Logger.Log(LogType.Error, "readLen (" + readLen.ToString("0") + ") != msgLenth (" + msgLength.ToString("0") + ") from " + this.ToString());
                            }
                            else
                            {
                                Logger.Log(LogType.CommRx, msg + " from " + Identifier);

                                InvokeMessageReceivedEvent(msg);
                            }
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        hasQuit = true;
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("From NetTunnel's BackgroundListener, " + this.ToString(), ex);
                    }

                    Thread.Sleep(50);
                }
                hasQuit = true;
                Logger.Log(LogType.Event, this.ToString() + ", listener thread ended");
            }
            catch (ThreadAbortException ex)
            {
                hasQuit = true;
                return;
            }
            catch (Exception ex)
            {
                Logger.Log("Thread Failure From NetTunnel's BackgroundListener, " + this.ToString(), ex);
            }
        }

        /// <summary>
        /// Used to invoke the OnMessageReceived event so that windows form controls
        /// can be modified from within the event handler
        /// </summary>
        /// <param name="msg">the message received</param>
        private void InvokeMessageReceivedEvent(string msg)
        {
            if (winFormObj == null)
            {
                if (OnMessageReceivedEvent(msg) == false)
                    messages.Enqueue(msg);
            }
            else
            {
                if (winFormObj.InvokeRequired)
                {
                    winFormObj.Invoke(new MessageReceivedInvoker(InvokeMessageReceivedEvent), new object[] { msg, });
                }
                else
                {
                    if (OnMessageReceivedEvent(msg) == false)
                        messages.Enqueue(msg);
                }
            }
        }

        /// <summary>
        /// Returns the length of the next message while catching then ignoring the nulls
        /// </summary>
        /// <returns>message length, or -1 if thread is exiting</returns>
        private int GetMessageLength()
        {
            if (quitThread)
                return -1;

            while (true)
            {
                if (stream.DataAvailable)
                {
                    int i = stream.ReadByte();
                    if (i > 0)
                        return i;
                }

                if (quitThread)
                    return -1;

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// the default handler for messages for when the event is not yet attached
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>always false as the message is unhandled</returns>
        private bool DefaultMessageReceivedEvent(string msg)
        {
            //Logger.Log(LogType.Debug, "NetTunnel's message received event is unattached, " + this.ToString());
            return false;
        }

        /// <summary>
        /// the default handler for unexpected disconnects, destroys the nettunnel
        /// </summary>
        /// <param name="tunnel">the nettunnel that disconnected</param>
        private void DefaultUnexpectedDisconnectEvent(NetTunnel tunnel)
        {
            //Logger.Log(LogType.Debug, "NetTunnel's unexpected disconnect event is unattached, " + this.ToString());
            tunnel.Destroy();
        }

        /// <summary>
        /// Called when a transmit has finished, used to signal clear to send
        /// </summary>
        /// <param name="ar"></param>
        private void FinishedSend(IAsyncResult ar)
        {
            clearToSend.Set();

            if (ar.IsCompleted == false)
            {
                Logger.Log(LogType.Error, "SendMessage did not complete, " + this.ToString());
            }
        }

        #endregion

        #region Public Methods

        public bool Ping()
        {
            try
            {
                if (disconnected)
                {
                    Logger.Log(LogType.Error, Identifier + " tried to ping while disconnected");
                    return false;
                }

                Logger.Log(LogType.CommTx, "Pinging " + this.ToString());
                if (stream != null)
                {
                    byte[] bArr = new byte[1];
                    bArr[0] = 0;
                    stream.BeginWrite(bArr, 0, bArr.Length, new AsyncCallback(FinishedSend), "ping");
                    clearToSend.WaitOne(this.timeout);
                    clearToSend.Reset();
                }
                return true;
            }
            catch (System.IO.IOException ex)
            {
                Logger.Log(LogType.Error, "IOException from NetTunnel's Ping," + " from " + Identifier);

                if (disconnected == false)
                {
                    disconnected = true;
                    OnUnexpectedDisconnectEvent(this);
                }

                disconnected = true;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log("From NetTunnel's Ping, " + this.ToString(), ex);
                return false;
            }
        }

        /// <summary>
        /// Sends a message as a string out the tunnel
        /// </summary>
        /// <param name="message">the message</param>
        /// <returns>true if successful</returns>
        public bool SendMessage(string message)
        {
            try
            {
                if (disconnected)
                {
                    Logger.Log(LogType.Error, Identifier + " tried to send " + message + " while disconnected");
                    return false;
                }

                Logger.Log(LogType.CommTx, message + " to " + Identifier);
                if (stream != null)
                {
                    byte[] bArr = new UTF8Encoding().GetBytes("  " + message);
                    bArr[0] = 0;
                    bArr[1] = Convert.ToByte(bArr.Length - 2);
                    clearToSend.WaitOne(this.timeout);
                    clearToSend.Reset();
                    stream.BeginWrite(bArr, 0, bArr.Length, new AsyncCallback(FinishedSend), message);
                }
                return true;
            }
            catch (System.IO.IOException ex)
            {
                Logger.Log(LogType.Error, "IOException from NetTunnel's SendMessage," + " from " + Identifier);

                if (disconnected == false)
                {
                    disconnected = true;
                    OnUnexpectedDisconnectEvent(this);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log("From NetTunnel's SendMessage, " + this.ToString(), ex);
                return false;
            }
        }

        /// <summary>
        /// Get an unhandled message from the queue
        /// </summary>
        /// <returns>an unhandled message, or null if none exists</returns>
        public string GetMessage()
        {
            if (messages.Count > 0)
                return messages.Dequeue();
            else
                return null;
        }

        /// <summary>
        /// Peek at an unhandled message from queue
        /// </summary>
        /// <returns>an unhandled message, or null if none exists</returns>
        public string PeekMessage()
        {
            if (messages.Count > 0)
                return messages.Peek();
            else
                return null;
        }

        /// <summary>
        /// Waits and gets an unhandled message within a timespan
        /// </summary>
        /// <param name="timeout">the timeout in milliseconds</param>
        /// <returns>an unhandled message, or null if timed out</returns>
        public string WaitMessage(int timeout)
        {
            string result = null;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            do
            {
                result = GetMessage();
                Thread.Sleep(50);
            }
            while (timer.ElapsedMilliseconds < timeout && result == null);
            return result;
        }

        /// <summary>
        /// Waits and gets an unhandled message using the default timeout
        /// </summary>
        /// <returns>an unhandled message, or null if timed out</returns>
        public string WaitMessage()
        {
            return WaitMessage(this.timeout);
        }

        /// <summary>
        /// Disconnect the socket
        /// </summary>
        public void Disconnect()
        {
            try
            {
                disconnected = true;

                if (socket != null)
                    socket.Close(this.timeout);
            }
            catch (Exception ex)
            {
                Logger.Log("From NetTunnel's Disconnect, " + this.ToString(), ex);
            }
        }

        /// <summary>
        /// End the listener thread
        /// </summary>
        public void KillThread()
        {
            try
            {
                Logger.Log(LogType.Event, "Killing NetTunnel thread, " + this.ToString());
                quitThread = true;

                listenerThread.Abort();
                hasQuit = true;

                return;
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception ex)
            {
                Logger.Log("From NetTunnel's KillThread, " + this.ToString(), ex);
            }
        }

        /// <summary>
        /// Distroy this net tunnel
        /// </summary>
        public void Destroy()
        {
            Logger.Log(LogType.Debug, "Destroying NetTunnel, " + this.ToString());

            KillThread();
            Disconnect();
        }

        public override string ToString()
        {
            return Identifier;
        }

        #endregion
    }
}
