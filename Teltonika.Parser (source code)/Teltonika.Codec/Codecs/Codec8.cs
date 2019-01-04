using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teltonika.Codec.Model;

namespace Teltonika.Codec.Devices
{
    public class Codec8
    {
        private static readonly DateTime AvlEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private readonly ReverseBinaryReader _reader;

        public Codec8(ReverseBinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
        }

        /// <summary>
        /// Decode AVL data packet
        /// </summary>
        /// <returns></returns>
        public AvlDataCollection DecodeAvlDataCollection()
        {
            
            var codecId = _reader.ReadByte();
            var dataCount = _reader.ReadByte();
            var data = new List<AvlData>();

            for (var i = 0; i < dataCount; i++)
            {
                var avlData = DecodeAvlData();
                data.Add(avlData);
            }

            return AvlDataCollection.Create(codecId, dataCount, data);
        }

        /// <summary>
        /// Decode single AVL data
        /// </summary>
        /// <returns></returns>
        private AvlData DecodeAvlData()
        {
            var timestamp = _reader.ReadInt64();
            var dateTime = AvlEpoch.AddMilliseconds(timestamp);
            var priority = (AvlDataPriority)_reader.ReadByte();

            // GPS element decoding
            var gpsElement = DecodeGpsElement();

            // IO Element decoding
            var eventId = _reader.ReadByte();
            var propertiesCount = _reader.ReadByte();
            // IO Element Properties decoding
            var ioProperties = DecodeIoProperties();

            var ioElement = IoElement.Create(eventId, propertiesCount, ioProperties);

            return AvlData.Create(priority.ToString(), dateTime, gpsElement, ioElement);
        }

        /// <summary>
        /// Decode Gps element
        /// </summary>
        /// <returns></returns>
        private GpsElement DecodeGpsElement()
        {
            var longitude = _reader.ReadInt32();
            var latitude = _reader.ReadInt32();
            var altitude = _reader.ReadInt16();
            var angle = _reader.ReadInt16();
            var satellites = _reader.ReadByte();
            var speed = _reader.ReadInt16();

            return GpsElement.Create(longitude, latitude, altitude, speed, angle, satellites);
        }

        private IList<IoProperty> DecodeIoProperties()
        {
            var result = new List<IoProperty>();

            // total number of I/O properties which length is 1 byte
            int ioCountInt8 = _reader.ReadByte();
            for (var i = 0; i < ioCountInt8; i++)
            {
                var propertyId = (int)_reader.ReadByte();
                var value = _reader.ReadSByte();

                result.Add(IoProperty.Create(propertyId, value));
            }

            // total number of I/O properties which length is 2 bytes
            int ioCountInt16 = _reader.ReadByte();
            for (var i = 0; i < ioCountInt16; i++)
            {
                var propertyId = (int)_reader.ReadByte();
                var value = _reader.ReadInt16();

                result.Add(IoProperty.Create(propertyId, value));
            }

            // total number of I/O properties which length is 4 bytes
            int ioCountInt32 = _reader.ReadByte();
            for (var i = 0; i < ioCountInt32; i++)
            {
                var propertyId = (int)_reader.ReadByte();
                var value = _reader.ReadInt32();

                result.Add(IoProperty.Create(propertyId, value));
            }

            // total number of I/O properties which length is 8 bytes
            int ioCountInt64 = _reader.ReadByte();
            for (var i = 0; i < ioCountInt64; i++)
            {
                var propertyId = (int)_reader.ReadByte();
                var value = _reader.ReadInt64();

                result.Add(IoProperty.Create(propertyId, value));
            }

            return result;
        }


    }
}
