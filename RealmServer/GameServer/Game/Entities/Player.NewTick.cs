using Common;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public partial class Player
    {
        private readonly object _entityStatLock = new object();
        private readonly Dictionary<int, ObjectStatusData> _entityStatUpdates = new Dictionary<int, ObjectStatusData>();

        private void SendNewTick()
        {
            lock (_entityStatLock)
            {
                NewTick.Write(User.Network, _entityStatUpdates);
                _entityStatUpdates.Clear();
            }
        }

        private void HandleEntityStatChanged(Entity en, StatType type, object value)
        {
            if (Dead)
                return;

            lock (_entityStatLock)
            {
                if (_entityStatUpdates.TryGetValue(en.Id, out var status))
                {
                    status.Update(en.Position, type, value);
                }
                else
                {
                    var objectStatusData = new ObjectStatusData()
                    {
                        ObjectId = en.Id,
                        Pos = en.Position,
                        Stats = new List<StatData>()
                    };

                    if (type != StatType.None)
                    {
                        var statData = new StatData();
                        statData.Type = type;
                        statData.SetValue(value);

                        objectStatusData.Stats.Add(statData);
                    }
                    _entityStatUpdates.TryAdd(en.Id, objectStatusData);
                }
            }
        }
    }
}
