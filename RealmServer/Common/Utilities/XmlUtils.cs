using System;
using System.Globalization;
using System.Xml.Linq;

namespace Common.Utilities
{
    public static class XmlUtils
    {
        private static readonly Logger Log = new Logger(typeof(XmlUtils));

        public static T GetValue<T>(this XElement elem, string name, T def = default)
        {
            if (elem.Element(name) == null)
                return def;

            string value = elem.Element(name).Value;
            Type type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(value, type);
            if (type == typeof(int))
                return (T)Convert.ChangeType(int.Parse(value), type);
            if (type == typeof(ushort))
                return (T)Convert.ChangeType(Convert.ToUInt16(value, 16), type);
            if (type == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(value, 16), type);
            if (type == typeof(double))
                return (T)Convert.ChangeType(double.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(float))
                return (T)Convert.ChangeType(float.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(value) || bool.Parse(value), type);

            Log.Error($"Type of {type} is not supported by this method, returning default value: {def}...");
            return def;
        }

        public static T GetAttribute<T>(this XElement elem, string name, T def = default)
        {
            if (elem.Attribute(name) == null)
                return def;

            string value = elem.Attribute(name).Value;
            Type type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(value, type);
            if (type == typeof(int))
                return (T)Convert.ChangeType(int.Parse(value), type);
            if (type == typeof(ushort))
                return (T)Convert.ChangeType(Convert.ToUInt16(value, 16), type);
            if (type == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(value, 16), type);
            if (type == typeof(double))
                return (T)Convert.ChangeType(double.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(float))
                return (T)Convert.ChangeType(float.Parse(value, CultureInfo.InvariantCulture), type);
            if (type == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(value) || bool.Parse(value), type);

            Log.Error($"Type of {type} is not supported by this method, returning default value: {def}...");
            return def;
        }

        public static bool HasElement(this XElement e, string name)
            => e.Element(name) != null;

        public static bool HasAttribute(this XElement e, string name)
            => e.Attribute(name) != null;
    }
}
