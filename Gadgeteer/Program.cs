using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Ws.Services.Binding;
using Ws.Services;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

using polistudio;
using FileTransportSocket;

namespace savePhotoETH {
    public partial class Program {
        const String ipEndPoint = "192.168.1.2";
        const String urlService = "https://192.168.1.2/gadgeteer/Service.svc";
        const String username = "Davide";
        const String password = "password";
        const int intervalPhoto = 15; //seconds
        IServiceClientProxy proxy;
        public Boolean imLogged;
        public Boolean imConnected;
        String token;
        int yCoord = 30;
        Bitmap b, preview;
        portRequest tmpPortReq;
        TcpHandler th;
        logoutResponse resp;
        portRequestResponse p;
        loginResponse respLogin;
        GT.Timer timer;
        int idPhoto;
        Resources.BinaryResources[] resources;

        enum Status { networkUp, networkDown, sendImageError, tryingToLogin, loginSuccess, loginFail, tryingToLogout, logoutSuccess, logoutFail, photoSent, timerSet, timerStop };

        void ProgramStarted() {
            Debug.Print("Program Started");
            //ethernetJ11D.UseStaticIP("192.168.1.10", "255.255.255.0", "0.0.0.0");
            ethernetJ11D.UseThisNetworkInterface();
            ethernetJ11D.NetworkUp += eth_NetworkUp;
            ethernetJ11D.NetworkDown += eth_NetworkDown;
            button.ButtonPressed += button_ButtonPressed;
            camera.PictureCaptured += camera_PictureCaptured;
            timer = new GT.Timer(intervalPhoto * 1000);
            timer.Tick += takePicture;
            imConnected = false;
            imLogged = false;
            multicolorLED.TurnRed();
            showLogTitle();
            showNetworkStatus();
            showLoginStatus();
            Debug.Print("Waiting for a connection...");
            idPhoto = 0;
            resources = new Resources.BinaryResources[] { Resources.BinaryResources._1,
                                                          Resources.BinaryResources._2,
                                                          Resources.BinaryResources._3,
                                                          Resources.BinaryResources._4,
                                                          Resources.BinaryResources._5,
                                                          Resources.BinaryResources._6,
                                                          Resources.BinaryResources._7,
                                                          Resources.BinaryResources._8,
                                                          Resources.BinaryResources._9
                                                        };
        }

        void eth_NetworkDown(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state) {
            Debug.Print("Network Down");
            imConnected = false;
            imLogged = false;
            showStatus(Status.networkDown);
        }

        void eth_NetworkUp(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state){
            Debug.Print("Network Up");
            Debug.Print("Network joined, IP Address: " + ethernetJ11D.NetworkSettings.IPAddress);
            imConnected = true;
            showStatus(Status.networkUp);
            try
            {
                Debug.Print("Trying to login...");
                showStatus(Status.tryingToLogin);
                token = login(username, password);
                imLogged = true;
                Debug.Print("OK, login success, token: " + token);
                showStatus(Status.loginSuccess);
                timer.Start();
                Debug.Print("Timer set");
                showStatus(Status.timerSet);
            }
            catch (LoginException)
            {
                Debug.Print("Error, login failed");
                showStatus(Status.loginFail);
            }
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            if( imConnected && imLogged)
            {
                Debug.Print("OK, picture taken");
                try
                {
                    b = new Bitmap(Resources.GetBytes(resources[idPhoto]), Bitmap.BitmapImageType.Bmp);
                    //b = picture.MakeBitmap();
                    preview = new Bitmap(160, 120);
                    preview.StretchImage(0, 0, b, 160, 120, 0xff);
                    try
                    {
                        Debug.Print("Requesting port to web service...");
                        displayTE35.SimpleGraphics.DisplayLine(GT.Color.White, 1, 319, 239, 320, 240);
                        int port = getPort();
                        displayTE35.SimpleGraphics.DisplayLine(GT.Color.Black, 1, 319, 239, 320, 240);
                        Debug.Print("OK, port: " + port);
                        Debug.Print("Trying to open a Tcp connection with the server...");
                        th = new TcpHandler(ipEndPoint, port);
                        Debug.Print("OK, connected: " + ipEndPoint + ":" + port);
                        Debug.Print("Trying to send token...");
                        th.sendToken(token);
                        Debug.Print("Ok, token sent");
                        Debug.Print("Trying to send image...");
                        th.sendImage(b);
                        Debug.Print("Ok, image sent");
                        th.Close();
                        Debug.Print("Tcp connection close");
                        showStatus(Status.photoSent);
                        idPhoto++;
                        if (idPhoto == 9)
                            idPhoto = 0;
                    }
                    catch (Exception e)
                    {
                        Debug.Print("Error with the network, unable to send the photo to server");
                        Debug.Print("Exception:" + e.Message);
                        showStatus(Status.sendImageError);
                    }
                }
                catch (Exception e)
                {
                    Debug.Print("Exception:" + e.Message);
                }  
            }
            
        }

        void button_ButtonPressed(Button sender, Button.ButtonState state) {
            Debug.Print("Button pressed");
            if (imConnected)
            {
                if (imLogged)
                {
                    try
                    {
                        Debug.Print("Trying to logout...");
                        showStatus(Status.tryingToLogout);
                        logout(token);
                        imLogged = false;
                        Debug.Print("OK, logout success");
                        timer.Stop();
                        showStatus(Status.timerStop);
                        Debug.Print("Timer stopped");
                        showStatus(Status.logoutSuccess);
                    }
                    catch (LogoutException)
                    {
                        Debug.Print("Error, logout failed");
                        showStatus(Status.logoutFail);
                    }
                }
                else
                {
                    try
                    {
                        Debug.Print("Trying to login...");
                        showStatus(Status.tryingToLogin);
                        token = login(username, password);
                        imLogged = true;
                        Debug.Print("OK, login success, token: " + token);
                        showStatus(Status.loginSuccess);
                        timer.Start();
                        showStatus(Status.timerSet);
                        Debug.Print("Timer set");
                    }
                    catch (LoginException)
                    {
                        Debug.Print("Error, login failed");
                        showStatus(Status.loginFail);
                    }
                }
            }
        }

        private void takePicture(object state) {
            if (imConnected && imLogged)
            {
                if (camera.CameraReady)
                {
                    Debug.Print("Taking a photo...");
                    camera.TakePicture();
                    multicolorLED.BlinkOnce(GT.Color.Yellow, new TimeSpan(0, 0, 1), GT.Color.Green);
                }
            }
        }

        private int getPort()
        {
            bool retry = true;
            int n_try = 0;
            int port = 0;

            while (retry)
            {
                try
                {
                    tmpPortReq = new portRequest();
                    tmpPortReq.token = token;
                    p = proxy.portRequest(tmpPortReq);
                    port = p.portRequestResult;
                    if (port == 0)
                        throw new PortException();
                    retry = false;
                }
                catch (PortException)
                {
                    n_try++;
                    if (n_try == 3)
                        throw new PortException();
                }
            }
            return port;
        }

        private String login(String username, String password) {
            bool retry = true;
            int n_try = 0;
            token = null;

            while(retry){
                try{
                    proxy = new IServiceClientProxy(new WS2007HttpBinding(), new ProtocolVersion11());
                    proxy.EndpointAddress = urlService;
                    respLogin = proxy.login(new login(){
                        username = username,
                        password = password
                    });
                    token = respLogin.loginResult;
                    if(token.Equals("false"))
                        throw new LoginException();
                    retry = false;
                }catch(Exception){
                    n_try++;
                    if (n_try == 3)
                        throw new LoginException();
                }
            }
            return token;
        }

        private void logout(String token) {
            resp = proxy.logout(new logout() { token = token });
            if (!resp.logoutResult)
                throw new LogoutException();
            token = null;
        }

        private void showStatus(Status state){
            if (yCoord >= 220){
                yCoord = 22;
                displayTE35.SimpleGraphics.Clear();
                showLogTitle();
                showNetworkStatus();
                showLoginStatus();
            }
            string message = "- ";
            switch (state)
            {
                case Status.networkUp:
                    message += "Network Up";
                    multicolorLED.TurnGreen();
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showNetworkStatus();
                    break;
                case Status.networkDown:
                    message += "Network Down";
                    multicolorLED.TurnRed();
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showNetworkStatus();
                    showLoginStatus();
                    break;
                case Status.tryingToLogin:
                    message += "Trying to login...";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
                case Status.loginSuccess:
                    message += "Login success";
                    multicolorLED.TurnGreen();
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showLoginStatus();
                    break;
                case Status.loginFail:
                    message += "Error, login failed";
                    multicolorLED.TurnRed();
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showLoginStatus();
                    break;
                case Status.tryingToLogout:
                    message += "Logging out...";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
                case Status.logoutSuccess:
                    message += "Logout success";
                    multicolorLED.TurnColor(GT.Color.Yellow);
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showLoginStatus();
                    break;
                case Status.logoutFail:
                    message += "Error, logout failed";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    showLoginStatus();
                    break;
                case Status.photoSent:
                    displayTE35.SimpleGraphics.DisplayImage(preview, 160, 60);
                    message += "Photo sent";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
                case Status.sendImageError:
                    displayTE35.SimpleGraphics.DisplayImage(preview, 160, 60);
                    message = "Error with the network, ";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    message = "photo discarded";
                    yCoord += 20;
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
                case Status.timerSet:
                    message += "Timer set";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
                case Status.timerStop:
                    message += "Timer stop";
                    displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, yCoord);
                    break;
            }
            yCoord += 20;
        }

        private void showLogTitle()
        {
            String message = "CONSOLE LOG";
            displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, 0);
            displayTE35.SimpleGraphics.DisplayLine(GT.Color.White, 1, 0, 17, 130, 17);
        }

        private void showNetworkStatus()
        {
            String message = "Network: ";
            GT.Color color;
            if (imConnected)
            {
                message += "connected";
                color = GT.Color.Green;
            }
            else 
            {
                message += "disconnected";
                color = GT.Color.Red;
            }
            displayTE35.SimpleGraphics.DisplayRectangle(GT.Color.Black, 1, GT.Color.Black, 160, 0, 160, 20);
            displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), color, 160, 0);

        }

        private void showLoginStatus()
        {
            String message;
            GT.Color color;
            if(imLogged){
                message = "You are logged";
                color = GT.Color.Green;
            }
            else 
            {
                message = "You are not logged";
                color = GT.Color.Red;
            }
            displayTE35.SimpleGraphics.DisplayRectangle(GT.Color.Black, 1, GT.Color.Black, 160, 20, 160, 20);
            displayTE35.SimpleGraphics.DisplayText(message, Resources.GetFont(Resources.FontResources.NinaB), color, 160, 20);
        }
    }
}
