namespace Teltonika.Codec.Model
{
    public class UdpDataPacket
    {
        public short Length { get; private set; }
        public short PacketId { get; private set; }
        public byte PacketType { get; private set; }
        public byte AvlPacketId { get; private set; }
        public short ImeiLength { get; private set; }
        public byte[] Imei { get; private set; }
        public AvlDataCollection AvlData { get; private set; }

        public static UdpDataPacket Create(short length, short packetId, byte packetType, byte avlPacketId, short imeiLength, byte[] imei, AvlDataCollection avlData)
        {
            return new UdpDataPacket
            {
                Length = length,
                PacketId = packetId,
                PacketType = packetType,
                AvlPacketId = avlPacketId,
                ImeiLength = imeiLength,
                Imei = imei,
                AvlData = avlData
            };
        }
    }
}
