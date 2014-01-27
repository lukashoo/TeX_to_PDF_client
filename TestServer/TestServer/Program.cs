using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TestServer
{
    //FILE TRANSFER USING C#.NET SOCKET PROGRAMMING - SERVER
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("That program can transfer small file. I've test up to 850kb file");
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, 5656);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                sock.Bind(ipEnd);
                sock.Listen(100);
                Socket clientSock = sock.Accept();
                int fullreceivedbytes = 0;

                //byte[] clientData = new byte[1024 * 5000];
                byte[] integer = new byte[4];
                string receivedPath = "C:/Users/Lukashoo/Desktop/Testy/";

                int receivedBytesLen = clientSock.Receive(integer,0,4,0);
                int fileNameLen = BitConverter.ToInt32(integer, 0);
                Console.Write("FilenameLen: " + fileNameLen.ToString() + "  \n");

                receivedBytesLen = clientSock.Receive(integer, 0, 4, 0);
                int fileLen = BitConverter.ToInt32(integer, 0);
                Console.Write("Filesize: " + fileLen.ToString() + "  \n");

                byte[] filenamebyte = new byte[fileNameLen];
                receivedBytesLen = clientSock.Receive(filenamebyte, 0, fileNameLen, 0);
                string filename = Encoding.ASCII.GetString(filenamebyte, 0, fileNameLen);
                Console.Write("Filename: " + filename + "  \n");

                byte[] filebyte = new byte[fileLen];
                while (fullreceivedbytes < fileLen)
                {
                    receivedBytesLen = clientSock.Receive(filebyte, fullreceivedbytes, fileLen-fullreceivedbytes, 0);
                    Console.Write("Received bytes: " + receivedBytesLen.ToString() + "  \n");
                    fullreceivedbytes = fullreceivedbytes + receivedBytesLen;
                }
                Console.Write("All bytes: " + fullreceivedbytes.ToString() + "  \n");

                BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + filename, FileMode.OpenOrCreate));
                bWrite.Write(filebyte, 0, fileLen);
                Console.WriteLine("File: {0} received & saved at path: {1}", filename, receivedPath);

                bWrite.Close();
                clientSock.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Receiving fail." + ex.Message);
            }
        }
    }
}