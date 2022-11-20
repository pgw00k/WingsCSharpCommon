using System.Collections;
using System.Collections.Generic;

namespace Wings.Extension
{
    public static class Int64Extension
    {
        public static byte[] ToBytes(this long value)
        {

            byte[] bytes = new byte[8];
            int offset = 0;

            bytes[offset++] = (byte)value;
            bytes[offset++] = (byte)(value >> 8);
            bytes[offset++] = (byte)(value >> 0x10);
            bytes[offset++] = (byte)(value >> 0x18);
            bytes[offset++] = (byte)(value >> 0x20);
            bytes[offset++] = (byte)(value >> 40);
            bytes[offset++] = (byte)(value >> 0x30);
            bytes[offset] = (byte)(value >> 0x38);

            return bytes;
        }
    }
}
