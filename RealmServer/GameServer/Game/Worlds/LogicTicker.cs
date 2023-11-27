using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Worlds
{
    public class LogicTicker
    {
        private static readonly Logger _log = new Logger(typeof(LogicTicker));

        public RealmTime Time;

        private readonly World _world;

        public LogicTicker(World world)
        {
            _world = world;
        }

        public void Run(int mspt)
        {
            var lagMs = (int)(mspt * 1.5);
            var sw = Stopwatch.StartNew();
            while (true)
            {
                _world.Update();

                // This approach uses more CPU power but it's much more accurate than others (e.g Thread.Sleep, ManualResetEvent)
                if (sw.ElapsedMilliseconds < mspt)
                    continue;

                Time.ElapsedMsDelta = (int)sw.ElapsedMilliseconds;
                Time.TotalElapsedMs += sw.ElapsedMilliseconds;
                Time.TickCount++;

                if (Time.ElapsedMsDelta >= lagMs)
                    _log.Warn($"{_world.Name}({_world.Id}) LAGGED | MsPT: {mspt} Elapsed: {Time.ElapsedMsDelta}");

                sw.Restart();

                _world.Tick(Time);
            }
        }
    }
}
