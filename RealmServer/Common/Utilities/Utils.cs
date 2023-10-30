using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities
{
    public static class Utils
    {
        private static readonly SHA1Managed _sha1 = new SHA1Managed();

        public static string ToSHA1(this string value)
            => Convert.ToBase64String(_sha1.ComputeHash(Encoding.UTF8.GetBytes(value)));

        public static bool ContainsIgnoreCase(this string self, string val)
            => self.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) != -1;

        public static bool EqualsIgnoreCase(this string self, string val)
            => self.Equals(val, StringComparison.InvariantCultureIgnoreCase);

        public static T GetEnumValue<T>(string val)
            => val == null ? default : (T)Enum.Parse(typeof(T), val.Replace(" ", ""));

        // https://stackoverflow.com/a/6553303
        public static bool IsDefaultValue(Type type, object obj)
        {
            if (obj == null)
                return true;
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
                return false;

            object defaultValue = Activator.CreateInstance(type);
            return obj.Equals(defaultValue);
        }

        public static object FromPrefix<T>(string x)
        {
            if (typeof(T) == typeof(long))
                return x.StartsWith("0x") ? long.Parse(x.Substring(2), NumberStyles.HexNumber) : long.Parse(x);
            if (typeof(T) == typeof(ulong))
                return x.StartsWith("0x") ? ulong.Parse(x.Substring(2), NumberStyles.HexNumber) : ulong.Parse(x);
            if (typeof(T) == typeof(int))
                return x.StartsWith("0x") ? int.Parse(x.Substring(2), NumberStyles.HexNumber) : int.Parse(x);
            if (typeof(T) == typeof(uint))
                return x.StartsWith("0x") ? uint.Parse(x.Substring(2), NumberStyles.HexNumber) : uint.Parse(x);
            if (typeof(T) == typeof(short))
                return x.StartsWith("0x") ? short.Parse(x.Substring(2), NumberStyles.HexNumber) : short.Parse(x);
            if (typeof(T) == typeof(ushort))
                return x.StartsWith("0x") ? ushort.Parse(x.Substring(2), NumberStyles.HexNumber) : ushort.Parse(x);
            throw new ArgumentException($"Type {typeof(T)} is not supported.");
        }
    }
}
