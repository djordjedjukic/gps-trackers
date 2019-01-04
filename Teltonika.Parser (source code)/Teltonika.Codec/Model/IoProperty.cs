namespace Teltonika.Codec.Model
{
    public struct IoProperty
    {
        public int Id { get; private set; }
        public long Value { get; private set; }

        /// <summary>
        /// Creates IoProperty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IoProperty Create(int id, long value)
        {
            return new IoProperty
            {
                Id = id,
                Value = value
            };
        }
    }
}
