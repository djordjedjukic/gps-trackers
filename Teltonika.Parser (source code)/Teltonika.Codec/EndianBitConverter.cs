using System;
using System.Runtime.InteropServices;

namespace Teltonika.Codec
{
    public class EndianBitConverter
    {
        public static float ToSingle(byte[] value, int startIndex)
        {
            var int32FromBytes = BytesSwapper.Swap(BitConverter.ToInt32(value, startIndex));
            return new Int32SingleUnion(int32FromBytes).AsSingle;
        }

        public static float Int32ToSingle(int value)
        {
            return new Int32SingleUnion(value).AsSingle;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Int32SingleUnion
        {
            /// <summary>
            /// Int32 version of the value.
            /// </summary>
            [FieldOffset(0)]
            int i;
            /// <summary>
            /// Single version of the value.
            /// </summary>
            [FieldOffset(0)]
            float f;

            /// <summary>
            /// Creates an instance representing the given integer.
            /// </summary>
            /// <param name="i">The integer value of the new instance.</param>
            internal Int32SingleUnion(int i)
            {
                f = 0; // Just to keep the compiler happy
                this.i = i;
            }

            /// <summary>
            /// Returns the value of the instance as a floating point number.
            /// </summary>
            internal float AsSingle
            {
                get { return f; }
            }
        }

    }
}
