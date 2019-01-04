using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace TcpListenerApp
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("");

        private static void Main()
        {
            XmlConfigurator.Configure();

            TcpServerAsync().Wait();
        }

        private static async Task TcpServerAsync()
        {
            IPAddress ip;
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out ip))
            {
                Console.WriteLine("Failed to get IP address, service will listen for client activity on all network interfaces.");
                ip = IPAddress.Any;
            }

            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["port"], out port))
            {
                throw new ArgumentException("Port is not valid.");
            }

            Log.Info("Starting listener...");
            var server = new TcpListener(ip, port);

            server.Start();
            Log.Info("Listening...");
            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                var cw = new TcpClientService(client);
                ThreadPool.UnsafeQueueUserWorkItem(x => ((TcpClientService) x).Run(), cw);
            }
        }
    }
}