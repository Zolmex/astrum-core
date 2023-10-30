using Common;
using GameServer.Game.Logic.Worlds;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Entities
{
    public partial class Player
    {
        private readonly object _entityStatLock = new object();
        private readonly Dictionary<int, ObjectStatusData> _entityStatUpdates = new Dictionary<int, ObjectStatusData>();

        private void SendNewTick()
        {
            lock (_entityStatLock)
            {
                User.SendPacket(PacketId.NEWTICK, NewTick.Write(User, _entityStatUpdates));
                _entityStatUpdates.Clear();
            }
        }

        private void HandleEntityStatChanged(Entity en, StatType type, object value)
        {
            lock (_entityStatLock)
            {
                if (_entityStatUpdates.TryGetValue(en.Id, out _))
                {
                    _entityStatUpdates[en.Id].Update(en.Position, type, value);
                }
                else
                {
                    var objectStatusData = new ObjectStatusData()
                    {
                        ObjectId = en.Id,
                        Pos = en.Position,
                        Stats = new List<StatData>()
                    };
                    var statData = new StatData();
                    statData.Type = type;
                    statData.SetValue(value);

                    objectStatusData.Stats.Add(statData);
                    _entityStatUpdates.TryAdd(en.Id, objectStatusData);
                }
            }
        }
    }
}
