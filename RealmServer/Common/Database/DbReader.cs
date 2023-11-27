using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Database
{
    public class DbReader
    {
        public int[] ReadIntArray(byte[] value)
        {
            if (value == null)
                return null;

            var ret = new int[value.Length / sizeof(int)];
            Buffer.BlockCopy(value, 0, ret, 0, value.Length);
            return ret;
        }

        public ushort[] ReadUInt16Array(byte[] value)
        {
            if (value == null)
                return null;

            var ret = new ushort[value.Length / sizeof(ushort)];
            Buffer.BlockCopy(value, 0, ret, 0, value.Length);
            return ret;
        }
    }
}
