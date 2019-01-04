using System;

namespace Teltonika.DataParser.Client
{
    public class DataReader
    {
        private readonly byte[] _data;
        private int _offset;

        public DataReader(byte[] data)
        {
            _data = data;
            _offset = 0;
        }
        public Data ReadData(byte size, DataType dataType)
        {
            var arraySegment = new ArraySegment<byte>(_data, _offset, size);

            _offset += size;
            return new Data(dataType, arraySegment, ValueConverter.GetStringValue(arraySegment, dataType));
        }
        public ArraySegment<byte> ReadArraySeqment(byte size)
        {
            _offset += size;
            return new ArraySegment<byte>(_data, _offset - size, size);
        }
    }
}
