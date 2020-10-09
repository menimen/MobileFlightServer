using System.Drawing;

namespace FlightAppServer
{
    //DB Class
    public class Data
    {
        public static string ip = "127.0.0.1";
        public static int port = 5403;
        public static int httpPort = 5000;

        public static string Aileron_Destination = "/controls/flight/aileron";
        public static string Throttle_Destination = "/controls/engines/current-engine/throttle";
        public static string Rudder_Destination = "/controls/flight/rudder";
        public static string Elevator_Destination = "/controls/flight/elevator";

        public static double throttle = 0;
        public static double aileron = 0;
        public static double elevator = 0;
        public static double rudder = 0;

        public static byte[] image_bytes = null;
    }
}
