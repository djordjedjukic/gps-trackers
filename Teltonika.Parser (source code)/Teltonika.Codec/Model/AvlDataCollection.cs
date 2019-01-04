using System.Collections.Generic;

namespace Teltonika.Codec.Model
{
    public struct AvlDataCollection
    {
        public int CodecId { get; private set; }
        public int DataCount { get; private set; }
        public IEnumerable<AvlData> Data { get; private set; }

        public static AvlDataCollection Create(int codecId, int dataCount, IEnumerable<AvlData> data)
        {
            return new AvlDataCollection
            {
                CodecId = codecId,
                DataCount = dataCount,
                Data = data
            };
        }
    }
}