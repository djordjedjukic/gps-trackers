using System;
using Teltonika.Codec.Devices;
using Teltonika.Codec.Model;

namespace Teltonika.Codec
{
    public class DataDecoder
    {
        private readonly ReverseBinaryReader _reader;

        public DataDecoder(ReverseBinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            _reader = reader;
        }

        /// <summary>
        /// Decode AVL Tcp data packet
        /// </summary>
        /// <returns></returns>
        public TcpDataPacket DecodeTcpData()
        {
            var preamble = _reader.ReadInt32();
            var length = _reader.ReadInt32();
            var codecId = _reader.ReadByte();
            _reader.BaseStream.Position = 8;
            var data = _reader.ReadBytes(length);
            var crc = _reader.ReadInt32();
            _reader.BaseStream.Position = 8;

            if (preamble != 0)
                throw new NotSupportedException("Unable to decode. Missing package prefix.");

            if (crc != CRC.Default.CalcCrc16(data))
                throw new InvalidOperationException("CRC does not match the expected.");

            var avlDataCollection = new AvlDataCollection();

            if (codecId == 7)
                avlDataCollection = new Codec7(_reader).DecodeAvlDataCollection();

            if (codecId == 8)
                avlDataCollection = new Codec8(_reader).DecodeAvlDataCollection();

            return TcpDataPacket.Create(preamble, length, crc, avlDataCollection);
        }


        /// <summary>
        /// Decode AVL udp data packet
        /// </summary>
        /// <returns></returns>
        public UdpDataPacket DecodeUdpData()
        {
            var length = _reader.ReadInt16();
            var packetId = _reader.ReadInt16();
            var packetType = _reader.ReadByte();
            var avlPacketId = _reader.ReadByte();
            var imeiLength = _reader.ReadInt16();
            var imei = _reader.ReadBytes(imeiLength - 1);
            var avlData = new AvlDataCollection();

            _reader.BaseStream.Position = _reader.BaseStream.Position + 1;
            var codecId = _reader.ReadByte();
            _reader.BaseStream.Position = _reader.BaseStream.Position - 1;

            if (codecId == 7)
            {
                avlData = new Codec7(_reader).DecodeAvlDataCollection();
            }
            if (codecId == 8)
            {
                avlData = new Codec8(_reader).DecodeAvlDataCollection();
            }

            return UdpDataPacket.Create(length, packetId, packetType, avlPacketId, imeiLength, imei, avlData);
        }
    }
}
