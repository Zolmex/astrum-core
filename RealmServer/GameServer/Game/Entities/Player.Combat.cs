using Common;
using Common.Resources.Xml;
using GameServer.Game.Collections;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Incoming;
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
        public void Shoot(ProjectileDesc projectile, byte numShots, float angle)
        {
            for (var i = 0; i < numShots; i++)
            {
                var projState = ProjectileState.GetProjectile();
                projState.SetTTL(projectile.LifetimeMS);
                _projectiles.Add(projState);

                var dmg = (uint)_rand.Next(projectile.MinDamage, projectile.MaxDamage);
                World.BroadcastAll(plr =>
                {
                    if (plr.DistSqr(this) <= SIGHT_RADIUS_SQR && plr.Id != Id)
                    {
                        plr.User.SendPacket(PacketId.ALLYSHOOT, AllyShoot.Write(plr.User,
                            Id,
                            projectile.ContainerType,
                            angle
                            ));
                    }
                });
            }
        }
    }
}
