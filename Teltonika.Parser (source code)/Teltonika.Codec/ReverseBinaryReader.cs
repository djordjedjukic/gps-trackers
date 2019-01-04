using System.IO;

namespace Teltonika.Codec
{
    public class ReverseBinaryReader : BinaryReader
    {
        public ReverseBinaryReader(Stream input) : base(input)
        {
        }

        public override ushort ReadUInt16()
        {
            return BytesSwapper.Swap(base.ReadUInt16());
        }

        public override short ReadInt16()
        {
            return BytesSwapper.Swap(base.ReadInt16());
        }

        public override int ReadInt32()
        {
            return BytesSwapper.Swap(base.ReadInt32());
        }

        public override uint ReadUInt32()
        {
            return BytesSwapper.Swap(base.ReadUInt32());
        }

        public override long ReadInt64()
        {
            return BytesSwapper.Swap(base.ReadInt64());
        }

        public override ulong ReadUInt64()
        {
            return BytesSwapper.Swap(base.ReadUInt64());
        }
    }

}
