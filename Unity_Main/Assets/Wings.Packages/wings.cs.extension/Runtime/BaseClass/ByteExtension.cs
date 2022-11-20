using System;
using System.Collections;
using System.Collections.Generic;

namespace Wings.Extension
{
    public static class ByteExtension
    {
        public static long ToInt64(this byte[] instance, int offset = 0)
        {

            uint num =  (uint)(((instance[offset++] | (instance[offset++] << 8)) | (instance[offset++] << 0x10)) | (instance[offset++] << 0x18));
            uint num2 = (uint)(((instance[offset++] | (instance[offset++] << 8)) | (instance[offset++] << 0x10)) | (instance[offset] << 0x18));
            return (long)((num2 << 0x20) | num);           
        }
    }
}
