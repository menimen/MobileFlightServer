namespace FlightAppServer
{
    //Configuration class
    public class FlightSimulatorConfig
    {
        public static int simulator_HTTP_port  = 5000;
        public static string simulator_ip = "127.0.0.1";
        public static int simulator_port  = 5403;
        public static string HTTP_REQUEST = "http://" + simulator_ip + ":" 
            + simulator_HTTP_port.ToString() + "/screenshot";
    }
}
