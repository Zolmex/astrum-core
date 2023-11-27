using Common.Utilities;
using GameServer.Game.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class ProjectileCollection : SmartCollection<ProjectileState>
    {
        private readonly Logger _log = new Logger(typeof(ProjectileCollection));

        private long _lastTime = RealmManager.GlobalTime.TotalElapsedMs;

        public override void Update()
        {
            base.Update();

            var timeDiff = (int)(RealmManager.GlobalTime.TotalElapsedMs - _lastTime);
            _lastTime = RealmManager.GlobalTime.TotalElapsedMs;

            foreach (var kvp in _dict)
            {
                var proj = kvp.Value;
                proj.TimeAlive += timeDiff;

                if (proj.TimeAlive >= proj.TTL)
                {
                    proj.Recycle();
                    Remove(proj.Id);
                }
            }
        }
    }
}
