namespace Teltonika.Codec
{
    public sealed class CRC
    {
        private readonly int _polynom;

        public static readonly CRC Default = new CRC(0xA001);

        public CRC(int polynom)
        {
            _polynom = polynom;
        }

        public int CalcCrc16(byte[] buffer)
        {
            return CalcCrc16(buffer, 0, buffer.Length, _polynom, 0);
        }

        public static int CalcCrc16(byte[] buffer, int offset, int bufLen, int polynom, int preset)
        {
            preset &= 0xFFFF;
            polynom &= 0xFFFF;

            var crc = preset;
            for (var i = 0; i < bufLen; i++)
            {
                var data = buffer[(i + offset) % buffer.Length] & 0xFF;
                crc ^= data;
                for (var j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc = (crc >> 1) ^ polynom;
                    }
                    else
                    {
                        crc = crc >> 1;
                    }
                }
            }
            return crc & 0xFFFF;
        }
    }
}
