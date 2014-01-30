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
        delegate void SaveFileCallback(DataObject state);


        public Form1(Socket socketFd)
        {
            InitializeComponent();
            this.obj = this;
            this.mySocket = socketFd;
        }

        // Changed on close functionality (have to close socket and send to server information that we're disconnecting)
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            switch (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo))
            {
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    int a = 7;
                    byte[] fatality = BitConverter.GetBytes(a);
                    mySocket.Send(fatality, 0, 4, 0);
                    System.Threading.Thread.Sleep(100);
                    mySocket.Shutdown(SocketShutdown.Both);
                    mySocket.Close();
                    break;
            }
        }

        // Threaded save file dialog
        private void saveThreadedFile(DataObject state)
        {
            if (this.button_ss.InvokeRequired)
            {
                SaveFileCallback saveCallback = new SaveFileCallback(saveThreadedFile);
                this.obj.Invoke(saveCallback, state);
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = "unknown.pdf";

                saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //File.WriteAllBytes(saveFileDialog1.FileName, state.m_DataBuf);
                    BinaryWriter bWrite = new BinaryWriter(File.Open(saveFileDialog1.FileName, FileMode.OpenOrCreate));
                    bWrite.Write(state.m_DataBuf, 0, state.m_data_size);
                    bWrite.Flush();
                    bWrite.Close();
                }
                setThreadedButton(true);
            }
        }







        // Threaded button state setter
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


        // Callback for Receive function
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                DataObject state = (DataObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* sent data */
                int size = socketFd.EndReceive(ar);
                state.m_sent = state.m_sent + size;

                if (state.m_sent < state.m_data_size)
                {
                    /* receive the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // got success/failure or filesize
                    if (state.m_data_size == 4)
                    {
                        state.m_sent=0;
                        switch (state.whatdo)
                        {
                            // If success: next 4 bytes: size of file. If failure - MessageBox.
                            case 3:
                                // success
                                if (BitConverter.ToInt32(state.m_DataBuf, 0) == 1)
                                {
                                     // read the filesize
                                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                                }
                                // failure
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) == 0)
                                { MessageBox.Show("Couldnt convert file."); setThreadedButton(true); }
                                // got filesize
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) > 1)
                                {
                                    //prepare data buffer and set m_filesize
                                    state.m_data_size=BitConverter.ToInt32(state.m_DataBuf, 0);
                                    byte[] dataBuffer = new byte[state.m_data_size];
                                    state.m_DataBuf = dataBuffer;
                                    // receive the file
                                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);

                                }
                                break;


                            // Show MessageBox: success/failure
                            case 4:
                                if (BitConverter.ToInt32(state.m_DataBuf, 0) == 1)
                                { MessageBox.Show("File successfully saved."); setThreadedButton(true); }
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) == 0)
                                { MessageBox.Show("Couldnt convert file."); setThreadedButton(true); }
                                break;
                            //  If success: next 4 bytes: size of file. If failure - MessageBox.

                            case 5:
                                // success
                                if (BitConverter.ToInt32(state.m_DataBuf, 0) == 1)
                                {
                                    // read the filesize
                                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                                }
                                // failure
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) == 0)
                                { MessageBox.Show("File does not exist."); setThreadedButton(true); }
                                //got filesize
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) > 1)
                                {
                                    //prepare data buffer and set m_filesize
                                    state.m_data_size=BitConverter.ToInt32(state.m_DataBuf, 0);
                                    byte[] dataBuffer2 = new byte[state.m_data_size];
                                    state.m_DataBuf = dataBuffer2;
                                    // receive the file
                                    socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                                }

                                break;

                            // Show MessageBox: success/failure
                            case 6:
                                if (BitConverter.ToInt32(state.m_DataBuf, 0) == 1)
                                { MessageBox.Show("File deleted."); setThreadedButton(true); }
                                else if (BitConverter.ToInt32(state.m_DataBuf, 0) == 0)
                                { MessageBox.Show("File does not exist."); setThreadedButton(true); }
                                break;
                        }
                    }
                    // got file in buffer. Now - save it
                    else
                    {
                        try
                        {
                            saveThreadedFile(state);
                            setThreadedButton(true);
                        }
                        catch
                        {
                            MessageBox.Show("Error saving file.");
                            setThreadedButton(true);
                        }

                    }


                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("5Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }



        // Callback for Send function
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
                    state.m_sent = 0;
                    state.m_data_size = 4;
                    byte[] receiveData = new byte[4];
                    state.m_DataBuf = receiveData;
                    /* all the data has been sent */

                    /* depending on option, do: */

                    switch (state.whatdo)
                    {
                        // read: first 4 bytes: success/failure, if success: next 4 bytes: size of file, next x bytes the file itself. Prompt user to save the file
                        case 3:
                            socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                            break;
                        // read: first 4 bytes: success/failure. MessageBox - success/failure
                        case 4:
                            socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                            break;
                        // read: first 4 bytes: success/failure. If success: next 4 bytes: size of file, next x bytes the file itself. Prompt user to save the file
                        case 5:
                            socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                            break;
                        // read: first 4 bytes: success/failure. Show MessageBox.
                        case 6:
                            socketFd.BeginReceive(state.m_DataBuf, state.m_sent, state.m_data_size - state.m_sent, 0, new AsyncCallback(ReceiveCallback), state);
                            break;
                    }
                    

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

                Socket socketFd = mySocket;
                DataObject state = new DataObject();
                state.whatdo = corobic;
                state.m_SocketFd = socketFd;
                state.m_sent = 0;

                byte[] whatdo = BitConverter.GetBytes(state.whatdo);
                

                /* Depending on request prepares suitable byte array to send */

                // Send specified file to server
                if (corobic==3 || corobic==4)
                {
                    //prepare data (option -> filenamesize -> filesize -> filename -> file)
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(filename);
                    byte[] fileNameLength = BitConverter.GetBytes(fileNameByte.Length);

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
                    if (this.textBox_fname.Text.Length > 0)
                    {
                        //prepare data (option -> filenamesize -> filename)
                        byte[] fileNameByte2 = Encoding.ASCII.GetBytes(this.textBox_fname.Text.ToString());
                        byte[] fileNameLength2 = BitConverter.GetBytes(fileNameByte2.Length);

                        byte[] clientData2 = new byte[4 + 4 + fileNameByte2.Length];
                        whatdo.CopyTo(clientData2, 0);
                        fileNameLength2.CopyTo(clientData2, 4);
                        fileNameByte2.CopyTo(clientData2, 8);

                        state.m_DataBuf = clientData2;
                        state.m_data_size = clientData2.Length;
                        //send it
                        socketFd.BeginSend(clientData2, state.m_sent, clientData2.Length - state.m_sent, 0, new AsyncCallback(SendCallback), state);
                    }
                    else { MessageBox.Show("No filename!"); setThreadedButton(true); }

                }

                //Dns.BeginGetHostEntry(this.textBox_ip.Text.ToString(), new AsyncCallback(GetHostEntryCallback), state);
                //setThreadedButton(true);
            }
            catch (Exception exc)
            {
                MessageBox.Show("1Exception:\t\n" + exc.Message.ToString());
                setThreadedButton(true);
            }
        }


        // Choose file to save
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
