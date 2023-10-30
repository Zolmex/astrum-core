using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Utilities
{
    public static class EnumUtils
    {
        private static Dictionary<Enum, string> _descriptors = new Dictionary<Enum, string>();

        public static void Load()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (type.IsEnum) // Load enum descriptors
                {
                    Array enums = type.GetEnumValues();
                    if (enums != null)
                    {
                        foreach (Enum value in enums)
                        {
                            string name = Enum.GetName(type, value);
                            FieldInfo field = type.GetField(name);
                            if (field != null)
                            {
                                DescriptionAttribute attr =
                                       Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                                _descriptors.Add(value, attr?.Description);
                            }
                        }
                    }
                }
            }
        }

        public static string GetDescription(this Enum value)
        {
            if (!_descriptors.TryGetValue(value, out string ret))
            {
                throw new KeyNotFoundException($"Enumerable '{value}' does not have a descriptor.");
            }
            return ret;
        }

        public static T[] CommaToArray<T>(this string x, string delim = ",")
        {
            if (string.IsNullOrWhiteSpace(x))
                return new T[0];

            if (typeof(T) == typeof(string))
                return x.Split(delim).Select(_ => (T)(object)_.Trim()).ToArray();
            else
                return x.Split(delim).Select(_ => (T)(object)(T)Utils.FromPrefix<T>(_.Trim())).ToArray();
        }

        public static string ToCommaSepString<T>(this IEnumerable<T> src, string delim = ", ", bool brackets = false)
        {
            if (src == null)
                return "";

            StringBuilder ret = new StringBuilder();
            if (brackets)
                ret.Append("[");
            for (var i = 0; i < src.Count(); i++)
            {
                if (i != 0) ret.Append(delim);
                ret.Append(src.ElementAt(i)?.ToString() ?? null);
            }
            if (brackets)
                ret.Append("]");
            return ret.ToString();
        }

        public static T RandomElement<T>(this IEnumerable<T> source)
            => source.RandomElements(1).FirstOrDefault();

        public static IEnumerable<T> RandomElements<T>(this IEnumerable<T> source, int count)
            => source.Shuffle().Take(count);

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
            => source.OrderBy(x => Guid.NewGuid());

        public static int IndexOf<T>(this T[] array, T element)
            => Array.IndexOf(array, element);

        // Thread-safe .ToArray() method
        //public static T[] ToArray<T>(this IEnumerable<T> src, object lockObj)
        //{
        //    lock (lockObj)
        //        return src.ToArray();
        //}

        public static void Clear(this Array arr)
            => Array.Clear(arr, 0, arr.Length);
    }
}
