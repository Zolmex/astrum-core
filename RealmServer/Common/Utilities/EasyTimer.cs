using System;
using System.Diagnostics;

namespace Common.Utilities
{
    public class EasyTimer : IDisposable
    {
        public static string Time = "[TIME]";
        private static readonly Logger Log = new Logger(typeof(EasyTimer));

        private readonly LogLevel _level;
        private readonly Stopwatch _sw;
        private readonly string _finalMessage;

        public EasyTimer(LogLevel level, string firstMessage = "Starting operation...", string finalMessage = "Operation took [TIME]")
        {
            if (firstMessage != null)
                Log.Log(level, firstMessage);
            _level = level;
            _sw = Stopwatch.StartNew();
            _finalMessage = finalMessage;
        }

        public void Dispose()
        {
            Log.Log(_level, _finalMessage.Replace(Time, _sw.Elapsed.TotalMilliseconds + " ms"));
        }
    }
}
