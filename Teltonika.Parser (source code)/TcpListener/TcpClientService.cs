using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;
using Teltonika.Codec;
using Teltonika.Codec.Model;

namespace TcpListenerApp
{
    public class TcpClientService
    {
        private static readonly ILog Log = LogManager.GetLogger("");
        readonly TcpClient _client;

        public TcpClientService(TcpClient client)
        {
            _client = client;
        }

        public async Task Run()
        {
            using (_client)
            using (var stream = _client.GetStream())
            {
                Log.Info(DateTime.Now + " Received connection request from " + _client.Client.RemoteEndPoint);

                var fullPacket = new List<byte>();
                int? avlDataLength = null;

                var bytes = new byte[4096];
                var connected = false;
                int length;

                // Loop to receive all the data sent by the client.
                while ((length = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    Log.Info(string.Format("{0} - received [{1}]", DateTime.Now, String.Join("", bytes.Take(length).Select(x => x.ToString("X2")).ToArray())));

                    byte[] response;

                    if (!connected)
                    {
                        // Accept imei
                        response = new byte[] { 01 };
                        connected = true;
                        await stream.WriteAsync(response, 0, response.Length);

                        Array.Clear(bytes, 0, bytes.Length);

                        Log.Info(string.Format("{0} - responded [{1}]", DateTime.Now, String.Join("", response.Select(x => x.ToString("X2")).ToArray())));
                    }
                    else
                    {
                        fullPacket.AddRange(bytes.Take(length));
                        Array.Clear(bytes, 0, bytes.Length);

                        var count = fullPacket.Count;

                        // continue if there is not enough bytes to get avl data array length
                        if (count < 8) continue;

                        avlDataLength = avlDataLength ?? BytesSwapper.Swap(BitConverter.ToInt32(fullPacket.GetRange(4, 4).ToArray(), 0));

                        var packetLength = 8 + avlDataLength + 4;
                        if (count > packetLength)
                        {
                            Log.Error("Too much data received.");
                            throw new ArgumentException("Too much data received.");
                        }
                        // continue if not all data received
                        if (count != packetLength) continue;

                        // Decode tcp packet
                        var decodedData = DecodeTcpPacket(fullPacket.ToArray());
                        response = BitConverter.GetBytes(BytesSwapper.Swap(decodedData.AvlData.DataCount));

                        await stream.WriteAsync(response, 0, response.Length);

                        avlDataLength = null;
                        fullPacket.Clear();

                        Log.Info(string.Format("{0} - responded [{1}]", DateTime.Now, String.Join("", response.Select(x => x.ToString("X2")).ToArray())));
                    }
                }
            }
        }

        private static TcpDataPacket DecodeTcpPacket(byte[] request)
        {
            var reader = new ReverseBinaryReader(new MemoryStream(request));
            var decoder = new DataDecoder(reader);

            return decoder.DecodeTcpData();
        }
    }
}