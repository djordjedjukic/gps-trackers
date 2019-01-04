using System;

namespace Teltonika.Codec.Model
{
    public struct AvlData
    {
        public string Priority { get; private set; }
        public DateTime DateTime { get; private set; }
        public GpsElement GpsElement { get; private set; }
        public IoElement IoElement { get; private set; }

        public static AvlData Create(string priority, DateTime dateTime, GpsElement gpsElement, IoElement ioElement)
        {
            return new AvlData
            {
                Priority = priority,
                DateTime = dateTime,
                GpsElement = gpsElement,
                IoElement = ioElement
            };
        }
    }
}
