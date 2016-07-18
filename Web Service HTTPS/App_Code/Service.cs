using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;

using DB;
// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
namespace polistudio {
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service : IService {
        public String login(String username, String password) {
            string token = null;
            DBConnect db = null;
            bool is_valid = false;

            try {
                db = new DBConnect();
                while (!is_valid) {
                    token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    if (db.isTokenValid(token) == 0)//isTokenValid return 0 if it isn't into the DB
                        is_valid = true;
                }
                bool amILogged = db.login(username, password, token);
                if (!amILogged)
                    token = "false";
            } catch (Exception) {
                return "false";
            }    

            return token;
        }
        
        public int portRequest(String token) {
            DBConnect db = null;
            int studyroomId;
            
            try {
                db = new DBConnect();
                studyroomId = db.isTokenValid(token);
                if(studyroomId != 0){
                    int port = getAvailablePort();
                    if (port != 0) {
                        Process p = new Process();
                        p.StartInfo = new ProcessStartInfo("C:/inetpub/wwwroot/gadgeteer/App_Code/ServerTCP.exe");
                        p.StartInfo.Arguments = port.ToString() + " " + studyroomId.ToString();
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.Verb = "runas";
                        p.Start();
                    }
                    return port;
                }
                return 0;
            } catch (Exception) {
                return 0;
            }            
            
        }

        public Boolean logout(String token) {
            DBConnect db = null;
            try {
                db = new DBConnect();
                return db.logout(token);
            } catch (Exception) {
                return false;
            }
        }

        private int getAvailablePort() {
            Random rnd = new Random();
            int port = 0;
            int n_try = 0;
            bool is_valid = false;

            while(!is_valid && n_try < 20){
                try {
                    port = rnd.Next(1001, 65536); //Generation of a random number between 1001 and 65535
                    TcpListener listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    is_valid = true;
                    listener.Stop();
                } catch (Exception) {
                    n_try++;
                }
            }

            return port;
        }
    }
}