using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Database
{
    public class DbWriter
    {
        public byte[] Write(int[] value)
        {
            if (value == null)
                return null;

            var ret = new byte[value.Length * sizeof(int)];
            Buffer.BlockCopy(value, 0, ret, 0, ret.Length);
            return ret;
        }
    }
}
