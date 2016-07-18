using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsocketClient {
    class WSClient {
        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 256;
        private const bool verbose = true;
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(30000);
        ClientWebSocket webSocket;

        public WSClient() {
            webSocket = null;
            Connect("ws://localhost:9000/polistudio/server.php").Wait();
        }

        public async Task Connect(string uri) {
            try {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                Notify();
            } catch (Exception ex) {
                Console.WriteLine("Exception: {0}", ex);
            } finally {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                /*lock (consoleLock) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();
                }*/
            }
        }
        static UTF8Encoding encoder = new UTF8Encoding();

        public void Notify() {

            byte[] buffer = encoder.GetBytes("{\"message\":\"notify\"}");
            webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}