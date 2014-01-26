using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace TeX_to_PDF_client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Socket socketFd;
            int success = 0;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Form2 foremka = new Form2();
            //Application.Run(foremka);
            //socketFd = foremka.getSocket();
            //success = foremka.getSuccess();
            //if (success==1)
            Application.Run(new Form1()); 
            //Application.Run(new Form1(socketFd));
        }
    }
}
