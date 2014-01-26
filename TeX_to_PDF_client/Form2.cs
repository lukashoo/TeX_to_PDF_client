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

namespace TeX_to_PDF_client
{
    public partial class Form2 : Form
    {
        private Socket mySocket;
        private int success;
        private Form obj;
        delegate void setThreadedButtonCallback(bool status);
        delegate void closeThreadedForm();

        // Constructor
        public Form2()
        {
            InitializeComponent();
            this.obj = this;
            this.success=0;

        }

        // Threaded close form call
        private void closeForm()
        {
            if (this.InvokeRequired)
            {
                closeThreadedForm closeCallback = new closeThreadedForm(closeForm);
                this.obj.Invoke(closeCallback);
            }
            else
            {
                this.Close();
            }
        }

        // Threaded button change status call
        private void setThreadedButton(bool status)
        {
            if ((this.button_login.InvokeRequired) || (this.button_register.InvokeRequired))
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, status);
            }
            else
            {
                this.button_login.Enabled = status;
                this.button_register.Enabled = status;
            }
        }

        // callback for Receive function
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                StateDataObject state = (StateDataObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* sent data */
                int size = socketFd.EndReceive(ar);
                state.m_sent = state.m_sent + size;

                /* if didnt send all data - continue sending */
                if (state.m_sent < state.m_data_size)
                {
                    /* receive the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                }
                /* else on success - logged in - close form. on Failure - messagebox */
                else
                {
                    /* all the data has arrived */
                    /* close the window if success, message error if error*/
                    if (BitConverter.ToInt32(state.m_DataBuf, 0) == 1)
                    { closeForm(); success=1; }
                    else if (BitConverter.ToInt32(state.m_DataBuf, 0) == 0) MessageBox.Show("Wrong username or password!");
                    else MessageBox.Show("Communication protocol error.");
                    

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("5Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }



        // callback for Send function
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                StateDataObject state = (StateDataObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* sent data */
                int size = socketFd.EndSend(ar);
                state.m_sent = state.m_sent + size;

                if (state.m_sent < state.m_data_size)
                {
                    /* send the rest of the data */
                    socketFd.BeginSend(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(SendCallback), state);
                }
                else
                {
                    /* all the data has arrived */
                    /* time to receive success/error */
                    state.m_sent=0;
                    byte [] receivedata=new byte[4];
                    state.m_DataBuf=receivedata;
                    state.m_data_size=4;
                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("4Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }



        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                /* retrieve the data from the state object */
                StateDataObject state = (StateDataObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* complete the connection */
                socketFd.EndConnect(ar);

                /* Prepare Data to send */
                string mystring = this.textBox_uname + "." + this.textBox_pass;
                byte[] whatdo = BitConverter.GetBytes(state.whatdo);
                byte[] mystringbyte = Encoding.ASCII.GetBytes(mystring);
                byte[] mystringLength=BitConverter.GetBytes(mystringbyte.Length);
                byte[] senddata = new byte[4 + 4 + mystringbyte.Length];
                whatdo.CopyTo(senddata, 0);
                mystringLength.CopyTo(senddata, 4);
                mystringbyte.CopyTo(senddata, 8);
                state.m_DataBuf = senddata;
                state.m_data_size = senddata.Length;
                state.m_sent = 0;

                /* begin sending the data */
                socketFd.BeginSend(senddata, state.m_sent, senddata.Length - state.m_sent, 0, new AsyncCallback(SendCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("3Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }



        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the data from the state object */
                StateDataObject state = (StateDataObject)ar.AsyncState;

                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                state.m_SocketFd = socketFd;
                this.mySocket = socketFd;
                

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(this.textBox_port.Text.ToString()));


                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), state);
                setThreadedButton(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show("2Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }



        // Funkcja zajmujaca sie obsluga przycisku
        private void buttonClicker(int corobic)
        {
            try
            {
                setThreadedButton(false);

                if (this.textBox_ip.Text.Length > 0 && this.textBox_port.Text.Length > 0 && this.textBox_uname.Text.Length > 0 && this.textBox_pass.Text.Length > 0)
                {
                    /* get DNS host information */
                    StateDataObject state = new StateDataObject();
                    if (corobic == 1) { state.whatdo = 1; } else { state.whatdo = 2; }
                    Dns.BeginGetHostEntry(this.textBox_ip.Text.ToString(), new AsyncCallback(GetHostEntryCallback), state);
                }
                else
                {
                    if (this.textBox_ip.Text.Length <= 0) MessageBox.Show("No server address!");
                    else
                        if (this.textBox_port.Text.Length <= 0) MessageBox.Show("No server port number!");
                        else
                            if (this.textBox_uname.Text.Length <= 0) MessageBox.Show("No username!");
                            else
                                if (this.textBox_pass.Text.Length <= 0) MessageBox.Show("No password!");
                    setThreadedButton(true);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("1Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }

        // return socket (needs to be used by another form for connecting)
        public Socket getSocket()
        {
            return mySocket;
        }

        // return if logging in was successful. (determines if we should open another Form)
        public int getSuccess()
        {
            return success;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            int a = 1;
            buttonClicker(a);
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            int a = 2;
            buttonClicker(a);
        }
    }
    public class StateDataObject
    {
        public int m_data_size;
        public int m_sent;
        public byte[] m_DataBuf;
        public Socket m_SocketFd = null;
        public int whatdo;
    }
}
