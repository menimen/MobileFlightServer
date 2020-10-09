using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlightAppServer
{
    public class Client
    {
        private Socket clientSocket;

        public Client(){
            FlightSimulatorConfig.simulator_ip = Data.ip;
            FlightSimulatorConfig.simulator_port = Data.port;
            FlightSimulatorConfig.simulator_HTTP_port = Data.httpPort;
            Start();
        }

        //Automatically start when class is created
        //Runs 2 thread - one handles the controllers get/set
        //The other handles the image
        public void Start()
        {
            Thread thread1 = new Thread(() =>
            {
                while (true)
                    RunClient(FlightSimulatorConfig.simulator_port);
            })
            {
                IsBackground = true
            };
            Thread thread2 = new Thread(new ThreadStart(syncImage));
            thread1.Start();
            thread2.Start();
        }

        //Create socket between server and client and runs sync
        private void RunClient(int port)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(FlightSimulatorConfig.simulator_ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                clientSocket = new Socket(ipAddress.AddressFamily,
                     SocketType.Stream, ProtocolType.Tcp);
                bool connected = false;
                while (!connected)
                {
                    try
                    {
                        clientSocket.Connect(remoteEP);
                        connected = true;
                        StartCommunication();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                while (clientSocket.Connected)
                    SyncValues();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //Syncing values between server and simulator
        private void SyncValues()
        {
            try
            {
                Set(Data.Throttle_Destination,Data.throttle);
                Set(Data.Aileron_Destination, Data.aileron);
                Set(Data.Rudder_Destination, Data.rudder);
                Set(Data.Elevator_Destination, Data.elevator);
                try { 
                    Data.aileron = GetValue(Data.Aileron_Destination);
                }
                catch (Exception) { 
                    Console.WriteLine("Couldn't get " + Data.Aileron_Destination);
                }
                try { 
                    Data.rudder = GetValue(Data.Rudder_Destination);
                }
                catch (Exception) { 
                    Console.WriteLine("Couldn't get " + Data.Rudder_Destination);
                }
                try { 
                    Data.elevator = GetValue(Data.Elevator_Destination);
                }
                catch (Exception) { 
                    Console.WriteLine("Couldn't get " + Data.Elevator_Destination);
                }
                try { 
                    Data.throttle = GetValue(Data.Throttle_Destination);
                }
                catch (Exception) { 
                    Console.WriteLine("Couldn't get " + Data.Throttle_Destination);
                }
                Console.WriteLine("Aileron :{0} Throttle :{1} Elevator :{2} Rudder :{3}"
                    , Data.aileron, Data.throttle, Data.elevator, Data.rudder);

            }
            catch (ArgumentNullException ane){
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se) {             
                Console.WriteLine("SocketException : {0}", se.ToString());
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e) { 
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            Thread.Sleep(100);
        }

        //Before set/get , sends data\n 
        private void StartCommunication()
        {
            byte[] msg = Encoding.ASCII.GetBytes(string.Format("data\n"));
            clientSocket.Send(msg);
        }

        //get /.../ method
        private double GetValue(string key)
        {
            byte[] bytes = new byte[1024];

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(string.Format("get {0}\n", key));

            // Send the data through the socket.
            clientSocket.Send(msg);
            
            // Receive the response from the remote device.
            int bytesRec = clientSocket.Receive(bytes);
            string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            return double.Parse(data);
        }

        //General set method
        private void Set(string key, double value)
        {
            try
            {
                SetValue(key, value);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't set " + key);
            }
        }

        //set /.../ method
        private void SetValue(string key, double value)
        {
            byte[] msg = Encoding.ASCII.GetBytes(string.Format("set {0} {1}\n", key, value));
            clientSocket.Send(msg);
        }

        //Updated images in DB , handled by thread2
        private void syncImage()
        {
            while (true)
            {
                var client = new WebClient();
                try
                {
                    Data.image_bytes = client.DownloadData(FlightSimulatorConfig.HTTP_REQUEST);
                    //using (Image image = Image.FromStream(new MemoryStream(Data.image_bytes)))
                        //image.Save("screenshot.Png", ImageFormat.Png);  // Or Png
                }
                catch (Exception)
                {
                    Console.WriteLine("Error getting image from {0}", FlightSimulatorConfig.HTTP_REQUEST);
                }
                Thread.Sleep(100);
            }
        }
    }
}