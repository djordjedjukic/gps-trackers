using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Teltonika.DataParser.Client
{
    public class Data
    {
        public Data(string name)
        {
            _name = name;
        }

        public Data(DataType dataType, ArraySegment<byte> arraySegment, string value)
        {
            DataType = dataType;
            Value = value;
            if (dataType != DataType.PriorityGh)
            {
                ArraySegment = arraySegment;
            }
        }
        public Data[] SubItems { get; set; }
        public DataType DataType { get; set; }
        public ArraySegment<byte> ArraySegment { get; set; }
        public string Value { get; set; }
        public string HexValue
        {
            get
            {
                if (_name == null)
                {
                    return ArraySegment.Array != null
                        ? BitConverter.ToString(ArraySegment.Array.Skip(ArraySegment.Offset).Take(ArraySegment.Count).ToArray())
                        : null;
                }
                return null;
            }
        }

        public string Size
        {
            get
            {
                if (_name == "GPS Element")
                {
                    return ArraySegment.Count.ToString(CultureInfo.InvariantCulture);
                }
                if (ArraySegment.Count == 0 || _name != null)
                {
                    return "var";
                }

                return ArraySegment.Count.ToString(CultureInfo.InvariantCulture);
            }
        }

        private readonly string _name;
        public string Name
        {
            get
            {
                return _name ?? GetDataTypeDisplayName();
            }
        }

        private string GetDataTypeDisplayName()
        {
            var type = DataType.GetType();

            var members = type.GetMember(DataType.ToString());
            if (members.Length == 0) return DataType.ToString();

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes.Length == 0) return DataType.ToString();

            var attribute = (DisplayAttribute)attributes[0];
            return attribute.GetName();
        }
    }
}