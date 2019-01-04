using System;

namespace Teltonika.Codec.Model
{
    [Flags]
    public enum GlobalMaskCodec7 : byte
    {
        GpsElement = 1 << 0,
        IoInt8 = 1 << 1,
        IoInt16 = 1 << 2,
        IoInt32 = 1 << 3,
    }
}
