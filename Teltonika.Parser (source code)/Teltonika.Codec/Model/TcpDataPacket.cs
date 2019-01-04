namespace Teltonika.Codec.Model
{
    public class TcpDataPacket
    {
        public int Preamble { get; private set; }
        public int Length { get; private set; }
        public int Crc { get; private set; }
        public AvlDataCollection AvlData { get; private set; }

        public static TcpDataPacket Create(int preamble, int length, int crc, AvlDataCollection avlDataCollection)
        {
            return new TcpDataPacket
            {
                Preamble = preamble,
                Length = length,
                Crc = crc,
                AvlData = avlDataCollection
            };
        }
    }
}
