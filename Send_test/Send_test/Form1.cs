using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Send_test
{
    public partial class Form1 : Form
    {
        private Form obj;
        private Socket mySock;
        delegate void setThreadedButtonCallback(bool status);
        delegate void setThreadedLabelCallback(string napis);

        public Form1()
        {
            InitializeComponent();
            this.obj = this;
            this.mySock = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    return;
                case DialogResult.Yes:
                    if (mySock != null)
                    {
                        mySock.Shutdown(SocketShutdown.Both);
                        mySock.Close();
                    }
                    return;
                default:
                    return;
            }
        }



        private void setThreadedButton(bool status)
        {
            if (this.button_send.InvokeRequired)
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, status);
            }
            else
            {
                this.button_send.Enabled = status;
            }
        }

        private void setThreadedLabel(string napis)
        {
            if (this.label3.InvokeRequired)
            {
                setThreadedLabelCallback buttonCallback = new setThreadedLabelCallback(setThreadedLabel);
                this.obj.Invoke(buttonCallback, napis);
            }
            else
            {
                this.label3.Text = napis;
            }
        }



        // OBSLUGA POLACZENIA (callbacks)
        //======================================================================

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SendStateObject state = (SendStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* sent data */
                int size = socketFd.EndSend(ar);
                state.m_sent = state.m_sent + size;

                if (state.m_sent < state.m_data_size)
                {
                    /* send the rest of the data */
                    socketFd.BeginSend(state.m_DataBuf, state.m_sent, state.m_data_size-state.m_sent, 0, new AsyncCallback(SendCallback), state);
                }
                else
                {
                    /* all the data has arrived */

                        setThreadedButton(true);

                    
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }


        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                //otwarcie na sucho wpisanego pliku i zamiana go na tablice byte
                string fileName = "myzip.zip";
                //string fileName = this.textBox_filename.Text.ToString();
                string filePath = "C://Users/Lukashoo/Desktop/Sieci projekt/";
                //string filePath = this.textBox_path.Text.ToString();
                byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);
                setThreadedLabel(Path.Combine(filePath, fileName));
                byte[] fileData = File.ReadAllBytes(Path.Combine(filePath, fileName));
                MessageBox.Show(fileData.Length.ToString());
                byte[] clientData = new byte[4 + 4 + fileNameByte.Length + fileData.Length];
                byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);
                byte[] fileDataLen = BitConverter.GetBytes(fileData.Length);
                fileNameLen.CopyTo(clientData, 0);
                fileDataLen.CopyTo(clientData, 4);
                fileNameByte.CopyTo(clientData, 8);
                fileData.CopyTo(clientData, 8 + fileNameByte.Length);



                /* retrieve the socket from the state object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);

                /* create the SocketStateObject */
                SendStateObject state = new SendStateObject();
                state.m_SocketFd = socketFd;
                state.m_DataBuf = clientData;
                state.m_data_size = clientData.Length;
                state.m_sent = 0;

               // byte[] byteData = Encoding.ASCII.GetBytes(this.textBoxAddr.Text.ToString());
                /* begin sending the data */
                //socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
               socketFd.BeginSend(clientData, state.m_sent, clientData.Length-state.m_sent, 0, new AsyncCallback(SendCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }


        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.mySock = socketFd;

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[1], Int32.Parse(this.textBox_port.Text.ToString()));


                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
                setThreadedButton(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }

        // ================================================================
        
        //OBSLUGA EVENTOW

        private void button_send_Click(object sender, EventArgs e)
        {
            try
            {
                setThreadedButton(false);

                if (this.textBox_ip.Text.Length > 0 && this.textBox_port.Text.Length > 0)
                {
                    /* get DNS host information */
                    Dns.BeginGetHostEntry(this.textBox_ip.Text.ToString(), new AsyncCallback(GetHostEntryCallback), null);
                }
                else
                {
                    if (this.textBox_ip.Text.Length <= 0) MessageBox.Show("No server address!");
                    else
                        if (this.textBox_port.Text.Length <= 0) MessageBox.Show("No server port number!");
                    setThreadedButton(true);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }

    }

    public class SendStateObject
    {
        public int m_data_size;
        public int m_sent;
        public byte[] m_DataBuf;
        public Socket m_SocketFd = null;
    }

}
