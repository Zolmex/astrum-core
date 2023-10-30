using System;
using System.Diagnostics;
using System.IO;

namespace Common.Utilities
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error,
        Fatal
    }

    public class Logger : ILogger
    {
        private const int PADDING = 18;

        private static readonly string CurrentDir = Directory.GetCurrentDirectory();
        private static readonly string LogDir = $"/logs/{Process.GetCurrentProcess().ProcessName}/";

        private static readonly object _consoleLock = new object();
        private readonly string _loggerName;

        public Logger(Type type)
            : this(type.Name) { }
        public Logger(string name)
            => _loggerName = name;

        static Logger()
        {
            // Create directories for the log files if they don't exist
            foreach (var level in Enum.GetValues(typeof(LogLevel)))
            {
                string path = $"{CurrentDir}{LogDir}{level.ToString().ToLower()}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static void Info(object obj, string loggerName = "Logger")
            => Log(obj.ToString(), LogLevel.Info, false, loggerName);

        public void Info(object obj, bool saveToFile = true)
            => Log(obj.ToString(), LogLevel.Info, saveToFile, _loggerName);

        public static void Debug(object obj, string loggerName = "Logger")
            => Log(obj.ToString(), LogLevel.Debug, false, loggerName);

        public void Debug(object obj, bool saveToFile = false)
            => Log(obj.ToString(), LogLevel.Debug, saveToFile, _loggerName);

        public static void Warn(object obj, string loggerName = "Logger")
            => Log(obj.ToString(), LogLevel.Warn, false, loggerName);

        public void Warn(object obj, bool saveToFile = true)
            => Log(obj.ToString(), LogLevel.Warn, saveToFile, _loggerName);

        public static void Error(object obj, string loggerName = "Logger")
            => Log(obj.ToString(), LogLevel.Error, false, loggerName);

        public void Error(object obj, bool saveToFile = true)
            => Log(obj.ToString(), LogLevel.Error, saveToFile, _loggerName);

        public static void Fatal(object obj, string loggerName = "Logger")
            => Log(obj.ToString(), LogLevel.Fatal, false, loggerName);

        public void Fatal(object obj, bool saveToFile = true)
            => Log(obj.ToString(), LogLevel.Fatal, saveToFile, _loggerName);

        public void Log(LogLevel level, object obj, bool saveToFile = false)
            => Log(obj.ToString(), level, saveToFile, _loggerName);

        private static void Log(string text, LogLevel level, bool saveToFile, string loggerName)
        {
#if RELEASE
            if (level == LogLevel.Debug)
                return;
#endif
            string lvl = level.ToString().ToUpper();
            int lvlPad = lvl.Length + (7 - lvl.Length);
            int senderPad = loggerName.Length + (PADDING - loggerName.Length);

            text = $"{DateTime.Now.TimeOfDay}  {lvl.PadRight(lvlPad) + loggerName.PadRight(senderPad) + text}";

            lock (_consoleLock)
            {
                Console.BackgroundColor = GetBackColor(level);
                Console.ForegroundColor = GetForeColor(level);
                Console.WriteLine(text);
            }

            try
            {
                if (saveToFile)
                {
                    string path = $"{CurrentDir}{LogDir}{level.ToString().ToLower()}/log.txt";
                    File.AppendAllLines(path, new string[] { text });
                }
            }
            catch (IOException e) { } // uhhh, leave this here ok?
        }

        private static ConsoleColor GetBackColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                case LogLevel.Debug:
                case LogLevel.Warn:
                case LogLevel.Fatal:
                    return ConsoleColor.Black;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default:
                    throw new ArgumentException($"Invalid LogLevel '{level}'");
            }
        }

        private static ConsoleColor GetForeColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.DarkGray;
                case LogLevel.Warn:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.White;
                case LogLevel.Fatal:
                    return ConsoleColor.White;
                default:
                    throw new ArgumentException($"Invalid LogLevel '{level}'");
            }
        }
    }
}
