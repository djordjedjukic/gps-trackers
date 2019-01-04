using System;

namespace Teltonika.Codec.Model.GH
{
    [Flags]
    public enum GpsElementMaskCodec7 : byte
    {
        Coordinates = 1 << 0,
        Altitude = 1 << 1,
        Angle = 1 << 2,
        Speed = 1 << 3,
        Satellites = 1 << 4,
        CellId = 1 << 5,
        SignalQuality = 1 << 6,
        OperatorCode = 1 << 7,
    }
}
