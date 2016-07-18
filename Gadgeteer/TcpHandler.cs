using System;
using System.IO;
using Microsoft.SPOT.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net;
using GHI.Utilities;

using TCP;

namespace FileTransportSocket {
    class TcpHandler {
        private TcpClient client;
        private const int DIMBUF = 1024;
        private byte[] wBuffer = new byte[DIMBUF];
        byte[] buffer;
        byte[] lengthBytes;
        int size;
        System.Net.Sockets.SocketFlags flag;

        public TcpHandler(String ip, int remotePort) {
            IPAddress remoteIP = IPAddress.Parse(ip);
            try
            {
                client = new TcpClient();
                IPEndPoint IP_End = new IPEndPoint(remoteIP, remotePort);
                client.Connect(IP_End);
            }catch (Exception e) {
                throw e;
            } 
        }

        public void sendToken(String token)
        {
            try
            {
                flag = new System.Net.Sockets.SocketFlags();
                size = token.Length;
 
                //Send size
                lengthBytes = new byte[sizeof(int)];
                lengthBytes = BitConverter.GetBytes(size);
                client.getSocket().Send(lengthBytes, 0, lengthBytes.Length, flag);
                lengthBytes = null;
                
                //Send token
                buffer = new byte[size];
                buffer = Encoding.UTF8.GetBytes(token);
                client.getSocket().Send(buffer, 0, buffer.Length, flag);
                buffer = null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void sendImage(Bitmap bmp) {
            try{
                flag = new System.Net.Sockets.SocketFlags();
                size = bmp.Width * bmp.Height * 3 + 54;

                //Send size
                lengthBytes = new byte[sizeof(int)];
                lengthBytes = BitConverter.GetBytes(size);            
                client.getSocket().Send(lengthBytes, 0, lengthBytes.Length, flag);
                lengthBytes = null;

                //Send image
                buffer = new byte[size];
                Bitmaps.ConvertToFile(bmp, buffer);               
                client.getSocket().Send(buffer, 0, buffer.Length, flag);
                buffer = null;

            }catch (Exception e) 
            {
                throw e;
            }
        }

        public void Close() {
            try{
                client.Close();
            }catch (Exception e)
            {
                throw e;
            }
        }
    }
}
