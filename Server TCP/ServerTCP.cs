using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;
using EMGU_Program;
using System.Security.AccessControl;

using DB;
using SpotDao;
using TableDAO;
using WebsocketClient;

namespace FileTransportSocket {
    class ServerTCP {
        private TcpListener listener;
        private int tcpPort;
        private TcpClient client;
        private const int DIMBUF = 65536;
        private byte[] rBuffer = new byte[DIMBUF];
        public FileStream fs;

        public ServerTCP(int tcpPort)
        {
            this.tcpPort = tcpPort;
            this.client = null;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, tcpPort);
            Console.WriteLine("SERVER: Port open: " + tcpPort);
            listener.Start();
            Console.WriteLine("SERVER: Waiting for a client to connect...");
            client = listener.AcceptTcpClient();
            listener.Stop();

        }

        public void Stop(){
            if(client!=null)
            {
                client.Close();
                client = null;
            }
        }

        public byte[] receiveFile()
        {
            try
            {
                //Receive size
                byte[] lengthBytes = new byte[sizeof(int)];
                int bytesRead = 0;

                while (bytesRead < 4)
                {
                    bytesRead += client.GetStream().Read(lengthBytes, bytesRead, 1);
                    if (bytesRead <= 0)
                        throw new Exception("SERVER: Connection lost with client");
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);
                Console.WriteLine("SERVER: The file to receive is " + lengthBytes + "B");

                //Receive file
                byte[] bufferDst = new byte[length];
                int bytesReamining = length;
                int bytesReceived = 0;
                bytesRead = 0;
                int bytesToRead;
                int chunk = DIMBUF;

                Console.WriteLine("SERVER: Reception started...");
                while (bytesReamining > 0)
                {
                    if (bytesReamining > chunk)
                        bytesToRead = chunk;
                    else
                        bytesToRead = bytesReamining;

                    bytesRead = client.GetStream().Read(rBuffer, 0, bytesToRead);
                    if (bytesRead < 0)
                        throw new Exception("SERVER: Connection lost with client");
                    Buffer.BlockCopy(rBuffer, 0, bufferDst, bytesReceived, bytesRead);
                    bytesReceived += bytesRead;
                    bytesReamining -= bytesRead;
                }
                Console.WriteLine("SERVER: ...File received!");
                return bufferDst;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        static void Main(string[] args) {
            
            List<Spot> spots = new List<Spot>();
            List<Table> tables = new List<Table>();
            DBConnect db = new DBConnect();
            State state;
            Boolean hasUpdate = false;

            if (args.Length < 2) {
                throw new Exception("Number of params insufficient, it requires port and studyRoomID");
            }
            
            int studyroomId = int.Parse(args[1]);

            ServerTCP s = new ServerTCP(int.Parse(args[0]));
            
            try {
                s.Start();
                
                //Receive token
                byte[] tokenBytes = s.receiveFile();
                String token = Encoding.UTF8.GetString(tokenBytes, 0, tokenBytes.Length);
                
                //Receive photo
                byte[] bitmap = s.receiveFile();
    
                s.Stop();
                
                //Check if token is valid
                if (!db.isValidToken(studyroomId, token))
                    Console.WriteLine("Invalid token, frame discarded");     
                else{
                    MyGraphicsLib myGraphLib = new MyGraphicsLib();
                    state = db.getState(studyroomId);
                    db.readTable(tables, studyroomId);
                    db.readSpots(spots, studyroomId);
                    foreach (Spot tmp in spots)
                        Console.WriteLine("Spot " + tmp.Id + " (x,y): " + "(" + tmp.X + "," + tmp.Y + ") stato: " + tmp.IsFree + " oldState: " + tmp.OldState);
                    myGraphLib.compute(db.getRoom0(studyroomId), bitmap, state, spots, tables);
                    foreach (Spot tmp in spots)
                    {
                        if (tmp.Id == 0) {
                            db.addSpot(tmp, studyroomId);
                            hasUpdate = true;
                        } else {
                            if (tmp.IsFree != tmp.OldState || !tmp.IsFree) {
                                Console.WriteLine("Spot to update: " + tmp.Id + " " + tmp.OldState + " => " + tmp.IsFree);
                                db.updateSpot(tmp);
                                hasUpdate = true;
                            }
                        }
                        Console.WriteLine("Spot " + tmp.Id + " => x: " + tmp.X + " y: " + tmp.Y);
                    }
                    if (hasUpdate)
                    {
                        WSClient webSocket = new WSClient();
                        Console.WriteLine("Web socket notified");
                    }
                    foreach (Table tmp in tables)
                    {
                        if (tmp.Id == 0)
                            db.addTable(tmp, studyroomId);
                        Console.WriteLine("Table " + tmp.Id + " => x: " + tmp.X + " y: " + tmp.Y + " height: " + tmp.Height + " width: " + tmp.Width);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

    }
}
