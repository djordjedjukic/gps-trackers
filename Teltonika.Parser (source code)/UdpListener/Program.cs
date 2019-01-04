using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Teltonika.Codec;

namespace UdpListener
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger("");

        static void Main()
        {
            XmlConfigurator.Configure();

            IPAddress ip;
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out ip))
            {
                Log.Error("Ip is not valid.");
                throw new ArgumentException("Ip is not valid.");
            }

            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings["port"], out port))
            {
                Log.Error("Port is not valid.");
                throw new ArgumentException("Port is not valid.");
            }

            Task.Run(async () =>
            {
                using (var udpClient = new UdpClient(new IPEndPoint(ip, port)))
                {
                    Log.Info("Listening...");

                    while (true)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var receivedResults = await udpClient.ReceiveAsync();

                        byte[] data = receivedResults.Buffer;

                        Log.Info(string.Format("Received connection from: {0}", receivedResults.RemoteEndPoint));
                        Log.Info(string.Format("{0} - received [{1}]", DateTime.Now, String.Join("", data.Select(x => x.ToString("X2")).ToArray())));

                        var reader = new ReverseBinaryReader(new MemoryStream(data));

                        // Decode data
                        var avlData = new DataDecoder(reader).DecodeUdpData();

                        // Create response
                        var bytes = new List<byte>();

                        const short packetLength = 2 /* Id */+ 1 /* Type */ + 1 /* Avl packet id */+ 1 /* num of accepted elems */;
                        bytes.AddRange(BitConverter.GetBytes(BytesSwapper.Swap(packetLength)));
                        bytes.AddRange(BitConverter.GetBytes(BytesSwapper.Swap(avlData.PacketId)));
                        bytes.Add(avlData.PacketType);
                        bytes.Add(avlData.AvlPacketId);
                        bytes.Add((byte)avlData.AvlData.DataCount);

                        var response = bytes.ToArray();

                        Log.Info(string.Format("{0} - received [{1}]", DateTime.Now, String.Join("", bytes.Select(x => x.ToString("X2")).ToArray())));

                        await udpClient.SendAsync(response, response.Length, receivedResults.RemoteEndPoint);
                    }
                }
            });

            Console.ReadLine();
        }
    }
}
