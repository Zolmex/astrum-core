using Common;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var dx = a.Position.X - x;
            var dy = a.Position.Y - y;
            return dx * dx + dy * dy;
        }

        public static float DistSqr(this Entity a, Entity b)
        {
            var dx = a.Position.X - b.Position.X;
            var dy = a.Position.Y - b.Position.Y;
            return dx * dx + dy * dy;
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

        //public static IEnumerable<Entity> GetNearestEntities(this Entity entity, float radius, bool byTile = false)
        //    => entity.World.ChunkManager.HitTest(entity.X, entity.Y, radius)
        //        .Where(e => e.Dist(entity, byTile) < radius);

        public static IEnumerable<KeyValuePair<int, Player>> GetNearbyPlayers(this Entity entity, float radiusSqr)
            => entity.World.Players.Where(kvp => kvp.Value.TileDistSqr(entity) < radiusSqr);

        //public static Entity GetNearestEntity(this Entity entity, float radius, bool byTile = false)
        //{
        //    var ret = GetNearestEntities(entity, radius, byTile);
        //    ret = ret.OrderBy(e => entity.Dist(e, byTile));
        //    return ret.FirstOrDefault();
        //}

        public static Player GetNearbyPlayer(this Entity entity, float radius)
        {
            var ret = GetNearbyPlayers(entity, radius);
            ret = ret.OrderBy(kvp => entity.DistSqr(kvp.Value));
            return ret.FirstOrDefault().Value;
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
