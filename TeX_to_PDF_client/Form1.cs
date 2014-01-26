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
    public partial class Form1 : Form
    {
        private Socket mySocket;
        private Form obj;
        private string filename;
        private string path;
        
        delegate void setThreadedButtonCallback(bool status);
        delegate void buttonClickerCallback(int corobic);


        public Form1()
        {
            InitializeComponent();
            this.obj = this;
        }

        private void setThreadedButton(bool status)
        {
            if (this.button_ss.InvokeRequired)
            {
                setThreadedButtonCallback buttonCallback = new setThreadedButtonCallback(setThreadedButton);
                this.obj.Invoke(buttonCallback, status);
            }
            else
            {
                this.button_send.Enabled = status;
                this.button_ss.Enabled = status;
                this.button_get.Enabled = status;
                this.button_delete.Enabled = status;
                this.button_choose.Enabled = status;
            }
        }


        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                DataObject state = (DataObject)ar.AsyncState;
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

                    setThreadedButton(true);

                    /* shutdown and close socket */
                    socketFd.Shutdown(SocketShutdown.Both);
                    socketFd.Close();

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }




        private void buttonClicker(int corobic)
        {
            try
            {
                setThreadedButton(false);

                Socket socketFd = mySocket
                DataObject state = new DataObject();
                state.whatdo = corobic;
                state.m_SocketFd = socketFd;
                state.m_sent = 0;

                byte[] whatdo = BitConverter.GetBytes(state.whatdo);
                byte[] fileNameByte = Encoding.ASCII.GetBytes(filename);
                byte[] fileNameLength = BitConverter.GetBytes(fileNameByte.Length);

                /* Depending on request prepares suitable byte array to send */

                // Send specified file to server
                if (corobic==3 || corobic==4)
                {
                    //prepare data (option -> filenamesize -> filesize -> filename -> file)
                    byte[] fileData = File.ReadAllBytes(Path.Combine(this.path, this.filename));
                    byte[] fileDataLength = BitConverter.GetBytes(fileData.Length);
                    byte[] clientData = new byte[4 + 4 + 4 + fileNameByte.Length + fileData.Length];
                    whatdo.CopyTo(clientData, 0);
                    fileNameLength.CopyTo(clientData, 4);
                    fileDataLength.CopyTo(clientData, 8);
                    fileNameByte.CopyTo(clientData, 12);
                    fileData.CopyTo(clientData, 12 + fileNameByte.Length);

                    state.m_DataBuf = clientData;
                    state.m_data_size = clientData.Length;
                    //send it
                    socketFd.BeginSend(clientData, state.m_sent, clientData.Length - state.m_sent, 0, new AsyncCallback(SendCallback), state);
                }

                // Get/delete specified file from server
                else if (corobic == 5 || corobic == 6)
                {
                    //prepare data (option -> filenamesize -> filename)
                    byte[] clientData2 = new byte[4 + 4 + fileNameByte.Length];
                    whatdo.CopyTo(clientData2, 0);
                    fileNameLength.CopyTo(clientData2, 4);
                    fileNameByte.CopyTo(clientData2, 8);

                    state.m_DataBuf = clientData2;
                    state.m_data_size = clientData2.Length;
                    //send it
                    socketFd.BeginSend(clientData, state.m_sent, clientData.Length - state.m_sent, 0, new AsyncCallback(SendCallback), state);
                }
                else { MessageBox.Show("Unspecified error!"); }
                



                //Dns.BeginGetHostEntry(this.textBox_ip.Text.ToString(), new AsyncCallback(GetHostEntryCallback), state);
                setThreadedButton(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show("1Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }


        
        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "zip files (*.zip)|*.zip";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            this.path = Path.GetDirectoryName(openFileDialog1.FileName.ToString());
                            this.filename = Path.GetFileName(openFileDialog1.FileName.ToString());
                            this.textBox_filename.Text = this.filename;
                            this.textBox_path.Text = this.path;
                            this.button_send.Enabled = true;
                            this.button_ss.Enabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button_ss_Click(object sender, EventArgs e)
        {
            int a = 3;
            buttonClicker(a);
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            int a = 4;
            buttonClicker(a);
        }

        private void button_get_Click(object sender, EventArgs e)
        {
            int a = 5;
            buttonClicker(a);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            int a = 6;
            buttonClicker(a);
        }
        

    }

    public class DataObject
    {
        public int m_data_size;
        public int m_sent;
        public byte[] m_DataBuf;
        public Socket m_SocketFd = null;
        public int whatdo;
    }
}
