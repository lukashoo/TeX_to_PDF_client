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
        private Socket socketFd;
        private Form obj;
        private string filename;
        private string path;
        private bool choosen;

        public Form1()
        {
            InitializeComponent();
            this.obj = this;
            this.choosen = false;
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
                            this.choosen = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }


    }
}
