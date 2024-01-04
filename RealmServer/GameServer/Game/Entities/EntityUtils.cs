using Common;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public static class EntityUtils
    {
        private const float MinAttackMult = 0.5f;
        private const float MaxAttackMult = 2f;
        private const float MinAttackFreq = 0.0015f;
        private const float MaxAttackFreq = 0.008f;

        public static float DistSqr(this Entity a, float x, float y)
        {
            return DistSqr(a.Position.X, a.Position.Y, x, y);
        }

        public static float DistSqr(this Entity a, Entity b)
        {
            var dx = a.Position.X - b.Position.X;
            var dy = a.Position.Y - b.Position.Y;
            return dx * dx + dy * dy;
        }

        public static float DistSqr(WorldPosData pos1, WorldPosData pos2)
        {
            return (pos1.X - pos2.X) * (pos1.Y - pos2.Y);
        }
        public static float DistSqr(Vector2 a, Vector2 b)
        {
            return (a.X - b.X) * (a.Y - b.Y);
        }
        public static float DistSqr(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (y1 - y2);
        }

        public static double TileDistSqr(this Entity a, int tileX, int tileY)
        {
            var dx = (int)a.Position.X - tileX;
            var dy = (int)a.Position.Y - tileY;
            return dx * dx + dy * dy;
        }

        public static double TileDistSqr(this Entity a, Entity b)
        {
            var dx = (int)a.Position.X - (int)b.Position.X;
            var dy = (int)a.Position.Y - (int)b.Position.Y;
            return dx * dx + dy * dy;
        }

        public static Player GetNearestPlayer(this Entity entity, float radiusSqr)
        {
            Player ret = null;
            var center = entity.Tile?.Chunk;
            var minDist = 0f;
            for (var cY = center.CY - Player.ACTIVE_RADIUS; cY <= center.CY + Player.ACTIVE_RADIUS; cY++)
                for (var cX = center.CX - Player.ACTIVE_RADIUS; cX <= center.CX + Player.ACTIVE_RADIUS; cX++)
                {
                    var chunk = entity.World.Map.Chunks[cX, cY];
                    if (chunk != null)
                        lock (chunk.Players)
                            foreach (var plr in chunk.Players)
                            {
                                var distSqr = plr.DistSqr(entity);
                                if (minDist == 0 || distSqr < minDist)
                                {
                                    ret = plr as Player;
                                    minDist = distSqr;
                                }
                            }
                }
            return ret;
        }

        public static float GetSpeed(this Character entity, float speed)
            => /*entity.HasConditionEffect(ConditionEffectIndex.Slowed) ? (5.55f * speed + 0.74f) / 2 :*/ 5.55f * speed + 0.74f;

        //public static int GetDefenseDamage(this Character entity, int damage, int def, bool ignoreDef)
        //{
        //    if (entity.HasAnyConditionEffects(ConditionEffectIndex.Invulnerable, ConditionEffectIndex.Invincible))
        //        return 0;

        //    if (entity.HasConditionEffect(ConditionEffectIndex.ArmorBroken) || ignoreDef)
        //        return damage;

        //    // curse check

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Armored))
        //        def *= 2;

        //    var min = (int)(damage * 0.15);
        //    return Math.Max(damage - def, min);
        //}

        //public static int GetAttackDamage(this Character entity, int damage)
        //{
        //    if (entity.HasConditionEffect(ConditionEffectIndex.Weak))
        //        return (int)(damage * MinAttackMult);

        //    if (entity.HasConditionEffect(ConditionEffectIndex.Damaging))
        //        damage = (int)(damage * 1.5);

        //    return damage;
        //}

        //public static int GetAttackDamage(this Player player, int min, int max, bool isAbility)
        //{
        //    var damage = (int)player.User.Random.NextIntRange((uint)min, (uint)max);
        //    damage = GetAttackDamage(player, damage);
        //    var mult = isAbility ? 1 : MinAttackMult + player.Attack / 75.0 * (MaxAttackMult - MinAttackMult);
        //    return (int)(damage * mult);
        //}

        //public static float GetAttackFrequency(this Player player)
        //{
        //    if (player.HasConditionEffect(ConditionEffectIndex.Dazed))
        //        return MinAttackFreq;

        //    var ret = MinAttackFreq + player.Dexterity / 75 * (MaxAttackFreq - MinAttackFreq);
        //    if (player.HasConditionEffect(ConditionEffectIndex.Berserk))
        //        ret *= 1.5f;

        //    return ret;
        //}

        //public static int GetAttackPeriod(this Player player, float rof)
        //    => (int)(1 / player.GetAttackFrequency() * (1 / rof));
    }
}
