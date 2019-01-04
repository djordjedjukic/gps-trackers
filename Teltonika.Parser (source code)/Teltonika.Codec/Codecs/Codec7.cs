using System;
using System.Collections.Generic;
using System.Linq;
using Teltonika.Codec.Model;
using Teltonika.Codec.Model.GH;

namespace Teltonika.Codec.Devices
{
    public class Codec7
    {
        public static class Constants
        {
            public const int CellIdPropertyId = 200;
            public const int SignalQualityPropertyId = 201;
            public const int OperatorCodePropertyId = 202;
            public const int AlarmPropertyId = 204;
        }

        private static readonly DateTime GHepoch = new DateTime(2007, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private readonly ReverseBinaryReader _reader;

        public Codec7(ReverseBinaryReader reader)
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
            var priorityAndTimestamp = _reader.ReadInt32();

            // priority
            var priorityMapIndex = 0x03 & (priorityAndTimestamp >> 30);
            var priority = (GhAvlDataPriority) priorityMapIndex;

            // timestamp
            var timestamp = (long) (priorityAndTimestamp & 0x3FFFFFFF);
            var dateTime = GHepoch.AddSeconds(timestamp);

            var eventId = 0;

            IoProperty? alarmProperty = null;
      
            if (priority == GhAvlDataPriority.Alarm)
            {
                eventId = Constants.AlarmPropertyId;
                alarmProperty = IoProperty.Create(Constants.AlarmPropertyId, 1);
            }

            // global mask Codec7
            var mask = (GlobalMaskCodec7) _reader.ReadByte();

            var gps = GpsElement.Default;

            var gpsIo = new IoElement();


            if (mask.HasFlag(GlobalMaskCodec7.GpsElement))
            {
                var element = DecodeGpsElement();
                gps = element.GPS;
                gpsIo = element.IO;
            }
      
            var ioInt8 = GetProperties(mask, GlobalMaskCodec7.IoInt8, FieldEncoding.Int8);
            var ioInt16 = GetProperties(mask, GlobalMaskCodec7.IoInt16, FieldEncoding.Int16);
            var ioInt32 = GetProperties(mask, GlobalMaskCodec7.IoInt32, FieldEncoding.Int32);
            
            var properties = new List<IoProperty>();

            if (alarmProperty != null)
            {
                properties.Add(alarmProperty.Value);
            }

            properties.AddRange(gpsIo.Properties);
            
            properties.AddRange(ioInt8 ?? Enumerable.Empty<IoProperty>());
            properties.AddRange(ioInt16 ?? Enumerable.Empty<IoProperty>());
            properties.AddRange(ioInt32 ?? Enumerable.Empty<IoProperty>());

            var ioElement = IoElement.Create(eventId, properties.Count(), properties);

            return AvlData.Create(priority.ToString(), dateTime, gps, ioElement);
        }

        public IEnumerable<IoProperty> GetProperties(GlobalMaskCodec7 maskCodec7, GlobalMaskCodec7 flag, FieldEncoding encoding)
        {
            if (maskCodec7.HasFlag(flag))
            {
                return DecodeIoElement(encoding).Properties.ToList();
            }

            return null;
        }

        public IoElement DecodeIoElement(FieldEncoding encoding)
        {
            var count = _reader.ReadByte();

            var properties = new List<IoProperty>(count);
            for (var i = 0; i < count; i++)
            {
                properties.Add(DecodeProperty(encoding));
            }
            return IoElement.Create(0, properties.Count(), properties); 
        }

        public IoProperty DecodeProperty(FieldEncoding encoding)
        {
            // read ID
            var propertyId = (int)_reader.ReadByte();
            switch (encoding)
            {
                case FieldEncoding.Int8:
                    return IoProperty.Create(propertyId, _reader.ReadSByte());
                case FieldEncoding.Int16:
                    return IoProperty.Create(propertyId, _reader.ReadInt16());
                case FieldEncoding.Int32:
                    return IoProperty.Create(propertyId, _reader.ReadInt32());
                default:
                    throw new NotSupportedException(string.Format("The field encoding \"{0}\" is not supported.", encoding));
            }
        }

        /// <summary>
        /// Decode Gps element
        /// </summary>
        /// <returns></returns>
        private GpsElementExt DecodeGpsElement()
        {
            var mask = (GpsElementMaskCodec7)_reader.ReadByte();

            float x = 0;
            float y = 0;

            if (mask.HasFlag(GpsElementMaskCodec7.Coordinates))
            {
                var lat = EndianBitConverter.Int32ToSingle(_reader.ReadInt32());
                var lng = EndianBitConverter.Int32ToSingle(_reader.ReadInt32());

                if (!GpsElement.IsLatValid(lat))
                {
                    lat = 0;
                }
                if (!GpsElement.IsLngValid(lng))
                {
                    lng = 0;
                }

                y = lat;
                x = lng;
            }

            short altitude = 0;
            if (mask.HasFlag(GpsElementMaskCodec7.Altitude))
            {
                altitude = _reader.ReadInt16();
            }

            short angle = 0;
            if (mask.HasFlag(GpsElementMaskCodec7.Angle))
            {
                angle = (short)Math.Round(((double)_reader.ReadByte() * 360 / 256));
            }

            short speed = 0;
            if (mask.HasFlag(GpsElementMaskCodec7.Speed))
            {
                speed = _reader.ReadByte();
            }
            
            var satellites = mask.HasFlag(GpsElementMaskCodec7.Satellites) ? _reader.ReadByte() : (byte)3;

            var properties = new List<IoProperty>(3);

            if (mask.HasFlag(GpsElementMaskCodec7.CellId))
            {
                var cellId = _reader.ReadInt32();
                properties.Add(IoProperty.Create(Constants.CellIdPropertyId, cellId));
            }

            if (mask.HasFlag(GpsElementMaskCodec7.SignalQuality))
            {
                var signalQuality = _reader.ReadByte();
                properties.Add(IoProperty.Create(Constants.SignalQualityPropertyId, signalQuality));
            }

            if (mask.HasFlag(GpsElementMaskCodec7.OperatorCode))
            {
                var code = _reader.ReadInt32();
                properties.Add(IoProperty.Create(Constants.OperatorCodePropertyId, code));
            }

            // set the N/A position if it's not available
            if (x == 0 && y == 0)
            {
                speed = GpsElement.InvalidGpsSpeed;
                satellites = 0;
            }

            var gps = GpsElement.Create(x, y, altitude, speed, angle, satellites);

            var io = IoElement.Create(0, properties.Count, properties);

            return new GpsElementExt(gps: gps, io: io);
        }
    }
}
