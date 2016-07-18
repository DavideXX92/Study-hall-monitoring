using System;
using System.Text;
using Microsoft.SPOT.Net;
using Socket = System.Net.Sockets.Socket;
using System.Net;
using System.Net.Sockets;

namespace TCP {
    class TcpClient {

        private Socket socket;
        public Boolean Connected { set; get; }

        public TcpClient(){
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connected = false;
        }

        public void Connect(IPEndPoint remoteEnd){
            try{
                socket.Connect(remoteEnd);
                if (isSocketConnected(socket))
                    Connected = true;
                else
                    throw new Exception();
            } catch (Exception e) {
                throw e;
            }
        }

        private bool isSocketConnected(Socket s)
        {
            if (s.Poll(1000, SelectMode.SelectRead) && s.Available == 0)
                return false;
            else
                return true;
        }

        public void Close(){
            try{
                socket.Close();
                Connected = false;
            } catch (Exception e) {
                throw e;
            }
            
        }

        public Socket getSocket()
        {
            return socket;
        }
    }
}
