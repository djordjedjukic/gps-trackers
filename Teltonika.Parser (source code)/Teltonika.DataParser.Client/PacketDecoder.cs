using System;
using System.Collections.Generic;
using Teltonika.Codec.Model;
using Teltonika.Codec.Model.GH;

namespace Teltonika.DataParser.Client
{
    public sealed class PacketDecoder
    {
        public List<Data> AvlItems = new List<Data>();
        public List<Data> GpsElementSubItems = new List<Data>();
        public List<Data> IoElementSubItems = new List<Data>();
        private readonly DataReader _parser;

        public PacketDecoder(DataReader parser, string codecId)
        {
            _parser = parser;
            if (codecId == "7") ParseAndDecodeCodec7Data();
            if (codecId == "8") ParseAndDecodeCodec8Data();
        }
      
        private void ParseAndDecodeCodec8Data()
        {
            var avlItems = new List<Data>();
            var gpsElementSubItems = new List<Data>();
            var ioElementSubItems = new List<Data>();

            avlItems.Add(_parser.ReadData(8, DataType.Timestamp));
            avlItems.Add(_parser.ReadData(1, DataType.Priority));
            gpsElementSubItems.Add(_parser.ReadData(4, DataType.Longitude));
            gpsElementSubItems.Add(_parser.ReadData(4, DataType.Latitude));
            gpsElementSubItems.Add(_parser.ReadData(2, DataType.Altitude));
            gpsElementSubItems.Add(_parser.ReadData(2, DataType.Angle));
            gpsElementSubItems.Add(_parser.ReadData(1, DataType.Satellites));
            gpsElementSubItems.Add(_parser.ReadData(2, DataType.Speed));
            ioElementSubItems.Add(_parser.ReadData(1, DataType.EventIoId));
            ioElementSubItems.Add(_parser.ReadData(1, DataType.IoCount));

            var ioCount1BData = _parser.ReadData(1, DataType.IoCount1B);
            ioElementSubItems.Add(ioCount1BData);
            for (var j = 0; j < Int32.Parse(ioCount1BData.Value); j++)
            {
                ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId1B));
                ioElementSubItems.Add(_parser.ReadData(1, DataType.IoValue1B));
            }

            var ioCount2BData = _parser.ReadData(1, DataType.IoCount2B);
            ioElementSubItems.Add(ioCount2BData);
            for (var j = 0; j < Int32.Parse(ioCount2BData.Value); j++)
            {
                ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId2B));
                ioElementSubItems.Add(_parser.ReadData(2, DataType.IoValue2B));
            }

            var ioCount4BData = _parser.ReadData(1, DataType.IoCount4B);
            ioElementSubItems.Add(ioCount4BData);
            for (var j = 0; j < Int32.Parse(ioCount4BData.Value); j++)
            {
                ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId4B));
                ioElementSubItems.Add(_parser.ReadData(4, DataType.IoValue4B));
            }

            var ioCount8BData = _parser.ReadData(1, DataType.IoCount8B);
            ioElementSubItems.Add(ioCount8BData);
            for (var j = 0; j < Int32.Parse(ioCount8BData.Value); j++)
            {
                ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId8B));
                ioElementSubItems.Add(_parser.ReadData(8, DataType.IoValue8B));
            }
            AvlItems = avlItems;
            GpsElementSubItems = gpsElementSubItems;
            IoElementSubItems = ioElementSubItems;
        }
        private void ParseAndDecodeCodec7Data()
        {
            var avlItems = new List<Data>();
            var gpsElementSubItems = new List<Data>();
            var ioElementSubItems = new List<Data>();

            var priorityAndTimeStampSegment = _parser.ReadArraySeqment(4);
            
         
            var timestamp = new Data(DataType.TimestampGh, priorityAndTimeStampSegment, ValueConverter.GetStringValue(priorityAndTimeStampSegment, DataType.TimestampGh));
            var priority = new Data(DataType.PriorityGh, priorityAndTimeStampSegment, ValueConverter.GetStringValue(priorityAndTimeStampSegment, DataType.PriorityGh));
            avlItems.Add(timestamp);
            avlItems.Add(priority);

            var globalMaskByteSegment = _parser.ReadArraySeqment(1);
            var globalmask = new Data(DataType.GlobalMask, globalMaskByteSegment, ValueConverter.GetStringValue(globalMaskByteSegment, DataType.GlobalMask));
            avlItems.Add(globalmask);

            // gps Element
            if (globalmask.Value.Contains(GlobalMaskCodec7.GpsElement.ToString()))
            {
                var gpsElementMaskSegment = _parser.ReadArraySeqment(1);
                var gpsElementMask = new Data(DataType.GpsElementMask, gpsElementMaskSegment, ValueConverter.GetStringValue(gpsElementMaskSegment, DataType.GpsElementMask));
                gpsElementSubItems.Add(gpsElementMask);

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Coordinates.ToString()))
                {
                    gpsElementSubItems.Add(_parser.ReadData(4, DataType.LatitudeGh));
                    gpsElementSubItems.Add(_parser.ReadData(4, DataType.LongitudeGh));
                }

                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Altitude.ToString()))
                    gpsElementSubItems.Add(_parser.ReadData(2, DataType.Altitude));
                
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Angle.ToString()))
                    gpsElementSubItems.Add(_parser.ReadData(1, DataType.AngleGh));
                
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Speed.ToString()))
                    gpsElementSubItems.Add(_parser.ReadData(1, DataType.SpeedGh));
                
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.Satellites.ToString()))
                    gpsElementSubItems.Add(_parser.ReadData(1, DataType.Satellites));
                
                // gps Io Elements
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.CellId.ToString()))
                    ioElementSubItems.Add(_parser.ReadData(4, DataType.CellIdAndLocalAreaGh));
                
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.SignalQuality.ToString()))
                    ioElementSubItems.Add(_parser.ReadData(1, DataType.SignalQualityGh));
                
                if (gpsElementMask.Value.Contains(GpsElementMaskCodec7.OperatorCode.ToString()))
                    ioElementSubItems.Add(_parser.ReadData(4, DataType.OperatorCodeGh));
                
            }

            // io Elements
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt8.ToString()))
            {
                var ioCount1BData = _parser.ReadData(1, DataType.IoCount1B);
                ioElementSubItems.Add(ioCount1BData);
                for (var j = 0; j < Int32.Parse(ioCount1BData.Value); j++)
                {
                    ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId1B));
                    ioElementSubItems.Add(_parser.ReadData(1, DataType.IoValue1B));
                }

            }
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt16.ToString()))
            {
                var ioCount2BData = _parser.ReadData(1, DataType.IoCount2B);
                ioElementSubItems.Add(ioCount2BData);
                for (var j = 0; j < Int32.Parse(ioCount2BData.Value); j++)
                {
                    ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId2B));
                    ioElementSubItems.Add(_parser.ReadData(2, DataType.IoValue2B));
                }
            }
            if (globalmask.Value.Contains(GlobalMaskCodec7.IoInt32.ToString()))
            {
                var ioCount4BData = _parser.ReadData(1, DataType.IoCount4B);
                ioElementSubItems.Add(ioCount4BData);
                for (var j = 0; j < Int32.Parse(ioCount4BData.Value); j++)
                {
                    ioElementSubItems.Add(_parser.ReadData(1, DataType.IoId4B));
                    ioElementSubItems.Add(_parser.ReadData(4, DataType.IoValue4B));
                }
            }
            AvlItems = avlItems;
            GpsElementSubItems = gpsElementSubItems;
            IoElementSubItems = ioElementSubItems;
        }
    }
}
